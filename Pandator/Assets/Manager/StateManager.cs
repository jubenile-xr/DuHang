using UnityEngine;

public class StateManager : MonoBehaviour
{
    private bool isInterrupted;
    private bool isAlive;

    void Start()
    {
        isInterrupted = false;
        isAlive = true;
    }

    // Update is called once per frame
    void Update()
    {

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
        isAlive = value;
    }

    public bool GetAlive()
    {
        return isAlive;
    }
}
