using UnityEngine;

public class EffectAutoDestroy : MonoBehaviour {
    public float Time;
    
    private void Update() {
        Destroy(gameObject, Time);
    }
}