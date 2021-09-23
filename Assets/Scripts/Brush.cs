using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using PaintIn3D;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;

public class Brush : MonoBehaviour,ITool
{
    public Texture shape;

    GameObject brush;

    public void ApplyAttributes(ItemDefinition item_def)
    {
        if (item_def == null)
        {
            //P3dPaintDecal white_paint = new P3dPaintDecal();
            //ApplyPaintToBrush(new P3dPaintDecal[1] { white_paint });
        }
        else
        {
            GameObject obj = item_def.GetAttribute<Attribute<GameObject>>("color_obj").GetValue();
            ApplyPaintToBrush(obj.GetComponents<P3dPaintDecal>(),obj);
        }
    }

    public void ApplyPaintToBrush(P3dPaintDecal[] paint_list,GameObject obj = null)
    {
        if (brush)
        {
            Destroy(brush);
        }


        //GameObject _brush = new GameObject("brush");
        //_brush.transform.parent = transform;


        //brush.AddComponent<P3dHitScreen>();

        //foreach (P3dPaintDecal paint in paint_list)
        //{
        //    P3dPaintDecal p = brush.AddComponent<P3dPaintDecal>();
        //    p.Layers = paint.Layers;
        //    p.Group = paint.Group;
        //    p.BlendMode = paint.BlendMode;
        //    p.Texture = paint.Texture;
        //    p.Color = paint.Color;
        //    p.Shape = shape;

        //}


        GameObject _brush = Instantiate(obj);
        _brush.transform.parent = transform;
        brush = _brush;

        foreach (P3dPaintDecal item in _brush.GetComponents<P3dPaintDecal>())
        {
            item.Shape = shape;
        }

        P3dHitScreen hit = brush.AddComponent<P3dHitScreen>();
        hit.NormalDirection = P3dHitScreen.DirectionType.RayDirection;
        hit.RotateTo = P3dHitScreen.RotationType.Normal;

        _brush.GetComponent<P3dHitScreen>().ClearHitCache();
        _brush.GetComponent<P3dHitScreen>().ResetConnections();
    }

    public void OnRadiusChanged(float radius)
    {
        if (brush)
        {
            foreach (P3dPaintDecal item in brush.GetComponents<P3dPaintDecal>())
            {
                item.Radius = radius +0.1f;
            }
        }
    }


}
