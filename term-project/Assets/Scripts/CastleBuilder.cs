using UnityEngine;

public class CastleBuilder : MonoBehaviour
{
    private const float BlockSize = 1f;
    private const float BlockGap = 0.02f;
    private const float BaseY = 0.5f;

    // Level 1: 3D Pyramid Fortress
    // 4x4 base → 3x3 → 2x2 → 1 target on top
    public static GameObject BuildCastle(Vector3 center)
    {
        var castle = new GameObject("Castle");
        castle.transform.position = center;

        // Layer 0: 4x4 base
        CreateSquareLayer(castle.transform, 0, 4, 0);
        // Layer 1: 3x3
        CreateSquareLayer(castle.transform, 1, 3, 0);
        // Layer 2: 2x2
        CreateSquareLayer(castle.transform, 2, 2, 0);
        // Layer 3: 1x1 target on top
        CreateSquareLayer(castle.transform, 3, 1, 1);

        return castle;
    }

    private static void CreateSquareLayer(Transform parent, int layerIndex, int size, int targetCount)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        float offset = (size - 1) * (BlockSize + BlockGap) / 2f;
        int targetsLeft = targetCount;

        for (int z = 0; z < size; z++)
        {
            for (int x = 0; x < size; x++)
            {
                float posX = (x * (BlockSize + BlockGap)) - offset;
                float posZ = (z * (BlockSize + BlockGap)) - offset;
                bool makeTarget = targetsLeft > 0;
                if (makeTarget) targetsLeft--;
                CreateBlock(parent, new Vector3(posX, y, posZ), makeTarget);
            }
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
