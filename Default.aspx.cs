using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : System.Web.UI.Page
{
    protected string script;
    protected void Page_Load(object sender, EventArgs e)
    {
        //LoginControl1.goMain = true;

        if (Request.QueryString["Logout"] != null)
        {
            LoginUser.logout();
            Response.Redirect("~");
        }

        if (Session["PageChange"] != null)
        {
            script = "  $('div[name=menu]').hide();$('#" + (string)Session["PageChange"] + "').show();";
            Session["PageChange"] = null;
        }

    }

    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(txtPassword.Text))
        {
            Javascript.Alert("密碼不能為空白");
            return;
        }
        bool flag=LoginUser.validateUser(txtLogin.Text, txtPassword.Text);

        if (!flag)
        {
            //lblError.Text = "帳號或密碼錯誤!";
            txtLogin.Text = "";
            txtPassword.Text = "";
            Javascript.Alert("帳號或密碼錯誤!");
            return;
        }

        Response.Redirect("material/Default.aspx");
    }

    protected void BtnEnter_Click(object sender, EventArgs e)
    {
        Session["ViewMode"] = null;
        if (LoginUser.role == 2)
        {
            //教師全引導至此業
            Response.Redirect("DefaultPage.aspx");
        }
        else
        {
            Response.Redirect(Request.ApplicationPath + "/login/loginDefault.aspx");
        }

    }
}