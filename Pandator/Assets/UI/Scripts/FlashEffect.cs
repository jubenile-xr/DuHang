using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
	public Image flashImage; // UI Image ���
	public float flashDuration = 0.3f; // ��˸����ʱ��

	void Start()
	{
		// ȷ��һ��ʼ��͸����
		flashImage.color = new Color(1, 0, 0, 0);
	}

	void Update()
	{
		// ����Ұ��� F ��ʱ������˸
		if (Input.GetKeyDown(KeyCode.F))
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
		// ����Ļ���
		flashImage.color = new Color(1, 0, 0, 0.5f);
		yield return new WaitForSeconds(flashDuration);

		// �𽥱�͸��
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
