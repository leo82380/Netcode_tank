using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class ClientGameManager
{
    public async Task InitAsync()
    {
        // UGS 인증파트
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene(SceneNames.MenuScene);
    }
}
