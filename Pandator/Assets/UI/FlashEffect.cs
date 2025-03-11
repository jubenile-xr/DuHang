using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlashEffect : MonoBehaviour
{
	public Image flashImage; // UI Image 组件
	public float flashDuration = 0.3f; // 闪烁持续时间

	void Start()
	{
		// 确保一开始是透明的
		flashImage.color = new Color(1, 0, 0, 0);
	}

	void Update()
	{
		// 当玩家按下 F 键时触发闪烁
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
		// 让屏幕变红
		flashImage.color = new Color(1, 0, 0, 0.5f);
		yield return new WaitForSeconds(flashDuration);

		// 逐渐变透明
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
