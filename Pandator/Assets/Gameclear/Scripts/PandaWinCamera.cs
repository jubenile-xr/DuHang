using UnityEngine;

public class PandaWinCamera : MonoBehaviour
{
    [Header("カメラ移動のパス点")]
    public Transform pointA;
    public Transform pointB_Start;
    public Transform pointB_End;
    public Transform pointC;

    [Header("各セクションの移動時間")]
    public float durationA = 2f;
    public float durationB = 2f;
    public float durationC = 1f;

    [Header("補間制御")]
    public bool interpolateRotation = true;

    [Header("勝利テキストエフェクトスクリプト")]
    public VictoryTextEffect victoryEffect;  // スクリプト参照


    private float timer = 0f;
    private int phase = 0; // 0: A -> B, 1: B -> C, 2: C -> 終了

    private Vector3 startPos;
    private Quaternion startRot; // 開始位置と回転

    private ResultManager resultManager;

    void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        timer = 0f;
        phase = 0;

        resultManager = GameObject.Find("ResultSceneManager").GetComponent<ResultManager>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        switch (phase)
        {
            case 0:
                MoveTo(pointA, durationA, 1);
                break;

            case 1:
                InstantJumpTo(pointB_Start);
                break;

            case 2:
                MoveToWithText(pointC, durationC, 3, true); // テキストをフェードインしながら移動
                break;
            case 3:
                if (timer > 5.0f)
                {
                    resultManager.SetActiveResultScene();
                    resultManager.SetDeActiveWinScene();
                }
                break;
        }
    }

    void MoveTo(Transform target, float duration, int nextPhase, bool fast = false)
    {
        float t = Mathf.Clamp01(timer / duration);
        // float smoothT = fast ? Mathf.SmoothStep(0, 1, t) : t;

        // transform.position = Vector3.Lerp(startPos, target.position, smoothT);
        // if (interpolateRotation)
        //     transform.rotation = Quaternion.Slerp(startRot, target.rotation, smoothT); // 補間回転

        if (t >= 1f)
        {
            phase = nextPhase;
            timer = 0f;
            // startPos = target.position;
            // startRot = target.rotation;
        }
    }

    void MoveToWithText(Transform target, float duration, int nextPhase, bool fast = false)
    {
        float t = Mathf.Clamp01(timer / duration);
        // float smoothT = fast ? Mathf.SmoothStep(0, 1, t) : t;

        // transform.position = Vector3.Lerp(startPos, target.position, smoothT);
        // if (interpolateRotation)
        //     transform.rotation = Quaternion.Slerp(startRot, target.rotation, smoothT);

        if (t >= 1f)
        {
            phase = nextPhase;
            timer = 0f;

            if (victoryEffect != null)
            {
                victoryEffect.ShowVictoryText();  // アニメーション＋カメラ振動を実行
            }
        }
    }

    void InstantJumpTo(Transform point)
    {
        // transform.position = point.position;
        // transform.rotation = point.rotation;

        // startPos = point.position;
        // startRot = point.rotation;

        phase = -1;
        timer = 0f;
    }

    void LateUpdate()
    {
        if (phase == -1)
        {
            float t = Mathf.Clamp01(timer / durationB);
            // float smoothT = Mathf.SmoothStep(0, 1, t);

            // transform.position = Vector3.Lerp(pointB_Start.position, pointB_End.position, smoothT);
            // if (interpolateRotation)
            //     transform.rotation = Quaternion.Slerp(pointB_Start.rotation, pointB_End.rotation, smoothT);

            if (t >= 1f)
            {
                phase = 2;
                timer = 0f;
                // startPos = pointB_End.position;
                // startRot = pointB_End.rotation;
            }
        }
    }
}