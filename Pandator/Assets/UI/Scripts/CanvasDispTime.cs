using UnityEngine;
using TMPro;

public class CanvasDispTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    
    public void SetTimeText(string time)
    {
        timeText.text = time;
    }
}
