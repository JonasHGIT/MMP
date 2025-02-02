/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script ermöglicht Drag-and-Drop-Interaktionen für Karten in einem UI. Es simuliert das Einführen von Karten in einen Kartenschlitz und zeigt dynamische Informationen basierend auf der gewählten Karte an. 
 * Jede Karte kann per Drag and Drop in einen Slot gezogen werden, und beim Ablegen werden detaillierte Informationen zur Karte angezeigt, einschließlich einer Audio-Wiedergabe zur Erklärung.
 * 
 * Features:
 * - Ziehe eine Karte per Drag and Drop in den Slot.
 * - Anzeige von spezifischen Informationen zu jeder Karte, wenn sie korrekt abgelegt wird.
 * - Audio-Feedback, das mit jeder Karte verknüpft ist.
 * - Rücksetzen der Karte an ihre Ausgangsposition, wenn sie nicht korrekt abgelegt wird.
 * - Verwendung von UI-Elementen wie Buttons und Textanzeigen zur Interaktion mit dem Benutzer.
 * 
 */

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class DragAndDropCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Referenzen für Drag-and-Drop-Interaktionen
    private RectTransform rectTransform;  // Speichert die Referenz auf das RectTransform der Karte
    private Canvas canvas;  // Referenz auf das übergeordnete Canvas
    private Vector3 startPosition;  // Startposition der Karte, falls sie zurückgesetzt werden muss
    private Quaternion startRotation;  // Startrotation der Karte, um sie bei Bedarf zurückzusetzen
    private int siblingIndex;  // Sibling Index zur Verwaltung der Position innerhalb des Canvas

    // Slot und Textreferenzen
    public Transform cardSlot;  // Der Slot, in den die Karte gezogen werden muss
    public static GameObject cardInSlot = null;  // Speichert die Karte, die im Slot abgelegt ist
    public TMP_Text slotText;  // Text-UI-Element, das Informationen zur Karte anzeigt
    public AudioSource audioSource;  // Audioquelle für die Wiedergabe von Kartenspezifischen Sounds
    public AudioClip card1Clip, card2Clip, card3Clip, card4Clip, defaultClip;  // Audio-Clips für jede Karte und den Standard-Sound

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();  // Holen der RectTransform-Komponente der Karte
        canvas = GetComponentInParent<Canvas>();  // Holen des Canvas übergeordnete Objekts
        startPosition = rectTransform.position;  // Speichern der ursprünglichen Position der Karte
        startRotation = rectTransform.rotation;  // Speichern der ursprünglichen Rotation der Karte
        siblingIndex = transform.GetSiblingIndex();  // Speichern des Sibling-Index

        // Überprüfen, ob slotText einen Button hat, um ein Klick-Event hinzuzufügen
        if (slotText != null)
        {
            Button textButton = slotText.GetComponent<Button>();
            if (textButton == null)
            {
                textButton = slotText.gameObject.AddComponent<Button>();  // Button hinzufügen, falls nicht vorhanden
            }
            textButton.onClick.AddListener(PlayTextAudio);  // Audio abspielen, wenn auf den Text geklickt wird
        }

        slotText.text = "Ziehe eine Karte per Drag and Drop in den Slot!";  // Anfangstext
        audioSource.clip = defaultClip;  // Standard-Audioclip einstellen
    }

    // Wird aufgerufen, wenn das Dragging beginnt
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetAsLastSibling();  // Setzt das GameObject als das letzte Sibling (oberste Ebene)
    }

    // Wird bei jedem Dragging-Update aufgerufen
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;  // Bewege die Karte mit dem Drag
        transform.SetSiblingIndex(siblingIndex);  // Setze den Sibling-Index zurück, um sicherzustellen, dass die Karte immer an der richtigen Position bleibt

        // Berechnung der Distanz zur Slot-Position und sanfte Rotation
        float distanceToSlot = Vector3.Distance(rectTransform.position, cardSlot.position);
        float normalizedDistance = Mathf.Clamp01(distanceToSlot / 350f);
        rectTransform.rotation = Quaternion.Slerp(startRotation, Quaternion.identity, 1f - normalizedDistance);
    }

    // Wird aufgerufen, wenn das Dragging endet
    public void OnEndDrag(PointerEventData eventData)
    {
        // Wenn die Karte nahe genug am Slot ist, wird sie dort abgelegt
        if (Vector3.Distance(rectTransform.position, cardSlot.position) < 50f)
        {
            // Falls bereits eine Karte im Slot ist, zurücksetzen
            if (cardInSlot != null && cardInSlot != gameObject)
            {
                DragAndDropCard previousCard = cardInSlot.GetComponent<DragAndDropCard>();
                previousCard.ResetToStartPosition();
            }

            rectTransform.position = cardSlot.position;  // Setze die Karte in den Slot
            rectTransform.rotation = Quaternion.identity;  // Setze die Rotation zurück
            cardInSlot = gameObject;  // Markiere die Karte als im Slot

            // Ändere den Text je nach Karte
            if (slotText != null)
            {
                switch (gameObject.name)
                {
                    case "Card1":
                        slotText.text = "<b>Infos zu Karte 1: Bankkarte (Debitkarte)</b>\n\n" + 
                            "- <b>Verwendung:</b> Direkte Abbuchung vom Bankkonto bei Zahlungen oder Abhebungen.\n" +
                            "- <b>Name des Karteninhabers:</b> Identifiziert den Besitzer.\n" +
                            "- <b>Kartennummer:</b> Einzigartige Nummer, oft 16-stellig.\n" +
                            "- <b>Ablaufdatum:</b> Monat und Jahr, bis wann die Karte gültig ist.\n" +
                            "- <b>Bankname:</b> Herausgebende Bank oder Institution.\n" +
                            "- <b>Chip und Magnetstreifen:</b> Für sichere Transaktionen und Offline-Lesbarkeit.\n" +
                            "- <b>CVV (auf der Rückseite):</b> Sicherheitscode für Online-Zahlungen.\n" +
                            "- <b>Kontaktlos-Symbol (bei modernen Karten):</b> Ermöglicht NFC-Zahlungen.";
                        audioSource.clip = card1Clip;
                        break;
                    case "Card2":
                        slotText.text = "<b>Infos zu Karte 2: Kreditkarte</b>\n\n" +
                            "- <b>Verwendung:</b> Ermöglicht das Bezahlen auf Kreditbasis, mit einer Abrechnung am Ende eines Abrechnungszeitraums.\n" +
                            "- <b>Name des Karteninhabers:</b> Für die Identifikation.\n" +
                            "- <b>Kartennummer:</b> Einzigartige Identifikation, meist 16-stellig.\n" +
                            "- <b>Ablaufdatum:</b> Die Karte wird nach diesem Datum ungültig.\n" +
                            "- <b>Bank- oder Kreditkartenanbieter:</b> z. B. Visa, Mastercard, American Express.\n" +
                            "- <b>Chip und Magnetstreifen:</b> Für Transaktionen.\n" +
                            "- <b>CVV/Sicherheitscode:</b> Zur Verifizierung bei Online-Zahlungen.\n" +
                            "- <b>Kreditlimit (nicht direkt auf der Karte):</b> Der maximal verfügbare Kreditbetrag.";
                        audioSource.clip = card2Clip;
                        break;
                    case "Card3":
                        slotText.text = "<b>Infos zu Karte 3: Prepaid-Karte</b>\n\n" +
                            "- <b>Verwendung:</b> Eine Karte, die vorab mit einem bestimmten Betrag aufgeladen wird, ähnlich einer Debitkarte, aber ohne direkte Verbindung zu einem Bankkonto.\n" +
                            "- <b>Kartennummer:</b> Zur Identifikation und Nutzung.\n" +
                            "- <b>Ablaufdatum:</b> Gültigkeitszeitraum.\n" +
                            "- <b>Anbieterlogo:</b> z. B. Visa, Mastercard.\n" +
                            "- <b>Kontaktlos-Symbol:</b> Für NFC-Zahlungen (bei neueren Karten).\n" +
                            "- <b>Aufladestatus (meist online einsehbar):</b> Zeigt den verfügbaren Betrag.";
                        audioSource.clip = card3Clip;
                        break;
                    case "Card4":
                        slotText.text = "<b>Infos zu Karte 4: EC-Karte (girocard)</b>\n\n" +
                            "- <b>Verwendung:</b> Direktes Bezahlen und Geldabheben von einem verknüpften Bankkonto, häufig in Europa.\n" +
                            "- <b>Name des Karteninhabers:</b> Für die Identifikation.\n" +
                            "- <b>IBAN (manchmal aufgedruckt):</b> Verknüpftes Bankkonto.\n" +
                            "- <b>Banklogo und/oder girocard-Logo</b>.\n" +
                            "- <b>Kartennummer:</b> Nicht immer vorhanden, variiert nach Anbieter.\n" +
                            "- <b>Chip und Magnetstreifen:</b> Für Transaktionen.\n" +
                            "- <b>Maestro- oder V-Pay-Logo:</b> bei internationalen Zahlungen.";
                        audioSource.clip = card4Clip;
                        break;
                    default:
                        slotText.text = "Ziehe eine Karte per Drag and Drop in den Slot!";
                        audioSource.clip = defaultClip;
                        break;
                }
            }
        }
        else
        {
            ResetToStartPosition();  // Setzt die Karte zurück, wenn sie nicht korrekt abgelegt wurde
        }
    }

    // Setzt die Karte zurück zu ihrer Startposition
    public void ResetToStartPosition()
    {
        StartCoroutine(SmoothReset());  // Starte die Rücksetz-Animation
        if (cardInSlot == gameObject)
        {
            cardInSlot = null;
            slotText.text = "Ziehe eine Karte per Drag and Drop in den Slot!";
            audioSource.clip = defaultClip;
        }
    }

    // Führt eine weiche Rücksetz-Animation durch
    private System.Collections.IEnumerator SmoothReset()
    {
        float duration = 0.3f;
        float elapsed = 0f;
        Vector3 initialPosition = rectTransform.position;
        Quaternion initialRotation = rectTransform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.position = Vector3.Lerp(initialPosition, startPosition, t);
            rectTransform.rotation = Quaternion.Slerp(initialRotation, startRotation, t);
            yield return null;
        }

        rectTransform.position = startPosition;
        rectTransform.rotation = startRotation;
    }

    // Spielt das Audio ab, wenn auf den Text geklickt wird
    public void PlayTextAudio()
    {
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.Play();
        }
    }
}
