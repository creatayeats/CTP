using UnityEngine;
using System.Collections;

public class ClearColourPiece : ClearablePiece {

	private ColourPiece.ColourType colour;

	public ColourPiece.ColourType Colour
    {
		get { return colour; }
		set { colour = value; }
	}

	public override void Clear()
	{
		base.Clear ();
		piece.GridRef.ClearColour (colour);
	}
}
