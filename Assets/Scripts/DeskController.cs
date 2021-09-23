using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using Opsive.UltimateInventorySystem.Core.InventoryCollections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Opsive.UltimateInventorySystem.Exchange;
using UnityEngine.EventSystems;
using PaintIn3D;

public class DeskController : MonoBehaviour
{
    public Transform desk_env;
    public DeskGridCatFill gridCatFill;
    public ItemFilter itemFilter_Paint;
    public Transform stage;
    public GameObject tool;


    [Header("360 button")]
    public GameObject rotate_scripts;
    public TextMeshProUGUI rotate_tool_text;
    public Image rotate_tool_image;

    [Header("Brush Tools")]
    public GameObject color_drawer;
    public Slider radius_slider;

    [Header("Textures")]
    public Texture ta;
    public Texture tn;
    public Texture tm;

    public static Dictionary<string, int> tools_used_prices;



    public void FillTools()
    {
        ItemCategory itemCategory = InventorySystemManager.GetItemCategory("tool_bar_item");
        List<(ItemCategory, int)> list = itemCategory.GetAllChildrenWithLevel(false, 1);
        List<ItemCategory> itemCategories = new List<ItemCategory>();

        foreach (var item in list)
        {
            itemCategories.Add(item.Item1);
        }
        gridCatFill.Fill(itemCategories);
        itemFilter_Paint.FillItemsWithCat(InventorySystemManager.GetItemCategory("paint"), true);


    }

    public void OnVisibilityChanged(float s)
    {
        desk_env.gameObject.SetActive(s != 0);
        if (s == 1)
        {
            tools_used_prices = new Dictionary<string, int>();
            FillTools();
            brushToolSetActive(false);
            PreparePhone();

        }
        else
        {
            if (tool)
            {
                Destroy(tool);
            }
        }
    }

    public void OnToolSelected(ItemDefinition item)
    {
        if (tool)
        {
            Destroy(tool);
            tool = null;
        }

        tool = Instantiate(item.GetAttribute<Attribute<GameObject>>("tool").GetValue());
        OnPaintedSelected(selected_paints);
        OnSliderChanged(radius_slider.value);
        SetActiveRotationTool(false);
        ItemSelected(item);


        if (tool.TryGetComponent(out Brush brush))
        {
            brushToolSetActive(true);

 
        }
        else
        {
            brushToolSetActive(false);

            if (tool.TryGetComponent(out ITool tool_script))
            {
                tool_script.ApplyAttributes(item);
            }

        }

        
    }

    ItemDefinition selected_paints;
    public void OnPaintedSelected(ItemDefinition paint_items)
    {
        print("Paint Selected" + paint_items==null);
        if (tool)
        {
            if (tool.TryGetComponent(out ITool tool_script) && tool.TryGetComponent(out Brush brush))
            {
                tool_script.ApplyAttributes(paint_items);
                selected_paints = paint_items;
            }
        }
        OnSliderChanged(radius_slider.value);
        SetActiveRotationTool(false);

        ItemSelected(paint_items);
    }

    public void ItemSelected(ItemDefinition item)
    {
        if (item != null)
        {
            if (!tools_used_prices.ContainsKey(item.name))
            {
                tools_used_prices.Add(item.name, item.GetAttribute<Attribute<CurrencyAmounts>>("BuyPrice").GetValue()[0].Amount);
                print(item.GetAttribute<Attribute<CurrencyAmounts>>("BuyPrice").GetValue()[0].Amount);
            }
        }


    }

    public void PreparePhone()
    {
        if (SelectedPhone.selected_phone)
        {
            GameObject phone_obj = SelectedPhone.selected_phone;
            phone_obj.transform.parent = stage;
            phone_obj.transform.localPosition = Vector3.zero;
            phone_obj.transform.localRotation = Quaternion.identity;

            P3dPaintable p = phone_obj.AddComponent<P3dPaintable>();
            phone_obj.AddComponent<P3dMaterialCloner>();

            P3dSlot sa = new P3dSlot(0, "_MainTex");
            P3dSlot sn = new P3dSlot(0, "_BumpMap");
            P3dSlot sm = new P3dSlot(0, "_MetallicGlossMap");

            P3dPaintableTexture p_a = phone_obj.AddComponent<P3dPaintableTexture>();
            P3dPaintableTexture p_n = phone_obj.AddComponent<P3dPaintableTexture>();
            P3dPaintableTexture p_m = phone_obj.AddComponent<P3dPaintableTexture>();

            P3dGroup ga = new P3dGroup(0);
            P3dGroup gn = new P3dGroup(10);
            P3dGroup gb = new P3dGroup(25);

            p_a.Slot = sa;
            p_n.Slot = sn;
            p_m.Slot = sm;

            p_a.Group = ga;
            p_n.Group = gn;
            p_m.Group = gb;

            p_n.Conversion = P3dPaintableTexture.ConversionType.Normal;


            p.Activate();

            PhoneMaterials p_s = phone_obj.AddComponent<PhoneMaterials>();
            p_s.albeto = p_a;
            p_s.normal = p_n;
            p_s.metallic = p_m;

        }
    }

    public void OnSliderChanged(float value)
    {
        if (tool)
        {
            if (tool.TryGetComponent(out Brush brush))
            {
                brush.OnRadiusChanged(value);
            }
        }
    }

    public void brushToolSetActive(bool active)
    {
        color_drawer.SetActive(active);
        radius_slider.gameObject.SetActive(active);
    }

    public void OnRotateToolToggled()
    {
        bool _active = rotate_scripts.activeInHierarchy;
        rotate_scripts.SetActive(!_active);
        print(_active);
        ToolSetActive(_active);
        UpdateRotateToolUI();
    }

    public void SetActiveRotationTool(bool active)
    {
        rotate_scripts.gameObject.SetActive(active);
        UpdateRotateToolUI();
        ToolSetActive(!active);
    }

    public void UpdateRotateToolUI()
    {
        if (rotate_scripts.gameObject.activeInHierarchy)
        {
            rotate_tool_image.color = Color.green;
            rotate_tool_text.text = "ON 360";
        }
        else
        {
            rotate_tool_image.color = Color.white;
            rotate_tool_text.text = "OFF 360";
        }
    }

    public void ToolSetActive(bool _active)
    {
        if (tool)
        {
            tool.SetActive(_active);
        }
    }


    public bool IsPointerOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

}


