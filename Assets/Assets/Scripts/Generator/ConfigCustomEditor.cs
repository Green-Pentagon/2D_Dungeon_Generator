using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomEditor(typeof(Configure))]
public class ConfigCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Generate"))
        {
            target.GetComponent<Configure>().Exec();
        }
        DrawDefaultInspector();
    }
}
