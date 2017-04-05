using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LSWalls : MonoBehaviour
{
    //Wall prefab + its spawner
    public GameObject lineSpawner;
    public GameObject line;

    //Set spawner rotation and wall lifetime
    private float angle = 90.0f;
    private float lifetime = 5.0f;

    //Variables to use for taking instantiated grid size
    private int sizeX;
    private int sizeY;
    private float sizeHalfX;
    private float sizeHalfY;

    //Stopwatch variables so items spawn at a set rate
    private float stopwatch;
    public float spawnTime = 1.0f;

    //String used for rules
    private string rulesString;

    //Use to stop wall replication
    private bool needsCorrecting;

    //Object and class references
    private GameObject gridRef;
    private Grid grid;

    //Needs to block between pieces

    void Start()
    {
        //Instiate array size by pulling set grid values
        GameObject gridRef = GameObject.Find("Grid");
        Grid grid = gridRef.GetComponent<Grid>();
        sizeX = grid.xDim;
        sizeY = grid.yDim;
        sizeHalfX = sizeX / 2;
        sizeHalfY = sizeY / 2;

        //Instantiate correction bool
        needsCorrecting = false;

        //Initial prefab placement
        rulesString = "f";

        SpawningSpawner();
    }

    void Update()
    {
        //Timer for spawning the next instance
        stopwatch += Time.deltaTime * 4;

        if (stopwatch >= spawnTime)
        {
            StringCorrecter();
            stopwatch = 0.0f;
        }
    }
    private void SpawningSpawner()
    {
        //Spawner placed in the middle of the grid
        lineSpawner.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
    }

    private void CoreLoop()
    {
        //Case which reads the current string
        foreach (char character in rulesString)
        {
            switch (character)
            {

                case 'f':
                case 'F':
                    PlaceForward();
                    break;
                case '+':
                    PosTurn();
                    break;
                case '-':
                    NegTurn();
                    break;
                default:
                    print("default");
                    break;
            }
        }
    }

    private void StringCorrecter()
    {
        Boundary();
        //Rotate back x degrees depending on current string (for correction on boundary mostly)
        if (needsCorrecting)
        {
            if (rulesString == "f-")
            {
                rulesString = rulesString.Replace(rulesString, "-f");
                //Debug.Log(rulesString);
                needsCorrecting = false;
                CoreLoop();
            }
            if (rulesString == "f+")
            {
                rulesString = rulesString.Replace(rulesString, "+f");
                //Debug.Log(rulesString);
                needsCorrecting = false;
                CoreLoop();
            }
            if (rulesString == "f")
            {
                rulesString = rulesString.Replace(rulesString, "++f");
                //Debug.Log(rulesString);
                needsCorrecting = false;
                CoreLoop();
            }
        }

        //Select the next path at random
        int path = Random.Range(1, 4);

        //Right 90 degrees
        if (path == 1)
        {
            rulesString = rulesString.Replace(rulesString, "f+");
            //Debug.Log(rulesString);
            path = 0;
            CoreLoop();
        }
        //Left 90 degrees
        if (path == 2)
        {
            rulesString = rulesString.Replace(rulesString, "f-");
            //Debug.Log(rulesString);
            path = 0;
            CoreLoop();
        }
        //Double up forward
        if (path == 3)
        {
            rulesString = rulesString.Replace(rulesString, "f");
            //Debug.Log(rulesString);
            path = 0;
            CoreLoop();
        }
    }

    private void PlaceForward()
    {
        //Create a line at the spawner location and rotation
        GameObject clone = (GameObject)Instantiate(line, lineSpawner.transform.position, lineSpawner.transform.rotation);
        //Move spawner forward
        lineSpawner.transform.Translate(Vector3.up);

        DestroyEnds(clone);
    }

    private void PosTurn()
    {
        //Turn right at 90 degrees
        lineSpawner.transform.Rotate(new Vector3(0.0f, 0.0f, angle));
    }

    private void NegTurn()
    {
        //Turn left at 90 degrees
        lineSpawner.transform.Rotate(new Vector3(0.0f, 0.0f, -angle));
    }

    void Boundary()
    {
        //Check to see if the spawner is hitting the grid boundary
        if ((lineSpawner.transform.position.x >= sizeHalfX - 0.1) || (lineSpawner.transform.position.x <= -sizeHalfX + 0.1))
        {
            //print("Needs Correcting");
            needsCorrecting = true;
        }
        if ((lineSpawner.transform.position.y >= sizeHalfY - 0.1) || (lineSpawner.transform.position.y <= -sizeHalfY + 0.1))
        {
            //print("Needs Correcting");
            needsCorrecting = true;
        }
    }

    private void DestroyEnds(GameObject clone)
    {
        //Destroy wall after set lifetime
        Destroy(clone, lifetime);
    }
}




