using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public int maxNumOfPlayers = 15;

                                                                           
    public static NetworkManager instance;                                  // Instance of the network manager

    void Awake ()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start ()
    {
                                                                            // Connects to the master server
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster ()                             // Overrides default base onConnectedToMaster
    {
        
        PhotonNetwork.JoinLobby();
   
    }

  
    public void CreateRoom (string roomName)
    {
        RoomOptions options = new RoomOptions();                            // Sets the value of Maxplayers equals to maxNumOfPlayers
        options.MaxPlayers = (byte)maxNumOfPlayers;

        PhotonNetwork.CreateRoom(roomName, options);                        // Creates a room with the given options
    }
    
                                                                            // Joins a room of the requested room name
    public void JoinRoom (string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

                                                                           
    [PunRPC]                                                                 // Changes the scene through Photon's system
    public void ChangeScene (string sceneName)
    {
        PhotonNetwork.LoadLevel(sceneName);
    }

                                                                             // This gets called when we disconnect from the Photon server
    public override void OnDisconnected (DisconnectCause cause)
    {
        PhotonNetwork.LoadLevel("Menu");
    }

    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        GameManager.gameManagerInstance.numOfAlivePlayers--;
        UI.instanceUI.UpdatePlayerInfo();

        if(PhotonNetwork.IsMasterClient)
        {
            GameManager.gameManagerInstance.CheckIfSomeoneWon();
        }
    }
}