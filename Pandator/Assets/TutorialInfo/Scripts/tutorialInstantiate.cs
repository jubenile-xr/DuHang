using UnityEngine;

public class tutorialInstantiate : MonoBehaviour
{
    private GameObject player;
    private GameObject camera;
    public GameObject roomPrefab;
    public GameObject passthrough;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        switch (Character.GetSelectedAnimal())
        {
            //各キャラの生成
            case Character.GameCharacters.BIRD:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialBird"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                GameObject eyePos = player.transform.Find("eyePos").gameObject;
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"), eyePos.transform.position, Quaternion.identity);
                player.GetComponent<BirdMoveController>().SetCenterEyeAnchor(camera.transform.Find("TrackingSpace/CenterEyeAnchor").transform);
                break;
            case Character.GameCharacters.RABBIT:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialRabbit"), new Vector3(0f, 2.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case Character.GameCharacters.MOUSE:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialMouse"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("TutorialCameraRig/TutorialCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            case Character.GameCharacters.PANDA:
                player = Instantiate(Resources.Load<GameObject>("TutorialPlayer/TutorialPanda"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                camera = Instantiate(Resources.Load<GameObject>("CameraRig/PandaCameraRig"), new Vector3(0f, 1.0f, 0f), Quaternion.identity);
                break;
            // Add other cases for MOUSE and PANDA as needed
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
}
