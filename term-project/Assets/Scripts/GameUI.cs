using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public static GameUI Instance { get; private set; }

    private Text ammoText;
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
    }

    private void CreateHUD()
    {
        var canvas = new GameObject("HUDCanvas");
        var canvasGroup = canvas.AddComponent<Canvas>();
        canvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        ammoText = new GameObject("AmmoText").AddComponent<Text>();
        ammoText.transform.SetParent(canvas.transform, false);
        ammoText.rectTransform.sizeDelta = new Vector2(200, 40);
        ammoText.rectTransform.anchoredPosition = new Vector2(255, 30);
        ammoText.rectTransform.anchorMin = new Vector2(0.5f, 0);
        ammoText.rectTransform.anchorMax = new Vector2(0.5f, 0);
        ammoText.font = Font.CreateDynamicFontFromOSFont("Arial", 24) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 24);
        ammoText.color = Color.white;
        ammoText.alignment = TextAnchor.MiddleCenter;

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
    }

    public void UpdateAmmo(int current, int max)
    {
        ammoText.text = $"Ammo: {current}/{max}";
    }

    public void ShowWin()
    {
        gameOverOverlay.SetActive(true);
        gameOverText.text = "YOU WIN!";
        gameOverText.color = Color.green;
    }

    public void ShowLose()
    {
        gameOverOverlay.SetActive(true);
        gameOverText.text = "YOU LOSE!";
        gameOverText.color = Color.red;
    }

    public void HideGameOver()
    {
        gameOverOverlay.SetActive(false);
    }
}
