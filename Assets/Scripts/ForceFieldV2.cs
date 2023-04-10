using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceFieldV2 : MonoBehaviour
{

    [Header("Blue Force Field Settings")]
    public float minReductionAmount;
    public float reductionWaitTime;
    public float reductionDuration;
    public float sizeReductionAmount;
    public int playerDamageAmount;

    private float previousReductionEndTime;
    private float targetDiameter;
    private float lastPlayerCheckTime;
    private bool reducingSize;

    void Start ()
    {
        previousReductionEndTime = Time.time;
        targetDiameter = transform.localScale.x;
    }

    void Update()
    {
        
        if(reducingSize)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale,
                Vector3.one * targetDiameter, (sizeReductionAmount /
                reductionDuration) * Time.deltaTime);

            if(transform.localScale.x == targetDiameter)
            {
                reducingSize = false;
            }
               
        }
        else
        {
            if(Time.time - previousReductionEndTime >= 
                reductionWaitTime && transform.localScale.x > minReductionAmount)
            {
                DecreaseForceFieldSize();
            }
        }

        CheckPlayersLocation();
    }

    void DecreaseForceFieldSize ()
    {
        reducingSize = true;

        if(transform.localScale.x - sizeReductionAmount > minReductionAmount)                       // Caps the minmum blue force field size
        {
            targetDiameter -= sizeReductionAmount;
        }

        else
        {
            targetDiameter = minReductionAmount;
        }

        previousReductionEndTime = Time.time + reductionDuration;
    }

    void CheckPlayersLocation ()
    {
        if(Time.time - lastPlayerCheckTime > 1.0f)
        {
            lastPlayerCheckTime = Time.time;

           
            foreach(PlayerController player in GameManager.gameManagerInstance.players)                  // Loops through all players
            {
                
                if(!player || player.isDead)                                                             // Skips the checking if the player is dead
                {
                    continue;
                }

                if(Vector3.Distance(Vector3.zero, player.transform.position)                             // Checks if players are outside the blue forceField   
                    >= transform.localScale.x)
                {
                    player.photonView.RPC("TakeBulletDamage",                                           // Applies bullet damage
                        player.photonPlayer, 0, playerDamageAmount);
                }
            }
        }
    }
}