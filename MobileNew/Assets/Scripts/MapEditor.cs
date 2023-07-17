using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;



public class MapEditor : EditorWindow
{
    //input
    string filePath = "Assets/Graphics/Maps/Map1";
    Texture2D rawMap;
    //


    //output
    bool[,,] mapArray;// [x,y,a] a = 0 is Checked , a = 1 is Painted



    Texture2D map;

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
        Object obj =  EditorGUILayout.ObjectField(rawMap, typeof(Texture2D),false);
        GUILayout.Box(rawMap);
        EditorGUILayout.Space(10);
        filePath = EditorGUILayout.TextField("File Path", filePath);

        rawMap = obj as Texture2D;
        EditorGUILayout.Space(30);
        if (GUILayout.Button("Cutting Map")) CuttingMap();   
        if (GUILayout.Button("Remove Map")) RemoveMap();   
    }
    private void RemoveMap()
    {
        
    }
    private void CuttingMap()
    {
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

    
        }
        else
        {
            return;
        }
    }
    private void CutProvince(int x,int y)
    {
        ClearMapSize();
        CheckPixel(x, y);
        int width = maxX - minX + 1;
        int height = maxY - minY + 1 ;
        map = new Texture2D(width, height);
        
       

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                map.SetPixel(j, i, new Color(0, 0, 0, 0));
            }
        }


        PaintPixel(x, y);

        map.Apply();
        map.filterMode = FilterMode.Point;        

        AssetDatabase.CreateAsset(map, filePath + "/Textures/" + rawMap.name + number.ToString() +".asset");
        AssetDatabase.Refresh();

        GameObject gameObject = new GameObject(map.name,typeof(SpriteRenderer));

        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(map ,new Rect(0, 0, width, height),new Vector2(0.5f,0.5f));
        gameObject.transform.position = new Vector3((maxX + minX) / 2 * 0.01f, (maxY + minY) / 2 * 0.01f, 0);
        gameObject.tag = "Province";

        gameObject.AddComponent<BoxCollider2D>();


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

                
                map.SetPixel(x - minX, y - minY, color);
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
