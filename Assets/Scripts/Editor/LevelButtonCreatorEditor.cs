using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelButtonCreator))]
public class LevelButtonCreatorEditor : Editor {
    public override void OnInspectorGUI() {
        base.DrawDefaultInspector();
        if (GUILayout.Button("Create Level Buttons")){
            (target as LevelButtonCreator).CreateLevelButtons();
        }
    }
}