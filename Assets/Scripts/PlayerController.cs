using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPun
{

    [Header("Player Components")]
    public Rigidbody rigidBody;
    public Player photonPlayer;
    public Weapon playerWeapon;
    public MeshRenderer meshRenderer;


    [Header("Player Info")]
    private int currentAttackerId;
    public int id;                                                          // Unique player identifier set at runtime
    

    [Header("Player Stats")]
    public int healthPoints;
    public int maxHealthPoints;
    public int numberOfKills;
    public bool isDead;
    public float moveSpeed;
    public float jumpForce;

    private bool colorChangeOnDamage;



    void Update ()
    {
                                                                                // If this is not the local player or if the player is dead
        if(!photonView.IsMine || isDead)
        {
            return;
        }

        Move();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            TryJumping();
        }

        if (Input.GetMouseButtonDown(0))
        {
            playerWeapon.TryShooting();
        }
           
    }


    [PunRPC]
    public void Initialize(Player player)
    {
        id = player.ActorNumber;
        photonPlayer = player;

        GameManager.gameManagerInstance.players[id - 1] = this;

                                                                            
        if (!photonView.IsMine)                                                         // Condition to check if this is a local player
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);               // Disbales the camera if false
            rigidBody.isKinematic = true;                                               // Disables all phisycs
        }
        else
        {
           UI.instanceUI.Initialize(this);
        }
    }

    void Move ()
    {
                                                                                         
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

                                                                                          // Calcualtes a direction relative to where we are facing
        Vector3 direction = (transform.forward * z + transform.right * x) * moveSpeed;
        direction.y = rigidBody.velocity.y;

        
        rigidBody.velocity = direction;
    }

    void TryJumping()
    {
                                                                                          // Creates a raycast facing down
        Ray ray = new Ray(transform.position, Vector3.down);

                                                                                          // If we shoot the raycast and hit something
        if(Physics.Raycast(ray, 1.5f))
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    [PunRPC]
    public void TakeBulletDamage (int attackersId, int bulletDamage)
    {
        if (isDead)
        {
            return;
        }
           
        healthPoints -= bulletDamage;
        currentAttackerId = attackersId;

        photonView.RPC("ColorChange", RpcTarget.Others);                                     // Change the player's color to red if hit
                                                                                             // only to others because we are on first person view
       
        UI.instanceUI.UpdateHealthBar();                                                     // Updates the health bar

        
        if(healthPoints <= 0)
        {
            photonView.RPC("Die", RpcTarget.All);                                            // Calsl the die function of the health is less
                                                                                             // equal to 0   
        }
            
    }

    [PunRPC]
    void ColorChange()
    {
        if (colorChangeOnDamage)
        {
            return;
        }
            
        StartCoroutine(ColorChangeCoroutine());

        IEnumerator ColorChangeCoroutine ()
        {
            colorChangeOnDamage = true;

            Color originalColor = meshRenderer.material.color;                               // Keeps track of the original color  
            meshRenderer.material.color = Color.red;                                         // Changes color to red 

            yield return new WaitForSeconds(0.06f);                                          // Waits for 0.06 secs

            meshRenderer.material.color = originalColor;                                      // Retunrs palyer capsule to original color
            colorChangeOnDamage = false;
        }
    }

    [PunRPC]
    void Die()
    {
        healthPoints = 0;
        isDead = true;

        GameManager.gameManagerInstance.numOfAlivePlayers--;
       
        if (PhotonNetwork.IsMasterClient)                                                            // Host server checks if the game has been won
        {
            GameManager.gameManagerInstance.CheckIfSomeoneWon();
        }
            
        if(photonView.IsMine)                                                                       // Condition to check if this is the local player
        {
            if(currentAttackerId != 0)
            {
                GameManager.gameManagerInstance.GetPlayer(currentAttackerId).                       // Updates kill count for all players
                   photonView.RPC("AddAKill", RpcTarget.All);
            }
               

            GetComponentInChildren<CameraController>().SetAsSpectator();                            // Sets the camera to spectator mode

            transform.position = new Vector3(0, 40, 0);                                             // Moves the player out of view
            rigidBody.isKinematic = true;                                                           // Disbales all physics on the player
        }
    }

    [PunRPC]
    public void AddAKill()
    {
        numberOfKills++;                                                                            // Increases kill count
        UI.instanceUI.UpdatePlayerInfo();                                                           // Updates the kill count displayed
    }

    [PunRPC]
    public void Heal (int amountToHeal)
    {
        healthPoints = Mathf.Clamp                                                           // Adds Heal points up to the maximum allowed
            (healthPoints + amountToHeal, 0, maxHealthPoints);

        UI.instanceUI.UpdateHealthBar();
    }
}