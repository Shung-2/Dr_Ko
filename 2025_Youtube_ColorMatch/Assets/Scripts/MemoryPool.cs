using System.Collections.Generic;
using UnityEngine;

public class MemoryPool
{
    // 메모리 풀로 관리되는 오브젝트 정보
    private class PoolItem
    {
        public GameObject gameObject; // 화면에 보이는 실제 게임오브젝트
        private bool isActive; // "gameObject"의 활성화/비활성화 정보

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                gameObject.SetActive(value);
            }
        }
    }

    private readonly GameObject poolObject;         // 오브젝트 풀링에서 관리하는 게임 오브젝트 프리팹
    private readonly Transform parent;              // UI Canvas 등과 같이 부모 오브젝트가 필요할 때 사용
    private readonly int increaseCount;             // 오브젝트가 부족할 때 Instantiate()로 추가 생성되는 오브젝트 개수

    private readonly List<PoolItem> poolItemList;   // 관리되는 모든 오브젝트를 저장하는 리스트
    
    public int MaxCount { private get; set; }       // 현재 리스트에 등록되어 있는 오브젝트 개수
    public int ActiveCount {private  get; set;}     // 현재 게임에 사용되고 있는(활성화) 오브젝트 개수
    
    // 오브젝트가 임시로 보관되는 위치 (적당히 먼 위치)
    private readonly Vector3 tempPosition = new Vector3(-100f, -100f, -100f);

    public MemoryPool(GameObject poolObject, int increaseCount = 5, Transform parent = null)
    {
        this.poolObject = poolObject;
        this.parent = parent;
        this.increaseCount = Mathf.Max(1, increaseCount);
        poolItemList = new List<PoolItem>();
        MaxCount = 0;
        ActiveCount = 0;

        InstaniateObjects();
    }

    /// <summary>
    /// increaseCount 단위로 오브젝트를 생성
    /// </summary>
    public void InstaniateObjects()
    {
        MaxCount += increaseCount;
        for (int i = 0; i < increaseCount; ++i)
        {
            var go = GameObject.Instantiate(poolObject, parent);
            go.transform.position = tempPosition;
            
            var poolItem = new PoolItem { gameObject = go, IsActive = false };
            poolItemList.Add(poolItem);
        }
    }

    /// <summary>
    /// 현재 관리중인(활성/비활성) 모든 오브젝트를 삭제
    /// </summary>
    public void DestroyObjects()
    {
        for (int i = 0; i < poolItemList.Count; ++i)
        {
            if (poolItemList[i].gameObject)
            {
                GameObject.Destroy(poolItemList[i].gameObject);
            }
        }
        
        poolItemList.Clear();
        MaxCount = 0;
        ActiveCount = 0;
    }

    /// <summary>
    /// PoolItemList에 저장되어 있는 오브젝트를 활성화해서 사용
    /// 현재 모든 오브젝트가 사용중이라면, InstantiateObjects()로 추가 구성 
    /// </summary>
    public GameObject ActivatePoolItem(Vector3 position)
    {
        // 현재 생성해서 관리하는 모든 오브젝트 개수와 현재 활성화 상태인 오브젝트 개수 비교
        // 모든 오브젝트가 활성화 상태이면 새로운 오브젝트 필요
        if (MaxCount == ActiveCount)
        {
            InstaniateObjects();
        }

        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
            if (!poolItem.IsActive && poolItem.gameObject != null)
            {
                ActiveCount++;
                poolItem.gameObject.transform.position = position;
                poolItem.IsActive = true;
                return poolItem.gameObject;
            }
        }

        return null;
    }

    /// <summary>
    /// 현재 사용이 완료된 오브젝트를 비활성화 상태로 설정 
    /// </summary>
    public void DeactivatePoolItem(GameObject removeObject)
    {
        if (removeObject == null)
            return;
        
        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
            if (poolItem.gameObject == removeObject)
            {
                ActiveCount--;
                removeObject.transform.position = tempPosition;
                poolItem.IsActive = false;
                return;
            }
        }
    }

    public void DeactivateAllPoolItems()
    {
        for (int i = 0; i < poolItemList.Count; ++i)
        {
            PoolItem poolItem = poolItemList[i];
            if (poolItem.gameObject != null && poolItem.IsActive)
            {
                poolItem.gameObject.transform.position = tempPosition;
                poolItem.IsActive = false;
            }
        }
        
        ActiveCount = 0;
    }
}
