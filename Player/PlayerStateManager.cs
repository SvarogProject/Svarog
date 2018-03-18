using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerStateManager : MonoBehaviour {
    public int Health = 100;

    // public float horizontal;
    // public float vertical;

    // 输入的参数
/*    public bool AttackP;
    public bool AttackK;
    public bool AttackS;
    public bool AttackHs;*/
    public AttacksBase[] Attacks;

    public bool Crouch;
    public bool Jump;
    public bool Left;
    public bool Right;

    public bool LeftDouble;
    public bool RightDouble;

    public bool JumpRight;
    public bool JumpLeft;

    public bool Attackable;

    public bool IsGettingHit;

    // public bool CurrentlyAttacking;
    public bool OnGround;
    public bool LookRight;

    public Slider HealthSlider;
    public GameObject[] MovementColliders;

    private DamageColliderHandler _damageColliderHandler;
    private PlayerAnimationHandler _animationHandler;
    private MovementHandler _movementHandler;

    private SpriteRenderer _spriteRenderer;

    // ParticleSystem blood;

    public void Start() {
        _damageColliderHandler = GetComponent<DamageColliderHandler>();
        _animationHandler = GetComponent<PlayerAnimationHandler>();
        _movementHandler = GetComponent<MovementHandler>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // blood = GetComponentInChildren<ParticleSystem>();
    }

    public void FixedUpdate() {
        GetComponent<Transform>().localRotation = Quaternion.Euler(0, LookRight ? 0 : 180, 0);

        OnGround = IsOnGround();

        if (HealthSlider != null) {
            HealthSlider.value = Health * 0.01f;
        }

        if (Health <= 0 && LevelManager.GetInstance().EnableCountdown) {
            LevelManager.GetInstance().EndRoundFunction();
            _animationHandler.Animator.Play("Dead");
        }
    }

    private bool IsOnGround() {
        // LayerMask layer = ~(1 << gameObject.layer | 1 << 3);        
        // LayerMask layer = ~(1 << gameObject.layer); // 除了角色自己这层
        // bool retVal = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layer);

        var movementCollider = GameObject.Find("SpriteRenderer/MovementColliders/MovementCollider")
            .GetComponent<BoxCollider2D>();

        LayerMask layer = 1 << 10; // 只检测地板这层
        bool retVal = Physics2D.Raycast(movementCollider.transform.position, Vector2.down, 0.3f, layer);

        if (retVal && _animationHandler.Animator.GetBool("Jump")) { // 跳跃落地进行一系列处理
            JumpRight = false;
            JumpLeft = false;
            ResetAttacks();
            _animationHandler.CloseJumpAnim();
        }

        return retVal;
    }

    public void ResetAttacks() {
        foreach (var attack in Attacks) {
            attack.Reset();
        }
    }

    public void ResetStateInputs() {
        ResetAttacks();
        Jump = false;
        Crouch = false;
        Right = false;
        Left = false;
        IsGettingHit = false;
        JumpRight = false;
        JumpLeft = false;
        RightDouble = false;
        LeftDouble = false;
    }

    public void CloseMovementCollider(int index) {
        MovementColliders[index].SetActive(false);
    }

    public void OpenMovementCollider(int index) {
        MovementColliders[index].SetActive(true);
    }

    public void TakeDamage(int damage, DamageColliderHandler.DamageType damageType) {
        if (IsGettingHit) return;

        switch (damageType) {
            case DamageColliderHandler.DamageType.Light:
                StartCoroutine(CloseImmortality(0.1f));

                break;
            case DamageColliderHandler.DamageType.Heavy:

                _movementHandler.AddVelocityOnCharacter(
                    (!LookRight ? Vector3.right * 1 : Vector3.right * -1) + Vector3.up, 0.5f);

                StartCoroutine(CloseImmortality(0.5f));

                break;
            default:

                throw new ArgumentOutOfRangeException("damageType", damageType, null);
        }

        // if (blood != null) blood.Emit(30);

        Health -= damage;
        IsGettingHit = true;
    }

    private IEnumerator CloseImmortality(float timer) {
        yield return new WaitForSeconds(timer);

        IsGettingHit = false;
    }
}