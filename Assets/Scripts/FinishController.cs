using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using EasyMobile;
using static PopUpManager;

public class FinishController : MonoBehaviour
{
    public Transform finish_env;

    public Transform stage;

    public Transform value_t;
    public Transform level_t;
    public Transform double_t;
    public Transform skip_t;


    public TextMeshProUGUI value;
    public TextMeshProUGUI xp;
    public TextMeshProUGUI level;




    public void OnVisibilityChanged(float s)
    {
        finish_env.gameObject.SetActive(s != 0);

        if (s == 1)
        {


            Reset();
            StartCoroutine(CR_Display());

        }
        else
        {
            foreach (Transform item in stage)
            {
                Destroy(item.gameObject);
            }
        }

    }
    int total_price = 0;
    int gained_xp = 0;
    int current_xp = 0;
    int total_xp = 0;

    public IEnumerator CR_Display()
    {



        total_price = 0;
        gained_xp = 0;
        current_xp = SaveManager.instance.GetXP();

        foreach (var item in DeskController.tools_used_prices)
        {
            total_price += (int)(item.Value/1.5f);
            gained_xp += item.Value / 2;
        }


        SaveManager.instance.AddXP(gained_xp);

        total_xp = current_xp + gained_xp;

        value_t.gameObject.SetActive(true);
        for (int i = 0; i < total_price; i+=(total_price/10))
        {
            value.text = i.ToString();
            yield return new WaitForSeconds(0.01f);
        }

        value.text = total_price.ToString();

        level_t.gameObject.SetActive(true);
        level_t.GetComponent<LevelBar>().SetLevel(current_xp);

        yield return new WaitForSeconds(1.0f);


        for (int i = current_xp; i < total_xp; i+=30)
        {
            level_t.GetComponent<LevelBar>().SetLevel(i);
            yield return new WaitForSeconds(0.01f);
        }
        level_t.GetComponent<LevelBar>().SetLevel(total_xp);

        double_t.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f);
        skip_t.gameObject.SetActive(true);


    }


    public void DoubleCoins()
    {
        Advertising.ShowRewardedAd();
    }
    int ad_index = 0;
    public void OnSkip()
    {
        if (ad_index % 2 == 0)
        {
            Advertising.ShowInterstitialAd();
        }
        ad_index++;
    }

    public void PreparePhone()
    {
  
        if (SelectedPhone.selected_phone)
        {
 
            GameObject phone_def = SelectedPhone.selected_phone;

            phone_def.transform.parent = stage;
            phone_def.transform.localPosition = Vector3.zero;
            phone_def.transform.localRotation = Quaternion.identity;

            PaintedPhonesData.SavePaintedPhone();

        }

    }

    void Start()
    {
        Advertising.RewardedAdCompleted += RewardedAdCompletedHandler;
        Advertising.RewardedAdSkipped += RewardedAdSkippedHandler;

        Advertising.InterstitialAdCompleted += InterstitialAdCompletedHandler;
    }

    void RewardedAdCompletedHandler(RewardedAdNetwork network, AdPlacement location)
    {
        print("DoubleCoins added: " + total_price * 2);

        PopUpData popUpData = new PopUpData();
        popUpData.Description = "AD Completed";
        popUpData.Title = "Recived x2 coins";
        ShowPopUp(popUpData);
        SaveManager.instance.addCoins(total_price * 2);
        total_price = 0;


    }
    void RewardedAdSkippedHandler(RewardedAdNetwork network, AdPlacement location)
    {

        print("skipped Coins added: " + total_price);
        SaveManager.instance.addCoins(total_price );
        total_price = 0;
    }

    void InterstitialAdCompletedHandler(InterstitialAdNetwork network, AdPlacement location)
    {
        print("Coins added: " + total_price);
        SaveManager.instance.addCoins(total_price);
        total_price = 0;
    }


    public void Reset()
    {
        PreparePhone();
        value_t.gameObject.SetActive(false);
        level_t.gameObject.SetActive(false);
        double_t.gameObject.SetActive(false);
        skip_t.gameObject.SetActive(false);

    }
}
