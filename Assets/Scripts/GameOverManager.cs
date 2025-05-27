using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void BackToGame()
    {
        // Reloads the main game scene by name or index
        SceneManager.LoadScene("MainLevel"); // replace with your actual game scene name
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
