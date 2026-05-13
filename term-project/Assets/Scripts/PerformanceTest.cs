#if UNITY_EDITOR
using UnityEngine;

public class PerformanceTest : MonoBehaviour
{
    private int testIndex = 0;
    private float startTime;
    private long startMemory;
    private int blockCount;

    private readonly int[][] tests = new int[][]
    {
        new int[] { 10, 2 },
        new int[] { 20, 3 },
        new int[] { 30, 4 },
        new int[] { 40, 5 },
        new int[] { 50, 6 },
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

        Cleanup();
        SetupTest();
        Invoke(nameof(TriggerExplosion), 0.5f);
        Invoke(nameof(RecordResults), 1.0f);

        testIndex++;
    }

    private void Cleanup()
    {
        var castle = GameObject.Find("TestCastle");
        if (castle != null) Destroy(castle);
        foreach (var p in FindObjectsOfType<Projectile>()) Destroy(p.gameObject);
    }

    private void SetupTest()
    {
        blockCount = tests[testIndex][0];
        BuildTestCastle(blockCount);
        startMemory = System.GC.GetTotalMemory(false);
        startTime = Time.realtimeSinceStartup;
        Debug.Log($"=== TEST {testIndex + 1}: {blockCount} blocks ===");
    }

    private void BuildTestCastle(int count)
    {
        var castle = new GameObject("TestCastle");
        castle.transform.position = new Vector3(0, 0, 15);

        int perLayer = 4;

        for (int i = 0; i < count; i++)
        {
            int layer = i / perLayer;
            int posInLayer = i % perLayer;

            float x = (posInLayer % 2) * 1.02f - 0.5f;
            float z = (posInLayer / 2) * 1.02f - 0.5f;
            float y = 0.5f + layer * 1.02f;

            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.transform.SetParent(castle.transform);
            go.transform.localPosition = new Vector3(x, y, z);
            go.GetComponent<Renderer>().material.color = Color.gray;

            var rb = go.AddComponent<Rigidbody>();
            rb.mass = 5f;
            rb.useGravity = true;
        }
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

        Debug.Log($"RESULTS: Blocks={blockCount}, Physics={physicsObjects}, FPS={fps:F1}, GC={memoryUsed}KB, Time={elapsed*1000:F1}ms");
    }
}
#endif