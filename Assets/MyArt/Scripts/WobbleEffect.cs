/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script fügt einem GameObject einen "Wobble"-Effekt hinzu. Das Objekt wackelt über eine bestimmte Dauer 
 * und mit einer bestimmten Intensität, basierend auf den Parametern wobbleDuration und wobbleAmount.
 * 
 * Features:
 * - Wobble-Effekt, der in der Dauer und Intensität angepasst werden kann
 * - Verwendung von Sinus- und Kosinus-Funktionen für das Wackeln in X- und Y-Achse
 * - Rückkehr zur Originalposition nach dem Wackeln
 */
using UnityEngine;
using System.Collections;

public class WobbleEffect : MonoBehaviour
{
    public float wobbleDuration = 3.0f;   // Dauer des Wackel-Effekts
    public float wobbleAmount = 3f;       // Intensität des Wackel-Effekts
    private bool isWobbling = false;      // Flag, das anzeigt, ob der Wackel-Effekt gerade aktiv ist

    // Methode zum Starten des Wackel-Effekts
    public void StartWobble()
    {
        // Wenn der Effekt noch nicht läuft, starte die Coroutine
        if (!isWobbling)
        {
            StartCoroutine(Wobble());
        }
    }

    // Coroutine, die den Wackel-Effekt über die angegebene Dauer ausführt
    private IEnumerator Wobble()
    {
        isWobbling = true;      // Setze das Flag, dass der Effekt läuft
        float elapsedTime = 0;  // Verstrichene Zeit für den Effekt

        // Solange die verstrichene Zeit kleiner ist als die Dauer des Effekts
        while (elapsedTime < wobbleDuration)
        {
            // Berechne die Verschiebung in X- und Y-Richtung basierend auf Sinus- und Kosinus-Funktionen
            float wobbleOffsetX = Mathf.Sin(elapsedTime * Mathf.PI * 4) * wobbleAmount;
            float wobbleOffsetY = Mathf.Cos(elapsedTime * Mathf.PI * 4) * wobbleAmount;
            float wobbleOffsetZ = wobbleOffsetX * wobbleOffsetY; // Z-Achse als Kombination von X- und Y

            // Wackeln: Verschiebe das Objekt in den berechneten Richtungen
            transform.Translate(new Vector3(wobbleOffsetX, wobbleOffsetY, wobbleOffsetZ), Space.Self);

            elapsedTime += Time.deltaTime;  // Erhöhe die verstrichene Zeit
            yield return null;  // Warten auf den nächsten Frame

            // Rückkehr zur Originalposition in kleinen Schritten
            transform.Translate(new Vector3(-wobbleOffsetX, -wobbleOffsetY, -wobbleOffsetZ), Space.Self);
        }

        isWobbling = false;  // Setze das Flag zurück, wenn der Effekt beendet ist
    
    }
}
