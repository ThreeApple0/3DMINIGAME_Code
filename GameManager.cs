using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;

using Hashtable = ExitGames.Client.Photon.Hashtable;
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject[] SpawnPoint;
    public Text PingTx;
    public Text StartTimeTx;
    public Text LocalTimeTx;

    public GameObject[] CheckPoints;

    public float GoalTime;
    public float FirstGoalTime;
    public float LocalGametime = 0;
    public float ServerGametime = 0;
    public float StartCount = 4;
    public bool GameStarted = false;
    public bool GameEnded = false;
    public int IsGoal = 0;
    public int OneIsGoal = 0;
    public int TextStat = 0;
    public bool IsTimeAt = false;



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



    // Start is called before the first frame update
    void Awake()
    {
        
    }

    void Start()
    {
        reference = FirebaseDatabase.DefaultInstance.RootReference;

        Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
        playerCP["GoalTime"] = -1f;
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerCP);
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable roomCP = PhotonNetwork.CurrentRoom.CustomProperties;
            if (roomCP["GameRound"].ToString() == "0")
            {
                IsTimeAt = true;
            }
        }
        

        if (PhotonNetwork.IsMasterClient)
            for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
            {
                
                photonView.RPC("PlayerSpawn",PhotonNetwork.PlayerList[i],SpawnPoint[i].transform.position);
            }
        
    }

    // Update is called once per frame
    void Update()
    {
        LocalGametime += Time.deltaTime;
        PingTx.text = "Ping : " + PhotonNetwork.GetPing().ToString();
        if (PhotonNetwork.IsMasterClient)
        {
            if (GameStarted)
            {
                ServerGametime = LocalGametime + 3;
            }
            else
            {
                ServerGametime = LocalGametime;
            }
        }
        float minutes = Mathf.FloorToInt(LocalGametime / 60);
        float seconds = Mathf.FloorToInt(LocalGametime % 60);
        if(GameStarted)
            LocalTimeTx.text = minutes + " : " + seconds + " : " + Mathf.FloorToInt((LocalGametime - (60 * minutes) - seconds) * 100);
        if (GameStarted && LocalGametime >= 3 && TextStat == 0)
        {
            StartTimeTx.text = "";
            TextStat = 1;
        }

        if (!GameStarted)
        {
            if (Mathf.CeilToInt(3 - ServerGametime) != StartCount && !GameStarted)
            {
                StartCount = Mathf.CeilToInt(3 - ServerGametime);
                
                if(StartCount <= 0)
                {
                    StartTimeTx.color = new Color(233 / 255f, 200 / 255f, 96 / 255f, 100 / 255f);
                    StartTimeTx.text = "GO";
                    GameStarted = true;
                    LocalGametime = 0;
                }
                else
                {
                    StartTimeTx.text = StartCount.ToString();
                }
            }
        }
        if(IsGoal == 1)
        {
            GoalTime = LocalGametime;
            Hashtable playerCP = PhotonNetwork.LocalPlayer.CustomProperties;
            playerCP["GoalTime"] = GoalTime;
            PhotonNetwork.LocalPlayer.SetCustomProperties(playerCP);
            Debug.Log(playerCP["GoalTime"]);
            IsGoal = 2;
            TextStat = 2;
            StartTimeTx.color = new Color(255 / 255f, 236 / 255f, 0 / 255f, 255 / 255f);
            if (IsTimeAt)
            {
                StartTimeTx.text = "완주";
                Invoke("ToTimeAtScene", 3);
                return;
            }
            if (OneIsGoal == 1)
            {
                StartTimeTx.text = "완주";
            }
            else
            {
                StartTimeTx.text = "1등";
            }
            
            if(OneIsGoal == 0)
            {
                photonView.RPC("OneGoal", RpcTarget.All, GoalTime);
            }
        }
        if (IsTimeAt)
        {
            return;
        }
        if(TextStat == 2)
        {
            if(LocalGametime - GoalTime >= 7 && !GameEnded)
            {
                StartTimeTx.text = "";
                TextStat = 3;
            }
        }
        if ( OneIsGoal != 0 && !GameEnded)
        {
            if (Mathf.CeilToInt(20 - LocalGametime + FirstGoalTime) != StartCount)
            {
                StartCount = Mathf.CeilToInt(20 - LocalGametime + FirstGoalTime);

                if (StartCount <= 0)
                {
                    if(IsGoal != 2)
                    {
                        StartTimeTx.color = new Color(190 / 255f, 48 / 255f, 84 / 255f, 250 / 255f);
                        StartTimeTx.text = "완주 실패";
                    }
                    else
                    {
                        StartTimeTx.color = new Color(255 / 255f, 236 / 255f, 0 / 255f, 255 / 255f);
                        StartTimeTx.text = "게임 종료";
                    }
                    GameEnded = true;
                    Invoke("ToScoreScene", 3);
                }
                else
                {
                    if (IsGoal != 2)
                    {
                        StartTimeTx.color = new Color(190/255f,48 / 255f, 84 / 255f, 100 / 255f);
                        StartTimeTx.text = StartCount.ToString();
                    }
                        
                }
            }
        }
       
    }
    [PunRPC]
    private void PlayerSpawn(Vector3 SpawnPos)
    {
        
        PhotonNetwork.Instantiate("PlayerTT", SpawnPos,new Quaternion(0,180,0,0));
    }

    [PunRPC]
    private void OneGoal(float rectime)
    {
        StartCount = 21;
        FirstGoalTime = rectime;
        OneIsGoal = 1;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(ServerGametime);
        }
        else
        {
            ServerGametime = (float)stream.ReceiveNext();
        }
    }

    void ToScoreScene()
    {
        PhotonNetwork.LoadLevel("ScoreAndNext");
    }
    void ToTimeAtScene()
    {
        //PhotonNetwork.LoadLevel("TimeAtScore");
        float minutes = Mathf.FloorToInt(GoalTime / 60);
        float seconds = Mathf.FloorToInt(GoalTime % 60);
        StartTimeTx.text = minutes + " : " + seconds + " : " + Mathf.FloorToInt((GoalTime - (60 * minutes) - seconds) * 100);

        Rank rank = new Rank(PhotonNetwork.NickName, GoalTime);
        string json = JsonUtility.ToJson(rank);

        Hashtable LocalCP = PhotonNetwork.LocalPlayer.CustomProperties;

        if (LocalCP["TimeMap"].ToString() == "2")
        {
            string key = reference.Child("rank").Child("map2").Push().Key;
            reference.Child("rank").Child("map2").Child(key).SetRawJsonValueAsync(json);
        }
        if (LocalCP["TimeMap"].ToString() == "3")
        {
            string key = reference.Child("rank").Child("map3").Push().Key;
            reference.Child("rank").Child("map3").Child(key).SetRawJsonValueAsync(json);
        }
        

        Invoke("GOLOBBY", 7);

    }
    void GOLOBBY()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
