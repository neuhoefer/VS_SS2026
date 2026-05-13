using UnityEngine;
using UnityEngine.InputSystem;

public class HydraulicEffector : MonoBehaviour, IPowerConsumer
{
    private const float MAX_POWER = 4.5f;  // kW

    private float _armInput = 0.0f;
    private float _bucketInput = 0.0f;

    [Space(10)]
    [SerializeField] private float _armSpeed = 10.0f;       // Grad/s
    [SerializeField] private float _bucketSpeed = 20.0f;    // Grad/s
    [SerializeField] private InputAction _effectorAction;

    private Engine _engine;

    public Engine.Consumer ConsumerType => Engine.Consumer.Effector;

    private void Awake()
    {
        _engine = GetComponent<Engine>();
        _engine.RegisterConsumer(this);
    }

    public float RequestPower()
    {
        return MAX_POWER;
    }

    public void ReceivePower(float grantedPower)
    {

    }
}