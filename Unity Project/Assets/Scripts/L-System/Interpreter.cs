using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    public GameObject turtle;
    public GameObject line;

    private int needsCorrecting;
    private float angle = 90.0f;
    private float lifetime = 5.0f;
    //private float boundary = 5.0f;
    private float stopwatch;
    public float spawnTime = 1.0f;
    private string inter;

    private static Stack<Vector3> thePosStack = new Stack<Vector3>();
    private static Stack<Quaternion> theRotStack = new Stack<Quaternion>();

    //private IEnumerator coroutine;

    void Start()
    {
        //initial prefab placement
        inter = "f";
        Debug.Log(inter);

        centerTurtle();
    }

    void Update()
    {
        //timer for spawning the next instance
        stopwatch += Time.deltaTime * 4;

        if (stopwatch >= spawnTime)
        {
            stringCorrecter();
            stopwatch = 0.0f;
        }
    }
    private void centerTurtle()
    {
        //places turtle in the middle of the grid
        turtle.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
    }

    private void mainLoop()
    {
        //look at each character in the final string
        foreach (char c in inter)
        {
            switch (c)
            {

                case 'f':
                case 'F':
                    F2D();
                    break;
                //case '[':
                //    OnStack();
                //    break;
                //case ']':
                //    OffStack();
                //    break;
                case '+':
                    PTree();
                    break;
                case '-':
                case '?':
                    NTree();
                    break;
                default:
                    print("default");
                    break;
            }
        }
    }

    private void stringCorrecter()
    {

        int path = Random.Range(1, 6);

        if (path == 1 || needsCorrecting == 1)
        {
            inter = inter.Replace(inter, "f++");
            Debug.Log(inter);
            path = 0;
            mainLoop();

            if (needsCorrecting == 1)
            {
                //if the path needs correcting
                inter = inter.Replace(inter, "ff");
            }
        }
        if (path == 2 || needsCorrecting == 2)
        {
            inter = inter.Replace(inter, "f--");
            Debug.Log(inter);
            path = 0;
            mainLoop();

            if (needsCorrecting == 2)
            {
                //if the path needs correcting
                inter = inter.Replace(inter, "ff");
            }
        }
            if (path == 3)
        {
            inter = inter.Replace(inter, "f+");
            Debug.Log(inter);
            path = 0;
            mainLoop();
        }
        if (path == 4)
        {
            inter = inter.Replace(inter, "f-");
            Debug.Log(inter);
            path = 0;
            mainLoop();
        }
        if (path == 5)
        {
            inter = inter.Replace(inter, "ff");
            Debug.Log(inter);
            path = 0;
            mainLoop();
        }
    }

    private void F2D()
    {
        //create trunk at turtles location and rotation
        GameObject clone = (GameObject)Instantiate(line, turtle.transform.position, turtle.transform.rotation);
        //move turtle forward
        turtle.transform.Translate(Vector3.up);

        //boundaryCheck(clone);
        destroyEnds(clone);
    }

    private void PTree()
    {
        //angle right with slight varience 
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, angle));
    }

    private void NTree()
    {
        //angle left with slight varience
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, -angle));
    }

    //void OnTriggerEnter2D(Collider2D col)
    //{
    //    if (col.gameObject.tag == "Boundary")
    //    {
    //        needsCorrecting = 1;
    //        print("Correcting..");
    //    }
    //}

    private void destroyEnds(GameObject clone)
    {
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




