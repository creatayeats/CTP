using UnityEngine;
using System.Collections;

public class PNoise : MonoBehaviour
{
    public float scale = 0;
    public float heightscale = 0;
    public int planesize = 0;
    public GameObject cube;
    public GameObject[] c;

    // Use this for initialization 
    void Start ()
    {
        for (int x = 0; x < planesize;  x++)
        {
            for (int z = 0; z < planesize; z++)
            {
                (Instantiate(cube, new Vector3(x,0,z), Quaternion.identity) as GameObject).transform.parent = transform;
                //c.transform.parent = transform; 
            }
        }
    }

    // Update is called once per frame 
    void Update ()
    {
        foreach (Transform child in transform)
        {
            float y;
            y = heightscale * Mathf.PerlinNoise(Time.time + (child.transform.position.x * scale), Time.time + (child.transform.position.z * scale));
            child.transform.position = new Vector3(child.transform.position.x, y, child.transform.position.z);
        }
    }
}﻿
