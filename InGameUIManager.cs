using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class InGameUIManager : MonoBehaviourPunCallbacks
{
    public GameObject SettingPanel;
    public Slider AimSlider;
    public float AimSpeed;
    public GameObject TimeAtReBt;
    public bool IsTimeAt = false;
    void Start()
    {
        Hashtable LocalCP = PhotonNetwork.CurrentRoom.CustomProperties;
        if(LocalCP["GameRound"].ToString() == "0")
        {
            
            IsTimeAt = true;
        }
    }

    public void SettingButtonOn()
    {
        SettingPanel.SetActive(true);
        if (TimeAtReBt == null)
        {
            TimeAtReBt = GameObject.Find("TimeAtRe");
        }
        if(IsTimeAt)
        {
            TimeAtReBt.SetActive(true);
        }
        else
        {
            TimeAtReBt.SetActive(false);
        }
    }
    public void SettingExit()
    {
        SettingPanel.SetActive(false);
    }
    public void AimChanged()
    {
        AimSpeed = AimSlider.value;
    }
    public void TimeReStart()
    {
        Hashtable LocalCP = PhotonNetwork.LocalPlayer.CustomProperties;
        if(LocalCP["TimeMap"].ToString() == "2")
        {
            PhotonNetwork.LoadLevel("RMAP1");
        }
        if (LocalCP["TimeMap"].ToString() == "3")
        {
            PhotonNetwork.LoadLevel("RMAP2");
        }
    }
    public void GoLobby()
    {
        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
