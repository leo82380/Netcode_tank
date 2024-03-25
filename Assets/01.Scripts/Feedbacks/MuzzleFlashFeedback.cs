using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MuzzleFlashFeedback : Feedback
{
    [SerializeField] private GameObject _muzzleFlash;
    [SerializeField] private Light2D _muzzleLight;
    [SerializeField] private float _turnOnTime = 0.08f;
    [SerializeField] private bool _defaulrState = false;
    

    public override void CompleteFeedback()
    {
        StopAllCoroutines();
        _muzzleFlash.SetActive(_defaulrState);
        _muzzleLight.gameObject.SetActive(_defaulrState);
    }
    
    public override void CreatFeedback()
    {
        StartCoroutine(ActiveCoroutine());
    }

    private IEnumerator ActiveCoroutine()
    {
        _muzzleLight.gameObject.SetActive(true);
        _muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(_turnOnTime);
        _muzzleFlash.SetActive(false);
        _muzzleLight.gameObject.SetActive(false);
    }
}
