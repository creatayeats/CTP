using UnityEngine;
using System.Collections;

public class PNoiseColour : MonoBehaviour {

    //Variables to use for takinf instantiated grid size
    private int sizeX;
    private int sizeY;
    private int size2D;

    //Coord values to be used on grid
    public Vector2 coord1;
    public Vector2 coord2;
    public Vector2 coord3;
    public Vector2 coord4;
    public Vector2 coord5;

    //PN Variables
    private float scale;
    private bool move = false;

    //Arrays for storing and ordering height
    private float[,] coordinates;
    private float[] heightValues;

    //Object and class references
    public GameObject cube;
    private GameObject gridRef;
    private Grid grid;

    void Start()
    {
        //Instiate array size by pulling set grid values
        GameObject gridRef = GameObject.Find("Grid");
        Grid grid = gridRef.GetComponent<Grid>();
        sizeX = grid.xDim;
        sizeY = grid.yDim;
        size2D = sizeX * sizeY;

        //Counters
        int z = 0;
        int w = 0;

        //Two arrays, one for sorting and one for placing inside 2D array
        coordinates = new float[sizeX, sizeY];
        heightValues = new float[size2D + 1];

        //Randomise colour scale on start
        //Change for different results in PN
        scale = Random.Range(0.5f, 2.5f);

        //Spawn grid of cubes
        //Assign each cube a coordinate
        for (int x = 0; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
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
        for (int a = 0; a < sizeX; a++)
        {
            for (int b = 0; b < sizeY; b++)
            {
                coordinates[a, b] = heightValues[w];
                w++;          
            }
        }

        //Test print to specify the coordinate and height value of that particular slot
        //Coordinates are set to the slot in the array
        //Debug.Log(coordinates[2, 8]);

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
        for (int c = 0; c < sizeX; c++)
        {
            for (int d = 0; d < sizeY; d++)
            {
                //End of the array holds the largest item
                if (coordinates[c, d] == heightValues[size2D - 1])
                {
                    coord1.x = c;
                    coord1.y = d;
                    print(coord1);
                }

                if (coordinates[c, d] == heightValues[size2D - 2])
                {
                    coord2.x = c;
                    coord2.y = d;
                    print(coord2);
                }

                if (coordinates[c, d] == heightValues[size2D - 3])
                {
                    coord3.x = c;
                    coord3.y = d;
                    print(coord3);
                }
                if (coordinates[c, d] == heightValues[size2D - 4])
                {
                    coord4.x = c;
                    coord4.y = d;
                    print(coord4);
                }

                if (coordinates[c, d] == heightValues[size2D - 5])
                {
                    coord5.x = c;
                    coord5.y = d;
                    print(coord5);
                }
            }
        }
    }

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
