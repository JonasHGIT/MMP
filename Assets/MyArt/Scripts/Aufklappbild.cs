using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageTransition : MonoBehaviour
{
    public Image image1;          // Bild 1
    public Image image2;          // Bild 2
    public float transitionSpeed = 1f;  // Geschwindigkeit des Übergangs
    private bool transitioning = false;  // Um zu prüfen, ob der Übergang gerade läuft
    private RectTransform rectImage1;    // RectTransform von Bild 1
    private RectTransform rectImage2;    // RectTransform von Bild 2

    void Start()
    {
        // Initialisiert mit Bild 1 sichtbar und Bild 2 unsichtbar
        rectImage1 = image1.GetComponent<RectTransform>();
        rectImage2 = image2.GetComponent<RectTransform>();

        SetVisibility(image1, true);  // Bild 1 ist sichtbar
        SetVisibility(image2, false);  // Bild 2 ist unsichtbar zu Beginn
    }

    void Update()
    {
        // Überprüfen, ob der Mauszeiger in der Hitbox von Bild 1 oder Bild 2 ist
        if (Input.GetMouseButtonDown(0)) // Klick wird erkannt
        {
            if (!transitioning)
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

    // Coroutine, die den Überblend-Effekt durchführt
    private IEnumerator FadeImages(Image fromImage, Image toImage)
    {
        transitioning = true;

        // Bild 1 (fromImage) verblassen lassen
        float timeElapsed = 0;
        while (timeElapsed < transitionSpeed)
        {
            fromImage.canvasRenderer.SetAlpha(1 - (timeElapsed / transitionSpeed));  // Transparenz verringern
            toImage.canvasRenderer.SetAlpha(timeElapsed / transitionSpeed);  // Transparenz von Bild 2 erhöhen
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Sicherstellen, dass die Transparenz am Ende genau 0 oder 1 ist
        fromImage.canvasRenderer.SetAlpha(0);
        toImage.canvasRenderer.SetAlpha(1);

        // Sichtbarkeit der Child-Objekte entsprechend aktualisieren
        SetVisibility(fromImage, false);
        SetVisibility(toImage, true);

        transitioning = false;
    }

    // Setzt die Sichtbarkeit eines Bildes und seiner Child-Objekte
    private void SetVisibility(Image image, bool visible)
    {
        float alpha = visible ? 1 : 0;
        image.canvasRenderer.SetAlpha(alpha);

        // Child-Objekte durchgehen und sichtbar/unsichtbar setzen
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
