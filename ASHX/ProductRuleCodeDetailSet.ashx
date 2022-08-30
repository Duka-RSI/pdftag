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
            case "getByPrcdid":
                getByPrcdid(context);
                break;
            case "delete":
                delete(context);
                break;
            case "add":
                add(context);
                break;

        }
    }

    public void getByPrcdid(HttpContext context)
    {
        string prcdid = context.Request["prcdid"];
        string sSql = "";

        sSql = "select *   \n";
        sSql += "from ProductRuleCodeDetailSet a             \n";
        sSql += " where a.prcdid=@prcdid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var list = cn.Query(sSql, new { prcdid = prcdid }).ToList();



            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

    public void add(HttpContext context)
    {
        string prcid = context.Request["prcid"];
        string prcdid = context.Request["prcdid"];
        string prcdscode = context.Request["prcdscode"];
        string prcdsname = context.Request["prcdsname"];
        string sSql = "";

        sSql = "insert ProductRuleCodeDetailSet (prcid,prcdid,prcdscode,prcdsname) values  (@prcid,@prcdid,@prcdscode,@prcdsname)          \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { prcid = prcid, prcdid = prcdid, prcdscode = prcdscode, prcdsname = prcdsname });



            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void delete(HttpContext context)
    {
        string prcdsid = context.Request["prcdsid"];
        string sSql = "";

        sSql = "delete ProductRuleCodeDetailSet              \n";
        sSql += " where prcdsid=@prcdsid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { prcdsid = prcdsid });



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