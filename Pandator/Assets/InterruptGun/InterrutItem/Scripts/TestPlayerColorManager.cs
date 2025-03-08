using UnityEngine;

public class TestPlayerColorManager : MonoBehaviour
{
    // 妨害時
    public void ChangeColorRed()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public void ChangeColorWhite()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
    // 死んだ時
    public void ChangeColorBlack()
    {
        GetComponent<Renderer>().material.color = Color.black;
    }
}
