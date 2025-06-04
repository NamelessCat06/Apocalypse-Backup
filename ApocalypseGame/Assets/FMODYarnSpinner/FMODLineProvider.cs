using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Text.RegularExpressions;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class LineToFmodEvent
{
    public string lineID; // Yarn-generated line ID
    public string description; // First #tag used as a short description
    public EventReference fmodEventEN; // FMOD event for English
    public EventReference fmodEventALT; // FMOD event for Alternative Language
}

public class FMODLineProvider : MonoBehaviour
{
    [Header("Yarn Script (Drag Here)")]
    public TextAsset yarnScript; // Drag & drop Yarn file

    [Header("FMOD Event Assignments")]
    [SerializeField] public List<LineToFmodEvent> lineEventMappings = new List<LineToFmodEvent>();

    private Dictionary<string, (EventReference en, EventReference alt)> lineToFmodEvents = new Dictionary<string, (EventReference, EventReference)>();
    private TextAsset lastLoadedYarnScript; // Stores the last Yarn file to detect changes

    public enum LanguageOption { EN, ALT }
    public static LanguageOption currentLanguage = LanguageOption.EN;

    private void Awake()
    {
        LoadMappings();
    }

    /// <summary>
    /// Toggles between English and Alternative language.
    /// </summary>
    public void ToggleLanguage()
    {
        currentLanguage = (currentLanguage == LanguageOption.EN) ? LanguageOption.ALT : LanguageOption.EN;
        Debug.Log($"🌍 Language switched to: {currentLanguage}");
    }

    private void LoadMappings()
    {
        lineToFmodEvents.Clear();
        foreach (var mapping in lineEventMappings)
        {
            if (!string.IsNullOrEmpty(mapping.lineID))
            {
                lineToFmodEvents[mapping.lineID] = (mapping.fmodEventEN, mapping.fmodEventALT);
                Debug.Log($"✅ Mapped Yarn Line: {mapping.lineID} → EN: {mapping.fmodEventEN}, ALT: {mapping.fmodEventALT}");
            }
        }
    }

    [ContextMenu("Update Lines")]
    public void LoadYarnLineIDs()
    {
        if (yarnScript == null)
        {
            Debug.LogWarning("⚠️ No Yarn script assigned to FMODLineProvider.");
            return;
        }

        string[] lines = yarnScript.text.Split('\n');

        // ✅ Preserve existing FMOD event assignments
        Dictionary<string, (EventReference, EventReference, string)> existingAssignments = lineEventMappings
            .Where(e => !string.IsNullOrEmpty(e.lineID))
            .ToDictionary(e => e.lineID, e => (e.fmodEventEN, e.fmodEventALT, e.description));

        lineEventMappings.Clear(); // ✅ Clear the list before adding new data
        int addedCount = 0;

        foreach (string line in lines)
        {
            // Find Yarn-generated Line IDs (`#line:xxxxx`)
            Match lineIDMatch = Regex.Match(line, @"#line:([a-fA-F0-9]+)");
            if (!lineIDMatch.Success) continue;

            string lineID = $"line:{lineIDMatch.Groups[1].Value}";

            // Find first #tag for description (e.g., `#line1` or `#greeting`)
            Match firstTagMatch = Regex.Match(line, @"#(\w+)");
            string description = firstTagMatch.Success ? firstTagMatch.Groups[1].Value : "";

            // Restore previously assigned FMOD events & descriptions if they exist
            (EventReference assignedEventEN, EventReference assignedEventALT, string previousDescription) = existingAssignments.ContainsKey(lineID)
                ? existingAssignments[lineID]
                : (new EventReference(), new EventReference(), "");

            lineEventMappings.Add(new LineToFmodEvent
            {
                lineID = lineID,
                description = !string.IsNullOrEmpty(description) ? description : previousDescription,
                fmodEventEN = assignedEventEN,
                fmodEventALT = assignedEventALT
            });

            addedCount++;
        }

        Debug.Log($"✅ Refreshed {addedCount} Yarn Line IDs from Yarn file, including descriptions.");

        // **Force Unity to refresh the Inspector immediately**
        #if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            EditorUtility.SetDirty(this);
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
            UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();    
        }
        #endif
    }

    public bool TryGetFmodEvent(string lineID, out EventReference fmodEvent)
    {
        if (lineToFmodEvents.TryGetValue(lineID, out var events))
        {
            fmodEvent = (currentLanguage == LanguageOption.EN) ? events.en : events.alt;
            return true;
        }
        Debug.LogWarning($"⚠️ No FMOD event assigned for Yarn Line ID: {lineID}");
        fmodEvent = new EventReference();
        return false;
    }
}