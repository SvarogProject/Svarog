using UnityEngine;


public class MasaAnimEvent : MonoBehaviour {
    public AudioClip AttackAudio;
    
    public void PlayAttackAudio() {
        AudioSource.PlayClipAtPoint(AttackAudio, FindObjectOfType<AudioListener>().transform.position, 100);
    }
}
