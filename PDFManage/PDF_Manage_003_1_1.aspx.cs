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
    protected string hdid;

    protected void Page_Load(object sender, EventArgs e)
    {
        hdid = Request["hdid"];

        if (!IsPostBack)
        {
            setDl();
            DataBind();
        }
    }

    private void setDl()
    {
        var sql = new SQLHelper();
        var dt = new DataTable();

        string sSql = "";


        sSql += "select c.pipid,b.pipid as copy_pipid,b.ptitle,b.pidate,b.piuploadfile,b.pver,b.creator,a.hdid,a.hisversion,a.editdate \n";
        sSql += "from PDFTAG.dbo.HistoryData a              \n";
        sSql += " join PDFTAG.dbo.P_inProcess b on a.hdid=b.hdid            \n";//找出複製的主檔
        sSql += " join PDFTAG.dbo.P_inProcess c on a.pipid=c.pipid            \n";//找出群組
        sSql += " where 1=1 and a.isshow=0  \n";
        sSql += " and a.hdid = '" + hdid + "'   \n";


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            lblInfo.Text = dt.Rows[0]["pipid"].ToString() + "-" + dt.Rows[0]["ptitle"].ToString() + "-" + dt.Rows[0]["hisversion"].ToString() + "-" +
                (dt.Rows[0]["pver"].ToString() == "1" ? "Lulu" : dt.Rows[0]["pver"].ToString() == "2" ? "UA" : dt.Rows[0]["pver"].ToString() == "3" ? "GAP" : "");

            hidpipid.Value = dt.Rows[0]["copy_pipid"].ToString();//複製的主檔pipid
        }

    }



    private void DataBind()
    {
        bool _AllowPaging = true;
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";
        string txt = txtSearch.Text;


        try
        {
            sSql += "select a.* \n";
            sSql += "from PDFTAG.dbo.Lu_Header_stylelevel1 a              \n";
            sSql += " where 1=1 and a.isshow=0  \n";


            if (!string.IsNullOrEmpty(txt))
                sSql += " and (a.hsl1name like '%" + txt + "%'  ) \n";


            sSql += "order by a.hsl1name \n";
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

    private void Delete(string hsl1id)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";

        sSql += "update PDFTAG.dbo.Lu_Header_stylelevel1 set isshow=1    \n";
        sSql += " where hsl1id=@hsl1id     \n";



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@hsl1id", hsl1id);

            cm.ExecuteNonQuery();
        }
    }

    protected void btnAdd_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        string sDir2 = "upload/PDF";

        try
        {
            DateTime dtNow = DateTime.Now;
           

            sSql = @"insert into PDFTAG.dbo.Lu_Header_stylelevel1 
(hsl1name,creator,creatordate,isShow) 
values 
(@hsl1name,@creator,@creatordate,@isShow);SELECT SCOPE_IDENTITY();";

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {


                cm.Parameters.AddWithValue("@hsl1name", hsl1name.Text);
                cm.Parameters.AddWithValue("@creator", LoginUser.PK);
                cm.Parameters.AddWithValue("@creatordate", dtNow.ToString("yyyy/MM/dd HH:mm:ss"));
                cm.Parameters.AddWithValue("@isShow", "0");



                long result = Convert.ToInt64(cm.ExecuteScalar().ToString());


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
           
            sSql = "update PDFTAG.dbo.Lu_Header_stylelevel1    \n";
            sSql += " set hsl1name=@hsl1name \n";
           
            sSql += "where hsl1id=@hsl1id                          \n";


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@hsl1id", hidhsl1id.Value);
                cm.Parameters.AddWithValue("@hsl1name", hsl1name.Text);
                cm.ExecuteNonQuery();




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


}