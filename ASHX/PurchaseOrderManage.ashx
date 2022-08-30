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
        string porid = context.Request["porid"];
        string sSql = "";

        sSql = "select a.*,b.cspeople,b.csphone,b.cspmobile  \n";
        sSql += "from PurchaseOrderManage a \n";
        sSql += "left join D_CustSupport b on a.csNO=b.csNO \n";
        sSql += " where 1=1  \n";
        sSql += " and a.porid=" + porid + "  \n";


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