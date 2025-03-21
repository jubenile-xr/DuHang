using UnityEngine;

public class PhotonStateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")]
    [SerializeField] float interruptedTime = 3.0f;
    [Header("妨害時の速度")]
    [SerializeField] float interruptedSpeed = 2.0f;
    private float time;
    [SerializeField] private TestPlayerColorManager playerColorManager;
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
            // ここは本来はkeyMoveではなく、PlayerControllerのスクリプトにアクセスする
            photonKeyMove.SetSpeed(interruptedSpeed);
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
        playerColorManager.ChangeColorWhite();
        photonKeyMove.SetSpeed(10.0f);
    }
    public void SetInterrupted(bool value)
    {
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
        scoreManager.SetAliveTime(Time.time);
        gameManager.SetDecrementAliveCount();
        //地面の落とす
        parentObject.transform.position = new Vector3(0, 0, 0);
        Debug.Log("Dead");
    }
}
