using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
public partial class master_MasterPage_Login : System.Web.UI.MasterPage
{
    protected string script;
    protected override void OnInit(EventArgs e)
    {
        if (!LoginUser.isLogin())
        {
            Response.Redirect("../Login.aspx");
            Response.End();
        }

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!LoginUser.isLogin())
        {
            Response.Redirect("../Login.aspx");
            Response.End();
        }

        if (Session["subMemu"] != null)
        {
            string id = Session["subMemu"].ToString();

            script = "showSubMenu('"+ id + "');";
        }
        if (Session["selectSubMenu"] != null)
        {
            string id = Session["selectSubMenu"].ToString();

            script += "selectSubMenu('" + id + "');";
        }

        if (!IsPostBack)
        {
            //SetMenu();
        }
    }

    protected void SetMenu()
    {
        //SQLHelper sql = new SQLHelper();
        //DataTable dt = new DataTable();
        //string sSql = "";

        //sSql = "select pcid,pcname from [Member].[dbo].[ProductCate] where isshow=0 order by pcno asc";

        //using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        //{
        //    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
        //    {
        //        da.Fill(dt);
        //    }
        //    StringBuilder sb = new StringBuilder();

        //    foreach(DataRow dr in dt.Rows)
        //    {
        //        sb.Append("<a class='list-group-item list-group-item-action' id='listProductCate_"+ dr["pcid"].ToString() + "' data-toggle='list' href='#' role='tab' aria-controls='home' onclick=\"window.location = '../ProductCate/ProductCate_001_2.aspx?pcid=" + dr["pcid"].ToString() + "'\">" + dr["pcname"].ToString()+"</a>");
        //    }
        //    listProductCate.InnerHtml = sb.ToString();
        //    listProductCate_Vendor.InnerHtml = sb.ToString();


        //    sSql = "select vcid,vcname from [Member].[dbo].[VedioCate] where isshow=0 order by vcno asc";
        //    cm.CommandText = sSql;
        //    dt = new DataTable();
        //    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
        //    {
        //        da.Fill(dt);
        //    }
        //    sb = new StringBuilder();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        sb.Append("<a class='list-group-item list-group-item-action' id='listVedioCate_" + dr["vcid"].ToString() + "' data-toggle='list' href='#' role='tab' aria-controls='home' onclick=\"window.location = '../VedioCate/VedioCate_001_1.aspx?vcid=" + dr["vcid"].ToString() + "'\">" + dr["vcname"].ToString() + "</a>");
        //    }
        //    listVedioCate.InnerHtml = sb.ToString();
  


        //    sSql = "select edmid,edmname from [Member].[dbo].[eDMCate] where isshow=0 order by edmno asc";
        //    cm.CommandText = sSql;
        //    dt = new DataTable();
        //    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
        //    {
        //        da.Fill(dt);
        //    }
        //    sb = new StringBuilder();

        //    foreach (DataRow dr in dt.Rows)
        //    {
        //        sb.Append("<a class='list-group-item list-group-item-action' id='listEDMCate_" + dr["edmid"].ToString() + "' data-toggle='list' href='#' role='tab' aria-controls='home' onclick=\"window.location = '../EDMCate/EDMCate_001_1.aspx?edmid=" + dr["edmid"].ToString() + "'\">" + dr["edmname"].ToString() + "</a>");
        //    }
        //    listEDMCate.InnerHtml = sb.ToString();

        //}
    }

    protected void btnModifyPeoplePW_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {


            //sSql = "   update [Member].[dbo].[People]  set pwd=@pwd                                               \n";
            //sSql += "    where 1=1                                                              \n";
            //sSql += " and paccount='" + LoginUser.PK + "' \n";
            sSql = @"update Account set PASSWORD = @pwd
                where USER_NAME = '" + LoginUser.PK + "' ";

            cm.CommandText = sSql;
            cm.Parameters.Clear();
            cm.Parameters.AddWithValue("@pwd", pw1.Text);
            cm.ExecuteNonQuery();

            script = "alert('修改完成!')";
        }


    }
}
