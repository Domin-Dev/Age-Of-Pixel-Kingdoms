
using UnityEngine;




public class SelectingProvinces : MonoBehaviour
{
    private Transform selectedObject;
    [SerializeField] Material material;

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
                            selectedObject = item.collider.gameObject.transform;
                        }
                        else
                        {

                            selectedObject = item.collider.gameObject.transform;
                        }

                        Color color = selectedObject.GetComponent<SpriteRenderer>().color;
                            

                        selectedObject.GetComponent<SpriteRenderer>().color = Color.white;
                        selectedObject.GetComponent<SpriteRenderer>().material = GameAssets.Instance.highlight;
                        selectedObject.GetComponent<SpriteRenderer>().material.SetColor("_Color_2", color);



                        UIManager.Instance.OpenProvinceStats(int.Parse(item.collider.name));
                        break;
                    }
                }
            }
            
        }
    }
}
