using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")]private const float interruptedTime = 3.0f;
    private float time;
    [SerializeField] private PlayerColorManager playerColorManager;
    [Header("ゲームマネージャー")] private GameManager gameManager;
    private GameObject canvasObject;
    [SerializeField] private ScoreManager scoreManager;
    [Header("親オブジェクト操作用"), SerializeField] private GameObject parentObject;
    [SerializeField] private RabbitMove rabbitMove;
    [SerializeField] private RabbitJump rabbitJump;
    [SerializeField] private BirdMoveController birdMoveController;
    [SerializeField] private MouseMove mouseMove;
    private static string PlayerName;
    private static int PlayerNameIndex;
    private enum GameCharacter
    {
        BIRD,
        RABBIT,
        MOUSE
    }
    [Header("キャラクターの種類"), SerializeField] private GameCharacter character;

    void Start()
    {
        isInterrupted = false;
        isAlive = true;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Debug.Log("GameManager: " + gameManager);

        canvasObject = GameObject.FindWithTag("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("GameManager object not found!");
        }
    }

    void Update()
    {
        if (isInterrupted)
        {
            time += Time.deltaTime;
            if (time > interruptedTime)
            {
                ResetState();
            }
        }
    }

    private void ResetState()
    {
        isInterrupted = false;
        time = 0;
        playerColorManager?.ChangeColorOriginal();
        switch (character)
        {
            case GameCharacter.BIRD:
                birdMoveController?.SetMoveSpeedNormal();
                break;
            case GameCharacter.RABBIT:
                rabbitMove?.SetMoveSpeedNormal();
                rabbitJump?.SetJumpSpeedNormal();
                break;
            case GameCharacter.MOUSE:
                mouseMove?.SetMoveSpeedNormal();
                break;
            default:
                Debug.Log("Unknown character type: " + character);
                break;
        }
    }

    public void SetInterrupted(bool value)
    {
        if (value)
        {
            InterruptLogic();
        }
        isInterrupted = value;
    }

    public bool GetInterrupted()
    {
        return isInterrupted;
    }

    public void SetAlive(bool value)
    {
        if (!value)
        {
            DeadLogic();
        }
        isAlive = value;
    }

    public bool GetAlive()
    {
        return isAlive;
    }

    // 死亡時の処理
    private void DeadLogic()
    {
        if (!isAlive || !GetComponent<PhotonView>().IsMine) return;
        scoreManager.SetAliveTime(Time.time);
        gameManager.SetDecrementAliveCount();

        if (canvasObject != null)
        {
            MRKilledImagedAttach mrKilleImagedAttach = canvasObject.GetComponent<MRKilledImagedAttach>();
            if (mrKilleImagedAttach != null)
            {
                // Photon のカスタムプロパティから名前に基づくインデックスを取得

                string[] playerNames = gameManager.GetAllPlayerNames();
                for (int i = 0; i < playerNames.Length; i++)
                {
                    if (playerNames[i].Contains(PlayerName))
                    {
                        switch (i)
                        {
                            case 0:
                                mrKilleImagedAttach.SetFirstPlayerDead();
                                break;
                            case 1:
                                mrKilleImagedAttach.SetSecondPlayerDead();
                                break;
                            case 2:
                                mrKilleImagedAttach.SetThirdPlayerDead();
                                break;
                        }
                    }
                }

                //地面に落とす
                //TODO: 実際の地面との調整が必要
                // parentObject.transform.position = new Vector3(parentObject.transform.position.x, 0, parentObject.transform.position.z);
                Debug.Log("Dead");
            }
        }
    }

    private void InterruptLogic()
    {
        playerColorManager?.ChangeColorRed();
        switch (character)
        {
            case GameCharacter.BIRD:
                birdMoveController?.SetMoveSpeedSlow();
                break;
            case GameCharacter.RABBIT:
                rabbitMove?.SetMoveSpeedSlow();
                rabbitJump?.SetJumpSpeedSlow();
                break;
            case GameCharacter.MOUSE:
                mouseMove?.SetMoveSpeedSlow();
                break;
            default:
                Debug.Log("Unknown character type: " + character);
                break;
        }
    }

    private int GetPlayerIndexByName(string playerName)
    {
        // PhotonNetwork.PlayerList からプレイヤー情報をリストに格納し、ActorNumber の昇順にソートする
        List<Player> players = new List<Player>(PhotonNetwork.PlayerList);
        players.Sort((a, b) => a.ActorNumber.CompareTo(b.ActorNumber));

        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].CustomProperties.TryGetValue("playerName", out object existingName))
            {
                if (existingName.ToString() == playerName)
                {
                    return i;
                }
            }
        }
        return -1;
    }

    public static void SetPlayerName(string name)
    {
        PlayerName = name;
    }
}