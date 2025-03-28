using UnityEngine;
using TMPro;

public class setTime : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;
    private float time = 0.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetTimeText(time);
        time += Time.deltaTime;
    }
    public void SetTimeText(float time)
    {
        timeText.text = time.ToString("F2");
    }
}
