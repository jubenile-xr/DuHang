using UnityEngine;

public class CreateTutorialAvatar : MonoBehaviour
{
    private GameObject masterPlayerObject;
    private GameObject[] rootTargets = new GameObject[4];
    [SerializeField] GameObject[] Targets = new GameObject[4];
    private bool isCreated = false;

    // Update is called once per frame
    void Update()
    {
        if (isCreated)
        {
            //HMDの座標・角度を共有モデルに同期
            transform.position = masterPlayerObject.transform.position;
            transform.rotation = masterPlayerObject.transform.rotation;
            for (int i = 0; i < 4; i++)
            {
                Targets[i].transform.position = rootTargets[i].transform.position;
                Targets[i].transform.rotation = rootTargets[i].transform.rotation;
            }
        }
    }

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

        if (rootTargets[0] != null && rootTargets[1] != null && rootTargets[2] != null && rootTargets[3] != null)
        {
            isCreated = true;
        }
    }

    public void ExecuteCreatePhotonAvatar()
    {
        OnCreate();
    }
}
