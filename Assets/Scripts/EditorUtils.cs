using System;
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
}
