/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script steuert einen Geldautomaten (ATM) in einem UI, das Interaktionen wie das Einführen einer Karte, das Eingeben eines PIN-Codes und das Abheben sowie Einzahlen von Geld ermöglicht.
 * Es enthält Animationen für das Einführen der Karte und das Erscheinen von Bargeld sowie Interaktionen, bei denen der Benutzer Bargeld sammeln kann.
 * 
 * Features:
 * - Karte einführen und PIN eingeben
 * - Kontostand anzeigen und aktualisieren
 * - Geld abheben und einzahlen
 * - Möglichkeit, Bargeld zu sammeln
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Geldautomat : MonoBehaviour
{
    // Referenzen für Karteninteraktionen
    public GameObject bankCard;  // Referenz auf die Bankkarte
    public Transform cardSlot;  // Der Slot, in den die Karte eingelegt wird
    public TMP_InputField pinInputField;  // Eingabefeld für den PIN
    public TextMeshProUGUI screenText;  // Wird für alle dynamischen Texte verwendet
    public TextMeshProUGUI uiBalanceDisplay;  // Kontostand-Anzeige
    public TextMeshProUGUI collectedCashDisplay;  // Anzeige für gesammeltes Bargeld

    public GameObject flaeche1;  // Bereich für die Funktionserwahl
    public GameObject flaeche2a;  // Bereich für die Kontostand-Anzeige
    public GameObject flaeche2b;  // Bereich für Abhebungsoptionen
    public GameObject flaeche2c;  // Bereich für Einzahlungsmöglichkeiten
    public GameObject flaeche3;  // Neue Fläche für Animation und Info nach Transaktion

    public Transform cashSpawnPoint;  // Punkt, an dem Bargeld erscheinen soll
    public GameObject cashPrefab;  // Prefab für Bargeld

    private bool isCardInserted = false;  // Status, ob die Karte eingeführt wurde
    private bool isPinEntered = false;  // Status, ob der PIN korrekt eingegeben wurde
    private int accountBalance = 1000;  // Startbetrag des Kontos
    private int focus = 0;  // Fokus-Status für die PIN-Eingabe
    private int collectedCash = 100;  // Startwert für gesammeltes Bargeld

    private Vector3 cardStartPosition;  // Die Ausgangsposition der Bankkarte

    void Start()
    {
        cardStartPosition = bankCard.transform.position;  // Speichert die Startposition der Bankkarte
        UpdateBalanceDisplay();  // Aktualisiert die Anzeige des Kontostands
        UpdateCollectedCashDisplay();  // Aktualisiert die Anzeige des gesammelten Bargelds
    }

    void Update()
    {
        if (pinInputField.isFocused)  // Überprüft, ob das PIN-Eingabefeld fokussiert ist
        {
            focus = 1;
        }

        if (focus == 1 && Input.GetKeyDown(KeyCode.Return))  // Wenn die Eingabetaste gedrückt wird, PIN eingeben
        {
            OnPinEntered();
        }

        if (!isCardInserted)  // Wenn die Karte noch nicht eingeführt wurde
        {
            CheckCardInsertion();
        }
    }

    // Überprüft, ob die Karte richtig im Kartenschlitz platziert wurde
    private void CheckCardInsertion()
    {
        if (Vector3.Distance(bankCard.transform.position, cardSlot.position) < 0.1f)
        {
            // Karte ist im Slot, wir machen die Karte unsichtbar und setzen sie als eingeführt
            bankCard.SetActive(false);
            screenText.text = "Bitte PIN eingeben.";  // Text ändern
            isCardInserted = true;
        }
    }

    // Wird aufgerufen, wenn die Karte zu ziehen beginnt
    public void OnCardDragStart()
    {
        // Karte wird gezogen
    }

    // Wird aufgerufen, wenn das Ziehen der Karte endet
    public void OnCardDragEnd()
    {
        if (Vector3.Distance(bankCard.transform.position, cardSlot.position) < 0.1f)
        {
            isCardInserted = true;
            bankCard.SetActive(false);  // Karte wird ausgeblendet
            screenText.text = "Bitte PIN eingeben.";  // Text ändern
        }
        else
        {
            bankCard.transform.position = cardStartPosition;  // Karte zurücksetzen, wenn sie nicht richtig platziert wurde
        }
    }

    // Wird aufgerufen, wenn der PIN eingegeben wurde
    public void OnPinEntered()
    {
        string enteredPin = pinInputField.text.Trim();
        if (enteredPin == "1234")
        {
            isPinEntered = true;
            screenText.text = "Bitte wählen Sie eine Funktion.";
            flaeche1.SetActive(true);  // Zeige die Funktionenauswahl
        }
        else
        {
            screenText.text = "Falsche PIN. Versuchen Sie es erneut.";
        }
        pinInputField.text = "";  // Leere das Eingabefeld
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

    // Abhebung von Geld
    public void Withdraw(int amount)
    {
        if (accountBalance >= amount)
        {
            accountBalance -= amount;
            SetActiveFlaeche(flaeche3);
            screenText.text = $"Du hast {amount}€ abgehoben.";
            SpawnCash(amount);  // Erstelle Bargeld ohne Animation
        }
        else
        {
            screenText.text = "Nicht genügend Guthaben.";
        }
        UpdateBalanceDisplay();
    }

    // Einzahlung von Geld
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

    // Erstelle Bargeld ohne Animation
    private void SpawnCash(int amount)
    {
        GameObject cash = Instantiate(cashPrefab, cashSpawnPoint.position, Quaternion.identity);  // Erstelle Bargeld-Objekt
        cash.transform.SetParent(cashSpawnPoint);  // Setze das Bargeld an den Spawnpunkt
        cash.transform.localScale = Vector3.one;

        // Klicken für das Sammeln des Bargeldes
        cash.AddComponent<Button>().onClick.AddListener(() => CollectCash(cash, amount));
    }

    // Methode zum Sammeln von Bargeld
    private void CollectCash(GameObject cash, int amount)
    {
        collectedCash += amount;
        UpdateCollectedCashDisplay();  // Update die gesammelte Bargeldanzeige

        // Zerstöre das Bargeldobjekt direkt
        Destroy(cash);
    }

    // Kontostand-Anzeige aktualisieren
    private void UpdateBalanceDisplay()
    {
        if (uiBalanceDisplay != null)
        {
            uiBalanceDisplay.text = $"{accountBalance}€";
        }
    }

    // Anzeige des gesammelten Bargelds aktualisieren
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

    // Karte zurückgeben
    public void ReturnCard()
    {
        isCardInserted = false;
        isPinEntered = false;
        bankCard.transform.position = cardStartPosition;  // Rücksetzen der Position
        bankCard.transform.rotation = Quaternion.identity;  // Rücksetzen der Rotation

        bankCard.SetActive(true);
        screenText.text = "Bitte Karte einführen.";
    }
}
