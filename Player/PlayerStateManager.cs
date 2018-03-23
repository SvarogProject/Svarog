using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class PlayerStateManager : MonoBehaviour {
    
    public int Health = 100;

    #region Action Params
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
    public bool OnGround;
    public bool LookRight;
    public bool Dead;
    public bool ShouldLookBack; // 需要回头，但是还没回
    public bool CanSpurtOrRetreatOnAir;
    #endregion

    public BoxCollider2D MovementCollider;

    public Slider HealthSlider;
    public GameObject[] MovementColliders;

    [HideInInspector] public PlayerAnimationHandler AnimationHandler;
    private MovementHandler _movementHandler;

    private SpriteRenderer _spriteRenderer;

    // ParticleSystem blood;

    public void Start() {
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
        _movementHandler = GetComponent<MovementHandler>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        // blood = GetComponentInChildren<ParticleSystem>();
    }

    public void FixedUpdate() {
        GetComponent<Transform>().localRotation = Quaternion.Euler(0, LookRight ? 0 : 180, 0);

        OnGround = IsOnGround();

        //HandleOnAnotherPlayer();

        if (HealthSlider != null) {
            HealthSlider.value = Health * 0.01f;
        }

        if (Health <= 0 && LevelManager.GetInstance().EnableCountdown) {
            LevelManager.GetInstance().EndRoundFunction();
            Dead = true;
        }
    }

    /*
    private void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("MovementCollider")) {
        
            // 得到碰撞位置（世界坐标）
            var contact = other.contacts[0];
            var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            Vector3 pos = contact.point;
            
            Debug.Log("pos: " + pos);
            Debug.Log("size: " + ((BoxCollider2D)other.collider).size);
            
            if (Math.Abs(pos.y - other.transform.position.y + ((BoxCollider2D)other.collider).size.y) < 0.1f) { // 如果在上方
                if (LookRight) { // 说明自己的中轴线在对手的左边，就往左边移动一点
                    transform.Translate(Vector3.left * MovementCollider.size.x);
                } else {
                    transform.Translate(Vector3.right * MovementCollider.size.x);
                }
            }
        }
    }*/

    private void HandleOnAnotherPlayer() {
        //TODO 射线不行，碰撞的时候中点不一定挨着了，斜射线呢？
        
        LayerMask pLayerLayer;
        if (gameObject.layer == 9) {
            pLayerLayer = 1 << 10;
        } else {
            pLayerLayer = 1 << 9;
        }

        Debug.DrawRay(MovementCollider.transform.position, Vector2.down * 0.01f, Color.green);
        var hit = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.01f, pLayerLayer);

        // Debug.Log(gameObject.layer);
        
        if (hit) {
            // 如果射线碰撞的角色不是自己的话，说明对手在自己脚下，调整自己的位置
            if (LookRight) { // 说明自己的中轴线在对手的左边，就往左边移动一点
                transform.Translate(Vector3.left * MovementCollider.size.x);
            } else {
                transform.Translate(Vector3.right * MovementCollider.size.x);
            }
        }

    }

    private bool IsOnGround() {
        // LayerMask layer = ~(1 << gameObject.layer | 1 << 3);        
        // LayerMask layer = ~(1 << gameObject.layer); // 除了角色自己这层
        // bool retVal = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layer);

        LayerMask groundLayer = 1 << 11; // 只检测地板这层
        //Debug.DrawRay(MovementCollider.transform.position, Vector2.down, Color.green);
        // TODO 现在跳跃的时候脚超出了锚点，所以0.4f来修正，之后图变了再改
        bool retVal = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.5f, groundLayer);

        if (retVal && AnimationHandler.Animator.GetBool("Jump") && _movementHandler.Rigidbody.velocity.y <= 0) {
            // 跳跃落地进行一系列处理
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