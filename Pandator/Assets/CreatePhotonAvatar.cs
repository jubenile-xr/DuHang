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
    private GameObject[] rootTargets = new GameObject[7];
    private GameObject _stateManager;
    //private NetworkStateManagerSample _manager;
    //private IsGrabStateManage _isGrabStateManage;
    private CharacterController _controller;

    //[SerializeField] private XRType _xRType;

    [SerializeField] GameObject[] Targets = new GameObject[4];

    private bool isCreated = false;


    private void OnCreate()
    {
        masterPlayerObject = GameObject.FindGameObjectWithTag("MasterPlayer");
        rootTargets[0] = GameObject.FindGameObjectWithTag("CameraRig");
        rootTargets[1] = GameObject.FindGameObjectWithTag("TrackingSpace");
        //rootTargets[2] = GameObject.FindGameObjectWithTag("MainCamera");
        rootTargets[2] = GameObject.FindGameObjectWithTag("LHandTargetAnchor");
        rootTargets[3] = GameObject.FindGameObjectWithTag("RHandTargetAnchor");
        //_stateManager = GameObject.FindGameObjectWithTag("StateManager");
        //_manager = _stateManager.GetComponent<NetworkStateManagerSample>();
        //_isGrabStateManage = _stateManager.GetComponent<IsGrabStateManage>();
        _controller = masterPlayerObject.GetComponent<CharacterController>();
        
        for(int i = 0; i < 5; i++)
        {
            if(rootTargets[i] == null)
            {
                Debug.Log(i);
                Debug.LogError("Target is not found");
            }
        }
        isCreated = true; 
    }

    private void OnEnable()
    {
        if(photonView.IsMine)
        {
            OnCreate();
        }
    }

    void Update()
    {
        if(isCreated)
        {
            this.transform.position = masterPlayerObject.transform.position;
                this.transform.rotation = masterPlayerObject.transform.rotation;
                for (int i = 0; i < 5; i++)
                {
                    Targets[i].transform.localPosition = rootTargets[i].transform.localPosition;
                    Targets[i].transform.localRotation = rootTargets[i].transform.localRotation;
                }
        }
    }

    private void Start()
    {
        /*this.UpdateAsObservable()
            .Where(_ => photonView.IsMine && Input.GetKeyDown(KeyCode.Z) && isCreated)
            .Subscribe(_ =>
            {
                _manager.AddCount(1);
            }).AddTo(this);*/
    }
}