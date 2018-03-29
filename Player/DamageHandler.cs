using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DamageHandler : MonoBehaviour {
    public DamageType DamageType;
    
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;
    
    private List<Collider2D> _hurtColliders = new List<Collider2D>();
    private Collider2D _defenseCollider;

    private float _onTriggerTimer;
    private float _isDefensed;
    
    private bool _doDamage = true;

    public void Start() {
        _states = GetComponentInParent<PlayerStateManager>();
        _animation = GetComponentInParent<PlayerAnimationHandler>();
    }

    private void Update() {

        if (_onTriggerTimer <= 0) { // 一次碰撞完成，list中包含这次碰撞的所有碰撞体
            //foreach (var c in _hurtColliders) {
            //    Debug.Log(c.name);    
            //}

            if (_defenseCollider != null && _hurtColliders.Count > 0) {
                StartDamage(_defenseCollider);
            } else if (_hurtColliders.Count == 1) {
                StartDamage(_hurtColliders[0]);
            } else if (_hurtColliders.Count > 1) {
                var head = _hurtColliders.Find(c => c.name == "Head");
                var body = _hurtColliders.Find(c => c.name == "Body");
                var foot = _hurtColliders.Find(c => c.name == "Foot");
                
                // 规则：头>脚>身
                if (head) {
                    StartDamage(head);
                } else if (foot) {
                    StartDamage(foot);
                } else if (body) {
                    StartDamage(body);
                }
            }

            // 清空
            _defenseCollider = null;
            _hurtColliders.Clear();
        } else {
            _onTriggerTimer -= Time.deltaTime;
        }
    }

    public void OnTriggerEnter2D(Collider2D other) {
        
        //Debug.Log(other.name);
        
        if (!_doDamage) {
            return;
        }
        
        if (other.GetComponentInParent<PlayerStateManager>() && other.CompareTag("HurtCollider")) {
            var otherState = other.GetComponentInParent<PlayerStateManager>();
            if (otherState != _states) {                  
                _hurtColliders.Add(other);
            }        
        }

        if (other.GetComponentInParent<PlayerStateManager>() && other.CompareTag("DefenseCollider")) {
            
            var otherState = other.GetComponentInParent<PlayerStateManager>();

            if (otherState != _states) {

                if (!otherState.Attacks.Any(attack => attack.Attack)) { // 对方没有攻击才可以防御成功
                    if (_onTriggerTimer < 0) {                          // 一段时间能的首次碰撞
                        _onTriggerTimer = 0.01f;
                    }

                    if (_onTriggerTimer < 0) { // 一段时间能的首次碰撞
                        _onTriggerTimer = 0.01f;
                    }

                    _defenseCollider = other;
                }
            }
        }
    }

    private void StartDamage(Collider2D collider) {
        Debug.Log("Attack-Start:" + collider.name);
        
        var otherState = collider.GetComponentInParent<PlayerStateManager>();

            if (collider.CompareTag("DefenseCollider")) {
                otherState.TakeDamage(0.01f, DamageType.Defensed);
            } 
            else if (_animation.Animator.GetBool(AnimatorBool.IS_FIRE_PUNCH)) {
                otherState.TakeDamage(6, DamageType.FirePunch);
            } else if (_animation.Animator.GetBool(AnimatorBool.IS_HEAVY_ATTACK)) {
                otherState.TakeDamage(5, DamageType.Heavy);
            } else {
                otherState.TakeDamage(3, DamageType);
            }
        
            
        // 顿帧
        _animation.Animator.speed = 0f;
        //gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _doDamage = false;

        Invoke("AnimPlay", 0.1f);
    }

    private void AnimPlay() {
        //gameObject.GetComponent<BoxCollider2D>().enabled = true;
        _animation.Animator.speed = 1;
        _doDamage = true;
    }
}