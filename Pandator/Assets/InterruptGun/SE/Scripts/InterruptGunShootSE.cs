using UnityEngine;

public class InterruptGunShootSE : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.S))
		{
			audioSource.Play();
		}
	}
}
