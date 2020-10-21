using UnityEngine.UI;
using UnityEngine;
using System;

public class LocationReferenceUIController : MonoBehaviour
{
    private Camera mainCamera;
    private float minPixelCoordX, minPixelCoordY;
    private float maxPixelCoordX, maxPixelCoordY;

    private Transform playerTransform;

    [SerializeField] private UIReference ballRefPrefab = null;
    private Transform[] ballTransforms;
    private UIReference[] ballReferences;
    private readonly Vector2 ballReferenceOffset = Vector2.up * 50f;

    [SerializeField] private UIReference teammatePrefab;
    private Transform teammateTransform;
    private UIReference teammateReference;

    public void Init()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();

        mainCamera = Camera.main;

        minPixelCoordX = ballRefPrefab.GetComponentInChildren<Image>().GetPixelAdjustedRect().width / 4;
        maxPixelCoordX = Screen.width - minPixelCoordX;
        minPixelCoordY = ballRefPrefab.GetComponentInChildren<Image>().GetPixelAdjustedRect().height / 4;
        maxPixelCoordY = Screen.height - minPixelCoordY;
    }

    public void HandleUpdate()
    {
        for (int i = 0; i < ballTransforms.Length; i++)
        {
            DisplayReference(ballReferences[i], ballTransforms[i], ballReferenceOffset);
        }

        if(teammateReference)
            DisplayReference(teammateReference, teammateTransform, Vector2.zero);
    }

    public void PrepareBallTransforms(ColoredBall[] balls)
    {
        ballTransforms = new Transform[balls.Length];
        ballReferences = new UIReference[balls.Length];
        for (int i = 0; i < ballTransforms.Length; i++)
        {
            ballTransforms[i] = balls[i].transform;
            ballReferences[i] = Instantiate(ballRefPrefab, transform);
        }
    }

    public void PrepareTeammateTransform(Transform teammate)
    {
        teammateTransform = teammate;
        teammateReference = Instantiate(teammatePrefab, transform);
    }

    private void DisplayReference(UIReference reference, Transform worldTransform, Vector2 offset)
    {
        int distance;

        distance = (int)Vector3.Distance(playerTransform.position, worldTransform.position);

        if (distance > 2)
        {
            reference.gameObject.SetActive(true);

            Vector2 screenPos = mainCamera.WorldToScreenPoint(worldTransform.position);

            screenPos.x = Mathf.Clamp(screenPos.x, minPixelCoordX, maxPixelCoordX);
            screenPos.y = Mathf.Clamp(screenPos.y, minPixelCoordY, maxPixelCoordY);

            // Check if ball is behind us
            if (Vector3.Dot(worldTransform.position - mainCamera.transform.position, mainCamera.transform.forward) < 0)
            {
                // Account for opposite X
                if (screenPos.x < Screen.width / 2)
                {
                    screenPos.x = maxPixelCoordX;
                }
                else
                {
                    screenPos.x = minPixelCoordX;
                }

                // Account for opposite Y
                if (screenPos.y < Screen.height / 2)
                {
                    screenPos.y = maxPixelCoordY;
                }
                else
                {
                    screenPos.y = minPixelCoordY;
                }
            }

            reference.GetComponent<RectTransform>().position = screenPos + offset;
            reference.SetDistanceText(distance);
        }
        else
        {
            reference.gameObject.SetActive(false);
        }
    }
}
