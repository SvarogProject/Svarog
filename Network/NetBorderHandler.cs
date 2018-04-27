using UnityEngine;
using UnityEngine.Networking;

public class NetBorderHandler : NetworkBehaviour {
    public bool CloseToRightWall;
    public bool CloseToLeftWall;
    public bool CloseToWall;

    public BoxCollider2D MovementCollider;
    private NetAnimationHandler _animation;
    private NetPlayerStateManager _state;

    private void Start() {
        _animation = GetComponent<NetAnimationHandler>();
        _state = GetComponent<NetPlayerStateManager>();
    }

    private void FixedUpdate() {
        HandleOnAnotherPlayer();
        HandleDistanceToWall();
        CloseToWall = CloseToRightWall || CloseToLeftWall;
    }

    private void HandleDistanceToWall() {
        // 判断是否会撞墙(被击退，跳头位移)
        LayerMask wallLayer = 1 << LayerMask.NameToLayer(LayerName.WALL);
        bool hitRightWall = Physics2D.Raycast(MovementCollider.transform.position, Vector2.right, 2, wallLayer);
        bool hitLeftWall = Physics2D.Raycast(MovementCollider.transform.position, Vector2.left, 2, wallLayer);

        //Debug.DrawRay(MovementCollider.transform.position, Vector2.right * 3, Color.green);
        //Debug.DrawRay(MovementCollider.transform.position, Vector2.left * 3, Color.green);

        //Debug.Log("Right:" + hitRightWall + ", Left:" + hitLeftWall);

        // 如果碰撞了说明足够近了
        CloseToRightWall = hitRightWall;
        CloseToLeftWall = hitLeftWall;
    }

    private void HandleOnAnotherPlayer() {

        LayerMask pLayerLayer;

        if (gameObject.layer == LayerMask.NameToLayer(LayerName.PLAYER)) {
            pLayerLayer = 1 << LayerMask.NameToLayer(LayerName.MOVEMENT_COLLIDER_2);
        } else {
            pLayerLayer = 1 << LayerMask.NameToLayer(LayerName.MOVEMENT_COLLIDER);
        }

        var hitRight = Physics2D.Raycast(MovementCollider.transform.position
                                         + new Vector3(MovementCollider.offset.x * (_state.LookRight ? 1 : -1), 0, 0)
                                         + Vector3.right *
                                         (MovementCollider.size.x / 2), // 0.1f为了防止正好落在中心，位移到边缘卡住
            Vector2.down, 1, pLayerLayer);

        var hitLeft = Physics2D.Raycast(MovementCollider.transform.position
                                        + new Vector3(MovementCollider.offset.x * (_state.LookRight ? 1 : -1), 0, 0)
                                        + Vector3.left * (MovementCollider.size.x / 2),
            Vector2.down, 1, pLayerLayer);


        if (_animation.Animator.GetBool(AnimatorBool.JUMP)
            && !_animation.Animator.GetBool(AnimatorBool.IS_SPURTING)
            && !_animation.Animator.GetBool(AnimatorBool.IS_RETREATING)) { // 但当前角色在跳跃而且不再冲刺的时候才判断

            // 先要判断是否离墙足够近了，那样就只能后退一条路可以走了，不能前进撞墙了
            if (CloseToLeftWall) {
                if (hitLeft || hitRight) { // 不管哪边撞到了
                    transform.Translate(
                        Vector3.right * (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f));
                }
            } else if (CloseToRightWall) {
                if (hitLeft || hitRight) { // 不管哪边撞到了
                    transform.Translate(Vector3.left *
                                        (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f));
                }
            } else {

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

                    transform.Translate(Vector3.right *
                                        (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f) *
                                        (_state.LookRight ? 1 : -1));

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

                    transform.Translate(Vector3.left *
                                        (MovementCollider.size.x / 2 + MovementCollider.offset.x + 0.1f) *
                                        (_state.LookRight ? 1 : -1));

                }
            }
        }
    }
}