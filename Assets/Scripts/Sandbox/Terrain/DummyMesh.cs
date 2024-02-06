using System.Collections.Generic;
using UnityEngine;

public class DummyMesh : TerrainMesh
{
    public int Width = 640;
    public int Height = 480;

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
    public Color32 pointColor = Color.black;
    List<Point> points;

    public int lineSize = 5;
    public Color32 lineColor = Color.black;
    List<Line> lines;

    public int polygonSize = 5;
    public Color32 polygonColor = Color.black;
    List<Polygon> polygons;

    void initializeShapes()
    {
        points = new List<Point>();
        points.Add(new Point(new Vector3(0.1f, 0.2f, 0.5f), pointSize, pointColor, "circle"));

        lines = new List<Line>();
        Vector3[] initHandles =
        {
            new Vector3(0.2f, 0.6f, 0.5f),
            new Vector3(0.4f, 0.1f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.9f, 0.35f, 0.5f)
        };
        lines.Add(new Line(initHandles, lineSize, lineColor, "solid"));

        polygons = new List<Polygon>();
    }

    void Start()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        transform.position = new Vector3(-Width / 2f, -Height / 2f, 0);

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
        // colors[i] = drawAllPolygons();
        drawAllLines(ref colors);
        drawAllPoints(ref colors);
    }

    void drawAllPolygons(ref Color32[] colors) { }

    void drawAllLines(ref Color32[] colors)
    {
        foreach (var line in lines)
            renderLinePath(line, ref colors);
    }

    void drawAllPoints(ref Color32[] colors)
    {
        foreach (var point in points)
            renderPoint(point, ref colors);
    }

    void renderLinePath(Line line, ref Color32[] colors)
    {
        for (int i = 0; i < line.handles.Length - 1; i++)
        {
            renderPoint(new Point((Vector3)line.handles[i], 5, Color.red, "circle"), ref colors);
            renderLineSegment(line.handles[i], line.handles[i + 1], line, ref colors);
        }
    }

    void renderLineSegment(Vector3 A, Vector3 B, Line line, ref Color32[] colors)
    {
        Vector2 ptA = new Vector2(Mathf.Round(A.x * Width), Mathf.Round(A.y * Height));
        Vector2 ptB = new Vector2(Mathf.Round(B.x * Width), Mathf.Round(B.y * Height));

        Vector2 left;
        Vector2 right;
        if (ptA.x < ptB.x)
        {
            left = ptA;
            right = ptB;
        }
        else
        {
            left = ptB;
            right = ptA;
        }

        // line segment of size(width) 1
        colors[getIndex(left.x, left.y)] = line.color;
        int prevY = (int)left.y;
        for (int x = (int)left.x + 1; x < (int)left.x; x++)
        {
            if (gradientChange(left, right, x, (float)(prevY + 0.5)) >= 0)
            {
                colors[getIndex(x, prevY + 1)] = line.color;
                prevY++;
            }
            else
            {
                colors[getIndex(x, prevY)] = line.color;
            }
        }
    }

    int getIndex(float x, float y)
    {
        return ((int)(x + y * Width));
    }

    float gradientChange(Vector2 left, Vector2 right, float x, float y)
    {
        float slope = (right.y - left.y) / (right.x - left.x);
        if (slope >= 0)
            return left.y + slope * (x - left.x) - y;
        else
            return -(left.y + slope * (x - left.x) - y);
    }

    void renderPoint(Point point, ref Color32[] colors)
    {
        int x = (int)(point.location.x * Width); // assuming location of points will range from 0 - 1.
        int y = (int)(point.location.y * Height);
        int i = x + y * Width;

        colors[i] = point.color;

        if (point.type == "circle")
        {
            for (int ry = -point.size; ry < point.size; ry++)
            for (int rx = -point.size; rx < point.size; rx++)
                if ((rx * rx) + (ry * ry) <= point.size * point.size)
                    colors[(x + rx) + (y + ry) * Width] = point.color;
        }
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
