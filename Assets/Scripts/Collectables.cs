using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class Collectables : MonoBehaviour
{
    public PowerUpType powerUpType;
    public int value;

    void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        if(other.CompareTag("Player"))                                  // Checks if the object that collided
                                                                        // with it hast the tag Player
        {
            PlayerController player = GameManager.
                gameManagerInstance.GetPlayer(other.gameObject);

            if(powerUpType == PowerUpType.Health)                       // Increases health  on the player if its
            {                                                           // of the type Health
                player.photonView.RPC("Heal",
                    player.photonPlayer, value);
            }
                
            else if(powerUpType == PowerUpType.Ammo)                    // Increases ammo on the player if its
            {                                                           // of the type ammo
                player.photonView.RPC("IncreaseAmmo",
                    player.photonPlayer, value);
            }

            PhotonNetwork.Destroy(gameObject);
        }
    }

    public enum PowerUpType
    {
        Health,
        Ammo
    }
}