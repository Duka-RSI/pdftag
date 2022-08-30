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
 case "del":
                del(context);
                break;
        }
    }


    public void get(HttpContext context)
    {
        string plmid = context.Request["plmid"];

        string sSql = "";

        sSql = @"select a.*,b.mwcname 
                   from ProductLineManagSource a
                    join MaterialWareCate b on a.mwcid=b.mwcid ";
        sSql += " where 1=1  \n";
        sSql += " and a.plmid=@plmid  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var list = cn.Query(sSql,new {plmid=plmid }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

   public void del(HttpContext context)
    {
        string plmsid = context.Request["plmsid"];

        string sSql = "";

        sSql = @"delete ProductLineManagSource ";
        sSql += " where 1=1  \n";
        sSql += " and plmsid=@plmsid  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql,new {plmsid=plmsid });

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