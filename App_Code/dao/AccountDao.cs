using System;
using System.Collections.Generic;
using System.Web;
using Dapper;
using System.Linq;

/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class AccountDao
{
    public AccountDao()
    {
    }

    public static Account getAccount(string account, string password)
    {

        string sqlCmd = "SELECT * FROM PDFTAG.dbo.Account WHERE USER_AD = @account AND PASSWORD = @pwd and isshow=0";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<Account>(sqlCmd, new { account = account, pwd = password }).FirstOrDefault();
            return res;
        }
    }

    public static Vendor getVendorAccount(string account, string password)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[Vendor] WHERE account = @account AND pwd = @pwd";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<Vendor>(sqlCmd, new { account = account, pwd = password }).FirstOrDefault();
            return res;
        }
    }

    public static Vendor getVendorAccount(string account)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[Vendor] WHERE account = @account";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<Vendor>(sqlCmd, new { account = account }).FirstOrDefault();
            return res;
        }
    }

    public static Personal getPersonalAccount(string account, string password)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[Personal] WHERE account = @account AND pwd = @pwd";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<Personal>(sqlCmd, new { account = account, pwd = password }).FirstOrDefault();
            return res;
        }
    }

    public static Personal getPersonalAccount(string account)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[Personal] WHERE account = @account";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<Personal>(sqlCmd, new { account = account }).FirstOrDefault();
            return res;
        }
    }

    public static People getPeopleAccount(string account, string password)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[People] WHERE paccount = @paccount AND pwd = @pwd";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<People>(sqlCmd, new { paccount = account, pwd = password }).FirstOrDefault();
            return res;
        }
    }

    public static People getPeopleAccount(string account)
    {

        string sqlCmd = "SELECT * FROM [Member].[dbo].[People] WHERE paccount = @paccount";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<People>(sqlCmd, new { paccount = account }).FirstOrDefault();
            return res;
        }
    }
}
public class Account
{
    public Account()
    {
    }

    public long aid { get; set; }

    /// <summary>
    /// 英文姓名
    /// </summary>
    public string USER_NAME { get; set; }
    /// <summary>
    /// 中文姓名
    /// </summary>
    public string USER_FULLNAME { get; set; }

    /// <summary>
    /// 帳號
    /// </summary>
    public string USER_AD { get; set; }
    public string DEPARTMEN { get; set; }
    public string CUST_NO { get; set; }
    public int Role { get; set; }

}
public class Vendor
{
    public Vendor()
    {
    }

    public long vid { get; set; }
    public int accounty { get; set; }
    public string companyname { get; set; }
    public string taxid { get; set; }
    public string conperson { get; set; }
    public string address { get; set; }
    public string tel { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string filldate { get; set; }
    public int isShow { get; set; }
    public string account { get; set; }
    public string pwd { get; set; }
    public int isCert { get; set; }
    public string reviewaccount { get; set; }
    public string reviewdate { get; set; }
    public int vstatus { get; set; }

}

public class Personal
{
    public Personal()
    {
    }

    public long pid { get; set; }
    public int accounty { get; set; }
    public string pname { get; set; }
    public string snid { get; set; }
    public string address { get; set; }
    public string tel { get; set; }
    public string phone { get; set; }
    public string email { get; set; }
    public string filldate { get; set; }
    public int isShow { get; set; }
    public string account { get; set; }
    public string pwd { get; set; }
    public int isCert { get; set; }
    public string reviewaccount { get; set; }
    public string reviewdate { get; set; }
    public int vstatus { get; set; }

}

public class People
{
    public People()
    {
    }

    public long pid { get; set; }
    public long vid { get; set; }
    public string paccount { get; set; }
    public string pname { get; set; }
    public string pwd { get; set; }
    public string email { get; set; }
    public int pstatus { get; set; }
    public string creator { get; set; }
    public string creatordate { get; set; }
    public int isdel { get; set; }

}