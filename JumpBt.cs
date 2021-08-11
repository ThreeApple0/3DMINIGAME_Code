using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpBt : MonoBehaviour
{
    public bool jumpDn = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void onclick()
    {
        jumpDn = true;
    }
    public void exclick()
    {
        jumpDn = false;
    }
}
