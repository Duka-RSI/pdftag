using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

public partial class Passport_Passport_A000 : System.Web.UI.Page
{
    protected string script;
    protected string queryStr = "";

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            setDl();
            DataBind();
        }
    }

    private void setDl()
    {

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
            sSql += "select a.*,'' as strCUST_NO \n";
            sSql += "from PDFTAG.dbo.Account a              \n";
            sSql += " where 1=1  and a.isShow=0 \n";


            if (!string.IsNullOrEmpty(txt))
                sSql += " and (a.USER_NAME like '%" + txt + "%' or a.USER_FULLNAME like '%" + txt + "%'  ) \n";


            sSql += " ORDER BY USER_NAME asc  \n";

            Response.Write("<!--" + sSql + "-->");

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                foreach(DataRow dr in dt.Rows)
                {
                    string[] arrCUST_NO = dr["CUST_NO"].ToString().Split(',');
                    List<string> arrStrCUST_NO = new List<string>();

                    if (arrCUST_NO.Contains("1")) arrStrCUST_NO.Add("LuLu");
                    else if (arrCUST_NO.Contains("2")) arrStrCUST_NO.Add("UA");
                    else if(arrCUST_NO.Contains("3")) arrStrCUST_NO.Add("GAP");

                    dr["strCUST_NO"] = string.Join(",", arrStrCUST_NO);
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
        if ("resetPw" == e.CommandName)
        {
            ResetPw(e.CommandArgument.ToString());
            DataBind();
        }
    }

    private void Delete(string aid)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";

        sSql += "update PDFTAG.dbo.Account set isShow=1   \n";
        sSql += " where aid=@aid     \n";



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@aid", aid);

            cm.ExecuteNonQuery();
        }
    }

    private void ResetPw(string aid)
    {
        SQLHelper sql = new SQLHelper();
        string sSql = "";

        sSql += "update PDFTAG.dbo.Account set PASSWORD='1111'   \n";
        sSql += " where aid=@aid     \n";



        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@aid", aid);

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


            sSql = @"select * from PDFTAG.dbo.Account where USER_AD=@USER_AD and isshow=0 ";



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {

                cm.Parameters.AddWithValue("@USER_AD", USER_AD.Text);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    script = "alert('帳號已存在!');";
                    return;
                }


    

                sSql = @"insert into PDFTAG.dbo.Account 
(USER_NAME,USER_FULLNAME,USER_AD,DEPARTMENT,PASSWORD,CUST_NO,Role,isShow) 
values 
(@USER_NAME,@USER_FULLNAME,@USER_AD,@DEPARTMENT,@PASSWORD,@CUST_NO,@Role,@isShow) ";
                cm.CommandText = sSql;
                cm.Parameters.Clear();

                cm.Parameters.AddWithValue("@USER_NAME", USER_NAME.Text);
                cm.Parameters.AddWithValue("@USER_FULLNAME", USER_FULLNAME.Text);
                cm.Parameters.AddWithValue("@USER_AD", USER_AD.Text);
                cm.Parameters.AddWithValue("@DEPARTMENT", DEPARTMENT.Text);
                cm.Parameters.AddWithValue("@PASSWORD", "1111");

          
                List<string> arrCUST_NO = new List<string>();
                if (CUST_NO_1.Checked)
                    arrCUST_NO.Add("1");
                if (CUST_NO_2.Checked)
                    arrCUST_NO.Add("2");
                if (CUST_NO_3.Checked)
                    arrCUST_NO.Add("3");

                cm.Parameters.AddWithValue("@CUST_NO", string.Join(",", arrCUST_NO));
                cm.Parameters.AddWithValue("@Role", dlRole.SelectedItem.Value);
                cm.Parameters.AddWithValue("@isShow", 0);
              
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

        try
        {


            sSql = @"select * from PDFTAG.dbo.Account where aid != @aid and  USER_AD=@USER_AD and isshow=0 ";



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@aid", hidaid.Value);
                cm.Parameters.AddWithValue("@USER_AD", USER_AD.Text);

                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                if (dt.Rows.Count > 0)
                {
                    script = "alert('帳號已存在!');";
                    return;
                }

                sSql = @"update PDFTAG.dbo.Account 
 set USER_NAME=@USER_NAME,USER_FULLNAME=@USER_FULLNAME,USER_AD=@USER_AD,DEPARTMENT=@DEPARTMENT,CUST_NO=@CUST_NO,Role=@Role
 where aid=@aid  ";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@aid", hidaid.Value);
                cm.Parameters.AddWithValue("@USER_NAME", USER_NAME.Text);
                cm.Parameters.AddWithValue("@USER_FULLNAME", USER_FULLNAME.Text);
                cm.Parameters.AddWithValue("@USER_AD", USER_AD.Text);
                cm.Parameters.AddWithValue("@DEPARTMENT", DEPARTMENT.Text);
                cm.Parameters.AddWithValue("@PASSWORD", "1111");


                List<string> arrCUST_NO = new List<string>();
                if (CUST_NO_1.Checked)
                    arrCUST_NO.Add("1");
                if (CUST_NO_2.Checked)
                    arrCUST_NO.Add("2");
                if (CUST_NO_3.Checked)
                    arrCUST_NO.Add("3");

                cm.Parameters.AddWithValue("@CUST_NO", string.Join(",", arrCUST_NO));
                cm.Parameters.AddWithValue("@Role", dlRole.SelectedItem.Value);


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