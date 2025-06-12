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
    private Coroutine currentWaitCoroutine;

    // Track currently active line and its callback
    private LocalizedLine activeLine;
    private System.Action activeCallback;

    private void Start()
    {
        if (fmodLineProvider == null)
        {
            fmodLineProvider = FindFirstObjectByType<FMODLineProvider>();
        }

        if (fmodLineProvider == null)
        {
            Debug.LogError("❌ FMODLineProvider not found in the scene!");
        }
    }

    public override void RunLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        Debug.Log($"▶️ FMOD RunLine called for: {dialogueLine.TextID}");

        activeLine = dialogueLine;
        activeCallback = onDialogueLineFinished;

        // Stop previous audio if still playing
        if (instance.isValid())
        {
            instance.stop(STOP_MODE.ALLOWFADEOUT);
            instance.release();
        }

        if (currentWaitCoroutine != null)
        {
            StopCoroutine(currentWaitCoroutine);
            currentWaitCoroutine = null;
        }

        // Play the correct FMOD event
        if (fmodLineProvider.TryGetFmodEvent(dialogueLine.TextID, out EventReference fmodEvent))
        {
            Debug.Log($"✅ Found FMOD event: {fmodEvent} for LineID: {dialogueLine.TextID}");

            instance = RuntimeManager.CreateInstance(fmodEvent);
            RuntimeManager.AttachInstanceToGameObject(instance, transform, GetComponent<Rigidbody>());
            instance.start();

            Debug.Log("⏳ Waiting for FMOD to finish...");
            currentWaitCoroutine = StartCoroutine(WaitForEventCompletion());
        }
        else
        {
            Debug.LogWarning($"⚠️ No FMOD event found for LineID: {dialogueLine.TextID}");
            FinishLine();
        }
    }

    public override void InterruptLine(LocalizedLine dialogueLine, System.Action onDialogueLineFinished)
    {
        Debug.Log("⏹️ FMOD InterruptLine called");

        // Stop audio immediately
        if (instance.isValid())
        {
            instance.stop(STOP_MODE.IMMEDIATE);
            instance.release();
        }

        if (currentWaitCoroutine != null)
        {
            StopCoroutine(currentWaitCoroutine);
            currentWaitCoroutine = null;
        }

        FinishLine(); // Cleanly finalize the dialogue line
    }

    private System.Collections.IEnumerator WaitForEventCompletion()
    {
        PLAYBACK_STATE playbackState;
        do
        {
            yield return null;
            instance.getPlaybackState(out playbackState);
        } while (playbackState != PLAYBACK_STATE.STOPPED);

        Debug.Log("✅ FMOD event finished");
        //FinishLine();
    }

    private void FinishLine()
    {
        if (instance.isValid())
        {
            instance.stop(STOP_MODE.IMMEDIATE);
            instance.release();
        }

        if (activeCallback != null)
        {
            Debug.Log("🎯 FMOD onDialogueLineFinished invoked");
            activeCallback.Invoke();
            activeCallback = null;
        }

        activeLine = null;
        currentWaitCoroutine = null;
    }
}
