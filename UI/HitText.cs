using UnityEngine;
using UnityEngine.UI;

public class HitText : MonoBehaviour {

	public int Hits;
	private const float ComboTime = 1f;
	private Text _text;
	private Animator _animator;
	private float _timer; // 倒计时清零

	private void Start () {
		_text = GetComponent<Text>();
		_animator = GetComponent<Animator>();
	}
	
	private void Update () {
		_timer -= Time.deltaTime;

		if (_timer < 0) {
			Hits = 0;
		}

		if (Hits == 1) {
			_text.text = "Hit";			
			_text.enabled = true;
		} else if (Hits >= 2) {
			_text.text = Hits + " Combo";
			_text.enabled = true;
		} else {
			_text.enabled = false;
		}
	}

	public void GetHit() {
		Hits++;
		_timer = ComboTime;
		_animator.Play("Show");
	}
}
