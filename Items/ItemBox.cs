using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    public static event System.Action<UsableItem> EventGainItem = delegate { };

    [SerializeField] private ItemBoxController.Items[] possibleItems = null;
    [SerializeField] private float respawnTime = 10f;
    private float currentTime = 0f;
    private bool respawning = false;

    private ItemBoxController parent;
    private int ID;
    public void Init(ItemBoxController p, int id)
    {
        parent = p;
        ID = id;
    }

    public void UpdateFromParent()
    {
        if (respawning)
        {
            currentTime = Mathf.Min(currentTime + Time.deltaTime, respawnTime);

            if (currentTime == respawnTime)
            {
                gameObject.SetActive(true);
                respawning = false;
                currentTime = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Networked Player"))
        {

            if (!NetworkManager.instance) { GainItemFromNetwork(); }
            else {
                if (NetworkManager.instance.IsHost()) { NetworkManager.instance.SendItemBoxUsed(ID, other.gameObject); }
            }          
        }
    }

    public void GainItemFromNetwork()
    {
        int index = Random.Range(0, possibleItems.Length);
        print(parent.GetItem(possibleItems[index]));
        EventGainItem(parent.GetItem(possibleItems[index]));

        SoundManager.PlaySoundAt("Item Pickup", gameObject.transform.position);
        Use();
    }

    public void Use()
    {
        gameObject.SetActive(false);
        respawning = true;
        currentTime = 0;
    }
}
