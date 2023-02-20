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


        }
    }
    public void get(HttpContext context)
    {
        string IdName = context.Request["IdName"];
        string id = context.Request["id"];
        string ColName = context.Request["ColName"];
        string sSql = "";

        sSql = "select * \n";
        sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
        sSql += "where 1=1 \n";
        sSql += " and a.IdName =@IdName  and a.id=@id and a.ColName =@ColName \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
        {
            var res = cn.Query(sSql, new { IdName = IdName, id = id, ColName = ColName }).FirstOrDefault();

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

