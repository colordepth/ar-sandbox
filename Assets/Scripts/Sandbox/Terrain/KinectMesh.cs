using System;
using System.Collections.Generic;
using UnityEngine;

public class KinectMesh : TerrainMesh
{
    private KinectInterface iKinect;

    public Material defaultMat;
    private Menu gameMenu;

    private int width = -1;
    private int height = -1;

    public bool lockBoundRange = false;
    public int Width
    {
        get { return width; }
    }
    public int Height
    {
        get { return height; }
    }
    public float terrainAssistanceAccomodation = 5f;

    private float noiseClampMin = 500;
    private float noiseClampMax = 1500;

    public Color32 contourColor = Color.black;
    public Color32[] heightmapColors = { Color.blue, Color.green, Color.red, Color.white };

    [Range(0, 1)]
    public float contourThickness = .15f;
    private float DepthScalingFactor = -0.5f;
    private float DepthShiftingFactor = -900f;

    Vector3[] targetTerrain;
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] normals;
    Color32[] colors;
    Vector2[] uv;
    int[] triangles;

    [SerializeField]
    float minZ,
        maxZ;
    float targetMinZ,
        targetMaxZ;

    public List<Marker> markers;

    public GameObject prefab_FRIENDLY_SOLDIER;
    public GameObject prefab_FRIENDLY_TANK;
    public GameObject prefab_FRIENDLY_JET;
    public GameObject prefab_HOSTILE_SOLDIER;

    public int pointSize = 5;
    public Color32 pointColor = Color.magenta;
    List<Point> points;

    List<Vector3[]> lineMarkersList;
    public int lineSize = 5;
    public int numOfLines;
    public Color32 lineColor = Color.black;
    List<Line> lines;
    private int lineChange = 1;

    List<Vector3[]> polygonMarkersList;
    public int polygonSize = 2;
    public int numOfPolygons;
    public Color32 polygonColor = Color.cyan;
    List<Polygon> polygons;
    private int polyChange = 1;
    
    int wasVisualizing = 0;
    GameObject objects;

    void initializeMarkers() {
      markers = new List<Marker>();

      // *** WRITE GET REQUEST FUNCTIONALITY HERE ***

      // dummy points
      markers.Add(new Marker(1, 1, new Vector3(0.1f, 0.2f, 1f), Marker.Type.POINT));
      markers.Add(new Marker(2, 1, new Vector3(0.42f, 0.71f, 1f), Marker.Type.POINT));
      markers.Add(new Marker(3, 1, new Vector3(0.67f, 0.12f, 1f), Marker.Type.POINT));
      // dummy line 1
      markers.Add(new Marker(4, 1, new Vector3(0.2f, 0.6f, 1f), Marker.Type.LINE));
      markers.Add(new Marker(5, 1, new Vector3(0.4f, 0.1f, 1f), Marker.Type.LINE));
      markers.Add(new Marker(6, 1, new Vector3(0.5f, 0.5f, 1f), Marker.Type.LINE));
      markers.Add(new Marker(7, 1, new Vector3(0.9f, 0.35f, 1f), Marker.Type.LINE));
      // dummy line 2
      markers.Add(new Marker(8, 2, new Vector3(0.2f, 0.6f, 1f), Marker.Type.LINE));
      markers.Add(new Marker(9, 2, new Vector3(0.4f, 0.1f, 1f), Marker.Type.LINE));
      markers.Add(new Marker(10, 2, new Vector3(0.5f, 0.5f, 1f), Marker.Type.LINE));
      // dummy polygons
      markers.Add(new Marker(11, 1, new Vector3(0.15f, 0.8f, 1f), Marker.Type.POLYGON));
      markers.Add(new Marker(12, 1, new Vector3(0.3f, 0.5f, 1f), Marker.Type.POLYGON));
      markers.Add(new Marker(13, 1, new Vector3(0.65f, 0.6f, 1f), Marker.Type.POLYGON));
      markers.Add(new Marker(14, 1, new Vector3(0.85f, 0.9f, 1f), Marker.Type.POLYGON));
      // dummy friendly soldiers
      markers.Add(new Marker(15, 1, new Vector3(0.5f, 0.7f, 1f), Marker.Type.FRIENDLY_SOLDIER));
      markers.Add(new Marker(16, 1, new Vector3(0.55f, 0.63f, 1f), Marker.Type.FRIENDLY_SOLDIER));
      // dummy friendly tanks
      markers.Add(new Marker(17, 1, new Vector3(0.4f, 0.62f, 1f), Marker.Type.FRIENDLY_TANK));
      // dummy friendly jets
      markers.Add(new Marker(18, 1, new Vector3(0.3f, 0.6f, 1f), Marker.Type.FRIENDLY_JET));
      // dummy hostile soldiers
      markers.Add(new Marker(19, 1, new Vector3(0.8f, 0.45f, 1f), Marker.Type.HOSTILE_SOLDIER));

    }
    void initializeMarkerArrays () {
        points = new List<Point>();
        lines = new List<Line>();
        polygons = new List<Polygon>();
        lineMarkersList = new List<Vector3[]>();
        polygonMarkersList = new List<Vector3[]>();
    }
    void initializeShapes()
    {

        foreach (Marker marker in markers)
          if (marker.type == Marker.Type.POINT) 
            points.Add(new Point(marker.id, marker.location, pointSize, pointColor, Point.Type.CIRCLE));
        

        // for (int i = 0; i < numOfLines; i++)
        //   lineMarkersList.Add(new Vector3[4]());
        // int lgid = -1;
        // foreach (Vector3[] lineMarkers in lineMarkersList) {
        //   for (int i = 0; i < markers.Count; i++){
        //     Marker marker = markers[i];
        //     if (marker.type == Marker.Type.LINE) {
        //       lineMarkers[i] = marker.location;
        //       if (lgid == -1 && lgid != marker.groupID)
        //         lgid = marker.groupID;
        //     }
        //   }
        //   lines.Add(new Line(lgid, lineMarkers, lineSize, lineColor, Line.Type.SOLID));
        // }

        // for (int i = 0; i < numOfPolygons; i++)
        //   polygonMarkersList.Add(new Vector3[4]());
        // int pgid = -1;
        // foreach (Vector3[] polygonMarkers in polygonMarkersList) {
        //   for (int i = 0; i < markers.Count; i++){
        //     Marker marker = markers[i];
        //     if (marker.type == Marker.Type.POLYGON) {
        //       polygonMarkers[i] = marker.location;
        //       if (pgid == -1 && pgid != marker.groupID)
        //         pgid = marker.groupID;
        //     }
        //   }
        //   polygons.Add(new Polygon(pgid, polygonMarkers, polygonSize, polygonColor, Polygon.Type.SOLID));
        // }

        
    lines = new List<Line>();
    Vector3[] initHandlesLine =
    {
            new Vector3(0.2f, 0.6f, 0.5f),
            new Vector3(0.4f, 0.1f, 0.5f),
            new Vector3(0.5f, 0.5f, 0.5f),
            new Vector3(0.9f, 0.35f, 0.5f)
        };
    lines.Add(new Line(1, initHandlesLine, lineSize, lineColor, Line.Type.SOLID));

    polygons = new List<Polygon>();
    Vector3[] initHandlesPoly =
    {
            new Vector3(0.15f, 0.8f, 0.5f),
            new Vector3(0.3f, 0.5f, 0.5f),
            new Vector3(0.65f, 0.6f, 0.5f),
            new Vector3(0.85f, 0.9f, 0.5f)
        };
    polygons.Add(new Polygon(2, initHandlesPoly, polygonSize, polygonColor, Polygon.Type.SOLID));

    }

    void initializeMarkerObjects () {
        // objects = new GameObject("Objects");
        // GameObject sandboxObj = GameObject.Find("Sandbox");
        // objects.transform.SetParent(sandboxObj);
        // foreach (Marker marker in markers) {
        //     if (marker.Type == Marker.Type.FRIENDLY_SOLDIER || marker.Type == Marker.Type.FRIENDLY_JET || marker.Type == Marker.Type.FRIENDLY_TANK || marker.Type == Marker.Type.HOSTILE_SOLDIER) {
        //         GameObject unit = new GameObject("Unit");
        //         unit.transform.SetParent(objects);
        //     }
        // };
    }

    void destroyMarkerObjects() {
        Destroy(objects);
    }

    void Start()
    {
        initializeMarkers();
        initializeMarkerArrays();
        initializeShapes();

        gameMenu = GetComponent<Menu>();

        iKinect = GetComponentInParent<Sandbox>().GetComponentInChildren<KinectInterface>();
        if (!iKinect)
        {
            Debug.Log("KinectInterface not found!");
            return;
        }
    }

    void Update()
    {
        if (!iKinect)
            return;

        if (width == -1 || height == -1)
        {
            width = iKinect.FrameWidth;
            height = iKinect.FrameHeight;
            transform.position = new Vector3(-Width / 2f, -Height / 2f, 0);

            Debug.Log("Initializing mesh with resolution: " + width + " " + height);
            initializeMesh();
        }

        if (!iKinect.Poll())
            return;

        vertices[0].z = (noiseClampMin + noiseClampMax) / 2;
        for (int y = 0; y < Height; y++)
        {
            for (int x = 1; x < Width; x++)
            {
                int i = x + y * Width;
                int iMirrored = x + (Height - 1 - y) * Width;

                float z = iKinect.DepthData[iMirrored];

                if ((z <= noiseClampMin || z >= noiseClampMax))
                    z = vertices[i - 1].z;

                if ((z <= noiseClampMin || z >= noiseClampMax))
                    z = (noiseClampMin + noiseClampMax) / 2f; //vertices[i - 1].z;

                z += DepthShiftingFactor;
                z *= DepthScalingFactor;
                if (!lockBoundRange)
                {
                    minZ = Mathf.Min(z, minZ);
                    maxZ = Mathf.Max(z, maxZ);
                }
                // initialize terrain
                targetTerrain[i] = new Vector3(0, 0, minZ + 0.75f * (maxZ - minZ) / 2);
                float zTarget = targetTerrain[i].z;
                targetMinZ = Mathf.Min(zTarget, targetMinZ);
                targetMaxZ = Mathf.Max(zTarget, targetMaxZ);

                vertices[i].z = z;
                if (gameMenu.page == Menu.Page.ASSIST) terrainAssistance(ref colors, ref vertices, targetTerrain, i, Mathf.Min(targetMinZ, minZ), Mathf.Max(targetMaxZ, maxZ));
                else if (gameMenu.page == Menu.Page.DEPTH) colors[i] = getVertexColorFromGradient(z);
            }
        }
        if (gameMenu.isDrawing == 1) drawShapes(ref colors);

        if (gameMenu.isVisualizing == 0) {
            mesh.vertices = vertices;
            mesh.colors32 = colors;
            mesh.RecalculateNormals();
        }

        if (wasVisualizing == 0 && gameMenu.isVisualizing == 1) {
            initializeMarkerObjects();
        } else if (wasVisualizing == 1 && gameMenu.isVisualizing == 0) {
            destroyMarkerObjects();
        }
        wasVisualizing = gameMenu.isVisualizing;
    }

    void terrainAssistance(
        ref Color32[] colors,
        ref Vector3[] vertices,
        Vector3[] targetTerrain,
        int flattenedIndex,
        float minZ,
        float maxZ
    )
    {
        float terrainMin = minZ;
        float terrainMax = maxZ;

        float relativeHeight = (vertices[flattenedIndex].z - targetTerrain[flattenedIndex].z) * 5;

        if (
            relativeHeight < terrainAssistanceAccomodation
            && relativeHeight > -terrainAssistanceAccomodation
        )
        {
            colors[flattenedIndex] = Color.black;
        }
        else if (relativeHeight < 0)
        {
            colors[flattenedIndex] =
                Color.green * -(relativeHeight / (vertices[flattenedIndex].z - terrainMin));
        }
        else if (relativeHeight > 0)
        {
            colors[flattenedIndex] =
                Color.red * (relativeHeight / (terrainMax - vertices[flattenedIndex].z));
        }
        ;
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
        GameObject polygonParent = new GameObject("polygon", typeof(LineRenderer));
        polygonParent.transform.SetParent(gameObject.transform);
        GameObject polyPath = new GameObject("polygonPath", typeof(LineRenderer));
        polyPath.transform.SetParent(polygonParent.transform);
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
            renderer.SetPosition(
                i,
                new Vector3(
                    (polygon.handles[i].x * Width) - Width / 2,
                    300,
                    -polygon.handles[i].y * Height
                )
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
                (polygon.handles[i].x * Width) - Width / 2,
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
        GameObject area = new GameObject("polygonArea");
        area.transform.SetParent(polygonParent.transform);
        MeshFilter meshFilter = area.AddComponent<MeshFilter>();
        meshFilter.mesh = polygonMesh;
        polygonMesh.MarkDynamic();
        MeshRenderer mr = area.AddComponent<MeshRenderer>();
        mr.material = defaultMat;
    }

    void renderLinePath(Line line, ref Color32[] colors)
    {
        GameObject linePath = new GameObject("linePath", typeof(LineRenderer));
        linePath.transform.SetParent(gameObject.transform);
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
            renderPoint(new Point(i+100, (Vector3)line.handles[i], 5, line.color, Point.Type.CIRCLE), ref colors);
            // renderLineSegment(line.handles[i], line.handles[i + 1], line, ref colors);

            renderer.SetPosition(
                i,
                new Vector3(
                    (line.handles[i].x * Width) - Width / 2,
                    300,
                    -line.handles[i].y * Height
                )
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

        if (point.type == Point.Type.CIRCLE)
        {
            for (int ry = -point.size; ry < point.size && (y + ry < Height) && (y + ry > 0); ry++)
            for (int rx = -point.size; rx < point.size && (x + rx < Width) && (x + rx > 0); rx++)
                if ((rx * rx) + (ry * ry) <= point.size * point.size)
                    colors[(x + rx) + (y + ry) * Width] = point.color;
        }
        else if (point.type == Point.Type.SQUARE)
        {
            for (int ry = -point.size; ry < point.size && (y + ry < Height) && (y + ry > 0); ry++)
            for (int rx = -point.size; rx < point.size && (x + rx < Width) && (x + rx > 0); rx++)
                colors[(x + rx) + (y + ry) * Width] = point.color;
        }

        // add render code for different primitive shapes
    }

    void initializeMesh()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        targetTerrain = new Vector3[Width * Height];
        vertices = new Vector3[Width * Height];
        normals = new Vector3[Width * Height];
        colors = new Color32[Width * Height];
        uv = new Vector2[Width * Height];
        triangles = new int[(Width - 1) * (Height - 1) * 6];

        GetComponent<MeshFilter>().mesh = mesh;
        mesh.MarkDynamic();

        if (!lockBoundRange)
        {
            minZ = float.PositiveInfinity;
            maxZ = float.NegativeInfinity;
        }

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int i = x + y * Width;
                float z = (noiseClampMin + noiseClampMax) / 2f;
                z += DepthShiftingFactor;
                z *= DepthScalingFactor;

                vertices[i] = new Vector3(x, y, z);
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
        float normalizedZ = relativeZ / heightRange - 0.0001f; // Replace with heightRange/1000f?
        float colorIndexFloat = normalizedZ * nColors;
        int colorIndexInt = Mathf.Clamp((int)colorIndexFloat, 0, nColors - 1);

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
        int colorIndex = Mathf.Clamp((int)colorIndexFloat, 0, heightmapColors.Length - 2);
        return Color.Lerp(
            heightmapColors[colorIndex],
            heightmapColors[colorIndex + 1],
            colorIndexFloat - colorIndex
        );
    }
}
