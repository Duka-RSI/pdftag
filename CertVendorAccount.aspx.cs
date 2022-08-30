using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class CertVendorAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        string msg = "驗證失敗!";
        string account = Request["account"];
        string code = Request["code"];

        string sSql = @"select * from  [Member].[dbo].[Vendor] where account=@account ";


        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            cm.Parameters.AddWithValue("@account", account);
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }

            if (dt.Rows.Count > 0)
            {
                AccessTokenClass accessDao = new AccessTokenClass();
                string vid = dt.Rows[0]["vid"].ToString();
                string code2 = accessDao.Hash(vid);

                if (code2 == code)
                {
                    sSql = @"update [Member].[dbo].[Vendor] 
 set isCert=@isCert
 where vid=@vid ";

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@vid", vid);
                    cm.Parameters.AddWithValue("@isCert", 1);
                    cm.ExecuteNonQuery();
                    msg = "驗證完成!";
                    Response.Write("<script>alert('" + msg + "');window.location = 'Login.aspx'</script>");
                    Response.End();
                    return;
                }
            }
        }

        Response.Write("<script>alert('" + msg + "');window.location = 'Index.aspx'</script>");
        Response.End();
    }
}