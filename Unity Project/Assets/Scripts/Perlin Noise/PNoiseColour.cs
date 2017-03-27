using UnityEngine;
using System.Collections;

public class PNoiseColour : MonoBehaviour {

    public int size = 9;

    //Coord values to be used on grid
    public Vector2 coord1;
    public Vector2 coord2;
    public Vector2 coord3;
    public Vector2 coord4;
    public Vector2 coord5;

    public float scale;
    public bool move = false;

    private float[,] coordinates;
    private float[] heightValues;

    public GameObject cube;

    void Start()
    {
        //counters
        int z = 0;
        int w = 0;

        //Two arrays, one for sorting and one for placing inside 2D array
        coordinates = new float[9, 9];
        heightValues = new float[82];

        //Randomise colour scale on start
        //Change for different results in PN
        scale = Random.Range(0.5f, 2.5f);

        //Spawn grid of cubes
        //Assign each cube a coordinate
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                (Instantiate(cube, new Vector3(x, 0, y), Quaternion.identity) as GameObject).transform.parent = transform;
            }
        }

        //Assign a gradient between black - white to each child cube
        foreach (Transform child in transform)
        {
            float height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
            child.GetComponent<Renderer>().material.color = new Color(height, height, height, height);

            //Assign height values to an array
            heightValues[z] = height;
            z++;
        }

        //Give height values a coordinate by placing in 2D array
        for (int a = 0; a < size; a++)
        {
            for (int b = 0; b < size; b++)
            {
                coordinates[a, b] = heightValues[w];
                w++;          
            }
        }

        //Test print to specify the coordinate and height value of that particular slot
        //Coordinates are set to the slot in the array
        Debug.Log(coordinates[2, 8]);

        //Bubble sort of height values from closest to 0 -> 1;
        float temp = 0;

        for (int write = 0; write < coordinates.Length; write++)
        {
            for (int sort = 0; sort < coordinates.Length - 1; sort++)
            {
                if (heightValues[sort] > heightValues[sort + 1])
                {
                    temp = heightValues[sort + 1];
                    heightValues[sort + 1] = heightValues[sort];
                    heightValues[sort] = temp;
                }
            }
        }
        //Print the sorted height values
        for (int j = 0; j < coordinates.Length; j++)
        {
            print(heightValues[j] + " ");
        }

        //Compare the sorted list to the unsorted list for highest point coordinates
        //Need to make less messy! List perhaps?
        //Coords tend to replicate as some hight values are the same
        for (int c = 0; c < size; c++)
        {
            for (int d = 0; d < size; d++)
            {
                //80 holds largest item
                if (coordinates[c, d] == heightValues[80])
                {
                    coord1.x = c;
                    coord1.y = d;
                    print(coord1);
                }

                if (coordinates[c, d] == heightValues[79])
                {
                    coord2.x = c;
                    coord2.y = d;
                    print(coord2);
                }

                if (coordinates[c, d] == heightValues[78])
                {
                    coord3.x = c;
                    coord3.y = d;
                    print(coord3);
                }
                if (coordinates[c, d] == heightValues[77])
                {
                    coord4.x = c;
                    coord4.y = d;
                    print(coord4);
                }

                if (coordinates[c, d] == heightValues[76])
                {
                    coord5.x = c;
                    coord5.y = d;
                    print(coord5);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float height;

        //Assign a gradient between black - white to each child cube - if it has been changed
        foreach (Transform child in transform)
        {
            height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
            child.GetComponent<Renderer>().material.color = new Color(height, height, height, height);
        }

        //Enable heights dependent on gradient on true
        if (move == true)
        {
            foreach (Transform child in transform)
            {
                height = Mathf.PerlinNoise(child.transform.position.x / scale, child.transform.position.z / scale);
                float yPos;
                yPos = height * 3;

                child.transform.position = new Vector3(child.transform.position.x, yPos, child.transform.position.z);
                move = false;
            }
        }

    }
}
