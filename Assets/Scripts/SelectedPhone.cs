using System.Collections;
using System.Collections.Generic;
using Opsive.UltimateInventorySystem.Core;
using UnityEngine;

public class SelectedPhone : MonoBehaviour
{
    public static SelectedPhone instance;
    private void Awake()
    {
        instance = this;
    }
    public static GameObject selected_phone;
    public static ItemDefinition itemDefinition;
}
