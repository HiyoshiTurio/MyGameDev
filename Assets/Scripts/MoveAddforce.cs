using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAddforce : MonoBehaviour
{
    [SerializeField] float _movePower = 3;
    [SerializeField] Animator _animator;
    Rigidbody _rb = default;
    bool _isGround = false;
    /// <summary>���͂��ꂽ������ XZ ���ʂł̃x�N�g��</summary>
    Vector3 _dir;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 cameraForward = Camera.main.transform.TransformDirection(Vector3.forward);
        Vector3 cameraRight = Camera.main.transform.TransformDirection(Vector3.right);
        cameraForward.y = 0;
        cameraRight.y = 0;
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        _dir = Vector3.forward * v + Vector3.right * h;
        // �J�����̃��[�J�����W�n����� dir ��ϊ�����
        _dir = Camera.main.transform.TransformDirection(_dir);
        // �J�����͎΂߉��Ɍ����Ă���̂ŁAY ���̒l�� 0 �ɂ��āuXZ ���ʏ�̃x�N�g���v�ɂ���
        _dir.y = 0;
        // �L�����N�^�[���u���݂́iXZ ���ʏ�́j�i�s�����v�Ɍ�����
        Vector3 forward = _rb.velocity;
        forward.y = 0;

        if (forward != Vector3.zero)
        {
            this.transform.forward = forward;
        }
        float _speed = forward.magnitude;

        float _fallSpeed = _rb.velocity.y;
        _animator.SetFloat("Speed", _speed);
        _animator.SetFloat("FallSpeed", _fallSpeed);
        _animator.SetBool("IsGround", _isGround);
    }

    private void FixedUpdate()
    {
        _dir.y = 0;
        _rb.AddForce(_dir.normalized * _movePower, ForceMode.Force);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            _isGround = true;
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            _isGround = false;
        }
    }
}
