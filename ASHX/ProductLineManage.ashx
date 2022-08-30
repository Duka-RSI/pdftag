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
        string plmid = context.Request["plmid"];
        string sSql = "";

         sSql = "select a.* \n";
        sSql += "from ProductLineManage a              \n";
        sSql += " where 1=1   \n";
        sSql += "and a.plmid=@plmid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { plmid = plmid }).FirstOrDefault();



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