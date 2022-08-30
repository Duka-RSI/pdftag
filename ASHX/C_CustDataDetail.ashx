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
            case "getSearch":
                getSearch(context);
                break;
        }
    }

    public void get(HttpContext context)
    {
        string cddid = context.Request["cddid"];
        string sSql = "";


        sSql = "select * from C_CustDataDetail \n";
        sSql += " where 1=1  \n";
        sSql += " and cddid=" + cddid + "  \n";



        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void getSearch(HttpContext context)
    {
        string txt = context.Request["txt"];

        string sSql = "";


        sSql = "select * from C_CustDataDetail \n";
        sSql += " where 1=1  \n";
        sSql += " and (cddNO like '%" + txt + "%' or cddname like '%" + txt + "%' )  \n";



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