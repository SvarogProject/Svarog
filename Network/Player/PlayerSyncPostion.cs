using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/**
 *  角色位置同步，更加平滑（应对网络延迟）
 */
[NetworkSettings (channel = 0, sendInterval = 0.01f)]
public class PlayerSyncPostion : NetworkBehaviour {
    [SyncVar(hook = "SyncPositionValue")] 
    private Vector3 _syncPos;
    [SerializeField] 
    private Transform _mTransform;
    private float _lerpRate;
    private float _normalLerpRate = 25;
    private float _fasterLerpRate = 50; // 分两个速度是为了但历史点过多的时候加快切换速度

    private Vector3 _lastPos;
    private float threshold = 0.2f;

    // 显示延迟
    private NetworkClient _nClient;
    private int _latency;
    private Text _latercyText;

    private List<Vector3> _syncPosList = new List<Vector3>(); // 用来保存同步点的历史，顺序播放过去会比直接目标点lerp平滑
    [SerializeField] 
    private bool _useHistoricalLerping = false;
    
    private float _closeEnough = 0.1f; 

    private void Start() {
        // 显示延迟
        //_nClient = GameObject.Find("NetworkManager").GetComponent<NetworkManager>().client;
        //_latercyText = GameObject.Find("Latency Text").GetComponent<Text>();
        _lerpRate = _normalLerpRate;
    }

    private void Update() {
        // lerp的位移每次位移离目标点的30%，放在Update中好些
        LerpPosition();
        // 显示延迟
        //ShowLatency();
    }

    private void FixedUpdate() {
        TransmitPosition();
    }

    private void LerpPosition() {
        if (!isLocalPlayer) { // 本地用户的位移自己可以做主，所以只移动其他用户
            if (_useHistoricalLerping) {
                HistoricalLerping();
            } else {
                OrdinaryLerping();
            }        
        }
    }
    
    [ClientCallback]
    private void TransmitPosition() {
        if (isLocalPlayer && Vector3.Distance(_mTransform.position, _lastPos) > threshold) {
            CmdProvidePosToServer(_mTransform.position);
            _lastPos = _mTransform.position;
        }
    }

    [Command]
    private void CmdProvidePosToServer(Vector3 pos) {
        _syncPos = pos;
    }

    private void ShowLatency() {
        if (isLocalPlayer) {
            _latency = _nClient.GetRTT();
            _latercyText.text = _latency.ToString();
        }
    }

    private void OrdinaryLerping() {
        _mTransform.position = Vector3.Lerp(_mTransform.position, _syncPos, Time.deltaTime * _lerpRate);
    }

    private void HistoricalLerping() {       
        if (_syncPosList.Count > 0) {
            _mTransform.position = Vector3.Lerp(_mTransform.position, _syncPosList[0], Time.deltaTime * _lerpRate);

            if (Vector3.Distance(_mTransform.position, _syncPosList[0]) < _closeEnough) { // 足够近了，到达某个历史点了
                _syncPosList.RemoveAt(0);
            }

            if (_syncPosList.Count > 10) {
                _lerpRate = _fasterLerpRate;
            } else {
                _lerpRate = _normalLerpRate;
            }
        }
    }

    [Client]
    private void SyncPositionValue(Vector3 latestPos) {
        _syncPos = latestPos;
        _syncPosList.Add(latestPos);
        
        Debug.Log(_syncPosList.Count);
    }
}