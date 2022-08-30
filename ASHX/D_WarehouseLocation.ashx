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
            case "chech_wlNO":
                chech_wlNO(context);
                break;
        }
    }

    public void get(HttpContext context)
    {
        string wlid = context.Request["wlid"];
        string sSql = "";

        sSql = "select *   \n";
        sSql += "from D_WarehouseLocation a             \n";
        sSql += " where a.wlid=@wlid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { wlid = wlid }).FirstOrDefault();



            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void chech_wlNO(HttpContext context)
    {
        string wlNO = context.Request["wlNO"];
        string sSql = "";

        sSql = "select *   \n";
        sSql += "from D_WarehouseLocation a             \n";
        sSql += " where isshow=0 and  a.wlNO=@wlNO  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { wlNO = wlNO }).FirstOrDefault();



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