using UnityEngine;

public class KinectMesh : TerrainMesh
{
    private KinectInterface iKinect;

    private int width;
    private int height;

    public int Width { get { return width; } }
    public int Height { get { return height; } }

    public Color32 contourColor = Color.black;
    public Color32[] heightmapColors = {
        Color.blue,
        Color.green,
        Color.red,
        Color.white
    };

    [Range(0, 1)]
    public float contourThickness = .15f;
    private float DepthScalingFactor = 1 / 8000f;

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Color32[] colors;
    Vector2[] uv;
    int[] triangles;

    [SerializeField]
    float minZ, maxZ;

    void Start()
    {
        iKinect = GetComponentInParent<KinectInterface>();

        width = iKinect.FrameWidth;
        height = iKinect.FrameHeight;
        transform.position = new Vector3(-Width / 2f, -Height / 2f, 0);

        initializeMesh();
    }

    void Update()
    {
        if (!iKinect.Poll())
            return;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + y * Width;
                float z = iKinect.DepthData[i] * DepthScalingFactor;

                minZ = Mathf.Min(z, minZ);
                maxZ = Mathf.Max(z, maxZ);

                vertices[i].z = z;
                colors[i] = getVertexColor(z);
            }
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;
    }

    void initializeMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices = new Vector3[Width * Height];
        normals = new Vector3[Width * Height];
        colors = new Color32[Width * Height];
        uv = new Vector2[Width * Height];
        triangles = new int[(Width - 1) * (Height - 1) * 6];

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.MarkDynamic();

        minZ = float.PositiveInfinity;
        maxZ = float.NegativeInfinity;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + y * Width;

                vertices[i] = new Vector3(x, y, 0);
                normals[i] = new Vector3(0, 0, 1);
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
        float colorIndexFloat = normalizedZ * (heightmapColors.Length - 1);
        int colorIndex = (int)colorIndexFloat;
        return Color.Lerp(heightmapColors[colorIndex], heightmapColors[colorIndex + 1], colorIndexFloat - colorIndex);
    }
}
