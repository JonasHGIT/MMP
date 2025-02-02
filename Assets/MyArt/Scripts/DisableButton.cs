/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script deaktiviert einen Button, wenn das "verblassen"-Child-GameObject aktiv ist,
 * und macht ihn interaktiv, wenn es nicht aktiv ist. Zudem wird geprüft, ob der Button
 * eine verwendete Methode in der OnClick-Liste hat, um das "verblassen"-Objekt zu steuern.
 * 
 * Features:
 * - Deaktivierung des Buttons basierend auf dem "verblassen"-Objekt
 * - Überprüfung der OnClick-Liste des Buttons
 * - Aktivierung/Deaktivierung des "verblassen"-Objekts
 */
using UnityEngine;
using UnityEngine.UI;

public class DisableButton : MonoBehaviour
{
    private Button button;            // Der Button selbst
    private GameObject verblassen;    // Das "verblassen"-Child-GameObject

    private void Start()
    {
        // Button-Komponente automatisch holen
        button = GetComponent<Button>();
        if (button == null)
        {
            Debug.LogError("Dieses Script muss an ein GameObject mit einem Button-Component angehängt werden!");
            return;
        }

        // Die Hierarchie durchsuchen, um das "verblassen"-Child zu finden
        verblassen = FindChildByName(transform, "verblassen");

        // Fehler, falls die Hierarchie nicht wie angegeben ist
        if (verblassen == null)
        {
            Debug.LogError("Das 'verblassen'-GameObject konnte nicht gefunden werden. Überprüfe die Hierarchie (Button -> Image -> Image).");
        }

        // Überprüfen, ob der Button tatsächlich eine verwendete Methode hat
        CheckButtonUsage();
    }

    private void Update()
    {
        if (verblassen != null && button != null)
        {
            // Button interaktiv setzen basierend auf dem aktiven Zustand von 'verblassen'
            button.interactable = !verblassen.activeSelf;
        }
    }

    // Überprüfen, ob der Button tatsächlich eine Methode in der OnClick-Liste benutzt
    private void CheckButtonUsage()
    {
        if (button != null && verblassen != null)
        {
            // Prüfen, ob in der OnClick-Liste tatsächlich Einträge vorhanden sind
            bool hasValidFunction = false;

            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
            {
                string methodName = button.onClick.GetPersistentMethodName(i);
                if (!string.IsNullOrEmpty(methodName))
                {
                    hasValidFunction = true;
                    break;
                }
            }

            // Wenn keine gültige Methode vorhanden ist, aktiviere "verblassen"
            if (!hasValidFunction)
            {
                verblassen.SetActive(true);
                Debug.Log("Keine verwendete Methode im Button-OnClick. 'verblassen' wurde aktiviert.");
            }
            else
            {
                verblassen.SetActive(false);
                Debug.Log("Verwendete Methode gefunden. 'verblassen' bleibt deaktiviert.");
            }
        }
    }

    // Hilfsmethode zum Finden eines Kindes nach Name, rekursiv
    private GameObject FindChildByName(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child.gameObject;

            GameObject found = FindChildByName(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }
}
