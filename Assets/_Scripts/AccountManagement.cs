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
	public Text currentUName;
	public bool loginServer;
	public NW_Server networkServer;
	public MenuFunctions menuF;
	
	public static bool loggedIn;
	
	private AccountList list_of_accounts;
	private Account log_acc;
	
	void Start ()
	{
		list_of_accounts = new AccountList ();
		log_acc = new Account ("", "");
		loggedIn = true;
		loginServer = true;
		if (BasicFunctions.firstStart)
		{
			BasicFunctions.activeAccount = new Account("Debug", "-");
		}
		//loggedIn = false;
		
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
		string username = registerU.text.ToString();
		string password = registerP.text.ToString();
		string passwordC = registerPC.text.ToString();
		registerAccount (username, password, passwordC);
	}
	/* Lokaal registreren van account; dit doet alleen de Server
	 */
	public void registerAccount (string Uname, string Pword, string PwordC)
	{
		if (Pword.Equals(PwordC))
		{
			if (loginServer)
			{
				string url = "http://drproject.twi.tudelft.nl:8082/Register?playerName="+Uname+"&playerPassword="+Pword;
				WWW www = new WWW(url);
				StartCoroutine(WaitForRegistration(www));
			}
			else
			{
				Account reg_acc = new Account (Uname, Pword);
				if (Uname == " ")
				{
					Debug.Log("No Username is given");
				}
				else if (!list_of_accounts.containsUsername(reg_acc))
				{
					registerU.text = "";
					registerP.text = "";
					registerPC.text = "";
					
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
		}
		else
		{
			Debug.Log("Passwords don't match");
		}
	}
	/* Lees de username, het password en het team in
	 */
	public void loginAccount ()
	{
		string username = loginU.text.ToString();
		string password = loginP.text.ToString();
		loginAccount (username, password);
	}
	/* Login op een account en geef de username en het gekozen team mee aan de Server
	 */
	public void loginAccount (string Uname, string Pword) //,string Team)
	{
		log_acc.Name = Uname;
		log_acc.Word = Pword;
	
		if (loginServer)
		{
			Debug.Log ("SERVER");
			string url = "http://drproject.twi.tudelft.nl:8082/Authenticate?playerName="+Uname+"&playerPassword="+Pword;
			WWW www = new WWW(url);
			StartCoroutine(WaitForAuthorization(www));
		}
		else
		{
			Debug.Log("NOTSERVER");
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
				loginU.text = "";
				loginP.text = "";
				BasicFunctions.activeAccount = new Account(log_acc.Name, log_acc.Word);
				currentUName.text = BasicFunctions.activeAccount.Name;
				loggedIn = true;
			}
			else
			{
				Debug.Log("Login info incorrect");
			}
		}
	}

	IEnumerator WaitForRegistration(WWW www)
	{
		yield return www;

		if (www.error == null){
			if(www.text.Equals ("Succesfully Registered")){
				Debug.Log ("Succesfully Registered");

			}
			else{
				Debug.Log ("Failed to register");
			}
		}

		registerU.text = "";
		registerP.text = "";
		registerPC.text = "";
	}

	IEnumerator WaitForAuthorization(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			if(www.text.Equals("Succesfully Authorized")){
				//activeAccount.Name = log_acc.Name;
				//activeAccount.Word = log_acc.Word;
				BasicFunctions.activeAccount = new Account(log_acc.Name, log_acc.Word);
				currentUName.text = BasicFunctions.activeAccount.Name;
				loggedIn = true;
				menuF.isLoggedIn = true;
			}
			else{
				Debug.Log("Login info incorrect");
			}
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}  
		loginU.text = "";
		loginP.text = "";
	}
}