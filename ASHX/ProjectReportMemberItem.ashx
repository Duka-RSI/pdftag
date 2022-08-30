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
            case "saveFollow":
                saveFollow(context);
                break;
            case "saveClose":
                saveClose(context);
                break;

        }
    }

    public void get(HttpContext context)
    {
        string prmiid = context.Request["prmiid"];
        string sSql = "";

        sSql = "select a.* \n";
        sSql += "from ProjectReportMemberItem a              \n";
        sSql += " where 1=1   \n";
        sSql += "and a.prmiid=@prmiid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { prmiid = prmiid }).FirstOrDefault();



            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }


    public void delUploadFile(HttpContext context)
    {
        string prmiid = context.Request["prmiid"];
        string sSql = "";

        sSql = "update ProjectReportMemberItem set filepath=''";
        sSql += " where 1=1   \n";
        sSql += "and prmiid=@prmiid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { prmiid = prmiid });

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }

    public void saveFollow(HttpContext context)
    {
        string prmiid = context.Request["prmiid"];
        string isfollow = context.Request["isfollow"];
        string sSql = "";
        string isfollowdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        sSql = "update ProjectReportMemberItem set isfollow=@isfollow";

        if (isfollow == "0")
        {
            sSql += ", isfollowdate=''  \n";
            isfollowdate = "";
        }
        else
        {
            sSql += ", isfollowdate='" + isfollowdate + "'  \n";
        }

        sSql += " where 1=1   \n";
        sSql += "and prmiid=@prmiid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { prmiid = prmiid, isfollow = isfollow });

            var result = new
            {
                res = res,
                isfollowdate = isfollowdate

            };


            context.Response.Write(JsonConvert.SerializeObject(result));
        }
    }

    public void saveClose(HttpContext context)
    {
        string prmiid = context.Request["prmiid"];
        string isclose = context.Request["isClose"];
        string sSql = "";
        string isfollowdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        sSql = "update ProjectReportMemberItem set isclose=@isclose";
        sSql += " where 1=1   \n";
        sSql += "and prmiid=@prmiid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { prmiid = prmiid, isclose = isclose });



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