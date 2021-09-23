using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] public float rotate_speed = 5;

    private void Update()
    {
        transform.Rotate( Vector3.up, rotate_speed * Time.deltaTime);
    }
}
