using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(Wfc))]
public class WfcEditor : Editor
{
    Wfc wfc;

    private void OnEnable()
    {
        wfc = target as Wfc;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Model");
        EditorUtils.filePicker(wfc.modelFile, $"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}", (object parameter) => {
            wfc.modelFile = parameter as string;
        });
        EditorGUILayout.EndHorizontal();
        EditorUtils.guiButton("Run", () => {
            int[,,] res = wfc.run();
            Debug.Log("aaa");
        });
    }
}
