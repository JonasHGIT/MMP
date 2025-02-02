/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script ermöglicht das Ziehen einer Karte in einem UI und das Verhindern, dass sie außerhalb des vorgesehenen Bereichs abgelegt wird.
 * Es beinhaltet auch eine Drehung der Karte basierend auf der Entfernung zum Ziel (Kartenschlitz) und setzt die Karte zurück, wenn sie nicht im richtigen Bereich abgelegt wird.
 * 
 * Features:
 * - Ziehen von Karten im UI
 * - Rotation der Karte basierend auf der Entfernung zum Ziel
 * - Zurücksetzen der Karte, wenn sie nicht im richtigen Bereich abgelegt wird
 * - Interaktion mit einem Kartenschlitz (z.B. in einem Geldautomaten)
 */

using UnityEngine;
using UnityEngine.EventSystems;

public class DragCardHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 startPosition;
    public Transform cardSlot; // Referenz zum Kartenschlitz

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();  // Holen der RectTransform-Komponente für Positions- und Drehungssteuerung
        canvas = GetComponentInParent<Canvas>();        // Holen des Canvas-Referenz für die Skalierung der Canvas-Einheit
        startPosition = rectTransform.position;         // Startposition der Karte speichern, um sie bei Bedarf zurückzusetzen
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;  // Speichern der Startposition für das Zurücksetzen bei Ende des Ziehens

        // Setze die Karte an die oberste Ebene, sodass sie immer im Vordergrund bleibt
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Verschieben der Karte basierend auf der Mausbewegung unter Berücksichtigung der Canvas-Skalierung
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Berechne die Entfernung zur Zielposition (Kartenschlitz)
        float distance = Vector3.Distance(rectTransform.position, cardSlot.position);

        // Rotieren der Karte basierend auf der Entfernung zum Ziel
        RotateCardBasedOnDistance(distance - 50f); // Subtrahiere 50f, um sicherzustellen, dass die Karte erst ab einer bestimmten Entfernung rotiert
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Geldautomat geldautomat = FindObjectOfType<Geldautomat>();  // Suche den Geldautomaten im Spiel

        if (geldautomat != null)
        {
            // Überprüfen, ob die Karte nah genug am Kartenschlitz des Geldautomaten ist
            if (Vector3.Distance(rectTransform.position, geldautomat.cardSlot.position) < 50f)
            {
                rectTransform.position = geldautomat.cardSlot.position;  // Setze die Karte auf die Position des Kartenschlitzes
                geldautomat.OnCardDragEnd();  // Benachrichtige den Geldautomaten, dass das Ziehen beendet ist
            }
            else
            {
                // Setze die Karte zurück, wenn sie nicht in der Nähe des Kartenschlitzes abgelegt wurde
                rectTransform.position = startPosition;
                rectTransform.rotation = Quaternion.identity;  // Setze die Drehung zurück
            }
        }
    }

    private void RotateCardBasedOnDistance(float distance)
    {
        float maxDistance = 350f;  // Maximale Entfernung, bei der die Karte zu rotieren beginnt
        float rotationAngle = Mathf.Clamp(90f * (1f - distance / maxDistance), 0f, 90f);  // Berechne den Drehwinkel basierend auf der Entfernung

        // Drehe die Karte basierend auf dem berechneten Winkel
        rectTransform.rotation = Quaternion.Euler(0, 0, -rotationAngle);  // Rotiert die Karte negativ entlang der Z-Achse
    }
}
