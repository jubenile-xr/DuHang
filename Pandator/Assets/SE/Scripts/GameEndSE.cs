using UnityEngine;

public class GameEndSE : MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			audioSource.Play();
		}
	}
}
