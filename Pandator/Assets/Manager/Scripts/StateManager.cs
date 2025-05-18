using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class StateManager : MonoBehaviourPun
{
    private bool isInterrupted;
    private bool isAlive = true;
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
        if (gameManager != null && gameManager.GetGameState() == GameManager.GameState.PLAY)
        {
            // GameManagerの死亡状態を確認して、isAliveを更新
            bool shouldBeAlive = !GetMyDeadStatus();
            if (isAlive != shouldBeAlive)
            {
                SetAlive(shouldBeAlive);
            }
        }

        if (isInterrupted)
        {
            time += Time.deltaTime;
            if (time > interruptedTime)
            {
                ResetState();
            }
        }
        else
        {
            ResetState();
        }
        if (Character.GetSelectedAnimal() != Character.GameCharacters.GOD && Character.GetSelectedAnimal() != Character.GameCharacters.PANDA && !isGetDeadVolumeController)
        {
            deadVolumeController = GameObject.FindWithTag("DeadVolume")?.GetComponent<DeadVolumeController>();
            Debug.Log("DeadVolumeController component found in the DeadVolume.");
            isGetDeadVolumeController = true;
        }
        if (Character.GetSelectedAnimal() != Character.GameCharacters.GOD && Character.GetSelectedAnimal() != Character.GameCharacters.PANDA && !isGetFlashEffect)
        {
            flashEffect = GameObject.FindWithTag("Canvas")?.GetComponentInChildren<FlashEffect>();
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
        if (string.IsNullOrEmpty(myName)) return false;

        // 一致するインデックスを検索
        int myIndex = -1;
        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Equals(playerName))
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
        time = 0;
    }

    public void SetInterrupted(bool value)
    {
        if (value && !isAlive)
        {
            Debug.Log("SetInterrupted: Dead");
            InterruptLogic();
        }
        isInterrupted = value;
    }

    // 生存状態を設定するメソッド。falseの場合（死亡時）には視覚効果も適用する
    public void SetAlive(bool value)
    {
        // 状態に変化がなければ何もしない
        if (isAlive == value) return;

        isAlive = value;

        // 死亡した場合
        if (!isAlive && !isDeadLogicExecuted)
        {
            // 視覚効果の適用と死亡ロジックの実行をRPCで同期
            photonView.RPC("ApplyDeadEffects", RpcTarget.AllBuffered);
        }
    }

    // 死亡エフェクトを適用するRPCメソッド
    [PunRPC]
    private void ApplyDeadEffects()
    {
        // すでに実行済みなら何もしない
        if (isDeadLogicExecuted) return;

        // 透明化エフェクト（全クライアントで実行）
        playerColorManager?.ChangeColorInvisible();

        // スコア記録（全クライアントで実行）
        if (scoreManager != null)
        {
            scoreManager.SetAliveTime(Time.time);
        }

        // 自分が所有するプレイヤーの場合のみの処理
        if (photonView.IsMine)
        {
            // 画面効果
            if (deadVolumeController != null)
            {
                deadVolumeController.RunDeadVolume();
            }

            // 必要に応じてGameManagerの死亡状態も更新
            UpdateGameManagerDeadStatus();
        }

        // 死亡ロジック実行済みフラグを設定
        isDeadLogicExecuted = true;
    }

    // GameManagerの死亡状態を更新
    private void UpdateGameManagerDeadStatus()
    {
        if (gameManager == null || string.IsNullOrEmpty(playerName)) return;

        string[] playerNames = gameManager.GetAllPlayerNames();
        int myIndex = -1;

        for (int i = 0; i < playerNames.Length; i++)
        {
            if (playerNames[i].Equals(playerName))
            {
                myIndex = i;
                break;
            }
        }

        if (myIndex >= 0 && !gameManager.GetPlayerDeadStatus()[myIndex])
        {
            gameManager.SetPlayerDeadStatusTrue(myIndex);
            gameManager.UpdateAliveCountFromDeadStatus();
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
