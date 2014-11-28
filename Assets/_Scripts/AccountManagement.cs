﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class AccountManagement : MonoBehaviour {
	public InputField registerU;
	public InputField registerP;
	public InputField registerPC;
	public InputField loginU;
	public InputField loginP;
	public TextMesh currentUName;

	public Account activeAccount;

	private AccountList list_of_accounts;
	private Account log_acc;

	void Start ()
	{
		list_of_accounts = new AccountList ();
		log_acc = new Account ("", "");
		activeAccount = new Account ("", "");

		using (StreamReader sread = new StreamReader("Accounts.txt"))
		{
			list_of_accounts = AccountList.readAccounts(sread); //Reading in all the Accounts
			sread.Close ();
		}
	}
	/* Lees de username en password in
	 */
	public void registerAccount ()
	{
		string username = registerU.text.text.ToString();
		string password = registerP.text.text.ToString();
		string passwordC = registerPC.text.text.ToString();
		registerAccount (username, password, passwordC);
	}
	/* Lokaal registreren van account; dit doet alleen de Server
	 */
	public void registerAccount (string Uname, string Pword, string PwordC)
	{
		if (Pword.Equals(PwordC))
		{
			Account reg_acc = new Account (Uname, Pword);
			if (!list_of_accounts.containsUsername(reg_acc))
			{
				registerU.value = "";
				registerP.value = "";
				registerPC.value = "";

				list_of_accounts.addAccount(reg_acc);
				using (StreamWriter swrite = new StreamWriter ("Accounts.txt", true))
				{
					Account.writeAccount (reg_acc, swrite);
					swrite.Close ();
				}
				Debug.Log("Account: " + reg_acc.Name + " created");
			}
			else
			{
				Debug.Log("Username not available");
			}
		}
		else
		{
			Debug.Log("Passwords don't coincide");
		}
	}
	/* Lees de username, het password en het team in
	 */
	public void loginAccount ()
	{
		string username = loginU.text.text.ToString();
		string password = loginP.text.text.ToString();
		//string team = chooseTeamText.text.ToString();
		loginAccount (username, password);
		//networkView.RPC("loginAccServer", RPCMode.AllBuffered, username, password);
	}
	/* Login op een account en geef de username en het gekozen team mee aan de Server
	 */
	public void loginAccount (string Uname, string Pword) //,string Team)
	{
		log_acc.Name = Uname;
		log_acc.Word = Pword;
		using (StreamReader slread = new StreamReader("Accounts.txt"))
		{
			list_of_accounts = AccountList.readAccounts(slread);
			slread.Close ();
		}

		int i = 0;
		while (i < list_of_accounts.sizeList)
		{
			Account acc2 = list_of_accounts.indexOf(i);
			if (log_acc.equals(acc2))
			{
				Debug.Log("Account: " + this.log_acc.Name + "; " + this.log_acc.Word);
				break;
			}
			i++;
		}

		if (i != this.list_of_accounts.sizeList) //Dus account bestaat en password is correct
		{
			loginU.value = "";
			loginP.value = "";
			activeAccount.Name = log_acc.Name;
			activeAccount.Word = log_acc.Word;
			currentUName.text = activeAccount.Name;
		}
		else
		{
			Debug.Log("Login info incorrect");
		}
	}
}