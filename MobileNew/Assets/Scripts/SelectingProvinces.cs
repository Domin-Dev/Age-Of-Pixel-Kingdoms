using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class SelectingProvinces : MonoBehaviour
{
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
                        spriteRenderer.color =
                            pixel;
                        break;
                    }
                }
            }
            
        }
    }
}
