using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    public GameObject RoundPn;
    public GameObject SumPn;
    public GameObject NextPn;

    public Button LobbyBt;
    public Button ReStartBt;

    Hashtable playerCP;
    public Text RoundNumTx;
    public Text ScoreTimeTx;
    public Text NextGameTx;
    public Text ScoreSumTx;
    public Text SumInfoTx;
    float[,] PlayerByScore = new float[21, 2];
    float[,] PlayerBySum = new float[21, 2];
    int RandomMap = 0;
    bool IsEnded = false;
    // Start is called before the first frame update
    void Start()
    {
        Hashtable RoomCP = PhotonNetwork.CurrentRoom.CustomProperties;

        RoundPn.SetActive(true);
        SumPn.SetActive(false);
        NextPn.SetActive(false);
        //RoundNumTx.text = RoomCP["GameRound"].ToString() + "라운드 결과";
        Invoke("ScoreTimeShow", 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void ScoreTimeShow()
    {
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerCP = PhotonNetwork.PlayerList[i].CustomProperties;
            Debug.Log(playerCP["LocalNum"]);
            Debug.Log(playerCP["GoalTime"]);
            Debug.Log(float.Parse(playerCP["LocalNum"].ToString()));
            PlayerByScore[i, 0] = float.Parse(playerCP["LocalNum"].ToString());
            PlayerByScore[i, 1] = float.Parse(playerCP["GoalTime"].ToString());
            

        }
        float temp;
        
        for(int i = 0;i< PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            for(int j = 0;j< PhotonNetwork.CurrentRoom.PlayerCount - 1; j++)
            {
                if ((PlayerByScore[j, 1] > PlayerByScore[j + 1, 1] && PlayerByScore[j + 1, 1] != -1f) || (PlayerByScore[j,1] == -1f && PlayerByScore[j+1, 1] != -1f))
                {
                    temp = PlayerByScore[j, 1];
                    PlayerByScore[j, 1] = PlayerByScore[j + 1, 1];
                    PlayerByScore[j + 1, 1] = temp;

                    temp = PlayerByScore[j, 0];
                    PlayerByScore[j, 0] = PlayerByScore[j + 1, 0];
                    PlayerByScore[j + 1, 0] = temp;
                }
            }
        }

        string NickName = "";
        float minutes;
        float seconds;
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            for(int j = 0; j < PhotonNetwork.CurrentRoom.PlayerCount; j++)
            {
                playerCP = PhotonNetwork.PlayerList[j].CustomProperties;
                if(PlayerByScore[i,0] == float.Parse(playerCP["LocalNum"].ToString()))
                {
                    NickName = PhotonNetwork.PlayerList[j].NickName;
                    if(PhotonNetwork.PlayerList[j] == PhotonNetwork.LocalPlayer)
                    {
                        NickName = NickName + " < Me >";
                    }
                    if(PlayerByScore[i, 1] == -1f || PlayerByScore[i, 1] > PlayerByScore[0, 1] + 20f)
                    {

                    }
                    else if(PhotonNetwork.IsMasterClient)
                    {
                        playerCP["LocalScore"] = int.Parse(playerCP["LocalScore"].ToString()) + (PhotonNetwork.CurrentRoom.PlayerCount - i);
                        PhotonNetwork.PlayerList[j].SetCustomProperties(playerCP);
                    }
                }
            }
            if(PlayerByScore[i,1] == -1f || PlayerByScore[i,1] > PlayerByScore[0,1] + 20f)
            {
                //ScoreTimeTx.text = ScoreTimeTx.text + "X    :  " + NickName + "   ||   완주 실패  ||  + 0" + "\n";
                ScoreTimeTx.text = ScoreTimeTx.text + "X    :  " + NickName + "   ||   완주 실패" + "\n";
            }
            else
            {
                minutes = Mathf.FloorToInt(PlayerByScore[i, 1] / 60);
                seconds = Mathf.FloorToInt(PlayerByScore[i, 1] % 60);

                //ScoreTimeTx.text = ScoreTimeTx.text + (i + 1).ToString() + "등  :  " + NickName + "   ||   " + minutes + " : " + seconds + " : " + Mathf.FloorToInt((PlayerByScore[i, 1] - (60 * minutes) - seconds) * 1000) + "  ||  + " + (PhotonNetwork.CurrentRoom.PlayerCount - i) + "\n";
                ScoreTimeTx.text = ScoreTimeTx.text + (i + 1).ToString() + "등  :  " + NickName + "   ||   " + minutes + " : " + seconds + " : " + Mathf.FloorToInt((PlayerByScore[i, 1] - (60 * minutes) - seconds) * 1000) + "\n";
            }
            
        }
        LobbyBt.interactable = true;
        ReStartBt.interactable = true;
        //Invoke("SumScoreShowRd", 7);

    }

    void SumScoreShowRd()
    {
        Hashtable RoomCP = PhotonNetwork.CurrentRoom.CustomProperties;
        
        RoundPn.SetActive(false);
        SumPn.SetActive(true);
        NextPn.SetActive(false);
        if (int.Parse(RoomCP["GameRound"].ToString()) == 3)
        {
            IsEnded = true;
            SumInfoTx.text = "|| 최종 점수 ||";
        }
        Invoke("SumScoreShow", 2);

    }

    void SumScoreShow()
    {
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            playerCP = PhotonNetwork.PlayerList[i].CustomProperties;
            PlayerBySum[i, 0] = float.Parse(playerCP["LocalNum"].ToString());
            PlayerBySum[i, 1] = float.Parse(playerCP["LocalScore"].ToString());


        }
        float temp;

        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            for (int j = 0; j < PhotonNetwork.CurrentRoom.PlayerCount - 1; j++)
            {
                if (PlayerBySum[j, 1] < PlayerBySum[j + 1, 1])
                {
                    temp = PlayerBySum[j, 1];
                    PlayerBySum[j, 1] = PlayerBySum[j + 1, 1];
                    PlayerBySum[j + 1, 1] = temp;

                    temp = PlayerBySum[j, 0];
                    PlayerBySum[j, 0] = PlayerBySum[j + 1, 0];
                    PlayerBySum[j + 1, 0] = temp;
                }
            }
        }
        string NickName = "";
        for (int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            for (int j = 0; j < PhotonNetwork.CurrentRoom.PlayerCount; j++)
            {
                playerCP = PhotonNetwork.PlayerList[j].CustomProperties;
                if (PlayerBySum[i, 0] == float.Parse(playerCP["LocalNum"].ToString()))
                {
                    NickName = PhotonNetwork.PlayerList[j].NickName;
                    if (PhotonNetwork.PlayerList[j] == PhotonNetwork.LocalPlayer)
                    {
                        NickName = NickName + " < Me >";
                    }
                }
            }
            ScoreSumTx.text = ScoreSumTx.text + NickName + "  :  " + PlayerBySum[i, 1] + "\n";
        }
        if (IsEnded)
        {
            Invoke("GoToLobby", 7);
        }
        else
        {
            Invoke("NextGameR", 7);
        }
        
    }
    void NextGameR()
    {
        RoundPn.SetActive(false);
        SumPn.SetActive(false);
        NextPn.SetActive(true);

        Invoke("GameScene", 6);
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable RoomCP = PhotonNetwork.CurrentRoom.CustomProperties;
            RoomCP["GameRound"] = int.Parse(RoomCP["GameRound"].ToString()) + 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(RoomCP);

            Invoke("MapShowR", 3);
            RandomMap = Random.Range(2, 4);

        }
        
    }
    public void MapShowR()
    {
        photonView.RPC("MapShow", RpcTarget.All, RandomMap);
    }
    [PunRPC]
    private void MapShow(int ran)
    {
        RandomMap = ran;
        if (ran == 1)
        {
            NextGameTx.text = "맵 1";
        }
        if (ran == 2)
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
        if (RandomMap == 1)
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
    public void GoToLobby()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
