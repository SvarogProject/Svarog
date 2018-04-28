using UnityEngine;

public class MobileManager : MonoBehaviour {
    public static bool IsMobile;

    private void Start() {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer) {
            IsMobile = true;
            // Android不灭屏
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        } else {
            IsMobile = false;
        }
    }
}