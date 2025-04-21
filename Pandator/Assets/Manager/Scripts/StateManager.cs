using Photon.Pun;
using UnityEngine;

public class StateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")]private const float interruptedTime = 3.0f;
    private float time;
    [SerializeField] private PlayerColorManager playerColorManager;
    [Header("ゲームマネージャー")] private GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;
    [Header("親オブジェクト操作用"), SerializeField] private GameObject parentObject;
    [SerializeField] private RabbitMove rabbitMove;
    [SerializeField] private RabbitJump rabbitJump;
    [SerializeField] private BirdMoveController birdMoveController;
    [SerializeField] private MouseMove mouseMove;
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
        //地面に落とす
        //TODO: 実際の地面との調整が必要
        // parentObject.transform.position = new Vector3(parentObject.transform.position.x, 0, parentObject.transform.position.z);
        Debug.Log("Dead");
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
}