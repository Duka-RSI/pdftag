<%@ WebHandler Language="C#" Class="Passport" %>

using System;
using System.Web;
using System.IO;
using System.Data;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Data.SqlClient;
using Dapper;
using System.Web.SessionState;

public class Passport : IHttpHandler, IRequiresSessionState
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
        string pipid = context.Request["pipid"];
        string sSql = "";

        sSql = "select * \n";
        sSql += "from PDFTAG.dbo.GroupManage  a             \n";
        sSql += "where 1=1 and isShow=0\n";
        sSql += " and a.pipid =@pipid \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { pipid = pipid }).FirstOrDefault();

            context.Response.Write(JsonConvert.SerializeObject(res));
        }
    }
    public void getSearch(HttpContext context)
    {
        string gmcate = context.Request["gmcate"];
        string txt = context.Request["txt"];
        string sSql = "";

        sSql += "select a.* \n";
        sSql += "from PDFTAG.dbo.GroupManage a              \n";
        sSql += " where 1=1 and a.isshow=0  \n";

        sSql += " and (a.gmcate ='"+gmcate+"' ) \n";

        if (!string.IsNullOrEmpty(txt))
            sSql += " and (a.gmname like '%" + txt + "%'  ) \n";


        sSql += "order by a.gmname \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
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

