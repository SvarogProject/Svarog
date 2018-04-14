using UnityEngine;
using System.Collections;
using System.Linq;

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
    public float RetreatOnAirTime;
    public float SpurtOnAirTime;

    public Rigidbody2D Rigidbody;
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;

    private float _actualSpeed;
    private float _endRunTimer; // 跑步小刹车倒计时
    private bool _isRetreatingOnAir;
    private bool _isSpurtingOnAir;

    private bool _usedFirestOnGroundInFirePunch;

    private float _gravityScale;

    private Vector2 _saveVelocity;   // 顿帧保存的力
    private bool _needResetVelocity; // 是否需要重设力  

    public void Start() {
        Rigidbody = GetComponent<Rigidbody2D>();
        _states = GetComponent<PlayerStateManager>();
        _animation = GetComponent<PlayerAnimationHandler>();
        Rigidbody.freezeRotation = true;
        _gravityScale = Rigidbody.gravityScale;
    }

    public void FixedUpdate() {
        if (_states.Stop) { // 顿帧中不能移动，保存一下力
            _saveVelocity += Rigidbody.velocity;
            Rigidbody.velocity = Vector2.zero; // 清空力
            Rigidbody.gravityScale = 0;
            _needResetVelocity = true;
        } else {
            if (_needResetVelocity) {
                Rigidbody.velocity = _saveVelocity;
                Rigidbody.gravityScale = _gravityScale;
                _needResetVelocity = false;
                _saveVelocity = Vector2.zero;
            }
            
            SpecialAttack();
            HorizontalMovement();
            Jump();
        }
    }

    private void SpecialAttack() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_FIRE_PUNCH)) {
            if (!_usedFirestOnGroundInFirePunch) {
                if (_states.OnGround) { // 起跳
                    
                    DoJump(true);
                    
                } else { // 空中
                    _usedFirestOnGroundInFirePunch = true;
                }
            }      
        } else {
            _usedFirestOnGroundInFirePunch = false;
        }
    }

    private void HorizontalMovement() {
        // 刹车
        RunEnding();
        var isRetreating = Retreat();
        var isSpurting = Spurt();

        if (!isRetreating && !isSpurting) {
            Walk();
        }
    }

    private bool Spurt() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_RUNNING)) {
            transform.Translate(Vector2.right * Time.deltaTime * RunSpeed);

            return true;
        }

        if (_animation.Animator.GetBool(AnimatorBool.IS_SPURTING) && _states.CanSpurtOrRetreatOnAir) {
            StartCoroutine(DoSpurtingOnAir());

            return true;
        }

        if (_isSpurtingOnAir) {
            transform.Translate(Vector2.right * Time.deltaTime * SpurtOnAirSpeed);

            return true;
        }

        return false;
    }

    private void Walk() {
        if (_states.JumpRight) {
            transform.Translate((_states.LookRight ? Vector2.right : Vector2.left) * Time.deltaTime * RunSpeed * 0.8f);

        } else if (_states.JumpLeft) {
            transform.Translate((_states.LookRight ? Vector2.left : Vector2.right) * Time.deltaTime * RunSpeed * 0.8f);

        } else if (_states.Right && _states.OnGround && !_animation.Animator.GetBool(AnimatorBool.CROUCH)) {
            transform.Translate((_states.LookRight ? Vector2.right : Vector2.left) * Time.deltaTime * WalkForwardSpeed);

        } else if (_states.Left && _states.OnGround  && !_animation.Animator.GetBool(AnimatorBool.CROUCH)) {
            transform.Translate((_states.LookRight ? Vector2.left : Vector2.right) * Time.deltaTime * WalkBackSpeed);

        }
    }

    private bool Retreat() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_RETREATING)) {
            _endRunTimer = 0; // 清除刹车的效果

            if (_states.OnGround) {
                
                transform.Translate(Vector2.left * Time.deltaTime * RetreatSpeed);

                return true;
            }

            if (_states.CanSpurtOrRetreatOnAir) {

                StartCoroutine(DoRetreatingOnAir());
            }
        }

        if (_isRetreatingOnAir) {
            transform.Translate(Vector2.left * Time.deltaTime * RetreatOnAirSpeed);

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
        if (_states.Jump && _states.OnGround) {
            
            if (_states.Right) {
                _states.JumpRight = true;
            } else if (_states.Left) {
                _states.JumpLeft = true;
            } else if (_endRunTimer > 0 && _states.LookRight) { // 除了正按着前进还有一种可能是跑步的刹车中起跳
                _states.JumpRight = true;
            } else if (_endRunTimer > 0 && !_states.LookRight) {
                _states.JumpLeft = true;
            }

            if (_states.JumpHigh) {
                //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, HighJumpSpeed);
                DoJump(false);
            } else {
                //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpSpeed);
                DoJump(true);
            }

            _states.ResetAttacks();

            // 初始化空中冲刺或者急退
            _states.CanSpurtOrRetreatOnAir = true;
        }

        if (_states.JumpDouble) { // 二段跳
            
            _states.ResetAttacks();
            //Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpSpeed);
            DoJump(true);
        }
    }

    public void AddVelocityOnCharacter(Vector3 direction, float timer) {
        StartCoroutine(AddVelocity(timer, direction));
    }

    private IEnumerator AddVelocity(float timer, Vector3 direction) {
        float t = 0;

        while (t < timer) {
            
            if (_states.Stop) {
                _saveVelocity += new Vector2(direction.x, direction.y);
            } else {
                t += Time.deltaTime;
                Rigidbody.velocity = direction;
            }

            yield return null;
        }
    }

    private void DoJump(bool normal) {
        if (_states.Stop) { // 顿帧中
            if (normal) {
                _saveVelocity += new Vector2(Rigidbody.velocity.x, JumpSpeed);
            } else {
                _saveVelocity += new Vector2(Rigidbody.velocity.x, HighJumpSpeed);
            }
            
        } else {
            if (normal) {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpSpeed);
            } else {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, HighJumpSpeed);
            }
        }
    }

    private IEnumerator DoSpurtingOnAir() {
        _states.CanSpurtOrRetreatOnAir = false;
        _isSpurtingOnAir = true;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;

        if (_states.LookRight) {
            _states.JumpRight = true;
            _states.JumpLeft = false;
        } else {
            _states.JumpLeft = true;
            _states.JumpRight = false;
        }

        yield return new WaitForSeconds(SpurtOnAirTime);

        _isSpurtingOnAir = false;
        Rigidbody.gravityScale = _gravityScale;
        Rigidbody.AddForce(new Vector2(0, -Rigidbody.gravityScale));

    }

    private IEnumerator DoRetreatingOnAir() {
        _states.CanSpurtOrRetreatOnAir = false;
        _isRetreatingOnAir = true;
        Rigidbody.velocity = Vector2.zero;
        Rigidbody.gravityScale = 0;

        if (_states.LookRight) {
            _states.JumpLeft = true;
            _states.JumpRight = false;
        } else {
            _states.JumpRight = true;
            _states.JumpLeft = false;
        }

        yield return new WaitForSeconds(RetreatOnAirTime);

        _isRetreatingOnAir = false;
        Rigidbody.gravityScale = _gravityScale;
        Rigidbody.AddForce(new Vector2(0, -Rigidbody.gravityScale));

    }
   
}