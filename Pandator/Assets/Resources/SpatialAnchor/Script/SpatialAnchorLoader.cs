using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class SpatialAnchorLoader : MonoBehaviour
{
    private AnchorManager[] _allAnchors;
    public bool isLoaded { get; private set; } = false;

    public void AnchorLoad()
    {
        this.UpdateAsObservable()
            .Where(_ => _allAnchors != null && _allAnchors.Length > 0 && _allAnchors.All(anchor => anchor.isCreated))
            .First()
            .Subscribe(_ =>
            {
                isLoaded = true;
            }).AddTo(this);
        // Anchorのコンポーネントを持つすべてのGameObjectを取得
        _allAnchors = FindObjectsOfType<AnchorManager>();

        foreach (AnchorManager anchor in _allAnchors)
        {
            // まずアンカーの読み込みを試みる
            anchor.OnLoadLocalButtonPressed();

            // アンカーが読み込めたかチェック（0.5秒待ってから）
            StartCoroutine(CheckAndCreateAnchorIfNeeded(anchor));

            // デバッグ時は再調整のために以下を実行しない
            if (DebugManager.GetDebugMode()) return;

            // Grabbable関連のコンポーネントを削除
            Grabbable grabbable = anchor.gameObject.GetComponent<Grabbable>();
            if (grabbable != null)
            {
                Destroy(grabbable);
            }

        }
    }

    private IEnumerator CheckAndCreateAnchorIfNeeded(AnchorManager anchor)
    {
        // アンカーの読み込みを待機
        yield return new WaitForSeconds(1.0f);

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
