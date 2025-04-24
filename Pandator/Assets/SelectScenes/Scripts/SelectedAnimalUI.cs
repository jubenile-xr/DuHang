using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用するための名前空間

public class SelectedAnimalUI : MonoBehaviour
{
    [SerializeField] private RawImage rabbitFrameUI;
    [SerializeField] private TextMeshProUGUI rabbitNameUI;
    [SerializeField] private RawImage mouseFrame;
    [SerializeField] private TextMeshProUGUI mouseNameUI;
    [SerializeField] private RawImage birdFrame;
    [SerializeField] private TextMeshProUGUI birdNameUI;

    public void ResetAnimalUI()
    {
        rabbitFrameUI.color = Color.white;
        rabbitNameUI.color = Color.white;
        birdFrame.color = Color.white;
        birdNameUI.color = Color.white;
        mouseFrame.color = Color.white;
        mouseNameUI.color = Color.white;
    }

    public void RabbitedSelected()
    {
        rabbitFrameUI.color = Color.yellow;
        rabbitNameUI.color = Color.yellow;
        birdFrame.color = Color.white;
        birdNameUI.color = Color.white;
        mouseFrame.color = Color.white;
        mouseNameUI.color = Color.white;
    }
    public void BirdSelected()
    {
        rabbitFrameUI.color = Color.white;
        rabbitNameUI.color = Color.white;
        birdFrame.color = Color.yellow;
        birdNameUI.color = Color.yellow;
        mouseFrame.color = Color.white;
        mouseNameUI.color = Color.white;
    }
    public void MouseSelected()
    {
        rabbitFrameUI.color = Color.white;
        rabbitNameUI.color = Color.white;
        birdFrame.color = Color.white;
        birdNameUI.color = Color.white;
        mouseFrame.color = Color.yellow;
        mouseNameUI.color = Color.yellow;
    }
}
