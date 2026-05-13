using UnityEngine;

public static class LevelData
{
    public static int CurrentLevel { get; set; } = 0;
    public static int TotalLevels => Levels.Length;
    public static Vector3 CastlePosition = new Vector3(0, 0, 50);

    public static int MaxAmmo => CurrentLevelData.maxAmmo;
    public static float CannonPowerMin => CurrentLevelData.powerMin;
    public static float CannonPowerMax => CurrentLevelData.powerMax;

    private static LevelConfig CurrentLevelData => Levels[CurrentLevel];

    public static BlockType[,,] GetCurrentLayout()
    {
        return Levels[CurrentLevel].layout;
    }

    public static int GetTargetCount()
    {
        return Levels[CurrentLevel].targetCount;
    }

    public static bool IsValidLevel(int level) => level >= 0 && level < TotalLevels;

    private static readonly LevelConfig[] Levels = new LevelConfig[]
    {
        // Level 1: Pyramid (4x4 -> 3x3 -> 2x2 -> 1 target)
        new LevelConfig(
            layout: new BlockType[,,]
            {
                // Layer 0 (4x4 base)
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 1 (3x3)
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Empty },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Empty },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                // Layer 2 (2x2)
                {
                    { BlockType.Block, BlockType.Block, BlockType.Empty, BlockType.Empty },
                    { BlockType.Block, BlockType.Block, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                // Layer 3 (target)
                {
                    { BlockType.Target, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                }
            },
            targetCount: 1,
            maxAmmo: 5,
            powerMin: 20f,
            powerMax: 60f
        ),

        // Level 2: Walled compound with inner tower
        new LevelConfig(
            layout: new BlockType[,,]
            {
                // Layer 0: 5x3 solid base
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 1: hollow walls
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 2: hollow walls
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 3: 2x2 inner tower
                {
                    { BlockType.Empty, BlockType.Block, BlockType.Block, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Block, BlockType.Block, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                },
                // Layer 4: target
                {
                    { BlockType.Empty, BlockType.Target, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty }
                }
            },
            targetCount: 1,
            maxAmmo: 5,
            powerMin: 20f,
            powerMax: 60f
        ),

        // Level 3: Multi-tower castle with 4 targets
        new LevelConfig(
            layout: new BlockType[,,]
            {
                // Layer 0: 4x4 base
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 1: base + corner pillars
                {
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block },
                    { BlockType.Block, BlockType.Block, BlockType.Block, BlockType.Block }
                },
                // Layer 2-4: corner towers (expanded layout)
                {
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block }
                },
                {
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block }
                },
                {
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Block, BlockType.Empty, BlockType.Empty, BlockType.Block }
                },
                // Layer 5: targets on towers
                {
                    { BlockType.Target, BlockType.Empty, BlockType.Empty, BlockType.Target },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Empty, BlockType.Empty, BlockType.Empty, BlockType.Empty },
                    { BlockType.Target, BlockType.Empty, BlockType.Empty, BlockType.Target }
                }
            },
            targetCount: 4,
            maxAmmo: 6,
            powerMin: 25f,
            powerMax: 70f
        )
    };

    private struct LevelConfig
    {
        public BlockType[,,] layout;
        public int targetCount;
        public int maxAmmo;
        public float powerMin;
        public float powerMax;

        public LevelConfig(BlockType[,,] layout, int targetCount, int maxAmmo, float powerMin, float powerMax)
        {
            this.layout = layout;
            this.targetCount = targetCount;
            this.maxAmmo = maxAmmo;
            this.powerMin = powerMin;
            this.powerMax = powerMax;
        }
    }
}