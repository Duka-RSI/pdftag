using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

/// <summary>
/// LoginUser 的摘要描述
/// </summary>
public class LoginUser
{
	//廠商會員(1)、個人會員(2)、訪客(3)、管理者(99)
	public const int ROLE_ADMIN = 99;
	public const int ROLE_Vendor = 1;
	public const int ROLE_Personal = 2;
	public const int ROLE_Guest = 3;
	public const int ROLE_People = 4;
	public const int ROLE_User = 5;


	#region 公用屬性
	/// <summary>
	/// Primary Key
	/// </summary>
	public static String PK
	{
		get
		{
			return (String)HttpContext.Current.Session["PK"];
		}
		set
		{
			HttpContext.Current.Session["PK"] = value;
			System.Web.Security.FormsAuthentication.SetAuthCookie(value, true);
		}
	}

	public static String Name
	{
		get
		{
			return (String)HttpContext.Current.Session["Name"];
		}
		set
		{
			HttpContext.Current.Session["Name"] = value;
		}
	}

	public static int role
	{
		get
		{
			return Convert.ToInt32(HttpContext.Current.Session["role"]);
		}
		set
		{
			HttpContext.Current.Session["role"] = value;
		}
	}

	public static int role2
	{
		get
		{
			return Convert.ToInt32(HttpContext.Current.Session["role2"]);
		}
		set
		{
			HttpContext.Current.Session["role2"] = value;
		}
	}

	public static List<String> CUST_NO
	{
		get
		{
			return (List<String>)HttpContext.Current.Session["CUST_NO"];
		}
		set
		{
			HttpContext.Current.Session["CUST_NO"] = value;
		}
	}

	#endregion
	public LoginUser()
	{
	}

	public static void logout()
	{
		LoginUser.PK = null;
		LoginUser.Name = null;
		LoginUser.role = 0;
		LoginUser.role2 = 0;
	}
	public static bool validateUser(string account, string password)
	{
		//使用者
		Account accountUser = AccountDao.getAccount(account, password);
		if (accountUser != null)
		{
			LoginUser.PK = accountUser.USER_AD;
			LoginUser.Name = accountUser.USER_NAME + "-" + accountUser.USER_FULLNAME;

			if (accountUser.Role == 1)
			{
				LoginUser.role = ROLE_ADMIN;
				LoginUser.role2 = ROLE_User;
				LoginUser.CUST_NO = string.IsNullOrEmpty(accountUser.CUST_NO) ? new List<string>() : accountUser.CUST_NO.Split(',').ToList();
			}
			else
			{
				LoginUser.role = ROLE_User;
				LoginUser.role2 = ROLE_User;
			}

			LoginUser.CUST_NO = string.IsNullOrEmpty(accountUser.CUST_NO) ? new List<string>() : accountUser.CUST_NO.Split(',').ToList();

			return true;
		}

		//Admin
		LocalAccount localAccount = LocalAccountDao.getLocalAccount(account, password);
		if (localAccount != null)
		{
			LoginUser.PK = localAccount.account;
			LoginUser.Name = localAccount.name;
			LoginUser.role = ROLE_ADMIN; ;
			LoginUser.role2 = ROLE_ADMIN;
			LoginUser.CUST_NO = new List<string>() { "1", "2", "3" };

			return true;
		}

		return false;
	}

	public static bool isLogin()
	{
		if (LoginUser.PK == null || LoginUser.PK == "")
		{
			return false;
		}
		return true;
	}

}
