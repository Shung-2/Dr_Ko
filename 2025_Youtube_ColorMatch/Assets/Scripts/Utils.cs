using UnityEngine.InputSystem;

public static class Utils
{
    public static bool IsAnyInputDown()
    {
        // 키보드 키 입력
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }
        
        // 마우스 버튼 입력 (좌/우/휠)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame)
            {
                return true;
            }
        }
        
        // 모바일 터치
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            return true;
        }
        
        return false;
    }
}
