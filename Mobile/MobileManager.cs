using UnityEngine;

public class MobileManager : MonoBehaviour {
    public static bool IsMobile;

    private void Start() {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer) {
            IsMobile = true;
        } else {
            IsMobile = false;
        }
    }
}