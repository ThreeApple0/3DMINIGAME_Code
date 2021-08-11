using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScriptT : MonoBehaviourPunCallbacks, IPunObservable
{
    public LayerMask layerMask;
    public float AimSpeed;
    public float movespeed;
    public float jumpspeed;
    public float gravity = -0.4f;
    public float grav;
    public bool chisground;
    Vector2 moveInput;
    Vector3 curPos;

    public bool jdown;
    public bool isJump;

    public PhotonView Pv;
    GameObject cameraarmGO;
    Transform cameraarm;
    Transform chbody;
    Collider colider;
    
    MeshRenderer meshRenderer;
    Rigidbody rb;
    
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        chbody = GetComponent<Transform>();
        
        cameraarmGO = GameObject.Find("CameraArm");
        cameraarm = cameraarmGO.transform;
        colider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();

        //if (!Pv.IsMine)
            //gameObject.layer = LayerMask.NameToLayer("JUMPON");
        
    }
    
    // Update is called once per frame
    void Update()
    {
        //if (Pv.IsMine)
        //{

            Getbutton();
            LookAround();

        IsGround();



        Movee();
        IsGround();

        Jumpp();

        Gravityy();
        IsGround();
        if (chisground == true)
        {

            grav = -0.3f;
        }

        //if (chisground && rb.velocity.y < 0) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        //if (Mathf.Abs(rb.velocity.x) <= 0.1) rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        //if (Mathf.Abs(rb.velocity.y) <= 0.1) rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        //if (Mathf.Abs(rb.velocity.z) <= 0.1) rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);


        cameraarm.transform.position = chbody.transform.position;

        //}
        /*else if ((transform.position - curPos).sqrMagnitude >= 100)
            transform.position = curPos;
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);

        }*/
    }

    void FixedUpdate()
    {
        //if (Pv.IsMine)
        {
            
            


            
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);


        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();

        }
    }

    void Getbutton()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        jdown = Input.GetButton("Jump");
    }
    void Movee()
    {

        bool isMove = moveInput.magnitude != 0;
        Debug.DrawRay(cameraarm.position, new Vector3(cameraarm.forward.x, 0f, cameraarm.forward.z).normalized, Color.red);
        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraarm.forward.x, 0f, cameraarm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraarm.right.x, 0f, cameraarm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            chbody.forward = moveDir;
            rb.velocity = (moveDir * Time.deltaTime * movespeed);
            

        }

    }

    void LookAround()
    {
        Vector2 mouse = new Vector2(Input.GetAxis("Mouse X") * AimSpeed, Input.GetAxis("Mouse Y") * AimSpeed) * Time.deltaTime;
        Vector3 camAngle = cameraarm.rotation.eulerAngles;

        float x = camAngle.x - mouse.y;
        if (x < 100f)
        {
            x = Mathf.Clamp(x, -1f, 36f);
        }
        else
        {
            x = Mathf.Clamp(x, 320f, 361f);
        }
        cameraarm.rotation = Quaternion.Euler(x, camAngle.y + mouse.x, camAngle.z);
    }


    void Jumpp()
    {
        if (jdown && !isJump)
        {
            grav = jumpspeed;
            isJump = true;
            chisground = false;
        }



    }

    void Gravityy()
    {
        grav += gravity * Time.deltaTime;
        rb.velocity = (new Vector3(rb.velocity.x, grav, rb.velocity.z));

    }



    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //if (Pv.IsMine)
        {

            if (hit.gameObject.tag == "Fallout")
            {

                chbody.position = new Vector3(0, 0, 0);

            }
        }
    }
    void IsGround()
    {
        chisground = Physics.CheckSphere(new Vector3(transform.position.x,transform.position.y-0.52f,transform.position.z),0.49f,layerMask);
        if (chisground == true)
        {
            isJump = false;
        }

    }

}
