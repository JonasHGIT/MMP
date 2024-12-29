using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // Für den TMP-Text

public class DragAndDropCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 startPosition;
    private int siblingIndex; // Speichert den ursprünglichen z-Index der Karte

    public Transform cardSlot; // Der Slot, in den die Karte gezogen werden kann
    public static GameObject cardInSlot = null; // Verfolgt die Karte, die sich im Slot befindet
    public TMP_Text slotText; // TMP-Text, der sich basierend auf der Karte im Slot ändert

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.position; // Speichere die Anfangsposition der Karte
        siblingIndex = transform.GetSiblingIndex(); // Speichere den ursprünglichen z-Index
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Setze Karte auf die höchste Ebene
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Bewege die Karte basierend auf der Mausbewegung
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        transform.SetSiblingIndex(siblingIndex); // Wiederherstellen des ursprünglichen z-Index
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Überprüfe, ob die Karte nah genug am Ziel-Slot ist
        if (Vector3.Distance(rectTransform.position, cardSlot.position) < 50f) // Toleranzradius von 50
        {
            // Wenn bereits eine Karte im Slot ist, schicke sie zurück zu ihrer Startposition
            if (cardInSlot != null && cardInSlot != gameObject)
            {
                DragAndDropCard previousCard = cardInSlot.GetComponent<DragAndDropCard>();
                previousCard.ResetToStartPosition();
            }

            // Setze die neue Karte in den Slot
            rectTransform.position = cardSlot.position;
            cardInSlot = gameObject;

            // Aktualisiere den TMP-Text basierend auf der Karte
            if (slotText != null)
            {
                switch (gameObject.name)
                {
                    case "Card1":
                        slotText.text = "<b>Infos zu Karte 1: Bankkarte (Debitkarte)</b>\n\n"
    + "- <b>Verwendung:</b> Direkte Abbuchung vom Bankkonto bei Zahlungen oder Abhebungen.\n"
    + "- <b>Name des Karteninhabers:</b> Identifiziert den Besitzer.\n"
    + "- <b>Kartennummer:</b> Einzigartige Nummer, oft 16-stellig.\n"
    + "- <b>Ablaufdatum:</b> Monat und Jahr, bis wann die Karte gültig ist.\n"
    + "- <b>Bankname:</b> Herausgebende Bank oder Institution.\n"
    + "- <b>Chip und Magnetstreifen:</b> Für sichere Transaktionen und Offline-Lesbarkeit.\n"
    + "- <b>CVV (auf der Rückseite):</b> Sicherheitscode für Online-Zahlungen.\n"
    + "- <b>Kontaktlos-Symbol (bei modernen Karten):</b> Ermöglicht NFC-Zahlungen.";
                        break;
                    case "Card2":
                        slotText.text = "<b>Infos zu Karte 2: Kreditkarte</b>\n\n"
    + "- <b>Verwendung:</b> Ermöglicht das Bezahlen auf Kreditbasis, mit einer Abrechnung am Ende eines Abrechnungszeitraums.\n"
    + "- <b>Name des Karteninhabers:</b> Für die Identifikation.\n"
    + "- <b>Kartennummer:</b> Einzigartige Identifikation, meist 16-stellig.\n"
    + "- <b>Ablaufdatum:</b> Die Karte wird nach diesem Datum ungültig.\n"
    + "- <b>Bank- oder Kreditkartenanbieter:</b> z. B. Visa, Mastercard, American Express.\n"
    + "- <b>Chip und Magnetstreifen:</b> Für Transaktionen.\n"
    + "- <b>CVV/Sicherheitscode:</b> Zur Verifizierung bei Online-Zahlungen.\n"
    + "- <b>Kreditlimit (nicht direkt auf der Karte):</b> Der maximal verfügbare Kreditbetrag.";
                        break;
                    case "Card3":
                        slotText.text = "<b>Infos zu Karte 3: Prepaid-Karte</b>\n\n"
    + "- <b>Verwendung:</b> Eine Karte, die vorab mit einem bestimmten Betrag aufgeladen wird, ähnlich einer Debitkarte, aber ohne direkte Verbindung zu einem Bankkonto.\n"
    + "- <b>Kartennummer:</b> Zur Identifikation und Nutzung.\n"
    + "- <b>Ablaufdatum:</b> Gültigkeitszeitraum.\n"
    + "- <b>Anbieterlogo:</b> z. B. Visa, Mastercard.\n"
    + "- <b>Kontaktlos-Symbol:</b> Für NFC-Zahlungen (bei neueren Karten).\n"
    + "- <b>Aufladestatus (meist online einsehbar):</b> Zeigt den verfügbaren Betrag.";
                        break;
                    case "Card4":
                        slotText.text = "<b>Infos zu Karte 4: EC-Karte (girocard)</b>\n\n"
    + "- <b>Verwendung:</b> Direktes Bezahlen und Geldabheben von einem verknüpften Bankkonto, häufig in Europa.\n"
    + "- <b>Name des Karteninhabers:</b> Für die Identifikation.\n"
    + "- <b>IBAN (manchmal aufgedruckt):</b> Verknüpftes Bankkonto.\n"
    + "- <b>Banklogo und/oder girocard-Logo</b>.\n"
    + "- <b>Kartennummer:</b> Nicht immer vorhanden, variiert nach Anbieter.\n"
    + "- <b>Chip und Magnetstreifen:</b> Für Transaktionen.\n"
    + "- <b>Maestro- oder V-Pay-Logo:</b> bei internationalen Zahlungen.";
                        break;
                }
            }
        }
        else
        {
            // Karte zurück zur Startposition bewegen, wenn sie nicht in den Slot gezogen wurde
            ResetToStartPosition();
        }
    }

    public void ResetToStartPosition()
    {
        rectTransform.position = startPosition;
        transform.SetSiblingIndex(siblingIndex); // Wiederherstellen des ursprünglichen z-Index

        // Wenn diese Karte die im Slot war, entferne sie aus dem Slot
        if (cardInSlot == gameObject)
        {
            cardInSlot = null;

            // Aktualisiere den TMP-Text, um den Slot als leer anzuzeigen
            if (slotText != null)
            {
                slotText.text = "<b>Infos zur Karte:</b>\n\n"
                + "Ziehe eine Karte per Drag and Drop in den Slot!";
            }
        }
    }
}
