using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using Dapper;
using System.Linq;
/// <summary>
/// MarqeeDao 的摘要描述
/// </summary>
public class LocalAccountDao
{
    public LocalAccountDao()
    {
    }

    public static LocalAccount getLocalAccount(string account, string password)
    {
        string sqlCmd = "SELECT * FROM [PDFTAG].[dbo].[LocalAccount] WHERE account = @account AND password = @password";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query<LocalAccount>(sqlCmd, new { account = account, password = password }).FirstOrDefault();
            return res;
        }
    }

    public static LocalAccount getLocalAccount(string account)
    {
        LocalAccount obj = null;
        string sqlCmd = "SELECT * FROM [PDFTAG].[dbo].[LocalAccount] WHERE account=@account";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var list = cn.Query<LocalAccount>(sqlCmd, new { account = account });

            foreach (var item in list)
            {
                obj = new LocalAccount() { account = item.account, password = item.password, name = item.name };
            }

            return obj;
        }
    }
}

public class LocalAccount
{
    public LocalAccount()
    {
    }


    private string _account;
    public string account
    {
        get { return _account; }
        set { _account = value; }
    }
    private string _password;
    public string password
    {
        get { return _password; }
        set { _password = value; }
    }
    private string _name;
    public string name
    {
        get { return _name; }
        set { _name = value; }
    }

    private int _type;
    public int type
    {
        get { return _type; }
        set { _type = value; }
    }

    public int role
    {
        get;
        set;
    }
}