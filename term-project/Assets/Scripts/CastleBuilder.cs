using UnityEngine;

public class CastleBuilder : MonoBehaviour
{
    private const float BlockSize = 1f;
    private const float BlockGap = 0.02f;
    private const float BaseY = 0.5f;

    public static GameObject BuildCastle(Vector3 center, int level = 0)
    {
        var castle = new GameObject("Castle");
        castle.transform.position = center;

        switch (level)
        {
            case 0: // Level 1: 3D Pyramid
                BuildLevel1Pyramid(castle.transform);
                break;
            case 1: // Level 2: Walled Compound
                BuildLevel2Compound(castle.transform);
                break;
            case 2: // Level 3: Multi-Tower Castle
                BuildLevel3MultiTower(castle.transform);
                break;
        }

        return castle;
    }

    // Level 1: 4x4 → 3x3 → 2x2 → 1 target
    private static void BuildLevel1Pyramid(Transform parent)
    {
        CreateSquareLayer(parent, 0, 4, 0);
        CreateSquareLayer(parent, 1, 3, 0);
        CreateSquareLayer(parent, 2, 2, 0);
        CreateSquareLayer(parent, 3, 1, 1);
    }

    // Level 2: 5x3 walled compound with inner tower
    private static void BuildLevel2Compound(Transform parent)
    {
        // Layer 0: 5x3 solid base
        CreateRectLayer(parent, 0, 5, 3, 0);
        // Layer 1: hollow walls (5x3)
        CreateHollowWalls(parent, 1, 5, 3, 0);
        // Layer 2: hollow walls
        CreateHollowWalls(parent, 2, 5, 3, 0);
        // Layer 3: 2x2 inner tower with 1 target
        CreateSquareLayer(parent, 3, 2, 1, offsetX: 0, offsetZ: 0);
        // Layer 4: 1x1 target on tower
        CreateSquareLayer(parent, 4, 1, 1, offsetX: 0, offsetZ: 0);
    }

    // Level 3: 4x4 base with 4 corner towers, each with 1 target
    private static void BuildLevel3MultiTower(Transform parent)
    {
        // Layer 0: 4x4 solid base
        CreateSquareLayer(parent, 0, 4, 0);
        // Layer 1: 4x4 solid + corner towers start
        CreateSquareLayer(parent, 1, 4, 0);
        CreateCornerTowers(parent, 1, 0);
        // Layer 2: corner towers
        CreateCornerTowers(parent, 2, 0);
        // Layer 3: corner towers
        CreateCornerTowers(parent, 3, 0);
        // Layer 4: corner towers with 4 targets (1 each)
        CreateCornerTowers(parent, 4, 4);
    }

    private static void CreateSquareLayer(Transform parent, int layerIndex, int size, int targetCount, float offsetX = 0, float offsetZ = 0)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        float offset = (size - 1) * (BlockSize + BlockGap) / 2f;
        int targetsLeft = targetCount;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                float posX = (x * (BlockSize + BlockGap)) - offset + offsetX;
                float posZ = (z * (BlockSize + BlockGap)) - offset + offsetZ;
                bool makeTarget = targetsLeft > 0;
                if (makeTarget) targetsLeft--;
                CreateBlock(parent, new Vector3(posX, y, posZ), makeTarget);
            }
        }
    }

    private static void CreateRectLayer(Transform parent, int layerIndex, int width, int depth, int targetCount)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        float offsetX = (width - 1) * (BlockSize + BlockGap) / 2f;
        float offsetZ = (depth - 1) * (BlockSize + BlockGap) / 2f;
        int targetsLeft = targetCount;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float posX = (x * (BlockSize + BlockGap)) - offsetX;
                float posZ = (z * (BlockSize + BlockGap)) - offsetZ;
                bool makeTarget = targetsLeft > 0;
                if (makeTarget) targetsLeft--;
                CreateBlock(parent, new Vector3(posX, y, posZ), makeTarget);
            }
        }
    }

    private static void CreateHollowWalls(Transform parent, int layerIndex, int width, int depth, int targetCount)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        float offsetX = (width - 1) * (BlockSize + BlockGap) / 2f;
        float offsetZ = (depth - 1) * (BlockSize + BlockGap) / 2f;
        int targetsLeft = targetCount;

        for (int z = 0; z < depth; z++)
        {
            for (int x = 0; x < width; x++)
            {
                // Only create blocks on edges (hollow center)
                if (x != 0 && x != width - 1 && z != 0 && z != depth - 1) continue;

                float posX = (x * (BlockSize + BlockGap)) - offsetX;
                float posZ = (z * (BlockSize + BlockGap)) - offsetZ;
                bool makeTarget = targetsLeft > 0;
                if (makeTarget) targetsLeft--;
                CreateBlock(parent, new Vector3(posX, y, posZ), makeTarget);
            }
        }
    }

    private static void CreateCornerTowers(Transform parent, int layerIndex, int targetCount)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        float spacing = 3f * (BlockSize + BlockGap);
        int targetsLeft = targetCount;

        // Four corners
        float[][] corners = new float[][]
        {
            new float[] { -spacing, -spacing },
            new float[] { -spacing, spacing },
            new float[] { spacing, -spacing },
            new float[] { spacing, spacing }
        };

        foreach (var corner in corners)
        {
            bool makeTarget = targetsLeft > 0;
            if (makeTarget) targetsLeft--;
            CreateBlock(parent, new Vector3(corner[0], y-1, corner[1]), makeTarget);
        }
    }

    private static void CreateBlock(Transform parent, Vector3 localPos, bool isTarget)
    {
        GameObject go;
        if (isTarget)
        {
            // Human-like target: thin cylinder standing upright
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Target";
            go.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Block";
            go.transform.localScale = new Vector3(BlockSize, BlockSize, BlockSize);
        }

        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;

        var mat = new Material(Shader.Find("Diffuse"));
        mat.color = isTarget ? Color.red : new Color(0.6f, 0.6f, 0.6f);
        go.GetComponent<Renderer>().material = mat;

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = isTarget ? 2f : 5f;
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        rb.sleepThreshold = 0.005f;
        rb.maxAngularVelocity = 5f;
        rb.drag = 0.1f;
        rb.angularDrag = 0.5f;
        
        // Targets start kinematic for stability, wake on explosion
        if (isTarget)
        {
            rb.isKinematic = true;
        }

        if (isTarget)
        {
            var destructible = go.AddComponent<Destructible>();
            destructible.IsTarget = true;
            destructible.Health = 50f;
        }
    }
}
