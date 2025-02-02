/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script ermöglicht das Ziehen und Ablegen von Summen auf Slots,
 * führt Simulationen mit Zinsberechnungen durch und zeigt die Ergebnisse an.
 * 
 * Features:
 * - Drag-and-Drop der Summen
 * - Zinsberechnung für mehrere Jahre
 * - Anzeige der Summen und des aktuellen Jahres
 * - Möglichkeit zur Simulation von Zinseszins
 */

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class DragAndDropSum : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector3 startPosition;

    [Header("Drag and Drop Konfiguration")]
    [SerializeField] private Transform[] slots; // Array von Slots
    [SerializeField] private TMP_Text[] sumTexts; // TMP-Texte für die Summen
    [SerializeField] private TMP_Text interestText; // Zinsanzeige für Slot 2 (in %)
    [SerializeField] private TMP_Text yearsText; // Jahresanzeige für Slot 2
    [SerializeField] private TMP_InputField interestInputFieldSlot3; // Zinsanzeige für Slot 3 (Inputfeld)
    [SerializeField] private TMP_InputField yearsInputFieldSlot3; // Jahresanzeige für Slot 3 (Inputfeld)
    [SerializeField] private TMP_Text currentYearText; // Anzeige für aktuelles Jahr
    [SerializeField] private Button simulateButton; // Simulations-Button

    private Transform currentSlot; // Der Slot, in dem diese Summe aktuell ist

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        startPosition = rectTransform.position;

        UpdateSumHeight(); // Passe die Höhe des Summen-Elements direkt zu Beginn an

        // Simulation starten, wenn der Button gedrückt wird
        simulateButton.onClick.AddListener(StartSimulation);

        // InputField-Events für Slot 3 einrichten
        if (interestInputFieldSlot3 != null)
        {
            interestInputFieldSlot3.onEndEdit.AddListener(OnInterestInputFieldEndEdit);
        }
        if (yearsInputFieldSlot3 != null)
        {
            yearsInputFieldSlot3.onEndEdit.AddListener(OnYearsInputFieldEndEdit);
        }
    }

    /// <summary>
    /// Wird aufgerufen, wenn das Ziehen des Objekts beginnt.
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        rectTransform.SetParent(canvas.transform, true);
        transform.SetAsLastSibling();
    }

    /// <summary>
    /// Wird kontinuierlich aufgerufen, während das Objekt gezogen wird.
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    /// <summary>
    /// Wird aufgerufen, wenn das Ziehen des Objekts endet.
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        Transform closestSlot = null;
        float closestDistance = float.MaxValue;

        foreach (var slot in slots)
        {
            float distance = Vector3.Distance(rectTransform.position, slot.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        if (closestSlot != null && closestDistance < 100f)
        {
            SnapToSlot(closestSlot);
        }
        else
        {
            ResetToStartPosition();
        }
    }

    /// <summary>
    /// Positioniert das Objekt am nächsten Slot.
    /// </summary>
    private void SnapToSlot(Transform slot)
    {
        rectTransform.SetParent(slot);

        int stackIndex = slot.childCount - 1;
        float dynamicYOffset = rectTransform.sizeDelta.y / 2; // Dynamischer Y-Offset basierend auf der Höhe des Elements
        rectTransform.anchorMin = new Vector2(0.5f, 0);
        rectTransform.anchorMax = new Vector2(0.5f, 0);
        rectTransform.anchoredPosition = new Vector2(0, dynamicYOffset + stackIndex * rectTransform.sizeDelta.y);

        currentSlot = slot;

        UpdateSumText(slot);
    }

    /// <summary>
    /// Passt die Höhe des Summen-Elements an.
    /// </summary>
    private void UpdateSumHeight()
    {
        float value = GetSumValue();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, value);
    }

    /// <summary>
    /// Aktualisiert den Text für die Summe im Slot.
    /// </summary>
    private void UpdateSumText(Transform slot)
    {
        if (sumTexts != null)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == slot)
                {
                    float totalValue = 0f;

                    foreach (Transform child in slot)
                    {
                        DragAndDropSum sum = child.GetComponent<DragAndDropSum>();
                        if (sum != null)
                        {
                            totalValue += sum.GetSumValue();
                        }
                    }

                    sumTexts[i].text = $"€ {totalValue:F2}";
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Setzt das Objekt zurück an die Startposition.
    /// </summary>
    public void ResetToStartPosition()
    {
        rectTransform.position = startPosition;
        rectTransform.SetParent(canvas.transform, true);

        if (currentSlot != null)
        {
            UpdateSumText(currentSlot);
            currentSlot = null;
        }
    }

    /// <summary>
    /// Holt den Wert der Summe aus dem Namen des GameObjects.
    /// </summary>
    private float GetSumValue()
    {
        string[] parts = gameObject.name.Split('_');
        if (parts.Length > 1 && float.TryParse(parts[1], out float value))
        {
            return value;
        }

        return 100f;
    }

    /// <summary>
    /// Fügt "%" hinzu, wenn der Benutzer die Eingabe im Zins-Inputfeld beendet.
    /// </summary>
    private void OnInterestInputFieldEndEdit(string input)
    {
        if (float.TryParse(input, out float value))
        {
            interestInputFieldSlot3.text = $"{value} %";
        }
    }

    /// <summary>
    /// Fügt "Jahre" hinzu, wenn der Benutzer die Eingabe im Jahres-Inputfeld beendet.
    /// </summary>
    private void OnYearsInputFieldEndEdit(string input)
    {
        if (int.TryParse(input, out int value))
        {
            yearsInputFieldSlot3.text = $"{value} Jahre";
        }
    }

    /// <summary>
    /// Liest den Zinssatz für die Berechnung.
    /// </summary>
    private float GetInterestRate()
    {
        if (currentSlot == slots[1] && interestText != null)
        {
            // Für Slot 2 (Text mit "%")
            if (float.TryParse(interestText.text.Replace("%", "").Trim(), out float interest))
            {
                return interest / 100f;
            }
        }
        else if (currentSlot == slots[2] && interestInputFieldSlot3 != null)
        {
            // Für Slot 3 (Input mit "%")
            if (float.TryParse(interestInputFieldSlot3.text.Replace("%", "").Trim(), out float interest))
            {
                return interest / 100f;
            }
        }

        return 0f; // Standardzins: 0%
    }

    /// <summary>
    /// Holt die Jahre aus dem Text im entsprechenden Slot.
    /// </summary>
    private int GetYears()
    {
        if (currentSlot == slots[1] && yearsText != null)
        {
            // Für Slot 2 (Text mit "Jahre")
            if (int.TryParse(yearsText.text.Replace("Jahre", "").Trim(), out int years))
            {
                return years;
            }
        }
        else if (currentSlot == slots[2] && yearsInputFieldSlot3 != null)
        {
            // Für Slot 3 (Input mit "Jahre")
            if (int.TryParse(yearsInputFieldSlot3.text.Replace("Jahre", "").Trim(), out int years))
            {
                return years;
            }
        }

        return 0; // Standardjahre: 0
    }

    /// <summary>
    /// Startet die Simulation basierend auf den aktuellen Eingabewerten.
    /// </summary>
    private void StartSimulation()
    {
        if (currentSlot == null)
        {
            Debug.LogWarning("Das Summen-Element ist in keinem Slot. Die Simulation kann nicht starten.");
            return;
        }

        float initialSum = GetSumValue();
        float interestRate = GetInterestRate();
        int years = GetYears();

        if (years > 0)
        {
            StartCoroutine(SimulateYears(initialSum, interestRate, years));
        }
    }

    /// <summary>
    /// Simuliert den Zinseszins über die Jahre.
    /// </summary>
    private IEnumerator SimulateYears(float initialSum, float interestRate, int years)
    {
        float currentSum = initialSum;

        for (int year = 1; year <= years; year++)
        {
            currentSum += currentSum * interestRate; // Zinseszins berechnen
            UpdateSumDisplay(currentSum);

            if (currentYearText != null)
            {
                currentYearText.text = $"Jahr {year}";
            }

            yield return new WaitForSeconds(2f); // 2 Sekunden Pause pro Jahr
        }
    }

    /// <summary>
    /// Aktualisiert die Anzeige der Summe im UI.
    /// </summary>
    private void UpdateSumDisplay(float currentSum)
    {
        if (currentSlot != null)
        {
            TMP_Text slotText = currentSlot.GetComponentInChildren<TMP_Text>();
            if (slotText != null)
            {
                slotText.text = $"€ {currentSum:F2}";
            }

            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, currentSum);

            // Berechne den dynamischen Y-Offset basierend auf der neuen Höhe
            float dynamicYOffset = rectTransform.sizeDelta.y / 2;
            rectTransform.anchoredPosition = new Vector2(0, dynamicYOffset + (currentSlot.childCount - 1) * rectTransform.sizeDelta.y);
        }
    }
}
