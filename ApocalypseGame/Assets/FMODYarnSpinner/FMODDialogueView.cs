using UnityEngine;
using Yarn.Unity;
using FMODUnity;
using FMOD.Studio;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class FMODDialogueView : DialogueViewBase
{
    [SerializeField]
    private FMODLineProvider fmodLineProvider;
    private EventInstance instance;

    private void Start()
    {
        if (fmodLineProvider == null)
        {
            fmodLineProvider = FindFirstObjectByType<FMODLineProvider>(); // ✅ New Unity-friendly method
        }
        if (fmodLineProvider == null)
        {
            Debug.LogError("❌ FMODLineProvider not found in the scene! Make sure it's added.");
        }
    }

    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        if (fmodLineProvider == null)
        {
            Debug.LogError("❌ FMODLineProvider is null! Skipping audio.");
            onDialogueLineFinished?.Invoke();
            return;
        }

        Debug.Log($"🔎 Yarn sent LineID: {dialogueLine.TextID}");

        // Stop the current event before playing a new one
        if (instance.isValid())
        {
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }

        // Retrieve the correct FMOD event based on the selected language
        if (fmodLineProvider.TryGetFmodEvent(dialogueLine.TextID, out EventReference fmodEvent))
        {
            Debug.Log($"✅ Found FMOD event: {fmodEvent} for LineID: {dialogueLine.TextID}");
            instance = RuntimeManager.CreateInstance(fmodEvent);
            instance.start();

            // Wait until FMOD event finishes before continuing Yarn dialogue
            RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
            StartCoroutine(WaitForEventCompletion(onDialogueLineFinished));
        }
        else
        {
            Debug.LogWarning($"⚠️ No FMOD event found for LineID: {dialogueLine.TextID}");
            onDialogueLineFinished?.Invoke();
        }
    }

    private System.Collections.IEnumerator WaitForEventCompletion(System.Action onDialogueLineFinished)
    {
        PLAYBACK_STATE playbackState;
        do
        {
            yield return null;
            instance.getPlaybackState(out playbackState);
        } while (playbackState != PLAYBACK_STATE.STOPPED);

        instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        instance.release();

        onDialogueLineFinished?.Invoke();
    }
}