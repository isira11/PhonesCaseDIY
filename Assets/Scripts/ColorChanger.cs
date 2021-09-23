using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

public class ColorChanger : MonoBehaviour
{
    public P3dPaintDecal p3DPaintDecal;
    public Gradient gradient;
    public Color final;

    public float lerpSpeed = 5;

    public float v = 0;

    public void Update()
    {

        if (Input.GetMouseButton(0))
        {
            v = Mathf.Lerp(0, 1, Mathf.PingPong(Time.time * lerpSpeed, 1));


            final = gradient.Evaluate(v);

            if (p3DPaintDecal)
            {
                p3DPaintDecal.Color = final;
            }

        }
    }
}

