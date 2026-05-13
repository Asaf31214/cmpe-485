using UnityEngine;
using System.Diagnostics;
using System.IO;

public static class PerformanceLogger
{
    private static Stopwatch stopwatch = new Stopwatch();
    private static int totalBlocks;

    public static void LogExplosionStart(int blockCount)
    {
        totalBlocks = blockCount;
        stopwatch.Restart();
    }

    public static void LogExplosionEnd(int blocksDestroyed, string method)
    {
        stopwatch.Stop();
        long ms = stopwatch.ElapsedMilliseconds;

        float fps = 1f / Time.deltaTime;

        UnityEngine.Debug.Log($"[{method}] Time: {ms}ms | FPS: {fps:F1} | Blocks: {blocksDestroyed}/{totalBlocks}");

        string path = method == "optimized" ? "performance_log_optimized.csv" : "performance_log.csv";

        if (!File.Exists(path))
            File.WriteAllText(path, "blocks,explosion_ms,fps,blocks_destroyed\n");

        string line = $"{totalBlocks},{ms},{fps:F1},{blocksDestroyed}\n";
        File.AppendAllText(path, line);
    }
}