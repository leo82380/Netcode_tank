using Unity.Netcode;
using UnityEngine;

public class PlayerAiming : NetworkBehaviour
{
    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] private Transform _turretTrm;

    private void LateUpdate()
    {
        if (!IsOwner) return;
        Vector2 mousePos = _playerInput.AimPosition;

        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector3 dir = (worldMousePos - transform.position).normalized;

        //float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        //_turretTrm.rotation = Quaternion.Euler(0, 0, angle);

        _turretTrm.up = new Vector2(dir.x, dir.y);
    }
}