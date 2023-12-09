using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DummyMesh : MonoBehaviour
{
    public int Width = 640;
    public int Height = 480;

    public Color32 contourColor = Color.black;
    public Color32[] heightmapColors = {
        Color.blue,
        Color.green,
        Color.red,
        Color.white
    };

    [Range(0, 1)]
    public float contourThickness = .15f;

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Color32[] colors;
    Vector2[] uv;
    int[] triangles;

    float timestep = 0;

    [SerializeField]
    float minZ, maxZ;

    void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        transform.position = new Vector3(-Width / 2f, -Height / 2f, 0);

        vertices = new Vector3[Width * Height];
        normals = new Vector3[Width * Height];
        colors = new Color32[Width * Height];
        uv = new Vector2[Width * Height];
        triangles = new int[(Width - 1) * (Height - 1) * 6];

        GetComponent<MeshFilter>().mesh = mesh;

        minZ = float.PositiveInfinity;
        maxZ = float.NegativeInfinity;
    }

    void Update()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + y * Width;
                float z = (new Vector2(x, y) - new Vector2(Width / 2f, Height / 2f)).magnitude * 0 +
                          Mathf.PerlinNoise(10f * x / (float)Width + timestep, 10f * y / (float)Height + timestep) * (float)Width / 10f;

                minZ = Mathf.Min(z, minZ);
                maxZ = Mathf.Max(z, maxZ);

                vertices[i] = new Vector3(x, y, z);
                normals[i] = new Vector3(0, 0, 1);
                colors[i] = getVertexColor(z);
                uv[i] = new Vector2(x / (float)Width, y / (float)Height);

                if (x < Width - 1 && y < Height - 1)
                {
                    int topleft = i;
                    int topright = i + 1;
                    int bottomleft = topleft + Width;
                    int bottomright = topright + Width;

                    int triangleindex = (x + y * (Width - 1)) * 6;
                    triangles[triangleindex + 0] = topleft;
                    triangles[triangleindex + 1] = bottomleft;
                    triangles[triangleindex + 2] = topright;
                    triangles[triangleindex + 3] = bottomleft;
                    triangles[triangleindex + 4] = bottomright;
                    triangles[triangleindex + 5] = topright;
                }
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.colors32 = colors;
        // mesh.RecalculateNormals();
        // mesh.RecalculateBounds();
        // mesh.RecalculateTangents();
        timestep += Time.smoothDeltaTime / 2f;
    }

    Color32 getVertexColor(float z)
    {
        int nColors = heightmapColors.Length;
        float heightRange = maxZ - minZ;
        float relativeZ = (z - minZ);
        float normalizedZ = relativeZ / heightRange - 0.0001f;  // Replace with heightRange/1000f?
        float colorIndexFloat = normalizedZ * nColors;
        int colorIndexInt = (int)colorIndexFloat;

        // Undefined. Usually because the global minZ, maxZ is undetermined.
        if (float.IsNaN(colorIndexFloat))
            return heightmapColors[0];

        // Contour coloring
        {
            // Normalized thickness of each individual level (ignoring contour lines)
            float decimalVal = colorIndexFloat - colorIndexInt;

            if (
                decimalVal < contourThickness &&    // Check if we lie in the contour region
                colorIndexInt < nColors             // Do not try to contour near top region
               )
                return contourColor;
        }

        // Heightmap coloring
        return heightmapColors[colorIndexInt];
    }

    Color32 getVertexColorFromGradient(float z)
    {
        float normalizedZ = (z - minZ) / (maxZ - minZ + 0.0001f);    // = [0, 1)
        float colorIndexFloat = normalizedZ * (heightmapColors.Length-1);
        int colorIndex = (int)colorIndexFloat;
        return Color.Lerp(heightmapColors[colorIndex], heightmapColors[colorIndex + 1], colorIndexFloat - colorIndex);
    }
}
