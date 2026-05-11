using UnityEngine;

public class CastleBuilder : MonoBehaviour
{
    private const float BlockSize = 1f;
    private const float BlockGap = 0.02f;
    private const float BaseY = 0.5f;

    public static GameObject BuildCastle(Vector3 center)
    {
        var castle = new GameObject("Castle");
        castle.transform.position = center;

        // Layer 0: 4x1 wide wall
        CreateLayer(castle.transform, 0, 4, 0, isTarget: false);
        // Layer 1: 3 blocks
        CreateLayer(castle.transform, 1, 3, 0, isTarget: false);
        // Layer 2: 2 blocks
        CreateLayer(castle.transform, 2, 2, 0, isTarget: false);
        // Layer 3: 3 blocks with 2 targets
        CreateLayer(castle.transform, 3, 3, targetCount: 2);
        // Layer 4: 1 target on top
        CreateLayer(castle.transform, 4, 1, targetCount: 1);

        return castle;
    }

    private static void CreateLayer(Transform parent, int layerIndex, int count, int targetCount = 0, bool isTarget = false)
    {
        float y = BaseY + layerIndex * (BlockSize + BlockGap);
        int targetsLeft = targetCount;
        for (int i = 0; i < count; i++)
        {
            float x = (i - (count - 1) / 2f) * (BlockSize + BlockGap);
            bool makeTarget = targetsLeft > 0 && (i == 0 || i == count - 1 || targetsLeft == 1 && count == 1);
            if (makeTarget) targetsLeft--;
            CreateBlock(parent, new Vector3(x, y, 0), makeTarget);
        }
    }

    private static void CreateBlock(Transform parent, Vector3 localPos, bool isTarget)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = isTarget ? "Target" : "Block";
        go.transform.SetParent(parent);
        go.transform.localPosition = localPos;
        go.transform.localScale = new Vector3(BlockSize, BlockSize, BlockSize);

        var mat = new Material(Shader.Find("Diffuse"));
        mat.color = isTarget ? Color.red : new Color(0.6f, 0.6f, 0.6f);
        go.GetComponent<Renderer>().material = mat;

        var rb = go.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.useGravity = true;

        var destructible = go.AddComponent<Destructible>();
        destructible.IsTarget = isTarget;
        destructible.Health = isTarget ? 50f : 30f;
    }
}
