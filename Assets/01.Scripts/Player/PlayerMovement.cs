using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _bodyTrm;
    private Rigidbody2D _rigidbody;
    
    [Header("Setting Values")]
    [SerializeField] private float _movementSpeed = 4f;
    [SerializeField] private float _turningSpeed = 30f;
    private Vector2 _movementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
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
        if (!IsOwner) return;
        _rigidbody.velocity = _bodyTrm.up * (_movementInput.y * _movementSpeed);
    }
}
