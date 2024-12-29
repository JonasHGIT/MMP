using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;  // TextMeshPro Namespace hinzufügen

public class AudioManager : MonoBehaviour
{
    // Hintergrundmusik
    public AudioSource backgroundMusic;

    // Allgemeiner Button-Sound
    public AudioSource generalButtonSound;
    public Button[] generalButtons;  // Buttons, die den allgemeinen Sound verwenden

    // Buttontext-bezogene Audioquellen
    public AudioSource[] buttonTextReadingSounds;
    public Button[] buttonTexts;

    // TMP-Text-bezogene Audioquellen
    public AudioSource[] textReadingSounds;
    public TMP_Text[] normalTMPTexts;  // TMP-Text-Elemente

    private bool isMuted = false;

    private void Start()
    {
        AssignButtonSounds();
        AssignTMPTextSounds();
        AssignGeneralButtonSounds();
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;
        SetAudioState(isMuted);
        Debug.Log("Audio ON/OFF gestellt.");
    }

    private void SetAudioState(bool mute)
    {
        if (backgroundMusic != null)
        {
            backgroundMusic.mute = mute;
        }

        foreach (AudioSource buttonSound in buttonTextReadingSounds)
        {
            buttonSound.mute = mute;
        }

        foreach (AudioSource textSound in textReadingSounds)
        {
            textSound.mute = mute;
        }

        if (generalButtonSound != null)
        {
            generalButtonSound.mute = mute;
        }
    }

    private void AssignButtonSounds()
    {
        for (int i = 0; i < buttonTexts.Length; i++)
        {
            int index = i;
            buttonTexts[i].onClick.AddListener(() => PlayButtonTextReadingSound(index));
        }
    }

    private void AssignTMPTextSounds()
    {
        for (int i = 0; i < normalTMPTexts.Length; i++)
        {
            int index = i;
            normalTMPTexts[i].gameObject.AddComponent<TextSoundTrigger>().Initialize(index, PlayTMPTextReadingSound);
        }
    }

    // Zuordnung der allgemeinen Buttons
    private void AssignGeneralButtonSounds()
    {
        foreach (Button button in generalButtons)
        {
            button.onClick.AddListener(PlayGeneralButtonSound);
        }
    }

    // Spielt den Sound für Buttons mit Neustart bei erneutem Klicken
    public void PlayButtonTextReadingSound(int index)
    {
        if (index >= 0 && index < buttonTextReadingSounds.Length)
        {
            if (buttonTextReadingSounds[index].isPlaying)
            {
                buttonTextReadingSounds[index].Stop();
            }
            buttonTextReadingSounds[index].Play();
            Debug.Log("Button-Text-Sound abgespielt: " + index);
        }
        else
        {
            Debug.LogWarning("Kein Sound für diesen Button-Index vorhanden.");
        }
    }

    // Spielt den allgemeinen Button-Sound
    public void PlayGeneralButtonSound()
    {
        if (generalButtonSound.isPlaying)
        {
            generalButtonSound.Stop();
        }
        generalButtonSound.Play();
        Debug.Log("Allgemeiner Button-Sound abgespielt.");
    }

    // Spielt den Sound für normalen TMP-Text mit Neustart bei erneutem Klicken
    public void PlayTMPTextReadingSound(int index)
    {
        if (index >= 0 && index < textReadingSounds.Length)
        {
            if (textReadingSounds[index].isPlaying)
            {
                textReadingSounds[index].Stop();
            }
            textReadingSounds[index].Play();
            Debug.Log("Normaler TMP-Text-Sound abgespielt: " + index);
        }
        else
        {
            Debug.LogWarning("Kein Sound für diesen TMP-Text-Index vorhanden.");
        }
    }
}

public class TextSoundTrigger : MonoBehaviour, IPointerClickHandler
{
    private int index;
    private System.Action<int> playSoundAction;

    public void Initialize(int index, System.Action<int> playSoundAction)
    {
        this.index = index;
        this.playSoundAction = playSoundAction;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        playSoundAction?.Invoke(index);
    }
}
