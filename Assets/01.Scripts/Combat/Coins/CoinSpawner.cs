using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawner : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private ReSpawnCoin _coinPrefab;
    [SerializeField] private DecalCircle _decalCircle;

    [Header("Setting Values")] 
    [SerializeField] private int _maxCoins = 30; // 최대 30개
    [SerializeField] private int _coinValue = 10; // 코인당 10
    [SerializeField] private LayerMask _layerMask; //코인 생성 지역에 장애물 있는지 검사
    
    [SerializeField] private float _spawnTerm = 30f;
    [SerializeField] private List<SpawnPoint> spawnPointList;

    private bool _isSpawning = false;
    private float _spawnTime = 0;
    private int _spawnCountTime = 5; // 5초 카운트다운

    private float _coinRadius;
    
    private Stack<ReSpawnCoin> _coinPool = new Stack<ReSpawnCoin>(); // 코인 풀
    private List<ReSpawnCoin> _activeCoinList = new List<ReSpawnCoin>(); // 활성화된 코인

    // 서버만 실행
    private ReSpawnCoin SpawnCoin()
    {
        if (IsServer == false) return null;

        ReSpawnCoin coin = Instantiate(_coinPrefab, Vector3.zero, Quaternion.identity);
        coin.SetValue(_coinValue);
        coin.GetComponent<NetworkObject>().Spawn();
        
        coin.OnCollected += HandleCoinCollected;
        
        return coin;
    }

    // 서버만 실행
    private void HandleCoinCollected(ReSpawnCoin coin)
    {
        if (IsServer == false) return;
        
        _activeCoinList.Remove(coin);
        coin.SetVisible(false);
        _coinPool.Push(coin);
    }
    
    public override void OnNetworkSpawn()
    {
        if (IsServer == false) return;
        
        _coinRadius = _coinPrefab.GetComponent<CircleCollider2D>().radius;

        for (int i = 0; i < _maxCoins; i++)
        {
            ReSpawnCoin coin = SpawnCoin();
            coin.SetVisible(false); // 처음 생성된 애들을 안보이게
            _coinPool.Push(coin);
        }
    }
    
    public override void OnNetworkDespawn()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (IsServer == false) return;
        
        // 나중에 여기에 게임이 시작되었을 때만 코인이 생성되게 변경해야함
        if (GameManager.Instance.IsGameStarted == false) return;
        if (_isSpawning == false && _activeCoinList.Count == 0)
        {
            _spawnTime += Time.deltaTime;
            if (_spawnTime >= _spawnTerm)
            {
                _spawnTime = 0;
                StartCoroutine(SpawnCoroutine());
            }
        }
    }

    private IEnumerator SpawnCoroutine()
    {
        _isSpawning = true;
        int pointIndex = Random.Range(0, spawnPointList.Count);
        SpawnPoint point = spawnPointList[pointIndex];
        int maxCoinCnt = Mathf.Min(_maxCoins, point.SpawnPoints.Count);
        int coinCount = Random.Range(maxCoinCnt / 2, maxCoinCnt + 1);

        for (int i = _spawnCountTime; i > 0; --i)
        {
            CountDownClientRpc(i, pointIndex, coinCount);
            yield return new WaitForSeconds(1f);
        }
        
        float coinDelay = 2f;
        List<Vector3> points = point.SpawnPoints;
        for (int i = 0; i < coinCount; ++i)
        {
            int end = points.Count - i - 1;
            int idx = Random.Range(0, end + 1);
            Vector3 pos = points[idx];
            
            (points[idx], points[end]) = (points[end], points[idx]);

            ReSpawnCoin coin = _coinPool.Pop();
            coin.transform.position = pos;
            coin.ResetCoin();
            _activeCoinList.Add(coin);
            
            yield return new WaitForSeconds(coinDelay);
        }
        _isSpawning = false;
        DecalCircleCloseClientRpc();
    }
    
    [ClientRpc]
    private void CountDownClientRpc(int sec, int pointIndex, int coinCount)
    { 
        SpawnPoint point = spawnPointList[pointIndex];
        
        if (_decalCircle.showDecal == false)
        {
            _decalCircle.OpenCircle(point.Position, point.Radius);
        }

        if (sec <= 1)
        {
            _decalCircle.StopBlinkIcon();
        }
        MessageSystem.Instance.ShowText(
            $"{point.pointName}쪽 지점에서 {sec}초후 {coinCount}개의 코인이 생성됩니다.", 
            0.8f);
    }
    
    [ClientRpc]
    private void DecalCircleCloseClientRpc()
    {
        _decalCircle.CloseCircle();
    }
}
