using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MapTimeScript : EventListener
{
    [SerializeField] private List<Material> skyboxMaterials = null;

    public GameObject sunLightHolder;
    public GameObject fillLightHolder;
    public GameObject dayPointLights;
    public GameObject nightPointLights;

    private Material skyboxLerpMat;
    private GameObject pointLights;
    private Light sunRef, moonRef;
    private Light sunLerpLight;
    private Light[] sunAccentLights;

    private bool isDay = true;

    [ContextMenu("Day Time")]
    public void DayTime()
    {
        RenderSettings.skybox = skyboxMaterials[0];
        sunLightHolder.SetActive(true);
        fillLightHolder.transform.GetChild(2).gameObject.SetActive(false);
        dayPointLights.SetActive(true);
        nightPointLights.SetActive(false);
        isDay = true;
    }

    [ContextMenu("Night Time")]
    public void NightTime()
    {
        RenderSettings.skybox = skyboxMaterials[1];
        sunLightHolder.SetActive(false);
        fillLightHolder.transform.GetChild(2).gameObject.SetActive(true);
        dayPointLights.SetActive(false);
        nightPointLights.SetActive(true);
        isDay = false;
    }

    private void Awake()
    {

        // Skybox
        skyboxLerpMat = new Material(skyboxMaterials[0]);
        // Sun/Moon
        Transform sunHolder = new GameObject("Sun Holder").transform;
        sunHolder.transform.SetParent(transform);
        sunRef = sunLightHolder.transform.GetChild(0).GetComponent<Light>();
        moonRef = fillLightHolder.transform.GetChild(2).GetComponent<Light>();
        sunLerpLight = Instantiate(sunRef, sunHolder);
        sunLerpLight.name = "Lerp Light";
        sunLerpLight.gameObject.SetActive(true);

        // Sun Accent Lights
        sunAccentLights = new Light[]
        {
            Instantiate(sunLightHolder.transform.GetChild(1).GetComponent<Light>(), sunHolder),
            Instantiate(sunLightHolder.transform.GetChild(2).GetComponent<Light>(), sunHolder),
            Instantiate(sunLightHolder.transform.GetChild(3).GetComponent<Light>(), sunHolder)
        };

        // Point lights
        pointLights = Instantiate(dayPointLights, transform);
        pointLights.name = "Lerp Point Lights";
        pointLights.SetActive(true);

        // Turn off standard lights in Hierarchy
        dayPointLights.SetActive(false);
        nightPointLights.SetActive(false);
        moonRef.gameObject.SetActive(false);
        sunLightHolder.SetActive(false);
    }

    private void OnEnable()
    {
        EventController.AddListener(typeof(GameTimeMessage), this);
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(GameTimeMessage), this);
    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is GameTimeMessage timeMessage)
        {
            TransitionDayToNight(timeMessage.percentDone * 2);
        }
    }

    public void ToggleLighting()
    {
        if (isDay)
            NightTime();
        else
            DayTime();
    }

    private void TransitionDayToNight(float percentToNight)
    {
        // Cap lerp to max
        percentToNight = Mathf.Clamp01(percentToNight);
        // Skybox lerp

        skyboxLerpMat.Lerp(skyboxMaterials[0], skyboxMaterials[1], percentToNight);      
        RenderSettings.skybox = skyboxLerpMat;

        // Sun to Moon lerp
        sunLerpLight.transform.rotation = Quaternion.Slerp(sunRef.transform.rotation, moonRef.transform.rotation, percentToNight);
        sunLerpLight.color = Color.Lerp(sunRef.color, moonRef.color, percentToNight);
        sunLerpLight.intensity = Mathf.Lerp(sunRef.intensity, moonRef.intensity, percentToNight);

        // Sun Accents
        for (int i = 0; i < sunAccentLights.Length; i++)
        {
            sunAccentLights[i].intensity = Mathf.Lerp(0.5f, 0f, percentToNight);
        }

        // Point Light lerp
        int amount = pointLights.transform.childCount;
        for (int i = 0; i < amount; i++)
        {
            Light thisLight = pointLights.transform.GetChild(i).GetComponent<Light>();
            Light dayLight = dayPointLights.transform.GetChild(i).GetComponent<Light>();
            Light nightLight = nightPointLights.transform.GetChild(i).GetComponent<Light>();

            thisLight.color = Color.Lerp(dayLight.color, nightLight.color, percentToNight);
            thisLight.intensity = Mathf.Lerp(dayLight.intensity, nightLight.intensity, percentToNight);
        }
      


    }
}
