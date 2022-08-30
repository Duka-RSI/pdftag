<%@ WebHandler Language="C#" Class="Control" %>

using System;
using System.Web;
using Newtonsoft.Json;
using Dapper;
using System.Collections.Generic;
using System.Data;

public class Control : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {


        switch (context.Request["fun"])
        {
            case "GetChart":
                GetChart(context);
                break;
            case "GetPara":
                GetPara(context);
                break;
        }
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void GetPara(HttpContext context)
    {
        string sSql = "";
        string sUfmid = context.Request["Ufmid"];

        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {

            sSql += " select *   ";
            sSql += " from UFparaData1 Where  ufmid=@ufmid and para!='Date' and para !='Time' ";


            var list = cn.Query(sSql, new { ufmid = sUfmid });


            context.Response.Write(JsonConvert.SerializeObject(list));

        }
    }
    

    public void GetChart(HttpContext context)
    {
        string sSql="";
        string sCol = "";
        string sUfmid = context.Request["Ufmid"];
        string sSDate = context.Request["SDate"];
        string sEDate = context.Request["EDate"];
        string sUfpdid=context.Request["ufpdid"];


        using (var cn = SqlMapperUtil.GetOpenConnection("DBUF"))
        {

            sSql = "select ROWID,para from ( ";
            sSql += " select ROW_NUMBER() OVER(ORDER BY ufpdid) AS ROWID,*   ";
            sSql += " from UFparaData1 Where  ufmid=" + sUfmid + "  ";
            sSql += ") T ";
            if (!string.IsNullOrEmpty(sUfpdid))
                sSql += "where  ufpdid in (" + sUfpdid.TrimEnd(',') + ")  ";


            var arrROWID = cn.Query(sSql);

            List<string> arrPara = new List<string>();
            foreach (var item in arrROWID)
            {

                sCol += "col" + item.ROWID + ",";
                arrPara.Add(item.para);
            }

            sCol = sCol.TrimEnd(',');

            sSql = "select  DateTime," + sCol;
            sSql += " from  UFData1   ";
            sSql += "Where  ufmid=@ufmid ";
            sSql += "and  (DateTime>=@Sdate and DateTime <=@Edate) ";
            sSql += "order by  ufdid asc ";

            SQLHelper sql = new SQLHelper();
            DataTable dt = new DataTable();

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, cn))
            {
                cm.Parameters.Add("@ufmid", sUfmid);
                cm.Parameters.Add("@Sdate", sSDate);
                cm.Parameters.Add("@Edate", sEDate);
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }
            }

            List<string> arrCategories = new List<string>();
            List<decimal> arrData = new List<decimal>();
            List<HSeries> series = new List<HSeries>();


            for (int i = 0; i < dt.Columns.Count; i++)
            {
                arrData = new List<decimal>();

                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (i == 0)
                        arrCategories.Add(dt.Rows[j]["DateTime"].ToString());
                    else
                    {
                        arrData.Add(Convert.ToDecimal(dt.Rows[j][i].ToString()));
                    }

                }

                if (i > 0)
                    series.Add(new HSeries
                    {
                        name = arrPara[i - 1],
                        data = arrData.ToArray(),
                        type= "spline",
                    });
            }

           

            HData r = new HData();
            r.categories = arrCategories.ToArray();
            r.series = series;

            context.Response.Write(JsonConvert.SerializeObject(r));
        }
        
        
    }

    
}


public class HCategories
{
    public object categories { get; set; }
}

public class HSeries
{
    public string name { get; set; }
    public object data { get; set; }
    public string type { get; set; }
}

public class HData
{
    public object categories { get; set; }
    public object series { get; set; }
}