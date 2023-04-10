using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxAmmoCount;
    public int currentAmmo;
    public float bulletSpeed;
    public float rateOfFire;
    public int damagePerBullet;

    private float previousShotTime;

    public GameObject bulletPrefab;
    public Transform bulletSpawnPosition;

    private PlayerController player;

    void Awake ()
    {

        player = GetComponent<PlayerController>();
    }

    public void TryShoot ()
    {
        // can we shoot?
        if(currentAmmo <= 0 || Time.time - previousShotTime < rateOfFire)
            return;

        currentAmmo--;
        previousShotTime = Time.time;

        // update the ammo UI
        UI.instanceUI.UpdateAmmo();

        // spawn the bullet
        player.photonView.RPC("SpawnABullet", RpcTarget.All, bulletSpawnPosition.position, Camera.main.transform.forward);
    }

    [PunRPC]
    void SpawnBullet (Vector3 pos, Vector3 dir)
    {
        // spawn and orientate it
        GameObject bulletObj = Instantiate(bulletPrefab, pos, Quaternion.identity);
        bulletObj.transform.forward = dir;

        // get bullet script
        BulletScript bulletScript = bulletObj.GetComponent<BulletScript>();

        // initialize it and set the velocity
        bulletScript.Initialize(damagePerBullet, player.id, player.photonView.IsMine);
        bulletScript.rb.velocity = dir * bulletSpeed;
    }

    [PunRPC]
    public void GiveAmmo (int ammoToGive)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + ammoToGive, 0, maxAmmoCount);

        // update the ammo text
        UI.instanceUI.UpdateAmmo();
    }
}