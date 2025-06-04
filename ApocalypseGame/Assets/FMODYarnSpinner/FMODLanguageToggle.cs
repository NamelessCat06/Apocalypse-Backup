using UnityEngine;
using UnityEngine.UI;
using TMPro;  // ✅ Added for TextMeshPro support

public class FMODLanguageToggle : MonoBehaviour
{
    [SerializeField] private FMODLineProvider lineProvider;
    [SerializeField] private Button toggleButton;
    [SerializeField] private TextMeshProUGUI buttonText; // ✅ Updated to use TextMeshPro

    private void Start()
    {
        if (lineProvider == null)
        {
            lineProvider = FindFirstObjectByType<FMODLineProvider>();
        }

        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleLanguage);
        }

        UpdateButtonText();
    }

    public void ToggleLanguage()
    {
        if (lineProvider != null)
        {
            lineProvider.ToggleLanguage();
            UpdateButtonText();
        }
    }

    private void UpdateButtonText()
    {
        if (buttonText != null)
        {
            buttonText.text = (FMODLineProvider.currentLanguage == FMODLineProvider.LanguageOption.EN) ? "EN" : "ALT";
        }
        else
        {
            Debug.LogWarning("⚠️ Button text reference is missing! Assign a TextMeshProUGUI component in the Inspector.");
        }
    }
}