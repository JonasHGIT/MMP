/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script steuert das Laden und Beenden von Szenen in einem Unity-Projekt.
 * Es bietet Funktionen zum Laden einer spezifischen Szene und zum Beenden des Spiels.
 * 
 * Features:
 * - Lade eine Szene anhand des übergebenen Namens
 * - Beende das Spiel mit einer entsprechenden Log-Ausgabe
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    // Methode zum Laden einer Szene nach einer Pause von 0,5 Sekunden um das richtige Abspielverhalten des Buttonsounds zu gewährleisten
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAfterDelay(sceneName, 0.5f));  // Starte die Coroutine
    }

    // Coroutine für das verzögerte Laden der Szene
    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);  // Warten für die angegebene Zeit (0,5 Sekunden)
        SceneManager.LoadScene(sceneName);  // Lade die neue Szene
    }

    // Methode zum Beenden des Spiels
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet.");
        Application.Quit(); // Funktioniert nur in der Build-Version
    }
}
