using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueForceField : MonoBehaviour
{
    [Header("Barrier Settings")]
    public float reductionDuration;
    public float minimumReduction;
    public float reductionWaitTime;
    public float reductionAmount;
    public int outsideBarrierDamage;

    private float targetDiameter;
    private float previousPlayerTimeCheck;
    private float previousDecreaseEndtime;
    private bool barrierGettingSmaller;
   

    void Start ()
    {
        targetDiameter = transform.localScale.x;
        previousDecreaseEndtime = Time.time;
    }

    void Update()
    {
        if(barrierGettingSmaller)
        {
            
            transform.localScale = Vector3.MoveTowards(transform.localScale,
                Vector3.one * targetDiameter,
                (reductionAmount / reductionDuration) * Time.deltaTime);

            
            if(transform.localScale.x == targetDiameter)                        // Stops decrease once we reach target diameter
            {
                barrierGettingSmaller = false;
            }
                
        }
        else
        {
            
            if(Time.time - previousDecreaseEndtime >= reductionWaitTime
                && transform.localScale.x > minimumReduction)
            {
                ReduceForceFieldSize();
            }
                
        }

        CheckPlayers();
    }

    void ReduceForceFieldSize ()
    {
        barrierGettingSmaller = true;

        if(transform.localScale.x - reductionAmount > minimumReduction)
        {
            targetDiameter -= reductionAmount;
        }
        else
        {
            targetDiameter = minimumReduction;
        }
 
        previousDecreaseEndtime = Time.time + reductionDuration;
    }

    void CheckPlayers ()
    {
        if(Time.time - previousPlayerTimeCheck > 1.0f)
        {
            previousPlayerTimeCheck = Time.time;
       
            foreach(PlayerController player in 
                GameManager.gameManagerInstance.players)
            {
               
                if(!player || player.isDead)                                        // Does not perform the check if                                    
                {                                                                   // the player is dead
                    continue;
                }
                    

                
                if(Vector3.Distance(Vector3.zero, player.transform.position)        // Checks if player is outside the barrier
                    >= transform.localScale.x)
                {
                    
                    player.photonView.RPC("TakeBulletDamage",                       // Applies damage to the player
                        player.photonPlayer, 0, outsideBarrierDamage);
                }
            }
        }
    }
}