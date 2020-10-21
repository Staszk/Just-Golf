using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameplayUIController : EventListener
{
    public static GameplayUIController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private Camera mainCamera;

    //[SerializeField] private GameObject persistentElements = null;
    [SerializeField] private GameObject staticElements = null;
    [SerializeField] private GameObject dynamicElements = null;
    [SerializeField] private DeathScreen deathScreen = null;

    // Switching Modes Variables
    [SerializeField] private Image clubImage = null;
    [SerializeField] private Image gunImage = null;
    private Color activeColor = new Color(.8301f, .8301f, .8301f, 1f);
    private Color inactiveColor = new Color(.8301f, .8301f, .8301f, .3490f);
    [SerializeField] private GameObject[] clubPowerIcons = null;

    [SerializeField] private DamageIndicator damageIndicator = null;

    [SerializeField] private Transform damagePopOffParent = null;
    [SerializeField] private DamagePopOff damagePopOffObject = null;
    private Queue<DamagePopOff> availablePopOffs = null;
    private List<DamagePopOff> activePopOffs = null;
    private readonly int popOffCount = 30;

    [SerializeField] private GameObject[] rankImages = null;
    [SerializeField] private TMP_Text currentStrokeText = null;
    [SerializeField] private TMP_Text personalBestText = null;

    [SerializeField] private StrokeChangeController strokeController = null;

    [SerializeField] private TMP_Text healthText = null;
    [SerializeField] private Image[] healthbars = null;
    private Color standardHealthColor = new Color(0f, 0.6256f, 1f);
    private Color fullHealthColor = new Color(0.6256f, 1f, 1f);
    private Color damageColor = new Color(1.0f, 0.1490196f, 0.1529412f);
    private Color lowHealthColor = new Color(0.5686275f, 0f, 0.003921569f);
    private Color currentHealthbarColor = Color.white;
    private Coroutine healthBarCoroutine = null;

    [SerializeField] private Transform ballTransform = null;
    [SerializeField] private Transform playerTransform = null;
    [SerializeField] private Image ballLocationReference = null;
    [SerializeField] private TMP_Text distanceText = null;
    private float minPixelCoordX, minPixelCoordY;
    private float maxPixelCoordX, maxPixelCoordY;

    // Items
    [SerializeField] private Image itemTimerFillImage = null;
    [SerializeField] private Image itemIconImage = null;

    // Kill Feed
    [SerializeField] private KillFeedController killfeed = null;
    [SerializeField] private ItemFeedController itemfeed = null;

    // Elimated Feed
    [SerializeField] private EliminatedFeedController eliminatedFeed = null;

    // Respects Feed
    [SerializeField] private RespectsFeedController respectsFeed = null;

    // Screen Button Prompts
    [SerializeField] private KeybindingPromptController keybindPrompts = null;

    [SerializeField] private TMP_Text[] timeTexts = null;

    private bool isGameOver = false;

    private float timeSeconds = 8 * 60;

    public void SetUp()
    {
        mainCamera = Camera.main;

        DisplayHealth(100, 100);

        minPixelCoordX = ballLocationReference.GetPixelAdjustedRect().width / 4;
        maxPixelCoordX = Screen.width - minPixelCoordX;
        minPixelCoordY = ballLocationReference.GetPixelAdjustedRect().height / 4;
        maxPixelCoordY = Screen.height - minPixelCoordY;

        SetUpDamagePopOffs();

        strokeController.Init();
        killfeed.Init();
        itemfeed.Init();
        eliminatedFeed.Init();
        respectsFeed.Init();

        UpdatePoints(0, null);
        ChangeClubPowerIcons(new ClubPowerChangedMessage(0));
    }

    private void OnEnable()
    {
        //GolfClubController.EventModeChanged += PlayModeChange;
        EventController.AddListener(typeof(ModeChangedMessage), this);
        EventController.AddListener(typeof(ClubPowerChangedMessage), this);
        UsableItem.EventItemInUse += UpdateItemFill;
        UsableItem.EventItemUseDone += StopShowingItem;

        EventController.AddListener(typeof(AtFullHealthMessage), this);

        EventController.AddListener(typeof(ClientDeathMessage), this);
        EventController.AddListener(typeof(ClientWaitToRespawnMessage), this);
        EventController.AddListener(typeof(ClientRespawnMessage), this);

        NetworkGameStateManager.EventKillFeed += DisplayKill;
        NetworkGameStateManager.EventKillFeed += DisplayEliminatedMessage;
        NetworkGameStateManager.EventEndGame += EndGame;
        NetworkItemManager.EventItemFeedEvent += DisplayItem;
        NetworkGameStateManager.EventPayRespects += DisplayRespectsMessage;
    }

    private void OnDisable()
    {
        EventController.RemoveListener(typeof(ModeChangedMessage), this);
        EventController.RemoveListener(typeof(ClubPowerChangedMessage), this);
        UsableItem.EventItemInUse -= UpdateItemFill;
        UsableItem.EventItemUseDone -= StopShowingItem;

        EventController.RemoveListener(typeof(AtFullHealthMessage), this);

        EventController.RemoveListener(typeof(ClientDeathMessage), this);
        EventController.RemoveListener(typeof(ClientWaitToRespawnMessage), this);
        EventController.RemoveListener(typeof(ClientRespawnMessage), this);

        NetworkGameStateManager.EventKillFeed -= DisplayKill;
        NetworkGameStateManager.EventKillFeed -= DisplayEliminatedMessage;
        NetworkGameStateManager.EventEndGame -= EndGame;
        NetworkItemManager.EventItemFeedEvent -= DisplayItem;
        NetworkGameStateManager.EventPayRespects -= DisplayRespectsMessage;

    }

    public override void HandleEvent(EventMessage e)
    {
        if (e is ModeChangedMessage modeChange)
        {
            PlayModeChange(modeChange);
        }
        else if (e is ClubPowerChangedMessage clubPower)
        {
            ChangeClubPowerIcons(clubPower);
        }
        else if (e is ClientDeathMessage deathMessage)
        {
            ShowDeathScreen(deathMessage);
        }
        else if (e is ClientWaitToRespawnMessage waitRespawnMessage)
        {
            UpdateDeathScreen(waitRespawnMessage);
        }
        else if (e is ClientRespawnMessage respawnMessage)
        {
            HideDeathScreen();
        }
        else if (e is AtFullHealthMessage)
        {
            AtFullHealth();
        }
    }

    private void Update()
    {
        if (!isGameOver) { CheckTimer(); }
       
        ShowBall();
        ShowPopOffs();
    }

    #region Ball Indicator

    private void ShowBall()
    {
        int dist = (int)Vector3.Distance(playerTransform.transform.position, ballTransform.position);

        if (dist > 2)
        {
            ballLocationReference.gameObject.SetActive(true);

            Vector2 screenPos = (Vector2)mainCamera.WorldToScreenPoint(ballTransform.position);

            screenPos.x = Mathf.Clamp(screenPos.x, minPixelCoordX, maxPixelCoordX);
            screenPos.y = Mathf.Clamp(screenPos.y, minPixelCoordY, maxPixelCoordY);

            // Check if ball is behind us
            if (Vector3.Dot(ballTransform.position - mainCamera.transform.position, mainCamera.transform.forward) < 0)
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

            ballLocationReference.rectTransform.position = screenPos + Vector2.up * 60f;
            distanceText.text = dist.ToString() + "m";
        }
        else
        {
            ballLocationReference.gameObject.SetActive(false);
        }
    }

    #endregion

    #region Damage Indicators

    private void SetUpDamagePopOffs()
    {
        availablePopOffs = new Queue<DamagePopOff>();
        activePopOffs = new List<DamagePopOff>();

        for (int i = 0; i < popOffCount; i++)
        {
            availablePopOffs.Enqueue(CreateNewPopoff());
        }
    }

    private DamagePopOff CreateNewPopoff()
    {
        DamagePopOff newPopOff = Instantiate(damagePopOffObject, damagePopOffParent);
        newPopOff.Init(this);

        return newPopOff;
    }

    public void UsePopOff(SimulatedDamageMessage eventMessage)
    {
        DamagePopOff newPopOff;

        if (availablePopOffs.Count > 0)
        {
            newPopOff = availablePopOffs.Dequeue();
        }
        else
        {
            Debug.LogWarning("Instantiated new Damage Pop Off");
            newPopOff = CreateNewPopoff();
        }

        float squareDistance = (eventMessage.worldPosition - mainCamera.transform.position).sqrMagnitude;
        float scale = Mathf.Clamp(squareDistance.Map(300f, 3000f, 1f, 0.45f), 0.45f, 1f);

        newPopOff.Use(eventMessage.worldPosition, eventMessage.damageAmount.ToString(), eventMessage.isCritical, scale);
        activePopOffs.Add(newPopOff);
    }

    private void ShowPopOffs()
    {
        foreach (DamagePopOff damagePop in activePopOffs)
        {
            damagePop.transform.position = mainCamera.WorldToScreenPoint(damagePop.worldPos);

            float squareDistance = (damagePop.worldPos - mainCamera.transform.position).sqrMagnitude;

            float scale = Mathf.Clamp(squareDistance.Map(300f, 3000f, 1f, 0.35f), 0.35f, 1f);

            damagePop.GetComponent<RectTransform>().localScale = Vector3.one * scale;
        }
    }

    public void StopPopOff(DamagePopOff damagePop)
    {
        activePopOffs.Remove(damagePop);
        availablePopOffs.Enqueue(damagePop);
    }

    public void ShowDamageReceived(Vector3 shotOrigin)
    {
        //damageIndicator.Show(mainCamera.transform.forward, shotOrigin - playerTransform.position);

        if (healthBarCoroutine != null)
            StopCoroutine(healthBarCoroutine);

        healthBarCoroutine = StartCoroutine(HealthbarFlash(damageColor));
    }

    private IEnumerator HealthbarFlash(Color fromColor, float length = 1.0f)
    {
        ChangeHealthBarColor(fromColor);

        float time = 0f;
        float maxTime = length;

        while (time != maxTime)
        {
            time = Mathf.Clamp(time + Time.deltaTime, 0, maxTime);

            Color c = Color.Lerp(fromColor, currentHealthbarColor, time / maxTime);
            Color c2 = Color.Lerp(fromColor, Color.white, time / maxTime);

            healthText.transform.localScale = Vector3.one * (2 - time / maxTime);

            ChangeHealthBarColor(c);
            ChangeHealthTextColor(c2);

            yield return null;
        }
    }

    #endregion

    #region Health

    public void DisplayHealth(int health, int maxHealth)
    {
        if (health == maxHealth)
        {
            AtFullHealth(0.5f);
            damageIndicator.MarkLowHealth(false);
        }
        else if (health <= 20 && health > 0 && !damageIndicator.LowHealth)
        {
            damageIndicator.MarkLowHealth(true);
        }
        else if (health > 20)
        {
            damageIndicator.MarkLowHealth(false);
        }

        Color c = health > 20 ? standardHealthColor : lowHealthColor;

        if (c != currentHealthbarColor)
        {
            ChangeHealthBarColor(c);
            currentHealthbarColor = c;
        }

        float divisor = 10.0f * (maxHealth / 100.0f);
        int barsUsed = (int)Mathf.Ceil(health / divisor);

        for (int i = 0; i < healthbars.Length; i++)
        {
            healthbars[i].fillAmount = 1;
            if (i <= barsUsed - 1)            
                healthbars[i].gameObject.SetActive(true);          
            else            
                healthbars[i].gameObject.SetActive(false);            
        }

        float diff = (barsUsed * divisor) - health;

        float fillAmount = diff > 0 ? 1 - diff / divisor : 1;

        int index = barsUsed > 0 ? barsUsed - 1 : 0;

        healthbars[index].fillAmount = fillAmount;
        healthText.text = health.ToString();
    }

    private void AtFullHealth()
    {
        AtFullHealth(0.25f);
    }

    private void AtFullHealth(float time)
    {
        if (healthBarCoroutine != null)
            StopCoroutine(healthBarCoroutine);

        healthBarCoroutine = StartCoroutine(HealthbarFlash(fullHealthColor, time));
    }

    private void ChangeHealthBarColor(Color toColor)
    {
        foreach (Image bar in healthbars)
        {
            bar.color = toColor;
        }

        //healthText.color = toColor;
    }

    private void ChangeHealthTextColor(Color toColor)
    {
        healthText.color = toColor;
    }

    #endregion

    #region Club Icons

    private void PlayModeChange(ModeChangedMessage eventMessage)
    {
        gunImage.color = eventMessage.modeChangedToGolf ? inactiveColor : activeColor;
        clubImage.color = eventMessage.modeChangedToGolf ? activeColor : inactiveColor;

        keybindPrompts.FadeClubPowerPrompt(!eventMessage.modeChangedToGolf);

        //for (int i = 0; i <= eventMessage.golfPower; i++)
        //{
        //    clubPowerIcons[i].SetActive(eventMessage.modeChangedToGolf);
        //}
    }

    private void ChangeClubPowerIcons(ClubPowerChangedMessage eventMessage)
    {
        for (int i = 0; i < clubPowerIcons.Length; i++)
        {
            clubPowerIcons[i].SetActive(i <= eventMessage.power);
        }
    }

    #endregion

    #region Points

    public void UpdatePoints(int current, int? best)
    {
        int old = int.Parse(currentStrokeText.text);
        int diff = current - old;

        if (diff != 0)
        {
            string text = diff > 0 ? "+" + diff.ToString() : diff.ToString();
            Color col = diff > 0 ? Color.red : Color.green;
            strokeController.UseObject(text, col);
        }

        currentStrokeText.text = current.ToString();
        personalBestText.text = best != null ? best.ToString() : "X";
    }

    public void ShowRank(int rank)
    {
        for (int i = 0; i < rankImages.Length; i++)
        {
            rankImages[i].SetActive(i == rank);
        };
    }

    #endregion

    #region Items

    public void ShowNewItem(Sprite icon)
    {
        itemIconImage.sprite = icon;
        UpdateItemFill(1.0f);

        itemIconImage.gameObject.SetActive(true);
        itemTimerFillImage.gameObject.SetActive(true);
    }

    private void UpdateItemFill(float ratio)
    {
        itemTimerFillImage.fillAmount = ratio;
    }

    private void StopShowingItem()
    {
        itemIconImage.gameObject.SetActive(false);
        itemTimerFillImage.gameObject.SetActive(false);
    }

    #endregion

    #region Kill Feed
    public void DisplayKill(NetworkKillFeedMessage eventMessage)
    {
        killfeed.UseElement(eventMessage.killerID, eventMessage.victimID);
    }

    #endregion

    #region Item Feed
    private void DisplayItem(ItemFeedMessage eventMessage)
    {
        Sprite icon = ItemBoxController.GetItemSprite(eventMessage.item);
        itemfeed.UseElement(eventMessage.userID, icon);
    }

    #endregion

    #region Eliminated Feed

    private void DisplayEliminatedMessage(NetworkKillFeedMessage eventMessage)
    {
        eliminatedFeed.UseElement(eventMessage.killerID, eventMessage.victimID);
    }

    #endregion

    #region Respects Feed

    private void DisplayRespectsMessage(NetworkPayRespectsMessage e)
    {
        respectsFeed.UseElement(e.playerID);
        print("RESPECT");

    }

    #endregion

    #region Death Screen

    // NEW EVENT SYSTEM REMOVES NEED FOR IENUMERATOR
    private void ShowDeathScreen(ClientDeathMessage e)
    {
        while (activePopOffs.Count > 0)
        {
            activePopOffs[0].Done();
        }

        ShowStatic(false);
        ShowDynamic(false);

        damageIndicator.Hide();

        deathScreen.NewDeath(e.killerID);
    }

    private void UpdateDeathScreen(ClientWaitToRespawnMessage eventMessage)
    {
        deathScreen.UpdateTimer(eventMessage.timeLeft);
    }

    private void HideDeathScreen()
    {
        ShowStatic(true);
        ShowDynamic(true);

        respectsFeed.Done();

        deathScreen.Hide();
    }

    #endregion

    #region General

    public void ShowDynamic(bool show)
    {
        dynamicElements.SetActive(show);
    }

    public void ShowStatic(bool show)
    {
        staticElements.SetActive(show);
    }

    private void CheckTimer()
    {
        timeSeconds -= Time.deltaTime;
        if (timeSeconds >= 1)
        {
            float min = Mathf.Floor(timeSeconds / 60.0f);
            float sec = Mathf.Floor(timeSeconds % 60.0f);

            timeTexts[0].text = timeTexts[1].text = string.Format("{0:}:{1:}", min.ToString("00"), sec.ToString("00"));
        }
        else
        {
            if (NetworkManager.instance) { NetworkManager.instance.SendEndGameRequest(); }

            timeSeconds = 0;

            isGameOver = true;
            timeTexts[0].text = timeTexts[1].text = "";
          
        }
    }

    private void EndGame()
    {
        timeSeconds = 0.0f;
        isGameOver = true;
        timeTexts[0].text = timeTexts[1].text = "";
    }

    #endregion
}
