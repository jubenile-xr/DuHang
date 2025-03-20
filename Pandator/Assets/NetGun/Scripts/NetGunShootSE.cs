using UnityEngine;

public class NetGunShootSE : MonoBehaviour

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