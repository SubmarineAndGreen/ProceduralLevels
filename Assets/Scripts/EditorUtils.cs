using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorUtils
{
    public static void guiButton(string label, Action callback)
    {
        if (GUILayout.Button(label))
        {
            callback();
        }
    }

    public static void filePicker(string label, string path, GenericMenu.MenuFunction2 itemClickCallback)
    {

        if (EditorGUILayout.DropdownButton(new GUIContent(label), FocusType.Passive))
        {
            GenericMenu gridPickerMenu = new GenericMenu();
            var files = getFilesAtPath(path);
            if (files != null)
            {
                foreach (string item in files)
                {
                    gridPickerMenu.AddItem(new GUIContent(item), false, itemClickCallback, item);
                }
            }

            gridPickerMenu.ShowAsContext();
        }
    }

    private static List<string> getFilesAtPath(string path)
    {
        try
        {
            List<string> fullPaths = Directory.GetFiles(path)
            .Where(
                fileName => !fileName.EndsWith(".meta")
            ).ToList<string>();

            List<string> fileNames = new List<string>();
            fullPaths.ForEach(fullPath => fileNames.Add(Path.GetFileName(fullPath)));
            return fileNames;
        }
        catch
        {
            Debug.LogError("Trouble getting files for picker: invalid directory");
            return null;
        }

    }
}
