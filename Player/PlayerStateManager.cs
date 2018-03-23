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

    public bool JumpDouble;

    public bool JumpHigh;
    
    public bool JumpRight;
    public bool JumpLeft;

    public bool Attackable;

    public bool IsGettingHurtSmall;
    public bool IsGettinghurtLarge;
    public bool IsGettingFriePunch;

    // public bool CurrentlyAttacking;
    public bool OnGround;
    public bool LookRight;

    public bool Dead;

    public bool CanSpurtOrRetreatOnAir;

    public BoxCollider2D MovementCollider;

    public Slider HealthSlider;
    public GameObject[] MovementColliders;

    private DamageColliderHandler _damageColliderHandler;
    [HideInInspector]
    public PlayerAnimationHandler AnimationHandler;
    private MovementHandler _movementHandler;

    private SpriteRenderer _spriteRenderer;

    // ParticleSystem blood;

    public void Start() {
        _damageColliderHandler = GetComponent<DamageColliderHandler>();
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
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
            Dead = true;
        }
    }

    private bool IsOnGround() {
        // LayerMask layer = ~(1 << gameObject.layer | 1 << 3);        
        // LayerMask layer = ~(1 << gameObject.layer); // 除了角色自己这层
        // bool retVal = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layer);

        LayerMask layer = 1 << 10; // 只检测地板这层
        // TODO 现在跳跃的时候脚超出了锚点，所以0.4f来修正，之后图变了再改
        bool retVal = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.5f, layer);

        if (retVal && AnimationHandler.Animator.GetBool("Jump") && _movementHandler.Rigidbody.velocity.y <= 0) { // 跳跃落地进行一系列处理
            JumpRight = false;
            JumpLeft = false;
            ResetAttacks();
            AnimationHandler.CloseJumpAnim();
        }

        return retVal;
    }

    public void ResetAttacks() {
        foreach (var attack in Attacks) {
            attack.Reset();
        }
    }

    public void ResetPlayer() {
        ResetAttacks();
        Dead = false;
        Jump = false;
        Crouch = false;
        Right = false;
        Left = false;
        IsGettingHurtSmall = false;
        IsGettinghurtLarge = false;
        IsGettingFriePunch = false;
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

    public void TakeDamage(int damage, DamageType damageType) {
        if (IsGettingHurtSmall || IsGettinghurtLarge) return;

        switch (damageType) {
            case DamageType.Light:               
                IsGettingHurtSmall = true;
                //if (!OnGround) {
                //    _movementHandler.AddVelocityOnCharacter(
                //        (!LookRight ? Vector2.right * 1 : Vector2.right * -1) + Vector2.up, 0.5f);
                //}
                StartCoroutine(CloseImmortality(0.1f));

                break;
            case DamageType.Heavy:

                IsGettinghurtLarge = true;
                _movementHandler.AddVelocityOnCharacter(
                    (!LookRight ? Vector2.right * 1 : Vector2.right * -1) + Vector2.up, 0.5f);

                StartCoroutine(CloseImmortality(0.5f));

                break;

            case DamageType.FirePunch:
                IsGettingFriePunch = true;
                _movementHandler.AddVelocityOnCharacter(Vector2.up * 10, 0.5f);

                StartCoroutine(CloseImmortality(0.5f));

                break;
            default:

                throw new ArgumentOutOfRangeException("damageType", damageType, null);
        }
        Health -= damage;       
    }

    private IEnumerator CloseImmortality(float timer) {
        yield return new WaitForSeconds(timer);

        IsGettingHurtSmall = false;
        IsGettinghurtLarge = false;
        IsGettingFriePunch = false;
    }
}