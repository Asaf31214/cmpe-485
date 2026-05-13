using UnityEngine;

public class TrajectoryPreview : MonoBehaviour
{
    public int Dots = 120;
    public float TimeStep = 0.075f;

    private GameObject[] dots;
    private CannonController cannon;

    private void Start()
    {
        cannon = FindObjectOfType<CannonController>();
        CreatePreviewDots();
    }

    private void CreatePreviewDots()
    {
        dots = new GameObject[Dots];
        for (int i = 0; i < Dots; i++)
        {
            dots[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            dots[i].name = $"TrajectoryDot{i}";
            dots[i].transform.localScale = Vector3.one * 0.2f;
            dots[i].GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.4f);
            Destroy(dots[i].GetComponent<Collider>());
        }
    }

    private void Update()
    {
        Vector3 startPos = cannon.Muzzle.position;
        Vector3 vel = cannon.GetFireDirection() * cannon.Power;
        Vector3 gravity = Physics.gravity;

        for (int i = 0; i < Dots; i++)
        {
            float t = (i + 1) * TimeStep;
            Vector3 pos = startPos + vel * t + 0.5f * gravity * t * t;

            if (pos.y < 0)
            {
                dots[i].SetActive(false);
                continue;
            }

            dots[i].SetActive(true);
            dots[i].transform.position = pos;
        }
    }
}