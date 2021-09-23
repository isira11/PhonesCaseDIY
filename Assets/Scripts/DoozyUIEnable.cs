using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;

public class DoozyUIEnable : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<UIView>().Hide();
        GetComponent<UIView>().Show();

    }



}
