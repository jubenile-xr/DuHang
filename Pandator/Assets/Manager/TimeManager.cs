using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float gameTime;

    void Start()
    {
        gameTime = 0;
    }
    void Update()
    {
        gameTime += Time.deltaTime;
    }
    // getter
    public float GetGameTime()
    {
        return gameTime;
    }

    // formatter 00:00
    private string formatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

}
