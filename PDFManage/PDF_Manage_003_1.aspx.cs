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

        StringBuilder sb = new StringBuilder();

        try
        {
            sSql = "select a.*,b1.hsl1name,b2.hsl2name,b3.hsl3name,b4.hsl4name \n";
            sSql += "from PDFTAG.dbo.Lu_Header a              \n";
            sSql += " left join PDFTAG.dbo.Lu_Header_stylelevel1 b1 on a.STYLE_DESC_LEVEL1=b1.hsl1id              \n";
            sSql += " left join PDFTAG.dbo.Lu_Header_stylelevel2 b2 on a.STYLE_DESC_LEVEL2=b2.hsl2id              \n";
            sSql += " left join PDFTAG.dbo.Lu_Header_stylelevel3 b3 on a.STYLE_DESC_LEVEL3=b3.hsl3id              \n";
            sSql += " left join PDFTAG.dbo.Lu_Header_stylelevel4 b4 on a.STYLE_DESC_LEVEL4=b4.hsl4id              \n";
            sSql += " where 1=1 and a.isshow=0   \n";
            sSql += " and a.pipid = '" + hidpipid.Value + "'   \n";
            Response.Write("<!--" + sSql + "-->");

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();
                string org_luhid = dt.Rows[0]["org_luhid"].ToString();

                hidluhid.Value = luhid;

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Header a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid in (select org_luhid as luhid from  PDFTAG.dbo.Lu_Header where pipid = '" + hidpipid.Value + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtLu_Header_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_Org);
                }


                List<Lu_Ch_Note> arrNotes = new List<Lu_Ch_Note>();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.Lu_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                DataTable dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                #region 類別1234

                sSql = "select hsl1id,hsl1name \n";
                sSql += "from PDFTAG.dbo.Lu_Header_stylelevel1  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.isshow=0 \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header_stylelevel1 = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_stylelevel1);
                }

                sSql = "select hsl2id,hsl2name \n";
                sSql += "from PDFTAG.dbo.Lu_Header_stylelevel2  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.isshow=0 \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header_stylelevel2 = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_stylelevel2);
                }

                sSql = "select hsl3id,hsl3name \n";
                sSql += "from PDFTAG.dbo.Lu_Header_stylelevel3  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.isshow=0 \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header_stylelevel3 = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_stylelevel3);
                }

                sSql = "select hsl4id,hsl4name \n";
                sSql += "from PDFTAG.dbo.Lu_Header_stylelevel4  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.isshow=0 \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header_stylelevel4 = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header_stylelevel4);
                }

                #endregion

                #region Lu_Header

                string season = dt.Rows[0]["Season"].ToString();
                string style = dt.Rows[0]["style"].ToString();
                string styledesc = dt.Rows[0]["styledesc"].ToString();
                string productsize = dt.Rows[0]["productsize"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string sClass = dt.Rows[0]["class"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();
                bool isEditHeader = (dt.Rows[0]["isEdit"].ToString().ToLower() == "true" ? true : false);

                string hsl1name = dt.Rows[0]["hsl1name"].ToString();
                string hsl2name = dt.Rows[0]["hsl2name"].ToString();
                string hsl3name = dt.Rows[0]["hsl3name"].ToString();
                string hsl4name = dt.Rows[0]["hsl4name"].ToString();

                string STYLE_DESC_LEVEL1 = dt.Rows[0]["STYLE_DESC_LEVEL1"].ToString();
                string STYLE_DESC_LEVEL2 = dt.Rows[0]["STYLE_DESC_LEVEL2"].ToString();
                string STYLE_DESC_LEVEL3 = dt.Rows[0]["STYLE_DESC_LEVEL3"].ToString();
                string STYLE_DESC_LEVEL4 = dt.Rows[0]["STYLE_DESC_LEVEL4"].ToString();

                string stylesketch = dt.Rows[0]["stylesketch"].ToString();

                DataRow[] drOrgHeades = dtLu_Header_Org.Select("luhid='" + org_luhid + "'");

                if (isEditHeader)
                {
                    Response.Write("<!--isEditHeader=" + isEditHeader + "-->");

                    if (drOrgHeades.Length > 0)
                    {
                        DataRow drOrgHeader = drOrgHeades[0];

                        season = Compare(drOrgHeader["season"].ToString(), season, FilterNote(arrNotes, luhid, "season"));
                        style = Compare(drOrgHeader["style"].ToString(), style, FilterNote(arrNotes, luhid, "style"));
                        styledesc = Compare(drOrgHeader["styledesc"].ToString(), styledesc, FilterNote(arrNotes, luhid, "styledesc"));
                        productsize = Compare(drOrgHeader["productsize"].ToString(), productsize, FilterNote(arrNotes, luhid, "productsize"));
                        brand = Compare(drOrgHeader["brand"].ToString(), brand, FilterNote(arrNotes, luhid, "brand"));
                        dividion = Compare(drOrgHeader["dividion"].ToString(), dividion, FilterNote(arrNotes, luhid, "dividion"));
                        pod = Compare(drOrgHeader["pod"].ToString(), pod, FilterNote(arrNotes, luhid, "pod"));
                        stylestatus = Compare(drOrgHeader["stylestatus"].ToString(), stylestatus, FilterNote(arrNotes, luhid, "stylestatus"));
                        generateddate = Compare(drOrgHeader["generateddate"].ToString(), generateddate, FilterNote(arrNotes, luhid, "generateddate"));
                    }

                }

                sb.Append("<table class='table table-hover'>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Season</th>");
                sb.Append(" <td scope='col'>" + season + "</td>");
                sb.Append(" <th scope='col'>Style</th>");
                sb.Append(" <td scope='col'>" + style + "</td>");
                sb.Append(" <th scope='col'>Style Desc</th>");
                sb.Append(" <td scope='col' >" + styledesc + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Product Size</th>");
                sb.Append(" <td scope='col'>" + productsize + "</td>");
                sb.Append(" <th scope='col'>Brand</th>");
                sb.Append(" <td scope='col' >" + brand + "</td>");
                sb.Append(" <th scope='col'>Dividion</th>");
                sb.Append(" <td scope='col' >" + dividion + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Class</th>");
                sb.Append(" <td scope='col'>" + sClass + "</td>");
                sb.Append(" <th scope='col'>Pod</th>");
                sb.Append(" <td scope='col'>" + pod + "</td>");
                sb.Append(" <th scope='col'>Style Status</th>");
                sb.Append(" <td scope='col'>" + stylestatus + "</td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Generated Date</th>");
                sb.Append(" <td scope='col'>" + generateddate + "</td>");
                sb.Append(" <th scope='col'>類別</th>");
                sb.Append(" <td scope='col'>" + (hsl1name + "-" + hsl2name + "-" + hsl3name + "-" + hsl4name) + "</td>");
                sb.Append(" <th scope='col'>圖檔位置</th>");
                sb.Append(" <td scope='col'>");

                string html = "";

                if (!string.IsNullOrEmpty(stylesketch))
                {
                    html += "<a href='#' onclick='Open(&#039;" + stylesketch + "&#039;)'>" + Path.GetFileName(stylesketch) + "</a>";
                    html += "<input type='button' value='刪除' onclick='deleteResultFile(" + luhid + ")'/>";
                }


                sb.Append(" <input type='file' onchange='uploadFile(this)' /><div id='divReultFile'>"+ html + "</div>");
                sb.Append(" </td>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>類別1</th>");
                sb.Append(" <td scope='col'>");

                sb.Append(" <select id='dlStyleLevel1'>");
                sb.Append(" <option value=''>請選擇</option>");
                foreach (DataRow dr in dtLu_Header_stylelevel1.Rows)
                {
                    sb.Append(" <option value='" + dr["hsl1id"].ToString() + "' " + (STYLE_DESC_LEVEL1 == dr["hsl1id"].ToString() ? "selected" : "") + ">" + dr["hsl1name"].ToString() + "</option>");
                }
                sb.Append(" </select>");
                sb.Append(" </td>");

                sb.Append(" <th scope='col'>類別2</th>");
                sb.Append(" <td scope='col'>");

                sb.Append(" <select id='dlStyleLevel2'>");
                sb.Append(" <option value=''>請選擇</option>");
                foreach (DataRow dr in dtLu_Header_stylelevel2.Rows)
                {
                    sb.Append(" <option value='" + dr["hsl2id"].ToString() + "' " + (STYLE_DESC_LEVEL2 == dr["hsl2id"].ToString() ? "selected" : "") + ">" + dr["hsl2name"].ToString() + "</option>");
                }
                sb.Append(" </select>");
                sb.Append(" </td>");

                sb.Append(" <th scope='col'>類別3</th>");
                sb.Append(" <td scope='col'>");

                sb.Append(" <select id='dlStyleLevel3'>");
                sb.Append(" <option value=''>請選擇</option>");
                foreach (DataRow dr in dtLu_Header_stylelevel3.Rows)
                {
                    sb.Append(" <option value='" + dr["hsl3id"].ToString() + "' " + (STYLE_DESC_LEVEL3 == dr["hsl3id"].ToString() ? "selected" : "") + ">" + dr["hsl3name"].ToString() + "</option>");
                }
                sb.Append(" </select>");
                sb.Append(" </td>");
                sb.Append("</tr>");

                sb.Append("<tr>");
                sb.Append(" <th scope='col'>類別4</th>");
                sb.Append(" <td scope='col'>");

                sb.Append(" <select id='dlStyleLevel4'>");
                sb.Append(" <option value=''>請選擇</option>");
                foreach (DataRow dr in dtLu_Header_stylelevel4.Rows)
                {
                    sb.Append(" <option value='" + dr["hsl4id"].ToString() + "' " + (STYLE_DESC_LEVEL4 == dr["hsl4id"].ToString() ? "selected" : "") + ">" + dr["hsl4name"].ToString() + "</option>");
                }
                sb.Append(" </select>");
                sb.Append(" </td>");
                sb.Append("</tr>");

                sb.Append("</table>");

                #endregion


            }

            divHeader.InnerHtml = sb.ToString();

        }
        catch (Exception err)
        {
            Response.Write("<!--" + err.ToString() + "-->");
        }
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
            string sFilePath = "";



            sSql = "update [PDFTAG].[dbo].[Lu_Header]    \n";
            sSql += " set STYLE_DESC_LEVEL1=@STYLE_DESC_LEVEL1,STYLE_DESC_LEVEL2=@STYLE_DESC_LEVEL2,STYLE_DESC_LEVEL3=@STYLE_DESC_LEVEL3,STYLE_DESC_LEVEL4=@STYLE_DESC_LEVEL4 \n";
            if (!string.IsNullOrEmpty(sFilePath))
            {
                sSql += " ,piuploadfile=@piuploadfile \n";
            }


            sSql += "where luhid=@luhid                          \n";


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@luhid", hidluhid.Value);
                cm.Parameters.AddWithValue("@STYLE_DESC_LEVEL1", hid_STYLE_DESC_LEVEL1.Value);
                cm.Parameters.AddWithValue("@STYLE_DESC_LEVEL2", hid_STYLE_DESC_LEVEL3.Value);
                cm.Parameters.AddWithValue("@STYLE_DESC_LEVEL3", hid_STYLE_DESC_LEVEL3.Value);
                cm.Parameters.AddWithValue("@STYLE_DESC_LEVEL4", hid_STYLE_DESC_LEVEL4.Value);
                if (!string.IsNullOrEmpty(sFilePath))
                    cm.Parameters.AddWithValue("@piuploadfile", sFilePath);

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