using UnityEngine;
using System.Collections;

public class MovementHandler : MonoBehaviour {
    public float JumpSpeed;
    public float HighJumpSpeed;
    public float SpurtOnAirSpeed;
    public float RetreatOnAirSpeed;
    public float RunSpeed;
    public float WalkForwardSpeed;
    public float WalkBackSpeed;
    public float RetreatSpeed;
    public float EndRunTime; // 跑步的刹车时间

    private Rigidbody2D _rigidbody;
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;

    private float _actualSpeed;
    private float _endRunTimer; // 跑步小刹车倒计时

    public void Start() {
        _rigidbody = GetComponent<Rigidbody2D>();
        _states = GetComponent<PlayerStateManager>();
        _animation = GetComponent<PlayerAnimationHandler>();
        _rigidbody.freezeRotation = true;
    }

    public void FixedUpdate() {
        HorizontalMovement();
        Jump();
    }

    private void HorizontalMovement() {
        // 刹车
        RunEnding();
        var isRetreating = Retreat();
        var isSpurting = Spurt();

        if (!isRetreating && !isSpurting) {
            Walk();
        }

/*        if (_states.LookRight) { // 人物面朝右边

            if (_states.RightDouble) {
                if (_states.OnGround) {
                    transform.Translate(Vector2.right * Time.deltaTime * RunSpeed);
                } else {
                    // TODO 空中冲刺
                }
            } else if (_states.LeftDouble) {
                // TODO 回退
            }
        } else { // 人物面朝左边
            if (_states.LeftDouble) {
                if (_states.OnGround) {
                    transform.Translate(Vector2.right * Time.deltaTime * RunSpeed);
                } else {
                    // TODO 空中冲刺
                }
            } else if (_states.RightDouble) {
                // TODO 回退
            }
        }*/
    }

    private bool Spurt() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_RUNNING)) {
            if (_states.OnGround) {
                transform.Translate(Vector2.right * Time.deltaTime * RunSpeed);
            } else {
                // TODO 空中冲刺
            }
            return true;
        }

        return false;
    }

    private void Walk() {
        if (_states.JumpRight) {
            transform.Translate((_states.LookRight ? Vector2.right : Vector2.left) * Time.deltaTime * RunSpeed * 0.8f);

        } else if (_states.JumpLeft) {
            transform.Translate((_states.LookRight ? Vector2.left : Vector2.right) * Time.deltaTime * RunSpeed * 0.8f);

        } else if (_states.Right && _states.OnGround) {
            transform.Translate((_states.LookRight ? Vector2.right : Vector2.left) * Time.deltaTime * WalkForwardSpeed);

        } else if (_states.Left && _states.OnGround) {
            transform.Translate((_states.LookRight ? Vector2.left : Vector2.right) * Time.deltaTime * WalkBackSpeed);

        }
    }

    private bool Retreat() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_RETREATING)) {
            EndRunTime = 0; // 清除刹车的效果
            transform.Translate(Vector2.left * Time.deltaTime * RetreatSpeed);

            return true;
        }

        return false;
    }

    private void RunEnding() {
        // 如果是大刹车
        if (_animation.Animator.GetBool(AnimatorBool.IS_RUN_ENDING)) {
            transform.Translate(Vector2.right * Time.deltaTime * RunSpeed * 0.5f);
        }

        // 通过使用招数等的小刹车动作
        if (!_animation.Animator.GetBool(AnimatorBool.IS_RUNNING) && _endRunTimer > 0) {
            transform.Translate(Vector2.right * Time.deltaTime * RunSpeed * 0.5f);
            _endRunTimer -= Time.deltaTime;
        }

        // 如果正在跑就准备好小刹车的倒计时
        if (_animation.Animator.GetBool(AnimatorBool.IS_RUNNING)) {
            _endRunTimer = EndRunTime;
        }

    }

    private void Jump() {
        if (_states.Jump) {
            if (_states.OnGround) {
                if (_states.Right) {
                    _states.JumpRight = true;
                } else if (_states.Left) {
                    _states.JumpLeft = true;
                } else if (_endRunTimer > 0 && _states.LookRight) { // 除了正按着前进还有一种可能是跑步的刹车中起跳
                    _states.JumpRight = true;
                } else if (_endRunTimer > 0 && !_states.LookRight) {
                    _states.JumpLeft = true;
                }

                _animation.JumpAnim();
                _states.ResetAttacks();

                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpSpeed);
            }
        }
    }

    public void AddVelocityOnCharacter(Vector3 direction, float timer) {
        StartCoroutine(AddVelocity(timer, direction));
    }

    private IEnumerator AddVelocity(float timer, Vector3 direction) {
        float t = 0;

        while (t < timer) {
            t += Time.deltaTime;

            _rigidbody.velocity = direction;

            yield return null;
        }
    }
}