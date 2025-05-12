using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
	public Image flashImage; // UI Image
	public float flashDuration = 0.3f;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			TriggerFlash();
		}
	}

	public void TriggerFlash()
	{
		StartCoroutine(FlashRoutine());
	}

	IEnumerator FlashRoutine()
	{
		for (int i = 0; i < 2; i++)
		{
			flashImage.color = new Color(1, 0, 0, 0.5f);
			yield return new WaitForSeconds(flashDuration);

			float fadeSpeed = 2f;
			while (flashImage.color.a > 0)
			{
				Color color = flashImage.color;
				color.a -= Time.deltaTime * fadeSpeed;
				flashImage.color = color;
				yield return null;
			}
		}
	}
}
