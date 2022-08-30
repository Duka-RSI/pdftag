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
      
        }
    }

    public void get(HttpContext context)
    {
        string prcid = context.Request["prcid"];
        string sSql = "";

        sSql = "select *   \n";
        sSql += "from ProductRuleCode a             \n";
        sSql += " where a.prcid=@prcid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { prcid = prcid }).FirstOrDefault();



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