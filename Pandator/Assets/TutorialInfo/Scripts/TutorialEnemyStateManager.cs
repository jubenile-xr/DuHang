using UnityEngine;

public class TutorialEnemyStateManager : MonoBehaviour
{
    [SerializeField] private PlayerColorManager playerColorManager;
    private bool isInterrupted;
    private bool isAlive;
    [Header("妨害の継続時間")] private const float resetTime = 3.0f;
    private float time;
    // Update is called once per frame
    void Update()
    {
        if (isInterrupted)
        {
            // 3秒後に消える
            time += Time.deltaTime;
            if (time > resetTime)
            {
                Debug.Log("Reset");
                ResetState();
                playerColorManager?.ChangeColorOriginal();
            }
        }
        if (isAlive)
        {
            // 3秒後に消える
            time += Time.deltaTime;
            if (time > resetTime)
            {
                Debug.Log("Reset");
                ResetState();
                playerColorManager?.ChangeColorWhite();
            }
        }
    }

    private void ResetState()
    {
        isInterrupted = false;
        isAlive = false;
        time = 0;
    }

    public void SetInterrupted(bool value)
    {
        isInterrupted = value;
        if (isInterrupted)
        {
            playerColorManager?.ChangeColorRed();
        }
    }
    public void SetAlive(bool value)
    {
        isAlive = value;
        if (isAlive)
        {
            playerColorManager?.ChangeColorInvisible();
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Net"))
        {
            SetAlive(true);
            Debug.Log("Dead");
        }
        if (collision.CompareTag("InterruptItem"))
        {
            SetInterrupted(true);
            Debug.Log("Hit");
        }
    }
}
