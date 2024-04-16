using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankSpawnPoint : MonoBehaviour
{
    private static List<TankSpawnPoint> _spawnPoints = new List<TankSpawnPoint>();

    public static Vector3 GetRandomSpawnPos()
    {
        if (_spawnPoints.Count == 0) return Vector3.zero;
        
        int randIndex = Random.Range(0, _spawnPoints.Count);
        return _spawnPoints[randIndex].transform.position;
    }

    private void OnEnable()
    {
        _spawnPoints.Add(this);
    }
    
    private void OnDisable()
    {
        _spawnPoints.Remove(this);
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.color = Color.white;
    }
#endif
}
