using DG.Tweening;
using TMPro;
using UnityEngine;

public class GameText : MonoBehaviour
{
    [SerializeField] private TextMeshPro _textMeshText;

    public void SetPopUo(string value, Vector3 position, Color color)
    {
        _textMeshText.text = value;
        _textMeshText.color = color;
        
        transform.position = position + new Vector3(0, 1.5f);

        Vector3 pos = position;
        float yDelta = 1f;
        float scaleTime = 0.2f;
        float fadeTime = 1.5f;
        
        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(2.5f, scaleTime));
        seq.Append(transform.DOScale(1.2f, scaleTime));
        seq.Append(transform.DOScale(0.3f, fadeTime));
        seq.Join(transform.DOLocalMoveY(pos.y + yDelta, fadeTime));
        seq.AppendCallback(() => Destroy(gameObject));
    }
}
