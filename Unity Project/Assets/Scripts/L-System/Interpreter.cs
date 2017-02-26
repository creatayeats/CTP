using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{

    public string interSting;//move to private
    public GameObject turtle;
    public GameObject trunk;

    public float angle;
    public float rotationOfTrunk;
    public int mode; //cant pass enum into switch witout converting to int, so just have int instead

    private static Stack<Vector3> thePosStack = new Stack<Vector3>();
    private static Stack<Quaternion> theRotStack = new Stack<Quaternion>();
    private ReWriter reWrite;


    void Start()
    {
        //get ReWriting script
        reWrite = GetComponent<ReWriter>();
    }

    void Update()
    {
        //if generate bool is true
        if (reWrite.getGen())
        {
            //get the final string for interptiation
            interSting = reWrite.getFinalString();
            //place turtle in center of screen
            centerTurtle();
            mainLoop();
            //turn gen back off
            reWrite.setGen(false);
        }

        //if reset key pressed 
        if (Input.GetKeyDown("r"))
        {
            //delete all trunks 
            foreach (GameObject T in GameObject.FindGameObjectsWithTag("Player"))
            { Destroy(T); }

        }
    }
    private void centerTurtle()
    {
        //places turtle in the middle of the screen, 20X closer than the far plain
        turtle.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.farClipPlane / 20));
        turtle.transform.rotation = Quaternion.LookRotation(Vector3.forward);
    }

    private void mainLoop()
    {
        //look at each character in the final string
        foreach (char c in interSting)
        {
            switch (c)
            {
                case 'f':
                case 'F':
                    switch (mode)
                    {
                        case 1:
                            F2D();
                            break;
                        case 2:
                            FTree();
                            //extra mode
                            break;
                        default:
                            break;
                    }
                    break;

                case '[':
                    OnStack();
                    break;

                case ']':
                    OffStack();
                    break;

                case '+':
                    switch (mode)
                    {
                        case 1:
                            P2D();
                            break;
                        case 2:
                            PTree();
                            //extra mode
                            break;
                        default:
                            break;
                    }
                    break;

                case '-':
                case '−':
                    switch (mode)
                    {
                        case 1:
                            N2D();
                            break;
                        case 2:
                            NTree();
                            //extra mode
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    print("default");
                    break;
            }
        }
    }

    private void F2D()
    {
        //create trunk at turtles location and rotation
        Instantiate(trunk, turtle.transform.position, turtle.transform.rotation);
        //move turtle forward
        turtle.transform.Translate(Vector3.up * 2);
    }

    private void P2D()
    {
        //angle right
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, -angle));
    }

    private void N2D()
    {
        //angle left
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, angle));
    }

    //3d tree specific functions

    //comon functions
    private void FTree()
    {
        //create trunk at turtles location and rotation
        Instantiate(trunk, turtle.transform.position, turtle.transform.rotation);
        //move turtle forward
        turtle.transform.Translate(Vector3.up * 2);
        //add rotation for simple 3d 
        turtle.transform.Rotate(new Vector3(0f, rotationOfTrunk, 0f));
    }

    //varience in angle
    private void PTree()
    {
        //angle right with slight varience 
        float tempAngle = Random.Range(angle - 5, angle + 5);
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, -tempAngle));
    }

    private void NTree()
    {
        //angle left with slight varience
        float tempAngle = Random.Range(angle - 5, angle + 5);
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, tempAngle));
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




