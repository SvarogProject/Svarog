using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[NetworkSettings(channel = 0, sendInterval = 0.1f)]
public class NetPlayerStateManager : NetworkBehaviour {
    
    #region Sync Params
    
    [SyncVar] public float Health = 100;
    public AttacksBase[] Attacks;
    [SyncVar] public bool Crouch;
    [SyncVar] public bool Jump;
    [SyncVar] public bool Left;
    [SyncVar] public bool Right;
    [SyncVar] public bool LeftDouble;
    [SyncVar] public bool RightDouble;
    [SyncVar] public bool JumpDouble;
    [SyncVar] public bool JumpHigh;
    [SyncVar] public bool JumpRight;
    [SyncVar] public bool JumpLeft;
    [SyncVar] public bool Attackable;
    [SyncVar] public bool IsGettingHurtSmall;
    [SyncVar] public bool IsGettingHurtLarge;
    [SyncVar] public bool IsGettingFriePunch;
    [SyncVar] public bool IsGettingHurtDefense;
    [SyncVar] public bool OnGround;
    [SyncVar] public bool LookRight;
    [SyncVar] public bool Dead;
    [SyncVar] public bool ShouldLookBack; // 需要回头，但是还没回
    [SyncVar] public bool CanSpurtOrRetreatOnAir;
    [SyncVar] public bool DefenseLeft;
    [SyncVar] public bool DefenseRight;
    [SyncVar] public bool Stop; // 顿帧
    [SyncVar] public float Score;

    #endregion
    
    #region Commands

    public void CmdScore(float score) {
        Score = score; 
        CmdScore_Comand(score); 
    }
    [Command]
    public void CmdScore_Comand(float score) {
        Score = score;
    }
    public void CmdStop(bool stop) {
        Stop = stop; 
        CmdStop_Comand(stop); 
    }
    [Command]
    public void CmdStop_Comand(bool stop) {
        Stop = stop;
    }
    public void CmdHealthSlider(GameObject heathSlider) {
        HealthSlider = heathSlider; // 这个修改本地，就不用等服务器会传
        CmdHealthSlider_Comand(heathSlider); // 这个通知服务器
    }
    [Command]
    public void CmdHealthSlider_Comand(GameObject heathSlider) {
        HealthSlider = heathSlider;
    }
    public void CmdChangeHealth(float health) {
        Health = health;
        CmdChangeHealth_Command(health);
    }
    [Command]
    public void CmdChangeHealth_Command(float health) {
        Health = health;
    }
    public void CmdCanSpurtOrRetreatOnAir(bool canSpurtOrRetreatOnAir) {
        CanSpurtOrRetreatOnAir = canSpurtOrRetreatOnAir;
        CmdCanSpurtOrRetreatOnAir_Command(canSpurtOrRetreatOnAir);
    }
    [Command]
    public void CmdCanSpurtOrRetreatOnAir_Command(bool canSpurtOrRetreatOnAir) {
        CanSpurtOrRetreatOnAir = canSpurtOrRetreatOnAir;
    }
    public void CmdShouldLookBack(bool shouldLookBack) {
        ShouldLookBack = shouldLookBack;
        CmdShouldLookBack_Command(shouldLookBack);
    }
    [Command]
    public void CmdShouldLookBack_Command(bool shouldLookBack) {
        ShouldLookBack = shouldLookBack;
    }
    public void CmdDead(bool dead) {
        Dead = dead;
        CmdDead_Command(dead);
    }
    [Command]
    public void CmdDead_Command(bool dead) {
        Dead = dead;
    }
    public void CmdLookRight(bool lookRight) {
        LookRight = lookRight;
        CmdLookRight_Command(lookRight);
    }
    [Command]
    public void CmdLookRight_Command(bool lookRight) {
        LookRight = lookRight;
    }
    public void CmdOnGround(bool onGround) {
        OnGround = onGround;
        CmdOnGround_Command(onGround);
    }
    [Command]
    public void CmdOnGround_Command(bool onGround) {
        OnGround = onGround;
    }
    public void CmdIsGettingFriePunch(bool isGettingFriePunch) {
        IsGettingFriePunch = isGettingFriePunch;
        CmdIsGettingFriePunch_Command(isGettingFriePunch);
    }
    [Command]
    public void CmdIsGettingFriePunch_Command(bool isGettingFriePunch) {
        IsGettingFriePunch = isGettingFriePunch;
    }
    public void CmdIsGettingHurtDefense(bool isGettingHurtDefense) {
        IsGettingHurtDefense = isGettingHurtDefense;
        CmdIsGettingHurtDefense_Command(isGettingHurtDefense);
    }
    [Command]
    public void CmdIsGettingHurtDefense_Command(bool isGettingHurtDefense) {
        IsGettingHurtDefense = isGettingHurtDefense;
    }
    public void CmdIsGettingHurtLarge(bool isGettingHurtLarge) {
        IsGettingHurtLarge = isGettingHurtLarge;
        CmdIsGettingHurtLarge_Command(isGettingHurtLarge);
    }
    [Command]
    public void CmdIsGettingHurtLarge_Command(bool isGettingHurtLarge) {
        IsGettingHurtLarge = isGettingHurtLarge;
    }
    public void CmdIsGettingHurtSmall(bool isGettingHurtSmall) {
        IsGettingHurtSmall = isGettingHurtSmall;
        CmdIsGettingHurtSmall_Command(isGettingHurtSmall);
    }
    [Command]
    public void CmdIsGettingHurtSmall_Command(bool isGettingHurtSmall) {
        IsGettingHurtSmall = isGettingHurtSmall;
    }
    public void CmdJumpRight(bool jumpRight) {
        JumpRight = jumpRight;
        CmdJumpRight_Command(jumpRight);
    }
    [Command]
    public void CmdJumpRight_Command(bool jumpRight) {
        JumpRight = jumpRight;
    }
    public void CmdJumpLeft(bool jumpLeft) {
        JumpLeft = jumpLeft;
        CmdJumpLeft_Command(jumpLeft);
    }    
    [Command]
    public void CmdJumpLeft_Command(bool jumpLeft) {
        JumpLeft = jumpLeft;
    }   
    public void CmdJumpHigh(bool jumpHigh) {
        JumpHigh = jumpHigh;
        CmdJumpHigh_Command(jumpHigh);
    }   
    [Command]
    public void CmdJumpHigh_Command(bool jumpHigh) {
        JumpHigh = jumpHigh;
    }   
    public void CmdJumpDouble(bool jumpDouble) {
        JumpDouble = jumpDouble;
        CmdJumpDouble_Command(jumpDouble);
    }
    [Command]
    public void CmdJumpDouble_Command(bool jumpDouble) {
        JumpDouble = jumpDouble;
    }
    public void CmdJump(bool jump) {
        Jump = jump;
        CmdJump_Command(jump);
    }   
    [Command]
    public void CmdJump_Command(bool jump) {
        Jump = jump;
    }    
    public void CmdRightDouble(bool rightDouble) {
        RightDouble = rightDouble;
        CmdRightDouble_Command(rightDouble);
    }   
    [Command]
    public void CmdRightDouble_Command(bool rightDouble) {
        RightDouble = rightDouble;
    }   
    public void CmdLeftDouble(bool leftDouble) {
        LeftDouble = leftDouble;
        CmdLeftDouble_Command(leftDouble);
    }    
    [Command]
    public void CmdLeftDouble_Command(bool leftDouble) {
        LeftDouble = leftDouble;
    }    
    public void CmdRight(bool right) {
        Right = right;
        CmdRight_Command(right);
    }   
    [Command]
    public void CmdRight_Command(bool right) {
        Right = right;
    }   
    public void CmdLeft(bool left) {
        Left = left;
        CmdLeft_Command(left);
    }
    [Command]
    public void CmdLeft_Command(bool left) {
        Left = left;
    }
    public void CmdDefenseLeft(bool defenseLeft) {
        DefenseLeft = defenseLeft;
        CmdDefenseLeft_Command(defenseLeft);
    }   
    [Command]
    public void CmdDefenseLeft_Command(bool defenseLeft) {
        DefenseLeft = defenseLeft;
    }   
    public void CmdDefenseRight(bool defenseRight) {
        DefenseRight = defenseRight;
        CmdDefenseRight_Command(defenseRight);
    }    
    [Command]
    public void CmdDefenseRight_Command(bool defenseRight) {
        DefenseRight = defenseRight;
    }    
    public void CmdCrouch(bool crouch) {
        Crouch = crouch;
        CmdCrouch_Command(crouch);
    }
    [Command]
    public void CmdCrouch_Command(bool crouch) {
        Crouch = crouch;
    }

    #endregion

    public BoxCollider2D MovementCollider;

    [SyncVar] public GameObject HealthSlider;

    [HideInInspector] public NetAnimationHandler AnimationHandler;
    private NetMovementHandler _movementHandler;

    public void Start() {
        AnimationHandler = GetComponent<NetAnimationHandler>();
        _movementHandler = GetComponent<NetMovementHandler>();
    }

    public void FixedUpdate() {
        if (Stop) return; // 顿帧中
        
        GetComponent<Transform>().localRotation = Quaternion.Euler(0, LookRight ? 0 : 180, 0);  
        
        if (isLocalPlayer) {
            CmdOnGround(IsOnGround());
        }

        if (Health <= 0 && NetLevelManger.GetInstance().EnableCountdown) {
            NetLevelManger.GetInstance().EndRoundFunction();

            CmdDead(true);                   
        }

        OnChangeHealth(Health);
    }

    private void OnChangeHealth(float health) {
        if (HealthSlider != null) {
            HealthSlider.GetComponent<Slider>().value = health / 100;
        }
    }

    private bool IsOnGround() {

        LayerMask groundLayer = 1 << LayerMask.NameToLayer(LayerName.GROUND); // 只检测地板这层
        //Debug.DrawRay(MovementCollider.transform.position, Vector2.down, Color.green);
        bool retVal = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.5f, groundLayer);

        if (retVal && AnimationHandler.Animator.GetBool("Jump") && _movementHandler.Rigidbody.velocity.y <= 0) {
            // 跳跃落地进行一系列处理
            CmdJumpRight(false);
            CmdJumpLeft(false);
            ResetAttacks();
            AnimationHandler.CmdCloseJumpAnim();
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
        CmdDead(false);
        CmdJump(false);
        CmdCrouch(false);
        CmdRight(false);
        CmdLeft(false);
        CmdIsGettingHurtSmall(false);
        CmdIsGettingHurtLarge(false);
        CmdIsGettingHurtDefense(false);
        CmdIsGettingFriePunch(false);
        CmdJumpRight(false);
        CmdJumpLeft(false);
        CmdRightDouble(false);
        CmdLeftDouble(false);
    }

    public void TakeDamage(float damage, DamageType damageType) {
        if (!isServer) return; // 伤害的计算只能服务器控制
        if (IsGettingHurtSmall || IsGettingHurtLarge) return;

        switch (damageType) {
            case DamageType.Light:

                if (!IsGettingHurtSmall) {
                    CmdIsGettingHurtSmall(true);
                    StartCoroutine(CloseImmortality(0.1f));
                }

                break;
            case DamageType.Heavy:

                if (!IsGettingHurtLarge) {
                    CmdIsGettingHurtLarge(true);
                    StartCoroutine(CloseImmortality(0.2f));
                }      

                break;

            case DamageType.FirePunch:

                if (!IsGettingFriePunch) {
                    CmdIsGettingFriePunch(true);                  
                    _movementHandler.AddVelocityOnCharacter(Vector2.up * _movementHandler.JumpSpeed * 1.2f, 0.01f);
                    StartCoroutine(CloseImmortality(0.8f));
                }

                break;
            case DamageType.Defensed:

                if (!IsGettingHurtDefense) {
                    CmdIsGettingHurtDefense(true);
                    StartCoroutine(CloseImmortality(0.5f));
                }           
                
                break;
        }
        
        //AnimationHandler.Stop(0.2f);
        StateStop(0.2f);

        Health -= damage;
    }

    private IEnumerator CloseImmortality(float timer) {
        yield return new WaitForSeconds(timer);

        CmdIsGettingHurtSmall(false);
        CmdIsGettingHurtLarge(false);
        CmdIsGettingHurtDefense(false);
        CmdIsGettingFriePunch(false);
    }
    
    // 顿帧
    public void StateStop(float time) {
        AnimationHandler.Stop(time);
        CmdStop(true);
        //Invoke("AnimPlay", time);
        StartCoroutine(StatePlay(time));
    }
    
    private IEnumerator StatePlay(float time) {
        yield return new WaitForSeconds(time);

        CmdStop(false);
    }
    
}