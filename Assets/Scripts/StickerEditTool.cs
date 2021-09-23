using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using PaintIn3D;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickerEditTool : MonoBehaviour ,ITool
{
    public LayerMask mask;
    public Transform tool;
    public RectTransform scale_handle;
    public Transform sticker_tool;
    public float ScaleMultipler;
    public P3dPaintDecal p3DPaintDecal_Albeto;
    public P3dPaintDecal p3DPaintDecal_Mattallic;

    public float rotation = 0;

    public float scale_factor = 0;

    public void ApplyAttributes(ItemDefinition definition)
    {
        print(definition.name);
        p3DPaintDecal_Albeto.Texture = textureFromSprite(definition.GetAttribute<Attribute<Sprite>>("item_icon").GetValue());
        p3DPaintDecal_Mattallic.Texture = textureFromSprite(definition.GetAttribute<Attribute<Sprite>>("item_smoothmap").GetValue());
        p3DPaintDecal_Mattallic.Shape = textureFromSprite(definition.GetAttribute<Attribute<Sprite>>("item_icon").GetValue());
    }

    public  Texture2D textureFromSprite(Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

    private void Start()
    {
        ApplyTranform();
        ApplyMoveTranform();
    }

    private void Update()
    {


        if (Input.GetMouseButton(0))
        {

            GameObject _ui_obj = EventSystem.current.currentSelectedGameObject;
            Vector3 _mouse_pos = Input.mousePosition;


            if (IsPointerOverUI() || _ui_obj!=null)
            {
                if (_ui_obj != null)
                {
                    print(_ui_obj.name);
                    if(_ui_obj.name == "handle")
                    {
                        scale_handle.GetComponent<RectTransform>().position = _mouse_pos;
                    }
                }
            }
            else
            {
                sticker_tool.GetComponent<RectTransform>().position = _mouse_pos;
                ApplyMoveTranform();
            }
        }

        ApplyTranform();
    }

    public void ApplyTranform()
    {
        Vector2 toVector = scale_handle.transform.position - sticker_tool.position;
        rotation = Vector2.SignedAngle(sticker_tool.up, toVector);
        scale_factor = toVector.magnitude * ScaleMultipler;

        if (p3DPaintDecal_Albeto != null)
        {
            p3DPaintDecal_Albeto.Angle = -rotation;
            p3DPaintDecal_Albeto.Radius = scale_factor / 1000;
        }

        if (p3DPaintDecal_Mattallic != null)
        {
            p3DPaintDecal_Mattallic.Angle = -rotation;
            p3DPaintDecal_Mattallic.Radius = scale_factor / 1000;
        }
    }

    public void ApplyMoveTranform()
    {
        Ray _ray = Camera.main.ScreenPointToRay(sticker_tool.position);

        if (Physics.Raycast(_ray, out RaycastHit hit, 100, mask))
        {
            tool.position = hit.point;
            Quaternion r = Quaternion.LookRotation(hit.normal);
            tool.eulerAngles = new Vector3(r.x, r.y, r.z);
        }
    }

    public void ApplySticker()
    {
        tool.GetComponent<P3dHitNearby>().ManuallyHitNow();
        Destroy(transform.gameObject);
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}
