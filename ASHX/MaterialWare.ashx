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
            case "set_mwcid":
                set_mwcid(context);
                break;
        }
    }

    public void set_mwcid(HttpContext context)
    {
        string mwid = context.Request["mwid"];
        string mwcid = context.Request["mwcid"];
        string sSql = "";

        sSql = "update MaterialWare \n";
        sSql += " set mwcid=@mwcid  \n";
        sSql += " where mwid=@mwid  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql,new {mwid=mwid,mwcid=mwcid });

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