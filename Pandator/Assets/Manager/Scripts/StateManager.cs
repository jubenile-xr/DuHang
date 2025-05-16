using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")] private const float interruptedTime = 3.0f;
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
    private bool isDeadLogicExecuted = false;
    private string playerName;
    private enum GameCharacter
    {
        BIRD,
        RABBIT,
        MOUSE
    }
    [Header("キャラクターの種類"), SerializeField] private GameCharacter character;
    private FlashEffect flashEffect;
    private DeadVolumeController deadVolumeController;
    private bool isGetFlashEffect = false;
    private bool isGetDeadVolumeController = false;
    private void Start()
    {
        isInterrupted = false;
        isAlive = true;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Debug.Log("GameManager: " + gameManager);
        if (!GameObject.FindWithTag("Canvas"))
        {
            Debug.Log("FlashEffect component not found in the canvas!");
        }
        else
        {
            flashEffect = GameObject.FindWithTag("Canvas").GetComponentInChildren<FlashEffect>();
            Debug.Log("FlashEffect component found in the canvas.");
        }
        if (!GameObject.FindWithTag("DeadVolume"))
        {
            Debug.Log("DeadVolumeController component found in the DeadVolume.");
        }
        else
        {
            Debug.Log("DeadVolumeController component not found in the DeadVolume.");
            deadVolumeController = GameObject.FindWithTag("DeadVolume").GetComponent<DeadVolumeController>();
        }
        switch (Character.GetSelectedAnimal())
        {
            case Character.GameCharacters.BIRD:
                character = GameCharacter.BIRD;
                break;
            case Character.GameCharacters.RABBIT:
                character = GameCharacter.RABBIT;
                break;
            case Character.GameCharacters.MOUSE:
                character = GameCharacter.MOUSE;
                break;
            default:
                Debug.LogError("Invalid character selected.");
                return; // 不正なキャラクターが選択された場合は処理を中断
        }

        canvasObject = GameObject.FindWithTag("Canvas");
        if (canvasObject == null)
        {
            Debug.LogError("Canvas object not found!");
        }
    }

    private void Update()
    {
        if(gameManager != null && gameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            SetAlive(!GetMyDeadStatus());
        }

        if (!isAlive && GetComponent<PhotonView>().IsMine && !isDeadLogicExecuted)
        {
            DeadLogic();
        }

        if (isInterrupted)
        {
            time += Time.deltaTime;
            if (time > interruptedTime)
            {
                ResetState();
            }
        }
        if (Character.GetSelectedAnimal() != Character.GameCharacters.GOD && Character.GetSelectedAnimal() != Character.GameCharacters.PANDA && !isGetDeadVolumeController)
        {
            deadVolumeController = GameObject.FindWithTag("DeadVolume").GetComponent<DeadVolumeController>();
            Debug.Log("DeadVolumeController component found in the DeadVolume.");
            isGetDeadVolumeController = true;
        }
        if (Character.GetSelectedAnimal() != Character.GameCharacters.GOD && Character.GetSelectedAnimal() != Character.GameCharacters.PANDA && !isGetFlashEffect)
        {
            flashEffect = GameObject.FindWithTag("Canvas").GetComponentInChildren<FlashEffect>();
            Debug.Log("FlashEffect component found in the canvas.");
            isGetFlashEffect = true;
        }
    }
    // 自分の死亡ステータスを取得するメソッド
    public bool GetMyDeadStatus()
    {
        if (gameManager == null) return false;

        // GameManagerから全プレイヤー名を取得
        string[] playerNames = gameManager.GetAllPlayerNames();
        if (playerNames == null || playerNames.Length == 0) return false;

        // 現在のプレイヤー名を取得
        string myName = Character.GetMyName();

        // 一致するインデックスを検索
        int myIndex = -1;
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Contains(myName))
            {
                myIndex = i;
                break;
            }
        }

        // インデックスが見つかった場合、対応する死亡ステータスを返す
        if (myIndex >= 0 && myIndex < gameManager.GetPlayerDeadStatus().Length)
        {
            bool isDead = gameManager.GetPlayerDeadStatus()[myIndex];
            Debug.Log($"プレイヤー {myName} の死亡ステータス: {isDead}");
            return isDead;
        }

        Debug.LogWarning($"プレイヤー {myName} のインデックスが見つかりません");
        return false;
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


    public void SetAlive(bool value)
    {
        isAlive = value;
    }


    // 死亡時の処理
    private void DeadLogic()
    {
        isDeadLogicExecuted = true;
        GetComponent<PlayerColorManager>()?.ChangeColorInvisible();
        scoreManager.SetAliveTime(Time.time);
        deadVolumeController?.RunDeadVolume();

        string[] playerNames = gameManager.GetAllPlayerNames();
        Debug.Log("State:DeadLogic: Player Names: " + string.Join(", ", playerNames));
        Debug.Log("State:PlayerName" + playerName);

        bool isAlreadyDead = false;
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Contains(playerName))
            {
                isAlreadyDead = gameManager.GetPlayerDeadStatus()[i];
                if (!isAlreadyDead)
                {
                    gameManager.SetPlayerDeadStatusTrue(i);
                    gameManager.UpdateAliveCountFromDeadStatus();
                }
                break;
            }
        }
    }

    private void InterruptLogic()
    {
        playerColorManager?.ChangeColorRed();
        flashEffect?.TriggerFlash();
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

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public string GetPlayerName()
    {
        return playerName;
    }
}
