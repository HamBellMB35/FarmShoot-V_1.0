using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class UI : MonoBehaviour
{
    public static UI instanceUI;
    public TextMeshProUGUI playerInfo;
    public TextMeshProUGUI playerWin;
    public TextMeshProUGUI ammoCount;
    public Image playerWinsImage;
    public Slider healthBar;

    private PlayerController player;

    void Awake ()
    {
        instanceUI = this;
    }

    public void Initialize (PlayerController localPlayer)
    {
        player = localPlayer;
        healthBar.maxValue = player.maxHealthPoints;
        healthBar.value = player.healthPoints;
        UpdateAmmo();
        UpdatePlayerInfo();
        
    }

    public void UpdateHealthBar ()
    {
        healthBar.value = player.healthPoints;
    }

    public void UpdatePlayerInfo ()
    {
        playerInfo.text = "<b>Alive:</b> " + 
            GameManager.gameManagerInstance.numOfAlivePlayers +
            "\n<b>Kills:</b> " + player.numberOfKills;
    }

    public void UpdateAmmo ()
    {
        ammoCount.text = player.playerWeapon.currentAmmo + " / " + player.playerWeapon.maxAmmoCount;
    }

    public void SetWinText (string theWinnersName)
    {
        playerWinsImage.gameObject.SetActive(true);
        playerWin.text = theWinnersName + " wins!";
    }
}