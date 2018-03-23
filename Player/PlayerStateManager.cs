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

    public void Start() {
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
        _movementHandler = GetComponent<MovementHandler>();
    }

    public void FixedUpdate() {
        GetComponent<Transform>().localRotation = Quaternion.Euler(0, LookRight ? 0 : 180, 0);

        OnGround = IsOnGround();

        HandleOnAnotherPlayer();

        if (HealthSlider != null) {
            HealthSlider.value = Health * 0.01f;
        }

        if (Health <= 0 && LevelManager.GetInstance().EnableCountdown) {
            LevelManager.GetInstance().EndRoundFunction();
            Dead = true;
        }
    }

    private void HandleOnAnotherPlayer() {

        LayerMask pLayerLayer;

        if (gameObject.layer == LayerMask.NameToLayer("Player")) {
            pLayerLayer = 1 << LayerMask.NameToLayer("MovementCollider2");
        } else {
            pLayerLayer = 1 << LayerMask.NameToLayer("MovementCollider");
        }

        Debug.DrawRay(MovementCollider.transform.position, Vector2.down * 0.1f, Color.green);

        Debug.DrawRay(MovementCollider.transform.position
                      + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                      + Vector3.left * MovementCollider.size.x / 2,
            Vector2.down * 0.1f, Color.red);

        Debug.DrawRay(MovementCollider.transform.position
                      + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                      + Vector3.right * MovementCollider.size.x / 2,
            Vector2.down * 0.1f, Color.red);

        var hitRight = Physics2D.Raycast(MovementCollider.transform.position
                                         + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                                         + Vector3.right * MovementCollider.size.x / 2 ,
            Vector2.down, 0.1f, pLayerLayer);

        var hitCenter = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.1f, pLayerLayer);

        var hitLeft = Physics2D.Raycast(MovementCollider.transform.position
                                        + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                                        + Vector3.left * MovementCollider.size.x / 2,
            Vector2.down, 0.1f, pLayerLayer);

        if (hitLeft) {
            Debug.Log("Left: " + hitLeft + ", Tag: " + hitLeft.collider.tag);
        }

        // Debug.Log(gameObject.layer);

        if (AnimationHandler.Animator.GetBool(AnimatorBool.JUMP) 
            && !AnimationHandler.Animator.GetBool(AnimatorBool.IS_SPURTING) 
            && !AnimationHandler.Animator.GetBool(AnimatorBool.IS_RETREATING)) { // 但当前角色在跳跃而且不再冲刺的时候才判断

            if (hitLeft) {
                Debug.Log(hitLeft.transform.position.y + ", " + MovementCollider.transform.position.y + ", " + ((BoxCollider2D)hitLeft.collider).size.y);
            }
            
            // 如果在下面重叠不采取措施，可能是回退等操作造成的
            if (hitLeft && hitLeft.transform.position.y < MovementCollider.transform.position.y - ((BoxCollider2D)hitLeft.collider).size.y) {                              

                transform.Translate(Vector3.right * (MovementCollider.size.x + MovementCollider.offset.x) / 2 * (LookRight ? 1 : -1));
                
            } else if (hitRight && hitRight.transform.position.y < MovementCollider.transform.position.y - ((BoxCollider2D)hitRight.collider).size.y) {
                transform.Translate(Vector3.left * (MovementCollider.size.x + MovementCollider.offset.x) / 2 * (LookRight ? 1 : -1));

            } else if (hitCenter && hitCenter.transform.position.y < MovementCollider.transform.position.y - ((BoxCollider2D)hitCenter.collider).size.y) {
                // 如果射线碰撞的角色不是自己的话，说明对手在自己脚下，调整自己的位置          
                if (hitCenter.transform.position.x > transform.position.x) { // 说明自己的中轴线在对手的左边，就往左边移动一点
                    transform.Translate(Vector3.left * MovementCollider.size.x / 2);
                } else {
                    transform.Translate(Vector3.right * MovementCollider.size.x / 2);
                }
            }
        }    
    }

    private bool IsOnGround() {
        // LayerMask layer = ~(1 << gameObject.layer | 1 << 3);        
        // LayerMask layer = ~(1 << gameObject.layer); // 除了角色自己这层
        // bool retVal = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, layer);

        LayerMask groundLayer = 1 << LayerMask.NameToLayer("Ground"); // 只检测地板这层
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