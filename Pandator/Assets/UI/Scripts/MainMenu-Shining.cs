using UnityEngine;
using System.Collections;

public class RotateAndScale : MonoBehaviour
{
    public float rotationSpeed = 100f; 
    public float scaleDuration = 2f;     
    public float scaleFactor = 0.8f;  
    private Vector3 originalScale; 

    void Start()
    {
        originalScale = transform.localScale;
        StartCoroutine(ScaleLoop());
    }

    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }

    IEnumerator ScaleLoop()
    {
        while (true)
        {
            yield return StartCoroutine(ScaleObject(originalScale, originalScale * scaleFactor, scaleDuration));
            yield return StartCoroutine(ScaleObject(originalScale * scaleFactor, originalScale, scaleDuration));
        }
    }

    IEnumerator ScaleObject(Vector3 startScale, Vector3 endScale, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        transform.localScale = endScale;
    }
}
