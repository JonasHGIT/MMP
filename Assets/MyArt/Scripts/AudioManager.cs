/*
 * Autor: Jonas Hammer
 * Last Edited: 02.02.2025
 * 
 * Beschreibung:
 * Dieses Script verwaltet die Wiedergabe von verschiedenen Audioeffekten in einer Benutzeroberfläche. 
 * Es ermöglicht das Abspielen von Hintergrundmusik, Button-Sounds und spezifischen Text-Lese-Sounds, 
 * wenn der Benutzer mit Buttons oder Texten interagiert.
 * 
 * Features:
 * - Hintergrundmusik und allgemeine Soundeffekte
 * - Zuordnung von Sounds zu Buttons und Texten
 * - Steuerung der Lautstärke (Stumm- bzw. Mute-Funktion)
 * - Interaktive Sound-Wiedergabe für Text und Buttons
 * - Stopp aller Sounds bei neuem Klick
 */
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AudioManager : MonoBehaviour
{
    // Hintergrundmusik
    [Header("Hintergrundmusik")]
    [SerializeField] private AudioSource backgroundMusic;

    // Allgemeiner Button-Sound
    [Header("Allgemeine Button-Sounds")]
    [SerializeField] private AudioSource generalButtonSound;
    [SerializeField] private Button[] generalButtons;

    // Button-Text Sounds
    [Header("Button-Text Sounds")]
    [SerializeField] private AudioSource[] buttonTextReadingSounds;
    [SerializeField] private Button[] buttonTexts;  // Zurück zu "buttonTexts"

    // TMP-Text Sounds
    [Header("TMP-Text Sounds")]
    [SerializeField] private AudioSource[] textReadingSounds;
    [SerializeField] private TMP_Text[] normalTMPTexts;

    private bool isMuted = false;

    private void Start()
    {
        AssignButtonSounds();
        AssignTMPTextSounds();
        AssignGeneralButtonSounds();
    }

    /// <summary>
    /// Schaltet die Audio-Wiedergabe ein oder aus.
    /// </summary>
    public void ToggleAudio()
    {
        isMuted = !isMuted;
        SetAudioState(isMuted);
        Debug.Log("Audio ON/OFF gestellt.");
    }

    /// <summary>
    /// Setzt den Zustand der Audioquellen (stumm oder nicht).
    /// </summary>
    private void SetAudioState(bool mute)
    {
        if (backgroundMusic != null)
            backgroundMusic.mute = mute;

        foreach (var sound in buttonTextReadingSounds)
            sound.mute = mute;

        foreach (var sound in textReadingSounds)
            sound.mute = mute;

        if (generalButtonSound != null)
            generalButtonSound.mute = mute;
    }

    /// <summary>
    /// Weist jedem Button eine entsprechende Sound-Funktion zu.
    /// </summary>
    private void AssignButtonSounds()
    {
        for (int i = 0; i < buttonTexts.Length; i++)  // Hier wieder auf "buttonTexts" geändert
        {
            int index = i;
            buttonTexts[i].onClick.AddListener(() => PlayButtonTextReadingSound(index));
        }
    }

    /// <summary>
    /// Weist jedem TMP-Text eine Sound-Funktion zu.
    /// </summary>
    private void AssignTMPTextSounds()
    {
        for (int i = 0; i < normalTMPTexts.Length; i++)
        {
            int index = i;
            normalTMPTexts[i].gameObject.AddComponent<TextSoundTrigger>().Initialize(index, PlayTMPTextReadingSound);
        }
    }

    /// <summary>
    /// Weist allgemeinen Buttons eine Sound-Funktion zu.
    /// </summary>
    private void AssignGeneralButtonSounds()
    {
        foreach (Button button in generalButtons)
        {
            button.onClick.AddListener(PlayGeneralButtonSound);
        }
    }

    /// <summary>
    /// Stoppt alle derzeit abgespielten Sounds.
    /// </summary>
    private void StopAllAudio()
    {
        foreach (var sound in buttonTextReadingSounds)
        {
            if (sound.isPlaying)
                sound.Stop();
        }

        foreach (var sound in textReadingSounds)
        {
            if (sound.isPlaying)
                sound.Stop();
        }

        if (generalButtonSound.isPlaying)
            generalButtonSound.Stop();
    }

    /// <summary>
    /// Spielt den Sound für einen bestimmten Button-Text ab.
    /// </summary>
    /// <param name="index">Index des Buttons</param>
    public void PlayButtonTextReadingSound(int index)
    {
        if (index >= 0 && index < buttonTextReadingSounds.Length)
        {
            StopAllAudio();
            buttonTextReadingSounds[index].Play();
            Debug.Log($"Button-Text-Sound abgespielt: {index}");
        }
        else
        {
            Debug.LogWarning($"Kein Sound für diesen Button-Index ({index}) vorhanden.");
        }
    }

    /// <summary>
    /// Spielt den allgemeinen Button-Sound ab.
    /// </summary>
    public void PlayGeneralButtonSound()
    {
        StopAllAudio();
        generalButtonSound.Play();
        Debug.Log("Allgemeiner Button-Sound abgespielt.");
    }

    /// <summary>
    /// Spielt den Sound für einen bestimmten TMP-Text ab.
    /// </summary>
    /// <param name="index">Index des TMP-Textes</param>
    public void PlayTMPTextReadingSound(int index)
    {
        if (index >= 0 && index < textReadingSounds.Length)
        {
            StopAllAudio();
            textReadingSounds[index].Play();
            Debug.Log($"TMP-Text-Sound abgespielt: {index}");
        }
        else
        {
            Debug.LogWarning($"Kein Sound für diesen TMP-Text-Index ({index}) vorhanden.");
        }
    }
}

/// <summary>
/// Klasse zur Verarbeitung von Klickereignissen auf TMP-Texten.
/// </summary>
public class TextSoundTrigger : MonoBehaviour, IPointerClickHandler
{
    private int index;
    private System.Action<int> playSoundAction;

    /// <summary>
    /// Initialisiert den Trigger mit dem zugehörigen Sound-Index.
    /// </summary>
    public void Initialize(int index, System.Action<int> playSoundAction)
    {
        this.index = index;
        this.playSoundAction = playSoundAction;
    }

    /// <summary>
    /// Reagiert auf Klicks und spielt den zugeordneten Sound ab.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        playSoundAction?.Invoke(index);
    }
}
