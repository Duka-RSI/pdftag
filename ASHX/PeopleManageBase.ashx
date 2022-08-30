<%@ WebHandler Language="C#" Class="Control" %>

using System;
using System.Web;
using Newtonsoft.Json;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

public class Control : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        switch (context.Request["fun"])
        {
            case "get":
                get(context);
                break;
                     case "deletefile":
                deletefile(context);
                break;
        }
    }

    public void get(HttpContext context)
    {
        string peomid = context.Request["peomid"];
        string sSql = "";

        sSql = "select * from PeopleManageBase \n";
        sSql += " where 1=1  \n";
        sSql += " and peomid=" + peomid + "  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
 public void deletefile(HttpContext context)
    {
        string peomid = context.Request["peomid"];
        string sSql = "";

        sSql = "update  PeopleManageBase set peofilepath='' \n";
        sSql += " where 1=1  \n";
        sSql += " and peomid=" + peomid + "  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql);

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}