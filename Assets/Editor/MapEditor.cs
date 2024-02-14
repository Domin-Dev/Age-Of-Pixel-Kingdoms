
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MapEditor : EditorWindow
{
    //input

    const string mapsPath = "Assets/Resources/Maps/";
    Texture2D rawMap;
    Material outlineMaterial;
    Material highlightMaterial;
    SelectingProvinces selectingProvinces;
    Transform buildingsParent;

    Color defaultColor = Color.grey;
    Color seaColor = new Color32(77, 101, 180, 255);
    //


    //output
    bool[,,] mapArray;// [x,y,a] a = 0 is Checked , a = 1 is Painted
    List<List<Vector2Int>> provincesList;
    List<Vector2Int> border;


    Texture2D map;
    Transform mapParent;
    MapStats mapStats;
    bool isSea;


    int maxX;
    int maxY;
    int minX;
    int minY;

    int number = 0;

    int players;

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

        players = EditorGUILayout.IntField(players,new GUILayoutOption[0]);

        defaultColor = new Color32(77, 101, 180,255);

        GUILayout.Box(rawMap);
        EditorGUILayout.Space(10);
       // filePath = EditorGUILayout.TextField("File Path", filePath);

        
        outlineMaterial = obj2 as Material; 
        rawMap = obj as Texture2D;
        EditorGUILayout.Space(30);
        if (GUILayout.Button("Cutting Map"))CuttingMap();
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
        selectingProvinces = FindAnyObjectByType<SelectingProvinces>();
        buildingsParent = GameObject.FindGameObjectWithTag("Buildings").transform;

        if (GUILayout.Button("Next"))
        {
            SpriteRenderer spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            selectingProvinces.ChangeProvinceBorderColor(spriteRenderer,Color.black);
            spriteRenderer.sortingOrder = -10;

            if (provinceNumber < mapParent.childCount - 1) provinceNumber++;
            else provinceNumber = 0;
            SetUpProvinces();

            spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            selectingProvinces.ChangeProvinceBorderColor(spriteRenderer, Color.yellow);
            spriteRenderer.sortingOrder = -1;
        }

        if (GUILayout.Button("Back"))
        {
            SpriteRenderer spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            selectingProvinces.ChangeProvinceBorderColor(spriteRenderer, Color.black);
            spriteRenderer.sortingOrder = -10;

            if (provinceNumber > 0) provinceNumber--;
            else provinceNumber = mapParent.childCount -1;
            SetUpProvinces();

            spriteRenderer = mapParent.GetChild(provinceNumber).GetComponent<SpriteRenderer>();
            selectingProvinces.ChangeProvinceBorderColor(spriteRenderer, Color.yellow);
            spriteRenderer.sortingOrder = -1;
        }

        if (GUILayout.Button("Set Mine"))
        {
            Build(0, provinceNumber);
        }
        EditorGUILayout.Space(20);
        if (GUILayout.Button("Set Owner"))
        {
            SetOwner(0, provinceNumber);
        }
    }


    private void CuttingMap()
    {
        Camera.main.GetComponent<CameraController>().Limit = new Vector3(rawMap.width/100 + 1f, rawMap.height/100 + 1f);

        provinceNumber = 0;

        mapParent = new GameObject("Game Map").transform;
        mapParent.tag = "GameMap";

      //  SetUpOutline();
        ClearMapSize();
        number = 0;
        mapArray = new bool [rawMap.width,rawMap.height,2];
        provincesList = new List<List<Vector2Int>> ();
        
        if(rawMap != null)
        {
            if (!AssetDatabase.IsValidFolder(mapsPath + rawMap.name))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Maps", rawMap.name);
                AssetDatabase.CreateFolder("Assets/Resources/Maps/" + rawMap.name, "Sprites");
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }

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
                if (provincesList[i].Count == 0)
                {
                    provinces[i] = new ProvinceStats(i,true);
                }
                else
                {
                    provinces[i] = new ProvinceStats(i,false);
                }
            }


            for (int i = 0; i < provincesList.Count; i++)
            {
                for (int j = 0; j < provincesList.Count; j++)
                {
                    if(i != j)
                    {
                        if(CheckBorders(provincesList[i], provincesList[j]))
                        {
                            provinces[i].AddNeighbor(j);
                        }
                    }
                }         
            }





            mapStats = new MapStats(mapParent.transform.childCount, provinces,4);

            PrefabUtility.SaveAsPrefabAssetAndConnect(mapParent.gameObject, mapsPath + rawMap.name + "/Map.prefab", InteractionMode.UserAction);
            AssetDatabase.CreateAsset(mapStats,mapsPath + rawMap.name + "/MapStats.asset");
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            Debug.Log("The map is ready!");
            while (mapParent.childCount > 0)
            {
                DestroyImmediate(mapParent.GetChild(0).gameObject);
            }
            DestroyImmediate(mapParent.gameObject);
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
        isSea = false;
        border = new List<Vector2Int>();
        ClearMapSize();
        CheckPixel(x, y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

       if(isSea) provincesList.Add(new List<Vector2Int>());
       else provincesList.Add(border);



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

        AssetDatabase.CreateAsset(map, mapsPath + rawMap.name + "/Sprites/" + number.ToString() +".asset");
        AssetDatabase.Refresh();

        GameObject gameObject = new GameObject(number.ToString(), typeof(SpriteRenderer));
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();


        spriteRenderer.sprite = Sprite.Create(map ,new Rect(0, 0, width + 10, height + 10),new Vector2(0.5f,0.5f));
        gameObject.transform.position = new Vector3((maxX + minX) / 2 * 0.01f, (maxY + minY) / 2 * 0.01f, 0);
        gameObject.tag = "Province";

        gameObject.AddComponent<BoxCollider2D>();

        spriteRenderer.material = outlineMaterial;
         if(isSea) spriteRenderer.sortingOrder = -11;
         else spriteRenderer.sortingOrder = -10;

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
            bool bool1 = CheckPixel(x + 1, y);
            bool bool2 = CheckPixel(x - 1, y);
            bool bool3 = CheckPixel(x, y + 1);
            bool bool4 = CheckPixel(x, y - 1);

            if(bool1 || bool2 || bool3 || bool4)
            {
                AddToBorderList(x, y);
            }
        }
    }
    private bool CheckPixel(int x,int y)
    {
        if (x >= 0 && y >= 0 && x < rawMap.width && y < rawMap.height && !mapArray[x,y,0])
        {
            Color color = rawMap.GetPixel(x, y);

            if (color.a == 1)
            {
                if(!isSea && color.b == defaultColor.b)
                {
                    isSea = true;
                    Debug.Log("water!");
                }

                mapArray[x, y, 0] = true;
                CheckSizeMap(x,y);
                CheckNeighbors(x, y, false);
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
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



    private void AddToBorderList(int x,int y)
    {
        Vector2Int vector = new Vector2Int(x,y);
        foreach (Vector2Int item in border)
        {
            if(vector == item)
            {
                return;
            }
        }
        border.Add(vector);
    }
    private bool CheckBorders(List<Vector2Int> borderList, List<Vector2Int> neighbor)
    {
        float distance = 99999;

        for (int i = 0; i < borderList.Count; i++)
        {
            for (int j = 0; j < neighbor.Count; j++)
            {
                float currentDistance = Vector2.Distance(borderList[i], neighbor[j]);
                if (currentDistance <= 4)
                {
                    return true;
                } 
                else if (currentDistance < distance)
                {               
                    distance = currentDistance;
                }
            }
        }
        if(distance <= 4) return true;
        else return false;      
    }
    private void Build(int buildingIndex,int provinceIndex)
    {
      ProvinceStats[] provinces = Resources.Load<MapStats>("Maps/World").provinces;
      provinces[provinceIndex].buildingIndex = buildingIndex;
    }
    private void SetOwner(int playerIndex,int provinceIndex)
    {
       Resources.Load<MapStats>("Maps/World").provinces[provinceIndex].SetNewOwner(playerIndex);
       selectingProvinces.ChangeProvinceColor(mapParent.GetChild(provinceIndex).GetComponent<SpriteRenderer>(), GameManager.Instance.GetPlayerColor(provinceIndex));
   
    }
}
