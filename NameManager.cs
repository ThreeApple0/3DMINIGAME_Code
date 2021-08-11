using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class NameManager : MonoBehaviourPunCallbacks
{
    public PhotonView Pv;
    Camera MainCamera;
    public Text NickName;
    // Start is called before the first frame update
    void Start()
    {
        MainCamera = Camera.main;
        NickName.text = Pv.Owner.NickName;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(transform.position + MainCamera.transform.rotation * Vector3.forward, 
            MainCamera.transform.rotation * Vector3.up);
    }
}
