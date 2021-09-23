using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PopUpManager;
using TMPro;

public class GalleryController : MonoBehaviour
{

    public TextMeshProUGUI text;
    public Transform gallery_env;

    public Transform stage;

    int current = 0;

    List<GameObject> list;


    public void OnVisibilityChanged(float s)
    {
        gallery_env.gameObject.SetActive(s != 0);

        if (s == 1)
        {
            StartCoroutine(CR_FILL());
        }
    }


    IEnumerator CR_FILL()
    {

        PopUpData popUpData = new PopUpData();
        popUpData.Title = "Wait";
        popUpData.Description = "Loading Data";
        ShowPopUp(popUpData);

        yield return new WaitForSeconds(1.0f);

        foreach (Transform item in stage)
        {
            Destroy(item.gameObject);

        }

        List<GameObject> _phone_stack = PaintedPhonesData.LoadPaintedPhones();
        list = new List<GameObject>();
        if (_phone_stack.Count > 0)
        {


            foreach (var item in _phone_stack)
            {
                list.Add(item);
                item.transform.parent = stage;
            }
            list.Reverse();
        }

        ShowNext(-1);
    }


    public void ShowNext(int direction)
    {
        if (list.Count > 0)
        {
            list[current].SetActive(false);
            current += direction;
            current = Mathf.Clamp(current, 0, list.Count - 1);
            list[current].SetActive(true);
            list[current].transform.localPosition = Vector3.zero;
            text.text = (current + 1) + "/" + list.Count;
        }
        else
        {
            text.text = 0+"/"+0;
        }

    }
}
