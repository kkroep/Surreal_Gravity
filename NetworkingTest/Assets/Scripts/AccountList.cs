using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class AccountList {

	private List<Account> Acclist;

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

	public bool containsUsername (Account acc)
	{
		bool res = false;
		string uname = acc.Name;
		int i = 0;
		while (i < Acclist.Count && res == false)
		{
			if (Acclist[i].Name.Equals(uname))
			{
				res = true;
			}
			i++;
		}

		return res;
	}

	public static AccountList readAccounts (StreamReader sread)
	{
		AccountList al = new AccountList ();
		int i = 0;
		while (sread.Peek() != -1)
		{
			Account acc = Account.readAccount(sread);
			al.addAccount(acc);
			i++;
		}
		sread.Close();
		return al;
	}

}
