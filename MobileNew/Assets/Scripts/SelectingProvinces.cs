
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;




public class SelectingProvinces : MonoBehaviour
{
    private Transform selectedObject;
    private Color selectedColor;


    [SerializeField] Material material;

    private void Start()
    {      
        GameAssets.Instance.buildWorkshop.GetComponent<Button>().onClick.AddListener(() =>Build(0));;
        GameAssets.Instance.buildFort.GetComponent<Button>().onClick.AddListener(() =>Build(1));;
        GameAssets.Instance.buildUniversity.GetComponent<Button>().onClick.AddListener(() =>Build(2));;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            foreach (RaycastHit2D item in Physics2D.RaycastAll(worldClickPosition,Vector2.zero))
            {
                if (item.collider.tag == "Province")
                {
                    SpriteRenderer spriteRenderer = item.collider.GetComponent<SpriteRenderer>();
                    Texture2D spriteTexture = spriteRenderer.sprite.texture;
                    Rect rect = spriteRenderer.sprite.rect;

                    float x = (worldClickPosition.x - item.collider.transform.position.x)*spriteRenderer.sprite.pixelsPerUnit;
                    float y = (worldClickPosition.y- item.collider.transform.position.y)*spriteRenderer.sprite.pixelsPerUnit;

                    x += rect.width / 2;
                    y += rect.height / 2;

                    x += rect.x;
                    y += rect.y;

                    Color pixel = spriteTexture.GetPixel(Mathf.FloorToInt(x), Mathf.FloorToInt(y));

                    if (pixel.a == 0) continue;
                    else
                    {
                        if (selectedObject != null)
                        {
                            SpriteRenderer spriteRenderer1 = selectedObject.GetComponent<SpriteRenderer>();
                            spriteRenderer1.color = selectedColor;
                            spriteRenderer1.material = GameAssets.Instance.outline;
                            spriteRenderer1.sortingOrder = -10;
                            selectedObject = item.collider.gameObject.transform;

                        }
                        else
                        {
                            selectedObject = item.collider.gameObject.transform;
                        }        
                        selectedColor = spriteRenderer.color;
                        spriteRenderer.color = Color.white;
                        spriteRenderer.material = GameAssets.Instance.highlight;
                        spriteRenderer.sortingOrder = -1;
                        spriteRenderer.material.SetColor("_Color_2", selectedColor);



                        UIManager.Instance.
                            OpenProvinceStats(int.Parse(item.collider.name));
                        break;
                    }
                }
            }
            
        }
    }

    public void Build(int index)
    {
        Sprite sprite = GameAssets.Instance.spriteWorkshop;
        switch (index)
        {
            case 0:
                sprite = GameAssets.Instance.spriteWorkshop;
                break;
            case 1:
                sprite = GameAssets.Instance.spriteFort;
                break;
            case 2:
                sprite = GameAssets.Instance.spriteUniversity;
                break;
        }

        Transform transform = new GameObject(selectedObject.name, typeof(SpriteRenderer)).transform;
        transform.position = selectedObject.position + new Vector3(0, 0.1f, 0);
        transform.GetComponent<SpriteRenderer>().sprite = sprite;
        transform.GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    public void Recruit(int index)
    {

        ProvinceStats provinceStats = GameManager.Instance.stats.provinces[int.Parse(selectedObject.name)];

        if(provinceStats.units == 0)
        {
            Instantiate(GameAssets.Instance.unitCounter,selectedObject.transform.position - new Vector3(0,0.05f,0),Quaternion.identity, selectedObject);
        }

    }
}
