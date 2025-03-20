using UnityEngine;
using TMPro;

public class CameraPositionLogger : MonoBehaviour
{
    public Camera targetCamera;
    public TextMeshProUGUI XText;
    public TextMeshProUGUI HText;
    public TextMeshProUGUI YText;

    void Update()
    {
        if (targetCamera != null)
        {
            Vector3 camPosition = targetCamera.transform.position;

            if (XText != null)
                XText.text = "X:" + camPosition.x.ToString("F2");

            if (HText != null)
                HText.text = "H:" + camPosition.y.ToString("F2");

            if (YText != null)
                YText.text = "Y:" + camPosition.z.ToString("F2");
        }
    }
}
