using UnityEngine;
using UnityEngine.Networking;

public class NetDefenseHandler : NetworkBehaviour {
    private NetPlayerStateManager _states;
    private BoxCollider2D _defenseCollider;
	
    public void Start() {
        _states = GetComponentInParent<NetPlayerStateManager>();
        _defenseCollider = GetComponent<BoxCollider2D>();
    }
	
    public void Update () {
        _defenseCollider.enabled = _states.LookRight && _states.DefenseLeft || !_states.LookRight && _states.DefenseRight;
    }
}