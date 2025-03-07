using UnityEngine;

public class TestPlayerColorManager : MonoBehaviour
{
    public void ChangeColorRed()
    {
        GetComponent<Renderer>().material.color = Color.red;
    }
    public void ChangeColorWhite()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }
}
