using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SpatialAnchorLoader : MonoBehaviour
{
    private AnchorManager[] _allAnchors;
    [SerializeField]
    private bool _debugMode;
    public bool isLoaded { get; private set; } = false;

    void Awake()
    {
        Load();
    }

    void Start()
    {
        this.UpdateAsObservable()
            .Where(_ => _allAnchors != null && _allAnchors.Length > 0 && _allAnchors.All(anchor => anchor.isCreated))
            .First()
            .Subscribe(_ =>
            {
                isLoaded = true;
            }).AddTo(this);
    }

    private void Load()
    {
        // Anchorのコンポーネントを持つすべてのGameObjectを取得
        _allAnchors = FindObjectsOfType<AnchorManager>();

        foreach (AnchorManager anchor in _allAnchors)
        {
            // まずアンカーの読み込みを試みる
            anchor.OnLoadLocalButtonPressed();

            // アンカーが読み込めたかチェック（0.5秒待ってから）
            StartCoroutine(CheckAndCreateAnchorIfNeeded(anchor));

            // デバッグ時は再調整のために以下を実行しない
            if (_debugMode) return;

            // Anchor位置調整用のコライダを削除
            BoxCollider collider = anchor.gameObject.GetComponent<BoxCollider>();
            Destroy(collider);
        }
    }

    private IEnumerator CheckAndCreateAnchorIfNeeded(AnchorManager anchor)
    {
        // アンカーの読み込みを待機
        yield return new WaitForSeconds(1.0f);

        // アンカーが作成されていなければ新規作成して保存
        if (!anchor.isCreated)
        {
            Debug.Log("No anchor found. Creating a new one...");
            anchor.CreateAnchor();

            // アンカー作成完了を待機
            yield return new WaitForSeconds(1.0f);

            // 作成されたアンカーを保存
            if (anchor.isCreated)
            {
                Debug.Log("New anchor created. Saving...");
                anchor.OnSaveLocalButtonPressed();
            }
        }

        // 物体を動かすためにコンポーネントを削除
        if (anchor.isCreated)
        {
            yield return new WaitForSeconds(0.5f);
            OVRSpatialAnchor spatialAnchor = anchor.gameObject.GetComponent<OVRSpatialAnchor>();
            if (spatialAnchor != null)
            {
                Destroy(spatialAnchor);
            }
        }
    }
}
