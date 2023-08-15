
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public class MapEditor : EditorWindow
{
    //input
    string filePath = "Assets/Maps/Map1";
    Texture2D rawMap;
    Material outlineMaterial;
    Material highlightMaterial;

    Color defaultColor = Color.grey;
    //


    //output
    bool[,,] mapArray;// [x,y,a] a = 0 is Checked , a = 1 is Painted
    Texture2D map;
    Transform mapParent;
    MapStats mapStats;

    int maxX;
    int maxY;
    int minX;
    int minY;

    int number = 0;
    Mesh mesh;


    [MenuItem("Window/Map Editor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditor>("Map Editor");
    }

   

    private void OnGUI()
    {
        outlineMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Graphics/Shaders/Outline.mat");
        highlightMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Graphics/Shaders/Highlight.mat");

        Object obj =  EditorGUILayout.ObjectField(rawMap, typeof(Texture2D),false);

        Object obj2 =  EditorGUILayout.ObjectField(outlineMaterial, typeof(Material),false);

        Object obj3 =  EditorGUILayout.ObjectField(highlightMaterial, typeof(Material),false);

        defaultColor = EditorGUILayout.ColorField(defaultColor);


        GUILayout.Box(rawMap);
        EditorGUILayout.Space(10);
        filePath = EditorGUILayout.TextField("File Path", filePath);


        highlightMaterial = obj3 as Material;
        outlineMaterial = obj2 as Material; 
        rawMap = obj as Texture2D;
        EditorGUILayout.Space(30);
        if (GUILayout.Button("Cutting Map")) CuttingMap();
        EditorGUILayout.Space(10);

        if (mapParent != null)
        {
            SetUpProvinces();  
        }


    }

    int provinceNumber = 0;
    private void SetUpProvinces()
    {
        EditorGUILayout.LabelField(mapParent.GetChild(provinceNumber).name);

        if (GUILayout.Button("Next"))
        {
            SpriteRenderer spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            spriteRenderer.material = outlineMaterial;
            spriteRenderer.sortingOrder = 0;

            if (provinceNumber < mapParent.childCount - 1) provinceNumber++;
            else provinceNumber = 0;
            SetUpProvinces();

            spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            spriteRenderer.material = highlightMaterial;
            spriteRenderer.sortingOrder = 1;
        }

        if (GUILayout.Button("Back"))
        {
            SpriteRenderer spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            spriteRenderer.material = outlineMaterial;
            spriteRenderer.sortingOrder = 0;

            if (provinceNumber > 0) provinceNumber--;
            else provinceNumber = mapParent.childCount -1;
            SetUpProvinces();

            spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            spriteRenderer.material = highlightMaterial;
            spriteRenderer.sortingOrder = 1;
        }

        if (GUILayout.Button("Set Neighbors"))
        {
            mapStats.provinces[provinceNumber].SetUp(Selection.objects);
        }
    }

    private void CuttingMap()
    {
        Camera.main.GetComponent<CameraController>().Limit = new Vector3(rawMap.width/100, rawMap.height/100);

        provinceNumber = 0;

        mapParent = new GameObject("Game Map",typeof(LineRenderer)).transform;

        SetUpOutline();
        ClearMapSize();
        number = 0;
        mapArray = new bool [rawMap.width,rawMap.height,2];

        if(rawMap != null)
        {


            map = new Texture2D(100, 100);

            Vector2Int startPosition = Vector2Int.zero;

            for (int y =0;  y < rawMap.height;  y++)
            {
                for (int x = 0; x < rawMap.width; x++)
                {
                    if (rawMap.GetPixel(x, y).a == 1 && !mapArray[x, y,0])
                    {
                        CutProvince(x, y);
                    }
                }
            }

            ProvinceStats[] provinces = new ProvinceStats[mapParent.transform.childCount];

            for (int i = 0; i < mapParent.transform.childCount; i++)
            {
                provinces[i] = new ProvinceStats(Random.Range(100,200),Random.Range(90,120),0.1f,0.1f,ProvinceStats.Building.None);
            }


            mapStats = new MapStats(mapParent.transform.childCount, provinces);
            AssetDatabase.CreateAsset(mapStats, "Assets/Resources/Maps/" + rawMap.name  + ".asset");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("The map is ready!");
        }
        else
        {
            return;
        }
    }
    private void SetUpOutline()
    {
        float width = rawMap.width / 100f;
        float height = rawMap.height / 100f;

        Vector3[] positionsArray = {
            new Vector3(0    , 0     ,0),
            new Vector3(0    , height,0),
            new Vector3(width, height,0),
            new Vector3(width, 0     ,0) };
        LineRenderer line = mapParent.GetComponent<LineRenderer>();
        line.positionCount = positionsArray.Length;
        line.SetPositions(positionsArray);
        line.loop = true;
        line.material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Lit-Default"));
        line.startWidth = 0.08f;
        line.startColor = Color.black;
        line.endColor = Color.black;
        line.sortingOrder = -1;
    }
    private void CutProvince(int x,int y)
    {
        ClearMapSize();
        CheckPixel(x, y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1 ;
        map = new Texture2D(width + 10, height + 10);
        
       

        for (int i = 0; i < height + 10; i++)
        {
            for (int j = 0; j < width + 10; j++)
            {
                map.SetPixel(j, i, new Color(0, 0, 0, 0));
            }
        }


        PaintPixel(x, y);

        map.Apply();
        map.filterMode = FilterMode.Point;        

        AssetDatabase.CreateAsset(map, filePath + "/"  + rawMap.name + number.ToString() +".asset");
        AssetDatabase.Refresh();

        GameObject gameObject = new GameObject(number.ToString(), typeof(SpriteRenderer));
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();


        spriteRenderer.sprite = Sprite.Create(map ,new Rect(0, 0, width + 10, height + 10),new Vector2(0.5f,0.5f));
        gameObject.transform.position = new Vector3((maxX + minX) / 2 * 0.01f, (maxY + minY) / 2 * 0.01f, 0);
        gameObject.tag = "Province";

        gameObject.AddComponent<BoxCollider2D>();

        spriteRenderer.material = outlineMaterial;
        spriteRenderer.color = defaultColor;
        spriteRenderer.sortingOrder = -10;

        gameObject.transform.parent = mapParent;


        number++;
    } 
    private void CheckNeighbors(int x,int y,bool paint)
    {
        if (paint)
        {
            PaintPixel(x + 1, y);
            PaintPixel(x - 1, y);
            PaintPixel(x, y + 1);
            PaintPixel(x, y - 1);
        }
        else
        {
            CheckPixel(x + 1, y);
            CheckPixel(x - 1, y);
            CheckPixel(x, y + 1);
            CheckPixel(x, y - 1);
        }
    }
    private void CheckPixel(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < rawMap.width && y < rawMap.height && !mapArray[x,y,0])
        {
            Color color = rawMap.GetPixel(x, y);
            if (color.a == 1)
            {
                mapArray[x, y, 0] = true;
                CheckSizeMap(x,y);
                CheckNeighbors(x, y, false);
            }
        } 
    }
    private void PaintPixel(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < rawMap.width && y < rawMap.height && !mapArray[x, y, 1])
        {
            Color color = rawMap.GetPixel(x, y);
            if (color.a == 1)
            {
                mapArray[x, y, 1] = true;

                
                map.SetPixel(x - minX + 5, y - minY + 5, color);
                CheckNeighbors(x, y, true);
            }
        }
    }
    private void CheckSizeMap(int x,int y)
    {
        if(x > maxX) maxX = x;
        if(x < minX) minX = x;
        if(y > maxY) maxY = y;
        if(y < minY) minY = y;
    }
    private void ClearMapSize()
    {
        maxX = 0;
        minX = 99999;
        minY = 99999;
        maxY = 0;
    }
}
