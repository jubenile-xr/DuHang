using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour
{
    private float score;
    private float aliveTime;
    private int interruptedCount;
    [Header("スコア計算用の定数")]
    [SerializeField] private float scoreMultiplier = 100f; // スコア計算のための定数
    [SerializeField] private float hitPoint = 50f; // 妨害のポイント
    [SerializeField] private float PandaMaxPoint = 1500; // パンダの最大ポイント
    private void Start()
    {
        score = 0;
        aliveTime = 0;
        interruptedCount = 0;
    }
    private void Update()
    {

    }

    public void SetAliveTime(float time)
    {
        aliveTime = time;
    }

    public void SetIncrementInterruptedCount()
    {
        interruptedCount++;
    }

    public float GetScore(string animal)
    {
        CalculateScore(animal);
        return score;
    }

    // スコア計算 10ポイント/秒 + 5ポイント/中断 これは適当
    private void CalculateScore(string animal)
    {
        if (animal == "PANDA")
        {
            score = PandaMaxPoint - aliveTime * 5;
        }
        else if (animal == "BIRD" || animal == "RABBIT" || animal == "MOUSE")
        {
            score = Mathf.Pow(aliveTime, 2) / scoreMultiplier + interruptedCount * hitPoint;
        }
        else
        {
            score = -1; // 不明な動物の場合はスコアを-1に設定
        }
    }

    public IEnumerator SendScore(string name, string animal)
    {
        string url = "https://script.google.com/macros/s/AKfycbzn6Gf0A_H40-PfM1wf7LRjDFOEHNNLutAMGTV5o4bYqTUE_Ppb7Nb1V5F6M7qWdY7N/exec";
        string jsonData = JsonUtility.ToJson(new ScoreData { name = name, animal = animal, score = (int)GetScore(animal) });

        UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "Post");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("スコア送信成功: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("スコア送信失敗: " + request.error);
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public string name;
        public string animal;
        public int score;
    }
}
