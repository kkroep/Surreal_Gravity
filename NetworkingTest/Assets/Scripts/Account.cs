using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class Account : MonoBehaviour {

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
	}

	public string Word
	{
		get { return Password; }
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

	public static Account readAccount (BinaryReader bread)
	{
		string Uname = bread.ReadString();
		string Pword = bread.ReadString();
		Account newAccount = new Account (Uname, Pword);
		return newAccount;
	}

	public static void writeAccount (Account acc, BinaryWriter bwrite)
	{
		string Uname = acc.Username;
		string Pword = acc.Password;
		bwrite.Write (Uname);
		bwrite.Write (Pword);
	}
}
