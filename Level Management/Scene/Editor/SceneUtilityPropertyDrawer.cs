// Written by Parker Staszkiewicz
// Based on the Scene Property Drawer concept detailed by Ramy Daghstani

using UnityEditor;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

[CustomPropertyDrawer(typeof(SceneUtilities.Scene))]
public class SceneUtilityPropertyDrawer : PropertyDrawer
{
    private string[] sceneNames;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        this.sceneNames = this.GetSceneNames();
        SerializedProperty sceneNameProp = property.FindPropertyRelative("name");
        SerializedProperty sceneIndexProp = property.FindPropertyRelative("index");
        sceneIndexProp.intValue = GetIndexOf(this.sceneNames, sceneNameProp.stringValue);
        if (sceneIndexProp.intValue == -1)
        {
            sceneIndexProp.intValue = 0;
            sceneNameProp.stringValue = this.sceneNames[0];
        }
        sceneIndexProp.intValue = EditorGUI.Popup(position, label.text, sceneIndexProp.intValue, this.sceneNames);
        sceneNameProp.stringValue = this.sceneNames[sceneIndexProp.intValue];

        EditorGUI.EndProperty();
    }

    public String[] GetSceneNames()
    {
        Regex regex = new Regex(@"([^/]*/)*([\w\d\-]*)\.unity");
        EditorBuildSettingsScene[] scenesInBuild = EditorBuildSettings.scenes;
        string[] sceneNames = new string[scenesInBuild.Length];
        for (int i = 0; i < scenesInBuild.Length; i++)
        {
            sceneNames[i] = regex.Replace(scenesInBuild[i].path, "$2");
        }
        return sceneNames;
    }

    // Returns the index of parameters withing the list
    private int GetIndexOf(string[] list, string parameter)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == parameter)
                return i;
        }
        return -1;
    }
}
