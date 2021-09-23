using System;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using PaintIn3D;
using UnityEngine;
using UnityEngine.UI;

public  class PaintedPhonesData : MonoBehaviour
{

    Text text;

    public static string PAINTED_KEYS = "PAINTED_KEYS";
    public static string LASTEST_PAINTED = "LASTEST_PAINTED";



    public static void SavePaintedPhone()
    {
        print("SavePaintedPhone");
        PhoneMaterials phoneMaterials = SelectedPhone.selected_phone.GetComponent<PhoneMaterials>();

        byte[] a = phoneMaterials.albeto.GetPngData();
        byte[] n = phoneMaterials.normal.GetPngData();
        byte[] m = phoneMaterials.metallic.GetPngData();



        string _base_key = Guid.NewGuid().ToString();

        ES3.Save(_base_key + "a", a);
        ES3.Save(_base_key + "n", n);
        ES3.Save(_base_key + "m", m);
        ES3.Save(_base_key + "def", SelectedPhone.itemDefinition.name);
        print("SavePaintedPhone1");

        if (!ES3.KeyExists(PAINTED_KEYS))
        {
            ES3.Save(PAINTED_KEYS, new List<string>());
        }
        print("SavePaintedPhone2");
        List<string> keys = ES3.Load<List<string>>(PAINTED_KEYS);
        print("SavePaintedPhone3");
        keys.Add(_base_key);
        ES3.Save(PAINTED_KEYS, keys);
        ES3.Save(LASTEST_PAINTED, _base_key);
        print("SavePaintedPhone4");

    }

    public static List<GameObject> LoadPaintedPhones()
    {

        print("LoadPaintedPhones");
        List<GameObject> _saved_painted_phone = new List<GameObject>();
        if (!ES3.KeyExists(PAINTED_KEYS)) return _saved_painted_phone;
        print("LoadPaintedPhones1");
        List<string> _base_keys = ES3.Load<List<string>>(PAINTED_KEYS);
        print("LoadPaintedPhones2");
        foreach (var _base_key in _base_keys)
        {
            _saved_painted_phone.Add(LoadPaintedPhone(_base_key));
        }

        return _saved_painted_phone;
    }

    public static GameObject LoadPaintedPhone(string _base_key)
    {
        print("loaded");
        string item_def = ES3.Load<string>(_base_key + "def");

        GameObject prefab_obj = InventorySystemManager.GetItemDefinition(item_def).GetAttribute<Attribute<GameObject>>("preview_obj").GetValue();
        GameObject _obj = new GameObject(_base_key);

        _obj.AddComponent<MeshFilter>().sharedMesh = Instantiate(prefab_obj.GetComponent<MeshFilter>().sharedMesh);
        _obj.AddComponent<MeshRenderer>().sharedMaterials = prefab_obj.GetComponent<MeshRenderer>().sharedMaterials;
        _obj.GetComponent<MeshRenderer>().receiveShadows = false;
        int i = 0;
        foreach (Material item in prefab_obj.GetComponent<MeshRenderer>().sharedMaterials)
        {
            print("FUCK");
            _obj.GetComponent<MeshRenderer>().materials[i] = Instantiate(item);
            i++;
        }

        Texture2D a = new Texture2D(1000,1000);
        a.LoadImage(ES3.Load<byte[]>(_base_key + "a"));

        Texture2D n = new Texture2D(1000, 1000);
        n.LoadImage(ES3.Load<byte[]>(_base_key + "n"));

        Texture2D m = new Texture2D(1000, 1000);
        m.LoadImage(ES3.Load<byte[]>(_base_key + "m"));



        _obj.GetComponent<MeshRenderer>().sharedMaterials[0].SetTexture("_MainTex", a);
        _obj.GetComponent<MeshRenderer>().sharedMaterials[0].SetTexture("_BumpMap", n);
        _obj.GetComponent<MeshRenderer>().sharedMaterials[0].SetTexture("_MetallicGlossMap", m);

        _obj.SetActive(false);


        return _obj;
    }

    
}
