using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.UI;
using UnityEngine;
using System;
public class PopUpManager : MonoBehaviour
{


    public static void ShowPopUp(PopUpData data)
    {
         UIPopup m_popup;
         m_popup = UIPopupManager.GetPopup("popup");
        m_popup.Data.SetImagesSprites(data.Icon);
        m_popup.Data.SetLabelsTexts(data.Title, data.Description);

        UIPopupManager.ShowPopup(m_popup, m_popup.AddToPopupQueue, false);
    }


    [Serializable]
    public class PopUpData
    {
        public string Title;
        public string Description;
        public Sprite Icon;
    }

}
