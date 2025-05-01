using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Photon.Pun;

public class AnchorManager : MonoBehaviourPunCallbacks
{
    private OVRSpatialAnchor _spatialAnchor;
    private System.Guid _uuid;
    // private  _anchorIcon;
    [SerializeField]
    private TextMeshProUGUI _message;
    [SerializeField]
    private TextMeshProUGUI _textPosition;
    [SerializeField]
    private TextMeshProUGUI _textRotation;
    public bool isCreated { get; private set; }
    private bool _isSaved;
    [SerializeField]
    private string _uniqueKey;
    public GameObject roomPrefab;

    public delegate void AnchorLoadedCallback();
    public event AnchorLoadedCallback OnAnchorLoaded;

    void Update()
    {
        // デバッグモード
        if (DebugManager.GetDebugMode())
        {
            Vector3 pos = gameObject.transform.position;
            Vector3 rot = gameObject.transform.eulerAngles;
            _textPosition.text = pos.ToString();
            _textRotation.text = rot.ToString();

            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                OnSaveLocalButtonPressed();
            }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                CreateAnchor();
            }
            if (OVRInput.GetDown(OVRInput.Button.Three))
            {
                OnDeleteLocalButtonPressed();
            }
            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                OnLoadLocalButtonPressed();
            }
            // エディットモード切り替え
            if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick))
            {
                ToggleEditMode();
            }
            // 最終確定
            if (OVRInput.GetDown(OVRInput.Button.SecondaryThumbstick))
            {
                FinalizeAnchor();
            }
       }
    }

    public async void CreateAnchor()
    {
        var token = this.GetCancellationTokenOnDestroy();

        // Anchorの作成
        _message.text = " Start creating the anchor....";
        // コンポーネントが既にあれば削除
        if (_spatialAnchor)
        {
            Destroy(_spatialAnchor);
        }
        _spatialAnchor = gameObject.AddComponent<OVRSpatialAnchor>();
        // Anchorの作成待ち(※UniTask使用)
        await UniTask.WaitUntil(() => !_spatialAnchor || _spatialAnchor.Created);

        if (_spatialAnchor)
        {
            // 作成に成功したらuuidを保管する
            _uuid = _spatialAnchor.Uuid;
            //if (_anchorIcon != null)
            if (true)
            {
                _message.text = "Successfully created anchor.";
                isCreated = true;
            }
        }
        else
        {
            //if (_anchorIcon != null)
            if (true)
            {
                _message.text = "Failed to create anchor.";
                isCreated = false;
            }
        }
    }

    public void OnDeleteLocalButtonPressed()
    {
        // 作られていない時は削除しない
        if (!_spatialAnchor) return;
        _message.text = "deleting...";
        _spatialAnchor.Erase((anchor, success) =>
        {
            if (success)
            {
                _message.text = "Successfully deleted anchor.";
                // PlayerPrefsからも削除
                PlayerPrefs.DeleteKey(_uniqueKey);
            }
            else
            {
                _message.text = "Failed to delete anchor.";
            }
            isCreated = false;
        });
    }

    public async void OnSaveLocalButtonPressed()
    {
        if (!_spatialAnchor) return;
        _message.text = "Saving to local...";

        // アンカーをローカルに保存
        var saveOptions = new OVRSpatialAnchor.SaveOptions();
        saveOptions.Storage = OVRSpace.StorageLocation.Local;
        _spatialAnchor.Save(saveOptions, (anchor, success) =>
        {
            if (success)
            {
                _isSaved = true;
                _message.text = "Successfully saved anchor to local.";

                // ローカルにUUIDを保存
                string anchorUUID = anchor.Uuid.ToString();
                Debug.Log("Saving Anchor UUID to PlayerPrefs: " + anchorUUID);
                PlayerPrefs.SetString(_uniqueKey, anchorUUID);
                PlayerPrefs.Save();
            }
            else
            {
                _isSaved = false;
                _message.text = "Failed to save anchor to local.";
                return;
            }
        });
    }

    public void OnLoadLocalButtonPressed()
    {
        string savedUuid = "";

        // PlayerPrefsからUUIDを取得
        savedUuid = PlayerPrefs.GetString(_uniqueKey, "");
        Debug.Log("Trying to load from PlayerPrefs with key " + _uniqueKey + ": " + savedUuid);

        if (string.IsNullOrEmpty(savedUuid))
        {
            _isSaved = false;
            _message.text = "No stored local anchor UUID found.";
            Debug.LogWarning("No stored local anchor UUID found.");
            return;
        }

        // ステータスをリセット
        isCreated = false;

        try
        {
            // Load Optionの作成
            var uuids = new Guid[1] { new Guid(savedUuid) };
            var loadOptions = new OVRSpatialAnchor.LoadOptions
            {
                Timeout = 0,
                StorageLocation = OVRSpace.StorageLocation.Local,
                Uuids = uuids
            };
            LoadAnchors(loadOptions);
        }
        catch (System.Exception e)
        {
            _message.text = "Error loading anchor: " + e.Message;
            Debug.LogError("Error loading anchor: " + e.Message);
        }
    }

    private void LoadAnchors(OVRSpatialAnchor.LoadOptions options)
    {
        // ローカルからアンカーをUUIDを指定して読み込み
        _message.text = "Loading anchor from local...";
        Debug.Log("Loading anchor from local...");
        OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
        {
            if (anchors.Length != 1)
            {
                // アンカーが読めなかった
                _message.text = "Failed to load anchor from local.";
                Debug.LogError("Failed to load anchor from local.");
                return;
            }
            if (anchors[0].Localized)
            {
                // すでにローカライズが終了していた場合
                OnLocalized(anchors[0], true);
                Debug.Log("Anchor is already localized.");
            }
            else if (!anchors[0].Localizing)
            {
                // 空間マッピングが不十分などの理由でローカライズに失敗している場合、再度ローカライズ
                anchors[0].Localize(OnLocalized);
                Debug.Log("Localizing anchor...");
            }
        });
    }

    private void OnLocalized(OVRSpatialAnchor.UnboundAnchor unboundAnchor, bool success)
    {
        if (!success)
        {
            // アンカーが読めなかった
            _message.text = "Failed to load anchor.";
            return;
        }
        // アンカーのGame Objectを読み取った位置に移動し、アンバインドのアンカーをコンポーネントにバインドする
        var pose = unboundAnchor.Pose;
        transform.SetPositionAndRotation(pose.position, pose.rotation);
        if (!_spatialAnchor)
        {
            _spatialAnchor = gameObject.AddComponent<OVRSpatialAnchor>();
        }
        unboundAnchor.BindTo(_spatialAnchor);
        _message.text = "Successfully loaded anchor from local.";
        Debug.Log("Successfully loaded anchor from local.");
        isCreated = true;

        // アンカーが正常に読み込まれたことをコールバックで通知
        OnAnchorLoaded?.Invoke();
    }

    public void ToggleEditMode()
    {
        if (!_spatialAnchor) return;

        if (_spatialAnchor.enabled)
        {
            // 編集モードにする
            _spatialAnchor.enabled = false;
            _message.text = "Edit mode: Object can be moved";
        }
        else
        {
            // 再度固定する
            _spatialAnchor.enabled = true;
            _message.text = "Fixed mode: Object is anchored";
        }
    }

    public void FinalizeAnchor()
    {
        if (!_spatialAnchor) return;

        // まず現在位置にアンカーを再作成
        Destroy(_spatialAnchor);
        CreateAnchor();

        // 作成後保存
        OnSaveLocalButtonPressed();
    }

    public void OnResetRotXButtonPressed()
    {
        float rotY = gameObject.transform.eulerAngles.y;
        float rotZ = gameObject.transform.eulerAngles.z;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, rotZ));
    }

    public void OnResetRotYButtonPressed()
    {
        float rotX = gameObject.transform.eulerAngles.x;
        float rotZ = gameObject.transform.eulerAngles.z;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(rotX, 0, rotZ));
    }

    public void OnResetRotZButtonPressed()
    {
        float rotX = gameObject.transform.eulerAngles.x;
        float rotY = gameObject.transform.eulerAngles.y;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY, 0));
    }

    public void OnRotYButtonPressed()
    {
        float rotX = gameObject.transform.eulerAngles.x;
        float rotY = gameObject.transform.eulerAngles.y;
        float rotZ = gameObject.transform.eulerAngles.z;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(rotX, rotY+45f, rotZ));
    }

    // 外部からアンカーロード処理を呼び出せるようにパブリックメソッドを追加
    public void LoadAnchorFromExternal()
    {
        OnLoadLocalButtonPressed();
    }

    // アンカーの位置と回転を取得するメソッド
    public Transform GetAnchorTransform()
    {
        if (_spatialAnchor && isCreated)
        {
            return transform;
        }
        return null;
    }
}
