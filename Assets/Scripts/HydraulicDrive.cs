using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class HydraulicDrive : MonoBehaviour, IPowerConsumer
{
    private const float MAX_POWER = 12.4f;      // kW
    private const float SPEED_MAX = 8.2f;       // km/h
    private const float BRAKE_TORQUE = 500.0f;  // Nm

    [Space(10)]
    [Range(-1, 1)]
    [SerializeField] private float _leftWheelsInput = 0.0f;
    [Range(-1, 1)]
    [SerializeField] private float _rightWheelsInput = 0.0f;
    [SerializeField] private bool _isoControl = true;
    [SerializeField] private float _inputSpeed = 2.0f;
    [SerializeField] private InputAction _driveAction;

    [Space(10)]
    [SerializeField] private WheelCollider[] _leftWheelColliders = new WheelCollider[2];
    [SerializeField] private WheelCollider[] _rightWheelColliders = new WheelCollider[2];
    [SerializeField] private Transform[] _leftWheelMeshes = new Transform[2];
    [SerializeField] private Transform[] _rightWheelMeshes = new Transform[2];

    private Engine _engine;
    private float _wheelRadius = 0.0f;
    private float _grantedPower = 0.0f;

    public Engine.Consumer ConsumerType => Engine.Consumer.Drive;

    private void OnEnable() => _driveAction.Enable();
    private void OnDisable() => _driveAction.Disable();

    private void Awake()
    {
        _engine = GetComponent<Engine>();
        _engine.RegisterConsumer(this);
        _wheelRadius = _leftWheelColliders[0].radius;
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if(_isoControl)
        {
            Vector2 input = _driveAction.ReadValue<Vector2>();
            float leverLeft = Mathf.Clamp(input.y + input.x, -1.0f, 1.0f);
            float leverRight = Mathf.Clamp(input.y - input.x, -1.0f, 1.0f);
            _leftWheelsInput = Mathf.MoveTowards(_leftWheelsInput, leverLeft, _inputSpeed * Time.deltaTime);
            _rightWheelsInput = Mathf.MoveTowards(_rightWheelsInput, leverRight, _inputSpeed * Time.deltaTime);
        }
    }

    private float GetCurrentSpeedMS()
    {
        float rpm = Mathf.Abs(_leftWheelColliders[0].rpm + _rightWheelColliders[0].rpm) / 2.0f;
        return rpm * _wheelRadius * Mathf.PI / 30.0f;
    }

    private float GetTargetSpeedMS()
    {
        float input = (Mathf.Abs(_leftWheelsInput) + Mathf.Abs(_rightWheelsInput)) / 2.0f;
        return input * SPEED_MAX / 3.6f;
    }

    public float RequestPower()
    {
        float input = (Mathf.Abs(_leftWheelsInput) + Mathf.Abs(_rightWheelsInput)) / 2.0f;
        return input * MAX_POWER;
    }

    public void ReceivePower(float power)
    {
        _grantedPower = power;
        ApplyTorque();
    }

    private void ApplyTorque()
    {
        float currentSpeed = GetCurrentSpeedMS();
        float targetSpeed = GetTargetSpeedMS();

        float targetOmega = targetSpeed / _wheelRadius;
        float torque = targetOmega != 0.0f ? (_grantedPower * 1000.0f) / (targetOmega * 4) : 0.0f;

        ApplyTorqueToWheelColliders(_leftWheelColliders, _leftWheelsInput, currentSpeed, targetSpeed, torque);
        ApplyTorqueToWheelColliders(_rightWheelColliders, _rightWheelsInput, currentSpeed, targetSpeed, torque);
    }

    private void ApplyTorqueToWheelColliders(WheelCollider[] colliders, float input, float currentSpeed, float targetSpeed, float torque)
    {
        foreach (WheelCollider wc in colliders)
        {
            if (currentSpeed < targetSpeed && input != 0.0f)
            {
                wc.motorTorque = input * torque;
                wc.brakeTorque = 0.0f;
            }
            else
            {
                wc.motorTorque = 0.0f;
                wc.brakeTorque = BRAKE_TORQUE;
            }
        }
    }
}
