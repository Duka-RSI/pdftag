using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Text;
using OfficeOpenXml;
using Aspose.Pdf;
using Aspose.Pdf.Text;

public partial class Passport_Passport_A000 : System.Web.UI.Page
{
	protected string script;


	protected void Page_Load(object sender, EventArgs e)
	{

		if (!IsPostBack)
		{
			setDl();
			DataBind();
		}
	}
	public string RemoveDateOnFileName(string fileName)
	{
		string sNewFile = fileName;
		try
		{
			sNewFile = sNewFile.Replace(sNewFile.Substring(0, sNewFile.IndexOf('_') + 1), "");
		}
		catch (Exception ex)
		{

		}
		return sNewFile;
	}
	private void setDl()
	{
		var sql = new SQLHelper();
		var dt = new DataTable();

		string sSql = "";


		sSql = "select  distinct b.ptitle \n";
		sSql += "from PDFTAG.dbo.HistoryData a              \n";
		sSql += " join PDFTAG.dbo.P_inProcess b on a.pipid=b.pipid and b.isshow=0            \n";
		sSql += " where 1=1 and a.isshow=0  \n";
		sSql += " and b.pver in ('" + string.Join("','", LoginUser.CUST_NO) + "') \n";

		if (LoginUser.role == LoginUser.ROLE_User)
			sSql += " and b.creator ='" + LoginUser.PK + "' \n";

		sSql += "order by b.ptitle \n";


		using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
		{
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dt);
			}

			//dlTitle.DataTextField = "ptitle";
			//dlTitle.DataValueField = "ptitle";
			//dlTitle.DataSource = dt;
			//dlTitle.DataBind();
			dlTitle.Items.Insert(0, new System.Web.UI.WebControls.ListItem("全部", ""));





		}


		if (LoginUser.CUST_NO.Contains("1"))
		{
			dlVersion.Items.Add(new System.Web.UI.WebControls.ListItem("Lulu", "1"));
			ddlpver.Items.Add(new System.Web.UI.WebControls.ListItem("Lulu", "1"));
			//dlgmcate.Items.Add(new System.Web.UI.WebControls.ListItem("Lulu", "1"));
		}
		if (LoginUser.CUST_NO.Contains("2"))
		{
			dlVersion.Items.Add(new System.Web.UI.WebControls.ListItem("UA", "2"));
			ddlpver.Items.Add(new System.Web.UI.WebControls.ListItem("UA", "2"));
			//dlgmcate.Items.Add(new System.Web.UI.WebControls.ListItem("UA", "2"));
		}
		if (LoginUser.CUST_NO.Contains("3"))
		{
			dlVersion.Items.Add(new System.Web.UI.WebControls.ListItem("GAP", "3"));
			ddlpver.Items.Add(new System.Web.UI.WebControls.ListItem("GAP", "3"));
			//dlgmcate.Items.Add(new System.Web.UI.WebControls.ListItem("GAP", "3"));
		}

		if (dlVersion.Items.Count > 0)
		{
			dlVersion_SelectedIndexChanged(null, null);
		}
	}



	private void DataBind()
	{
		bool _AllowPaging = true;
		SQLHelper sql = new SQLHelper();
		DataTable dt = new DataTable();
		string sSql = "";
		string txt = txtSearch.Text;
		string version = dlVersion.SelectedItem.Value;
		string ptitle = dlTitle.SelectedItem.Value;
		try
		{
			sSql += "select b.pipid,b.ptitle,b.pidate,b.piuploadfile,b.pver,b.creator,a.hdid,a.hisversion,a.editdate \n";
			sSql += "from PDFTAG.dbo.HistoryData a              \n";
			sSql += " join PDFTAG.dbo.P_inProcess b on a.hdid=b.hdid and b.isshow=0            \n";
			sSql += " where 1=1 and a.isshow=0  \n";
			sSql += " and a.pipid in (select pipid from PDFTAG.dbo.P_inProcess where isshow=0)   \n"; //濾掉已刪除的主檔

			if (!string.IsNullOrEmpty(ptitle))
				sSql += " and (b.ptitle = '" + ptitle + "'  ) \n";

			if (!string.IsNullOrEmpty(version))
				sSql += " and (b.pver = '" + version + "'  ) \n";

			if (!string.IsNullOrEmpty(txt))
				sSql += " and (b.ptitle like '%" + txt + "%'  ) \n";

			if (LoginUser.role != LoginUser.ROLE_ADMIN)
				sSql += " and (b.creator = '" + LoginUser.PK + "'  ) \n";

			sSql += "order by b.pipid desc \n";
			Response.Write("<!--" + sSql + "-->");

			using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
			{
				using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
				{
					da.Fill(dt);
				}
			}



			this.LinkPageFirst1.Visible = true;
			this.LinkPageUp1.Visible = true;
			this.LinkPageDown1.Visible = true;
			this.LinkPageLast1.Visible = true;

			int pageSize = 30;//每頁顯示5筆           
			int nowpage = 1;
			int.TryParse(this.LabelNowPage1.Text, out nowpage); //當前頁計數器
			if (nowpage != 0) nowpage--;
			int PageCount = ((dt.Rows.Count / pageSize) <= 1) ? 1 : (dt.Rows.Count / pageSize);
			if ((dt.Rows.Count % pageSize) != 0)
			{
				PageCount++;
			}
			if (PageCount > 0) this.PageData1.Visible = true;
			else this.PageData1.Visible = false;

			this.LabelPageCount1.Text = PageCount.ToString();  //總共頁數
			this.LabelAllCount1.Text = dt.Rows.Count.ToString();
			if (nowpage == 0)
			{
				this.LinkPageFirst1.Visible = false;
				this.LinkPageUp1.Visible = false;
			}
			if (nowpage == (PageCount - 1))
			{
				this.LinkPageDown1.Visible = false;
				this.LinkPageLast1.Visible = false;
			}

			if (dt.Rows.Count < pageSize)
			{
				_AllowPaging = false;
				this.PageData1.Visible = false;
			}

			//叫用DBUtilty來分頁
			PagedDataSource pds = new PagedDataSource();
			pds.DataSource = dt.DefaultView;
			pds.AllowPaging = _AllowPaging;//是否啟用分頁
			pds.PageSize = pageSize;//一頁顯示的條數
			pds.CurrentPageIndex = nowpage;//獲取當前頁的索引

			//panView.Visible = true;
			this.panQuery.Visible = false;
			Repeater1.DataSource = pds;
			Repeater1.DataBind();
		}
		catch (Exception err)
		{
			this.panQuery.Visible = true;
			this.labErrMsg.Text = err.Message;
		}
	}

	#region 分頁控制1
	protected void LinkPageFirst1_Click(object sender, EventArgs e)
	{
		this.LabelNowPage1.Text = "1";
		this.DataBind();
	}
	protected void LinkPageUp1_Click(object sender, EventArgs e)
	{
		int p = 0;
		int.TryParse(this.LabelNowPage1.Text, out p);
		p--;
		if (p <= 1) p = 1;
		this.LabelNowPage1.Text = p.ToString();
		this.DataBind();
	}
	protected void LinkPageDown1_Click(object sender, EventArgs e)
	{
		int p = 0;
		int.TryParse(this.LabelNowPage1.Text, out p);
		int PageCount = int.Parse(this.LabelPageCount1.Text);
		p++;
		if (p >= PageCount) p = PageCount;
		this.LabelNowPage1.Text = p.ToString();
		this.DataBind();
	}
	protected void LinkPageLast1_Click(object sender, EventArgs e)
	{
		int p = 0;
		int.TryParse(this.LabelPageCount1.Text, out p);
		this.LabelNowPage1.Text = p.ToString();
		this.DataBind();
	}
	#endregion

	private void CleanObject()
	{

	}

	protected void Repeater1_ItemCommand(object source, RepeaterCommandEventArgs e)
	{
		if ("del" == e.CommandName)
		{
			Delete(e.CommandArgument.ToString());
			DataBind();
		}

	}

	private void Delete(string hdid)
	{
		SQLHelper sql = new SQLHelper();
		string sSql = "";

		sSql += "update PDFTAG.dbo.HistoryData set isshow=1    \n";
		sSql += " where hdid=@hdid     \n";



		using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
		{
			cm.Parameters.AddWithValue("@hdid", hdid);

			cm.ExecuteNonQuery();
		}
	}



	protected void btnAdd_Click(object sender, EventArgs e)
	{
		SQLHelper sql = new SQLHelper();
		DataTable dt = new DataTable();
		string sSql = "";

		try
		{
			DateTime dtNow = DateTime.Now;
			string sFilePath = "";


			sSql = "select count(*)+1 as cnt \n";
			sSql += "from PDFTAG.dbo.HistoryData a              \n";
			sSql += " where pipid=@pipid \n";



			using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
			{
				cm.Parameters.AddWithValue("@pipid", hidpipid.Value);

				using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
				{
					da.Fill(dt);
				}

				string hisversion = dt.Rows[0]["cnt"].ToString();

				if (hisversion.Length == 1)
					hisversion = "00" + hisversion;
				else if (hisversion.Length == 2)
					hisversion = "0" + hisversion;

				hisversion = dtNow.ToString("yyyyMMdd") + "-" + hisversion;


				sSql = @"insert into PDFTAG.dbo.HistoryData 
(pipid,hisversion,editdate,creator,isShow) 
values 
(@pipid,@hisversion,@editdate,@creator,@isShow);SELECT SCOPE_IDENTITY();";

				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
				cm.Parameters.AddWithValue("@hisversion", hisversion);
				cm.Parameters.AddWithValue("@editdate", dtNow);
				cm.Parameters.AddWithValue("@creator", LoginUser.PK);
				cm.Parameters.AddWithValue("@isShow", "0");

				long hdid = Convert.ToInt64(cm.ExecuteScalar().ToString());


				sSql = @"insert into PDFTAG.dbo.P_inProcess 
(ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,hdid,gmid) 
select ptitle,pidate,piuploadfile,pver,creator,createordate,isShow,'" + hdid + @"' as hdid,gmid
 from PDFTAG.dbo.P_inProcess where pipid=@pipid";

				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
				cm.ExecuteNonQuery();

				sSql = "select pipid \n";
				sSql += "from PDFTAG.dbo.P_inProcess a              \n";
				sSql += " where hdid=@hdid \n";
				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@hdid", hdid);
				dt = new DataTable();
				using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
				{
					da.Fill(dt);
				}

				string new_pipid = dt.Rows[0]["pipid"].ToString();

				sSql = "select luhid \n";
				sSql += "from PDFTAG.dbo.Lu_Header  a              \n";
				sSql += " where pipid=@pipid \n";
				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
				dt = new DataTable();
				using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
				{
					da.Fill(dt);
				}

				string luhid = dt.Rows[0]["luhid"].ToString();


				sSql = @"insert into PDFTAG.dbo.Lu_Header 
(pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate,org_luhid) 
 select '" + new_pipid + @"'as pipid, season, style, styledesc, productsize, brand, dividion,class,pod,stylestatus,generateddate,stylesketch,creator,createordate,mdate, luhid as org_luhid
from PDFTAG.dbo.Lu_Header where pipid=@pipid; SELECT SCOPE_IDENTITY();";

				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
				long new_luhid = Convert.ToInt64(cm.ExecuteScalar().ToString());

				sSql = @"insert into PDFTAG.dbo.Lu_BOMGarmentcolor 
(luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10) 
 select '" + new_luhid + @"' as luhid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10
   from PDFTAG.dbo.Lu_BOMGarmentcolor where luhid=@luhid;

insert into PDFTAG.dbo.Lu_BOM 
(luhid,type,rowid,StandardPlacement,Placement,SupplierArticle,Supplier,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,org_lubid) 
 select '" + new_luhid + @"' as luhid,type,rowid,StandardPlacement,Placement,SupplierArticle,Supplier,B1,B2,B3,B4,B5,B6,B7,B8,B9,B10,lubid as org_lubid
  from PDFTAG.dbo.Lu_BOM where luhid=@luhid;

insert into PDFTAG.dbo.Lu_SizeTable 
(luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,org_lustid) 
 select '" + new_luhid + @"' as luhid,rowid,codeid,Name,Criticality,TolA,TolB,HTMInstruction,lusthid,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10, lustid as org_lustid
   from PDFTAG.dbo.Lu_SizeTable where luhid=@luhid;

";

				cm.CommandText = sSql;
				cm.Parameters.Clear();
				cm.Parameters.AddWithValue("@luhid", luhid);
				cm.ExecuteNonQuery();

			}


		}
		catch (Exception err)
		{
			Response.Write("btnAdd_Click:" + err.ToString());
		}


		DataBind();
	}

	protected void btnEdit_Click(object sender, EventArgs e)
	{
		SQLHelper sql = new SQLHelper();
		DataTable dt = new DataTable();
		string sSql = "";
		string sDir2 = "upload/PDF";
		try
		{
			DateTime dtNow = DateTime.Now;
			string sFilePath = "";



			sSql = "update PDFTAG.dbo.P_inProcess    \n";
			sSql += " set ptitle=@ptitle,pver=@pver \n";
			if (!string.IsNullOrEmpty(sFilePath))
			{
				sSql += " ,piuploadfile=@piuploadfile \n";
			}


			sSql += "where pipid=@pipid                          \n";


			using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
			{
				//cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
				//cm.Parameters.AddWithValue("@ptitle", ptitle.Text);
				//cm.Parameters.AddWithValue("@pidate", dtNow);
				//if (!string.IsNullOrEmpty(sFilePath))
				//    cm.Parameters.AddWithValue("@piuploadfile", sFilePath);
				//cm.Parameters.AddWithValue("@pver", ddlpver.SelectedItem.Value);
				//cm.ExecuteNonQuery();




			}


		}
		catch (Exception err)
		{
			Response.Write("btnEdit_Click:" + err.ToString());
		}


		DataBind();
	}

	protected void btnQuery_Click(object sender, EventArgs e)
	{
		DataBind();
	}

	protected void dlVersion_SelectedIndexChanged(object sender, EventArgs e)
	{
		var sql = new SQLHelper();
		var dt = new DataTable();

		string sSql = "";

		string version = dlVersion.SelectedItem.Value;

		sSql = "select  distinct b.ptitle \n";
		sSql += "from PDFTAG.dbo.HistoryData a              \n";
		sSql += " join PDFTAG.dbo.P_inProcess b on a.pipid=b.pipid and b.isshow=0            \n";
		sSql += " where 1=1 and a.isshow=0  \n";
		sSql += " and b.pver in ('" + version + "') \n";

		if (LoginUser.role == LoginUser.ROLE_User)
			sSql += " and b.creator ='" + LoginUser.PK + "' \n";

		sSql += "order by b.ptitle \n";


		using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
		{
			using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
			{
				da.Fill(dt);
			}

			dlTitle.DataTextField = "ptitle";
			dlTitle.DataValueField = "ptitle";
			dlTitle.DataSource = dt;
			dlTitle.DataBind();
			dlTitle.Items.Insert(0, new System.Web.UI.WebControls.ListItem("全部", ""));





		}

	}
}