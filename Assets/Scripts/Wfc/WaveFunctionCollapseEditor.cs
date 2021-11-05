using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomEditor(typeof(WaveFunctionCollapse))]
public class WaveFunctionCollapseEditor : Editor
{
    WaveFunctionCollapse wfc;

    private void OnEnable()
    {
        wfc = target as WaveFunctionCollapse;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        
        EditorUtils.filePicker("Model" ,wfc.modelFile, $"{Application.dataPath}/{ModelSampler.SAVE_FOLDER}", (object parameter) => {
            wfc.modelFile = parameter as string;
        });

        EditorUtils.guiButton("Run", () => {
            wfc.run();
        });


        EditorGUILayout.LabelField("Debug Tools", EditorStyles.boldLabel);
        
        EditorUtils.guiButton("Initialize", () => {
            wfc.initialize();
        });
        EditorUtils.guiButton("Constrain At Cursor", () => {
            wfc.solver.constrainVariable(wfc.variables, wfc.outputGrid.cursorPosition, wfc.model.rules);
        });
        EditorUtils.guiButton("Lock Tile At Cursor", () => {
            wfc.variables.at(wfc.outputGrid.cursorPosition).lockValue();
        });
        EditorUtils.guiButton("Populate Grid", () => {
            wfc.populateGrid();
        }) ;
    }
}
