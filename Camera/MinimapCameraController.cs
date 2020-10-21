using UnityEngine;
using System.Collections;

public class MinimapCameraController : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    private Vector3 cameraOffset = Vector3.up * 50f;
    private Vector3 normalRot = new Vector3(90, 0, 0);

    [SerializeField] private GameObject[] redDots = null;
    private Color zeroAlpha = new Color(1, 1, 1, 0);
    private Renderer[] materialRenderers = null;
    private Vector3 dotOffset = Vector3.up * 25f;
    private Coroutine[] dotCoroutines;

    private int clientID = 0;
    private bool isUsingRange = false;

    private void Start()
    {
        if (NetworkManager.instance)
            clientID = NetworkManager.instance.GetId();

        dotCoroutines = new Coroutine[4];
        materialRenderers = new Renderer[redDots.Length];

        for (int i = 0; i < redDots.Length; i++)
        {
            materialRenderers[i] = redDots[i].GetComponent<Renderer>();
        }
    }

    private void OnEnable()
    {
        // Add Listeners

    }

    private void OnDisable()
    {
        // Remove Listeners
    }

    private void LateUpdate()
    {
        transform.position = target.position + cameraOffset;

        if (ClientSettings.RotateMinimap)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90, target.rotation.eulerAngles.y, 0));
        }
        else
        {
            transform.rotation = Quaternion.Euler(normalRot);
        }
    }

    private void PlaceDot(NetworkedPlayerFireMessage eventMessage)
    {
        PlaceDot(eventMessage.id, eventMessage.pos);
    }

    private void PlaceDot(int id, Vector3 position)
    {
        if (id == clientID || isUsingRange)
            return;

        redDots[id].transform.position = position + dotOffset;

        if (dotCoroutines[id] != null)
        {
            StopCoroutine(dotCoroutines[id]);
        }

        dotCoroutines[id] = StartCoroutine(FadeDot(id));
    }

    private void RangeFinderEvent(float time)
    {
        StartCoroutine(RangeFinderDot(time));
    }

    private IEnumerator RangeFinderDot(float time)
    {
        isUsingRange = true;
        float timer = 0;

        int num = NetworkManager.instance ? NetworkManager.instance.NumOfConnectedPlayers : 1;

        while(timer < time)
        {
            for (int i = 0; i < num; i++)
            {
                if (i != clientID)
                {
                    materialRenderers[i].material.color = Color.white;
                    redDots[i].transform.position = NetworkManager.instance.GetNetworkPlayerPosition(i) + dotOffset;
                }               
            }

            timer += Time.deltaTime;
            yield return null;
        }
        

        for (int i = 0; i < num; i++)
        {
            if (i == clientID)
                continue;

            if (dotCoroutines[i] != null)
            {
                StopCoroutine(dotCoroutines[i]);
            }

            dotCoroutines[i] = StartCoroutine(FadeDot(i));
        }
        isUsingRange = false;
    }
      

    private IEnumerator FadeDot(int id, float maxTime = 1.0f)
    {
        materialRenderers[id].material.color = Color.white;

        yield return new WaitForSeconds(maxTime);

        float time = 0;

        while (time != maxTime)
        {
            time = Mathf.Min(time + Time.deltaTime, maxTime);

            materialRenderers[id].material.color = Color.Lerp(Color.white, zeroAlpha, time / maxTime);

            yield return null;
        }

    }
}
