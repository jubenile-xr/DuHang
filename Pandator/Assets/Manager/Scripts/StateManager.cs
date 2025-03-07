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
    [SerializeField] private TestPlayerColorManager playerColorManager;
    [SerializeField] private KeyMove keyMove;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ScoreManager scoreManager;


    void Start()
    {
        isInterrupted = false;
        isAlive = true;
    }
    void Update()
    {
        if(isInterrupted)
        {
            time += Time.deltaTime;
            // ここは本来はkeyMoveではなく、PlayerControllerのスクリプトにアクセスする
            keyMove.setSpeed(interruptedSpeed);
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
        keyMove.setSpeed(10.0f);
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
        if(value == false)
        {
            DeadLogic();
        }
        isAlive = value;
    }

    public bool GetAlive()
    {
        return isAlive;
    }

    // ここに死亡時の処理を書く
    private void DeadLogic()
    {
        scoreManager.setAliveTime(Time.time);
        gameManager.SetDecrementAliveCount();
    }
}
