using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class lazer : MonoBehaviour
{
    public Transform laserOrigin; // レーザーの発射位置
    public float laserRange = 50f; // レーザーの最大距離
    private LineRenderer lineRenderer;
    private bool isKeybord = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2; // 始点と終点の2点
    }

    // Update is called once per frame
    void Update()
    {
        ShootLaser();

        if (Input.GetKeyDown(KeyCode.K))
        {
            isKeybord = true;
        }

        if (isKeybord)
        {
            laserOrigin = Camera.main.transform;
        }
    }

    void ShootLaser()
    {
        RaycastHit hit;
        Vector3 startPosition = laserOrigin.position + new Vector3(0f, -0.1f, 0f); 
        Vector3 endPosition = startPosition + laserOrigin.forward * laserRange;

        if (Physics.Raycast(startPosition, laserOrigin.forward, out hit, laserRange))
        {
            endPosition = hit.point;
        }

        lineRenderer.SetPosition(0, startPosition);
        lineRenderer.SetPosition(1, endPosition);
    }
}
