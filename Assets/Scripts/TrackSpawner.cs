
using UnityEngine;
using System.Collections.Generic;

public class TrackSpawner : MonoBehaviour
{
    public Transform player;
    public float segmentLength = 30f;
    public int segmentsAhead = 6;
    public float spawnDistance = 0f;

    readonly List<GameObject> segments = new List<GameObject>();

    void Start()
    {
        spawnDistance = 0f;

        for (int i = 0; i < segmentsAhead; i++)
        {
            SpawnSegment(i * segmentLength);
        }

        spawnDistance = segmentsAhead * segmentLength;
    }

    void Update()
    {
        if (player == null) return;

        while (player.position.z + segmentsAhead * segmentLength > spawnDistance)
        {
            SpawnSegment(spawnDistance);
            spawnDistance += segmentLength;
        }

        for (int i = segments.Count - 1; i >= 0; i--)
        {
            if (segments[i] != null &&
                segments[i].transform.position.z + segmentLength < player.position.z - 15f)
            {
                Destroy(segments[i]);
                segments.RemoveAt(i);
            }
        }
    }

    void SpawnSegment(float z)
    {
        GameObject root = new GameObject("CityRoadSegment");
        root.transform.position = new Vector3(0, 0, z);
        segments.Add(root);

        CreateCube(root.transform, "Road",
            new Vector3(0, -0.15f, 15f),
            new Vector3(10, 0.3f, segmentLength),
            new Color(0.18f, 0.18f, 0.2f));

        CreateCube(root.transform, "Left Sidewalk",
            new Vector3(-6f, 0f, 15f),
            new Vector3(2f, 0.5f, segmentLength),
            Color.gray);

        CreateCube(root.transform, "Right Sidewalk",
            new Vector3(6f, 0f, 15f),
            new Vector3(2f, 0.5f, segmentLength),
            Color.gray);

        for (int i = 0; i < 5; i++)
        {
            float zz = 4f + i * 6f;

            CreateCube(root.transform, "Lane Mark",
                new Vector3(0, 0.03f, zz),
                new Vector3(0.12f, 0.04f, 2.5f),
                Color.white);

            CreateBuilding(root.transform, -1, zz);
            CreateBuilding(root.transform, 1, zz);
        }

        // Random obstacle
        if (Random.value > 0.25f)
        {
            int lane = Random.Range(0, 3);
            float x = (lane - 1) * 3f;

            CreateCube(root.transform, "Traffic Barrier",
                new Vector3(x, 0.8f, Random.Range(8f, 25f)),
                new Vector3(1.5f, 1.6f, 1f),
                new Color(0.9f, 0.25f, 0.1f),
                "Obstacle");
        }

        // Coins
        int coinLane = Random.Range(0, 3);

        for (int c = 0; c < 3; c++)
        {
            float x = (coinLane - 1) * 3f;

            CreateCollectible(root.transform, "Coin",
                new Vector3(x, 1.3f, 5f + c * 2f),
                Color.yellow, "Coin");
        }

        // Gem
        if (Random.value > 0.45f)
        {
            int gemLane = Random.Range(0, 3);

            CreateCollectible(root.transform, "Red Gem",
                new Vector3((gemLane - 1) * 3f, 1.5f, 23f),
                Color.red, "Gem");
        }
    }

    void CreateBuilding(Transform parent, int side, float z)
    {
        float h = Random.Range(5f, 13f);
        float x = side * Random.Range(8f, 11f);

        GameObject b = GameObject.CreatePrimitive(PrimitiveType.Cube);
        b.name = "City Building";
        b.transform.SetParent(parent);
        b.transform.localPosition = new Vector3(x, h / 2f, z);
        b.transform.localScale = new Vector3(
            Random.Range(3f, 5f), h, 4f);

        b.GetComponent<Renderer>().material.color =
            new Color(Random.Range(.35f, .75f),
                      Random.Range(.4f, .8f),
                      Random.Range(.45f, .85f));

        Destroy(b.GetComponent<Collider>());
    }

    void CreateCube(Transform parent, string name,
        Vector3 localPos, Vector3 scale, Color color,
        string tag = "")
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);

        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = scale;

        obj.GetComponent<Renderer>().material.color = color;

        if (!string.IsNullOrEmpty(tag))
        {
            obj.tag = tag;
        }

        // Obstacles use trigger colliders
        if (tag == "Obstacle")
        {
            Collider col = obj.GetComponent<Collider>();
            col.isTrigger = true;
        }
    }

    void CreateCollectible(Transform parent, string name,
        Vector3 localPos, Color color, string tag)
    {
        GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);

        obj.name = name;
        obj.transform.SetParent(parent);
        obj.transform.localPosition = localPos;
        obj.transform.localScale = Vector3.one * 0.65f;

        obj.GetComponent<Renderer>().material.color = color;
        obj.tag = tag;

        obj.GetComponent<Collider>().isTrigger = true;

        Rigidbody rb = obj.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        obj.AddComponent<SpinCollectible>();
    }
}
