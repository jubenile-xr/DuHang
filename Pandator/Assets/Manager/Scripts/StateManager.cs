using UnityEngine;

public class StateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")]
    [SerializeField] float interruptedTime = 3.0f;
    [Header("妨害時の速度")]
    [SerializeField] float interruptedSpeed = 2.0f;
    private float time;
    [SerializeField] private PlayerColorManager playerColorManager;
    [SerializeField] private PhotonKeyMove photonKeyMove;
    private GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;

    [Header("親オブジェクト操作用"),SerializeField] private GameObject parentObject;

    void Start()
    {
        isInterrupted = false;
        isAlive = true;
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        Debug.Log("GameManager: " + gameManager);
    }
    void Update()
    {
        if(isInterrupted)
        {
            time += Time.deltaTime;
            // TODO: 動物のインターフェースのスクリプトにアクセスする
            // photonKeyMove.SetSpeed(interruptedSpeed);
            if(time > interruptedTime)
            {
                ResetState();
            }
        }
    }
    // Reset the state of the player
    private void ResetState()
    {
        isInterrupted = false;
        time = 0;
        // TODO: 動物のインターフェースでスピードを元に戻す
        // photonKeyMove.SetSpeed(10.0f);
        playerColorManager?.ChangeColorOriginal();
    }
    public void SetInterrupted(bool value)
    {
        if (value)
        {
            // TODO: 動物のインターフェースでスピードを遅くする
            // photonKeyMove.SetSpeed(10.0f);
            playerColorManager?.ChangeColorRed();
        }
        isInterrupted = value;
    }

    public bool GetInterrupted()
    {
        return isInterrupted;
    }

    public void SetAlive(bool value)
    {
        if(!value)
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
        if (!isAlive) return;
        scoreManager.SetAliveTime(Time.time);
        gameManager.SetDecrementAliveCount();
        //地面に落とす
        //TODO: 実際の地面との調整が必要
        // parentObject.transform.position = new Vector3(parentObject.transform.position.x, 0, parentObject.transform.position.z);
        Debug.Log("Dead");
    }
}
