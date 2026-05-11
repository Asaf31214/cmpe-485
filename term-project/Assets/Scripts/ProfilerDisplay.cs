using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class ProfilerDisplay : MonoBehaviour
{
    private Text text;
    private float updateInterval = 0.5f;
    private float timer;
    private int frameCount;
    private float fps;
    private long lastGCMemory;

    private void Start()
    {
        CreateDisplay();
        lastGCMemory = System.GC.GetTotalMemory(false);
    }

    private void CreateDisplay()
    {
        var canvas = new GameObject("ProfilerCanvas");
        var canvasGroup = canvas.AddComponent<Canvas>();
        canvasGroup.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<CanvasScaler>();
        canvas.AddComponent<GraphicRaycaster>();

        text = new GameObject("ProfilerText").AddComponent<Text>();
        text.transform.SetParent(canvas.transform, false);
        text.rectTransform.sizeDelta = new Vector2(400, 150);
        text.rectTransform.anchoredPosition = new Vector2(-10, -10);
        text.rectTransform.anchorMin = new Vector2(0, 1);
        text.rectTransform.anchorMax = new Vector2(0, 1);
        text.font = Font.CreateDynamicFontFromOSFont("Arial", 16) ?? Font.CreateDynamicFontFromOSFont("Liberation Sans", 16);
        text.color = Color.yellow;
        text.alignment = TextAnchor.UpperLeft;
    }

    private void Update()
    {
        frameCount++;
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            fps = frameCount / timer;
            frameCount = 0;
            timer = 0;

            long currentGCMemory = System.GC.GetTotalMemory(false);
            long gcAllocThisFrame = currentGCMemory - lastGCMemory;
            lastGCMemory = currentGCMemory;

            float physicsTime = Time.deltaTime; // Approximate physics time
            int physicsObjects = Object.FindObjectsOfType<Rigidbody>().Length;

            text.text = $"FPS: {fps:F1}\n" +
                       $"Physics Objects: {physicsObjects}\n" +
                       $"GC Alloc (this frame): {gcAllocThisFrame / 1024.0:F1} KB\n" +
                       $"Total Memory: {currentGCMemory / 1024.0 / 1024.0:F1} MB";
        }
    }
}
