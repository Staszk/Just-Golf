using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeeManager : MonoBehaviour
{
    private static TeeManager instance;

    [SerializeField] private GameObject teePrefab = null;
    [SerializeField] private int numTees = 50;
    private static Vector3 localScale = new Vector3(0.15f, 0.15f, 0.15f);

    private List<GameObject> inactiveList;
    private List<GameObject> activeList;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        PopulateTees();
    }

    private void PopulateTees()
    {
        inactiveList = new List<GameObject>();
        activeList = new List<GameObject>();

        for (int i = 0; i < numTees; i++)
        {
            GameObject go = Instantiate(teePrefab, transform);
            go.SetActive(false);
            inactiveList.Add(go);
        }
    }

    public static void SpawnTee(Vector3 position, Transform parent)
    {
        GameObject tee;

        // available to spawn
        if (instance.inactiveList.Count > 0)
        {
            tee = instance.inactiveList[0];
            instance.inactiveList.RemoveAt(0);
        }
        else
        {
            tee = instance.activeList[0];
            instance.activeList.RemoveAt(0);
            tee.SetActive(false);
        }

        tee.transform.position = position;
        tee.transform.SetParent(parent);
        //tee.transform.localScale = localScale;
        tee.SetActive(true);
        instance.activeList.Add(tee);
        SoundManager.PlaySoundAt("Bullet Impact", tee.transform.position);
    }

    public static void ResetTee(Tee tee)
    {
        GameObject go = tee.gameObject;

        go.transform.SetParent(instance.transform);

        go.SetActive(false);

        instance.activeList.Remove(go);
        instance.inactiveList.Add(go);
    }
}
