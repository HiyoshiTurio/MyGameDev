using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class MoveAddforce : MonoBehaviour, IPause
{
    [SerializeField] float _movePower = 3f;
    [SerializeField] float _jumpPower = 1f;
    [SerializeField] float _LimitSpeed = 1f;
    [SerializeField] float _stuckTime = 1f;
    [Tooltip("���n���̍d������")]
    [SerializeField] float _waitForIsGround = 1f;
    [SerializeField] Animator _anim;
    [SerializeField]bool _cursorVisible = false;
    float _timer;
    float _speed;
    float _fallSpeed;
    float _savedAnimSpeed;
    
    bool _isGround = false;
    Vector3 _velocity;
    Vector3 _angularVelocity;
    Vector3 _reSponePos;
    Rigidbody _rb = default;

    /// <summary>���͂��ꂽ������ XZ ���ʂł̃x�N�g��</summary>
    Vector3 _dir;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
        if(!_cursorVisible) Cursor.visible = false;
        _reSponePos = this.transform.position;
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

        if (Input.GetButtonDown("Jump") && _isGround && _timer > _waitForIsGround)
        {
            Vector3 velocity = _rb.velocity;
            velocity.y = _jumpPower;
            _rb.velocity = velocity;
            _timer = 0;
        }

        _speed = forward.magnitude;
        _fallSpeed = _rb.velocity.y;
        if (_speed > _LimitSpeed)
        {
            _rb.velocity = new Vector3(_rb.velocity.x / 1.1f, _rb.velocity.y, _rb.velocity.z / 1.1f);
        }
        _anim.SetFloat("Speed", _speed);
        _anim.SetFloat("UpSpeed", _fallSpeed);
        _anim.SetBool("IsGround", _isGround);

        if (_isGround && _timer < _waitForIsGround)
        {
            _timer += Time.deltaTime;
        }

        if (this.transform.position.y < -10)
        {
            _rb.velocity = new Vector3(0, 0, 0);
            this.transform.position = _reSponePos;
        }
    }

    void FixedUpdate()
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
        if (collider.gameObject.tag == "Goal")
        {
            SceneManager.LoadScene("GoalScene");
        }
    }
    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Ground")
        {
            _isGround = false;
        }
    }

    void OnApplicationQuit()
    {
        Cursor.visible = true;
    }

    public void Pause()
    {
        Debug.Log("Pause");
        // ���x�E��]��ۑ����ARigidbody ���~����
        _angularVelocity = _rb.angularVelocity;
        _velocity = _rb.velocity;
        _rb.isKinematic = true;
        _rb.Sleep();
        
        Cursor.visible = true;
        // �A�j���[�V�������~�߂�i�Đ����x��0�ɂ���j
        _savedAnimSpeed = _anim.speed;
        _anim.speed = 0;
    }

    public void Resume()
    {
        Debug.Log("Resume");
        Debug.Log(_velocity);
        // Rigidbody �̊������ĊJ���A�ۑ����Ă��������x�E��]��߂�
        _rb.WakeUp();
        _rb.isKinematic = false;
        _rb.angularVelocity = _angularVelocity;
        _rb.AddForce(_velocity, ForceMode.Impulse);

        Cursor.visible = false;
        //
        _anim.speed = _savedAnimSpeed;
    }
}
