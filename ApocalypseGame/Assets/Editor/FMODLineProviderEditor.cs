using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(FMODLineProvider))]
    public class FMODLineProviderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var fmodLine = (FMODLineProvider)target;
            
            base.OnInspectorGUI();
            
            if (GUILayout.Button("Validate"))
            {
                fmodLine.LoadYarnLineIDs();
            }
        }
    }
}