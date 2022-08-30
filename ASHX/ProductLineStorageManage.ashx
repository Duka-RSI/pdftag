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
            case "getByBarcode":
                getByBarcode(context);
                break;
            case "getByWlNo":
                getByWlNo(context);
                break;

        }
    }

    public void getByBarcode(HttpContext context)
    {
        string psmid = context.Request["psmid"];
        string wlNO = context.Request["wlNO"];
        string sSql = "";

        sSql = "select a.*,b.psmsid,b.outquantity,b.outprice \n";
        sSql += "from ProductLineStorageManage a              \n";
        sSql += " left join  ProductShipmentManageSet b on b.psmid=@psmid and b.isShow=0 and  a.plsmid=b.plsmid              \n";
        sSql += " where 1=1   \n";
        sSql += "and plstatus=1 and a.barcode like '" + wlNO + "%'  \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var list = cn.Query(sSql, new { psmid = psmid }).ToList();



            context.Response.Write(JsonConvert.SerializeObject(list));
        }
    }

     public void getByWlNo(HttpContext context)
    {
        string wlNO = context.Request["wlNO"];
        string sSql = "";

         sSql = "select a.plsmid,a.barcode,a.lotnum,ISNULL(a.plquantity,0) as plquantity \n";
        sSql += "from ProductLineStorageManage a              \n";
        sSql += " where 1=1   \n";
        sSql += " and  a.plstatus=1 and a.barcode like '" + wlNO + "%'  \n";
        sSql += " \n";

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
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