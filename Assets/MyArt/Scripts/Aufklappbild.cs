/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script ermöglicht eine sanfte Übergangsanimation zwischen zwei Bildern. 
 * Wenn der Benutzer auf ein Bild klickt, wird das aktuelle Bild zu einem anderen Bild überblendet.
 * Das Überblenden erfolgt mit einer transparenten Überblendung, die die Sichtbarkeit beider Bilder steuert.
 * 
 * Features:
 * - Übergang zwischen zwei Bildern durch Fade-Effekt
 * - Bildwechsel bei Mausklick auf ein Bild
 * - Möglichkeit, die Geschwindigkeit des Übergangs anzupassen
 * - Überprüfung, ob der Mauszeiger auf einem der Bilder liegt
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageTransition : MonoBehaviour
{
    public Image image1;               // Bild 1
    public Image image2;               // Bild 2
    public float transitionSpeed = 1f; // Geschwindigkeit des Übergangs
    private bool transitioning = false; // Flag, um zu prüfen, ob der Übergang läuft
    private RectTransform rectImage1;   // RectTransform von Bild 1
    private RectTransform rectImage2;   // RectTransform von Bild 2

    // Initialisierung der Bilder und deren Sichtbarkeit
    void Start()
    {
        rectImage1 = image1.GetComponent<RectTransform>();
        rectImage2 = image2.GetComponent<RectTransform>();

        SetVisibility(image1, true);  // Bild 1 ist zu Beginn sichtbar
        SetVisibility(image2, false); // Bild 2 ist zu Beginn unsichtbar
    }

    // Update-Methode, um Mausinteraktionen zu erkennen
    void Update()
    {
        // Wenn der Benutzer mit der linken Maustaste klickt
        if (Input.GetMouseButtonDown(0)) 
        {
            if (!transitioning) // Wenn gerade kein Übergang läuft
            {
                if (IsPointerOverImage(rectImage1) && image1.canvasRenderer.GetAlpha() > 0)
                {
                    // Bild 1 wird zu Bild 2 überblenden
                    StartCoroutine(FadeImages(image1, image2));
                }
                else if (IsPointerOverImage(rectImage2) && image2.canvasRenderer.GetAlpha() > 0)
                {
                    // Bild 2 wird zu Bild 1 überblenden
                    StartCoroutine(FadeImages(image2, image1));
                }
            }
        }
    }

    // Überprüft, ob der Mauszeiger über dem Bild ist
    private bool IsPointerOverImage(RectTransform rect)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out localPoint);
        return rect.rect.Contains(localPoint);
    }

    // Coroutine für den Fade-Effekt zwischen zwei Bildern
    private IEnumerator FadeImages(Image fromImage, Image toImage)
    {
        transitioning = true; // Übergang läuft

        // Übergang von Bild 1 zu Bild 2
        float timeElapsed = 0;
        while (timeElapsed < transitionSpeed)
        {
            // Bild 1 verblasst aus, Bild 2 wird sichtbar
            fromImage.canvasRenderer.SetAlpha(1 - (timeElapsed / transitionSpeed));
            toImage.canvasRenderer.SetAlpha(timeElapsed / transitionSpeed);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Sicherstellen, dass die Transparenz am Ende exakt 0 und 1 ist
        fromImage.canvasRenderer.SetAlpha(0);
        toImage.canvasRenderer.SetAlpha(1);

        // Sichtbarkeit der Bilder und ihrer Child-Objekte anpassen
        SetVisibility(fromImage, false);
        SetVisibility(toImage, true);

        transitioning = false; // Übergang abgeschlossen
    }

    // Setzt die Sichtbarkeit eines Bildes (und seiner Kinder)
    private void SetVisibility(Image image, bool visible)
    {
        float alpha = visible ? 1 : 0;
        image.canvasRenderer.SetAlpha(alpha);

        // Alle Child-Objekte durchgehen und deren Sichtbarkeit ebenfalls setzen
        foreach (Transform child in image.transform)
        {
            CanvasRenderer renderer = child.GetComponent<CanvasRenderer>();
            if (renderer != null)
            {
                renderer.SetAlpha(alpha);
            }
        }
    }
}
