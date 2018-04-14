using System;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerID : NetworkBehaviour {

    [SyncVar] private string _playerUniqueIdentity;
    private NetworkInstanceId _playerNetId;
    private Transform _mTransform;
    
    public override void OnStartLocalPlayer() {
        GetNetIdentity();
        SetIdentity();
    }

    private void Awake() {
        _mTransform = transform;
    }

    private void Update() {
        if (_mTransform.name == "" || _mTransform.name == "NetMasa(Clone)") {
            SetIdentity();
        }
    }

    [Client]
    private void GetNetIdentity() {
        _playerNetId = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    private void SetIdentity() {
        if (!isLocalPlayer) {
            _mTransform.name = _playerUniqueIdentity;
        } else {
            _mTransform.name = MakeUniqueIdentity();
        }
    }

    private string MakeUniqueIdentity() {
        var uniqueIdentity = "Player" + _playerNetId;

        return uniqueIdentity;
    }

    [Command]
    private void CmdTellServerMyIdentity(string uniqueIdentity) {
        _playerUniqueIdentity = uniqueIdentity;
    }
}