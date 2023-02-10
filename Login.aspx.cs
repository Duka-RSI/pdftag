using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
public partial class Login : System.Web.UI.Page
{
    protected string script;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        string account = txtAccount.Text;
        string pwd = txtPw.Text;
        bool bLogin = false;

        try
        {
            pwd = PublicFunction.Base64Encode(pwd);
            bLogin = LoginUser.validateUser(account, pwd);

            LogFile.Logger.Log("Login account="+ account+" ,isLogin="+ bLogin);

          


        }
        catch (Exception ex)
        {
            LogFile.Logger.LogException(ex);
            //Response.Write("<!--btnLogin_Click:" + ex.ToString() + "-->");
            script = "alert('登入失敗!請聯絡系統管理員');";
            return;
        }

        if (bLogin)
        {
            Response.Redirect("material/Default.aspx");
            Response.End();
        }
        else
        {
            LogFile.Logger.Log("Login Fail.account="+ account+" pw="+ pwd);
            script = "alert('登入失敗,帳號密碼失敗!');";
        }

    }
}