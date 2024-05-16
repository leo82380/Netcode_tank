using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MapManager>();
                if (_instance == null)
                {
                    Debug.LogError("MapManager is not found");
                }
            }
            return _instance;
        }
    }
    
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private LayerMask _whatIsObstacle;
    [SerializeField] private Tilemap _safetyTilemap;
    
    public List<Vector3> GetAvailablePositionList(Vector3 center, float radius)
    {
        int radiusInt = Mathf.CeilToInt(radius);
        Vector3Int centerCell = _tilemap.WorldToCell(center);
        
        List<Vector3> pointList = new List<Vector3>();
        for (int i = -radiusInt; i <= radiusInt; ++i)
        {
            for (int j = -radiusInt; j <= radiusInt; ++j)
            {
                if (Mathf.Abs(i) + Mathf.Abs(j) > radius) continue;
                // 반지름을 벗어나는 영역은 제외
                
                Vector3Int cellPoint = centerCell + new Vector3Int(i, j, 0);
                TileBase tile = _tilemap.GetTile(cellPoint);
                
                if (tile != null) continue; // 해당 타일에는 장애불이 존재
                
                Vector3 worldPos = _tilemap.GetCellCenterWorld(cellPoint);
                Collider2D col = Physics2D.OverlapCircle(worldPos, 0.5f, _whatIsObstacle);
                
                if (col != null) continue;
                pointList.Add(worldPos);
            }
        }

        return pointList;
    }
    
    public bool IsInSafetyZone(Vector3 worldPos)
    {
        Vector3Int tilePos = _safetyTilemap.WorldToCell(worldPos);
        TileBase tile = _safetyTilemap.GetTile(tilePos);
        
        return tile != null;
    }
}
