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

        }
    }

    public void get(HttpContext context)
    {
        string qmid = context.Request["qmid"];
        string sSql = "";

        sSql = "select a.*,b.cddpeople,b.cddphone,b.cddpmobile,b.cddaddress  \n";
        sSql += "from QuotationManage a \n";
        sSql += "left join C_CustDataDetail b on a.cddNO=b.cddNO \n";
        sSql += " where 1=1  \n";
        sSql += " and a.qmid=" + qmid + "  \n";


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Query(sSql).FirstOrDefault();

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