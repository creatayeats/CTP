using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class Interpreter : MonoBehaviour
{

    // public string interSting;//move to private
    public GameObject turtle;
    public GameObject trunk;

    private float angle = 90;
    private string inter;

    private static Stack<Vector3> thePosStack = new Stack<Vector3>();
    private static Stack<Quaternion> theRotStack = new Stack<Quaternion>();



    void Start()
    {
        StreamReader sr = new StreamReader("dragon.txt");
        inter = sr.ReadLine();
        //iterations = int.Parse(sr.ReadLine());
        //stochY = bool.Parse(sr.ReadLine());
        //stochX = bool.Parse(sr.ReadLine());
        //stochF = bool.Parse(sr.ReadLine());
        //ruleX = sr.ReadLine();
        //ruleF = sr.ReadLine();
        //ruleY = sr.ReadLine();
        //ruleBO = sr.ReadLine();
        //ruleBC = sr.ReadLine();
        //ruleN = sr.ReadLine();
        //ruleP = sr.ReadLine();
        //forceSetAngle(float.Parse(sr.ReadLine()));
        Debug.Log(inter);
        sr.Close();

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
        //if (Input.GetKeyDown("r"))
        //{
        //    //delet all trunks 
        //    foreach (GameObject T in GameObject.FindGameObjectsWithTag("Player"))
        //    { Destroy(T); }

        //}
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
                    print("forward");
                    F2D();
                    break;
                case '[':
                    OnStack();
                    break;
                case ']':
                    OffStack();
                    break;
                case '+':
                    print("turn +");
                    PTree();
                    break;
                case '-':
                case '?':
                    print("turn -");
                    NTree();
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
        turtle.transform.Translate(Vector3.up);
    }

    private void PTree()
    {
        //angle right with slight varience 
        // float tempAngle = Random.Range(angle - 5, angle + 5);
        turtle.transform.Rotate(new Vector3(0.0f, 0.0f, angle));
    }

    private void NTree()
    {
        //angle left with slight varience
        //float tempAngle = Random.Range(angle - 5, angle + 5);
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




