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
            case "checkNote":
                checkNote(context);
                break;
        }
    }

    public void get(HttpContext context)
    {
        string prcsnid = context.Request["prcsnid"];
        string sSql = "";

        sSql = "select * from ProductRuleCodeSN \n";
        sSql += " where 1=1  \n";
        sSql += " and prcsnid=" + prcsnid + "  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void checkNote(HttpContext context)
    {
        string prcid = context.Request["prcid"];
        string note = context.Request["note"];
        string sSql = "";

        sSql = "select * from ProductRuleCodeSN \n";
        sSql += " where 1=1  \n";
        sSql += " and prcid=@prcid and note like '%"+note+"%' \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var list = cn.Query(sSql,new {prcid=prcid }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
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