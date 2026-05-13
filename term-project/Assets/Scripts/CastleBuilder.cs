using UnityEngine;

public enum BlockType { Empty = 0, Block = 1, Target = 2 }

public static class CastleBuilder
{
    private const float BlockSize = 1f;
    private const float BlockGap = 0.02f;
    private const float TargetHeight = 1f;

    public static GameObject BuildCastle(Vector3 center, BlockType[,,] layout)
    {
        var castle = new GameObject("Castle");
        castle.transform.position = center;

        int depth = layout.GetLength(0);
        int height = layout.GetLength(1);
        int width = layout.GetLength(2);

        float cellSize = BlockSize + BlockGap;
        float offsetX = (width - 1) * cellSize / 2f;
        float offsetZ = (depth - 1) * cellSize / 2f;

        for (int y = 0; y < height; y++)
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    var type = layout[z, y, x];
                    if (type == BlockType.Empty) continue;

                    float posX = x * cellSize - offsetX;
                    float posZ = z * cellSize - offsetZ;
                    float posY = y * cellSize + (type == BlockType.Target ? TargetHeight / 2f : BlockSize / 2f);

                    CreateBlock(castle.transform, new Vector3(posX, posY, posZ), type == BlockType.Target);
                }
            }
        }

        return castle;
    }

    private static void CreateBlock(Transform parent, Vector3 localPos, bool isTarget)
    {
        GameObject go;

        if (isTarget)
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = "Target";
            go.transform.localScale = new Vector3(0.4f, TargetHeight / 2f, 0.4f);
        }
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Block";
            go.transform.localScale = Vector3.one * BlockSize;
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
        rb.isKinematic = isTarget;

        if (isTarget)
        {
            var d = go.AddComponent<Destructible>();
            d.IsTarget = true;
            d.Health = 50f;
        }
    }
}