using UnityEngine;
using System.Collections;

public class tutorialInstantiate : MonoBehaviour
{
    private GameObject player;
    private GameObject camera;
    public GameObject roomPrefab;
    public GameObject passthrough;
    public GameObject planePrefab;
    public GameObject OVRSceneManager;
    private GameObject spatialAnchor;
    private bool isAnchorLoadAttempted = false;
    private bool isSpatialAnchorCreated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        switch (Character.GetSelectedAnimal())
        {
            //各キャラの生成
            case Character.GameCharacters.BIRD:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialBird"),
                    new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                GameObject eyePos = player.transform.Find("eyePos").gameObject;
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"),
                    eyePos.transform.position, Quaternion.identity);
                player.GetComponent<BirdMoveController>()
                    .SetCenterEyeAnchor(camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                SetLayerToIgnoreMyself();
                break;
            case Character.GameCharacters.RABBIT:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialRabbit"),
                    new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"),
                    new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                SetLayerToIgnoreMyself();
                break;
            case Character.GameCharacters.MOUSE:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialMouse"),
                    new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"),
                    new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                SetLayerToIgnoreMyself();
                break;
            case Character.GameCharacters.PANDA:
                // PANDAの場合はspatialAnchorを生成
                spatialAnchor = Instantiate(Resources.Load<GameObject>("SpatialAnchor/prefab/spatialAnchor"),
                    new Vector3(0f, 0f, 0f), Quaternion.identity);
                Transform roomCompleteTransform = spatialAnchor.transform.Find("room_complete004");
                roomCompleteTransform.gameObject.SetActive(false);

                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialPanda"),
                    new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 1.0f, 0f),
                    Quaternion.identity);
                break;
        }

        //カメラの親子関係を設定
        camera.transform.SetParent(player.transform);

        //CreatePhotonAvatarのOnCreate()を実行
        CreateTutorialAvatar avatarScript = player.GetComponent<CreateTutorialAvatar>();
        if (avatarScript == null)
        {
            Debug.LogError("CreatePhotonAvatar script is missing on the instantiated player object!");
            return;
        }

        avatarScript.ExecuteCreatePhotonAvatar();

        switch (Character.GetSelectedAnimal())
        {
            case Character.GameCharacters.PANDA:
                roomPrefab.SetActive(false);
                passthrough.SetActive(true);
                planePrefab.SetActive(true);
                OVRSceneManager.SetActive(true);
                break;
            case Character.GameCharacters.MOUSE:
                TutorialMouseMove mouseMoveScript = player.GetComponentInChildren<TutorialMouseMove>();
                if (mouseMoveScript == null)
                {
                    Debug.LogError("MouseMove script is missing on the instantiated player object!");
                    return;
                }

                mouseMoveScript.SetMouseOVRCameraRig();
                break;
            case Character.GameCharacters.RABBIT:
                TutorialRabbitMove rabbitMoveScript = player.GetComponentInChildren<TutorialRabbitMove>();
                if (rabbitMoveScript == null)
                {
                    Debug.LogError("RabbitMove script is missing on the instantiated player object!");
                    return;
                }

                rabbitMoveScript.SetRabbitOVRCameraRig();
                break;
            case Character.GameCharacters.BIRD:
                // BIRD用の処理があれば追加
                break;
            default:
                Debug.LogWarning("未処理のキャラクタータイプです: " + Character.GetSelectedAnimal());
                break;
        }

        CanvasCameraSetter.Instance.SetCanvasCamera();
        CanvasCameraSetter.Instance.SetCanvasSortingLayer();
    }


    private void Update()
    {
        // PANDAプレイヤーの場合のアンカーロード処理
        if (Character.GetSelectedAnimal() == Character.GameCharacters.PANDA && spatialAnchor != null &&
            !isAnchorLoadAttempted)
        {
            AnchorManager anchorManager = spatialAnchor.GetComponent<AnchorManager>();
            if (anchorManager != null)
            {
                LoadAnchorOnce(anchorManager);
            }
            else
            {
                // アンカーマネージャーが見つからない場合も続行
                isAnchorLoadAttempted = true;
                isSpatialAnchorCreated = true;
            }
        }
    }

    private void LoadAnchorOnce(AnchorManager anchorManager)
    {
        if (!isAnchorLoadAttempted)
        {
            isAnchorLoadAttempted = true;
            Debug.Log("Attempting to load anchor in tutorial (First and only attempt)...");
            anchorManager.LoadAnchorFromExternal();
            StartCoroutine(WaitForAnchorLoad(anchorManager));
        }
    }

    private System.Collections.IEnumerator WaitForAnchorLoad(AnchorManager anchorManager)
    {
        float elapsedTime = 0f;
        float timeout = 5.0f; // 5秒のタイムアウト

        while (elapsedTime < timeout && !anchorManager.isCreated)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (anchorManager.isCreated)
        {
            Transform anchorTransform = anchorManager.GetAnchorTransform();
            if (anchorTransform != null)
            {
                Debug.Log($"Tutorial: Anchor loaded successfully! Position: {anchorTransform.position}");
                isSpatialAnchorCreated = true;
            }
        }
        else
        {
            Debug.LogWarning("Tutorial: Failed to load anchor within timeout period. Continuing without anchor.");
        }

        // ロードの成功/失敗に関わらず、次の処理に進むためのフラグを設定
        isAnchorLoadAttempted = true;
        isSpatialAnchorCreated = true;

    }

    private void SetLayerToIgnoreMyself()
        {
            Character.SetLayer(player, LayerMask.NameToLayer("IgnoreMyself"));
            camera.transform.FindChildRecursive("CenterEyeAnchor").GetComponent<Camera>().cullingMask &=
                ~(1 << LayerMask.NameToLayer("IgnoreMyself"));
        }
    }
