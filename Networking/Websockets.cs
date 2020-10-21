using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class Websockets : MonoBehaviour
{
    public string url = "https://docs.google.com/spreadsheets/d/1x89mjAYoL0AXbRpiJczTHqq1g7yHQczOvRrPK4_9P9U/edit#gid=0&range=A1";
    string[] results;
    private void Start()
    {

         StartCoroutine(Yeet());
    }
    
    IEnumerator Yeet()
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            results = www.downloadHandler.text.Split('\n');
            print(Regex.Match(results[2], ".+?(?=\")"));
        }
    }

}
