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
            case "getByPsmid":
                getByPsmid(context);
                break;
      
        }
    }

    public void getByPsmid(HttpContext context)
    {
        string psmid = context.Request["psmid"];
        string sSql = "";

         sSql = "select a.* \n";
        sSql += "from ProductShipmentManageDetail a              \n";
        sSql += " where 1=1   \n";
        sSql += "and a.psmid=@psmid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { psmid = psmid }).FirstOrDefault();



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