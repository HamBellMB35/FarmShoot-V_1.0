using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxAmmoCount;
    public int currentAmmo;
    public float bulletSpeed;
    public float rateOfFire;
    public int damagePerBullet;

    public AudioClip shotSound;
    public AudioClip roosterSound;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPosition;

    private PlayerController player;
    private float previousShotTime;

    void Awake()
    {

        player = GetComponent<PlayerController>();
        AudioSource.PlayClipAtPoint(roosterSound, transform.position, 1.0f);
    }

    public void TryShooting()
    {
        
        if (currentAmmo <= 0 || Time.time - previousShotTime < rateOfFire)                              // Condition to check if we can shoot
        {
            return;
        }                      
 
        currentAmmo--;
        previousShotTime = Time.time;

        UI.instanceUI.UpdateAmmo();                                                                     // Updates the ammo count in the UI

      
        player.photonView.RPC("SpawnABullet",                                                           // Calls method to spawn a buscrpt. Updates to all players
            RpcTarget.All, bulletSpawnPosition.position,
            Camera.main.transform.forward);
    }

    [PunRPC]
    void SpawnABullet(Vector3 position, Vector3 direction)
    {
        GameObject bulletObject = Instantiate(bulletPrefab, position, Quaternion.identity);
        bulletObject.transform.forward = direction;

        BulletScript buscrpt = bulletObject.GetComponent<BulletScript>();

        buscrpt.Initialize(damagePerBullet, player.id, player.photonView.IsMine);                       // Initializes the buscrpt and sets its direction
        buscrpt.rb.velocity = direction * bulletSpeed;

        AudioSource.PlayClipAtPoint(shotSound, transform.position, 1.0f);
    }

    [PunRPC]
    public void IncreaseAmmo(int ammoToGive)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + ammoToGive, 0, maxAmmoCount);                           // Adds bullets up to the max bullet ammount

        UI.instanceUI.UpdateAmmo();                                                                     // Updates the ammo count in the UI
    }
}
