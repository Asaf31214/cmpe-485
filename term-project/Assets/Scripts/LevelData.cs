using UnityEngine;

public static class LevelData
{
    public static int CurrentLevel { get; set; } = 0;
    public static int TotalLevels => Levels.Length;
    public static Vector3 CastlePosition = new Vector3(0, 0, 50);

    public static int MaxAmmo => Levels[CurrentLevel].maxAmmo;
    public static float CannonPowerMin => Levels[CurrentLevel].powerMin;
    public static float CannonPowerMax => Levels[CurrentLevel].powerMax;

    public static BlockType[,,] GetCurrentLayout() => Levels[CurrentLevel].layout;
    public static int GetTargetCount() => Levels[CurrentLevel].targetCount;
    public static bool IsValidLevel(int level) => level >= 0 && level < TotalLevels;

    private static readonly LevelConfig[] Levels = new LevelConfig[]
    {
        Level1_Gate(),
        Level2_StepPyramid(),
        Level3_Fortress(),
        Level4_TwinTowers(),
        Level5_Citadel()
    };

    private static LevelConfig Level1_Gate()
    {
        // "The Arch" - arch with target inside
        var layout = new BlockType[6, 6, 6];

        // Left pillar (blocks)
        for (int y = 0; y < 4; y++)
        {
            layout[1, y, 0] = BlockType.Block;
            layout[1, y, 1] = BlockType.Block;
            layout[2, y, 0] = BlockType.Block;
            layout[2, y, 1] = BlockType.Block;
        }

        // Right pillar (blocks)
        for (int y = 0; y < 4; y++)
        {
            layout[1, y, 4] = BlockType.Block;
            layout[1, y, 5] = BlockType.Block;
            layout[2, y, 4] = BlockType.Block;
            layout[2, y, 5] = BlockType.Block;
        }

        // Arch top (anchors - floating)
        layout[1, 4, 2] = BlockType.Anchor;
        layout[1, 4, 3] = BlockType.Anchor;
        layout[2, 4, 2] = BlockType.Anchor;
        layout[2, 4, 3] = BlockType.Anchor;

        // Platform for target inside arch
        layout[1, 0, 2] = BlockType.Block;
        layout[1, 0, 3] = BlockType.Block;
        layout[2, 0, 2] = BlockType.Block;
        layout[2, 0, 3] = BlockType.Block;

        // Target
        layout[1, 1, 2] = BlockType.Target;

        return new LevelConfig(layout, 1, 3, 20f, 50f);
    }

    private static LevelConfig Level2_StepPyramid()
    {
        // "Floating Platform" - anchor-supported platform with target on top
        var layout = new BlockType[8, 6, 8];

        // Ground blocks forming a ring
        for (int x = 0; x < 8; x++)
        {
            layout[0, 0, x] = BlockType.Block;
            layout[7, 0, x] = BlockType.Block;
        }
        for (int z = 1; z < 7; z++)
        {
            layout[z, 0, 0] = BlockType.Block;
            layout[z, 0, 7] = BlockType.Block;
        }

        // Corner anchor pillars (support the floating platform)
        layout[0, 1, 0] = BlockType.Anchor;
        layout[0, 2, 0] = BlockType.Anchor;
        layout[0, 3, 0] = BlockType.Anchor;

        layout[0, 1, 7] = BlockType.Anchor;
        layout[0, 2, 7] = BlockType.Anchor;
        layout[0, 3, 7] = BlockType.Anchor;

        layout[7, 1, 0] = BlockType.Anchor;
        layout[7, 2, 0] = BlockType.Anchor;
        layout[7, 3, 0] = BlockType.Anchor;

        layout[7, 1, 7] = BlockType.Anchor;
        layout[7, 2, 7] = BlockType.Anchor;
        layout[7, 3, 7] = BlockType.Anchor;

        // Floating platform (anchors at y=4)
        for (int x = 2; x < 6; x++)
        {
            for (int z = 2; z < 6; z++)
            {
                layout[z, 4, x] = BlockType.Anchor;
            }
        }

        // Blocks on platform
        for (int x = 2; x < 6; x++)
        {
            for (int z = 2; z < 6; z++)
            {
                layout[z, 5, x] = BlockType.Block;
            }
        }

        // Target at center
        layout[4, 5, 4] = BlockType.Target;

        return new LevelConfig(layout, 1, 4, 25f, 55f);
    }

    private static LevelConfig Level3_Fortress()
    {
        // "The Tower" - tower with anchor core, 2 targets
        var layout = new BlockType[5, 8, 5];

        // Anchor core (central pillar that holds everything)
        layout[2, 0, 2] = BlockType.Anchor;
        layout[2, 1, 2] = BlockType.Anchor;
        layout[2, 2, 2] = BlockType.Anchor;
        layout[2, 3, 2] = BlockType.Anchor;
        layout[2, 4, 2] = BlockType.Anchor;
        layout[2, 5, 2] = BlockType.Anchor;

        // Block shell around core (each layer)
        for (int y = 0; y < 6; y++)
        {
            layout[1, y, 1] = BlockType.Block;
            layout[1, y, 2] = BlockType.Block;
            layout[1, y, 3] = BlockType.Block;
            layout[2, y, 1] = BlockType.Block;
            layout[2, y, 3] = BlockType.Block;
            layout[3, y, 1] = BlockType.Block;
            layout[3, y, 2] = BlockType.Block;
            layout[3, y, 3] = BlockType.Block;
        }

        // Top platform (anchors)
        for (int z = 1; z < 4; z++)
        {
            for (int x = 1; x < 4; x++)
            {
                layout[z, 6, x] = BlockType.Anchor;
            }
        }

        // Targets on top
        layout[1, 7, 1] = BlockType.Target;
        layout[3, 7, 3] = BlockType.Target;

        return new LevelConfig(layout, 2, 5, 25f, 60f);
    }

    private static LevelConfig Level4_TwinTowers()
    {
        // "The Bridge" - two towers with anchor bridge between them
        var layout = new BlockType[6, 7, 10];

        // Left tower (blocks)
        for (int y = 0; y < 4; y++)
        {
            layout[1, y, 0] = BlockType.Block;
            layout[1, y, 1] = BlockType.Block;
            layout[2, y, 0] = BlockType.Block;
            layout[2, y, 1] = BlockType.Block;
        }

        // Right tower (blocks)
        for (int y = 0; y < 4; y++)
        {
            layout[1, y, 8] = BlockType.Block;
            layout[1, y, 9] = BlockType.Block;
            layout[2, y, 8] = BlockType.Block;
            layout[2, y, 9] = BlockType.Block;
        }

        // Anchor bridge connecting towers (floating)
        for (int x = 2; x < 8; x++)
        {
            layout[1, 4, x] = BlockType.Anchor;
            layout[2, 4, x] = BlockType.Anchor;
        }

        // Blocks on bridge
        for (int x = 3; x < 7; x++)
        {
            layout[1, 5, x] = BlockType.Block;
            layout[2, 5, x] = BlockType.Block;
        }

        // Targets on bridge
        layout[1, 6, 3] = BlockType.Target;
        layout[2, 6, 6] = BlockType.Target;

        return new LevelConfig(layout, 2, 5, 25f, 65f);
    }

    private static LevelConfig Level5_Citadel()
    {
        // "The Citadel" - fortress with floating roof sections, 3 targets
        var layout = new BlockType[8, 7, 10];

        // Outer walls (blocks)
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 10; x++) layout[0, y, x] = BlockType.Block;
            for (int x = 0; x < 10; x++) layout[7, y, x] = BlockType.Block;
            for (int z = 1; z < 7; z++) layout[z, y, 0] = BlockType.Block;
            for (int z = 1; z < 7; z++) layout[z, y, 9] = BlockType.Block;
        }

        // Anchor pillars at corners (support floating roof)
        layout[0, 3, 0] = BlockType.Anchor;
        layout[0, 4, 0] = BlockType.Anchor;
        layout[7, 3, 0] = BlockType.Anchor;
        layout[7, 4, 0] = BlockType.Anchor;
        layout[0, 3, 9] = BlockType.Anchor;
        layout[0, 4, 9] = BlockType.Anchor;
        layout[7, 3, 9] = BlockType.Anchor;
        layout[7, 4, 9] = BlockType.Anchor;

        // Floating roof (anchors spanning walls)
        for (int x = 1; x < 9; x++)
        {
            layout[0, 5, x] = BlockType.Anchor;
            layout[7, 5, x] = BlockType.Anchor;
        }
        for (int z = 1; z < 7; z++)
        {
            layout[z, 5, 1] = BlockType.Anchor;
            layout[z, 5, 8] = BlockType.Anchor;
        }

        // Inner blocks on roof
        layout[2, 6, 2] = BlockType.Block;
        layout[2, 6, 3] = BlockType.Block;
        layout[5, 6, 6] = BlockType.Block;
        layout[5, 6, 7] = BlockType.Block;

        // Targets on floating roof
        layout[2, 6, 4] = BlockType.Target;
        layout[5, 6, 5] = BlockType.Target;

        // Central block tower inside
        layout[3, 0, 4] = BlockType.Block;
        layout[3, 0, 5] = BlockType.Block;
        layout[4, 0, 4] = BlockType.Block;
        layout[4, 0, 5] = BlockType.Block;
        layout[3, 1, 4] = BlockType.Block;
        layout[3, 1, 5] = BlockType.Block;
        layout[4, 1, 4] = BlockType.Block;
        layout[4, 1, 5] = BlockType.Block;
        layout[3, 2, 4] = BlockType.Target;

        return new LevelConfig(layout, 3, 6, 30f, 70f);
    }

    private static void FillRect(BlockType[,,] layout, int y, int zStart, int xStart, int depth, int width, BlockType type)
    {
        for (int z = zStart; z < zStart + depth; z++)
        {
            for (int x = xStart; x < xStart + width; x++)
            {
                layout[z, y, x] = type;
            }
        }
    }

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