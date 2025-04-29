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
            .Where(_ => _allAnchors.All(anchor => anchor.isCreated))
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
            anchor.OnLoadCloudButtonPressed();


            // 物体を動かすためにコンポーネントを削除
            OVRSpatialAnchor spatialAnchor = anchor.gameObject.GetComponent<OVRSpatialAnchor>();
            Destroy(spatialAnchor);

            // デバッグ時は再調整のために以下を実行しない
            if (_debugMode) return;

            // Anchor位置調整用のコライダを削除
            BoxCollider collider = anchor.gameObject.GetComponent<BoxCollider>();
            Destroy(collider);

            // 子オブジェクトのPokeable Canvasを非アクティブにする．
            Transform tmp = anchor.gameObject.transform.Find("Pokeable Canvas");
            if (tmp == null) {
                Debug.Log(anchor.gameObject.name + "のPokeable Canvasが見つかりません．");
            }
            else
            {
                GameObject pokeableCanvas = tmp.gameObject;
                pokeableCanvas.SetActive(false);
            }
        }
    }
}
