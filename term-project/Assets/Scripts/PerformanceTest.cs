using UnityEngine;
using UnityEngine.Profiling;

public class PerformanceTest : MonoBehaviour
{
    private int testIndex = 0;
    private bool testRunning = false;
    private float startTime;
    private long startMemory;
    private int blockCount;

    // Test configurations: [blockCount, explosionRadius]
    private int[][] tests = new int[][]
    {
        new int[] { 10, 2 },   // Small castle, small explosion
        new int[] { 20, 3 },   // Medium castle, medium explosion
        new int[] { 30, 4 },   // Large castle, large explosion
        new int[] { 40, 5 },   // Very large castle, very large explosion
        new int[] { 50, 6 },   // Massive castle, massive explosion
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RunNextTest();
        }
    }

    private void RunNextTest()
    {
        if (testIndex >= tests.Length)
        {
            Debug.Log("=== ALL TESTS COMPLETE ===");
            testIndex = 0;
            return;
        }

        // Clean up
        var castle = GameObject.Find("TestCastle");
        if (castle != null) Destroy(castle);
        foreach (var p in FindObjectsOfType<Projectile>()) Destroy(p.gameObject);

        // Setup test
        blockCount = tests[testIndex][0];
        int explosionRadius = tests[testIndex][1];

        // Build test castle (simple stack)
        BuildTestCastle(blockCount);

        // Record baseline
        startMemory = System.GC.GetTotalMemory(false);
        startTime = Time.realtimeSinceStartup;

        // Trigger explosion after 0.5s
        Invoke(nameof(TriggerExplosion), 0.5f);
        Invoke(nameof(RecordResults), 1.0f);

        Debug.Log($"=== TEST {testIndex + 1}: {blockCount} blocks, {explosionRadius}m explosion ===");
        testIndex++;
    }

    private void BuildTestCastle(int count)
    {
        var castle = new GameObject("TestCastle");
        castle.transform.position = new Vector3(0, 0, 15);

        int perLayer = 4;
        int layers = Mathf.CeilToInt(count / (float)perLayer);

        for (int i = 0; i < count; i++)
        {
            int layer = i / perLayer;
            int posInLayer = i % perLayer;

            float x = (posInLayer % 2) * 1.02f - 0.5f;
            float z = (posInLayer / 2) * 1.02f - 0.5f;
            float y = 0.5f + layer * 1.02f;

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "TestBlock";
            go.transform.SetParent(castle.transform);
            go.transform.localPosition = new Vector3(x, y, z);
            go.transform.localScale = Vector3.one;
            go.GetComponent<Renderer>().material.color = Color.gray;

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 5f;
            rb.useGravity = true;
        }

        Debug.Log($"Built castle with {count} blocks");
    }

    private void TriggerExplosion()
    {
        var colliders = Physics.OverlapSphere(new Vector3(0, 1, 15), 5f);
        foreach (var col in colliders)
        {
            var rb = col.attachedRigidbody;
            if (rb != null && !rb.isKinematic)
            {
                rb.AddExplosionForce(500f, new Vector3(0, 1, 15), 5f);
            }
        }
        Debug.Log("Explosion triggered");
    }

    private void RecordResults()
    {
        float elapsed = Time.realtimeSinceStartup - startTime;
        long memoryUsed = (System.GC.GetTotalMemory(false) - startMemory) / 1024;

        int physicsObjects = FindObjectsOfType<Rigidbody>().Length;
        float fps = 1f / Time.deltaTime;

        Debug.Log($"RESULTS:");
        Debug.Log($"  Blocks: {blockCount}");
        Debug.Log($"  Physics objects: {physicsObjects}");
        Debug.Log($"  FPS during test: {fps:F1}");
        Debug.Log($"  GC Alloc: {memoryUsed} KB");
        Debug.Log($"  Time: {elapsed * 1000:F1} ms");
        Debug.Log("---");
    }
}
