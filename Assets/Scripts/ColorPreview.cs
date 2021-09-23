using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using UnityEngine;

public class ColorPreview : MonoBehaviour,IPreview
{
    public MeshRenderer m1;
    public MeshRenderer m2;

    public void Preview(ItemDefinition item_def)
    {
        m1.material.color = item_def.GetAttribute<Attribute<Color>>("item_color").GetValue();
        m2.material.color = item_def.GetAttribute<Attribute<Color>>("item_color").GetValue();
    }
}
