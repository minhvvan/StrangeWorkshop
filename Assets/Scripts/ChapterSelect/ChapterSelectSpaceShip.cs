using System;
using UnityEngine;
using DG.Tweening;

public class ChapterSelectSpaceShip : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotationDuration = 0.3f;
    [SerializeField] private Ease _rotationEase = Ease.OutSine;
    
    [Header("Tilt Settings")]
    [SerializeField] private float _maxTiltAngle = 30f;
    [SerializeField] private float _tiltDuration = 0.2f;
    [SerializeField] private Ease _tiltEase = Ease.OutSine;
    
    private Vector3 _moveDirection;
    private Rigidbody _rb;
    private Tween _rotationTween;
    private Tween _tiltTween;
    private float _currentTiltAngle;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb != null)
        {
            _rb.constraints = RigidbodyConstraints.FreezeRotationZ;
            _rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    private void OnDestroy()
    {
        _rotationTween?.Kill();
        _tiltTween?.Kill();
    }

    private void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        _moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        Move();
        if (_moveDirection != Vector3.zero)
        {
            RotateWithTilt();
        }
        else
        {
            ResetTilt();
        }
    }

    private void Move()
    {
        if (_moveDirection == Vector3.zero)
        {
            _rb.velocity = Vector3.zero;
            return;
        }

        Vector3 movement = _moveDirection * _moveSpeed;
        
        if (_rb != null)
        {
            _rb.velocity = movement;
        }
        else
        {
            transform.position += movement * Time.deltaTime;
        }
    }

    private void RotateWithTilt()
    {
        _rotationTween?.Kill();
        _tiltTween?.Kill();

        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
        _rotationTween = transform.DORotateQuaternion(targetRotation, _rotationDuration)
            .SetEase(_rotationEase)
            .SetUpdate(UpdateType.Fixed);

        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float targetTiltAngle = -horizontalInput * _maxTiltAngle;

        if (Mathf.Abs(_currentTiltAngle - targetTiltAngle) > 0.1f)
        {
            _tiltTween = DOTween.To(() => _currentTiltAngle, x => {
                _currentTiltAngle = x;
                Vector3 currentRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, _currentTiltAngle);
            }, targetTiltAngle, _tiltDuration)
                .SetEase(_tiltEase)
                .SetUpdate(UpdateType.Fixed);
        }
    }

    private void ResetTilt()
    {
        if (Mathf.Abs(_currentTiltAngle) > 0.1f)
        {
            _tiltTween?.Kill();
            _tiltTween = DOTween.To(() => _currentTiltAngle, x => {
                _currentTiltAngle = x;
                Vector3 currentRotation = transform.rotation.eulerAngles;
                transform.rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, _currentTiltAngle);
            }, 0f, _tiltDuration)
                .SetEase(_tiltEase)
                .SetUpdate(UpdateType.Fixed);
        }
    }
}