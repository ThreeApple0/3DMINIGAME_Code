using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Firebase;
using Firebase.Database;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button ServerJoinbt;
    public Button GameStartbt;
    
    public Text ServerJoinTx;
    public Text PlayerNumTx;
    public Text NextGameTx;
    public Text OnlineStatTx;

    public Text VersionTx;
    public Text SerVerJoinStatTx;


    public GameObject LobbyPanel;
    public GameObject MatchPanel;
    public GameObject JoinServerPanel;
    public GameObject NextGamePanel;
    public GameObject TimeAtteckPanel;

    public GameObject TimeAtMapPn;
    public Text TimeAtMapTitle;

    public Text TimeAtGlobalT;

    public int RandomMap = 0;
    public GameObject QuickStartbt;
    public InputField NickNameIp;

    public string RoomInfo;
    public bool IsMob;
    public bool IsTimeAt = false;

    int TimeAtMapNum = 0;
    bool IsReLoad = true;
    bool IsReLoadINName = true;

    int count = 0;
    long strLen = 0;
    string[,] strRank = new string[1, 1];
    bool IsTimeAtshow = false;

    string NowVersion = "20210410";
    string NewVersion = "";
    bool IsVerCh = false;
    class Rank
    {
        // 순위 정보를 담는 Rank 클래스
        // Firebase와 동일하게 name, score, timestamp를 가지게 해야함
        public string name;

        public float goaltime;
        // JSON 형태로 바꿀 때, 프로퍼티는 지원이 안됨. 프로퍼티로 X

        public Rank(string name, float goaltime)
        {
            // 초기화하기 쉽게 생성자 사용
            this.name = name;

            this.goaltime = goaltime;
        }
    }
    public DatabaseReference reference { get; set; }


    void Awake()
    {
        if (IsMob)
        {
            Application.targetFrameRate = 60;
        }


        RoomInfo = "대기방";
        PhotonNetwork.SendRate = 40;
        PhotonNetwork.SerializationRate = 40;
    }
    void Start()
    {
        if(IsMob)
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
    void Update()
    {
        if(RoomInfo == "대기방")
        {
            ServerJoinTx.text = PhotonNetwork.NetworkClientState.ToString();
        }
        if(RoomInfo == "로비")
        {
            OnlineStatTx.text = "동접인원 : " + PhotonNetwork.CountOfPlayers + "  로비인원 : " + (PhotonNetwork.CountOfPlayers - PhotonNetwork.CountOfPlayersInRooms) + "  게임인원 : " + PhotonNetwork.CountOfPlayersInRooms;
        }   
        if (RoomInfo == "게임방")
        {
            PlayerNumTx.text = PhotonNetwork.CurrentRoom.PlayerCount + " / 20";
            if (PhotonNetwork.IsMasterClient)
            {
                QuickStartbt.SetActive(true);
            }
            else
            {
                QuickStartbt.SetActive(false);
            }
        }
        if (IsReLoad && PhotonNetwork.IsConnected)
        {
            IsReLoad = false;
            Hashtable LocalCP = PhotonNetwork.LocalPlayer.CustomProperties;
            LocalCP.Clear();
            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCP);
            JoinServerPanel.SetActive(false);
            MatchPanel.SetActive(false);
            LobbyPanel.SetActive(true);
            NextGamePanel.SetActive(false);
            PhotonNetwork.LeaveRoom();
            

        }
    }
    void LateUpdate()
    {
        if (IsTimeAtshow)
        {
            TimeAtLoadShow();
        }
        if (IsVerCh)
        {
            VersionChecked();
        }
    }
    public void GameReady()
    {
        GameStartbt.interactable = false;

        if(NickNameIp.text == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 100);
            NickNameIp.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = NickNameIp.text;
        }
        
        PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if (IsTimeAt)
        {
            PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName + "TIME" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 1 });
        }
        else
        {
            PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName + "!" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 20 });
        }
        
    }
    public override void OnJoinedRoom()
    {
        if (IsTimeAt)
        {
            Hashtable LocalCP = PhotonNetwork.LocalPlayer.CustomProperties;
            LocalCP.Add("GoalTime", -1f);
            LocalCP.Add("LocalNum", 0);
            LocalCP.Add("TimeMap", TimeAtMapNum);
            PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCP);

            Hashtable RoomCP = PhotonNetwork.CurrentRoom.CustomProperties;
            RoomCP.Add("GameRound", 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(RoomCP);

            if(TimeAtMapNum == 2)
            {
                PhotonNetwork.LoadLevel("RMAP1");
            }
            if(TimeAtMapNum == 3)
            {
                PhotonNetwork.LoadLevel("RMAP2");
            }
        }
        else
        {
            RoomInfo = "게임방";
            JoinServerPanel.SetActive(false);
            MatchPanel.SetActive(true);
            LobbyPanel.SetActive(false);
            NextGamePanel.SetActive(false);
        }
        
    }
    public void ServerJoin()
    {
        IsReLoad = false;
        IsReLoadINName = false;
        ServerJoinbt.interactable = false;
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        VersionCheck();


    }
    public void VersionCheck()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        reference.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                VersionCheck();
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log(NewVersion);
                IDictionary V = (IDictionary)snapshot.Value;
                NewVersion = V["version"].ToString();
                Debug.Log(NewVersion);
                IsVerCh = true;
            }  
        });
    }
    public void VersionChecked()
    {
        IsVerCh = false;
        if(NewVersion == NowVersion)
        {
            SerVerJoinStatTx.text = "최신버전임";
            VersionTx.text = "최신버전 : " + NewVersion;
            PhotonNetwork.JoinLobby();
        }
        else
        {
            SerVerJoinStatTx.text = "최신버전이 아님";
            VersionTx.text = "최신버전 : " + NewVersion + "   현재버전 : " + NowVersion;
        }
    }
    public override void OnJoinedLobby()
    {
        RoomInfo = "로비";
        JoinServerPanel.SetActive(false);
        MatchPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        NextGamePanel.SetActive(false);
        if (IsReLoadINName)
        {
            IsReLoadINName = false;
            NickNameIp.text = PhotonNetwork.LocalPlayer.NickName;
        }
        GameStartbt.interactable = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        RoomInfo = "대기방";
        JoinServerPanel.SetActive(true);
        MatchPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        NextGamePanel.SetActive(false);

        ServerJoinbt.interactable = true;
    }

    public void GameCancel()
    {
        RoomInfo = "로비";
        PhotonNetwork.LeaveRoom();
        JoinServerPanel.SetActive(false);
        MatchPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        NextGamePanel.SetActive(false);


    }
    public void GameStartBt()
    {
        if(PhotonNetwork.CurrentRoom.PlayerCount == 0)
        {
            return;
        }
        QuickStartbt.SetActive(false);
        PhotonNetwork.CurrentRoom.IsVisible = false;
        photonView.RPC("GameStart", RpcTarget.All);
        Invoke("MapShowR", 3);
        RandomMap = Random.Range(2, 4);
        //RandomMap = 3;
    }
    public void MapShowR()
    {
        photonView.RPC("MapShow", RpcTarget.All,RandomMap);
    }
    [PunRPC]
    private void GameStart()
    {
        JoinServerPanel.SetActive(false);
        MatchPanel.SetActive(false);
        LobbyPanel.SetActive(false);
        NextGamePanel.SetActive(true);

        Hashtable LocalCP = PhotonNetwork.LocalPlayer.CustomProperties;
        LocalCP.Add("GoalTime", -1f);
        PhotonNetwork.LocalPlayer.SetCustomProperties(LocalCP);
        
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            Hashtable playerCP = PhotonNetwork.PlayerList[i].CustomProperties;
            playerCP.Add("LocalNum", i);
            playerCP.Add("LocalScore", 0);
            PhotonNetwork.PlayerList[i].SetCustomProperties(playerCP);
            Debug.Log(i + " " + playerCP["LocalNum"]);
        }
        Hashtable RoomCP = PhotonNetwork.CurrentRoom.CustomProperties;
        RoomCP.Add("GameRound", 1);
        PhotonNetwork.CurrentRoom.SetCustomProperties(RoomCP);

        Invoke("GameScene", 6);
    }
    [PunRPC]
    private void MapShow(int ran)
    {
        RandomMap = ran;
        if(ran == 1)
        {
            NextGameTx.text = "맵 1";
        }
        if(ran == 2)
        {
            NextGameTx.text = "맵 2";
        }
        if (ran == 3)
        {
            NextGameTx.text = "맵 3";
        }
    }
    void GameScene()
    {
        if(RandomMap == 1)
        {
            PhotonNetwork.LoadLevel("MAP1");
        }
        if (RandomMap == 2)
        {
            PhotonNetwork.LoadLevel("RMAP1");
        }
        if (RandomMap == 3)
        {
            PhotonNetwork.LoadLevel("RMAP2");
        }
    }

    public void TimeAtteckOn()
    {
        IsTimeAt = true;
        TimeAtteckPanel.SetActive(true);
        if (NickNameIp.text == "")
        {
            PhotonNetwork.LocalPlayer.NickName = "Player" + Random.Range(0, 100);
            NickNameIp.text = PhotonNetwork.LocalPlayer.NickName;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = NickNameIp.text;
        }
    }
    public void TimeAtteckOff()
    {
        IsTimeAt = false;
        TimeAtteckPanel.SetActive(false);
    }

    public void TimeAtMap2()
    {
        TimeAtMapPn.SetActive(true);
        TimeAtMapTitle.text = "맵 2";
        TimeAtMapNum = 2;

        TimeAtGlobalT.text = "TOP 10 \n불러오는중...";
        TimeAttimeLoad();
    }
    public void TimeAtMap3()
    {
        TimeAtMapPn.SetActive(true);
        TimeAtMapTitle.text = "맵 3";
        TimeAtMapNum = 3;

        TimeAtGlobalT.text = "TOP 10 \n불러오는중...";
        TimeAttimeLoad();
    }
    public void TimeSAtMapExit()
    {
        TimeAtMapPn.SetActive(false);
        


    }
    public void TimeAtStart()
    {
        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName + "TIME" + Random.Range(0, 100), new RoomOptions { MaxPlayers = 1 });
        
    
    }
    void TimeAttimeLoad()
    {
        
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        
        reference.Child("rank").Child("map" + TimeAtMapNum.ToString()).OrderByChild("goaltime").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                TimeAttimeLoad();
                Debug.Log("!!");
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                count = 0;
                strLen = snapshot.ChildrenCount;
                strRank = new string[strLen, 10];
                Debug.Log(strLen);
                foreach (DataSnapshot data in snapshot.Children)
                {
                    Debug.Log("!!");
                    IDictionary rankInfo = (IDictionary)data.Value;
                    strRank[count, 0] = rankInfo["name"].ToString();
                    strRank[count, 1] = rankInfo["goaltime"].ToString();
                    Debug.Log(strRank[count, 1]);
                    count++;
                    
                }
                IsTimeAtshow = true;

            }
        });

        
    }
    void TimeAtLoadShow()
    {
        IsTimeAtshow = false;
        TimeAtGlobalT.text = "TOP 10\n";
        Debug.Log("!!");

        for (int i = 0; i < 10; i++)
        { 
            if (count <= i)
            {
                TimeAtGlobalT.text = TimeAtGlobalT.text + (i + 1) + " : 없음 \n";
            }
            else
            {
                float minutes = Mathf.FloorToInt(float.Parse(strRank[i, 1]) / 60);
                float seconds = Mathf.FloorToInt(float.Parse(strRank[i, 1]) % 60);
                TimeAtGlobalT.text = TimeAtGlobalT.text + (i + 1) + " : " + strRank[i, 0] + "  ||  " + minutes + " : " + seconds + " : " + Mathf.FloorToInt((float.Parse(strRank[i, 1]) - (60 * minutes) - seconds) * 100) + "\n";
            }

        }
    }
}
