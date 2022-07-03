using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D _rigidBody;
    Vector2 _lastVelocity;
    Vector2 _velocity;
    Collider2D _collider;

    bool JumpButtonHeld {get => Input.GetButton("Jump");}
    int InputDirection {get{
        if(Input.GetAxisRaw("Horizontal") > 0.01f)
            return 1;
        else if(Input.GetAxisRaw("Horizontal") < -0.01f)
            return -1;
        else
            return 0;
    }}
    [Header("PARTICLES")]
    [SerializeField] ParticleSystem JumpParticleSystem;
    [SerializeField] ParticleSystem WallJumpLeftParticleSystem;
    [SerializeField] ParticleSystem WallJumpRightParticleSystem;
    [SerializeField] ParticleSystem FallParticleSystem;

    [Header("RUN")]
    [SerializeField] float Speed = 10;
    [SerializeField] float SpeedMax = 8;
    [SerializeField] float InitialSpeedUpMultiplier = 5;
    [SerializeField] float TurnSpeed = 60;
    [SerializeField] float SlowDownGroundSpeed = 40;
    [SerializeField] float SlowDownAirSpeed = 20;

    [Header("JUMP")]
    [SerializeField] float JumpSpeed = 10;
    [SerializeField] float JumpBoost = 20;
    [SerializeField] float JumpBoostFalloffExponent = 4;
    [SerializeField] float JumpDuration = 1f;
    float _jumpStart = -1;
    [SerializeField] float JumpBufferDuration = 0.05f;
    float _jumpBufferTimer;
    [SerializeField] LayerMask GroundLayerMask;
    [SerializeField] float GroundMaxDistance = 0.05f;
    bool _grounded;
    [SerializeField] float GroundBufferDuration = 0.05f;
    float _groundBufferTimer;

    [SerializeField] Vector2 WallJumpSpeed = new Vector2(6,10);
    [SerializeField] float WallMaxDistance = 0.03f;
    [SerializeField] float nearWallBufferDuration = 0.05f;
    bool _nearWallLeft;
    float _nearWallLeftBufferTimer;
    bool _nearWallRight; 
    float _nearWallRightBufferTimer;
    [SerializeField] float WallJumpLockDuration = 0.3f;
    float _wallJumpLockTimer = 0;

    void Awake() {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    void Update() {
        if(Input.GetButtonDown("Jump"))
            _jumpBufferTimer = JumpBufferDuration;
    }

    void FixedUpdate() {
        _velocity = _rigidBody.velocity;

        _grounded = Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.down, GroundMaxDistance, GroundLayerMask);
        _nearWallLeft = Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.left, WallMaxDistance, GroundLayerMask);
        _nearWallRight = Physics2D.BoxCast(transform.position, _collider.bounds.size, 0, Vector2.right, WallMaxDistance, GroundLayerMask);

        CalculateTimers();
        CalculateWalk();
        CalculateJump();
        CalculateParticles();

        _rigidBody.velocity = _velocity;
        _lastVelocity = _velocity;
    }

    void CalculateParticles() {
        if(_lastVelocity.y < -20f && Mathf.Abs(_velocity.y) < 0.01f) {
            CameraManager.ShakeCamera(0.05f,0.08f);
            FallParticleSystem.Play();
        }

        if(_jumpStart == Time.time){
            if(_wallJumpLockTimer != WallJumpLockDuration){
                JumpParticleSystem.Play();
            } else {
                if(_velocity.x > 0f)
                    WallJumpLeftParticleSystem.Play();
                else
                    WallJumpRightParticleSystem.Play();
            }
        }
    }

    void CalculateTimers() {
        _jumpBufferTimer -= Time.fixedDeltaTime;
        _groundBufferTimer -= Time.fixedDeltaTime;
        
        _nearWallLeftBufferTimer -= Time.fixedDeltaTime;
        _nearWallRightBufferTimer -= Time.fixedDeltaTime;
        _wallJumpLockTimer -= Time.fixedDeltaTime;
        
        if(_grounded)
            _wallJumpLockTimer = 0;
    }

    void CalculateWalk() {
        if(InputDirection == 0){
            float slowDownSpeed = _grounded ? SlowDownGroundSpeed : SlowDownAirSpeed;
            if(Mathf.Abs(_velocity.x) < slowDownSpeed*Time.fixedDeltaTime)
                _velocity.x = 0;
            else 
                _velocity.x -= Mathf.Sign(_velocity.x)*slowDownSpeed*Time.fixedDeltaTime;
            return;
        }

        if(_wallJumpLockTimer > 0 && Mathf.Abs(_velocity.x) > 0.01f && Mathf.Sign(_velocity.x) != InputDirection){
            if(Mathf.Abs(_velocity.x) < SlowDownAirSpeed*Time.fixedDeltaTime)
                _velocity.x = 0;
            else 
                _velocity.x -= Mathf.Sign(_velocity.x)*SlowDownAirSpeed*Time.fixedDeltaTime;
            
            return;
        }


        if(Mathf.Abs(_velocity.x) > 0.01f && Mathf.Sign(_velocity.x) != InputDirection){
            _velocity.x += InputDirection*TurnSpeed*Time.fixedDeltaTime;
            return;
        }

        float deltaSpeed = Speed*Time.fixedDeltaTime*(1-Mathf.Pow(Mathf.Abs(_velocity.x)/SpeedMax,2))*InitialSpeedUpMultiplier;

        if(Mathf.Abs(_velocity.x) > SpeedMax){
            return;
        }

        if(Mathf.Abs(_velocity.x)+deltaSpeed > SpeedMax){
            _velocity.x = SpeedMax*InputDirection;
            return;
        }

        _velocity.x += deltaSpeed*InputDirection;
    }

    void CalculateJump() {
        if(_grounded)
            _groundBufferTimer = GroundBufferDuration;

        if(_nearWallLeft)
            _nearWallLeftBufferTimer = nearWallBufferDuration;
        if(_nearWallRight)
            _nearWallRightBufferTimer = nearWallBufferDuration;

        if(_jumpBufferTimer > 0){
            if(_groundBufferTimer > 0){
                _jumpStart = Time.time;
                _velocity.y = JumpSpeed;
                _jumpBufferTimer = 0;
                _groundBufferTimer = 0;
            } else if(_nearWallLeftBufferTimer > 0 || _nearWallRightBufferTimer > 0) {
                _jumpStart = Time.time;
                bool wallSide = _nearWallLeft; //true on left
                if(!_nearWallLeft && !_nearWallRight)
                    wallSide = _nearWallLeftBufferTimer > 0;
                
                _velocity = WallJumpSpeed * new Vector2(wallSide ? 1 : -1, 1);

                _wallJumpLockTimer = WallJumpLockDuration;
                _jumpBufferTimer = 0;
                _nearWallLeftBufferTimer = 0;
                _nearWallRightBufferTimer = 0;
            }
        }

        if(JumpButtonHeld && _velocity.y > 0 && Time.time - _jumpStart <= JumpDuration){
            _velocity.y += JumpBoost*(Mathf.Pow(1-(Time.time - _jumpStart)/JumpDuration,JumpBoostFalloffExponent))*Time.fixedDeltaTime;
        }
    }
}
