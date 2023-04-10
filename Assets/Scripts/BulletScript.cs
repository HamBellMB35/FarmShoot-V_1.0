using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Rigidbody rb;
    private bool isMine;
    private int damage;
    private int attackerId;
  
   
    public void Initialize (int damage, int attackerId, bool isMine)                    // This method is called when the bullet spawns
    {
        this.damage = damage;
        this.attackerId = attackerId;
        this.isMine = isMine;

        Destroy(gameObject, 4.0f);                                                      // Destorys object after 4 secs so it doenst last forever
    }

    void OnTriggerEnter (Collider other)                                                // This method uses client side hit detection
    {                                                                                   // to check if the bullet fired from the local
                                                                                        // player  hits another player, and then applies 
                                                                                        // the damagePerBullet ammount by calling TakeDamge
                                                                                        // on the player with the attackerId

        if (other.CompareTag("Player") && isMine)
        {
            PlayerController player =
                GameManager.gameManagerInstance.GetPlayer(other.gameObject);

            if(player.id != attackerId)                                                  // Condition to check if we are not shooting ourselves
            {
                player.photonView.RPC("TakeBulletDamage",                                // Calls TakeBulletDamage on the player that has been hit
                    player.photonPlayer, attackerId, damage);
            }
                
        }

        Destroy(gameObject);
    }
}