using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Geldautomat : MonoBehaviour
{
    // Referenzen für Karteninteraktionen
    public GameObject bankCard;
    public Transform cardSlot;
    public TMP_InputField pinInputField;
    public TextMeshProUGUI screenText; // Wird für alle dynamischen Texte verwendet
    public TextMeshProUGUI uiBalanceDisplay; // Kontostand-Anzeige
    public TextMeshProUGUI collectedCashDisplay; // Anzeige für gesammeltes Bargeld

    public GameObject flaeche1;
    public GameObject flaeche2a;
    public GameObject flaeche2b;
    public GameObject flaeche2c;
    public GameObject flaeche3; // Neue Fläche für die Animation und Info nach Transaktion

    public Transform cashSpawnPoint; // Punkt, an dem Bargeld erscheinen soll
    public GameObject cashPrefab; // Prefab für Bargeld

    private bool isCardInserted = false;
    private bool isPinEntered = false;
    private int accountBalance = 1000; // Startbetrag
    private int focus = 0;
    private int collectedCash = 100; // Startwert für gesammeltes Bargeld

    private Vector3 cardStartPosition;

    void Start()
    {
        cardStartPosition = bankCard.transform.position;
        UpdateBalanceDisplay();
        UpdateCollectedCashDisplay();
    }

    void Update()
    {
        if (pinInputField.isFocused)
        {
            focus = 1;
        }

        if (focus == 1 && Input.GetKeyDown(KeyCode.Return))
        {
            OnPinEntered();
        }

        if (!isCardInserted)
        {
            CheckCardInsertion();
        }
    }

    private void CheckCardInsertion()
    {
        if (Vector3.Distance(bankCard.transform.position, cardSlot.position) < 0.1f)
        {
            // Wenn Karte im Slot ist, starten wir die Animation
            StartCoroutine(AnimateCardInsertion());
        }
    }

    public void OnCardDragStart()
    {
        // Karte wird gezogen
    }

    public void OnCardDragEnd()
    {
        if (Vector3.Distance(bankCard.transform.position, cardSlot.position) < 0.1f)
        {
            isCardInserted = true;
            bankCard.SetActive(false);
            screenText.text = "Bitte PIN eingeben.";
        }
        else
        {
            bankCard.transform.position = cardStartPosition;
        }
    }

    // Coroutine für die Animation, dass die Karte eingezogen wird
    private IEnumerator AnimateCardInsertion()
    {
        Vector3 startPosition = bankCard.transform.position;
        Vector3 endPosition = cardSlot.position;
        float duration = 1.5f; // Dauer der Animation
        float elapsedTime = 0f;

        // Solange wir noch nicht das Ende der Animation erreicht haben
        while (elapsedTime < duration)
        {
            // Lerp für sanfte Bewegung von Startposition zu Zielposition
            bankCard.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Warten bis zum nächsten Frame
        }

        // Sicherstellen, dass die Karte exakt an der Zielposition landet
        bankCard.transform.position = endPosition;

        // Karte ausblenden und Text aktualisieren
        bankCard.SetActive(false);
        screenText.text = "Bitte PIN eingeben.";
        isCardInserted = true;
    }

    public void OnPinEntered()
    {
        string enteredPin = pinInputField.text.Trim();
        if (enteredPin == "1234")
        {
            isPinEntered = true;
            screenText.text = "Bitte wählen Sie eine Funktion.";
            flaeche1.SetActive(true);
        }
        else
        {
            screenText.text = "Falsche PIN. Versuchen Sie es erneut.";
        }
        pinInputField.text = "";
    }

    // Funktion für Fläche 2a: Kontostand anzeigen
    public void ShowBalance()
    {
        SetActiveFlaeche(flaeche2a);
        screenText.text = "Aktueller Kontostand beträgt:";
        UpdateBalanceDisplay();
    }

    // Funktion für Fläche 2b: Geld abheben
    public void ShowWithdrawOptions()
    {
        SetActiveFlaeche(flaeche2b);
        screenText.text = "Wähle den Betrag, den du abheben möchtest:";
    }

    // Funktion für Fläche 2c: Geld einzahlen
    public void ShowDepositOptions()
    {
        SetActiveFlaeche(flaeche2c);
        screenText.text = "Wähle den Betrag, den du einzahlen möchtest:";
    }

    // Methode für Zurück-Buttons
    public void BackToMain()
    {
        SetActiveFlaeche(flaeche1);
        screenText.text = "Bitte wählen Sie eine Funktion.";
    }

    public void Withdraw(int amount)
    {
        if (accountBalance >= amount)
        {
            accountBalance -= amount;
            SetActiveFlaeche(flaeche3);
            screenText.text = $"Du hast {amount}€ abgehoben.";
            AnimateCash(amount); // Bargeldanimation starten
        }
        else
        {
            screenText.text = "Nicht genügend Guthaben.";
        }
        UpdateBalanceDisplay();
    }

    public void Deposit(int amount)
    {
        if (collectedCash >= amount)
        {
            accountBalance += amount;
            SetActiveFlaeche(flaeche3);
            screenText.text = $"Du hast {amount}€ eingezahlt.";
            collectedCash -= amount;
        }
        else
        {
            screenText.text = "Nicht genügend Bargeld.";
        }
        UpdateBalanceDisplay();
        UpdateCollectedCashDisplay();
    }

    private void AnimateCash(int amount)
    {
        // Instantiate Cash Prefab
        GameObject cash = Instantiate(cashPrefab, cashSpawnPoint.position, Quaternion.identity);
        cash.transform.SetParent(cashSpawnPoint); // Sicherstellen, dass es an cashSpawnPoint bleibt
        cash.transform.localScale = Vector3.one; // Resetten der Skalierung, falls abweichend

        // Sicherstellen, dass es sichtbar ist (Sorting Layer für UI oder Welt-Layer-Anpassungen)
        SpriteRenderer renderer = cash.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.sortingLayerName = "UI"; // Anpassen an deinen Sorting Layer
            renderer.sortingOrder = 10; // Sicherstellen, dass es über anderen Objekten liegt
        }

        // Klicken für das Sammeln des Bargeldes
        cash.AddComponent<Button>().onClick.AddListener(() => CollectCash(cash, amount));

        // Animation: Bargeld von oben nach unten bewegen
        StartCoroutine(MoveCashDownwards(cash));
    }

    private void CollectCash(GameObject cash, int amount)
    {
        // Wenn das Bargeld angeklickt wird, sammeln wir es
        collectedCash += amount;
        UpdateCollectedCashDisplay();

        // Fading-Effekt
        StartCoroutine(FadeOutAndDestroyCash(cash));
    }

    private IEnumerator MoveCashDownwards(GameObject cash)
    {
        float duration = 1.5f;
        Vector3 startPosition = cash.transform.position;
        Vector3 endPosition = startPosition + Vector3.down * 1.0f;

        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            cash.transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cash.transform.position = endPosition;
    }

    private IEnumerator FadeOutAndDestroyCash(GameObject cash)
    {
        // Fading-Effekt über eine Dauer von 1 Sekunde
        SpriteRenderer renderer = cash.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color startColor = renderer.color;
            float fadeDuration = 0f; // Setze die Dauer für das Fading (erstmal 0 weil funktioniert nicht ganz)
            float elapsedTime = 0;

            while (elapsedTime < fadeDuration)
            {
                renderer.color = Color.Lerp(startColor, new Color(startColor.r, startColor.g, startColor.b, 0), elapsedTime / fadeDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Zerstören des Bargeldes nach dem Ausblenden
            Destroy(cash);
        }
    }

    private void UpdateScreenTextAfterAnimation()
    {
        screenText.text += "\nBitte entnehmen Sie das Bargeld dem Ausgabefach.";
    }

    // Kontostand-Anzeige aktualisieren
    private void UpdateBalanceDisplay()
    {
        if (uiBalanceDisplay != null)
        {
            uiBalanceDisplay.text = $"{accountBalance}€";
        }
    }

    private void UpdateCollectedCashDisplay()
    {
        if (collectedCashDisplay != null)
        {
            collectedCashDisplay.text = $"Bargeld: {collectedCash}€";
        }
    }

    // Aktive Fläche ändern
    private void SetActiveFlaeche(GameObject activeFlaeche)
    {
        // Alle Flächen deaktivieren
        flaeche1.SetActive(false);
        flaeche2a.SetActive(false);
        flaeche2b.SetActive(false);
        flaeche2c.SetActive(false);
        flaeche3.SetActive(false);

        // Gewählte Fläche aktivieren
        activeFlaeche.SetActive(true);
    }

    public void ReturnCard()
    {
        isCardInserted = false;
        isPinEntered = false;
        bankCard.transform.position = cardStartPosition; // Rücksetzen der Position
        bankCard.transform.rotation = Quaternion.identity; // Rücksetzen der Rotation

        bankCard.SetActive(true);
        screenText.text = "Bitte Karte einführen.";
    }
}
