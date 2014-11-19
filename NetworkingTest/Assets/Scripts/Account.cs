using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class Account {

	private string Username;
	private string Password;

	public Account (string Uname, string Pword)
	{
		Username = Uname;
		Password = Pword;
	}

	public string Name
	{
		get { return Username; }
		set { Username = value; }
	}

	public string Word
	{
		get { return Password; }
		set { Password = value; }
	}

	public bool equals (Account acc)
	{
		bool res = false;
		if (this.Username.Equals(acc.Username) && this.Password.Equals(acc.Password))
		{
			res = true;
		}

		return res;
	}

	public static Account readAccount (StreamReader sread)
	{
		string Uname = sread.ReadLine();
		string Pword = sread.ReadLine();
		Debug.Log("Username: " + Uname);
		Debug.Log("Password: " + Pword);
		Account newAccount = new Account (Uname, Pword);
		return newAccount;
	}

	public static void writeAccount (Account acc, StreamWriter swrite)
	{
		string Uname = acc.Username;
		string Pword = acc.Password;
		swrite.WriteLine (Uname);
		swrite.WriteLine (Pword);
	}
}
