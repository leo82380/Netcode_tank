using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector2> OnMovementEvent;
    public event Action OnFireEvent;
    public Vector2 AimPosition { get; private set; }

    private void Update()
    {
        CheckMovementInput();
        CheckFireInput();
        CheckMousePosition();
    }

    private void CheckMousePosition()
    {
        AimPosition = Input.mousePosition;
    }

    private void CheckFireInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnFireEvent?.Invoke();
        }
    }

    private void CheckMovementInput()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        Vector2 movement = new Vector2(horizontal, vertical);
        OnMovementEvent?.Invoke(movement.normalized);
    }
}
