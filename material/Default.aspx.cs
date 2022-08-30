using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class material_Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["subMemu"] = null;
        Session["selectSubMenu"] = null;
    }
}