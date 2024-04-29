using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private GameText _prefab;
    
    public static TextManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void PopUpText(string value, Vector3 pos, Color color)
    {
        GameText text = Instantiate(_prefab, pos, Quaternion.identity);
        text.SetPopUo(value, pos, color);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            PopUpText("999", Vector3.zero, Color.red);
        }
    }
}
