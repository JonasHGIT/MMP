/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script sucht in der aktuellen Szene nach Objekten mit dem Tag "Interactable"
 * und wendet einen "Wobble"-Effekt auf diese Objekte an, wenn sie das "WobbleEffect"-Script besitzen.
 * 
 * Features:
 * - Suche nach Objekten mit dem Tag "Interactable" in der aktuellen Szene
 * - Wendet den Wobble-Effekt auf diese Objekte an, falls vorhanden
 * - Loggt detaillierte Informationen über die gefundenen Objekte und angewandten Effekte
 */
using UnityEngine;
using UnityEngine.SceneManagement;

public class WobbleButton : MonoBehaviour
{
    // Methode zum Highlighten und Wackeln der interaktiven Objekte in der aktuellen Szene
    public void HighlightInteractables()
    {
        // Aktive Szene ermitteln
        Scene currentScene = SceneManager.GetActiveScene();
        
        // Alle Objekte mit dem Tag "Interactable" suchen
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");

        Debug.Log($"[HighlightInteractables] Anzahl der gefundenen Interactable-Objekte: {interactables.Length}");

        foreach (GameObject obj in interactables)
        {
            // Überprüfen, ob das Objekt zur aktuellen Szene gehört
            if (obj.scene == currentScene)
            {
                Debug.Log($"[HighlightInteractables] Objekt gefunden: {obj.name}, Position: {obj.transform.position}");
                
                WobbleEffect wobble = obj.GetComponent<WobbleEffect>();
                if (wobble != null)
                {
                    wobble.StartWobble();
                    Debug.Log($"[HighlightInteractables] WobbleEffect gestartet für Objekt: {obj.name}");
                }
                else
                {
                    Debug.LogWarning($"[HighlightInteractables] Kein WobbleEffect an Objekt: {obj.name}");
                }
            }
            else
            {
                Debug.Log($"[HighlightInteractables] Objekt {obj.name} gehört nicht zur aktuellen Szene.");
            }
        }
    }
}
