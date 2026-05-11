using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    private Camera cam;
    private GameObject barBackground;
    private GameObject barFill;
    private CannonController cannon;

    private void Start()
    {
        cam = Camera.main;
        cannon = FindObjectOfType<CannonController>();
        CreatePowerBar();
    }

    private void CreatePowerBar()
    {
        var canvas = new GameObject("PowerBarCanvas");
        var canvasGroup = canvas.AddComponent<Canvas>();
        canvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        // Background bar
        barBackground = new GameObject("BarBackground");
        barBackground.transform.SetParent(canvas.transform, false);
        var bgRect = barBackground.AddComponent<RectTransform>();
        bgRect.sizeDelta = new Vector2(200, 20);
        bgRect.anchoredPosition = new Vector2(-255, 30);
        bgRect.anchorMin = new Vector2(0.5f, 0);
        bgRect.anchorMax = new Vector2(0.5f, 0);
        var bgImage = barBackground.AddComponent<Image>();
        bgImage.color = new Color(0.3f, 0.3f, 0.3f, 0.7f);

        // Fill bar
        barFill = new GameObject("BarFill");
        barFill.transform.SetParent(barBackground.transform, false);
        var fillRect = barFill.AddComponent<RectTransform>();
        fillRect.sizeDelta = new Vector2(200, 20);
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(0, 1);
        fillRect.anchoredPosition = Vector2.zero;
        fillRect.pivot = new Vector2(0, 0.5f);
        var fillImage = barFill.AddComponent<Image>();
        fillImage.color = Color.red;
    }

    private void Update()
    {
        if (barFill == null || cannon == null) return;
        float ratio = (cannon.Power - cannon.MinPower) / (cannon.MaxPower - cannon.MinPower);
        var rect = barFill.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200f * ratio, 20);
    }
}
