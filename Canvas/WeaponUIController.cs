//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class WeaponUIController : EventListener
//{
//    private CanvasController canvController;
//    [SerializeField] private GameObject contentWindow = null;

//    [SerializeField] private Image sniperZoomCrosshairImage = null;
//    [SerializeField] private Image[] standardCrosshairImages = null;
//    [SerializeField] private Image hitmarkerImage = null;
//    [SerializeField] private Image chargeBackground = null;
//    [SerializeField] private Image[] chargeMeters = null;

//    // Gun heat variables
//    [SerializeField] private GameObject overheatObj = null;
//    //[SerializeField] private Image overheatBackground = null;
//    [SerializeField] private Image overheatMeter = null;
//    private bool overheated = false;
//    private Color whiteColor = new Color(1.0f, 1.0f, 1.0f, .7490f);
//    private Color redColor = new Color(1.0f, 0.1490196f, 0.1529412f, .7490f);

//    private Coroutine scopeCoroutine = null;
//    private Coroutine hitMarkerCoroutine = null;

//    public void SetUp(CanvasController c)
//    {
//        canvController = c;

//        overheatObj.SetActive(false);

//        SetChargeBarMeters(8);
//    }

//    private void OnEnable()
//    {
//        EventController.AddListener(typeof(FOVChangeMessage), this);
//        EventController.AddListener(typeof(WeaponFiredMessage), this);
//        EventController.AddListener(typeof(AmmoChangeMessage), this);
//        EventController.AddListener(typeof(GunHeatChangeMessage), this);
//    }

//    private void OnDisable()
//    {
//        EventController.RemoveListener(typeof(FOVChangeMessage), this);
//        EventController.RemoveListener(typeof(WeaponFiredMessage), this);
//        EventController.RemoveListener(typeof(AmmoChangeMessage), this);
//        EventController.RemoveListener(typeof(GunHeatChangeMessage), this);
//    }

//    public override void HandleEvent(EventMessage e)
//    {
//        if (e is FOVChangeMessage fovMessage)
//        {
//            ZoomAnimation(fovMessage);
//        }
//        else if (e is WeaponFiredMessage weaponFire)
//        {
//            ToggleHitMarker(weaponFire);
//        }
//        else if (e is GunHeatChangeMessage gunHeat)
//        {
//            ChangeOverheatValue(gunHeat);
//        }
//        else if (e is AmmoChangeMessage ammoMessage)
//        {
//            SetChargeBarMeters(ammoMessage.amount);
//        }
        
//    }

//    public void ShowElements(bool show)
//    {
//        contentWindow.SetActive(show);
//    }

//    private void ChangeImageOpacity(Image i, float opacity)
//    {
//        Color start = i.color;

//        start.a = opacity;

//        i.color = start;
//    }

//    private void ChangeImageColor(Image i, Color c)
//    {
//        i.color = c;
//    }

//    #region Overheating

//    private void ChangeOverheatValue(GunHeatChangeMessage eventMessage)
//    {
//        if (!overheated && eventMessage.overheated)
//        {
//            overheated = true;
//            overheatMeter.color = redColor;
//        }
//        else if (overheated && !eventMessage.overheated)
//        {
//            overheated = false;
//            overheatMeter.color = whiteColor;
//        }

//        overheatMeter.fillAmount = eventMessage.amount;

//        overheatObj.SetActive(eventMessage.amount > 0);
//    }

//    #endregion

//    #region Hit Markers
//    private void ToggleHitMarker(WeaponFiredMessage e)
//    {
//        if (hitMarkerCoroutine != null)
//            StopCoroutine(hitMarkerCoroutine);

//        hitMarkerCoroutine = StartCoroutine(WaitToReset(e.fireRate));
//    }

//    private IEnumerator WaitToReset(float waitTime)
//    {
//        float time = 0f;
//        float maxTime = waitTime;

//        Vector3 reducedScale = new Vector3(0.80f, 0.80f, 0.80f);

//        while (time != maxTime)
//        {
//            time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);

//            hitmarkerImage.transform.localScale = Vector3.Lerp(reducedScale, Vector3.one, time / maxTime);

//            yield return null;
//        }
//    }

//    #endregion

//    #region Charge Meter

//    public void SetChargeBarMeters(int num)
//    {
//        for (int i = 0; i < chargeMeters.Length; i++)
//        {
//            chargeMeters[i].gameObject.SetActive(i < num);
//        }
//    }

//    #endregion

//    #region Scope Animation

//    private void ZoomAnimation(FOVChangeMessage eventMessage)
//    {
//        if (scopeCoroutine != null)
//        {
//            StopCoroutine(scopeCoroutine);
//        }

//        scopeCoroutine = StartCoroutine(ScopeAnim(eventMessage.zoomSpeed, eventMessage.loweringFOV));

//        canvController.ChangeGameUIVisibility(!eventMessage.loweringFOV);
//    }

//    private IEnumerator ScopeAnim(float animSpeed, bool scopeIn)
//    {
//        int resultAlpha = scopeIn ? 1 : 0;
//        float startAlpha = sniperZoomCrosshairImage.color.a;
//        float time = 0;
//        while (time != 1.0f)
//        {
//            time = Mathf.Clamp(time + Time.deltaTime / animSpeed, 0.0F, 1.0f);

//            Color temp = sniperZoomCrosshairImage.color;
//            temp.a = Mathf.Lerp(startAlpha, resultAlpha, time);
//            sniperZoomCrosshairImage.color = temp;

//            // Hitmarkers match background opacity
//            ChangeImageOpacity(hitmarkerImage, temp.a);

//            // Charge meters match background opacity
//            ChangeImageOpacity(chargeBackground, temp.a);

//            foreach (Image meter in chargeMeters)
//            {
//                ChangeImageOpacity(meter, temp.a);
//            }

//            // Reverse the other image
//            temp.a = 1 - temp.a;
//            foreach (Image crosshairPiece in standardCrosshairImages)
//            {
//                crosshairPiece.color = temp;
//            }

//            yield return null;
//        }
//    }

//    #endregion
//}
