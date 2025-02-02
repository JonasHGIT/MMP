/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script wechselt beim Klick auf einen Button zwischen zwei Sprites.
 * 
 * Features:
 * - Einfaches Umschalten zwischen zwei Sprites.
 */
using UnityEngine;
using UnityEngine.UI;

public class SimpleSpriteToggle : MonoBehaviour
{
    [Header("Button und Image")]
    [SerializeField] private Button toggleButton;   // Der Button, auf den geklickt wird
    [SerializeField] private Image buttonImage;     // Das Image des Buttons

    [Header("Sprites")]
    [SerializeField] private Sprite spriteOn;       // Sprite für den "Ton an"-Zustand
    [SerializeField] private Sprite spriteOff;      // Sprite für den "Ton aus"-Zustand

    private bool isMuted = false;  // Der aktuelle Zustand, ob der Ton an oder aus ist

    private void Start()
    {
        // Setze den Listener für den Button
        toggleButton.onClick.AddListener(ToggleSprite);
    }

    /// <summary>
    /// Wechselt zwischen den zwei Sprites beim Klick auf den Button.
    /// </summary>
    private void ToggleSprite()
    {
        isMuted = !isMuted;  // Zustand umkehren (Ton ein/aus)
        
        // Setze das Sprite je nach Zustand
        buttonImage.sprite = isMuted ? spriteOff : spriteOn;
    }
}
