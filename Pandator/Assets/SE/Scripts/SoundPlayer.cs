using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    public void Play()
    {
        audioSource.Play();
    }
    public void Stop()
    {
        audioSource.Stop();
    }
    public void Pause()
    {
        audioSource.Pause();
    }
    public void SetLoop(bool value)
    {
        audioSource.loop = value;
    }
    public void SetPitch(float value)
    {
        audioSource.pitch = value;
    }
}
