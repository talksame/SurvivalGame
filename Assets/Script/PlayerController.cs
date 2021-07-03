using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
     * 기본적인 캐릭터의 움직임을 서술함.
     * wasd - 방향조작
     * 마우스 - 화면조작
     * space - 점프
     */
    [SerializeField]
    private float walkSpeed;
    private Rigidbody myRigid;


    private void Start()
    {
        myRigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        float _moveDirectX = Input.GetAxisRaw("Horizontal");
        float _moveDirectZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirectX;
        Vector3 _moveVertical = transform.forward * _moveDirectZ;

        //정규화하는 것이 속도가 더 빠름.
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        //Time.deltaTime = 0.016sec
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        
    }
}
