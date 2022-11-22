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
			case "get_by_lubid":
				get_by_lubid(context);
				break;
			case "edit":
				edit(context);
				break;

		}
	}


	public void get_by_lubid(HttpContext context)
	{
		string lubid = context.Request["lubid"];
		string sSql = "";

		sSql = "select * \n";
		sSql += "from PDFTAG.dbo.UA_TagData  a             \n";
		sSql += "where 1=1 \n";
		sSql += " and a.lubid =@lubid \n";

		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Query(sSql, new { lubid = lubid }).FirstOrDefault();

			context.Response.Write(JsonConvert.SerializeObject(res));
		}
	}

	public void edit(HttpContext context)
	{
		string lubid = context.Request["lubid"];
		string EW1 = context.Request["EW1"];
		string EW2 = context.Request["EW2"];
		string EW3 = context.Request["EW3"];
		string EW4 = context.Request["EW4"];
		string EW5 = context.Request["EW5"];
		string EW6 = context.Request["EW6"];
		string EW7 = context.Request["EW7"];
		string EW8 = context.Request["EW8"];
		string sSql = "";

		sSql += "update PDFTAG.dbo.UA_TagData               \n";
		sSql += " set EW1=@EW1,EW2=@EW2,EW3=@EW3,EW4=@EW4,EW5=@EW5,EW6=@EW6  \n";
		sSql += " ,EW7=@EW7,EW8=@EW8  \n";

		sSql += "where lubid =@lubid \n";


		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Execute(sSql, new
			{
				lubid = lubid,
				EW1 = EW1,
				EW2 = EW2,
				EW3 = EW3,
				EW4 = EW4,
				EW5 = EW5,
				EW6 = EW6,
				EW7 = EW7,
				EW8 = EW8

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

