using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextChangeScript : MonoBehaviour
{
    [SerializeField] private TMP_Text _title; // Assign the text object in Inspector
    [SerializeField] private Button _button; // Assign the button in Inspector

    [SerializeField] private string firstText = "Default First Text";
    [SerializeField] private string secondText = "Default Second Text";

    private bool _isSecondText = false; // Track the current state

    void Start()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(ToggleText);
        }
    }

    public void ToggleText()
    {
        _isSecondText = !_isSecondText;
        _title.text = _isSecondText ? secondText : firstText;
    }
}
