using Photon.Pun;
using UnityEngine;

public class Net : MonoBehaviour
{
    private float time = 0.0f;
    [SerializeField]private float collisionDeleteTime = 0.1f;
    private float collisionTime = 0.0f;
    private Animator animator;
    private GameManager gameManager;
    private PhotonView photonView;

    //一度当たったらonにする
    private bool isCollision = false;
    [SerializeField]private string targetTag;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GameManager")?.GetComponent<GameManager>();
        photonView = GetComponent<PhotonView>();

        animator.SetTrigger("Capture");
    }

    private void Update()
    {
        animator.SetTrigger("Idle");

        // 3秒後に消える
        time += Time.deltaTime;
        if(time > 3.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(isCollision) return;
        GameObject Player = collision.gameObject;

        if(Player.CompareTag("Player"))
        {
            isCollision = true;
            //速度を0に
            GetComponent<Rigidbody>().linearVelocity = Vector3.zero;

            // Make sure we have a GameManager
            if(gameManager == null)
            {
                gameManager = GameObject.FindWithTag("GameManager")?.GetComponent<GameManager>();
                if(gameManager == null)
                {
                    Debug.LogError("GameManager not found in Net collision!");
                    return;
                }
            }

            // Get the player's StateManager
            StateManager stateManager = Player.GetComponent<StateManager>();
            if(stateManager != null)
            {
                // Set the player to not alive in StateManager
                stateManager.SetAlive(false);

                // Get the player's name
                string playerName = stateManager.GetPlayerName();

                // Visual feedback - change color
                Player.GetComponent<PlayerColorManager>()?.ChangeColorInvisible();

                // Run dead volume effect if it's a VR player
                DeadVolumeController deadVol = GameObject.FindWithTag("DeadVolume")?.GetComponent<DeadVolumeController>();
                if(deadVol != null && stateManager.GetComponent<PhotonView>().IsMine)
                {
                    deadVol.RunDeadVolume();
                }

                // Update playerDeadStatus in GameManager
                string[] playerNames = gameManager.GetAllPlayerNames();
                for (int i = 0; i < playerNames.Length; i++)
                {
                    if (playerNames[i].Contains(playerName))
                    {
                        // Set this player's dead status to true
                        gameManager.SetPlayerDeadStatusTrue(i);

                        // Update aliveCount based on playerDeadStatus
                        gameManager.UpdateAliveCountFromDeadStatus();
                        break;
                    }
                }
            }
            else
            {
                // Handle case when no StateManager (might be PANDA MR player)
                // For PANDA player, we need to find out which VR player was hit
                PlayerColorManager colorManager = Player.GetComponent<PlayerColorManager>();
                if(colorManager != null)
                {
                    // Visual feedback
                    colorManager.ChangeColorInvisible();

                    // Find which player this is in the playerNames array
                    string[] playerNames = gameManager.GetAllPlayerNames();
                    for (int i = 0; i < playerNames.Length; i++)
                    {
                        if (Player.name.Contains(playerNames[i]))
                        {
                            // Set this player's dead status to true
                            gameManager.SetPlayerDeadStatusTrue(i);

                            // Update aliveCount based on playerDeadStatus
                            gameManager.UpdateAliveCountFromDeadStatus();
                            break;
                        }
                    }
                }
            }

            Debug.Log("Player hit by net: " + Player.name);
        }
    }
}
