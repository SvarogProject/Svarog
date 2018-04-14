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
    public void CmdCouch(bool crouch) {
        Crouch = crouch;
        CmdCouch_Command(crouch);
    }
    [Command]
    public void CmdCouch_Command(bool crouch) {
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
        

        HandleOnAnotherPlayer();

        if (Health <= 0 && NetLevelManger.GetInstance().EnableCountdown) {
            NetLevelManger.GetInstance().EndRoundFunction();

            CmdDead(true);                   
        }

        OnChangeHealth(Health);
    }

    /*
    public override bool OnSerialize(NetworkWriter writer, bool forceAll) {
        if (HealthSlider == null) {
            return false;
        }
        writer.Write(HealthSlider.gameObject);
        return true;
    }

    public override void OnDeserialize(NetworkReader reader, bool initialState) {
        if (HealthSlider != null && reader.ReadGameObject().GetComponent<Slider>() == null) {
            return;
        }
        HealthSlider = reader.ReadGameObject().GetComponent<Slider>();
    }*/

    private void OnChangeHealth(float health) {
        if (HealthSlider != null) {
            HealthSlider.GetComponent<Slider>().value = health / 100;
        }
    }

    // TODO 这一段是否应该把判断搬到CharactorManager中，移动放到MovementHandler中
    // TODO 边角的时候应该考虑让对手移动
    private void HandleOnAnotherPlayer() {

        LayerMask pLayerLayer;

        if (gameObject.layer == LayerMask.NameToLayer(LayerName.PLAYER)) {
            pLayerLayer = 1 << LayerMask.NameToLayer(LayerName.MOVEMENT_COLLIDER_2);
        } else {
            pLayerLayer = 1 << LayerMask.NameToLayer(LayerName.MOVEMENT_COLLIDER);
        }

        var hitRight = Physics2D.Raycast(MovementCollider.transform.position
                                         + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                                         + Vector3.right *
                                         (MovementCollider.size.x / 2), // 0.1f为了防止正好落在中心，位移到边缘卡住
            Vector2.down, 0.5f, pLayerLayer);

        //var hitCenter = Physics2D.Raycast(MovementCollider.transform.position, Vector2.down, 0.5f, pLayerLayer);

        var hitLeft = Physics2D.Raycast(MovementCollider.transform.position
                                        + new Vector3(MovementCollider.offset.x * (LookRight ? 1 : -1), 0, 0)
                                        + Vector3.left * (MovementCollider.size.x / 2),
            Vector2.down, 0.5f, pLayerLayer);


        if (AnimationHandler.Animator.GetBool(AnimatorBool.JUMP)
            && !AnimationHandler.Animator.GetBool(AnimatorBool.IS_SPURTING)
            && !AnimationHandler.Animator.GetBool(AnimatorBool.IS_RETREATING)) { // 但当前角色在跳跃而且不再冲刺的时候才判断
            
            if (hitLeft) { 

                var hitUpY = hitLeft.transform.position.y + ((BoxCollider2D) hitLeft.collider).offset.y +
                               ((BoxCollider2D) hitLeft.collider).size.y / 2;
                
                var hitDownY = hitLeft.transform.position.y + ((BoxCollider2D) hitLeft.collider).offset.y -
                               ((BoxCollider2D) hitLeft.collider).size.y / 2;

                var thisDownY = MovementCollider.transform.position.y + MovementCollider.offset.y -
                                  MovementCollider.size.y / 2;
                
                if (hitDownY > 0.1f) { // 玩家2也是跳跃的不位移
                    return;
                }

                if (thisDownY < hitUpY) { // 如果在下面重叠不采取措施，可能是回退等操作造成的
                    return;
                }

                transform.Translate(Vector3.right * (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f) *
                                    (LookRight ? 1 : -1));

            } else if (hitRight) {
                
                var hitUpY = hitRight.transform.position.y + ((BoxCollider2D) hitRight.collider).offset.y +
                             ((BoxCollider2D) hitRight.collider).size.y / 2;
                
                var hitDownY = hitRight.transform.position.y + ((BoxCollider2D) hitRight.collider).offset.y -
                               ((BoxCollider2D) hitRight.collider).size.y / 2;

                var thisDownY = MovementCollider.transform.position.y + MovementCollider.offset.y -
                                MovementCollider.size.y / 2;
                
                                
                if (hitDownY > 0.1f) { // 玩家2也是跳跃的不位移
                    return;
                }

                if (thisDownY < hitUpY) { // 如果在下面重叠不采取措施，可能是回退等操作造成的
                    return;
                }
                
                transform.Translate(Vector3.left * (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f) *
                                    (LookRight ? 1 : -1));

            } /*else if (hitCenter && hitCenter.transform.position.y <
                       MovementCollider.transform.position.y - ((BoxCollider2D) hitCenter.collider).size.y) {
                // 如果射线碰撞的角色不是自己的话，说明对手在自己脚下，调整自己的位置          
                if (hitCenter.transform.position.x > transform.position.x) { // 说明自己的中轴线在对手的左边，就往左边移动一点
                    transform.Translate(Vector3.left * (MovementCollider.size.x / 2 + 0.1f));
                } else {
                    transform.Translate(Vector3.right * (MovementCollider.size.x / 2 + 0.1f));
                }
            }*/
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
        CmdCouch(false);
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

                    _movementHandler.AddVelocityOnCharacter(
                        (LookRight ? Vector2.left : Vector2.right) * 2, 0.2f);
                    
                    StartCoroutine(CloseImmortality(0.1f));
                }

                break;
            case DamageType.Heavy:

                if (!IsGettingHurtLarge) {
                    CmdIsGettingHurtLarge(true);

                    _movementHandler.AddVelocityOnCharacter(
                        (LookRight ? Vector2.left : Vector2.right) * 4, 0.2f);

                    StartCoroutine(CloseImmortality(0.2f));
                }      

                break;

            case DamageType.FirePunch:

                if (!IsGettingFriePunch) {
                    CmdIsGettingFriePunch(true);
                    
                    _movementHandler.AddVelocityOnCharacter(Vector2.up * _movementHandler.JumpSpeed * 1.2f, 0.01f);
                    // TODO 这个位移只生效一次
                    StartCoroutine(CloseImmortality(0.8f));
                }

                break;
            case DamageType.Defensed:

                if (!IsGettingHurtDefense) {
                    CmdIsGettingHurtDefense(true);
                    
                    _movementHandler.AddVelocityOnCharacter(
                        (LookRight ? Vector2.left : Vector2.right) * 2, 0.2f);
                
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