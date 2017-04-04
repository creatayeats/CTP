using UnityEngine;
using System.Collections;

public class GamePiece : MonoBehaviour {

	private int x;
	private int y;

	public int X
	{
		get { return x; }
		set {
			if (IsMovable ())
            {
				x = value;
			}
		}
	}

	public int Y
	{
		get { return y; }
		set {
			if (IsMovable ())
            {
				y = value;
			}
		}
	}

	private Grid.PieceType type;

	public Grid.PieceType Type
	{
		get { return type; }
	}

	private Grid grid;

	public Grid GridRef
	{
		get { return grid; }
	}

	private MovablePiece movableComponent;

	public MovablePiece MovableComponent
	{
		get { return movableComponent; }
	}

	private ColourPiece colourComponent;

	public ColourPiece ColourComponent
	{
		get { return colourComponent; }
	}

	private ClearablePiece clearableComponent;

	public ClearablePiece ClearableComponent {
		get { return clearableComponent; }
	}

	void Awake()
	{
		movableComponent = GetComponent<MovablePiece>();
		colourComponent = GetComponent<ColourPiece>();
		clearableComponent = GetComponent<ClearablePiece>();
	}

	public void Init(int _x, int _y, Grid _grid, Grid.PieceType _type)
	{
		x = _x;
		y = _y;
		grid = _grid;
		type = _type;
	}

	void OnMouseEnter()
	{
		grid.EnterPiece (this);
	}

	void OnMouseDown()
	{
		grid.PressPiece (this);
	}

	void OnMouseUp()
	{
		grid.ReleasePiece ();
	}

	public bool IsMovable()
	{
		return movableComponent != null;
	}

	public bool IsColoured()
	{
		return colourComponent != null;
	}

	public bool IsClearable()
	{
		return clearableComponent != null;
	}
}
