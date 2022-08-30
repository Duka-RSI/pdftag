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
            case "saveClose":
                saveClose(context);
                break;

        }
    }

    public void get(HttpContext context)
    {
        string mriid = context.Request["mriid"];
        string sSql = "";

        sSql = "select a.* \n";
        sSql += "from ManageReportItem a              \n";
        sSql += " where 1=1   \n";
        sSql += "and a.mriid=@mriid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql, new { mriid = mriid }).FirstOrDefault();



            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }



    public void saveClose(HttpContext context)
    {
        string mriid = context.Request["mriid"];
        string isclose = context.Request["isClose"];
        string sSql = "";
        string isclosedate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

        sSql = "update ManageReportItem set isclose=@isclose";
        if (isclose == "0")
        {
            sSql += ", isclosedate=''  \n";
            isclosedate = "";
        }
        else
        {
            sSql += ", isclosedate='" + isclosedate + "'  \n";
        }
        sSql += " where 1=1   \n";
        sSql += "and mriid=@mriid  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new { mriid = mriid, isclose = isclose });

            var result = new
            {
                res = res,
                isclosedate = isclosedate

            };


            context.Response.Write(JsonConvert.SerializeObject(result));
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