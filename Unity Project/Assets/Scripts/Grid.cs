using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BUBBLE,
        ROW_CLEAR,
        COLUMN_CLEAR,
        RAINBOW,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    //Grid dimensions and grid fill variables
    public int xDim;
    public int yDim;
    public float fillTime;

    //Game board prefabs
    public PiecePrefab[] piecePrefabs;
    public GameObject backgroundPrefab;
    public GameObject[] matchLightDelete;
    public GameObject matchingLight;

    //Container for prefabs and their types
    private Dictionary<PieceType, GameObject> piecePrefabDict;

    //Current game piece arrays
    private GamePiece[,] pieces;

    //System script references and variables
    private PNoiseColour pNoise;
    private GameObject pNoiseRef;
    public bool isFilling;

    private bool inverse = false;

    //Mouse-on-game piece activity
    private GamePiece pressedPiece;
    private GamePiece enteredPiece;

    //Search variables
    private bool bredthFirst;
    private bool depthFirst;
    private int movesMade;

    //Use this for initialization
    void Start()
    {
        //Instantiate search
        isFilling = true;
        bredthFirst = false;
        bredthFirst = false;
        movesMade = 0;

        //Instantiate prefabs and their types
        piecePrefabDict = new Dictionary<PieceType, GameObject>();

        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDict.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDict.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        //Instantiate grid
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = (GameObject)Instantiate(backgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                background.transform.parent = transform;
            }
        }

        //Instantiate game pieces (as empty)
        pieces = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }

        //Instantiate Perlin Noise Coordinates by casting
        GameObject pNoiseRef = GameObject.Find("PNSpawner");
        PNoiseColour pNoise = pNoiseRef.GetComponent<PNoiseColour>();

        //Cast Vector2 floats to ints
        //Messy, is there a better option?
        //Comment out Random Range number before uisng this
        int coord1X = (int)Mathf.Ceil(pNoise.coord1.x);
        int coord1Y = (int)Mathf.Ceil(pNoise.coord1.y);
        int coord2X = (int)Mathf.Ceil(pNoise.coord2.x);
        int coord2Y = (int)Mathf.Ceil(pNoise.coord2.y);
        int coord3X = (int)Mathf.Ceil(pNoise.coord3.x);
        int coord3Y = (int)Mathf.Ceil(pNoise.coord3.y);
        int coord4X = (int)Mathf.Ceil(pNoise.coord4.x);
        int coord4Y = (int)Mathf.Ceil(pNoise.coord4.y);
        int coord5X = (int)Mathf.Ceil(pNoise.coord5.x);
        int coord5Y = (int)Mathf.Ceil(pNoise.coord5.y);

        //Random Range numbers for test purposes
        //Comment out casting before using this
        //int coord1X = Random.Range(0, xDim);
        //int coord1Y = Random.Range(0, yDim);
        //int coord2X = Random.Range(0, xDim);
        //int coord2Y = Random.Range(0, yDim);
        //int coord3X = Random.Range(0, xDim);
        //int coord3Y = Random.Range(0, yDim);
        //int coord4X = Random.Range(0, xDim);
        //int coord4Y = Random.Range(0, yDim);
        //int coord5X = Random.Range(0, xDim);
        //int coord5Y = Random.Range(0, yDim);

        //Casting attempt
        Destroy(pieces[coord1X, coord1Y].gameObject);
        SpawnNewPiece(coord1X, coord1Y, PieceType.BUBBLE);

        Destroy(pieces[coord2X, coord2Y].gameObject);
        SpawnNewPiece(coord2X, coord2Y, PieceType.BUBBLE);

        Destroy(pieces[coord3X, coord3Y].gameObject);
        SpawnNewPiece(coord3X, coord3Y, PieceType.BUBBLE);

        Destroy(pieces[coord4X, coord4Y].gameObject);
        SpawnNewPiece(coord4X, coord4Y, PieceType.BUBBLE);

        Destroy(pieces[coord5X, coord5Y].gameObject);
        SpawnNewPiece(coord5X, coord5Y, PieceType.BUBBLE);

        //Standard spawning procedure
        //Test spawns a rainbow piece(CA)
        Destroy(pieces[7, 5].gameObject);
        SpawnNewPiece(7, 5, PieceType.RAINBOW);

        StartCoroutine(Fill());
    }

    void Update()
    {
        //Activate searches on SPACEBAR on NUMPAD ENTER key
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bredthFirst = true;
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            depthFirst = true;
        }

        //Wall collision debugging 
        //if (Input.GetKeyDown(KeyCode.Backspace))
        //{
        //    isFilling = false;
        //}
        //if (Input.GetKeyDown(KeyCode.CapsLock))
        //{
        //    isFilling = true;
        //}

        //Search all current matches trigger
        if (bredthFirst)
        {
            SearchMatchesBredth();
            bredthFirst = false;
        }
        if (depthFirst)
        {
            SearchMatchesDepth();
            depthFirst = false;
        }

        //Slow down time while the swaps are current
        if (depthFirst || bredthFirst)
        {
            Time.timeScale = 0.1f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    //Game board refill loop
    public IEnumerator Fill()
    {
        bool needsRefill = true;

        while (needsRefill)
        {
            yield return new WaitForSeconds(fillTime);

            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);

            }
            needsRefill = ClearAllValidMatches();
        }
        //Refresh current valid search lights
        matchLightDelete = GameObject.FindGameObjectsWithTag("Light");

        for (int i = 0; i < matchLightDelete.Length; i++)
        {
            Destroy(matchLightDelete[i]);
        }
    }

    //Game board refill procedure
    public bool FillStep()
    {
        bool movedPiece = false;

        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;

                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }

                GamePiece piece = pieces[x, y];

                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];

                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.Move(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];

                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];

                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.Move(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];

            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[PieceType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                newPiece.transform.parent = transform;

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Init(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.Move(x, 0, fillTime);
                pieces[x, 0].ColourComponent.SetColour((ColourPiece.ColourType)Random.Range(0, pieces[x, 0].ColourComponent.NumColours));
                movedPiece = true;
            }
        }
        return movedPiece;
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }

    //Creates new pieces from the dictionary of prefabs
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = (GameObject)Instantiate(piecePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newPiece.transform.parent = transform;

        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Init(x, y, this, type);

        return pieces[x, y];
    }

    //Are the pressed piece and last entered piece adjacent
    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1) || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1);
    }

    //Breadth first search on SPACE
    public void SearchMatchesBredth()
    {
        {
            GamePiece piece1;
            GamePiece piece2;

            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < yDim; y++)
                {
                    //Piece 1
                    if (x < xDim && x + 1 < xDim)
                    {
                        piece1 = pieces[x, y];
                        piece2 = pieces[x + 1, y];
                        SwapPieces(piece1, piece2);
                    }
                }
            }
        }
        Debug.Log(movesMade);
    }

    //Depth first search on NUMPAD ENTER
    public void SearchMatchesDepth()
    {
        GamePiece piece1;
        GamePiece piece2;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                //Piece 1
                if (y < yDim && y + 1 < yDim)
                {
                    piece1 = pieces[x, y];
                    piece2 = pieces[x, y + 1];
                    SwapPieces(piece1, piece2);
                }
            }
        }
        Debug.Log(movesMade);
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            //Core matching piece search function
            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null
                || piece1.Type == PieceType.RAINBOW || piece2.Type == PieceType.RAINBOW)
            {

                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.Move(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.Move(piece1X, piece1Y, fillTime);
                movesMade++;

                //If one of the pieces is a rainbow, clear in this function with set colour
                if (piece1.Type == PieceType.RAINBOW && piece1.IsClearable() && piece2.IsColoured())
                {
                    ClearColourPiece clearColour = piece1.GetComponent<ClearColourPiece>();

                    if (clearColour)
                    {
                        clearColour.Colour = piece2.ColourComponent.Colour;
                    }

                    ClearPiece(piece1.X, piece1.Y);
                    movesMade++;
                }
                
                if (piece2.Type == PieceType.RAINBOW && piece2.IsClearable() && piece1.IsColoured())
                {
                    ClearColourPiece clearColour = piece2.GetComponent<ClearColourPiece>();

                    if (clearColour)
                    {
                        clearColour.Colour = piece1.ColourComponent.Colour;
                    }

                    ClearPiece(piece2.X, piece2.Y);
                    movesMade++;
                }

                ClearAllValidMatches();

                //If either pieces in the swap are special, clear in these functions
                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y);
                    movesMade++;
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                    movesMade++;
                }

                pressedPiece = null;
                enteredPiece = null;

                StartCoroutine(Fill());
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }
    //Ref to piece on mouse click
    public void PressPiece(GamePiece piece)
    {
        pressedPiece = piece;
    }
    //Ref to last game piece mouse is hovering over
    public void EnterPiece(GamePiece piece)
    {
        enteredPiece = piece;
    }
    //Ref to releasing clicked game piece
    public void ReleasePiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }

    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColoured())
        {
            ColourPiece.ColourType colour = piece.ColourComponent.Colour;
            //Lists for collating matches
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            //First check horizontal
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    { //Left
                        x = newX - xOffset;
                    }
                    else
                    { //Right
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= xDim)
                    {
                        break;
                    }
                    //If pieces match, add them to the list
                    if (pieces[x, newY].IsColoured() && pieces[x, newY].ColourComponent.Colour == colour)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                //Add the hroizontal matching pieces, to the list for all matching pieces
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                    //Spawn a light in the corner of matching pieces
                    GameObject matchLight = (GameObject)Instantiate(matchingLight, horizontalPieces[i].transform.position, horizontalPieces[i].transform.rotation);
                }
            }

            //Traverse vertically if we found a match (for L and T shapes)
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;

                            if (dir == 0)
                            { //Up
                                y = newY - yOffset;
                            }
                            else
                            { //Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break;
                            }
                            //If pieces match, add them to the list
                            if (pieces[horizontalPieces[i].X, y].IsColoured() && pieces[horizontalPieces[i].X, y].ColourComponent.Colour == colour)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //Clear the list if there is no match of 3 or over
                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        //Otherwise add pieces to matching list
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }

                        break;
                    }
                }
            }
         
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }

            //Didn't find anything going horizontally first,
            //so now check vertically
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    { //Up
                        y = newY - yOffset;
                    }
                    else
                    { //Down
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= yDim)
                    {
                        break;
                    }
                    //If pieces match, add them to the list
                    if (pieces[newX, y].IsColoured() && pieces[newX, y].ColourComponent.Colour == colour)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                //Add the vertical matching pieces, to the list for all matching pieces
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                    //Spawn a light in the corner of matching pieces
                    GameObject matchLight = (GameObject)Instantiate(matchingLight, verticalPieces[i].transform.position, verticalPieces[i].transform.rotation);
                }
            }

            //Traverse horizontally if we found a match (for L and T shapes)
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            { //Left
                                x = newX - xOffset;
                            }
                            else
                            { //Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= xDim)
                            {
                                break;
                            }
                            //If pieces match, add them to the list
                            if (pieces[x, verticalPieces[i].Y].IsColoured() && pieces[x, verticalPieces[i].Y].ColourComponent.Colour == colour)
                            {
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    //Clear the list if there is no match of 3 or over
                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        //Otherwise add pieces to matching list
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }

        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);

                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X;
                        int specialPieceY = randomPiece.Y;

                        //If match 4, clear pieces and spawn special row or column clear pieces
                        //Depending on the pressed piece movement makes the piece type
                        //If that is unknown, randomly choose
                        if (match.Count == 4)
                        {
                            if (pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR);
                            }
                            
                            else if (pressedPiece.Y == enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR;
                            }
                           
                            else
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR;
                            }
                        }
                        //If 5 match, spawn the rainbow piece
                        else if (match.Count >= 5)
                        {
                            specialPieceType = PieceType.RAINBOW;
                        }
                        //Clear all the pieces in the list and refill the slots
                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X;
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }

                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType);

                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR)
                                && newPiece.IsColoured() && match[0].IsColoured())
                            {
                                newPiece.ColourComponent.SetColour(match[0].ColourComponent.Colour);
                            }
                            else if (specialPieceType == PieceType.RAINBOW && newPiece.IsColoured())
                            {
                                newPiece.ColourComponent.SetColour(ColourPiece.ColourType.ANY);
                            }
                        }
                    }
                }
            }
        }

        return needsRefill;
    }
    //Clears standard matching pieces
    public bool ClearPiece(int x, int y)
    {
        //Removes the pieces if it is a clearable piece, and it isn't already being cleared
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            //Clear piece and create an empty slot
            pieces[x, y].ClearableComponent.Clear();
            SpawnNewPiece(x, y, PieceType.EMPTY);          

            ClearObstacles(x, y);

            return true;
        }
        return false;
        
    }

    //This is used to destroy the bubble pieces, however the functionality has been turned off for testing
    //To turn on, add the ClearablePiece Script to the BubblePiece prefab
    //..and add the BubblePieceClear Animator
    public void ClearObstacles(int x, int y)
    {
        //If the bubble is next to a clear on the X axis, clear
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BUBBLE && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.Clear();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);
                }
            }
        }
        //If the bubble is next to a clear on the Y axis, clear
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x, adjacentY].Type == PieceType.BUBBLE && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.Clear();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }
    //Clears all pieces in a row
    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row);
        }
    }
    //Clears all pieces in a column
    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y);
        }
    }
    //Clears all pieces in a colour
    //This is the core functionality for the Ceullular Automata
    public void ClearColour(ColourPiece.ColourType colour)
    {
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                //ColourType.ANY Represents the CA
                //It clears colours it is swapped with
                if (pieces[x, y].IsColoured() && (pieces[x, y].ColourComponent.Colour == colour
                    || colour == ColourPiece.ColourType.ANY))
                {
                    ClearPiece(x, y);
                }
            }
        }
    }
}
