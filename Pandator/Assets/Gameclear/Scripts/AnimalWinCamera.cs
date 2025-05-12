// using UnityEngine;

// public class AnimalWinCamera : MonoBehaviour
// {
//     public VictoryTextEffect victoryTextEffect;

//     [Header("WinTarget")]
//     public Transform target;

//     [Header("Oringinal Stuts")]
//     public Vector3 startOffset = new Vector3(2, 1.5f, -1.5f);
//     public float startAngle = 0f;
//     public float lookAtAngle = 2.1f;

//     [Header("Rotation Setting")]
//     public float rotateAngle = 90f;
//     public float rotateDuration = 1.5f;

//     [Header("Pull back Setting")]
//     public Vector3 finalOffset = new Vector3(0, 2, -1.3f);
//     public float pullBackDuration = 2f;
//     public float smoothSpeed = 3f;
//     public float lookAtHeight = 2.1f;

//     private enum State { Idle, ZoomIn, Rotate, PullBack, Done }
//     private State currentState = State.Idle;

//     private float timer = 0f;
//     private Vector3 startPos;
//     private Quaternion startRot;

//     private ResultManager resultManager;

//     void Start()
//     {
//         resultManager = GameObject.Find("ResultSceneManager").GetComponent<ResultManager>();
//     }
//     void Update()
//     {
//         if (target == null) return;

//         switch (currentState)
//         {
//             case State.ZoomIn:
//                 timer += Time.deltaTime;
//                 // transform.position = Vector3.Lerp(startPos, target.position + startOffset, timer * smoothSpeed);
//                 // transform.LookAt(target.position + Vector3.up * lookAtAngle);

//                 if (timer >= 1f)
//                 {
//                     timer = 0f;
//                     currentState = State.Rotate;
//                 }
//                 break;

//             case State.Rotate:
//                 timer += Time.deltaTime;
//                 // float angle = Mathf.Lerp(0, rotateAngle, timer / rotateDuration);
//                 // transform.position = target.position + Quaternion.Euler(0, angle, 0) * startOffset;
//                 // transform.LookAt(target.position + Vector3.up * lookAtAngle);

//                 if (timer >= rotateDuration)
//                 {
//                     timer = 0f;
//                     currentState = State.PullBack;
//                 }
//                 break;

//             case State.PullBack:
//                 timer += Time.deltaTime;
//                 // Vector3 desiredPos = target.position + finalOffset;
//                 // transform.position = Vector3.Lerp(transform.position, desiredPos, timer / pullBackDuration);
//                 // transform.LookAt(target.position + Vector3.up * lookAtHeight);

//                 if (timer >= pullBackDuration)
//                 {
//                     currentState = State.Done;
//                     victoryTextEffect?.ShowVictoryText();
//                     timer = 0f;
//                 }
//                 break;
//             case State.Done:
//                 timer += Time.deltaTime;
//                 if (timer >= 5f)
//                 {
//                     resultManager.SetActiveResultScene();
//                     resultManager.SetDeActiveWinScene();
//                 }
//                 break;
//         }
//     }

//     public void SetTarget(Transform winner)
//     {
//         target = winner;
//     }

//     public void PlayResultCamera()
//     {
//         currentState = State.ZoomIn;
//         timer = 0f;

//         if (target == null) return;

//         // startPos = target.position + Quaternion.Euler(0, startAngle, 0) * startOffset;
//         // startRot = Quaternion.LookRotation(target.position - startPos);

//         // transform.position = startPos;
//         // transform.rotation = startRot;
//     }
// }
