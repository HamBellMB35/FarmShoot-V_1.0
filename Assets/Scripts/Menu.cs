using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks
{
    [Header("Menu Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowsingScreen;

    [Header("Main Screen Buttons")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("Lobby Browsing Screen")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    void Start ()
    {
                                                                                                // Disables the menu buttons at the start
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        Cursor.lockState = CursorLockMode.None;                                                 // Enables the cursor

                                                                                                
        if(PhotonNetwork.InRoom)                                                                // Checks if we are in a room
        {
                                                                                                // Changes screen to the lobby
            SetScreen(lobbyScreen);
            UpdateLobbyUI();

                                                                                                // Makes the room visible 
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

                                                                                                // Method that changes current visible screen
    void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);                                                            // Disables all the other screens
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowsingScreen.SetActive(false);
                                                   
        screen.SetActive(true);                                                                 // Enables the requested screen

        if (screen == lobbyBrowsingScreen)                                                      // Updates the lobby browsing UI if we are in that screen
        {
            UpdateLobbyBrowserUI();
        }
    }
 
                                                                                                // Calls this method when the Back button is pressed
    public void OnBackButton()
    {
        SetScreen(mainScreen);
    }

    #region MAIN_SCREEN
    

                                                                                                // This method gets called when the player name input field has been changed
    public void OnPlayerNameValueChanged (TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

                                                                                                // This method gets called when we connect to the master server
    public override void OnConnectedToMaster ()
    {
                                                                                                // Enables the menu buttons after we connect to the server
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }

                                                                                                // This method gets called when we click on the create room button
    public void OnCreateRoomButton ()
    {
        SetScreen(createRoomScreen);
    }

                                                                                                // This method gets called when we click on the find room button
    public void OnFindRoomButton ()
    {
        SetScreen(lobbyBrowsingScreen);
    }
    #endregion

    #region CREATE_ROOM_SCREEN

    

    public void OnCreateButton(TMP_InputField roomNameInput)
    {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    #endregion

    #region LOBBY_SCREEN
    
                                                                                                // This method is called when we join a room
                                                                                                // It sets the current screen y and updates the UI for all players
    public override void OnJoinedRoom ()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

                                                                                                // Updates the lobby UI when a plyer leaves the room
    public override void OnPlayerLeftRoom (Player otherPlayer)
    {
        UpdateLobbyUI();
    }

                                                                                                 // Updates the  player list and buttons
    [PunRPC]
    void UpdateLobbyUI ()
    {
                                                                                                 // Enables or disables the start game button based on wether if we
        startGameButton.interactable = PhotonNetwork.IsMasterClient;                             // are the host or not



        playerListText.text = "";                                                               // Clears the player list text

        foreach(Player player in PhotonNetwork.PlayerList)                                      // Refreshes the player list text with the update player list
            playerListText.text += player.NickName + "\n";

        
        roomInfoText.text = "<b>Room Name</b>\n" 
            + PhotonNetwork.CurrentRoom.Name;                                                   // Updates the room info text
    }

                                                                                                // This method iscalled when the Start Game button is pressed
    public void OnStartGameButton ()
    {
                                                                                                // Hides the room for other players not in the game
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

                                                                                                // Tells all players to load into the Game scene
        NetworkManager.instance.photonView.
            RPC("ChangeScene", RpcTarget.All, "Demo");
    }

                                                                                                // This method is called when the "Leave Lobby" button has been pressed
    public void OnLeaveLobbyButton ()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }
    #endregion

    #region LOBBY_BROWSING_SCREEN

    GameObject CreateRoomButton ()
    {
        GameObject buttonGameObject = Instantiate(roomButtonPrefab,                             // Instantiates a button                             
            roomListContainer.transform);

        roomButtons.Add(buttonGameObject);                                                      // Adds the button to the roomButtons array

        return buttonGameObject;
    }

    void UpdateLobbyBrowserUI ()
    {
                                                                                                // Disables all room buttons
        foreach(GameObject button in roomButtons)
        {
            button.SetActive(false);
        }
                                                                                               
        for(int roomIndex = 0; roomIndex < roomList.Count; ++roomIndex)                         // Loop based on roomlist.Cont tp displays
        {                                                                                        // all current rooms in the master server

            GameObject button = roomIndex >=
                roomButtons.Count ? CreateRoomButton() : roomButtons[roomIndex];                // Either creates or gets a button object

            button.SetActive(true);

                                                                                                // Sets the room name 
            button.transform.Find("Room name text").
                GetComponent<TextMeshProUGUI>().text = roomList[roomIndex].Name;

            button.transform.Find("Player count text").                                         // Sets the player count text
                GetComponent<TextMeshProUGUI>().text = 
                roomList[roomIndex].PlayerCount + " / " + roomList[roomIndex].MaxPlayers;

                                                                                                
            Button buttonComp = button.GetComponent<Button>();                                  // Gets the button component

            string roomName = roomList[roomIndex].Name;                         

            buttonComp.onClick.RemoveAllListeners();                                            // Removes all listeners in case the button
                                                                                                // has been used before
            buttonComp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });              // Adds a mew button listener
        }
    }

    public void OnJoinRoomButton (string roomName)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public void OnRefreshButton ()
    {
        UpdateLobbyBrowserUI();
    }

    public override void OnRoomListUpdate (List<RoomInfo> allRooms)
    {
        roomList = allRooms;
    }

    #endregion
}