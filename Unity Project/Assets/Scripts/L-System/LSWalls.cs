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

    //private float boundary = 5.0f;

    //Stopwatch variables so items spawn at a set rate
    private float stopwatch;
    public float spawnTime = 1.0f;

    //String used for rules
    private string rulesString;

    //Use to stop wall replication
    private bool needsCorrecting;

    private static Stack<Vector3> thePosStack = new Stack<Vector3>();
    private static Stack<Quaternion> theRotStack = new Stack<Quaternion>();

    //Set outer boundaries for system
    //Needs to block between pieces

    void Start()
    {
        needsCorrecting = false;

        //Initial prefab placement
        rulesString = "f";
        Debug.Log(rulesString);

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
        foreach (char c in rulesString)
        {
            switch (c)
            {

                case 'f':
                case 'F':
                    PlaceForward();
                    break;
                //case '[':
                //    OnStack();
                //    break;
                //case ']':
                //    OffStack();
                //    break;
                case '+':
                    PosTurn();
                    break;
                case '-':
                case '?':
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
        //Select the next path at random
        int path = Random.Range(1, 5);

        //Set path depending on value
        //Rotate 180 degrees (for correction on boundary mostly
        if (path == 1 || needsCorrecting)
        {
            rulesString = rulesString.Replace(rulesString, "f++");
            Debug.Log(rulesString);
            path = 0;
            CoreLoop();

            //If a boundry was hit, double up forward after backing up
            if (needsCorrecting)
            {
                //if the path needs correcting
                rulesString = rulesString.Replace(rulesString, "ff");
                needsCorrecting = false;
            }
        }
        //Right 90 degrees
        if (path == 2)
        {
            rulesString = rulesString.Replace(rulesString, "f+");
            Debug.Log(rulesString);
            path = 0;
            CoreLoop();
        }
        //Left 90 degrees
        if (path == 3)
        {
            rulesString = rulesString.Replace(rulesString, "f-");
            Debug.Log(rulesString);
            path = 0;
            CoreLoop();
        }
        //Double up forward
        if (path == 4)
        {
            rulesString = rulesString.Replace(rulesString, "ff");
            Debug.Log(rulesString);
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

        //boundaryCheck(clone);
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

    //Not working
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag == "Boundary")
        {
            needsCorrecting = true;
            print("Correcting..");
        }
    }

    private void DestroyEnds(GameObject clone)
    {
        //Destroy wall after set lifetime
        Destroy(clone, lifetime);
    }

    //private void OnStack()
    //{ //place turtles position into the position stack
    //    Vector3 tempPos = turtle.transform.position;
    //    thePosStack.Push(tempPos);
    //    //place turtles rotation into the rotation stack
    //    Quaternion tempRot = turtle.transform.rotation;
    //    theRotStack.Push(tempRot);
    //}

    //private void OffStack()
    //{
    //    // move turtle to position on top of stack
    //    turtle.transform.position = thePosStack.Pop();
    //    //rotate turtle to rotation on stack
    //    turtle.transform.rotation = theRotStack.Pop();
    //}
}




