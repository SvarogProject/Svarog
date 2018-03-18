using System;
using System.CodeDom;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public const float SMOKE_OFFSET_Y = 3.08f;

    // TODO 加把动作锁，在执行某动作的时候无法执行其他动作（攻击）

    public GameObject Charactor;
    public GameObject SmokeStartNormalJump;
    public GameObject SmokeEndJump;
    public float JumpSpeed;
    public float HighJumpSpeed;
    public float SpurtOnAirSpeed;
    public float RetreatOnAirSpeed;
    public float RunSpeed;
    public float WalkForwardSpeed;
    public float WalkBackSpeed;
    public float RetreatSpeed;

    private Animator _animator;
    private Rigidbody2D _rigidbody;

    private float _beforeJumpY;

    private float _highJumpTimer;

    private DoubleClick _retreatDouble = new DoubleClick();
    private DoubleClick _retreatOnAirDouble = new DoubleClick();
    private DoubleClick _spurtOnAirDouble = new DoubleClick();

    private PlayerState _playerState = new PlayerState();

/*    
    void Awake() {
        Application.targetFrameRate = 24;
    }
*/
    void Start() {
        _animator = Charactor.GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        // Debug.Log(Time.time + "时的位置：" + transform.position + "，速度：" + _rigidbody.velocity);
        Init();
        Crouch();
        Attack();

        // TODO 测试防御
        if (Input.GetKey(KeyCode.P)) {
            Defense(0f);
            // _isMoveable = false; // 否则这一帧还是会有少许移动
        }

        if (_playerState.IsMoveable) {
            Walk();
            Jump();
        }

        if (_playerState.IsJumpping) {
            ActionOnAir();
            JumpMove();
            DoubleJump();
            _highJumpTimer = 0; // 这代码好脏
        } else {
            ActionOnGround();
            HighJump();
        }
    }

    private void HighJump() {
        if (Input.GetKeyUp(KeyCode.S)) {
            _highJumpTimer = 0.5f;
        }

        if (_highJumpTimer > 0 && Input.GetKeyDown(KeyCode.W)) {
            _beforeJumpY = transform.position.y;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, HighJumpSpeed);

            _animator.SetBool("JumpHigh", true);
            _playerState.IsJumpping = true;

            // 创建烟尘效果
            Instantiate(SmokeStartNormalJump,
                new Vector3(transform.position.x, transform.position.y - SMOKE_OFFSET_Y, 0),
                transform.rotation);
        }
    }

    private void DoubleJump() {
        if (!_playerState.usedDoubleJump) {
            if (Input.GetKeyUp(KeyCode.W) && !_playerState.canDoubleJump) {
                _playerState.canDoubleJump = true;
            }

            if (Input.GetKeyDown(KeyCode.W) && _playerState.canDoubleJump) {
                _playerState.usedDoubleJump = true;
                _animator.SetTrigger("JumpDouble");
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpSpeed);
            }
        }
    }

    private void ActionOnGround() {
        // 根据主角的方向判断是冲刺还是退后
        if (Math.Abs(transform.rotation.y) < 0.1f) // 朝左
        {
            if (_playerState.IsRetreating()) {
                DoRetreat(Vector2.right);

                return;
            }

            HandleRetreat(KeyCode.D);
        } else // TODO 朝右
        { }
    }

    private void DoRetreat(Vector2 vector) {
        transform.Translate(vector * Time.deltaTime * RetreatSpeed);
        _playerState.RetreatTime += Time.deltaTime;
    }


    private void HandleRetreat(KeyCode keyCode) {
        _retreatDouble.HandleDoubleClick(keyCode, () => {
            _animator.SetTrigger("Retreat");
            _playerState.RetreatTime = 0;
        });
    }

    private void HandleInputOfRetreatOnAir(KeyCode keyCode) {
        _retreatOnAirDouble.HandleDoubleClick(keyCode, () => {
            _animator.SetTrigger("RetreatOnAir");
            _playerState.RetreatOnAirTime = 0;
        });
    }

    private void HandleInputOfSpurtOnAir(KeyCode keyCode) {
        _spurtOnAirDouble.HandleDoubleClick(keyCode, () => {
            _animator.SetTrigger("SpurtOnAir");
            _playerState.SpurtOnAirTime = 0;
        });
    }

    private void ActionOnAir() {
        // 根据主角的方向判断是冲刺还是退后
        if (Math.Abs(transform.rotation.y) < 0.1f) // 朝左
        {
            if (_playerState.IsSpurtingOnAir()) // 用了冲刺了就不能再用了，判断位移
            {
                DoSpurtOnAir(Vector2.left);

                return;
            }

            if (_playerState.IsRetreatingOnAir()) {
                DoRetreatOnAir(Vector2.right);

                return;
            }

            if (_playerState.IsSpurtOnAirEnding() || _playerState.IsRetreatOnAirEnding()) {
                DoFallDown();

                return;
            }

            HandleInputOfSpurtOnAir(KeyCode.A);
            HandleInputOfRetreatOnAir(KeyCode.D);
        } else // 朝右
        {
            if (_playerState.IsSpurtingOnAir()) // 用了冲刺了就不能再用了，判断位移
            {
                DoSpurtOnAir(Vector2.right);

                return;
            }

            if (_playerState.IsRetreatingOnAir()) {
                DoRetreatOnAir(Vector2.left);

                return;
            }

            if (_playerState.IsSpurtOnAirEnding() || _playerState.IsRetreatOnAirEnding()) {
                DoFallDown();

                return;
            }

            HandleInputOfSpurtOnAir(KeyCode.D);
            HandleInputOfRetreatOnAir(KeyCode.A);
        }
    }

    private void DoRetreatOnAir(Vector2 vector) {
        _rigidbody.velocity = Vector2.zero;
        transform.Translate(vector * Time.deltaTime * RetreatOnAirSpeed);
        _playerState.RetreatOnAirTime += Time.deltaTime;
    }

    private void DoSpurtOnAir(Vector2 vector) {
        _rigidbody.velocity = Vector2.zero;
        transform.Translate(vector * Time.deltaTime * SpurtOnAirSpeed);
        _playerState.SpurtOnAirTime += Time.deltaTime;
    }

    private void DoFallDown() {
        _rigidbody.AddForce(new Vector2(0, -_rigidbody.gravityScale));
    }

    private void JumpMove() {
        if (_playerState.IsRetreatingOnAir() ||
            _playerState.IsSpurtingOnAir() ||
            _playerState.IsRetreatOnAirEnding() ||
            _playerState.IsSpurtOnAirEnding()) // 用了冲刺了就不能再用之前的位移了
        {
            return;
        }

        if (_playerState.IsJumpForwarding) {
            transform.Translate(Vector2.left * Time.deltaTime * RunSpeed);
        } else if (_playerState.IsJumpBacking) {
            transform.Translate(Vector2.right * Time.deltaTime * RunSpeed);
        }
    }

    private void Init() {
        // 判断能否移动（通过动画，所见即所得，不然会有和动画不对应的问题）
        JudgeMoveable();
        // 清除防御状态, 必须在判断防御之前
        CleanDefense();
    }

    private void CleanDefense() {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.DEFENSE_ANIM)) {
            _animator.SetBool("Defense", false);
        }
    }

    private void Defense(float hurt) {
        _animator.SetBool("Defense", true);
        // TODO 计算伤害
    }

    private void JudgeMoveable() {
        // 只有待机、行走时是可以移动
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.IDLE_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.WALK_FORWARD_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.WALK_BACK_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.WALK_BACK_INIT_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.RUN_INIT_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.RUN_ANIM) ||
            _animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.RUN_END_ANIM)) {
            _playerState.IsMoveable = true;
        } else {
            _playerState.IsMoveable = false;
        }

        if (_playerState.IsJumpping) {
            _playerState.IsMoveable = false;
        }
    }

    private void Jump() {
        if (Input.GetKeyDown(KeyCode.W)) {
            _beforeJumpY = transform.position.y;
            //Debug.Log("起跳，记录的y轴为：" + _beforeJumpY);
            _animator.SetBool("JumpNormal", true);
            _playerState.IsJumpping = true;

            _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, JumpSpeed);


            if (_playerState.IsForwarding) {
                _playerState.IsJumpForwarding = true;
            } else if (_playerState.IsBacking) {
                _playerState.IsJumpBacking = true;
            }

            // 创建烟尘效果
            Instantiate(SmokeStartNormalJump,
                new Vector3(transform.position.x, transform.position.y - SMOKE_OFFSET_Y, 0),
                transform.rotation);
        }
    }

    private void Walk() {
        if (Input.GetKey(KeyCode.Z)) {
            _animator.SetBool("Run", true);
            _playerState.IsForwarding = true;
            transform.Translate(Vector2.left * Time.deltaTime * RunSpeed);
        } else if (Input.GetKey(KeyCode.A)) {
            _animator.SetBool("WalkForward", true);
            _animator.SetBool("WalkBack", false);
            _playerState.IsForwarding = true;
            _playerState.IsBacking = false;
            transform.Translate(Vector2.left * Time.deltaTime * WalkForwardSpeed);
        } else if (Input.GetKey(KeyCode.D)) {
            _animator.SetBool("WalkBack", true);
            _animator.SetBool("WalkForward", false);
            _playerState.IsForwarding = false;
            _playerState.IsBacking = true;
            transform.Translate(Vector2.right * Time.deltaTime * WalkBackSpeed);
        } else {
            _animator.SetBool("WalkForward", false);
            _animator.SetBool("WalkBack", false);
            _animator.SetBool("Run", false);
            _playerState.IsForwarding = false;
            _playerState.IsBacking = false;

            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(Consts.RUN_END_ANIM)) {
                transform.Translate(Vector2.left * Time.deltaTime * RunSpeed);
            }
        }
    }

    private void Attack() {
        // P
        if (Input.GetKeyDown(KeyCode.J)) {
            _animator.SetTrigger("AttackP");
        }

        // K
        if (Input.GetKeyDown(KeyCode.K)) {
            _animator.SetTrigger("AttackK");
        }

        // S
        if (Input.GetKeyDown(KeyCode.U)) {
            _animator.SetTrigger("AttackS");
        }

        // HS
        if (Input.GetKeyDown(KeyCode.I)) {
            _animator.SetTrigger("AttackHS");
        }
    }

    private void Crouch() {
        if (Input.GetKeyDown(KeyCode.S)) {
            _animator.SetBool("Crouch", true);
        }

        if (Input.GetKeyUp(KeyCode.S)) {
            _animator.SetBool("Crouch", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //Debug.Log(Time.time + "时，碰撞了");

        // 碰撞时取消水平方向的力，这样人物之间就不会粘着
        _rigidbody.velocity = new Vector2(0, _rigidbody.velocity.y);

        if (other.gameObject.CompareTag("Ground") // 碰撞的是地面 
            && _playerState.IsJumpping            // 确实正在跳跃
            && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f /*刚起跳那是假撞*/) {
            //Debug.Log(Time.time + "时，跳跃碰撞地面");
            //Debug.Log(Time.time + "时的位置：" + transform.position + "，速度：" + _rigidbody.velocity);
            _rigidbody.velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, _beforeJumpY, 0);
            //Debug.Log(Time.time + "时，位移后的位置：" + transform.position + "，速度：" + _rigidbody.velocity);

            _animator.SetBool("JumpNormal", false); // TODO 我为什么用bool？！目前下蹲有用,还可以切换到END、、怎么换掉？
            _animator.SetBool("JumpHigh", false);

            // 创建落地烟尘
            Instantiate(SmokeEndJump,
                new Vector3(transform.position.x, transform.position.y - SMOKE_OFFSET_Y, 0),
                transform.rotation);

            // 重制空中冲刺
            _playerState.CleanStates();
        }
    }
}