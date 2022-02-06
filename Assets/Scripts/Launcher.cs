using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";
    private const string roomNormalId = "Mrl666";
    
    
    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";
    
    /// <summary>
    /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
    /// </summary>
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 0;

    public GameObject MainCanvas;

    public Text tipText;
    public GameObject joinBtn;
    public GameObject exitBtn;
    public GameObject GameUI;

    public GameObject prefabShowBtn;
    public GameObject prefabHideBtn;
    public GameObject prefabScroll;


    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    [Tooltip("player Start Point")]
    public Vector3 playerInitPos;
    
    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    bool isConnecting;
    
    
    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
        
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(MainCanvas);
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        string defaultName = string.Empty;
        if (nameInput != null)
        {
            if (PlayerPrefs.HasKey(playerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                nameInput.text = defaultName;
            }
        }

        PhotonNetwork.NickName = defaultName;
    }
    
    
    
    /// <summary>
    /// Start the connection process.
    /// - If already connected, we attempt joining a random room
    /// - if not yet connected, Connect this application instance to Photon Cloud Network
    /// </summary>
    public void Connect()
    {
        tipText.text = "join the room...";
        joinBtn.gameObject.SetActive(false);
        nameInput.gameObject.SetActive(false);

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.IsConnected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
            // PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinRoom(roomNormalId);
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        
        tipText.text = "exit the room...";

        isConnecting = true;
    }





    /// <summary>
    /// Game game
    /// </summary>
    public InputField nameInput;






































    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(roomNormalId, new RoomOptions(){ MaxPlayers = maxPlayersPerRoom });
        
        tipText.text = "Failed to join room randomly.\ncreate room automatically.";

    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        tipText.text = "Failed to create room.";
    }

    
    public override void OnJoinedRoom()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        
        tipText.text = "Join the room successfully.";
        
        exitBtn.SetActive(true);

        prefabShowBtn.SetActive(true);
        prefabHideBtn.SetActive(false);
        prefabScroll.SetActive(false);

        GameUI.SetActive(true);
        
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'",this);
        }
        else
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", Application.loadedLevelName);
            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
            Transform player = PhotonNetwork.Instantiate(this.playerPrefab.name, playerInitPos, Quaternion.identity, 0).transform;
            MapCameraControl.ins.SetPlayer(player);
        }
    }
    
    
    
    public override void OnConnectedToMaster()
    {
        // we don't want to do anything if we are not attempting to join a room.
        // this case where isConnecting is false is typically when you lost or quit the game, when this level is loaded, OnConnectedToMaster will be called, in that case
        // we don't want to do anything.
        if (isConnecting)
        {
            isConnecting = false;
            return;
        }
        
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
        
        tipText.text = "Server connection succeeded";

    }


    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        
        tipText.text = "Server disconnected";

        joinBtn.gameObject.SetActive(true);
        nameInput.gameObject.SetActive(true);
        
        exitBtn.SetActive(false);
        GameUI.SetActive(false);
    }

    
    
    
    
    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {        
        Debug.Log("PUN Basics leftroom.");

        tipText.text = "";

        joinBtn.gameObject.SetActive(true);
        nameInput.gameObject.SetActive(true);

        exitBtn.SetActive(false);
        GameUI.SetActive(false);


        // SceneManager.LoadScene(0);
    }
    
     /// <summary>
    /// Sets the name of the player, and save it in the PlayerPrefs for future sessions.
    /// </summary>
    /// <param name="value">The name of the Player</param>
    public void SetPlayerName(string value)
    {
        // #Important
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;

        PlayerPrefs.SetString(playerNamePrefKey, value);
    }
    
    
    
    
    // void LoadArena()
    // {
    //     if (!PhotonNetwork.IsMasterClient)
    //     {
    //         Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
    //     }
    //     Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
    //     // PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    // }
    //
    // public override void OnPlayerEnteredRoom(Player other)
    // {
    //     Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting
    //
    //
    //     if (PhotonNetwork.IsMasterClient)
    //     {
    //         Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
    //         
    //         LoadArena();
    //     }
    // }
    //
    //
    // public override void OnPlayerLeftRoom(Player other)
    // {
    //     Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects
    //
    //
    //     if (PhotonNetwork.IsMasterClient)
    //     {
    //         Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
    //         
    //         LoadArena();
    //     }
    // }
}
