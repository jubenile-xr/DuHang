using Photon.Pun;
using UnityEngine;

public class Net : MonoBehaviourPun
{
    private float time = 0.0f;
    [SerializeField]private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private Animator animator;
    private GameManager gameManager;

    //一度当たったらonにする
    private bool isCollision = false;
    [SerializeField]private string targetTag;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameManager")?.GetComponent<GameManager>();

        animator.SetTrigger("Capture");
    }

    private void Update()
    {
        animator.SetTrigger("Idle");

        // 3秒後に消える
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(isCollision) return;
        GameObject player = collision.gameObject;

        if(player.CompareTag("Player"))
        {
            isCollision = true;
            //速度を0に
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

            // プレイヤーオブジェクトがPhotonViewを持っているか確認
            PhotonView playerPhotonView = player.GetComponent<PhotonView>();
            if (playerPhotonView != null)
            {
                // プレイヤーのIDを取得
                int playerViewID = playerPhotonView.ViewID;

                // RPC関数を呼び出して全プレイヤーに死亡状態の更新を通知
                photonView.RPC("UpdatePlayerDeadStatus", RpcTarget.AllBuffered, playerViewID);
            }
            else
            {
                // PhotonViewがない場合は直接処理
                UpdateDeadStatusDirectly(player);
            }

            Debug.Log("Player hit by net: " + player.name);
        }
    }

    // RPCでネットワーク同期された死亡状態更新処理
    [PunRPC]
    private void UpdatePlayerDeadStatus(int playerViewID)
    {
        // プレイヤーIDからプレイヤーオブジェクトを検索
        PhotonView hitPlayerView = PhotonView.Find(playerViewID);
        if (hitPlayerView != null)
        {
            GameObject hitPlayer = hitPlayerView.gameObject;
            UpdateDeadStatusDirectly(hitPlayer);
        }
    }

    // プレイヤーの死亡状態を更新する
    private void UpdateDeadStatusDirectly(GameObject player)
    {
        // Make sure we have a GameManager
        if(gameManager == null)
        {
            gameManager = GameObject.FindWithTag("GameManager")?.GetComponent<GameManager>();
            if(gameManager == null)
            {
                Debug.LogError("GameManager not found in Net collision!");
                return;
            }
        }

        // Get the player's StateManager
        StateManager stateManager = player.GetComponent<StateManager>();
        if(stateManager != null)
        {
            // プレイヤー名を取得
            string playerName = stateManager.GetPlayerName();
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogWarning("Player name is null or empty");
                return;
            }

            // すでに死んでいないか確認
            bool isAlreadyDead = IsPlayerAlreadyDead(playerName);
            if (!isAlreadyDead)
            {
                // GameManagerの死亡状態を更新（これがマスターデータ）
                UpdateGameManagerDeadStatus(playerName);

                // StateManagerにも死亡を通知（表示の更新などStateManagerの責任に任せる）
                stateManager.SetAlive(false);
            }
        }
        else
        {
            // StateManagerがない場合は名前でプレイヤーを特定
            string[] playerNames = gameManager.GetAllPlayerNames();
            for (int i = 0; i < playerNames.Length; i++)
            {
                if (player.name.Contains(playerNames[i]))
                {
                    if (!gameManager.GetPlayerDeadStatus()[i])
                    {
                        gameManager.SetPlayerDeadStatusTrue(i);
                        gameManager.UpdateAliveCountFromDeadStatus();
                    }
                    break;
                }
            }
        }
    }

    // プレイヤーがすでに死亡状態かチェック
    private bool IsPlayerAlreadyDead(string playerName)
    {
        string[] playerNames = gameManager.GetAllPlayerNames();
        bool[] playerDeadStatus = gameManager.GetPlayerDeadStatus();

        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Contains(playerName) || playerName.Contains(playerNames[i]))
            {
                return playerDeadStatus[i];
            }
        }
        return false;
    }

    // GameManagerの死亡状態を更新
    private void UpdateGameManagerDeadStatus(string playerName)
    {
        string[] playerNames = gameManager.GetAllPlayerNames();
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Contains(playerName) || playerName.Contains(playerNames[i]))
            {
                gameManager.SetPlayerDeadStatusTrue(i);
                gameManager.UpdateAliveCountFromDeadStatus();
                break;
            }
        }
    }
}
