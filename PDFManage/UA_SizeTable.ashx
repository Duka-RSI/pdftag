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
using System.Text;

public class Passport : IHttpHandler, IRequiresSessionState
{



	public void ProcessRequest(HttpContext context)
	{

		switch (context.Request["fun"])
		{
			case "get":
				get(context);
				break;
			case "get_org":
				get_org(context);
				break;

			case "saveByCol":
				saveByCol(context);
				break;
			case "delete":
				delete(context);
				break;
			case "copy":
				copy(context);
				break;

			case "getContent":
				getContent(context);
				break;
		}
	}

	public void delete(HttpContext context)
	{
		string lustid = context.Request["lustid"];

		string sSql = "";


		sSql = "delete  PDFTAG.dbo.UA_SizeTable  where lustid=@lustid  ";


		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Execute(sSql, new { lustid = lustid });

			context.Response.Write(JsonConvert.SerializeObject(res));
		}
	}
	public void copy(HttpContext context)
	{
		string lustid = context.Request["lustid"];

		string sSql = "";


		sSql = @"insert into PDFTAG.dbo.UA_SizeTable 
(luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,org_lustid) 
 select luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10, org_lustid
   from PDFTAG.dbo.UA_SizeTable where lustid=@lustid;";


		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Execute(sSql, new { lustid = lustid });

			context.Response.Write(JsonConvert.SerializeObject(res));
		}
	}
	public void get(HttpContext context)
	{
		string pipid = context.Request["pipid"];
		string sSql = "";

		sSql = "select * \n";
		sSql += "from PDFTAG.dbo.P_inProcess  a             \n";
		sSql += "where 1=1 and isShow=0\n";
		sSql += " and a.pipid =@pipid \n";

		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Query(sSql, new { pipid = pipid }).FirstOrDefault();

			context.Response.Write(JsonConvert.SerializeObject(res));
		}
	}
	public void get_org(HttpContext context)
	{
		string lustid = context.Request["id"];
		string new_lustid = context.Request["newid"];
		string col = context.Request["col"];
		string sSql = "";

		sSql = "select " + col + " as text \n";
		sSql += "from PDFTAG.dbo.UA_SizeTable  a             \n";
		sSql += "where 1=1 \n";
		sSql += " and a.lustid =@lustid \n";

		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var res = cn.Query(sSql, new { lustid = lustid }).FirstOrDefault();

			var resNew = cn.Query(sSql, new { lustid = new_lustid }).FirstOrDefault();

			string orgText = (res == null ? "" : res.text);
			string newText = (resNew == null ? "" : resNew.text);

			double d = 0;
			if (orgText.Contains("/") && Double.TryParse(orgText.Split('/')[1], out d)) orgText = orgText.Replace(" ", "-");
			if (newText.Contains("/") && Double.TryParse(newText.Split('/')[1], out d)) newText = newText.Replace(" ", "-");

			context.Response.Write(JsonConvert.SerializeObject(new { orgText = orgText, newText = newText }));
		}
	}

	public class Text
	{
		public string TextOrg { get; set; }

		public long lusthid { get; set; }

	}

	public void saveByCol(HttpContext context)
	{
		string lustid_org = context.Request["orgId"];
		string lustid = context.Request["id"];
		string col = context.Request["col"];
		string text = context.Request["text"];
		string chNote = context.Request["chNote"];
		string isRecord = context.Request["isRecord"];
		string sSql = "";
		string co1_1 = "";
		string sizeColumnName = "";
		DateTime dtNow = DateTime.Now;



		sSql = "select " + col + " as TextOrg,lusthid  from  PDFTAG.dbo.UA_SizeTable where lustid=@lustid";


		using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
		{
			var resLu_SizeTable_Org = cn.Query<Text>(sSql, new { lustid = lustid_org }).FirstOrDefault();


			sSql = "update PDFTAG.dbo.UA_SizeTable \n";
			sSql += "set " + col + "=@text,isEdit=1              \n";
			sSql += "where lustid=@lustid\n";

			var res = cn.Execute(sSql, new { lustid = lustid, text = text });

			if (col.StartsWith("A"))
			{
				sSql = "select H" + (col.Replace("A", "")) + " as TextOrg  from  PDFTAG.dbo.UA_SizeTable_Header where lusthid=@lusthid";

				var resLu_SizeTable_Header = cn.Query<Text>(sSql, new { lusthid = resLu_SizeTable_Org.lusthid }).FirstOrDefault();

				sizeColumnName = resLu_SizeTable_Header.TextOrg;


				//sSql = "select lusthid  from  PDFTAG.dbo.UA_SizeTable_1 where lustid_relation=@lustid";
				//var resLu_SizeTable_1_Org = cn.Query<Text>(sSql, new { lustid = lustid_org }).FirstOrDefault();

				//sSql = "select *  from  PDFTAG.dbo.UA_SizeTable_Header_1  where lusthid=@lusthid";

				//var Lu_SizeTable_Header_1 = cn.Query(sSql, new { lusthid = resLu_SizeTable_1_Org.lusthid }).FirstOrDefault();

				//co1_1 = "A";
				//if (Lu_SizeTable_Header_1.H1 == sizeColumnName) co1_1 = "A1";
				//else if (Lu_SizeTable_Header_1.H2 == sizeColumnName) co1_1 = "A2";
				//else if (Lu_SizeTable_Header_1.H3 == sizeColumnName) co1_1 = "A3";
				//else if (Lu_SizeTable_Header_1.H4 == sizeColumnName) co1_1 = "A4";
				//else if (Lu_SizeTable_Header_1.H5 == sizeColumnName) co1_1 = "A5";
				//else if (Lu_SizeTable_Header_1.H6 == sizeColumnName) co1_1 = "A6";
				//else if (Lu_SizeTable_Header_1.H7 == sizeColumnName) co1_1 = "A7";
				//else if (Lu_SizeTable_Header_1.H8 == sizeColumnName) co1_1 = "A8";
				//else if (Lu_SizeTable_Header_1.H9 == sizeColumnName) co1_1 = "A9";
				//else if (Lu_SizeTable_Header_1.H10 == sizeColumnName) co1_1 = "A10";
				//else if (Lu_SizeTable_Header_1.H11 == sizeColumnName) co1_1 = "A11";
				//else if (Lu_SizeTable_Header_1.H12 == sizeColumnName) co1_1 = "A12";
				//else if (Lu_SizeTable_Header_1.H13 == sizeColumnName) co1_1 = "A13";
				//else if (Lu_SizeTable_Header_1.H14 == sizeColumnName) co1_1 = "A14";
				//else if (Lu_SizeTable_Header_1.H15 == sizeColumnName) co1_1 = "A15";



			}
			else
			{
				co1_1 = col;
			}


			//sSql = "update PDFTAG.dbo.UA_SizeTable_1 \n";
			//sSql += "set " + co1_1 + "=@text,isEdit=1              \n";
			//sSql += "where lustid_relation =@lustid\n";
			//var resLu_SizeTable_1 = cn.Execute(sSql, new { lustid = lustid_org, text = text });


			if (!string.IsNullOrEmpty(chNote))
			{
				sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.UA_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.UA_Ch_Note
                                set note=@note
                             where IdName=@IdName and id=@id and ColName=@ColName
                             end ";

				var res2 = cn.Execute(sSql, new { IdName = "lustid", id = lustid, ColName = col, note = chNote, creator = LoginUser.PK, createordate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") });
			}

			int iCntLearnmgrItem = 0;

			if (isRecord == "1")
			{
				string sFirstCharTermname_org = resLu_SizeTable_Org.TextOrg.Substring(0, 1);
				string sTermname_org = resLu_SizeTable_Org.TextOrg.Trim().Replace(" ", "").ToLower();


				sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_LearnmgrItem where ColSource=@ColSource and ColName=@ColName and FirstCharTermname_org=@FirstCharTermname_org and termname_org=@termname_org )
                             begin
                                  insert into PDFTAG.dbo.UA_LearnmgrItem
                                   (ColSource,ColName,FirstCharTermname_org,termname_org,termname,creator,creatordate)
                                    values 
                              (@ColSource,@ColName,@FirstCharTermname_org,@termname_org,@termname,@creator,@creatordate)
                              end 
                            -- else 
                            --    begin
                            --    update PDFTAG.dbo.UA_LearnmgrItem
                            --    set termname=@termname,updateDate=@updateDate,UpdateUser=@UpdateUser 
                            -- where ColSource=@ColSource and ColName=@ColName and FirstCharTermname_org=@FirstCharTermname_org and termname_org=@termname_org
                            -- end ";

				iCntLearnmgrItem = cn.Execute(sSql, new
				{
					ColSource = "Size",
					ColName = col,
					FirstCharTermname_org = sFirstCharTermname_org,
					termname_org = sTermname_org,
					termname = text,
					creator = LoginUser.PK,
					creatordate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
					UpdateUser = LoginUser.PK,
					updateDate = dtNow.ToString("yyyy/MM/dd HH:mm:ss"),
				});
			}

			context.Response.Write(JsonConvert.SerializeObject(new { data = res, learnmgrItem = iCntLearnmgrItem, sizeColumnName = sizeColumnName, co1_1 = co1_1 }));
		}
	}

	public void getContent(HttpContext context)
	{
		string pipid = context.Request["pipid"];

		SQLHelper sql = new SQLHelper();
		DataTable dt = new DataTable();
		string sSql = "";

		StringBuilder sb = new StringBuilder();



		sSql = "select * \n";
		sSql += "from PDFTAG.dbo.UA_Header a              \n";
		sSql += " where 1=1 and a.isshow=0   \n";
		sSql += " and a.pipid = '" + pipid + "'   \n";


		using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
		{
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dt);
			}

			string luhid = dt.Rows[0]["luhid"].ToString();
			string org_luhid = dt.Rows[0]["org_luhid"].ToString();

			sSql = "select * \n";
			sSql += "from PDFTAG.dbo.UA_Header a              \n";
			sSql += " where 1=1   \n";
			sSql += " and a.luhid in (select org_luhid as luhid from  PDFTAG.dbo.UA_Header where pipid = '" + pipid + "' )   \n";
			//Response.Write("<!--" + sSql + "-->");
			cm.CommandText = sSql;
			DataTable dtLu_Header_Org = new DataTable();
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dtLu_Header_Org);
			}


			List<Lu_Ch_Note> arrNotes = new List<Lu_Ch_Note>();

			sSql = "select * \n";
			sSql += "from PDFTAG.dbo.UA_Ch_Note  a             \n";
			sSql += "where 1=1 \n";
			sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.UA_Header where isshow=0 and pipid = @pipid ) \n";
			//Response.Write("<!--" + sSql + "-->");
			cm.CommandText = sSql;
			cm.Parameters.Clear();
			cm.Parameters.AddWithValue("@pipid", pipid);
			DataTable dtNote = new DataTable();
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dtNote);
			}

			arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

			int iSeq = 1;


			#region Lu_SizeTable

			sSql = "select * \n";
			sSql += "from PDFTAG.dbo.UA_Ch_Note  a             \n";
			sSql += "where 1=1 \n";
			sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.UA_SizeTable where pipid = @pipid ) \n";
			//Response.Write("<!--" + sSql + "-->");
			cm.CommandText = sSql;
			cm.Parameters.Clear();
			cm.Parameters.AddWithValue("@pipid", pipid);
			dtNote = new DataTable();
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dtNote);
			}

			arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

			sSql = "select * \n";
			sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
			sSql += " where 1=1   \n";
			sSql += " and a.pipid = '" + pipid + "'   \n";
			sSql += " order by lusthid,rowid asc    \n";
			//Response.Write("<!--" + sSql + "-->");
			cm.CommandText = sSql;
			dt = new DataTable();
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dt);
			}

			sSql = "select * \n";
			sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
			sSql += " where 1=1   \n";
			sSql += " and a.lustid in (select org_lustid as lustid from  PDFTAG.dbo.UA_SizeTable where pipid = '" + pipid + "' )   \n";
			//Response.Write("<!--" + sSql + "-->");
			cm.CommandText = sSql;
			DataTable dtSizeTable_Org = new DataTable();
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dtSizeTable_Org);
			}

			var arr_lusthids = dt.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().ToList();

			sb.Append("<table class='table table-hover'>");
			sb.Append("<tr>");
			sb.Append("<td><input type='checkbox'  onclick='checkAll(this," + pipid + ")'>全選</td>");
			sb.Append("<td>");
			sb.Append("</td>");
			sb.Append("</tr>");

			foreach (var lusthid in arr_lusthids)
			{
				sSql = "select * \n";
				sSql += "from PDFTAG.dbo.UA_SizeTable_Header a              \n";
				sSql += " where a.lusthid in ('" + lusthid + "')   \n";
				//Response.Write("<!--" + sSql + "-->");
				cm.CommandText = sSql;
				DataTable dtLu_SizeTable_Header = new DataTable();
				using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
				{
					da.Fill(dtLu_SizeTable_Header);
				}

				sb.Append("<tr>");
				sb.Append("<td><input type='checkbox' class='ck_" + pipid + "' data-lusthid='" + lusthid + "'></td>");
				sb.Append("<td>");

				sb.Append("<h4>Size Table</h4>");
				sb.Append("<table class='table table-hover'>");
				sb.Append("<tr>");
				sb.Append(" <th scope='col'>#</th>");
				sb.Append(" <th scope='col'>Description</th>");
				//sb.Append(" <th scope='col'>Criticality</th>");
				sb.Append(" <th scope='col'>Tol(-)</th>");
				sb.Append(" <th scope='col'>Tol(+)</th>");
				//sb.Append(" <th scope='col'>HTM Instruction</th>");


				int iOtherCnt = 0;
				if (dtLu_SizeTable_Header.Rows.Count > 0)
				{
					for (int i = 1; i <= 15; i++)
					{
						string sH = dtLu_SizeTable_Header.Rows[0]["H" + i].ToString();
						if (!string.IsNullOrEmpty(sH))
						{
							sb.Append(" <th scope='col'>" + sH + "</th>");
							iOtherCnt++;
						}
						else
						{
							break;
						}
					}
				}

				sb.Append(" <th scope='col'></th>");
				sb.Append("</tr>");

				DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
				int idx = 0;
				foreach (DataRow drSizeTable in drSizeTables)
				{
					string lustid = drSizeTable["lustid"].ToString();
					string org_lustid = drSizeTable["org_lustid"].ToString();
					bool isEdit = (drSizeTable["isEdit"].ToString().ToLower() == "true" ? true : false);


					string code = drSizeTable["Code"].ToString();
					string description = drSizeTable["Description"].ToString();
					//string criticality = drSizeTable["Criticality"].ToString();
					string tolA = drSizeTable["TolA"].ToString();
					string tolB = drSizeTable["TolB"].ToString();
					string hTMInstruction = drSizeTable["HTMInstruction"].ToString();

					DataRow[] drOrgSizeTables = dtSizeTable_Org.Select("lustid='" + org_lustid + "'");

					if (isEdit)
					{
						//Response.Write("<!--isEdit=" + isEdit + "-->");

						if (drOrgSizeTables.Length > 0)
						{
							DataRow drOrgSizeTable = drOrgSizeTables[0];

							code = Compare(drOrgSizeTable["code"].ToString(), code, FilterNote(arrNotes, lustid, "code"));
							description = Compare(drOrgSizeTable["description"].ToString(), description, FilterNote(arrNotes, lustid, "description"));
							//criticality = Compare(drOrgSizeTable["criticality"].ToString(), criticality, FilterNote(arrNotes, lustid, "criticality"));
							tolA = Compare(drOrgSizeTable["tolA"].ToString(), tolA, FilterNote(arrNotes, lustid, "tolA"));
							tolB = Compare(drOrgSizeTable["tolB"].ToString(), tolB, FilterNote(arrNotes, lustid, "tolB"));
							//hTMInstruction = Compare(drOrgSizeTable["hTMInstruction"].ToString(), hTMInstruction, FilterNote(arrNotes, lustid, "hTMInstruction"));
						}

					}

					iSeq++;
					sb.Append("<tr data-rowid='" + drSizeTable["rowid"].ToString() + "' id='row" + iSeq + "'>");
					sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='code' onclick='editSizeTable(this)'>" + code + "</td>");
					sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' onclick='editSizeTable(this)'>" + description + "</td>");
					//sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' onclick='editSizeTable(this)'>" + criticality + "</td>");
					sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' onclick='editSizeTable(this)'>" + tolA + "</td>");
					sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' onclick='editSizeTable(this)'>" + tolB + "</td>");
					//sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='HTMInstruction' onclick='editSizeTable(this)'>" + hTMInstruction + "</td>");

					for (int i = 1; i <= iOtherCnt; i++)
					{
						string other = drSizeTable["A" + i].ToString();
						if (isEdit)
						{
							if (drOrgSizeTables.Length > 0)
							{
								DataRow drOrgSizeTable = drOrgSizeTables[0];
								other = Compare(drOrgSizeTable["A" + i].ToString(), other, FilterNote(arrNotes, lustid, "A" + i));
							}
						}
						double d = 0;
						if (other.Contains("/") && Double.TryParse(other.Split('/')[1].Trim(), out d)) other = other.Replace(" ", "-");

						sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='A" + i + "' onclick='editSizeTable(this)'>"
							+ other + "</td>");
					}

					sb.Append(" <td scope='col'>");
					//sb.Append("   <input typ='button' class='btn btn-danger btn-sm' value='整筆刪除' onclick='deleteSizeTable(" + lustid + "," + iSeq + ")' />");

					//if (idx == drSizeTables.Length - 1)
					//sb.Append("   <input typ='button' class='btn btn-secondary btn-sm' value='整筆新增'  onclick='copySizeTable(" + lustid + ")'/>");
					sb.Append(" </td>");
					sb.Append("</tr>");



					idx++;
				}


				sb.Append("</table>");

				sb.Append(" </td>");
				sb.Append("</tr>");

			}



			#endregion

		}

		context.Response.Write(JsonConvert.SerializeObject(new { html = sb.ToString() }));
	}

	public class Lu_Ch_Note
	{
		public long Id { get; set; }
		public string ColName { get; set; }

		public string Note { get; set; }
	}
	public string FilterNote(List<Lu_Ch_Note> arrData, string id, string colName)
	{
		var res = arrData.FirstOrDefault(w => w.Id == long.Parse(id) && w.ColName.ToLower() == colName.ToLower());
		if (res == null) return "";

		return res.Note;
	}
	public string Compare(string org, string newText, string note)
	{
		if (org == newText)
			return org;
		else
			return "<font>原:" + org + "</font><br><font color='red'>修:" + newText + "</font><br><font color='blue'>中:" + note + "</font>";
	}

	public bool IsReusable
	{
		get
		{
			return false;
		}
	}



}

