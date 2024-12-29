using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // Lade die neue Szene
    }

    // Methode zum Beenden des Spiels
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet.");
        Application.Quit(); // Funktioniert nur in der Build-Version
    }
}
