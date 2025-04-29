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

    // クラウドストレージ用の設定
    private const string CLOUD_ANCHOR_UUID_KEY = "cloudAnchorUUID";

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
                OnSaveCloudButtonPressed();
            }
            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                CreateAnchor();
            }
            if (OVRInput.GetDown(OVRInput.Button.Three))
            {
                OnDeleteCloudButtonPressed();
            }
            if (OVRInput.GetDown(OVRInput.Button.Four))
            {
                OnLoadCloudButtonPressed();
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

    public void OnDeleteCloudButtonPressed()
    {
        // 作られていない時は削除しない
        if (!_spatialAnchor) return;
        _message.text = "deleting...";
        _spatialAnchor.Erase((anchor, success) =>
        {
            if (success)
            {
                _message.text = "Successfully deleted anchor.";
                // Photonから共有UUIDを削除
                if (PhotonNetwork.InRoom)
                {
                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props.Add(CLOUD_ANCHOR_UUID_KEY, null);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);
                }
            }
            else
            {
                _message.text = "Failed to delete anchor.";
            }
            isCreated = false;
        });
    }

    public async void OnSaveCloudButtonPressed()
    {
        if (!_spatialAnchor) return;
        _message.text = "Saving to cloud...";

        // アンカーをクラウドに保存
        var saveOptions = new OVRSpatialAnchor.SaveOptions();
        saveOptions.Storage = OVRSpace.StorageLocation.Cloud;
        _spatialAnchor.Save(saveOptions, (anchor, success) =>
        {
            if (success)
            {
                _isSaved = true;
                _message.text = "Successfully saved anchor to cloud.";

                // Photonのカスタムプロパティとしてアンカーのuuidを共有
                if (PhotonNetwork.InRoom)
                {
                    string anchorUUID = anchor.Uuid.ToString();
                    Debug.Log("Sharing Anchor UUID via Photon: " + anchorUUID);

                    ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
                    props.Add(CLOUD_ANCHOR_UUID_KEY, anchorUUID);
                    PhotonNetwork.CurrentRoom.SetCustomProperties(props);

                    // ローカルにも保存
                    PlayerPrefs.SetString(_uniqueKey, anchorUUID);
                }
            }
            else
            {
                _isSaved = false;
                _message.text = "Failed to save anchor to cloud.";
                return;
            }
        });
    }

    public void OnLoadCloudButtonPressed()
    {
        string savedUuid = "";

        // Photonのカスタムプロパティからuuidを取得
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(CLOUD_ANCHOR_UUID_KEY) &&
            PhotonNetwork.CurrentRoom.CustomProperties[CLOUD_ANCHOR_UUID_KEY] != null)
        {
            savedUuid = PhotonNetwork.CurrentRoom.CustomProperties[CLOUD_ANCHOR_UUID_KEY].ToString();
            Debug.Log("Retrieved Anchor UUID from Photon: " + savedUuid);
        }
        else
        {
            // フォールバックとしてプレイヤープレフから取得
            savedUuid = PlayerPrefs.GetString(_uniqueKey, "");
        }

        if (string.IsNullOrEmpty(savedUuid))
        {
            _isSaved = false;
            _message.text = "No stored cloud anchor UUID found.";
            return;
        }

        // Load Optionの作成
        var uuids = new Guid[1] { new Guid(savedUuid) };
        var loadOptions = new OVRSpatialAnchor.LoadOptions
        {
            Timeout = 0,
            StorageLocation = OVRSpace.StorageLocation.Cloud,
            Uuids = uuids
        };
        LoadAnchors(loadOptions);
    }

    private void LoadAnchors(OVRSpatialAnchor.LoadOptions options)
    {
        // クラウドからアンカーをUUIDを指定して読み込み
        _message.text = "Loading anchor from cloud...";
        OVRSpatialAnchor.LoadUnboundAnchors(options, anchors =>
        {
            if (anchors.Length != 1)
            {
                // アンカーが読めなかった
                _message.text = "Failed to load anchor from cloud.";
                return;
            }
            if (anchors[0].Localized)
            {
                // すでにローカライズが終了していた場合
                OnLocalized(anchors[0], true);
            }
            else if (!anchors[0].Localizing)
            {
                // 空間マッピングが不十分などの理由でローカライズに失敗している場合、再度ローカライズ
                anchors[0].Localize(OnLocalized);
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
        _message.text = "Successfully loaded anchor from cloud.";
        isCreated = true;

        // アンカーが正常に読み込まれたことをコールバックで通知
        OnAnchorLoaded?.Invoke();
    }

    // ローカルボタン処理（後方互換性のために残しておく）
    public void OnSaveLocalButtonPressed()
    {
        if (!_spatialAnchor) return;
        _message.text = "Saving locally...";

        // アンカーをローカルに保存
        var saveOptions = new OVRSpatialAnchor.SaveOptions();
        saveOptions.Storage = OVRSpace.StorageLocation.Local;
        _spatialAnchor.Save(saveOptions, (anchor, success) =>
        {
            if (success)
            {
                _isSaved = true;
                _message.text = "Successfully saved anchor locally.";
                // SaveしたアンカーのUUIDを覚えておく
                PlayerPrefs.SetString(_uniqueKey, anchor.Uuid.ToString());
            }
            else
            {
                _isSaved = false;
                _message.text = "Failed to save anchor locally.";
                return;
            }
        });
    }

    public void OnLoadLocalButtonPressed()
    {
        var savedUuid = PlayerPrefs.GetString(_uniqueKey, "");
        if (savedUuid == "")
        {
            _isSaved = false;
            _message.text = "No stored local data found.";
            return;
        }

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
}
