using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{
    public GameObject turtle;
    public GameObject line;

    private int needsCorrecting = 0;
    private float angle = 90;
    private string inter;
    private string tempInter;
    private string rightCorrecter;
    private string leftCorrecter;

    private static Stack<Vector3> thePosStack = new Stack<Vector3>();
    private static Stack<Quaternion> theRotStack = new Stack<Quaternion>();



    void Start()
    {
        StreamReader rules = new StreamReader("dragon.txt");
        inter = rules.ReadLine();
        Debug.Log(inter);
        rules.Close();

        StreamReader correcter1 = new StreamReader("left.txt");
        leftCorrecter = correcter1.ReadLine();
        Debug.Log(leftCorrecter);
        correcter1.Close();

        StreamReader correcter2 = new StreamReader("right.txt");
        rightCorrecter = correcter2.ReadLine();
        Debug.Log(rightCorrecter);
        correcter2.Close();

        //tempInter = tempInter.Replace(tempInter, inter);
        Debug.Log(tempInter);
        centerTurtle();
    }

    void Update()
    {
        //if generate bool is true
        if (Input.GetKeyDown("space"))
        {
            mainLoop();
        }

        //if reset key pressed
        if (Input.GetKeyDown("r"))
        {
            needsCorrecting = 1;
            stringCorrecter();
            //mainLoop();
            //delet all trunks 
            //foreach (GameObject T in GameObject.FindGameObjectsWithTag("Player"))
            //{ Destroy(T); }

        }
    }
    private void centerTurtle()
    {
        //places turtle in the middle of the screen, 20X closer than the far plain
        turtle.transform.position = new Vector3(0.5f, 0.5f, 0.0f);
        //turtle.transform.rotation = Quaternion.LookRotation(Vector3.forward);
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
                case '[':
                    OnStack();
                    break;
                case ']':
                    OffStack();
                    break;
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
        if (needsCorrecting == 1)
        {
            inter = inter.Replace(inter, leftCorrecter);
            Debug.Log(inter);
            needsCorrecting = 0;
            mainLoop();

            inter = inter.Replace(inter, "fff");
        }
        if (needsCorrecting == 2)
        {
            inter = inter.Replace(inter, rightCorrecter);
            Debug.Log(inter);
            needsCorrecting = 0;
            mainLoop();

            inter = inter.Replace(inter, "fff");
        }
        //if (needsCorrecting == 0)
        //{
        //    inter = inter.Replace(inter, tempInter);
        //    Debug.Log(inter);
        //}
    }


    private void F2D()
    {
        //create trunk at turtles location and rotation
        Instantiate(line, turtle.transform.position, turtle.transform.rotation);
        //move turtle forward
        turtle.transform.Translate(Vector3.up);
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

    private void OnStack()
    { //place turtles position into the position stack
        Vector3 tempPos = turtle.transform.position;
        thePosStack.Push(tempPos);
        //place turtles rotation into the rotation stack
        Quaternion tempRot = turtle.transform.rotation;
        theRotStack.Push(tempRot);
    }

    private void OffStack()
    {
        // move turtle to position on top of stack
        turtle.transform.position = thePosStack.Pop();
        //rotate turtle to rotation on stack
        turtle.transform.rotation = theRotStack.Pop();
    }
}




