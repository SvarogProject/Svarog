using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerStateManager : MonoBehaviour {
    public float Health = 100;

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
    public bool Attackable; // TODO 这个变量根本……没有写入
    public bool IsGettingHurtSmall;
    public bool IsGettingHurtLarge;
    public bool IsGettingFriePunch;
    public bool IsGettingHurtDefense;
    public bool OnGround;
    public bool LookRight;
    public bool Dead;
    public bool ShouldLookBack; // 需要回头，但是还没回
    public bool CanSpurtOrRetreatOnAir;
    public bool DefenseLeft;
    public bool DefenseRight;
    public bool Stop; // 顿帧

    #endregion

    public BoxCollider2D MovementCollider;

    public Slider HealthSlider;
    public HitText HitText;

    [HideInInspector] public PlayerAnimationHandler AnimationHandler;
    private MovementHandler _movementHandler;

    public void Start() {
        AnimationHandler = GetComponent<PlayerAnimationHandler>();
        _movementHandler = GetComponent<MovementHandler>();
    }

    public void FixedUpdate() {
        if (Stop) return; // 顿帧中
        
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
    
    private bool IsOnGround() {

        LayerMask groundLayer = 1 << LayerMask.NameToLayer(LayerName.GROUND); // 只检测地板这层
        //Debug.DrawRay(MovementCollider.transform.position, Vector2.down, Color.green);
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
        IsGettingHurtLarge = false;
        IsGettingFriePunch = false;
        IsGettingHurtDefense = false;
        JumpRight = false;
        JumpLeft = false;
        RightDouble = false;
        LeftDouble = false;
    }

    public void TakeDamage(float damage, DamageType damageType) {
        if (IsGettingHurtSmall || IsGettingHurtLarge) return;

        switch (damageType) {
            case DamageType.Light:

                if (!IsGettingHurtSmall) {
                    IsGettingHurtSmall = true;
                    StartCoroutine(CloseImmortality(0.1f));
                }

                break;
            case DamageType.Heavy:

                if (!IsGettingHurtLarge) {
                    IsGettingHurtLarge = true;
                    StartCoroutine(CloseImmortality(0.2f));
                }      

                break;

            case DamageType.FirePunch:

                if (!IsGettingFriePunch) {
                    IsGettingFriePunch = true;
                    
                    _movementHandler.AddVelocityOnCharacter(Vector2.up * _movementHandler.JumpSpeed * 1.2f, 0.02f);
                    // TODO 这个位移只生效一次
                    StartCoroutine(CloseImmortality(0.8f));
                }

                break;
            case DamageType.Defensed:

                if (!IsGettingHurtDefense) {
                    IsGettingHurtDefense = true;              
                    StartCoroutine(CloseImmortality(0.5f));
                }           
                
                break;
            default:

                throw new ArgumentOutOfRangeException("damageType", damageType, null);
        }
        
        //AnimationHandler.Stop(0.2f);
        StateStop(0.2f);

        Health -= damage;
    }

    private IEnumerator CloseImmortality(float timer) {

        yield return new WaitForSeconds(timer);

        IsGettingHurtSmall = false;
        IsGettingHurtLarge = false;
        IsGettingFriePunch = false;
        IsGettingHurtDefense = false;
    }
    
    // 顿帧
    public void StateStop(float time) {
        AnimationHandler.Stop(time);
        Stop = true;
        //Invoke("AnimPlay", time);
        StartCoroutine(StatePlay(time));
    }
    
    private IEnumerator StatePlay(float time) {
        yield return new WaitForSeconds(time);

        Stop = false;
    }
}