using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using FMOD.Studio;

public class YarnFmodTrigger : MonoBehaviour
{
    private EventInstance currentEventInstance;

    private void Start()
    {
        // Register custom command in the DialogueRunner
        var dialogueRunner = FindFirstObjectByType<DialogueRunner>(); // ✅ Updated for Unity 2023+
        if (dialogueRunner != null)
        {
            dialogueRunner.AddCommandHandler<string>("PlayFmodEvent", PlayFmodEvent);
            dialogueRunner.AddCommandHandler<string, string, float>("SetFmodParameter", SetFmodParameter);
        }
        else
        {
            Debug.LogError("❌ DialogueRunner not found! Make sure it's in the scene.");
        }
    }

    // 🎵 **Command to Play an FMOD Event**
    public void PlayFmodEvent(string eventPath)
    {
        Debug.Log($"🔊 Playing FMOD Event: {eventPath}");

        // Create an instance of the event
        currentEventInstance = RuntimeManager.CreateInstance(eventPath);
        currentEventInstance.start();
    }

    // 🎚 **Command to Change an FMOD Parameter**
    public void SetFmodParameter(string eventPath, string parameterName, float value)
    {
        Debug.Log($"🎛 Changing FMOD Parameter: {parameterName} → {value} on {eventPath}");

        if (currentEventInstance.isValid())
        {
            currentEventInstance.setParameterByName(parameterName, value);
        }
        else
        {
            Debug.LogWarning("⚠️ No valid FMOD event instance found!");
        }
    }
}