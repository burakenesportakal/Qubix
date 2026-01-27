using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    const string MAIN_MENU_SCENE_NAME = "Main Menu";
    const string GAME_SCENE_NAME = "Game";

    public void LoadGame()
    {
        SceneManager.LoadScene(GAME_SCENE_NAME);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(MAIN_MENU_SCENE_NAME);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
