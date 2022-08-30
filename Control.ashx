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
            case "getWarehouseManage":
                getWarehouseManage(context);
                break;
        }
    }

    public void getWarehouseManage(HttpContext context)
    {

        string ehs1 = context.Request["ehs1"];

        string sSql = "";

        sSql = "select * from WarehouseManage \n";
        sSql += " where 1=1   \n";
        sSql += " order by wmsn   \n";


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