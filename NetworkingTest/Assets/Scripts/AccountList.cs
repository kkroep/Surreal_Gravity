using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class AccountList : MonoBehaviour {

	private List<Account> Acclist;
    //string pathoffile = @"C:\Users\Roby\Documents\TU Delft\Minor\UnityProjects\NetworkingTest\Accounts.txt";

	public AccountList ()
	{
		Acclist = new List<Account> ();
	}

	public int sizeList
	{
		get { return Acclist.Count; }
	}

	public Account indexOf (int i)
	{
		return Acclist[i];
	}

	public void addAccount (Account acc)
	{
		if (!Acclist.Contains(acc))
		{
			Acclist.Add(acc);
		}
	}

	public static AccountList readAccounts ()
	{
		FileStream stream = new FileStream (@"C:\Users\Roby\Documents\TU Delft\Minor\UnityProjects\NetworkingTest\Accounts.txt", FileMode.Open);
		BinaryReader bread = new BinaryReader (stream);
		List<Account> acclijst = new List<Account> ();
		AccountList al = new AccountList ();
		int i = 0;
		while(i < 3)
		{
			string name = bread.ReadString();
			string password = bread.ReadString();
			Debug.Log("Name: " + name + ", Pass: " + password);
			Account acc = new Account (name, password);
			acclijst.Add(acc);
			al.addAccount(acclijst[i]);
			i++;
		}
		bread.Close();
		return al;
	}

}
