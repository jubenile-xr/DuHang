using UnityEngine;
using TMPro; // TextMeshProを使うのに必要

public class ScoreHandler : MonoBehaviour // MonoBehaviourを継承
{
    private GameObject StateManager;
    private StateManager StateCheck;

    [SerializeField] private TextMeshProUGUI AliveTime;
    [SerializeField] private TextMeshProUGUI InteruptCount;
    [SerializeField] private TextMeshProUGUI TotalScore;


    private void Update()
    {
        AliveTime.text = "ALIVE TIME\n" + ScoreField.GetAliveTime().ToString();
        InteruptCount.text = "HIT COUNT\n" + ScoreField.GetInterruptedCount().ToString();
        TotalScore.text = "TOTAL SCORE\n" + ScoreField.GetScore().ToString();
    }
}