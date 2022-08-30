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
            case "delUploadFile":
                delUploadFile(context);
                break;
           

        }
    }

    public void get(HttpContext context)
    {
        string mrid = context.Request["mrid"];
        string sSql = "";

        sSql = "select a.* \n";
        sSql += "from ManageReport a              \n";
        sSql += " where 1=1   \n";
        sSql += "and a.mrid=@mrid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { mrid = mrid }).FirstOrDefault();



            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }


    public void delUploadFile(HttpContext context)
    {
        string mrid = context.Request["mrid"];
        string sSql = "";

        sSql = "update ManageReport set fileuploadpath=''";
        sSql += " where 1=1   \n";
        sSql += "and mrid=@mrid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { mrid = mrid });

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