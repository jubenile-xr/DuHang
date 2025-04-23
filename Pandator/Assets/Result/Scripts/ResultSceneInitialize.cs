using UnityEngine;


public class ResultSceneInitialize : MonoBehaviour
{
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
    [SerializeField]private float rotationSpeed = 30f;
    [Header("GODかどうか")]
    [SerializeField] private bool isGod = false; // GODかどうかのフラグ
    // プレハブのインスタンスを格納する変数
    [Header("GODの時だけ名前をいれる")]
    [SerializeField] private string prefabName = "Panda";

    // 生成したプレハブの参照
    private GameObject spawnedInstance;

    void Start()
    {
        // Enumの値からResourcesフォルダ内のプレハブパスを決定
        string prefabPath = "";
        switch (Character.GetSelectedAnimal())
        {
            case Character.GameCharacters.BIRD:
                prefabPath = "ResultPlayer/ResultBird";
                break;
            case Character.GameCharacters.RABBIT:
                prefabPath = "ResultPlayer/ResultRabbit";
                break;
            case Character.GameCharacters.MOUSE:
                prefabPath = "ResultPlayer/ResultMouse";
                break;
            case Character.GameCharacters.PANDA:
                prefabPath = "ResultPlayer/ResultPanda";
                break;
            default:
                Debug.LogError("不正なキャラクターが選択されました");
                return;
        }

        if (isGod)
        {
            prefabPath = "ResultPlayer/Result" + prefabName;
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