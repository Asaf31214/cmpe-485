using UnityEngine;
using System.Diagnostics;
using System.IO;

public static class PerformanceLogger
{
    private static Stopwatch stopwatch = new Stopwatch();
    private static int totalBlocks;
    private static string logPath = "performance_log.csv";
    private static bool headerWritten = false;

    public static void LogExplosionStart(int blockCount)
    {
        totalBlocks = blockCount;
        stopwatch.Restart();
    }

    public static void LogExplosionEnd(int blocksDestroyed)
    {
        stopwatch.Stop();
        long ms = stopwatch.ElapsedMilliseconds;

        float fps = 1f / Time.deltaTime;

        UnityEngine.Debug.Log($"[Explosion] Time: {ms}ms | FPS: {fps:F1} | Blocks: {blocksDestroyed}/{totalBlocks}");

        WriteToFile(totalBlocks, ms, fps, blocksDestroyed);
    }

    private static void WriteToFile(int totalBlocks, long ms, float fps, int blocksDestroyed)
    {
        if (!headerWritten)
        {
            File.WriteAllText(logPath, "blocks,explosion_ms,fps,blocks_destroyed\n");
            headerWritten = true;
        }

        string line = $"{totalBlocks},{ms},{fps:F1},{blocksDestroyed}\n";
        File.AppendAllText(logPath, line);
    }
}