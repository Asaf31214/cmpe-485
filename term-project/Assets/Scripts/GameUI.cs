using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    private Text ammoText;
    private Text targetsText;
    private Text levelText;
    private Text profilerText;
    private GameObject gameOverOverlay;
    private Text gameOverText;
    private RectTransform powerBarFill;

    private CannonController cannon;

    private float fpsTimer;
    private int frameCount;
    private long lastGCMemory;

    private void Awake()
    {
        Instance = this;
        CreateHUD();
        lastGCMemory = System.GC.GetTotalMemory(false);
    }

    private void Start()
    {
        cannon = FindObjectOfType<CannonController>();
    }

    private void Update()
    {
        UpdatePowerBar();
        UpdateProfiler();
    }

    private void CreateHUD()
    {
        // HUD Canvas
        var canvas = new GameObject("HUDCanvas");
        var canvasGroup = canvas.AddComponent<Canvas>();
        canvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // Level text (top center)
        levelText = new GameObject("LevelText").AddComponent<Text>();
        levelText.transform.SetParent(canvas.transform, false);
        levelText.rectTransform.sizeDelta = new Vector2(200, 40);
        levelText.rectTransform.anchoredPosition = new Vector2(327, -55);
        levelText.rectTransform.anchorMin = new Vector2(0.5f, 1);
        levelText.rectTransform.anchorMax = new Vector2(0.5f, 1);
        levelText.font = Font.CreateDynamicFontFromOSFont("Arial", 28) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 28);
        levelText.color = Color.white;
        levelText.alignment = TextAnchor.MiddleCenter;

        // Targets text (bottom right, above ammo)
        targetsText = new GameObject("TargetsText").AddComponent<Text>();
        targetsText.transform.SetParent(canvas.transform, false);
        targetsText.rectTransform.sizeDelta = new Vector2(200, 40);
        targetsText.rectTransform.anchoredPosition = new Vector2(316, 128);
        targetsText.rectTransform.anchorMin = new Vector2(0.5f, 0);
        targetsText.rectTransform.anchorMax = new Vector2(0.5f, 0);
        targetsText.font = Font.CreateDynamicFontFromOSFont("Arial", 24) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 24);
        targetsText.color = Color.white;
        targetsText.alignment = TextAnchor.MiddleCenter;

        // Ammo text (bottom right)
        ammoText = new GameObject("AmmoText").AddComponent<Text>();
        ammoText.transform.SetParent(canvas.transform, false);
        ammoText.rectTransform.sizeDelta = new Vector2(200, 40);
        ammoText.rectTransform.anchoredPosition = new Vector2(314, 100);
        ammoText.rectTransform.anchorMin = new Vector2(0.5f, 0);
        ammoText.rectTransform.anchorMax = new Vector2(0.5f, 0);
        ammoText.font = Font.CreateDynamicFontFromOSFont("Arial", 24) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 24);
        ammoText.color = Color.white;
        ammoText.alignment = TextAnchor.MiddleCenter;

        // Power bar (bottom left)
        var barBackground = new GameObject("BarBackground");
        barBackground.transform.SetParent(canvas.transform, false);
        var bgRect = barBackground.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(200, 20);
        bgRect.anchoredPosition = new Vector2(-339, -152);
        bgRect.anchorMin = new Vector2(0.5f, 0.5f);
        bgRect.anchorMax = new Vector2(0.5f, 0.5f);
        var bgImage = barBackground.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);

        var barFill = new GameObject("BarFill");
        barFill.transform.SetParent(barBackground.transform, false);
        powerBarFill = barFill.AddComponent<RectTransform>();
        powerBarFill.sizeDelta = new Vector2(200, 20);
        powerBarFill.anchorMin = new Vector2(0, 0);
        powerBarFill.anchorMax = new Vector2(0, 1);
        powerBarFill.anchoredPosition = Vector2.zero;
        powerBarFill.pivot = new Vector2(0, 0.5f);
        var fillImage = barFill.AddComponent<Image>();
        fillImage.color = Color.red;

        // Game over overlay
        gameOverOverlay = new GameObject("GameOverOverlay");
        gameOverOverlay.transform.SetParent(canvas.transform, false);
        var overlayRect = gameOverOverlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        var overlayImage = gameOverOverlay.AddComponent<Image>();
        overlayImage.color = new Color(0, 0, 0, 0.7f);
        overlayImage.raycastTarget = false;
        gameOverOverlay.SetActive(false);

        gameOverText = new GameObject("GameOverText").AddComponent<Text>();
        gameOverText.transform.SetParent(gameOverOverlay.transform, false);
        gameOverText.rectTransform.sizeDelta = new Vector2(400, 60);
        gameOverText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.anchoredPosition = Vector2.zero;
        gameOverText.font = Font.CreateDynamicFontFromOSFont("Arial", 48) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 48);
        gameOverText.color = Color.white;
        gameOverText.alignment = TextAnchor.MiddleCenter;

        // Profiler canvas (separate, like original)
        var profilerCanvas = new GameObject("ProfilerCanvas");
        var profilerCanvasGroup = profilerCanvas.AddComponent<Canvas>();
        profilerCanvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        profilerCanvas.AddComponent<CanvasScaler>();
        profilerCanvas.AddComponent<GraphicRaycaster>();

        profilerText = new GameObject("ProfilerText").AddComponent<Text>();
        profilerText.transform.SetParent(profilerCanvas.transform, false);
        profilerText.rectTransform.sizeDelta = new Vector2(400, 150);
        profilerText.rectTransform.anchoredPosition = new Vector2(279, -118);
        profilerText.rectTransform.anchorMin = new Vector2(0, 1);
        profilerText.rectTransform.anchorMax = new Vector2(0, 1);
        profilerText.font = Font.CreateDynamicFontFromOSFont("Arial", 16) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
        profilerText.color = Color.yellow;
        profilerText.alignment = TextAnchor.UpperLeft;

        UpdateAmmo(LevelData.MaxAmmo);
        UpdateTargets(LevelData.GetTargetCount());
        UpdateLevel();
    }

    private void UpdatePowerBar()
    {
        if (cannon == null || powerBarFill == null) return;
        float ratio = (cannon.Power - cannon.MinPower) / (cannon.MaxPower - cannon.MinPower);
        powerBarFill.sizeDelta = new Vector2(200f * ratio, 20);
    }

    private void UpdateProfiler()
    {
        frameCount++;
        fpsTimer += Time.deltaTime;

        if (fpsTimer >= 0.1f)
        {
            float fps = frameCount / fpsTimer;
            frameCount = 0;
            fpsTimer = 0;

            long currentGC = System.GC.GetTotalMemory(false);
            long gcAlloc = currentGC - lastGCMemory;
            lastGCMemory = currentGC;

            int physicsObjects = FindObjectsOfType<Rigidbody>().Length;

            profilerText.text = $"FPS: {fps:F1}\n" +
                               $"Physics Objects: {physicsObjects}\n" +
                               $"GC Alloc (this frame): {gcAlloc / 1024.0:F1} KB\n" +
                               $"Total Memory: {currentGC / 1024.0 / 1024.0:F1} MB";
        }
    }

    public void Refresh()
    {
        UpdateAmmo(LevelData.MaxAmmo);
        UpdateTargets(LevelData.GetTargetCount());
        UpdateLevel();
        HideGameOver();
    }

    public void UpdateAmmo(int ammo)
    {
        ammoText.text = $"Ammo: {ammo}/{LevelData.MaxAmmo}";
    }

    public void UpdateTargets(int remaining)
    {
        targetsText.text = $"Targets: {remaining}/{LevelData.GetTargetCount()}";
    }

    public void UpdateLevel()
    {
        levelText.text = $"Level {LevelData.CurrentLevel + 1}/{LevelData.TotalLevels}";
    }

    public void ShowWin(bool finalLevel)
    {
        gameOverOverlay.SetActive(true);
        gameOverText.text = finalLevel ? "ALL LEVELS COMPLETE!\nYOU WIN!" : "LEVEL COMPLETE!\nPress N for next level";
        gameOverText.color = Color.green;
    }

    public void ShowLose()
    {
        gameOverOverlay.SetActive(true);
        gameOverText.text = "YOU LOSE!\nPress R to restart";
        gameOverText.color = Color.red;
    }

    public void HideGameOver()
    {
        gameOverOverlay.SetActive(false);
    }
}