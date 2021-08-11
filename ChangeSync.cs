using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSync : MonoBehaviourPun, IPunObservable
{
    Rigidbody rb;
    Vector3 latestPos;
    Quaternion latestRot;
    Vector3 velocity;
    Vector3 angularVelocity;

    bool vaRed = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!photonView.IsMine && vaRed)
        {
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(rb.velocity);
            stream.SendNext(rb.angularVelocity);
        }
        else
        {
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();
            vaRed = true;
        }
    }

    void OnCollisionStay(Collision contact)
    {
        
        if (!photonView.IsMine)
        {
            
            Transform collisionObjectRoot = contact.transform.root;
            if (collisionObjectRoot.CompareTag("Player"))
            {
                
                //Transfer PhotonView of Rigidbody to our local player
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }
        }
    }

}
