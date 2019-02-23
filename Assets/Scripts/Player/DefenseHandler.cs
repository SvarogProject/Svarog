using UnityEngine;
using System.Collections;

public class DefenseHandler : MonoBehaviour {

	private PlayerStateManager _states;
	private BoxCollider2D _defenseCollider;
	
	public void Start() {
		_states = GetComponentInParent<PlayerStateManager>();
		_defenseCollider = GetComponent<BoxCollider2D>();
	}
	
	public void Update () {
		_defenseCollider.enabled = _states.LookRight && _states.DefenseLeft || !_states.LookRight && _states.DefenseRight;
	}
}
