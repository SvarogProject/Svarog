using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManager : MonoBehaviour {
    public Transform CameraHolder;

    public List<Transform> Players = new List<Transform>();

    private Transform _player1;
    private Transform _player2;

    private Vector3 _middlePoint;

    //public float OrthoMin = 2;
    //public float OrthoMax = 6;

    private Camera _camera;

    #region Singleton

    private static CameraManager _instance;

    public static CameraManager GetInstance() {
        return _instance;
    }

    public void Awake() {
        _instance = this;
    }

    #endregion
    
    public void Start() {
        _camera = CameraHolder.GetComponent<Camera>();
    }

    public void FixedUpdate() {
        /*
        var distance = Vector3.Distance(Players[0].position, Players[1].position);
        var half = distance / 2;

        _middlePoint = (Players[1].position - Players[0].position).normalized * half;
        _middlePoint += Players[0].position;

        _camera.orthographicSize = 2 * (half / 2);

        if (_camera.orthographicSize > OrthoMax) {
            _camera.orthographicSize = OrthoMax;
        }

        if (_camera.orthographicSize < OrthoMin) {
            _camera.orthographicSize = OrthoMin;
        }

        CameraHolder.transform.position =
            Vector3.Lerp(CameraHolder.transform.position, _middlePoint, Time.deltaTime * 5);
       				
        */
				
        var destination = new Vector3
        (
            (Players[0].position.x + Players[1].position.x) / 2,
            (Players[0].position.y + Players[1].position.y) / 2 + 0.5f,
            CameraHolder.transform.position.z
        );

        //平滑移动(线性插值)
        var positionNow = Vector3.Lerp(CameraHolder.transform.position, destination, 5 * Time.deltaTime);

        CameraHolder.transform.position = new Vector3(positionNow.x, positionNow.y, CameraHolder.transform.position.z);
    }
}