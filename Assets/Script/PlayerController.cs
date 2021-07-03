using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
     * �⺻���� ĳ������ �������� ������.
     * wasd - ��������
     * ���콺 - ȭ������
     * space - ����
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

        //����ȭ�ϴ� ���� �ӵ��� �� ����.
        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * walkSpeed;

        //Time.deltaTime = 0.016sec
        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);

        
    }
}
