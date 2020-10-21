using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public static class ExtensionsLibrary
{
    public static string[] removedWords = new string[] { "Alpha", };

    public static string AddSpacesBetweenCamelCase(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return "";
        StringBuilder newText = new StringBuilder(text.Length * 2);
        newText.Append(text[0]);
        for (int i = 1; i < text.Length; i++)
        {
            if (char.IsUpper(text[i]) && text[i - 1] != ' ')
                newText.Append(' ');
            newText.Append(text[i]);
        }
        return newText.ToString();
    }

    public static string RemoveWords(this string text)
    {
        foreach (string word in removedWords)
        {
            text = text.Replace(word, "");
        }

        return text;
    }

    public static float Map(this float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
