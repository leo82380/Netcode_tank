using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _bodyTrm;
    [SerializeField] private ParticleSystem _dustEffect;
    private Rigidbody2D _rigidbody;
    
    [Header("Setting Values")]
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float _turningSpeed = 30f;
    private Vector2 _movementInput;

    [SerializeField] private float _dustEmissionValue = 10;
    private ParticleSystem.EmissionModule _emissionModule;
    private Vector3 _prevPos;
    private float _particleStopThreshold = 0.005f;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _emissionModule = _dustEffect.emission;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _playerInput.OnMovementEvent += HandleMovementEvent;
    }

    public override void OnNetworkDespawn()
    {
        _playerInput.OnMovementEvent -= HandleMovementEvent;
    }

    private void HandleMovementEvent(Vector2 movement)
    {
        _movementInput = movement;
    }

    // Update 포탑 회전

    private void Update()
    {
        if (!IsOwner) return;
        float zRotation = _movementInput.x * -_turningSpeed * Time.deltaTime;
        _bodyTrm.Rotate(0, 0, zRotation);
    }

    // FixedUpdate 이동

    private void FixedUpdate()
    {
        float moveDistance = (transform.position - _prevPos).sqrMagnitude;
        if (moveDistance > _particleStopThreshold)
        {
            _emissionModule.rateOverTime = _dustEmissionValue;
        }
        else
        {
            _emissionModule.rateOverTime = 0;
        }
        _prevPos = transform.position;
        if (!IsOwner) return;
        _rigidbody.velocity = _bodyTrm.up * (_movementInput.y * _movementSpeed);
    }
}
