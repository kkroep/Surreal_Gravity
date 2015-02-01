using UnityEngine;
using System.Collections;

public class AccountScore {

	private int Number;
	private int Points;

	public AccountScore (int Number, int Score)
	{
		this.Number = Number;
		this.Points = Score;
	}

	public int Numb
	{
		get { return Number; }
		set { Number = value; }
	}

	public int Score
	{
		get { return Points; }
		set { Points = value; }
	}

	public override string ToString ()
	{
		return Number + " " + Score;
	}
}
