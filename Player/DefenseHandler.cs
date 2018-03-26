using UnityEngine;
using System.Collections;

public class DefenseHandler : MonoBehaviour {

	private InputHandler _input;
	private PlayerStateManager _states;
	private BoxCollider2D _defenseCollider;
	
	public void Start() {
		_input = GetComponentInParent<InputHandler>();
		_states = GetComponentInParent<PlayerStateManager>();
		_defenseCollider = GetComponent<BoxCollider2D>();
	}
	
	public void Update () {
		// TODO 写到input中去
		var left = Input.GetButton("Left" + _input.PlayerInputId);
		var right = Input.GetButton("Right" + _input.PlayerInputId);

		_defenseCollider.enabled = _states.LookRight && left || !_states.LookRight && right;
	}
}
