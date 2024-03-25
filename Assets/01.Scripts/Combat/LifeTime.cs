using UnityEngine;

public class LifeTime : MonoBehaviour
{
    [SerializeField] private float _lifeTime;
    private float _currentTime;

    private void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
