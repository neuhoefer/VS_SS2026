using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoapboxController : MonoBehaviour
{
    [SerializeField] private float _motorForce;
    [SerializeField] private float _brakeForce;
    [SerializeField] private float _maxSteeringAngle;

    [SerializeField] private Transform _frontLeftWheel;
    [SerializeField] private Transform _frontRightWheel;
    [SerializeField] private Transform _rearLeftWheel;
    [SerializeField] private Transform _rearRightWheel;

    [SerializeField] private WheelCollider _frontLeftCollider;
    [SerializeField] private WheelCollider _frontRightCollider;
    [SerializeField] private WheelCollider _rearLeftCollider;
    [SerializeField] private WheelCollider _rearRightCollider;

    [SerializeField] private InputAction _moveAction;
    [SerializeField] private InputAction _brakeAction;

    private Vector2 _movement;
    private bool _isBraking;

    private void OnEnable()
    {
        _moveAction.Enable();    
        _brakeAction.Enable();    
    }

    private void OnDisable()
    {
        _moveAction.Disable();
        _brakeAction.Disable();
    }

    private void FixedUpdate()
    {
        GetInput();
        ApplyForces();
        HandleSteering();
        UpdateWheels();
    }

    private void GetInput()
    {
        _movement = _moveAction.ReadValue<Vector2>();
        _isBraking = _brakeAction.IsPressed();
    }

    private void ApplyForces()
    {
        _frontLeftCollider.motorTorque = _movement.y * _motorForce;
        _frontRightCollider.motorTorque = _movement.y * _motorForce;

        float currentBrakeForce = _isBraking ? _brakeForce : 0.0f;
        _frontLeftCollider.brakeTorque = currentBrakeForce;
        _frontRightCollider.brakeTorque = currentBrakeForce;
        _rearLeftCollider.brakeTorque = currentBrakeForce;
        _rearRightCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering()
    {
        _frontLeftCollider.steerAngle = _movement.x * _maxSteeringAngle;
        _frontRightCollider.steerAngle = _movement.x * _maxSteeringAngle;
    }

    private void UpdateWheels()
    {
        UpdateWheel(_frontLeftCollider, _frontLeftWheel);
        UpdateWheel(_frontRightCollider, _frontRightWheel);
        UpdateWheel(_rearLeftCollider, _rearLeftWheel);
        UpdateWheel(_rearRightCollider, _rearRightWheel);
    }

    private void UpdateWheel(WheelCollider collider, Transform wheel)
    {
        collider.GetWorldPose(out var pos, out var rot);
        wheel.rotation = rot;

        wheel.Rotate(Vector3.forward, 90);
    }
}
