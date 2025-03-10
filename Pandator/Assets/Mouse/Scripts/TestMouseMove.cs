using UnityEngine;
using Photon.Pun;
using System.Collections;

public class TestMouseMove : MonoBehaviourPun
{
    [Header("移動速度")]
    [SerializeField] private float speed = 10.0f;
    [Header("登る速度")]
    [SerializeField] private float climbSpeed = 5.0f;
    [Header("壁状態変更のディレイ")]
    [SerializeField] private float wallStateDelay = 0.5f;

    private bool isTouchingWall = false;
    private Vector3 wallNormal; // 壁の法線ベクトルを保存
    [SerializeField] private Rigidbody rb;

    private Coroutine wallStateCoroutine = null; // 状態変更のコルーチン参照

    private void Update()
    {
        // if (!photonView.IsMine)
        // {
        //     return;
        // }

        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveDirection += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            moveDirection += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A))
        {
            moveDirection += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right;
        }

        // 移動方向の正規化
        if (moveDirection != Vector3.zero)
        {
            moveDirection.Normalize();

            if (isTouchingWall)
            {
                // 移動方向と壁の法線ベクトルの内積を計算
                float dot = Vector3.Dot(moveDirection, -wallNormal);

                // 移動方向が壁に向かっている場合（内積が正）
                if (dot > 0)
                {
                    // 壁に向かう成分を垂直移動に変換
                    Vector3 wallComponent = wallNormal * -dot;
                    Vector3 tangentComponent = moveDirection - wallComponent;

                    // 壁に沿って移動し、壁に向かう力を上方向に変換
                    transform.Translate(tangentComponent * speed * Time.deltaTime);
                    transform.Translate(Vector3.up * climbSpeed * dot * Time.deltaTime);
                }
                else
                {
                    // 壁から離れる方向には通常通り移動
                    transform.Translate(moveDirection * speed * Time.deltaTime);
                }
            }
            else
            {
                // 壁に触れていない場合は通常移動
                transform.Translate(moveDirection * speed * Time.deltaTime);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall collision detected");

            // 壁の法線ベクトルを記録（複数の接触点がある場合は平均を取る）
            wallNormal = Vector3.zero;
            foreach (ContactPoint contact in collision.contacts)
            {
                wallNormal += contact.normal;
            }
            wallNormal.Normalize();

            // 既存のコルーチンがあれば停止
            if (wallStateCoroutine != null)
            {
                StopCoroutine(wallStateCoroutine);
            }

            // 新しいコルーチンを開始
            wallStateCoroutine = StartCoroutine(SetWallTouchingState(true));
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Wall collision exit");

            // 既存のコルーチンがあれば停止
            if (wallStateCoroutine != null)
            {
                StopCoroutine(wallStateCoroutine);
            }

            // 新しいコルーチンを開始
            wallStateCoroutine = StartCoroutine(SetWallTouchingState(false));
        }
    }

    private IEnumerator SetWallTouchingState(bool touching)
    {
        // 指定した時間待機
        yield return new WaitForSeconds(wallStateDelay);

        // 状態を変更
        isTouchingWall = touching;

        if (touching)
        {
            // 重力オフ
            rb.useGravity = false;
            rb.isKinematic = true;
            // 回転をすべてフリーズ
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            Debug.Log("Now touching wall - Gravity: " + rb.useGravity);
            Debug.Log("Kinematic: " + rb.isKinematic);
            Debug.Log("Constraints: " + rb.constraints);
        }
        else
        {
            // 重力オン
            rb.useGravity = true;
            rb.isKinematic = false;
            // フリーズを解除
            rb.constraints = RigidbodyConstraints.None;

            Debug.Log("No longer touching wall");
        }

        // コルーチン参照をクリア
        wallStateCoroutine = null;
    }
}