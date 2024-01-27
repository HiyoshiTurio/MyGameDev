using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _movePower = 1f;
    [SerializeField] float _jumpPower = 1f;
    [SerializeField] float _LimitSpeed = 1f;
    [SerializeField] Animator _animator;
    Rigidbody _rb;
    float _speed;
    bool _isGround = false;
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

        Vector3 dir = cameraForward * v + cameraRight * h;
        dir.Normalize();

        if (dir != Vector3.zero)
        {
            this.transform.forward = dir;
        }
        _rb.velocity = dir * _movePower + _rb.velocity.y * Vector3.up;

        if (Input.GetButtonDown("Jump") && _isGround)
        {
            Vector3 velocity = _rb.velocity;
            velocity.y = _jumpPower;
            _rb.velocity = velocity;
        }

        Vector3 vec = _rb.velocity;
        vec.y = 0;
        _speed = vec.magnitude;
        if (_speed > _LimitSpeed)
        {
            _rb.velocity = new Vector3(_rb.velocity.x / 1.1f, _rb.velocity.y, _rb.velocity.z / 1.1f);
        }
        _animator.SetFloat("Speed", _speed);
        _animator.SetBool("IsGround", _isGround);
    }

    void FixedUpdate()
    {
        
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
