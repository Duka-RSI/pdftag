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
            case "SearchCOLOR_DESC":
                SearchCOLOR_DESC(context);
                break;


        }
    }
    public void SearchCOLOR_DESC(HttpContext context)
    {
        string COLOR_DESC = context.Request["COLOR_DESC"];
        string sSql = "";

        COLOR_DESC = COLOR_DESC.Trim();

        sSql = @"select distinct COLOR_GROUP,COLOR_DESC,CUST_COLOR_WAY,ERP_COLORID
from [RD_SAMPLE].[dbo].[SAM_MAT_COLOR_DEF] a 
where 1=1 AND ENABLE_FLAG = 'Y' and a.COLOR_GROUP in ('MATERIAL','THREAD','STYLE','ZIPPER')";

        if (!string.IsNullOrEmpty(COLOR_DESC))
        {
            sSql += @" and COLOR_DESC like '%" + COLOR_DESC + "%'";
        }

        sSql += @" order by COLOR_DESC";



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

