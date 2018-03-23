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
    public float RetreatOnAirTime;
    public float SpurtOnAirTime;

    public Rigidbody2D Rigidbody;
    private PlayerStateManager _states;
    private PlayerAnimationHandler _animation;

    private float _actualSpeed;
    private float _endRunTimer; // 跑步小刹车倒计时
    private bool _isRetreatingOnAir;
    private bool _isSpurtingOnAir;

    private float _gravityScale;

    public void Start() {
        Rigidbody = GetComponent<Rigidbody2D>();
        _states = GetComponent<PlayerStateManager>();
        _animation = GetComponent<PlayerAnimationHandler>();
        Rigidbody.freezeRotation = true;
        _gravityScale = Rigidbody.gravityScale;
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

        } else if (_states.Right && _states.OnGround) {
            transform.Translate((_states.LookRight ? Vector2.right : Vector2.left) * Time.deltaTime * WalkForwardSpeed);

        } else if (_states.Left && _states.OnGround) {
            transform.Translate((_states.LookRight ? Vector2.left : Vector2.right) * Time.deltaTime * WalkBackSpeed);

        }
    }

    private bool Retreat() {
        if (_animation.Animator.GetBool(AnimatorBool.IS_RETREATING)) {
            EndRunTime = 0; // 清除刹车的效果

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
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, HighJumpSpeed);
            } else {
                Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpSpeed);
            }

            _states.ResetAttacks();

            // 初始化空中冲刺或者急退
            _states.CanSpurtOrRetreatOnAir = true;
        }

        if (_states.JumpDouble) { // 二段跳
            
            _states.ResetAttacks();
            Rigidbody.velocity = new Vector3(Rigidbody.velocity.x, JumpSpeed);
        }
    }

    public void AddVelocityOnCharacter(Vector3 direction, float timer) {
        StartCoroutine(AddVelocity(timer, direction));
    }

    private IEnumerator AddVelocity(float timer, Vector3 direction) {
        float t = 0;

        while (t < timer) {
            t += Time.deltaTime;

            Rigidbody.velocity = direction;

            yield return null;
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