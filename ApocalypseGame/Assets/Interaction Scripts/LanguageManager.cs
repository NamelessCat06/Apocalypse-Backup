using UnityEngine;
using TMPro;
using Yarn.Unity;

public class LanguageManager : MonoBehaviour
{
    [Header("UI Setup")]
    public TMP_Dropdown languageDropdown;
    public string[] supportedLanguages = { "en", "nl", "fr" };

    private TextLineProvider textLineProvider;
    private DialogueRunner dialogueRunner;

    private void Start()
    {
        textLineProvider = FindObjectOfType<TextLineProvider>();
        dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (textLineProvider == null)
            Debug.LogWarning("TextLineProvider not found in scene!");
        if (dialogueRunner == null)
            Debug.LogWarning("DialogueRunner not found in scene!");

        if (languageDropdown != null)
        {
            languageDropdown.onValueChanged.AddListener(OnLanguageDropdownChanged);
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (textLineProvider != null)
        {
            textLineProvider.textLanguageCode = languageCode;
            Debug.Log($"Language switched to: {languageCode}");

            if (dialogueRunner != null && dialogueRunner.IsDialogueRunning)
            {
                string currentNode = dialogueRunner.CurrentNodeName;
                dialogueRunner.Stop();
                dialogueRunner.StartDialogue(currentNode);
            }
        }
    }

    public void OnLanguageDropdownChanged(int index)
    {
        if (index >= 0 && index < supportedLanguages.Length)
        {
            SetLanguage(supportedLanguages[index]);
        }
    }
}

