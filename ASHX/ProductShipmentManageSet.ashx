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
            case "save":
                save(context);
                break;

        }
    }

    public void save(HttpContext context)
    {
        string psmsid = context.Request["psmsid"];
        string psmid = context.Request["psmid"];
        string plsmid = context.Request["plsmid"];
        string outquantity = context.Request["outquantity"];
        string outprice = context.Request["outprice"];

        string sSql = "";


        if (string.IsNullOrEmpty(psmsid))
        {
            sSql = @" insert into ProductShipmentManageSet 
(psmid,plsmid,outquantity,outprice,isShow) 
values 
(@psmid,@plsmid,@outquantity,@outprice,@isShow) ";
        }
        else
        {
            sSql = @"update ProductShipmentManageSet 
 set outquantity=@outquantity,outprice=@outprice
 where psmsid=@psmsid     ";

        }



        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {
            var res = cn.Execute(sSql, new
            {
                psmsid = psmsid,
                psmid = psmid,
                plsmid = plsmid,
                outquantity = outquantity,
                outprice = outprice,
                isShow = 0
            });



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