using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelBar : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI level_text;
    [SerializeField] public TextMeshProUGUI xp_text;
    [SerializeField] public Image xp_fill;


    public void SetLevel(int xp)
    {

        int _level = xp / 1000;
        int _remaining = xp % 1000;


        level_text.text = _level.ToString();
        xp_text.text = _remaining.ToString() + "XP";
        xp_fill.fillAmount = _remaining  / 1000.0f;
    }
}
