using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using StateManage;
//using UniRx;
//using UniRx.Triggers;

public class CreatePhotonAvatar : MonoBehaviourPunCallbacks
{ 
    private GameObject masterPlayerObject;
    private GameObject[] rootTargets = new GameObject[4];
    private GameObject _stateManager;
    //private NetworkStateManagerSample _manager;
    //private IsGrabStateManage _isGrabStateManage;
    private CharacterController _controller;

    //[SerializeField] private XRType _xRType;

    [SerializeField] GameObject[] Targets = new GameObject[4];

    private bool isCreated = false;


    private void OnCreate()
    {
       Debug.Log("Trying to find GameObject with tag 'MasterPlayer'");
        masterPlayerObject = GameObject.FindGameObjectWithTag("MasterPlayer");
        if (masterPlayerObject == null)
        {
            Debug.LogError("MasterPlayer object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            // オブジェクトが見つかった場合は active 状態にする
            masterPlayerObject.SetActive(true);
        }

        rootTargets[0] = GameObject.FindGameObjectWithTag("CameraRig");
        if (rootTargets[0] == null)
        {
            Debug.LogError("CameraRig object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[0].SetActive(true);
        }

        rootTargets[1] = GameObject.FindGameObjectWithTag("TrackingSpace");
        if (rootTargets[1] == null)
        {
            Debug.LogError("TrackingSpace object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[1].SetActive(true);
        }

        // rootTargets[2] を LHandTargetAnchor で取得
        rootTargets[2] = GameObject.FindGameObjectWithTag("LHandTargetAnchor");
        if (rootTargets[2] == null)
        {
            Debug.LogError("LHandTargetAnchor object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[2].SetActive(true);
        }

        // rootTargets[3] を RHandTargetAnchor で取得
        rootTargets[3] = GameObject.FindGameObjectWithTag("RHandTargetAnchor");
        if (rootTargets[3] == null)
        {
            Debug.LogError("RHandTargetAnchor object not found! Check the tag and its active status in the scene.");
        }
        else
        {
            rootTargets[3].SetActive(true);
        }
        //_stateManager = GameObject.FindGameObjectWithTag("StateManager");
        //_manager = _stateManager.GetComponent<NetworkStateManagerSample>();
        //_isGrabStateManage = _stateManager.GetComponent<IsGrabStateManage>();
        _controller = masterPlayerObject.GetComponent<CharacterController>();
        if (rootTargets[0] != null && rootTargets[1] != null && rootTargets[2] != null && rootTargets[3] != null)
        {
            isCreated = true;
        }
    }

    void Update()
    {
        if(isCreated)
        {
            this.transform.position = masterPlayerObject.transform.position;
                this.transform.rotation = masterPlayerObject.transform.rotation;
                for (int i = 0; i < 4; i++)
                {
                    Targets[i].transform.position = rootTargets[i].transform.position;
                    Targets[i].transform.rotation = rootTargets[i].transform.rotation;
                }
        }
    }

    public void ExecuteCreatePhotonAvatar()
    {
        /*this.UpdateAsObservable()
            .Where(_ => photonView.IsMine && Input.GetKeyDown(KeyCode.Z) && isCreated)
            .Subscribe(_ =>
            {
                _manager.AddCount(1);
            }).AddTo(this);*/
        OnCreate();
    }
}