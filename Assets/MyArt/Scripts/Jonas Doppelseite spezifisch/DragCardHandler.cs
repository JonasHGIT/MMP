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
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = rectTransform.position;

        // Setze Karte an die höchste Ebene
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Verschieben der Karte basierend auf der Mausbewegung
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

        // Berechne die Entfernung zum Kartenschlitz
        float distance = Vector3.Distance(rectTransform.position, cardSlot.position);

        // Drehung basierend auf der Entfernung
        RotateCardBasedOnDistance(distance-50f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Geldautomat geldautomat = FindObjectOfType<Geldautomat>();

        if (geldautomat != null)
        {
            if (Vector3.Distance(rectTransform.position, geldautomat.cardSlot.position) < 50f)
            {
                rectTransform.position = geldautomat.cardSlot.position;
                geldautomat.OnCardDragEnd();
            }
            else
            {
                // Zurücksetzen, wenn nicht im Kartenschlitz
                rectTransform.position = startPosition;
                rectTransform.rotation = Quaternion.identity; // Drehung zurücksetzen
            }
        }
    }

    private void RotateCardBasedOnDistance(float distance)
    {
        float maxDistance = 350f; // Maximale Entfernung, bei der die Karte beginnt zu rotieren
        float rotationAngle = Mathf.Clamp(90f * (1f - distance / maxDistance), 0f, 90f);

        // Drehung der Karte nach oben (negative Rotation)
        rectTransform.rotation = Quaternion.Euler(0, 0, -rotationAngle);
    }
}
