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
            case "getPARTS_TYPE":
                getPARTS_TYPE(context);
                break;
            case "getPARTS_CODE":
                getPARTS_CODE(context);
                break;

            case "getPARTS_DESC":
                getPARTS_DESC(context);
                break;
            case "getMAT_ID":
                getMAT_ID(context);
                break;

        }
    }
    public void getPARTS_TYPE(HttpContext context)
    {
        string MAT_NO = context.Request["MAT_NO"];
        string sSql = "";

        sSql = @"select distinct b.MO_ID,b.PARTS_TYPE
from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] a
 join [RD_SAMPLE].[dbo].[SAM_MO_D] b on a.id=b.MO_ID
where MAT_NO=@MAT_NO ";


        MAT_NO = MAT_NO.Replace(" ", "").Trim();

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var list = cn.Query(sSql, new { MAT_NO = MAT_NO }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

    public void getPARTS_CODE(HttpContext context)
    {
        string MAT_NO = context.Request["MAT_NO"];
        string PARTS_TYPE = context.Request["PARTS_TYPE"];
        string sSql = "";

        MAT_NO = MAT_NO.Replace(" ", "").Trim();

        sSql = @"select distinct b.MO_ID,b.PARTS_CODE
from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] a
 join [RD_SAMPLE].[dbo].[SAM_MO_D] b on a.id=b.MO_ID
where MAT_NO=@MAT_NO and b.PARTS_TYPE=@PARTS_TYPE";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var list = cn.Query(sSql, new { MAT_NO = MAT_NO, PARTS_TYPE = PARTS_TYPE }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

    public void getPARTS_DESC(HttpContext context)
    {
        string MAT_NO = context.Request["MAT_NO"];
        string PARTS_TYPE = context.Request["PARTS_TYPE"];
        string PARTS_CODE = context.Request["PARTS_CODE"];
        string sSql = "";

        MAT_NO = MAT_NO.Replace(" ", "").Trim();

        sSql = @"select distinct b.MO_ID,b.PARTS_DESC
from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] a
 join [RD_SAMPLE].[dbo].[SAM_MO_D] b on a.id=b.MO_ID
where MAT_NO=@MAT_NO and b.PARTS_TYPE=@PARTS_TYPE and b.PARTS_CODE=@PARTS_CODE";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var list = cn.Query(sSql, new { MAT_NO = MAT_NO, PARTS_TYPE = PARTS_TYPE, PARTS_CODE = PARTS_CODE }).ToList();

            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

    public void getMAT_ID(HttpContext context)
    {
        string MAT_NO = context.Request["MAT_NO"];
        string PARTS_TYPE = context.Request["PARTS_TYPE"];
        string PARTS_CODE = context.Request["PARTS_CODE"];
        string PARTS_DESC = context.Request["PARTS_DESC"];
        string sSql = "";

        MAT_NO = MAT_NO.Replace(" ", "").Trim();

        sSql = @"select distinct b.MO_ID,b.MAT_ID
from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] a
 join [RD_SAMPLE].[dbo].[SAM_MO_D] b on a.id=b.MO_ID
where MAT_NO=@MAT_NO and b.PARTS_TYPE=@PARTS_TYPE and b.PARTS_CODE=@PARTS_CODE";


        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var list = cn.Query(sSql, new { MAT_NO = MAT_NO, PARTS_TYPE = PARTS_TYPE, PARTS_CODE = PARTS_CODE, PARTS_DESC = PARTS_DESC }).ToList();

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

