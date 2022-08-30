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
            case "getall":
                getall(context);
                break;
           

        }
    }

    public void getall(HttpContext context)
    {
        string sSql = "";

        sSql = "select a.* \n";
        sSql += "from AccountManage a              \n";
        sSql += " where 1=1   \n";
        sSql += "and isdel=0  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var list = cn.Query(sSql).ToList();



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