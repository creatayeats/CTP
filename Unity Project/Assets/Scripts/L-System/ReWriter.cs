using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;


public class ReWriter : MonoBehaviour
{
    public string Axiom;
    public string StringA;//move to private
    public string StringB;//move to private
    public int iterations;
    public bool stochY;
    public bool stochX;
    public bool stochF;
    public int stochChance;

    //  \/move to private \/ - use ui for inputs
    public string ruleX;
    public string ruleF;
    public string ruleY;
    public string ruleBO;
    public string ruleBC;
    public string ruleN;
    public string ruleP;

    private bool flip = true;  //true = A    false = B
    private bool generate = false;
    private string fileName;

    //file name ui setup
    InputField input;
    InputField.SubmitEvent se;

    void Start()
    {
        //set up everything
        Axiom = "";
        iterations = 1;
        ruleX = "x";
        ruleF = "f";
        ruleY = "y";
        ruleBO = "[";
        ruleBC = "]";
        ruleP = "+";
        ruleN = "-";
        stochY = false;
        stochX = false;
        stochF = false;
        //set up listening to input field for filename
        input = GameObject.FindGameObjectWithTag("Input").GetComponent<InputField>();
        se = new InputField.SubmitEvent();
        se.AddListener(SubmitInput);
        input.onEndEdit = se;

    }

    void Update()
    {
        //if space is pressed, run for x amount of iterations
        if (Input.GetKeyDown("space"))
        {
            //  if strings are empty, use axiom
            if (StringA == "" && StringB == "")
            {
                StringA = Axiom;
                //rewrite string for n iterations
                for (int i = 0; i < iterations; i++)
                {
                    rewrite();
                }
            }
            else
            {
                for (int i = 0; i < iterations; i++)
                {
                    rewrite();
                }
            }

        }
        // resets inputs
        if (Input.GetKeyDown("r"))
        { reset(); }
        //shortcut for generation
        if (Input.GetKeyDown("p"))
        { generate = true; }
    }

    private void reset()
    {
        //clear strings
        deleteA();
        deleteB();
        //set flip to default 
        flip = true;
    }

    private void rewrite()
    {
        //write to stringB from A
        if (flip)
        {
            deleteB();
            StringB = stringReWrite(StringA);
            flip = false;
        }
        //write to stringA from B
        else
        {
            deleteA();
            StringA = stringReWrite(StringB);
            flip = true;
        }
    }

    private string stringReWrite(string readFrom)
    {
        //i keeps track of where in the new string to add to
        int i = 0;
        //temp string to write to
        string writeTo = "";

        //look at every character in the string
        foreach (char c in readFrom)
        {
            switch (c)
            {
                
                case 'f':
                case 'F':
                    if (stochF)
                    {
                        if (rndBool(stochChance))
                        {
                            writeTo = writeTo.Insert(i, ruleF);
                            i += ruleF.Length;
                        }
                        else
                        {
                            writeTo = writeTo.Insert(i, "f");
                            i++;
                        }
                    }
                    else
                    {
                        writeTo = writeTo.Insert(i, ruleF);
                        i += ruleF.Length;
                    }
                    break;
              
                case '[':

                    writeTo = writeTo.Insert(i, ruleBO);
                    i += ruleBO.Length;
                    break;

                case ']':

                    writeTo = writeTo.Insert(i, ruleBC);
                    i += ruleBC.Length;
                    break;

                case '+':

                    writeTo = writeTo.Insert(i, ruleP);
                    i += ruleP.Length;
                    break;

                case '-':
                case '−':
                    writeTo = writeTo.Insert(i, ruleN);
                    i += ruleN.Length;
                    break;
                default:
                    //debug
                    break;
            }

        }
        //sends back new string based on old string
        return writeTo;
    }


    private void deleteA()//Clears string A
    {
        int i = StringA.Length;
        StringA = StringA.Remove(0, i);
    }

    private void deleteB()//Clears string B
    {
        int i = StringB.Length;
        StringB = StringB.Remove(0, i);
    }

    public string getFinalString()//Getter for strings
    {
        if (flip)
        { return StringA; }
        else
        { return StringB; }

    }

    private void SubmitInput(string arg0)
    {  
        //print(arg0);
        fileName = arg0;
    }

    public void save()
    {
        //open file of name given
        //write a line for any info to be stored
        StreamWriter sw = new StreamWriter(fileName + ".txt");
        sw.WriteLine(Axiom);
        sw.WriteLine(iterations);
        sw.WriteLine(stochY);
        sw.WriteLine(stochX);
        sw.WriteLine(stochF);
        sw.WriteLine(ruleX);
        sw.WriteLine(ruleF);
        sw.WriteLine(ruleY);
        sw.WriteLine(ruleBO);
        sw.WriteLine(ruleBC);
        sw.WriteLine(ruleN);
        sw.WriteLine(ruleP);
        sw.WriteLine(getAngle());
        sw.Close();
    }

    public void load()
    {
        //open file and read in in same order as saved
        StreamReader sr = new StreamReader(fileName + ".txt");
        Axiom = sr.ReadLine();
        iterations = int.Parse(sr.ReadLine());
        stochY = bool.Parse(sr.ReadLine());
        stochX = bool.Parse(sr.ReadLine());
        stochF = bool.Parse(sr.ReadLine());
        ruleX = sr.ReadLine();
        ruleF = sr.ReadLine();
        ruleY = sr.ReadLine();
        ruleBO = sr.ReadLine();
        ruleBC = sr.ReadLine();
        ruleN = sr.ReadLine();
        ruleP = sr.ReadLine();
        forceSetAngle(float.Parse(sr.ReadLine()));
        sr.Close();

    }

    private bool rndBool(float value)//given value 0-100 returns a bool based on that % chance
    {
        if (Random.value >= value / 100)//between 0-1
        { return true; }
        else
        { return false; }
    }

    private float getAngle()//gets angle from interpriter
    {
        Interpreter inter = GetComponent<Interpreter>();
        return inter.angle;
    }

    private void forceSetAngle(float m_angle)//sets interpriter angle
    {
        Interpreter inter = GetComponent<Interpreter>();
        inter.angle = m_angle;
    }

    public bool getGen()//returns if generate is true
    { return generate; }

    public void setGen(bool setter)//sets generate
    { generate = setter; }
}








