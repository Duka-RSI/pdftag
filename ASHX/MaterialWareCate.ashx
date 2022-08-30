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
             case "getAll":
                getAll(context);
                break;
        }
    }

    public void get(HttpContext context)
    {
        string mwcid = context.Request["mwcid"];
        string sSql = "";

        sSql = "select * from MaterialWareCate \n";
        sSql += " where 1=1  \n";
        sSql += " and mwcid=" + mwcid + "  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    } 
    
    public void getAll(HttpContext context)
    {
        string sSql = "";

        sSql = "select * from MaterialWareCate \n";
        sSql += " where isshow=0  \n";


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