using UnityEngine;

public class TestSoundPlayer : MonoBehaviour
{
    [SerializeField] private SoundPlayer soundPlayer;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            soundPlayer.Play();
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            soundPlayer.Stop();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            soundPlayer.Pause();
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            soundPlayer.SetLoop(true);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            soundPlayer.SetLoop(false);
        }
    }
}
