using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviourPun
{
    private Vector3 m_camRot;
    private Transform m_camTransform;//Camera Transform
    private Transform m_transform;//Transform
    public float m_movSpeed=10;
    public float m_rotateSpeed=1;


    private CharacterController controller;
    private Vector3 moveDirection;
    public float speed = 19;
    public float gravity = 9;

    public TMP_Text nameText;
    public Animator playerAnim;

    private void Start()
    {
        m_camTransform = Camera.main.transform;
        m_transform = GetComponent<Transform>();
        
        controller = GetComponent<CharacterController>();

        nameText.text = GetComponent<PhotonView>().Owner.NickName;
    }



    private void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected)
        {
            return;
        }
        
        
        Control();
    }
    void Control()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        moveDirection = new Vector3(h, 0, v);
        moveDirection *= speed;
        moveDirection.y = -gravity;
        controller.Move(moveDirection * Time.deltaTime);

        playerAnim.SetFloat("Speed_f", Mathf.Abs(h) + Mathf.Abs(v));

        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));  
        }
    }
}
