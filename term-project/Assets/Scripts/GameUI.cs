using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    private Text ammoText;
    private Text levelText;
    private GameObject gameOverOverlay;
    private Text gameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        CreateHUD();
        UpdateLevel();
    }

    private void CreateHUD()
    {
        var canvas = new GameObject("HUDCanvas");
        var canvasGroup = canvas.AddComponent<Canvas>();
        canvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // Level text (top center)
        levelText = new GameObject("LevelText").AddComponent<Text>();
        levelText.transform.SetParent(canvas.transform, false);
        levelText.rectTransform.sizeDelta = new Vector2(200, 40);
        levelText.rectTransform.anchoredPosition = new Vector2(0, 250);
        levelText.rectTransform.anchorMin = new Vector2(0.5f, 1);
        levelText.rectTransform.anchorMax = new Vector2(0.5f, 1);
        levelText.font = Font.CreateDynamicFontFromOSFont("Arial", 28) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 28);
        levelText.color = Color.white;
        levelText.alignment = TextAnchor.MiddleCenter;

        // Ammo text (bottom right)
        ammoText = new GameObject("AmmoText").AddComponent<Text>();
        ammoText.transform.SetParent(canvas.transform, false);
        ammoText.rectTransform.sizeDelta = new Vector2(200, 40);
        ammoText.rectTransform.anchoredPosition = new Vector2(255, 30);
        ammoText.rectTransform.anchorMin = new Vector2(0.5f, 0);
        ammoText.rectTransform.anchorMax = new Vector2(0.5f, 0);
        ammoText.font = Font.CreateDynamicFontFromOSFont("Arial", 24) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 24);
        ammoText.color = Color.white;
        ammoText.alignment = TextAnchor.MiddleCenter;

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

        // Game over text
        gameOverText = new GameObject("GameOverText").AddComponent<Text>();
        gameOverText.transform.SetParent(gameOverOverlay.transform, false);
        gameOverText.rectTransform.sizeDelta = new Vector2(400, 60);
        gameOverText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        gameOverText.rectTransform.anchoredPosition = Vector2.zero;
        gameOverText.font = Font.CreateDynamicFontFromOSFont("Arial", 48) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 48);
        gameOverText.color = Color.white;
        gameOverText.alignment = TextAnchor.MiddleCenter;
    }

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"Ammo: {current}/{max}";
    }

    public void UpdateLevel()
    {
        levelText.text = $"Level {LevelData.CurrentLevel + 1}/3";
    }

    public void ShowWin()
    {
        gameOverOverlay.SetActive(true);
        bool isFinalLevel = LevelData.CurrentLevel >= 2;
        gameOverText.text = isFinalLevel ? "ALL LEVELS COMPLETE!\nYOU WIN!" : "LEVEL COMPLETE!\nPress N for next level";
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
