using UnityEngine;
using System.Collections;

public class MovablePiece : MonoBehaviour
{

    private GamePiece piece;
    private IEnumerator moveCoroutine;

    //Is movable booleans
    private bool movable;
    private bool isFilling;

    //Grid script refrences
    private GameObject gridRef;
    private Grid grid;

    void Awake()
    {
        //This game piece script reference
        piece = GetComponent<GamePiece>();

        //Grid refrence
        GameObject gridRef = GameObject.Find("Grid");
        Grid grid = gridRef.GetComponent<Grid>();
        isFilling = grid.isFilling;

        movable = true;
    }

    public void Move(int newX, int newY, float time)
    {
        if (movable)
        {
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }

            moveCoroutine = MoveCoroutine(newX, newY, time);
            StartCoroutine(moveCoroutine);
        }
    }

    public IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        piece.X = newX;
        piece.Y = newY;

        Vector3 startPos = transform.position;
        Vector3 endPos = piece.GridRef.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            piece.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        piece.transform.position = piece.GridRef.GetWorldPosition(newX, newY);
    }

    //Wall collision attempt
    //Make piece non-movable on collision, make movable on exit;
    //public void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (isFilling == false)
    //    {
    //        if (other.gameObject.tag == "Line")
    //        {
    //            movable = false;
    //            print("not movable");
    //        }
    //    }
    //}

    //public void OnTriggerExit2D(Collider2D other)
    //{
    //    movable = true;
    //    print("movable");
    //}
}
