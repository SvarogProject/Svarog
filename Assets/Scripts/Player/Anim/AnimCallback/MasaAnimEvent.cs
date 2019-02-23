using UnityEngine;


public class MasaAnimEvent : MonoBehaviour {
    public AudioClip AttackAudio;
    /*
    private void Start() {
        GetComponent<SpriteRenderer>().material.shader = Shader.Find("Sprites/MasaShader");
    }*/

    public void PlayAttackAudio() {
        AudioSource.PlayClipAtPoint(AttackAudio, FindObjectOfType<AudioListener>().transform.position, 100);
    }
}
