using UnityEngine;

// インスペクターで選択するEnumを定義
public enum ResultPlayerType
{
    Panda,
    Bird,
    Rabbit,
    Mouse
}

public class ResultSceneInitialize : MonoBehaviour
{
    // インスペクターからEnumで選択可能にする
    [SerializeField]
    private ResultPlayerType playerType = ResultPlayerType.Panda;

    // インスタンス化する位置
    [SerializeField]
    private Vector3 spawnPosition = new Vector3(203.0545f, -154f, 342.9874f);

    // インスタンス化する回転（オイラー角）
    [SerializeField]
    private Vector3 spawnEulerAngles = new Vector3(0f, 183.299f, 0f);

    // インスタンス化するスケール
    [SerializeField]
    private Vector3 spawnScale = new Vector3(100f, 100f, 100f);

    // 回転速度 (度/秒)
    [SerializeField]
    private float rotationSpeed = 30f;

    // 生成したプレハブの参照
    private GameObject spawnedInstance;

    void Start()
    {
        // Enumの値からResourcesフォルダ内のプレハブパスを決定
        string prefabPath = "";
        switch (playerType)
        {
            case ResultPlayerType.Panda:
                prefabPath = "ResultPlayer/ResultPanda";
                break;
            case ResultPlayerType.Bird:
                prefabPath = "ResultPlayer/ResultBird";
                break;
            case ResultPlayerType.Rabbit:
                prefabPath = "ResultPlayer/ResultRabbit";
                break;
            case ResultPlayerType.Mouse:
                prefabPath = "ResultPlayer/ResultMouse";
                break;
            default:
                Debug.LogError("不明なResultPlayerType");
                return;
        }

        // Resourcesからプレハブをロード
        GameObject prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Prefabが見つかりません: " + prefabPath);
            return;
        }

        // 回転をQuaternionに変換
        Quaternion rotation = Quaternion.Euler(spawnEulerAngles);

        // プレハブのインスタンス生成
        spawnedInstance = Instantiate(prefab, spawnPosition, rotation);
        spawnedInstance.transform.localScale = spawnScale;
    }

    void Update()
    {
        if (spawnedInstance != null)
        {
            // 毎フレーム、y軸回りに回転させる
            spawnedInstance.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}