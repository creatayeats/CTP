using UnityEngine;
using System.Collections;

public class PNoiseColour : MonoBehaviour {

    public int size = 9;

    public float scale;
    public bool move = false;

    private int xCoord;
    private int yCoord;

    public GameObject cube;
    public GameObject[] c;

    //Use this for initialization

    //Find values closest to 1
    //Take assigned coords from chosen GO

    void Start ()
    {
        //Randomise colour scale on start
        scale = Random.Range(0.5f, 2.5f);

        //Spawn grid of cubes
        //Assign each cube a coordinate
        for (int x = 0; x < size; x++)
        {
            xCoord = x;
            Debug.Log(xCoord);

            for (int z = 0; z < size; z++)
            {
                yCoord = z;
                Debug.Log(yCoord);
                (Instantiate(cube, new Vector3(x, 0, z), Quaternion.identity) as GameObject).transform.parent = transform;
            }
        }

        //Assign a gradient between black - white to each child cube
        foreach (Transform child in transform)
        {
            float height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
            child.GetComponent<Renderer>().material.color = new Color(height, height, height, height);
        }


    }

    // Update is called once per frame
    void Update()
    {
        float height;

        //Assign a gradient between black - white to each child cube - if it has been chnaged
        foreach (Transform child in transform)
        {
            height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
            child.GetComponent<Renderer>().material.color = new Color(height, height, height, height);
        }

        //Enable heights deoendent on gradient on true
        if (move == true)
        {
            foreach (Transform child in transform)
            {
                height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
                Debug.Log(height);
                float yPos;
                yPos = height * 3;

                child.transform.position = new Vector3(child.transform.position.x, yPos, child.transform.position.z);
                move = false;
            }
        }

    }
}
