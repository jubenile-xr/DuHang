using UnityEngine;

public class tutorialInstantiate : MonoBehaviour
{
    private enum GameCharacter
    {
        BIRD,
        RABBIT,
        MOUSE,
        PANDA
    }
    [SerializeField] private GameCharacter character;
    private GameObject player;
    private GameObject camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        switch (character)
        {
            //各キャラの生成
            case GameCharacter.BIRD:
                player = Instantiate(Resources.Load<GameObject>("Player/BirdPlayer"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                GameObject eyePos = player.transform.Find("eyePos").gameObject;
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/BirdCameraRig"), eyePos.transform.position, Quaternion.identity);
                player.GetComponent<BirdMoveController>().SetCenterEyeAnchor(camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                break;
            case GameCharacter.RABBIT:
                player = Instantiate(Resources.Load<GameObject>("Player/RabbitPlayer"), new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/RabbitCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.MOUSE:
                player = Instantiate(Resources.Load<GameObject>("Player/MousePlayer"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/MouseCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case GameCharacter.PANDA:
                player = Instantiate(Resources.Load<GameObject>("Player/PandaPlayer"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            // Add other cases for MOUSE and PANDA as needed
        }

        //カメラの親子関係を設定
        camera.transform.SetParent(player.transform);

        //CreatePhotonAvatarのOnCreate()を実行
        CreatePhotonAvatar avatarScript = player.GetComponent<CreatePhotonAvatar>();
        if (avatarScript == null)
        {
            Debug.LogError("CreatePhotonAvatar script is missing on the instantiated player object!");
            return;
        }
        avatarScript.ExecuteCreatePhotonAvatar();

        switch (character)
        {
            case GameCharacter.PANDA:
                CanvasCameraSetter.Instance.SetCanvasCamera();
                CanvasCameraSetter.Instance.SetCanvasSortingLayer();
                break;
            case GameCharacter.MOUSE:
                MouseMove mouseMoveScript = player.GetComponentInChildren<MouseMove>();
                if (mouseMoveScript == null)
                {
                    Debug.LogError("MouseMove script is missing on the instantiated player object!");
                    return;
                }
                mouseMoveScript.SetMouseOVRCameraRig();
                break;
            case GameCharacter.RABBIT:
                RabbitMove rabbitMoveScript = player.GetComponentInChildren<RabbitMove>();
                if (rabbitMoveScript == null)
                {
                    Debug.LogError("RabbitMove script is missing on the instantiated player object!");
                    return;
                }
                rabbitMoveScript.SetRabbitOVRCameraRig();
                break;
            case GameCharacter.BIRD:
                // BIRD用の処理があれば追加
                break;
            default:
                Debug.LogWarning("未処理のキャラクタータイプです: " + character);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
