using UnityEngine;

public class InterruptGunHits:MonoBehaviour
{
	[SerializeField] private AudioSource audioSource;
	private bool isHit = false; 
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.H))
		{
			isHit = true;
		}
		if (isHit)
		{
			audioSource.Play();
			isHit = false;
		}
	}
}
