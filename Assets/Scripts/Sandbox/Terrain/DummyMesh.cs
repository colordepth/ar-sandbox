using System.Collections.Generic;
using UnityEngine;

public class DummyMesh : TerrainMesh
{
    public int Width = 640;
    public int Height = 480;

    public Material defaultMat;

    public Color32 contourColor = Color.black;
    public Color32[] heightmapColors = { Color.blue, Color.green, Color.red, Color.white };

    [Range(0, 1)]
    public float contourThickness = .15f;

    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Color32[] colors;
    Vector2[] uv;
    int[] triangles;

    float timestep = 0;
    public float timescale = 1f;

    [SerializeField]
    float minZ,
        maxZ;

    public int pointSize = 5;
    public Color32 pointColor = Color.magenta;
    List<Point> points;

    public int lineSize = 5;
    public Color32 lineColor = Color.black;
    List<Line> lines;
    private int lineChange = 1;

    public int polygonSize = 2;
    public Color32 polygonColor = Color.cyan;
    List<Polygon> polygons;
    private int polyChange = 1;

    void initializeShapes()
    {
        points = new List<Point>();
        points.Add(new Point(new Vector3(0.1f, 0.2f, 0.5f), pointSize, pointColor, "circle"));
        points.Add(new Point(new Vector3(0.42f, 0.71f, 0.5f), pointSize, pointColor, "square"));
        points.Add(new Point(new Vector3(0.67f, 0.12f, 0.5f), pointSize, pointColor, "circle"));

        lines = new List<Line>();
        Vector3[] initHandlesLine =
        {
            new Vector3(0.2f, 0.6f, 0.5f),
            new Vector3(0.4f, 0.1f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.9f, 0.35f, 0.5f)
        };
        lines.Add(new Line(initHandlesLine, lineSize, lineColor, "solid"));

        polygons = new List<Polygon>();
        Vector3[] initHandlesPoly =
        {
            new Vector3(0.15f, 0.8f, 0.5f),
            new Vector3(0.3f, 0.5f, 0.5f),
            new Vector3(0.65f, 0.6f, 0.5f),
            new Vector3(0.85f, 0.9f, 0.5f)
        };
        polygons.Add(new Polygon(initHandlesPoly, polygonSize, polygonColor, "solid"));
    }

    void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        // transform.position = new Vector3(-Width / 2f, -Height / 2f, 0);

        initializeShapes();

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

    void Update()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + y * Width;

                float z = // (new Vector2(x, y) - new Vector2(Width / 2f, Height / 2f)).magnitude*0.01f +
                    Mathf.PerlinNoise(
                        10f * x / (float)Width + timestep,
                        10f * y / (float)Height + timestep
                    )
                    * (float)Width
                    / 10f;

                minZ = Mathf.Min(z, minZ);
                maxZ = Mathf.Max(z, maxZ);

                vertices[i].z = z;
                colors[i] = getVertexColor(z);
            }
        }
        // updateMarkers();             // to be implemented
        drawShapes(ref colors);

        mesh.vertices = vertices;
        mesh.colors32 = colors;
        // mesh.RecalculateNormals();
        // mesh.RecalculateBounds();
        // mesh.RecalculateTangents();
        timestep += timescale * Time.smoothDeltaTime / 2f;
    }

    void drawShapes(ref Color32[] colors)
    {
        drawAllPolygons(ref colors);
        drawAllLines(ref colors);
        drawAllPoints(ref colors);
    }

    void drawAllPolygons(ref Color32[] colors)
    {
        if (polyChange == 1)
        {
            foreach (var polygon in polygons)
                renderPolygon(polygon, ref colors);
            polyChange = 0;
        }
    }

    void drawAllLines(ref Color32[] colors)
    {
        if (lineChange == 1)
        {
            foreach (var line in lines)
                renderLinePath(line, ref colors);
            lineChange = 0;
        }
    }

    void drawAllPoints(ref Color32[] colors)
    {
        foreach (var point in points)
            renderPoint(point, ref colors);
    }

    void renderPolygon(Polygon polygon, ref Color32[] colors)
    {
        GameObject polyPath = new GameObject("polyPath", typeof(LineRenderer));
        LineRenderer renderer = polyPath.GetComponent<LineRenderer>();
        renderer.material = defaultMat;
        renderer.positionCount = polygon.handles.Length;
        renderer.loop = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(polygon.color, 0.0f),
                new GradientColorKey(polygon.color, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        );
        renderer.colorGradient = gradient;
        renderer.widthMultiplier = polygon.size;
        for (int i = 0; i < polygon.handles.Length; i++)
        {
            // renderPoint(new Point((Vector3)polygon.handles[i], 5, polygon.color, "circle"), ref colors);
            renderer.SetPosition(
                i,
                new Vector3(polygon.handles[i].x * Width, 300, -polygon.handles[i].y * Height)
            );
        }
        ;

        int[] polygonTriangles = new int[(polygon.handles.Length - 2) * 6];
        for (int i = 1; i < polygon.handles.Length - 1; i++)
        {
            polygonTriangles[(i - 1) * 3 + 2] = i + 1;
            polygonTriangles[(i - 1) * 3 + 1] = i;
            polygonTriangles[(i - 1) * 3] = 0;
            polygonTriangles[(i - 1) * 3] = 0;
            polygonTriangles[(i - 1) * 3 + 1] = i;
            polygonTriangles[(i - 1) * 3 + 2] = i + 1;
        }
        ;
        Mesh polygonMesh = new Mesh();
        Vector3[] polyVerts = new Vector3[polygon.handles.Length];
        for (int i = 0; i < polygon.handles.Length; i++)
        {
            polyVerts[i] = new Vector3(
                polygon.handles[i].x * Width,
                301,
                -polygon.handles[i].y * Height
            );
        }
        ;
        polygonMesh.vertices = polyVerts;
        polygonMesh.triangles = polygonTriangles;
        Color32[] polyCols = new Color32[polygon.handles.Length];
        for (int i = 0; i < polygon.handles.Length; i++)
        {
            polyCols[i] = (new Color32(polygon.color.r, polygon.color.g, polygon.color.b, 130));
        }
        ;

        polygonMesh.colors32 = polyCols;
        GameObject area = new GameObject("polygonMeshObject");
        MeshFilter meshFilter = area.AddComponent<MeshFilter>();
        meshFilter.mesh = polygonMesh;
        polygonMesh.MarkDynamic();
        MeshRenderer mr = area.AddComponent<MeshRenderer>();
        mr.material = defaultMat;
    }

    void renderLinePath(Line line, ref Color32[] colors)
    {
        GameObject linePath = new GameObject("linePath", typeof(LineRenderer));
        LineRenderer renderer = linePath.GetComponent<LineRenderer>();
        renderer.material = defaultMat;
        renderer.positionCount = line.handles.Length;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[]
            {
                new GradientColorKey(line.color, 0.0f),
                new GradientColorKey(line.color, 1.0f)
            },
            new GradientAlphaKey[]
            {
                new GradientAlphaKey(1.0f, 0.0f),
                new GradientAlphaKey(1.0f, 1.0f)
            }
        );
        renderer.colorGradient = gradient;
        renderer.widthMultiplier = line.size;
        for (int i = 0; i < line.handles.Length; i++)
        {
            renderPoint(new Point((Vector3)line.handles[i], 5, line.color, "circle"), ref colors);
            // renderLineSegment(line.handles[i], line.handles[i + 1], line, ref colors);

            renderer.SetPosition(
                i,
                new Vector3(line.handles[i].x * Width, 300, -line.handles[i].y * Height)
            );
        }
        ;
    }

    int getIndex(float x, float y)
    {
        return ((int)(x + y * Width));
    }

    void renderPoint(Point point, ref Color32[] colors)
    {
        int x = (int)(point.location.x * Width); // assuming location of points will range from 0 - 1.
        int y = (int)(point.location.y * Height);
        int i = x + y * Width;

        colors[i] = point.color;

        if (point.type == "circle")
        {
            for (int ry = -point.size; ry < point.size && (y + ry < Height) && (y + ry > 0); ry++)
            for (int rx = -point.size; rx < point.size && (x + rx < Width) && (x + rx > 0); rx++)
                if ((rx * rx) + (ry * ry) <= point.size * point.size)
                    colors[(x + rx) + (y + ry) * Width] = point.color;
        }
        else if (point.type == "square")
        {
            for (int ry = -point.size; ry < point.size && (y + ry < Height) && (y + ry > 0); ry++)
            for (int rx = -point.size; rx < point.size && (x + rx < Width) && (x + rx > 0); rx++)
                colors[(x + rx) + (y + ry) * Width] = point.color;
        }

        // add render code for different primitive shapes
    }

    Color32 getVertexColor(float z)
    {
        int nColors = heightmapColors.Length;
        float heightRange = maxZ - minZ;
        float relativeZ = (z - minZ);
        float normalizedZ = relativeZ / heightRange - 0.0001f; // Replace with heightRange/1000f?
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
                decimalVal < contourThickness
                && // Check if we lie in the contour region
                colorIndexInt < nColors // Do not try to contour near top region
            )
                return contourColor;
        }

        // Heightmap coloring
        return heightmapColors[colorIndexInt];
    }

    Color32 getVertexColorFromGradient(float z)
    {
        float normalizedZ = (z - minZ) / (maxZ - minZ + 0.0001f); // = [0, 1)
        float colorIndexFloat = normalizedZ * (heightmapColors.Length - 1);
        int colorIndex = (int)colorIndexFloat;
        return Color.Lerp(
            heightmapColors[colorIndex],
            heightmapColors[colorIndex + 1],
            colorIndexFloat - colorIndex
        );
    }
}
