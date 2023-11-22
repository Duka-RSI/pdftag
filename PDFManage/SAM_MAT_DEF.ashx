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
            case "SearchMAT_NO":
                SearchMAT_NO(context);
                break;


        }
    }
    public void SearchMAT_NO(HttpContext context)
    {
        string MAT_NO = context.Request["MAT_NO"];
        string searchonlyerp = context.Request["searchonlyerp"];
        string sSql = "";

        MAT_NO = MAT_NO.Trim();

        sSql = @"select distinct MAT_NO,MAT_NAME, ISNULL(ERP_EXIST,'') ERP_EXIST
from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] a 
where 1=1";

        if (!string.IsNullOrEmpty(MAT_NO))
        {
            sSql += @" and MAT_NO like '%" + MAT_NO + "%'";
        }
        if (searchonlyerp == "Y")
        {
            sSql += @" and ERP_EXIST = 'Y' ";
        }

        sSql += @" order by ERP_EXIST desc ,MAT_NO asc ";



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

