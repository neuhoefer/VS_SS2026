using UnityEngine;

public class EngineSound : MonoBehaviour
{
    private Engine _engine;
    private AudioSource _engineSound;

    private void Awake()
    {
        _engine = GetComponent<Engine>();
        _engineSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_engine.PowerOn)
        {
            if (!_engineSound.isPlaying)
                _engineSound.Play();
            _engineSound.pitch = 0.5f + _engine.GetNormalizedRPM() * 0.5f;
        }
        else if (_engineSound.isPlaying)
            _engineSound.Stop();
    }
}
