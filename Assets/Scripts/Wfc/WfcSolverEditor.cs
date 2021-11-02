using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(WfcSolver))]
public class WfcSolverEditor : Editor
{
    WfcSolver solver;

    private void OnEnable()
    {
        solver = target as WfcSolver;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorUtils.filePicker(solver.modelFile, $"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}", (object parameter) => {
            solver.modelFile = parameter as string;
        });
    }
}
