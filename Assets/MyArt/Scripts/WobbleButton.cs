using UnityEngine;
using UnityEngine.SceneManagement;

public class WobbleButton : MonoBehaviour
{
    public void HighlightInteractables()
    {
        // Aktive Szene ermitteln
        Scene currentScene = SceneManager.GetActiveScene();
        
        // Alle Objekte mit dem Tag "Interactable" suchen
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        foreach (GameObject obj in interactables)
        {
            // Überprüfen, ob das Objekt zur aktuellen Szene gehört
            if (obj.scene == currentScene)
            {
                WobbleEffect wobble = obj.GetComponent<WobbleEffect>();
                if (wobble != null)
                {
                    wobble.StartWobble();
                }
            }
        }
    }
}
