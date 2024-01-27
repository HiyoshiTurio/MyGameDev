using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float _movePower = 1f;
    [SerializeField] float _jumpPower = 1f;
    [SerializeField] float _LimitSpeed = 1f;
    [SerializeField] float _stuckTime = 1f;
    [SerializeField] Animator _animator;
    Rigidbody _rb;
    float _speed, _fallSpeed;
    float _mPower, _jPower;
    bool _isGround = false;
    Vector3 tmp;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mPower = _movePower;
        _jPower = _jumpPower;
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
        _rb.velocity = dir * _mPower + _rb.velocity.y * Vector3.up;

        if (Input.GetButtonDown("Jump") && _isGround)
        {
            Vector3 velocity = _rb.velocity;
            velocity.y = _jPower;
            _rb.velocity = velocity;
        }

        Vector3 vec = _rb.velocity;
        vec.y = 0;
        _speed = vec.magnitude;
        if (_speed > _LimitSpeed)
        {
            _rb.velocity = new Vector3(_rb.velocity.x / 1.1f, _rb.velocity.y, _rb.velocity.z / 1.1f);
        }

        _fallSpeed = _rb.velocity.y;
        _animator.SetFloat("Speed", _speed);
        _animator.SetFloat("FallSpeed", _fallSpeed);
        _animator.SetBool("IsGround", _isGround);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            _isGround = true;
            StartCoroutine(Stuck());
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            _isGround = false;
        }
    }
    IEnumerator Stuck()
    {
        _mPower = 0.1f;
        _jPower = 0.1f;
        yield return new WaitForSeconds(_stuckTime);
        _mPower = _movePower;
        _jPower = _jumpPower;
    }
}
