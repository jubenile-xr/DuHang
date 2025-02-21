using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float gameTime;

    void Start()
    {
        gameTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += Time.deltaTime;
    }

    public float GetGameTime()
    {
        return gameTime;
    }

}
