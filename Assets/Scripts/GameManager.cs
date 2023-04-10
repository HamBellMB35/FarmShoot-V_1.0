using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun
{
    public float waitAfterWinTime;

    [Header("Players Info")]
    public string playerPrefabLocation;                                                             // Player prefab location is a string because Photon 
                                                                                                    // uses a path to the prefab in the resource folder
    public PlayerController[] players;
    public Transform[] playerSpawnPoints;
    public static GameManager gameManagerInstance;                                                  // Game manager instanceUI
    public int numOfAlivePlayers;

    private int playersInGame;

    void Awake()
    {
        gameManagerInstance = this;
    }

    void Start()
    {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        numOfAlivePlayers = players.Length;

        photonView.RPC("IHaveLoadedInGame", RpcTarget.AllBuffered);                                 // We use all bufferd incase a player is still loading in
    }

    [PunRPC]
    void IHaveLoadedInGame()
    {
        playersInGame++;

        if(PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length)        // Spawns players after all have loaded in
            photonView.RPC("SpawnPlayer", RpcTarget.All);
    }

    [PunRPC]
    void SpawnPlayer()
    {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation,                      // Instantiates a plyer at a randomly chosen spawn location
            playerSpawnPoints[Random.Range
            (0, playerSpawnPoints.Length)].position, Quaternion.identity);

        playerObj.GetComponent<PlayerController>().                                                 // Initializes the player for all other players connected
            photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer(int playerId)                                                // Returns the player matched by player Id
    {
        foreach(PlayerController player in players)
        {
            if(player != null && player.id == playerId)
                return player;
        }

        return null;
    }

    public PlayerController GetPlayer(GameObject playerObject)                                    // Returns the player matched by game object
    {
        foreach(PlayerController player in players)
        {
            if(player != null && player.gameObject == playerObject)
                return player;
        }

        return null;
    }

    public void CheckIfSomeoneWon()
    {
        if(numOfAlivePlayers == 1)
        {
            photonView.RPC("GameWon", RpcTarget.All,                                                // If the player is not dead we return
                players.First(player => !player.isDead).id);                                        // the first(and only) player's id thats not dead
        }
            
    }

    [PunRPC]
    void GameWon(int playerWhowon)
    {
        
        UI.instanceUI.SetWinText(GetPlayer(playerWhowon).photonPlayer.NickName);

        Invoke("BackToMainMenu", waitAfterWinTime);
    }

    void BackToMainMenu()
    {
        NetworkManager.instance.ChangeScene("Menu");
    }
}