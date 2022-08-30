using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class CertPersonalAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();

        string msg = "驗證失敗!";
        string account = Request["account"];
        string code = Request["code"];

        string sSql = @"select * from  [Member].[dbo].[Personal] where account=@account ";


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
                string pid = dt.Rows[0]["pid"].ToString();
                string code2 = accessDao.Hash(pid);

                if (code2 == code)
                {
                    sSql = @"update [Member].[dbo].[Personal] 
 set isCert=@isCert
 where pid=@pid ";

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@pid", pid);
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