using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class NetCameraMoveManager : NetworkBehaviour  {

    public GameObject Player1;
    public GameObject Player2;

    public SpriteRenderer Background;
    public float FollowSpeed;

    private BoxCollider2D _playerBox;
    private BoxCollider2D _player2Box;
    private BoxCollider2D _playerHeadBox;
    private BoxCollider2D _playerHeadBox2;
    private Animator _playerAnimation;
    private Animator _playerAnimation2;

    private float _maxX;
    private float _minX;
    private float _maxY;
    private float _minY;
    private float _cameraMaxSize = 5.4f;
    private float _cameraMinSize = 4.2f;
    private float _cameraSizeTimer;
    private Camera _camera;
    private CameraSizeType _type = CameraSizeType.SMALL;

    #region Singleton

    private static NetCameraMoveManager _instance;

    public static NetCameraMoveManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion

    private void Start() {
        _camera = GetComponent<Camera>();
        Initial();
    }

    private void FixedUpdate() {

        while (Player1 == null || Player2 == null) { // 不停的自己找
            Initial();
        }

        // 必须要levelmanager设置了两个玩家之后才能使用
        if (Player1 != null && Player2 != null) {
            ChangeCameraSize();
            LimitCameraPosition();
            MoveCamera();
        }
    }

    public void Initial() {
 
        Player1 = GameObject.FindGameObjectWithTag("Player").GetComponent<NetPlayerStateManager>().gameObject;

        Player2 = GameObject.FindGameObjectsWithTag("Player").Last().GetComponent<NetPlayerStateManager>().gameObject;


        if (Player1 != null && Player2 != null) {
            _playerBox = Player1.GetComponentsInChildren<BoxCollider2D>()
                .First(c => c.CompareTag("MovementCollider"));

            _player2Box = Player2.GetComponentsInChildren<BoxCollider2D>()
                .First(c => c.CompareTag("MovementCollider"));

            _playerAnimation = Player1.GetComponentsInChildren<Animator>().First(c => c.name == "SpriteRenderer");
            _playerAnimation2 = Player2.GetComponentsInChildren<Animator>().First(c => c.name == "SpriteRenderer");
            _playerHeadBox = Player1.GetComponentsInChildren<BoxCollider2D>().First(c => c.name == "Head");
            _playerHeadBox2 = Player2.GetComponentsInChildren<BoxCollider2D>().First(c => c.name == "Head");
        }
    }

    private void MoveCamera() {
        var higherHeadBox = Player1.transform.position.y > Player2.transform.position.y
            ? _playerHeadBox
            : _playerHeadBox2;

        var destination = new Vector3
        (
            Mathf.Clamp((Player1.transform.position.x + Player2.transform.position.x) / 2, _minX, _maxX),
            Mathf.Clamp(higherHeadBox.transform.position.y + higherHeadBox.offset.y + higherHeadBox.size.y / 2
                        - _camera.orthographicSize + 0.5f, _minY, _maxY), // 0.5f 因为跳起头会少一点
            transform.position.z
        );


        if ((destination - transform.position).magnitude < 0.1f) {
            return; // 防止轻微推动镜头
        }

        transform.position = Vector3.Lerp(transform.position, destination, FollowSpeed * Time.deltaTime);
    }

    private void ChangeCameraSize() {
        var playersMaxSize = DistenceXToCameraSize(GetPlayersMaxWidth());

        switch (_type) {

            case CameraSizeType.SMALL:

                // 如果是Small态的话
                // 最大size > playersMaxSize > 最小size => size = distence
                // playersMaxSize > 最大 => 切换Large态，变为最大size
                // playersMaxSize < 最小 => 变为最小size不变
                // 只要有一个玩家为Jump则要平滑转为Large态
                if (_playerAnimation.GetBool(AnimatorBool.JUMP) || _playerAnimation2.GetBool(AnimatorBool.JUMP)) {
                    if (_camera.orthographicSize < _cameraMaxSize) {
                        _camera.orthographicSize += 0.1f;
                    } else {
                        _camera.orthographicSize = _cameraMaxSize;
                        _type = CameraSizeType.LARGE;
                    }
                } else {

                    if (playersMaxSize <= _cameraMinSize) {
                        _camera.orthographicSize = _cameraMinSize;
                    } else if (playersMaxSize >= _cameraMaxSize) {
                        _type = CameraSizeType.LARGE;
                        _camera.orthographicSize = _cameraMaxSize;
                    } else {
                        _camera.orthographicSize = playersMaxSize;
                    }
                }

                break;
            case CameraSizeType.LARGE:

                // 如果是large的话
                // playersMaxSize < 最小 => 开始计时 > 1s => 变为最小size（平滑改变）
                // 如果有jump不变
                if (_playerAnimation.GetBool(AnimatorBool.JUMP) || _playerAnimation2.GetBool(AnimatorBool.JUMP)) {
                    _camera.orthographicSize = _cameraMaxSize;
                    _cameraSizeTimer = 0.3f;
                } else {
                    if (playersMaxSize <= _cameraMinSize) {
                        if (_cameraSizeTimer <= 0) {
                            if (_camera.orthographicSize > _cameraMinSize) {
                                _camera.orthographicSize -= 0.05f;
                            } else {
                                _camera.orthographicSize = _cameraMinSize;
                                _type = CameraSizeType.SMALL;
                            }
                        } else {
                            _cameraSizeTimer -= Time.deltaTime;
                        }
                    } else {
                        _cameraSizeTimer = 1f;
                    }
                }

                break;
            default:

                throw new ArgumentOutOfRangeException();
        }

    }

    private void LimitCameraPosition() {

        var backgroundLeft = Background.bounds.max.x - 2;
        var backgroundRight = Background.bounds.min.x + 2;
        var backgroundUp = Background.bounds.max.y;
        var backgroundDown = Background.bounds.min.y + 0.5f; // 会露出一点下边不好

        _maxX = backgroundLeft - _camera.orthographicSize * 1920 / 1080;
        _minX = backgroundRight + _camera.orthographicSize * 1920 / 1080;
        _maxY = backgroundUp - _camera.orthographicSize;
        _minY = backgroundDown + _camera.orthographicSize;
    }

    /**
     * 得到两个角色的最大宽度，距离+玩家边框一半
     */
    private float GetPlayersMaxWidth() {
        var distence = Mathf.Abs(Player1.transform.position.x - Player2.transform.position.x);

        var playersWidth = distence + _playerBox.bounds.extents.x + _player2Box.bounds.extents.x;

        return playersWidth + 2f; // 给一点空间
    }

    /**
     * 将X距离换算成相机的大小，游戏默认是1920x1080的比例
     */
    private float DistenceXToCameraSize(float distence) {
        return distence * 1080 / 1920 / 2;
    }
}