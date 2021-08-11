using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScriptTT : MonoBehaviourPunCallbacks, IPunObservable
{
    public PhotonView Pv;
    GameObject cameraarmGO;
    Transform cameraarm;
    Transform chbody;
    Rigidbody rb;
    RaycastHit hitInfo;
    Vector2 moveInput;
    Vector3 curPos;
    Vector3 curRig;
    Quaternion curRot;
    Vector3 angvel;
    Vector3 StartPoint = Vector3.zero;
    Quaternion StartRotation;
    Quaternion StartCameraR;



    public Vector2 mouse;


    public bool IsMob = false;

    public LayerMask layerMask;

    public GameObject GM;
    public GameManager GMS;

    public GameObject joystick;
    public FixedJoystick FJK;

    public GameObject JumpBT;
    public JumpBt JBS;

    public GameObject TouchPd;
    public TouchPad TouchPdSc;

    public GameObject UIManager;
    public InGameUIManager UIManagerSc;

    public bool isMove;
    public float groundangle;
    public float rgroundangle;
    public float befgroundangle;
    public float gravity;
    public float grav;
    public float AimSpeed;
    public float movespeed;
    public bool chisground;
    public bool chisground1;
    public bool jdown = false;
    public float jumpspeed;
    public bool canMove = true;
    public bool gameStEd = false;
    public bool gameEd = false;
    public GameObject MobPanel;
    public float Maxgroundangle;

    public GameObject[] CheckPointsInGM;
    public bool[] CheckPointStat;

    public Animator Ani;
    void Awake()
    {
        if (Pv.IsMine)
        {


            MobPanel = GameObject.Find("MobPanel");
            if (IsMob)
            {
                MobPanel.SetActive(true);
                JumpBT = GameObject.Find("JumpButton");
                JBS = JumpBT.GetComponent<JumpBt>();
                joystick = GameObject.Find("Fixed Joystick");
                FJK = joystick.GetComponent<FixedJoystick>();
                TouchPd = GameObject.Find("Touchpad");
                TouchPdSc = TouchPd.GetComponent<TouchPad>();
                
            }
            else
            {
                MobPanel.SetActive(false);
            }
            UIManager = GameObject.Find("UIManager");
            UIManagerSc = UIManager.GetComponent<InGameUIManager>();
        }
        
        cameraarmGO = GameObject.Find("CameraArm");
        cameraarm = cameraarmGO.transform;
        chbody = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        GM = GameObject.Find("GameManager");
        GMS = GM.GetComponent<GameManager>();
        

        
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!Pv.IsMine)
            gameObject.layer = LayerMask.NameToLayer("JUMPON");
        else
        {
            gameObject.tag = "Player";
            cameraarm.rotation = Quaternion.Euler(20, 180, 0);
            StartPoint = transform.position;
            StartRotation = Quaternion.Euler(0, 0, 0);
            
            CheckPointsInGM = GMS.CheckPoints;
            CheckPointStat = new bool[CheckPointsInGM.Length];
            Debug.Log(CheckPointStat[0]);
            for(int i=0;i< CheckPointsInGM.Length; i++)
            {
                CheckPointStat[i] = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Pv.IsMine)
        {
            AimSpeed = UIManagerSc.AimSpeed;
            gameStEd = GMS.GameStarted;
            gameEd = GMS.GameEnded;
            InputAx();
            LookAround();

            /*Groundcheck();
            Move();
            Groundcheck();
            Jump();
            Gravityy();
            Groundcheck();*/


            //cameraarm.transform.position = Vector3.Lerp(cameraarm.transform.position, chbody.transform.position, Time.deltaTime * 100);

            //cameraarm.transform.position = chbody.transform.position;
            cameraarm.transform.position = new Vector3(chbody.transform.position.x, chbody.transform.position.y+1, chbody.transform.position.z);
        }
        else if ((transform.position - curPos).sqrMagnitude >= 100)
        {
            transform.position = curPos;
            transform.rotation = curRot;
            rb.velocity = curRig;
            rb.angularVelocity = angvel;
        }  
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 5);
            transform.rotation = curRot;
            rb.velocity = curRig;
            rb.angularVelocity = angvel;
        }
    }
    void FixedUpdate()
    {
        if (Pv.IsMine && !gameEd)
        {
            Groundcheck();
            Move();
            Groundcheck();
            Jump();
            Gravityy();
            Groundcheck();
            
        }
        if (Pv.IsMine && gameEd)
        {
            rb.velocity = Vector3.zero;
        }
        
    }

    void LookAround()
    {
        
        if (IsMob)
        {
             mouse = TouchPdSc.TouchDist * AimSpeed * 0.05f;
        }
        else
        {
             mouse = new Vector2(Input.GetAxis("Mouse X") * AimSpeed, Input.GetAxis("Mouse Y") * AimSpeed);
        }
        
        Vector3 camAngle = cameraarm.rotation.eulerAngles;

        float x = camAngle.x - mouse.y;
        if (x < 100f)
        {
            x = Mathf.Clamp(x, -1f, 39f);
        }
        else
        {
            x = Mathf.Clamp(x, 321f, 361f);
        }
        //if (IsMob)
        //{
            //cameraarm.rotation = Quaternion.Lerp(cameraarm.rotation, Quaternion.Euler(x, camAngle.y + mouse.x, camAngle.z), Time.deltaTime * 5);
           // cameraarm.rotation = new Vector3(cameraarm.rotation.x, cameraarm.rotation.y, 0);
        //}
       // else
        {
            cameraarm.rotation = Quaternion.Euler(x, camAngle.y + mouse.x, camAngle.z);
        }

    }

    void Move()
    {
        if (!gameStEd) return;
        if (!canMove)
        {
            
            if(rb.velocity.sqrMagnitude >= 30)
                return;
            canMove = true;
        }
        Vector3 velocity = new Vector3(0,0,0);
        Vector3 forward;

        Debug.DrawRay(transform.position, new Vector3(transform.forward.x + 0.01f, transform.forward.y - 4f, transform.forward.z + 0.01f).normalized, Color.red);
        //Physics.Raycast(transform.position, new Vector3(transform.forward.x + 0.01f, transform.forward.y-4f, transform.forward.z+0.01f).normalized, out hitInfo, 20, layerMask);
        Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 20, layerMask);

        Vector3 lookForward = new Vector3(cameraarm.forward.x, 0f, cameraarm.forward.z).normalized;
        Vector3 lookRight = new Vector3(cameraarm.right.x, 0f, cameraarm.right.z).normalized;
        Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;
        if (isMove)
        {
            chbody.forward = new Vector3(moveDir.x, 0f, moveDir.z);
        }
        befgroundangle = rgroundangle;
        rgroundangle = Vector3.Angle(hitInfo.normal, new Vector3(transform.forward.x, 0f, transform.forward.z));
        
        if (!chisground)
        {
            groundangle = 90;
            forward = transform.forward;
        }
        else
        {
            forward = Vector3.Cross(transform.right, hitInfo.normal);
            groundangle = Vector3.Angle(hitInfo.normal, new Vector3(transform.forward.x,0f, transform.forward.z));
        }
        if(groundangle == 0)
        {
            groundangle = 90;
            forward = transform.forward;
        }
        if (groundangle > Maxgroundangle)
        {
            forward = transform.forward;
            isMove = false;
        }
        lookForward = new Vector3(cameraarm.forward.x, forward.y, cameraarm.forward.z).normalized;
        lookRight = new Vector3(cameraarm.right.x, forward.y, cameraarm.right.z).normalized;
        moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

        Debug.DrawLine(transform.position, transform.position + forward, Color.blue);
        Debug.DrawRay(transform.position, new Vector3(cameraarm.forward.x, 0, cameraarm.forward.z).normalized, Color.red);
        if (isMove)
        {
            

            //chbody.forward = new Vector3(moveDir.x,0f,moveDir.z);
            
            velocity = (new Vector3(forward.x * movespeed, forward.y * movespeed , forward.z * movespeed ) );
            Ani.SetBool("isRun", true);

        }
        else
        {
            Ani.SetBool("isRun", false);
        }
        rb.velocity = new Vector3(velocity.x, velocity.y, velocity.z) * Time.deltaTime;
    }
    void Groundcheck()
    {
        
        chisground = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y-0.6f, transform.position.z),0.48f,layerMask);
        chisground1 = Physics.CheckSphere(new Vector3(transform.position.x, transform.position.y - 0.55f, transform.position.z), 0.48f, layerMask);
    }
    void Gravityy()
    {
        if (!chisground)
        {
            
            {
                grav += gravity * Time.deltaTime;
                rb.velocity = (new Vector3(rb.velocity.x, grav, rb.velocity.z));
            }
            
        }
        else
        {   if(canMove)
                grav = -20;
        }
        

    }
    void InputAx()
    {
        if (IsMob)
        {
            moveInput = FJK.moveinput.normalized;
            isMove = moveInput.magnitude >= 0.5f;
            jdown = JBS.jumpDn;
        }
        else
        {
            moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
            isMove = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).magnitude >= 0.5f;
            jdown = Input.GetButton("Jump");
        }
        
    }
    void Jump()
    {
        if (jdown && chisground && canMove && gameStEd)
        {
            grav = jumpspeed;
            chisground = false;
            Ani.SetBool("isJump", true);
        }
        else
        {
            Ani.SetBool("isJump", false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (Pv.IsMine)
        {
            if (collision.gameObject.tag == "Fallout")
            {
                chbody.position = StartPoint;
                chbody.rotation = StartRotation;
                rb.velocity = Vector3.zero;
            }
            
        }
            
    }
    private void OnTriggerStay(Collider other)
    {
        if (Pv.IsMine)
        {
            if (other.gameObject.tag == "GoalLine")
            {
                GMS.IsGoal = 1;
                if (IsMob)
                {
                    MobPanel.SetActive(false);
                }
                PhotonNetwork.Destroy(this.gameObject);

            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Pv.IsMine)
        {
            if(other.gameObject.tag == "CheckPoint")
            {
                bool NewCheck = false;
                
                for(int i = 0;i< CheckPointsInGM.Length; i++)
                {
                    if(other.gameObject == CheckPointsInGM[i])
                    {
                        NewCheck = true;
                        if (CheckPointStat[i] != true)
                        {
                            StartPoint = chbody.position;
                            StartRotation = chbody.rotation;
                            CheckPointStat[i] = true;
                            
                        }
                    }
                    else if(NewCheck == false)
                    {
                        CheckPointStat[i] = true;
                    }
                }
            }
        }
    }


    public void HitPlayer(Vector3 velocityF, float time)
    {
        if (Pv.IsMine)
        {
            
            rb.velocity = velocityF;
            grav = 30;
            canMove = false;
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
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();
            curRig = (Vector3)stream.ReceiveNext();
            angvel = (Vector3)stream.ReceiveNext();
        }
    }
    
}

