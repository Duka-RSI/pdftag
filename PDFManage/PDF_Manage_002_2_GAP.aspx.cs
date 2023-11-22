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
            //DataBind();
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


        sSql += "select c.pipid,b.pipid as copy_pipid,b.ptitle,b.pidate,b.piuploadfile,b.pver,b.creator,a.hdid,a.hisversion,a.editdate,a.pipid as org_pipid,c.gmid \n";
        sSql += "from PDFTAG.dbo.HistoryData a              \n";
        sSql += " join PDFTAG.dbo.P_inProcess b on a.hdid=b.hdid            \n";//找出複製的主檔
        sSql += " join PDFTAG.dbo.P_inProcess c on a.pipid=c.pipid            \n";//找出群組
        sSql += " where 1=1 and a.isshow=0  \n";
        sSql += " and a.hdid = '" + hdid + "'   \n";

        Response.Write("<!--" + sSql + "-->");
        using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
        {
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }
            string pipid = dt.Rows[0]["pipid"].ToString();
            string org_pipid = dt.Rows[0]["org_pipid"].ToString();//原文件
            hidorg_pipid.Value = dt.Rows[0]["org_pipid"].ToString();

            lblInfo.Text = dt.Rows[0]["copy_pipid"].ToString() + "-" + dt.Rows[0]["ptitle"].ToString() + "-" + dt.Rows[0]["hisversion"].ToString() + "-" +
                (dt.Rows[0]["pver"].ToString() == "1" ? "Lulu" : dt.Rows[0]["pver"].ToString() == "2" ? "UA" : dt.Rows[0]["pver"].ToString() == "3" ? "GAP" : "");

            hidpipid.Value = dt.Rows[0]["copy_pipid"].ToString();//複製的主檔pipid
                                                                 //divOrgFile.InnerHtml = "<a href='#' onclick='Open(&#039" + dt.Rows[0]["piuploadfile"].ToString() + "&#039;)'>" + System.IO.Path.GetFileName(dt.Rows[0]["piuploadfile"].ToString()) + "</a>";


            dlSourceFile.Items.Insert(0, new System.Web.UI.WebControls.ListItem("(原文件) " + System.IO.Path.GetFileName(dt.Rows[0]["piuploadfile"].ToString()) + " (" + hidorg_pipid.Value + ")", hidorg_pipid.Value));
            dlSourceFile.Items.Insert(1,
                new System.Web.UI.WebControls.ListItem(RemoveDateOnFileName(System.IO.Path.GetFileName(dt.Rows[0]["piuploadfile"].ToString())) + " (" + dt.Rows[0]["copy_pipid"].ToString() + ")"
                , dt.Rows[0]["copy_pipid"].ToString()));



            string gmid = dt.Rows[0]["gmid"].ToString();//群組
            Response.Write("<!--gmid=" + gmid + "-->");

            if (string.IsNullOrEmpty(gmid))
            {
                sSql = @"
              select a.pipid,a.piuploadfile,b.hisversion 
               from PDFTAG.dbo.P_inProcess a
                join PDFTAG.dbo.HistoryData b on a.hdid=b.hdid and b.isshow=0 and b.pipid=@pipid
               where a.isShow=0 and a.pver=3 and a.hdid >0
               order by hisversion asc";
            }
            else
            {
                sSql = @"
  select a.pipid,a.ptitle,a.piuploadfile,b.hisversion 
from PDFTAG.dbo.P_inProcess a
  join PDFTAG.dbo.HistoryData b on a.hdid=b.hdid and b.isshow=0 
where a.isShow=0 and a.pver=3 and a.pipid in (select pipid from PDFTAG.dbo.P_inProcess where isShow=0 and gmid='" + gmid + @"')
and a.pipid !='" + hidpipid.Value + @"'
order by hisversion asc";

                //顯示 所有群組內的文件
                sSql = @"
  select a.pipid,a.ptitle,a.piuploadfile,a.hdid
from PDFTAG.dbo.P_inProcess a
where a.isShow=0 and a.pver=3 and a.pipid in (select pipid from PDFTAG.dbo.P_inProcess where isShow=0 and gmid='" + gmid + @"')
and a.hdid not in (select hdid from PDFTAG.dbo.HistoryData where isshow=1  or pipid in (select pipid from PDFTAG.dbo.P_inProcess where isshow=1) )
order by a.pidate desc,a.pipid desc";
            }




            Response.Write("<!--" + sSql + "-->");
            cm.CommandText = sSql;
            cm.Parameters.Clear();
            cm.Parameters.AddWithValue("@pipid", hidorg_pipid.Value);
            dt = new DataTable();
            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }


            foreach (DataRow dr in dt.Rows)
            {
                //if (!string.IsNullOrEmpty(dr["hisversion"].ToString()))
                //    //dr["piuploadfile"] = dr["ptitle"].ToString() + "-" + dr["hisversion"].ToString() + "-" + System.IO.Path.GetFileName(dr["piuploadfile"].ToString());
                //    //dr["piuploadfile"] = dr["ptitle"].ToString() + "-" + dr["hisversion"].ToString();
                //    dr["piuploadfile"] = RemoveDateOnFileName(System.IO.Path.GetFileName(dr["piuploadfile"].ToString())) + " (" + dr["pipid"].ToString() + ")";
                //else

                if (dr["hdid"].ToString() == "0")
                {
                    dr["piuploadfile"] = "(原文件) " + System.IO.Path.GetFileName(dr["piuploadfile"].ToString()) + " (" + dr["pipid"].ToString() + ")";
                }
                else
                {
                    dr["piuploadfile"] = RemoveDateOnFileName(System.IO.Path.GetFileName(dr["piuploadfile"].ToString())) + " (" + dr["pipid"].ToString() + ")";
                }


            }

            dlVerFile.DataTextField = "piuploadfile";
            dlVerFile.DataValueField = "pipid";
            dlVerFile.DataSource = dt;
            dlVerFile.DataBind();

            //dlVerFile.Items.Insert(0, new System.Web.UI.WebControls.ListItem("原文件 ("+ hidorg_pipid.Value + ")", ""));


        }

    }

    public string Compare(string type, string org, string newText, string note)
    {
        if (type == "1" || type == "")
        {
            if (org == newText)
                return "";
            else
                return "<font color='red' data-org='" + org + "' data-newText='" + newText + "'>修:" + newText + "</font><br><font color='red'>中:" + note + "</font>";
        }
        if (type == "2")
        {
            if (org == newText)
                return "";
            else
                return org + "<br><font color='red'>修:" + newText + "</font>";
        }
        return "";
    }

    public class GAP_Ch_Note
    {
        public long Id { get; set; }
        public string ColName { get; set; }

        public string Note { get; set; }
    }
    public class GAP_Bom
    {
        public int rowId { get; set; }
        public string lubid { get; set; }
        public string org_lubid { get; set; }
        public string StandardPlacement { get; set; }

        public string Placement { get; set; }
        public string Usage { get; set; }
        public string SupplierArticle { get; set; }
        public string Supplier { get; set; }
        public string QualityDetails { get; set; }

        public string ColorName { get; set; }

        public string ColorVal { get; set; }

        public string ColorBName { get; set; }


        public string StandardPlacement_org { get; set; }

        public string Placement_org { get; set; }

        public string Usage_org { get; set; }

        public string SupplierArticle_org { get; set; }

        public string Supplier_org { get; set; }
        public string QualityDetails_org { get; set; }

        public bool isExistA { get; set; }
    }

    public class HeaderExist
    {
        public string ColName { get; set; }

        public bool IsExistSource { get; set; }

        public int ExistSourceIndex { get; set; }

        public bool IsExistCompare { get; set; }

        public int ExistCompareIndex { get; set; }
    }
    public string FilterNote(List<GAP_Ch_Note> arrData, string id, string colName)
    {
        var res = arrData.FirstOrDefault(w => w.Id == long.Parse(id) && w.ColName.ToLower() == colName.ToLower());
        if (res == null) return "";

        return res.Note;
    }

    private void DataBind()
    {
        bool _AllowPaging = true;
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        DataTable dtCompare = new DataTable();
        string sSql = "";

        string sSource_pipid = dlSourceFile.SelectedItem.Value;
        string sCompare_pipid = dlVerFile.SelectedItem.Value;
        //string sType = dlType.SelectedItem.Value;
        string sType = "";
        List<string> arrCompareTypes = new List<string>() { "1", "2" };//比對項目1:更改2:中文備註

        StringBuilder sb = new StringBuilder();
        StringBuilder sbNotExit = new StringBuilder();
        try
        {
            if (string.IsNullOrEmpty(sCompare_pipid))
            {
                sCompare_pipid = hidorg_pipid.Value;
                sType = "1";
            }

            Response.Write("<!--sSource_pipid=" + sSource_pipid + " sCompare_pipid=" + sCompare_pipid + "-->");

            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.GAP_Header a              \n";
            sSql += " where 1=1 and a.isshow=0   \n";
            sSql += " and a.pipid = @pipid   \n";
            Response.Write("<!--" + sSql + "-->");

            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                string luhid = dt.Rows[0]["luhid"].ToString();
                string org_luhid = dt.Rows[0]["org_luhid"].ToString();

                if (string.IsNullOrEmpty(org_luhid))
                    org_luhid = luhid;//原文件

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                }

                string compare_luhid = dtCompare.Rows[0]["luhid"].ToString();
                string compare_org_luhid = dtCompare.Rows[0]["org_luhid"].ToString();

                if (string.IsNullOrEmpty(compare_org_luhid))
                    compare_org_luhid = compare_luhid;//原文件

                #region GAP_Header

                string season_soure = dt.Rows[0]["Season"].ToString();
                string style_soure = dt.Rows[0]["style"].ToString();
                string styledesc_soure = dt.Rows[0]["styledesc"].ToString();
                string productsize_soure = dt.Rows[0]["productsize"].ToString();
                string productDescription_soure = dt.Rows[0]["ProductDescription"].ToString();
                string brand_soure = dt.Rows[0]["brand"].ToString();
                string dividion_soure = dt.Rows[0]["dividion"].ToString();
                string pod_soure = dt.Rows[0]["pod"].ToString();
                string strClass_soure = dt.Rows[0]["class"].ToString();
                string stylestatus_soure = dt.Rows[0]["stylestatus"].ToString();
                string generateddate_soure = dt.Rows[0]["generateddate"].ToString();

                string season = dt.Rows[0]["Season"].ToString();
                string style = dt.Rows[0]["style"].ToString();
                string styledesc = dt.Rows[0]["styledesc"].ToString();
                string productsize = dt.Rows[0]["productsize"].ToString();
                string productDescription = dt.Rows[0]["ProductDescription"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string strClass = dt.Rows[0]["class"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();

                string season_compare = dtCompare.Rows[0]["Season"].ToString();
                string style_compare = dtCompare.Rows[0]["style"].ToString();
                string styledesc_compare = dtCompare.Rows[0]["styledesc"].ToString();
                string productsize_compare = dtCompare.Rows[0]["productsize"].ToString();
                string productDescription_compare = dtCompare.Rows[0]["ProductDescription"].ToString();
                string brand_compare = dtCompare.Rows[0]["brand"].ToString();
                string dividion_compare = dtCompare.Rows[0]["dividion"].ToString();
                string pod_compare = dtCompare.Rows[0]["pod"].ToString();
                string strClass_compare = dtCompare.Rows[0]["class"].ToString();
                string stylestatus_compare = dtCompare.Rows[0]["stylestatus"].ToString();
                string generateddate_compare = dtCompare.Rows[0]["generateddate"].ToString();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.GAP_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                var arrData = dt.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                string season_note = FilterNote(arrData, luhid, "season");
                string style_note = FilterNote(arrData, luhid, "style");
                string styledesc_note = FilterNote(arrData, luhid, "styledesc");
                string productsize_note = FilterNote(arrData, luhid, "productsize");
                string productDescription_note = FilterNote(arrData, luhid, "productDescription");
                string brand_note = FilterNote(arrData, luhid, "brand");
                string dividion_note = FilterNote(arrData, luhid, "dividion");
                string pod_note = FilterNote(arrData, luhid, "pod");
                string strClass_note = FilterNote(arrData, luhid, "class");
                string stylestatus_note = FilterNote(arrData, luhid, "stylestatus");
                string generateddate_note = FilterNote(arrData, luhid, "generateddate");

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                dtCompare = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                }

                arrData = dtCompare.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                string season_compare_note = FilterNote(arrData, compare_luhid, "season");
                string style_compare_note = FilterNote(arrData, compare_luhid, "style");
                string styledesc_compare_note = FilterNote(arrData, compare_luhid, "styledesc");
                string productsize_compare_note = FilterNote(arrData, compare_luhid, "productsize");
                string productDescription_compare_note = FilterNote(arrData, compare_luhid, "productDescription");
                string brand_compare_note = FilterNote(arrData, compare_luhid, "brand");
                string dividion_compare_note = FilterNote(arrData, compare_luhid, "dividion");
                string pod_compare_note = FilterNote(arrData, compare_luhid, "pod");
                string strClass_compare_note = FilterNote(arrData, compare_luhid, "class");
                string stylestatus_compare_note = FilterNote(arrData, compare_luhid, "stylestatus");
                string generateddate_compare_note = FilterNote(arrData, compare_luhid, "generateddate");

                if (sType == "2")
                {
                    #region 中文備註


                    season = season_note;
                    style = style_note;
                    styledesc = styledesc_note;
                    productsize = productsize_note;
                    productDescription = productDescription_note;
                    brand = brand_note;
                    dividion = dividion_note;
                    pod = pod_note;
                    strClass = strClass_note;
                    stylestatus = stylestatus_note;
                    generateddate = generateddate_note;

                    cm.CommandText = sSql;

                    season_compare = season_compare_note;
                    style_compare = style_compare_note;
                    styledesc_compare = styledesc_compare_note;
                    productsize_compare = productsize_compare_note;
                    productDescription_compare = productDescription_compare_note;
                    brand_compare = brand_compare_note;
                    dividion_compare = dividion_compare_note;
                    pod_compare = pod_compare_note;
                    strClass_compare = strClass_compare_note;
                    stylestatus_compare = stylestatus_compare_note;
                    generateddate_compare = generateddate_compare_note;

                    #endregion
                }

                sb.Append("<table class='table table-hover'>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Season</th>");
                sb.Append(" <th scope='col'>Style</th>");
                sb.Append(" <th scope='col'>Style Desc</th>");
                sb.Append(" <th scope='col'>Product Description</th>");
                sb.Append(" <th scope='col'>Brand</th>");
                sb.Append(" <th scope='col'>Class</th>");
                sb.Append(" <th scope='col'>Style Status</th>");
                sb.Append(" <th scope='col'>Generated Date</th>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='Season' onclick='editHeader(this)'>" + season_soure + (string.IsNullOrEmpty(season_note) ? "" : "<br>中:" + season_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='style' onclick='editHeader(this)'>" + style_soure + (string.IsNullOrEmpty(style_note) ? "" : "<br>中:" + style_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='styledesc' onclick='editHeader(this)'>" + styledesc_soure + (string.IsNullOrEmpty(styledesc_note) ? "" : "<br>中:" + styledesc_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='productdescription' onclick='editHeader(this)'>" + productDescription_soure + (string.IsNullOrEmpty(productDescription_note) ? "" : "<br>中:" + productDescription_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='brand' onclick='editHeader(this)'>" + brand_soure + (string.IsNullOrEmpty(brand_note) ? "" : "<br>中:" + brand_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='class' onclick='editHeader(this)'>" + strClass_soure + (string.IsNullOrEmpty(strClass_note) ? "" : "<br>中:" + strClass_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='stylestatus' onclick='editHeader(this)'>" + stylestatus_soure + (string.IsNullOrEmpty(stylestatus_note) ? "" : "<br>中:" + stylestatus_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='generateddate' onclick='editHeader(this)'>" + generateddate_soure + (string.IsNullOrEmpty(generateddate_note) ? "" : "<br>中:" + generateddate_note) + "</td>");
                sb.Append("</tr>");


                sb.Append("<tr class='rowCompare'>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='Season'>" + Compare(sType, season, season_compare, season_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='style'>" + Compare(sType, style, style_compare, style_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='styledesc'>" + Compare(sType, styledesc, styledesc_compare, styledesc_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='productdescription'>" + Compare(sType, productDescription, productDescription_compare, productDescription_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='brand'>" + Compare(sType, brand, brand_compare, brand_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='class'>" + Compare(sType, strClass, strClass_compare, strClass_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='stylestatus'>" + Compare(sType, stylestatus, stylestatus_compare, stylestatus_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' onclick='editHeader(this,1)' data-col='generateddate'>" + Compare(sType, generateddate, generateddate_compare, generateddate_compare_note) + "</td>");
                sb.Append("</tr>");


                sb.Append("</table>");

                #endregion

                #region GAP_BOM

                sSql = "select a.*,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,'0' as isExist \n";
                sSql += "from PDFTAG.dbo.GAP_BOM a              \n";
                sSql += "join  PDFTAG.dbo.GAP_BOMGarmentcolor b on a.lubcid=b.lubcid               \n";
                sSql += " where 1=1   \n";
                //sSql += " and a.luhid = @luhid   \n";
                sSql += " and a.pipid = @pipid   \n";
                //sSql += " order by lubid,rowid asc    \n";
                sSql += "  order by (CASE WHEN org_lubid is null THEN lubid ELSE org_lubid END) asc ,rowid asc    \n";
                Response.Write("<!--" + sSql.Replace("@luhid", luhid) + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                Response.Write("<!--" + sSql.Replace("@luhid", compare_luhid) + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", compare_luhid);
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                dtCompare = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                }

                sSql = "select distinct A1,A2,A3,A4,A5,A6,A7,A8,A9,A10 \n";
                sSql += "from PDFTAG.dbo.GAP_BOMGarmentcolor a              \n";
                //sSql += " where a.luhid = @luhid   \n";
                sSql += " where pipid = @pipid   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                DataTable dtGAP_BOMGarmentcolor = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_BOMGarmentcolor);
                }

                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", compare_luhid);
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                DataTable dtGAP_BOMGarmentcolor_Compare = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_BOMGarmentcolor_Compare);
                }

                List<string> arrGAP_BOMGarmentcolorsSource = new List<string>();
                List<string> arrGAP_BOMGarmentcolorsCompare = new List<string>();
                //List<string> arrColors = new List<string>();
                List<string> arrGAP_BOMGarmentcolors = new List<string>();


                foreach (DataRow drColor in dtGAP_BOMGarmentcolor.Rows)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        string sA = drColor["A" + i].ToString();

                        if (string.IsNullOrEmpty(sA))
                        {
                            break;
                        }

                        arrGAP_BOMGarmentcolors.Add(sA);
                        arrGAP_BOMGarmentcolorsSource.Add(sA);
                    }
                }
                foreach (DataRow drColor in dtGAP_BOMGarmentcolor_Compare.Rows)
                {
                    for (int i = 1; i <= 10; i++)
                    {
                        string sA = drColor["A" + i].ToString();

                        if (string.IsNullOrEmpty(sA))
                        {
                            break;
                        }

                        arrGAP_BOMGarmentcolors.Add(sA);
                        arrGAP_BOMGarmentcolorsCompare.Add(sA);
                    }
                }

                arrGAP_BOMGarmentcolors = arrGAP_BOMGarmentcolors.Distinct().ToList();

                //foreach (var color in arrColors.Distinct().ToList())
                //{
                //    arrGAP_BOMGarmentcolors.Add(new HeaderExist { ColName = color });
                //}



                List<GAP_Ch_Note> arrNotes = new List<GAP_Ch_Note>();
                List<GAP_Ch_Note> arrNotesCompare = new List<GAP_Ch_Note>();
                //if (sType == "2")
                {
                    #region 中文備註

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                    sSql += "where 1=1 \n";
                    sSql += " and a.IdName='lubid'  and a.Id in (select lubid as id from PDFTAG.dbo.GAP_BOM where pipid = @pipid ) \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    //cm.Parameters.AddWithValue("@luhid", luhid);
                    cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                    DataTable dt2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt2);
                    }

                    arrNotes = dt2.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    //cm.Parameters.AddWithValue("@luhid", compare_luhid);
                    cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                    DataTable dtCompare2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtCompare2);
                    }

                    arrNotesCompare = dtCompare2.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    #endregion
                }

                //var arrTypes = dt.AsEnumerable().Select(s => s.Field<string>("type")).Distinct().ToList();
                var arrTypes = dt.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type") }).Distinct().ToList();

                List<string> arrExistTypes = new List<string>();
                List<string> arrNotCompareExistTypes = new List<string>();

                foreach (var itemType in arrTypes)
                {
                    StringBuilder sbHeader = new StringBuilder();

                    //if (!arrExistTypes.Contains(itemType.type))
                    sbHeader.Append("<h4>" + itemType.type + "</h4>");

                    arrExistTypes.Add(itemType.type);


                    sbHeader.Append("<table class='table table-hover'>");
                    sbHeader.Append("<tr>");
                    //sb.Append(" <th scope='col'>Standard Placement</th>");
                    //sb.Append(" <th scope='col'>Placement</th>");
                    //sb.Append(" <th scope='col'>Supplier / Supplier Article</th>"); ;

                    sbHeader.Append(" <th scope='col'>Product</th>");
                    sbHeader.Append(" <th scope='col'>Usage</th>");
                    sbHeader.Append(" <th scope='col'>Supplier Article Number</th>");
                    sbHeader.Append(" <th scope='col'>Quality Details</th>");
                    sbHeader.Append(" <th scope='col'>Supplier[Allocate]</th>");


                    List<bool> arrCompareHeaderResult = new List<bool>();
                    List<string> arrBomHeader = new List<string>();
                    List<string> arrBomHeaderCompare = new List<string>();



                    #region lubcid

                    //20220419
                    //	來源文件 與 比對目的 都有的欄位 ，欄位底色顯示水藍色 #00FFFF
                    //	來源文件 有的欄位，但 比對目的 沒有的欄位，欄位底色 顯示 橘色 	#FF9224
                    //	來源文件 沒有的欄位，但 比對目的 有的欄位，欄位底色 顯示 紅色 #FF0000



                    //foreach (var color in arrGAP_BOMGarmentcolors)
                    //{
                    //    var isExistSource = arrGAP_BOMGarmentcolorsSource.Any(x => x == color);
                    //    var isExistCompare = arrGAP_BOMGarmentcolorsCompare.Any(x => x == color);



                    //    if (isExistSource && isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#00FFFF'> " + color + "</th>");
                    //    else if (isExistSource && !isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#FF9224'> " + color + "</th>");
                    //    else if (!isExistSource && isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#FF0000'> " + color + "</th>");


                    //}

                    //20220425 以原文件欄位不動,去B找資料
                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.GAP_BOMGarmentcolor a              \n";
                    sSql += " where a.lubcid = '" + itemType.lubcid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    dtGAP_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtGAP_BOMGarmentcolor);
                    }
                    arrGAP_BOMGarmentcolors = new List<string>();
                    int iColorCnt = 0;
                    foreach (DataRow drColor in dtGAP_BOMGarmentcolor.Rows)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string sA = drColor["A" + i].ToString();

                            if (string.IsNullOrEmpty(sA))
                            {
                                break;
                            }

                            var isExistCompare = arrGAP_BOMGarmentcolorsCompare.Any(x => x == sA);

                            if (!isExistCompare)
                                sbHeader.Append(" <th scope='col' style='background-color:#FF9224'>" + sA + "</th>");
                            else
                                sbHeader.Append(" <th scope='col' style='background-color:#00FFFF'>" + sA + "</th>");
                            arrGAP_BOMGarmentcolors.Add(sA);
                            iColorCnt++;
                        }
                    }

                    sbHeader.Append("</tr>");

                    if (iColorCnt == 0)//沒有色組的部分不用顯示
                        continue;

                    sb.Append(sbHeader);

                    DataRow[] drBoms = dt.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "' ", "org_lubid asc");
                    DataRow[] drComareBoms = dtCompare.Select("type='" + itemType.type + "' ", "org_lubid asc");

                    List<GAP_Bom> arrCompareData = new List<GAP_Bom>();

                    int iRowId = 1;
                    foreach (DataRow drBomMain in drComareBoms)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            string sColorName = drBomMain["A" + i].ToString();
                            string sColorVal = drBomMain["B" + i].ToString();

                            if (string.IsNullOrEmpty(sColorName))
                            {
                                break;
                            }

                            arrCompareData.Add(new GAP_Bom()
                            {
                                rowId = iRowId,
                                lubid = drBomMain["lubid"].ToString(),
                                org_lubid = drBomMain["org_lubid"].ToString(),
                                StandardPlacement = drBomMain["StandardPlacement"].ToString().Replace(" ", string.Empty),
                                //Placement = drBomMain["Placement"].ToString().Replace(" ", string.Empty),
                                Usage = drBomMain["Usage"].ToString().Replace(" ", string.Empty),
                                SupplierArticle = drBomMain["SupplierArticle"].ToString().Replace(" ", string.Empty),
                                Supplier = drBomMain["Supplier"].ToString(),
                                QualityDetails = drBomMain["QualityDetails"].ToString(),

                                StandardPlacement_org = drBomMain["StandardPlacement"].ToString(),
                                //Placement_org = drBomMain["Placement"].ToString(),
                                Usage_org = drBomMain["Usage"].ToString(),
                                SupplierArticle_org = drBomMain["SupplierArticle"].ToString(),
                                Supplier_org = drBomMain["Supplier"].ToString(),
                                QualityDetails_org = drBomMain["QualityDetails"].ToString(),

                                ColorName = sColorName,
                                ColorVal = sColorVal,
                                ColorBName = "B" + i
                            });

                        }

                        iRowId++;
                    }

                    //20220419 原始不動,去比對找資料
                    //foreach (DataRow drBom in drBoms)
                    for (int b = 0; b < drBoms.Length; b++)
                    {
                        string lubid = "";
                        string org_lubid = "";


                        string standardPlacement_source = "";
                        string placement_source = "";
                        string usage_source = "";
                        string supplierArticle_source = "";
                        string supplier_source = "";
                        string QualityDetails_source = "";

                        string standardPlacement = "";
                        string placement = "";
                        string usage = "";
                        string supplierArticle = "";
                        string supplier = "";
                        string QualityDetails = "";


                        string standardPlacement_note = "";
                        string placement_note = "";
                        string usage_note = "";
                        string supplierArticle_note = "";
                        string supplier_note = "";
                        string QualityDetails_note = "";


                        try
                        {
                            lubid = drBoms[b]["lubid"].ToString();
                            org_lubid = drBoms[b]["org_lubid"].ToString();

                            if (string.IsNullOrEmpty(org_lubid))
                                org_lubid = lubid;//原文件

                            standardPlacement_source = drBoms[b]["StandardPlacement"].ToString();
                            usage_source = drBoms[b]["usage"].ToString();
                            supplierArticle_source = drBoms[b]["SupplierArticle"].ToString();
                            supplier_source = drBoms[b]["Supplier"].ToString();
                            QualityDetails_source = drBoms[b]["QualityDetails"].ToString();

                            standardPlacement = drBoms[b]["StandardPlacement"].ToString();
                            usage = drBoms[b]["usage"].ToString();
                            supplierArticle = drBoms[b]["SupplierArticle"].ToString();
                            supplier = drBoms[b]["Supplier"].ToString();
                            QualityDetails = drBoms[b]["QualityDetails"].ToString();

                            standardPlacement_note = FilterNote(arrNotes, lubid, "StandardPlacement");
                            usage_note = FilterNote(arrNotes, lubid, "usage");
                            supplierArticle_note = FilterNote(arrNotes, lubid, "SupplierArticle");
                            supplier_note = FilterNote(arrNotes, lubid, "Supplier");
                            QualityDetails_note = FilterNote(arrNotes, lubid, "QualityDetails");
                        }
                        catch (Exception ex)
                        {
                            //Response.Write("<!--" + ex.ToString() + "-->");
                        }



                        string lubid_compare = "";
                        string lubid_org_compare = "";
                        string standardPlacement_compare = "";
                        string placement_compare = "";
                        string usage_compare = "";
                        string supplierArticle_compare = "";
                        string supplier_compare = "";
                        string QualityDetails_compare = "";

                        string standardPlacement_compare_note = "";
                        string placement_compare_note = "";
                        string usage_compare_note = "";
                        string supplierArticle_compare_note = "";
                        string supplier_compare_note = "";
                        string QualityDetails_compare_note = "";

                        try
                        {
                            var tmpCompareData = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                    && x.Usage == usage && x.SupplierArticle == supplierArticle);
                            lubid_compare = tmpCompareData.lubid;
                            lubid_org_compare = tmpCompareData.org_lubid;
                            //lubid_compare = drComareBoms[b]["lubid"].ToString();
                            //lubid_org_compare = drComareBoms[b]["org_lubid"].ToString();

                            if (string.IsNullOrEmpty(lubid_org_compare))
                                lubid_org_compare = lubid_compare;//原文件

                            standardPlacement_compare = tmpCompareData.StandardPlacement;
                            placement_compare = tmpCompareData.Placement;
                            usage_compare = tmpCompareData.Usage;
                            supplierArticle_compare = tmpCompareData.SupplierArticle;
                            supplier_compare = tmpCompareData.Supplier_org;
                            QualityDetails_compare = tmpCompareData.QualityDetails;
                            //standardPlacement_compare = drComareBoms[b]["StandardPlacement"].ToString();
                            //placement_compare = drComareBoms[b]["Placement"].ToString();
                            //usage_compare = drComareBoms[b]["usage"].ToString();
                            //supplierArticle_compare = drComareBoms[b]["SupplierArticle"].ToString();
                            //supplier_compare = drComareBoms[b]["Supplier"].ToString();
                            //QualityDetails_compare = drComareBoms[b]["QualityDetails"].ToString();

                            standardPlacement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "StandardPlacement");
                            placement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Placement");
                            usage_compare_note = FilterNote(arrNotesCompare, lubid_compare, "usage");
                            supplierArticle_compare_note = FilterNote(arrNotesCompare, lubid_compare, "SupplierArticle");
                            supplier_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Supplier");
                            QualityDetails_compare_note = FilterNote(arrNotesCompare, lubid_compare, " QualityDetails");
                        }
                        catch (Exception ex)
                        {
                            //Response.Write("<!--" + ex.ToString() + "-->");
                        }

                        bool isCompare = false;

                        //if (standardPlacement_source == standardPlacement_compare)
                        isCompare = true;

                        try
                        {
                            #region         

                            if (sType == "2")
                            {
                                standardPlacement = standardPlacement_note;
                                placement = placement_note;
                                usage = usage_note;
                                supplierArticle = supplierArticle_note;
                                supplier = supplier_note;
                                QualityDetails = QualityDetails_note;

                                standardPlacement_compare = standardPlacement_compare_note;
                                placement_compare = placement_compare_note;
                                usage_compare = usage_compare_note;
                                supplierArticle_compare = supplierArticle_compare_note;
                                supplier_compare = supplier_compare_note;
                                QualityDetails_compare = QualityDetails_compare_note;

                            }

                            sb.Append("<tr data-rowid='" + drBoms[b]["rowid"].ToString() + "'>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement' onclick='editBom(this,1)'>" + standardPlacement_source + (string.IsNullOrEmpty(standardPlacement_note) ? "" : "<br>中:" + standardPlacement_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='usage' onclick='editBom(this,1)'>" + usage_source + (string.IsNullOrEmpty(usage_note) ? "" : "<br>中:" + usage_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='SupplierArticle' onclick='editBom(this,1)'>" + supplierArticle_source + (string.IsNullOrEmpty(supplierArticle_note) ? "" : "<br>中:" + supplierArticle_note) + "</td>");

                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='QualityDetails' onclick='editBom(this)'>" + QualityDetails_source + (string.IsNullOrEmpty(QualityDetails_note) ? "" : "<br>中:" + QualityDetails_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier' onclick='editBom(this)'>" + supplier_source + (string.IsNullOrEmpty(supplier_note) ? "" : "<br>中:" + supplier_note) + "</td>");

                            List<string> arrColorNameSource = new List<string>();
                            List<string> arrColorValSource = new List<string>();

                            foreach (var color in arrGAP_BOMGarmentcolors)
                            {
                                bool isFind = false;
                                for (int i = 1; i <= 10; i++)
                                {
                                    string colorName = drBoms[b]["A" + i].ToString();
                                    if (color == colorName)
                                    {
                                        string colorVal = drBoms[b]["B" + i].ToString();
                                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' onclick='editBom(this,1)' data-col='B" + i + "' >" + colorVal + "</td>");
                                        isFind = true;

                                        arrColorNameSource.Add(colorName);
                                        arrColorValSource.Add(colorVal);
                                        break;
                                    }
                                }
                                if (!isFind)
                                {
                                    sb.Append(" <td scope='col'></td>");
                                    arrColorValSource.Add("");
                                }
                            }


                            sb.Append("</tr>");


                            #region 比對結果

                            isCompare = false;

                            sb.Append("<tr  class='rowCompare'>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='StandardPlacement' >" + (isCompare ? Compare(sType, standardPlacement, standardPlacement_compare, standardPlacement_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='usage' >" + (isCompare ? Compare(sType, usage, usage_compare, usage_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='SupplierArticle' >" + (isCompare ? Compare(sType, supplierArticle, supplierArticle_compare, supplierArticle_compare_note) : "") + "</td>");

                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='QualityDetails' >" + (isCompare ? Compare(sType, QualityDetails, QualityDetails_compare, QualityDetails_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='Supplier' >" + (isCompare ? Compare(sType, supplier, supplier_compare, supplier_compare_note) : "") + "</td>");

                            isCompare = true;

                            int iIndexSource = 0;
                            foreach (var color in arrGAP_BOMGarmentcolors)
                            {
                                string colorValSource = arrColorValSource[iIndexSource];
                                string colorValCompare = "";
                                string colorNoteValCompare = "";
                                int iIndexCompare = 0;

                                bool isFind = false;
                                //for (int i = 1; i <= 10; i++)
                                //{
                                //    string colorNameCompare = drComareBoms[b]["A" + i].ToString();
                                //    if (color == colorNameCompare)
                                //    {
                                //        iIndexCompare = i;
                                //        colorValCompare = drComareBoms[b]["B" + i].ToString();
                                //        colorNoteValCompare = FilterNote(arrNotesCompare, lubid_compare, "B" + i);
                                //        isFind = true;
                                //        break;
                                //    }
                                //}

                                if (arrColorNameSource.Contains(color))
                                {
                                    standardPlacement = standardPlacement.Replace(" ", string.Empty);
                                    placement = placement.Replace(" ", string.Empty);
                                    usage = usage.Replace(" ", string.Empty);
                                    supplierArticle = supplierArticle.Replace(" ", string.Empty);

                                    //     var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                    //&& x.Placement == placement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    //20220524
                                    //var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    //20220719
                                    //var resCompare = arrCompareData.FirstOrDefault(x => x.Usage == usage && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    //20230130
                                    var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement 
                                    && x.Usage == usage && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    if (resCompare != null)
                                    {
                                        isFind = true;
                                        colorValCompare = resCompare.ColorVal;
                                        resCompare.isExistA = true;
                                    }

                                    if (isFind)
                                    {


                                        sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' onclick='editBom(this,1)' data-col='" + resCompare.ColorBName + "' compare>" + (isCompare ? Compare(sType, colorValSource, colorValCompare, colorNoteValCompare) : "") + "</td>");
                                    }
                                    else
                                    {
                                        sb.Append(" <td scope='col'>X</td>");
                                    }
                                }
                                else
                                {
                                    sb.Append(" <td scope='col'></td>");
                                }






                                iIndexSource++;
                            }

                            sb.Append("</tr>");

                            #endregion

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Response.Write("<!--" + ex.ToString() + "-->");
                        }

                    }


                    sb.Append("</table>");

                    #endregion

                    #region 沒比對到的
                    var isNotComare = arrCompareData.Any(w => !w.isExistA);
                    StringBuilder sbNotExitColor = new StringBuilder();
                    Response.Write("<!--NotComare Count=" + arrCompareData.Where(w => !w.isExistA).Count() + "-->");

                    if (isNotComare && !arrNotCompareExistTypes.Contains(itemType.type))
                    {
                        arrNotCompareExistTypes.Add(itemType.type);

                        sbNotExit.Append("<h4>" + itemType.type + "</h4>");
                        sbNotExitColor.Append("<table class='table table-hover'>");
                        sbNotExitColor.Append("<tr>");
                        //sbNotExit.Append(" <th scope='col'>Standard Placement</th>");
                        //sbNotExit.Append(" <th scope='col'>Placement</th>");
                        //sbNotExit.Append(" <th scope='col'>Supplier / Supplier Article</th>");

                        sbNotExitColor.Append(" <th scope='col'>Product</th>");
                        sbNotExitColor.Append(" <th scope='col'>Usage</th>");
                        sbNotExitColor.Append(" <th scope='col'>Supplier Article Number</th>");
                        sbNotExitColor.Append(" <th scope='col'>Quality Details</th>");
                        sbNotExitColor.Append(" <th scope='col'>Supplier[Allocate]</th>");

                        //foreach (var color in arrGAP_BOMGarmentcolors)
                        //{
                        //    var isExistCompare = arrGAP_BOMGarmentcolorsCompare.Any(x => x == color);

                        //    if (!isExistCompare)
                        //        sbNotExit.Append(" <th scope='col' style='background-color:red'>" + color + "</th>");
                        //    //else
                        //    //    sbNotExit.Append(" <th scope='col' style='background-color:#00FFFF'>" + color + "</th>");
                        //}

                        //20220718 顯示不存在來源的
                        bool isHasDifferent = false;
                        foreach (var color in arrGAP_BOMGarmentcolorsCompare)
                        {
                            var isExistSource = arrGAP_BOMGarmentcolorsSource.Any(x => x == color);

                            Response.Write("<!--color=" + color + " =" + isExistSource + "-->");

                            if (!isExistSource)
                            {
                                sbNotExitColor.Append(" <th scope='col' style='background-color:red'>" + color + "</th>");
                                isHasDifferent = true;
                            }
                        }

                        sbNotExitColor.Append("</tr>");

                        //var arrRowIds = arrCompareData.Where(w => !w.isExistA).Select(s => s.rowId).Distinct().OrderBy(o => o).ToList();
                        var arrProducts = arrCompareData.Where(w => !w.isExistA).Select(s => s.StandardPlacement_org).Distinct().OrderBy(o => o).ToList();
                        var arrNotExistProducts = new List<string>();
                        //foreach (var rowId in arrRowIds)
                        foreach (var product in arrProducts)
                        {
                            //var resItem = arrCompareData.FirstOrDefault(x => x.rowId == rowId);
                            var arrOrgItem = dt.Select("StandardPlacement='" + product + "'");

                            if (arrOrgItem.Length == 0)
                            {
                                arrNotExistProducts.Add(product);
                                continue;
                            }


                            var arrItem = arrCompareData.Where(x => x.StandardPlacement_org == product).ToList();
                            var resItem = arrItem.First();
                            var arrItemRowIds = arrItem.Select(s => s.rowId).ToList();

                            sbNotExitColor.Append("<tr>");
                            sbNotExitColor.Append(" <td scope='col'>" + resItem.StandardPlacement_org + "</td>");
                            sbNotExitColor.Append(" <td scope='col'>" + resItem.Usage_org + "</td>");
                            sbNotExitColor.Append(" <td scope='col'>" + resItem.SupplierArticle_org + "</td>");
                            sbNotExitColor.Append(" <td scope='col'>" + resItem.QualityDetails_org + "</td>");
                            sbNotExitColor.Append(" <td scope='col'>" + resItem.Supplier_org + "</td>");


                            foreach (var color in arrGAP_BOMGarmentcolorsCompare)
                            {
                                //var isExistCompare = arrGAP_BOMGarmentcolorsCompare.Any(x => x == color);
                                var isExistSource = arrGAP_BOMGarmentcolorsSource.Any(x => x == color);

                                if (!isExistSource)
                                {
                                    //sbNotExit.Append(" <td scope='col'>X</td>");
                                    //20230210 帶出目的資料
                                    //var resColor = arrCompareData.FirstOrDefault(x => x.rowId == rowId && x.ColorName == color);
                                    var resColor = arrCompareData.FirstOrDefault(x => arrItemRowIds.Contains(x.rowId) && x.ColorName == color);
                                    if (resColor == null)
                                        sbNotExitColor.Append(" <td scope='col'>X</td>");
                                    else
                                        sbNotExitColor.Append(" <td scope='col'>" + resColor.ColorVal + "</td>");
                                }
                            }
                            sbNotExitColor.Append("</tr>");
                        }
                        
                        sbNotExitColor.Append("</table>");

                        if (!isHasDifferent) sbNotExitColor.Length = 0;
                        sbNotExit.Append(sbNotExitColor);

                        //目的沒比對的Product
                        StringBuilder sbNotExitProduct = new StringBuilder();
                        if (arrNotExistProducts.Count > 0)
                        {
                            foreach (var product in arrNotExistProducts)
                            {
                                sbNotExitProduct.Append("<table class='table table-hover'>");
                                sbNotExitProduct.Append("<tr>");
                                sbNotExitProduct.Append(" <th scope='col'>Product</th>");
                                sbNotExitProduct.Append(" <th scope='col'>Usage</th>");
                                sbNotExitProduct.Append(" <th scope='col'>Supplier Article Number</th>");
                                sbNotExitProduct.Append(" <th scope='col'>Quality Details</th>");
                                sbNotExitProduct.Append(" <th scope='col'>Supplier[Allocate]</th>");

                                arrGAP_BOMGarmentcolorsCompare = arrCompareData.Where(w => !w.isExistA && w.StandardPlacement_org == product).Select(s => s.ColorName).Distinct().ToList();

                                foreach (var color in arrGAP_BOMGarmentcolorsCompare)
                                {
                                    sbNotExitProduct.Append(" <th scope='col' style='background-color:red'>" + color + "</th>");
                                }

                                sbNotExitProduct.Append("</tr>");

                                var arrItem = arrCompareData.Where(x => !x.isExistA && x.StandardPlacement_org == product).ToList();
                                var resItem = arrItem.First();
                                var arrItemRowIds = arrItem.Select(s => s.rowId).ToList();

                                sbNotExitProduct.Append("<tr>");
                                sbNotExitProduct.Append(" <td scope='col'>" + resItem.StandardPlacement_org + "</td>");
                                sbNotExitProduct.Append(" <td scope='col'>" + resItem.Usage_org + "</td>");
                                sbNotExitProduct.Append(" <td scope='col'>" + resItem.SupplierArticle_org + "</td>");
                                sbNotExitProduct.Append(" <td scope='col'>" + resItem.QualityDetails_org + "</td>");
                                sbNotExitProduct.Append(" <td scope='col'>" + resItem.Supplier_org + "</td>");


                                foreach (var color in arrGAP_BOMGarmentcolorsCompare)
                                {
                                    var resColor = arrCompareData.FirstOrDefault(x => !x.isExistA && x.StandardPlacement_org == product && x.ColorName == color);
                                    if (resColor == null)
                                        sbNotExitProduct.Append(" <td scope='col'>X</td>");
                                    else
                                        sbNotExitProduct.Append(" <td scope='col'>" + resColor.ColorVal + "</td>");
                                }
                                sbNotExitProduct.Append("</tr>");
                                sbNotExitProduct.Append("</table>");
                            }
                        }
                        sbNotExit.Append(sbNotExitProduct);
                        if (!isHasDifferent && sbNotExitProduct.Length == 0)
                            sbNotExit.Length = 0;
                    }
                    //divHeaderNotCompare.InnerHtml = sbNotExit.ToString();
                    //Response.Write("<!--sbNotExit =" + sbNotExit.ToString() + "-->");
                    #endregion
                }



                #endregion

                #region GAP_SizeTable

                List<string> listSourceSizeTableHeader = new List<string>();       //來源的尺寸
                List<string> listCompareSizeTableHeader = new List<string>();    //目的的尺寸

                sSql = @"select a.*,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15
                    from PDFTAG.dbo.GAP_SizeTable a
                    join  PDFTAG.dbo.GAP_SizeTable_Header b on a.lusthid=b.lusthid
                    where 1=1
                    and a.pipid =@pipid
                    --and a.lusthid in ('" + hid_Source_lusthid.Value.Replace(",", "','") + @"')--直接全抓
                    order by lusthid,rowid asc ";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                //cm.Parameters.AddWithValue("@luhid", luhid);
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                var arr_lusthids = dt.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().OrderBy(o => o).ToList();

                Dictionary<string, List<string>> dicHeardersSource = new Dictionary<string, List<string>>();

                for (int h = 0; h < arr_lusthids.Count; h++)
                {
                    string lusthid = arr_lusthids[h].ToString();
                    List<string> tmpSize = new List<string>();

                    sSql = @"select *
                        from PDFTAG.dbo.GAP_SizeTable_Header a
                        where a.lusthid in ('" + lusthid + "') ";
                    cm.CommandText = sSql;
                    DataTable dtUA_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtUA_SizeTable_Header);
                    }
                    for (int i = 1; i <= 15; i++)
                    {
                        string sH = dtUA_SizeTable_Header.Rows[0]["H" + i].ToString();
                        if (string.IsNullOrEmpty(sH)) { dicHeardersSource.Add(lusthid, tmpSize); break; }
                        if (!tmpSize.Contains(sH)) { tmpSize.Add(sH); listSourceSizeTableHeader.Add(sH); }
                    }
                }


                sSql = @"select a.*,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15 
                    from PDFTAG.dbo.GAP_SizeTable a
                    join  PDFTAG.dbo.GAP_SizeTable_Header b on a.lusthid=b.lusthid
                    where 1=1
                    and a.pipid =@pipid
                    --and a.lusthid in ('" + hid_Compare_lusthid.Value.Replace(",", "','") + @"')
                    order by lusthid,rowid asc ";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                //cm.Parameters.AddWithValue("@luhid", compare_luhid);
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                dtCompare = new DataTable();
                DataTable getSizeTable = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                    //複製一份Compare的，比對時找code的第一筆然後刪掉
                    da.Fill(getSizeTable);
                }

                var arr_Comparelusthids = dtCompare.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().OrderBy(o => o).ToList();
                Response.Write("<!--luhid=" + luhid + ";compare_luhid=" + compare_luhid + "-->");

                Dictionary<string, List<string>> dicHeardersCompare = new Dictionary<string, List<string>>();

                for (int h = 0; h < arr_Comparelusthids.Count; h++)
                {
                    string lusthidCompare = arr_Comparelusthids[h].ToString();
                    List<string> tmpSize = new List<string>();

                    sSql = @"select *
                        from PDFTAG.dbo.GAP_SizeTable_Header a
                        where a.lusthid in ('" + lusthidCompare + "') ";
                    cm.CommandText = sSql;
                    DataTable dtUA_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtUA_SizeTable_Header);
                    }
                    for (int i = 1; i <= 15; i++)
                    {
                        string sH = dtUA_SizeTable_Header.Rows[0]["H" + i].ToString();
                        if (string.IsNullOrEmpty(sH)) { dicHeardersCompare.Add(lusthidCompare, tmpSize); break; }
                        if (!tmpSize.Contains(sH)) { tmpSize.Add(sH); listCompareSizeTableHeader.Add(sH); }
                    }
                }

                arrNotes = new List<GAP_Ch_Note>();
                arrNotesCompare = new List<GAP_Ch_Note>();
                //if (sType == "2")
                {
                    #region 中文備註

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                    sSql += "where 1=1 \n";
                    sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.GAP_SizeTable where pipid = @pipid ) \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    //cm.Parameters.AddWithValue("@luhid", luhid);
                    cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                    DataTable dt2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt2);
                    }

                    arrNotes = dt2.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    //cm.Parameters.AddWithValue("@luhid", compare_luhid);
                    cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                    DataTable dtCompare2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtCompare2);
                    }

                    arrNotesCompare = dtCompare2.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    #endregion
                }

                #region 比對
                for (int h = 0; h < arr_lusthids.Count; h++)
                {
                    List<string> curSourceSize = new List<string>();
                    List<string> tmpCompareSize = new List<string>();
                    List<string> listComparelusthids = new List<string>();
                    bool isMatch = false;
                    if (dicHeardersSource.ContainsKey(arr_lusthids[h].ToString()))
                    {
                        string lusthid = arr_lusthids[h].ToString();
                        curSourceSize = dicHeardersSource[arr_lusthids[h].ToString()];
                        foreach (string compareKey in dicHeardersCompare.Keys)
                        {
                            //找有沒有相同的尺寸
                            if (dicHeardersCompare[compareKey].SequenceEqual(curSourceSize))
                            {
                                listComparelusthids.Add(compareKey);
                                isMatch = true;
                            }
                        }
                        //這邊找多的或是少的，以完全包含出發
                        if (!isMatch)
                        {
                            foreach (string compareKey in dicHeardersCompare.Keys)
                            {
                                if (curSourceSize.All(x => dicHeardersCompare[compareKey].Any(y => y == x)) || dicHeardersCompare[compareKey].All(x => curSourceSize.Any(y => y == x)))
                                {
                                    listComparelusthids.Add(compareKey);
                                    foreach (string tmpSize in dicHeardersCompare[compareKey])
                                    {
                                        if (!tmpCompareSize.Contains(tmpSize))
                                            tmpCompareSize.Add(tmpSize);
                                    }
                                }
                            }
                        }
                        //剩下是同時有少跟有多
                        if (listComparelusthids == null)
                        { }

                        DataTable dtUA_SizeTableCompare_Header = new DataTable();
                        string lusthid_compare = string.Join("','", listComparelusthids);

                        sSql = @"select * 
                            from PDFTAG.dbo.GAP_SizeTable_Header a
                            where a.lusthid in ('" + lusthid_compare + "') ";
                        Response.Write("<!--" + sSql + "-->");
                        cm.CommandText = sSql;

                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                        {
                            da.Fill(dtUA_SizeTableCompare_Header);
                        }

                        if (h == 0)
                        {
                            sb.Append("<h4>Size Table</h4>");
                        }
                        sb.Append("<table class='table table-hover'>");
                        sb.Append("<tr>");
                        sb.Append(" <th scope='col'>POM</th>");
                        sb.Append(" <th scope='col'>Description</th>");
                        //sb.Append(" <th scope='col'>AddlComments</th>");
                        sb.Append(" <th scope='col'>Tol(-)</th>");
                        sb.Append(" <th scope='col'>Tol(+)</th>");
                        sb.Append(" <th scope='col'>Variation</th>");

                        foreach (var size in curSourceSize)
                        {
                            if (isMatch)
                            {
                                sb.Append(" <th scope='col' style='background-color:#00FFFF'>" + size + "</th>");
                            }
                            else
                            {
                                var isExistCompare = tmpCompareSize.Any(x => x == size);
                                if (!isExistCompare)
                                    sb.Append(" <th scope='col' style='background-color:#FF9224'>" + size + "</th>");
                                else
                                    sb.Append(" <th scope='col' style='background-color:#00FFFF'>" + size + "</th>");
                            }
                        }

                        sb.Append("</tr>");

                        DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
                        //DataRow[] drCompareSizeTables = dtCompare.Select("lusthid in ('" + lusthid_compare + "')", "rowid asc");

                        for (int s = 0; s < drSizeTables.Length; s++)
                        {
                            //找compare code符合的第一筆
                            var tmpSizetable = getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("POM").ToString().Replace(" ", "") == drSizeTables[s]["POM"].ToString().Replace(" ", ""));
                            //var tmpSizetable = getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("Code").ToString().Replace(" ", "") == drSizeTables[s]["Code"].ToString().Replace(" ", "")
                            //    && (x.Field<string>("H1") == drSizeTables[s]["H1"].ToString() || x.Field<string>("H1") == drSizeTables[s]["H2"].ToString()));

                            string lustid = "";
                            string org_lustid = "";

                            string code_source = "";
                            string description_source = "";
                            string AddlComments_source = "";
                            string tolA_source = "";
                            string tolB_source = "";
                            string Variation_source = "";

                            string code = "";
                            string description = "";
                            string AddlComments = "";
                            string tolA = "";
                            string tolB = "";
                            string Variation = "";

                            string code_note = "";
                            string description_note = "";
                            string AddlComments_note = "";
                            string tolA_note = "";
                            string tolB_note = "";
                            string Variation_note = "";

                            string lustid_compare = "";
                            string lustid_org_compare = "";
                            string code_compare = "";
                            string description_compare = "";
                            string AddlComments_compare = "";
                            string tolA_compare = "";
                            string tolB_compare = "";
                            string Variation_compare = "";

                            string code_compare_note = "";
                            string description_compare_note = "";
                            string AddlComments_compare_note = "";
                            string tolA_compare_note = "";
                            string tolB_compare_note = "";
                            string Variation_compare_note = "";

                            try
                            {
                                lustid = drSizeTables[s]["lustid"].ToString();
                                org_lustid = drSizeTables[s]["org_lustid"].ToString();

                                code_source = drSizeTables[s]["POM"].ToString();
                                description_source = drSizeTables[s]["Description"].ToString();
                                AddlComments_source = drSizeTables[s]["AddlComments"].ToString();
                                tolA_source = drSizeTables[s]["TolA"].ToString();
                                tolB_source = drSizeTables[s]["TolB"].ToString();
                                Variation_source = drSizeTables[s]["Variation"].ToString();

                                code = drSizeTables[s]["POM"].ToString();
                                description = drSizeTables[s]["Description"].ToString();
                                AddlComments = drSizeTables[s]["AddlComments"].ToString();
                                tolA = drSizeTables[s]["TolA"].ToString();
                                tolB = drSizeTables[s]["TolB"].ToString();
                                Variation = drSizeTables[s]["Variation"].ToString();

                                code_note = FilterNote(arrNotes, lustid, "POM");
                                description_note = FilterNote(arrNotes, lustid, "Description");
                                AddlComments_note = FilterNote(arrNotes, lustid, "AddlComments");
                                tolA_note = FilterNote(arrNotes, lustid, "tolA");
                                tolB_note = FilterNote(arrNotes, lustid, "tolB");
                                Variation_note = FilterNote(arrNotes, lustid, "Variation");
                            }
                            catch (Exception ex) { }

                            try
                            {
                                lustid_compare = tmpSizetable["lustid"].ToString();
                                lustid_org_compare = tmpSizetable["org_lustid"].ToString();
                                code_compare = tmpSizetable["POM"].ToString();
                                description_compare = tmpSizetable["Description"].ToString();
                                AddlComments_compare = tmpSizetable["AddlComments"].ToString();
                                tolA_compare = tmpSizetable["TolA"].ToString();
                                tolB_compare = tmpSizetable["TolB"].ToString();
                                Variation_compare = tmpSizetable["Variation"].ToString();

                                code_compare_note = FilterNote(arrNotesCompare, lustid_compare, "POM");
                                description_compare_note = FilterNote(arrNotesCompare, lustid_compare, "Description");
                                AddlComments_compare_note = FilterNote(arrNotesCompare, lustid_compare, "AddlComments");
                                tolA_compare_note = FilterNote(arrNotesCompare, lustid_compare, "tolA");
                                tolB_compare_note = FilterNote(arrNotesCompare, lustid_compare, "tolB");
                                Variation_compare_note = FilterNote(arrNotesCompare, lustid_compare, "Variation");
                            }
                            catch (Exception ex) { }

                            try
                            {
                                #region 

                                if (sType == "2")
                                {
                                    code = code_note;
                                    description = description_note;
                                    AddlComments = AddlComments_note;
                                    tolA = tolA_note;
                                    tolB = tolB_note;
                                    Variation = Variation_note;

                                    code_compare = code_compare_note;
                                    description_compare = description_compare_note;
                                    AddlComments_compare = AddlComments_compare_note;
                                    tolA_compare = tolA_compare_note;
                                    tolB_compare = tolB_compare_note;
                                    Variation_compare = Variation_compare_note;
                                }

                                sb.Append("<tr data-rowid='" + drSizeTables[s]["rowid"].ToString() + "'>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='POM' >" + code_source + (string.IsNullOrEmpty(code_note) ? "" : "<br>中:" + code_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' >" + description_source + (string.IsNullOrEmpty(description_note) ? "" : "<br>中:" + description_note) + "</td>");
                                //sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='AddlComments' onclick='editSizeTable(this)'>" + AddlComments_source + (string.IsNullOrEmpty(AddlComments_note) ? "" : "<br>中:" + AddlComments_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' >" + tolA_source + (string.IsNullOrEmpty(tolA_note) ? "" : "<br>中:" + tolA_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' >" + tolB_source + (string.IsNullOrEmpty(tolB_note) ? "" : "<br>中:" + tolB_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Variation' onclick='editSizeTable(this)'>" + Variation_source + (string.IsNullOrEmpty(Variation_note) ? "" : "<br>中:" + Variation_note) + "</td>");

                                foreach (var header in curSourceSize)
                                {
                                    bool isFind = false;
                                    for (int i = 1; i <= 15; i++)
                                    {
                                        string otherHeader = drSizeTables[s]["H" + i].ToString();
                                        if (header == otherHeader)
                                        {
                                            string otherValue = drSizeTables[s]["A" + i].ToString();
                                            double d = 0;
                                            if (otherValue.Contains("/") && Double.TryParse(otherValue.Split('/')[1], out d)) otherValue = otherValue.Replace(" ", "-");
                                            sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='A" + i + "' >" + otherValue + "</td>");
                                            isFind = true;
                                            break;
                                        }
                                    }
                                    if (!isFind)
                                        sb.Append(" <td scope='col'></td>");
                                }
                                sb.Append("</tr>");

                                sb.Append("<tr  class='rowCompare'>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='Code' >" + (string.IsNullOrEmpty(code_compare) ? "" : Compare(sType, code, code_compare, code_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='Description' >" + (string.IsNullOrEmpty(description_compare) ? "" : Compare(sType, description, description_compare, description_compare_note)) + "</td>");
                                //sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' onclick='editSizeTable(this,1)' data-col='AddlComments' >" + Compare(sType, AddlComments, AddlComments_compare, AddlComments_compare_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='TolA' >" + (string.IsNullOrEmpty(tolA_compare) ? "" : Compare(sType, tolA, tolA_compare, tolA_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='TolB' >" + (string.IsNullOrEmpty(tolB_compare) ? "" : Compare(sType, tolB, tolB_compare, tolB_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' onclick='editSizeTable(this,1)' data-col='Variation' >" + Compare(sType, Variation, Variation_compare, Variation_compare_note) + "</td>");

                                foreach (var header in curSourceSize)
                                {
                                    bool isFind = false;
                                    for (int i = 1; i <= 15; i++)
                                    {
                                        string otherHeader = "";
                                        string otherValue = "";
                                        string otherNote = "";
                                        string otherHeaderCompare = "";
                                        string otherValueCompare = "";
                                        string otherNoteCompare = "";

                                        try
                                        {
                                            otherHeader = drSizeTables[s]["H" + i].ToString();
                                            otherValue = drSizeTables[s]["A" + i].ToString();
                                            otherNote = FilterNote(arrNotes, lustid, "A" + i);
                                        }
                                        catch (Exception) { }
                                        try
                                        {
                                            int tmpCompare = 0;
                                            for (int tmp = 1; tmp <= 15; tmp++)
                                            {
                                                otherHeaderCompare = tmpSizetable["H" + tmp].ToString();
                                                if (otherHeaderCompare == otherHeader)
                                                {
                                                    tmpCompare = tmp;
                                                    break;
                                                }
                                            }
                                            otherHeaderCompare = tmpSizetable["H" + tmpCompare].ToString();
                                            otherValueCompare = tmpSizetable["A" + tmpCompare].ToString();
                                            otherNoteCompare = FilterNote(arrNotesCompare, lustid_compare, "A" + tmpCompare);
                                        }
                                        catch (Exception) { }

                                        if (header == otherHeaderCompare)
                                        {
                                            double d = 0;

                                            if (otherValue.Contains("/") && Double.TryParse(otherValue.Split('/')[1], out d)) otherValue = otherValue.Replace(" ", "-");
                                            if (otherValueCompare.Contains("/") && Double.TryParse(otherValueCompare.Split('/')[1], out d)) otherValueCompare = otherValueCompare.Replace(" ", "-");

                                            string other = Compare(sType, otherValue, otherValueCompare, otherNoteCompare);

                                            sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='A" + i + "' >" + other + "</td>");

                                            isFind = true;
                                            break;
                                        }

                                    }
                                    if (!isFind)
                                        sb.Append(" <td scope='col'>X</td>");
                                    else
                                    {

                                    }
                                }

                                sb.Append("</tr>");

                                #endregion
                            }
                            catch (Exception ex)
                            {
                                Response.Write("<!--" + ex.ToString() + "-->");
                            }
                            //如果不存在就表示是來源多的
                            if (tmpSizetable == null) { continue; }
                            //如果有找到就刪掉，剩下的就是目的文件多的
                            if (!string.IsNullOrEmpty(code_compare))
                                getSizeTable.Rows.Remove(getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("POM").ToString().Replace(" ", "") == drSizeTables[s]["POM"].ToString().Replace(" ", "")));
                            //getSizeTable.Rows.Remove(getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("code").ToString().Replace(" ", "") == drSizeTables[s]["code"].ToString().Replace(" ", "")
                            //        && x.Field<string>("H1") == drSizeTables[s]["H1"].ToString()));
                        }
                        sb.Append("</table>");
                    }
                }
                #endregion
                #region 沒比對到的
                #region 尺寸
                StringBuilder sbNotExitSizeHeader = new StringBuilder();
                StringBuilder sbNotExitSizeTableHeader = new StringBuilder();
                StringBuilder sbNotExitSizeTableBody = new StringBuilder();
                bool initFlag = true;
                for (int h = 0; h < arr_Comparelusthids.Count; h++)
                {
                    List<string> curCompareSize = new List<string>();
                    List<string> tmpSize = new List<string>();
                    List<string> listSourcelusthids = new List<string>();
                    bool isMatch = false;
                    if (dicHeardersCompare.ContainsKey(arr_Comparelusthids[h].ToString()))
                    {
                        string lusthidCompare = arr_Comparelusthids[h].ToString();
                        curCompareSize = dicHeardersCompare[arr_Comparelusthids[h].ToString()];
                        foreach (string compareKey in dicHeardersSource.Keys)
                        {
                            //找有沒有相同的尺寸
                            if (dicHeardersSource[compareKey].SequenceEqual(curCompareSize))
                            {
                                listSourcelusthids.Add(compareKey);
                                isMatch = true;
                            }
                        }
                        //這邊找多的尺寸
                        if (!isMatch)
                        {
                            foreach (string SourceKey in dicHeardersSource.Keys)
                            {
                                //if (curCompareSize.All(x => dicHeardersSource[compareKey].Any(y => y == x)) || dicHeardersSource[compareKey].All(x => curCompareSize.Any(y => y == x)))
                                if (dicHeardersSource[SourceKey].All(x => curCompareSize.Any(y => y == x)))
                                {
                                    listSourcelusthids.Add(SourceKey);
                                    foreach (string Size in curCompareSize)
                                    {
                                        if (!dicHeardersSource[SourceKey].Contains(Size) && !tmpSize.Contains(Size))
                                            tmpSize.Add(Size);
                                    }
                                }
                            }
                        }
                        //剩下是同時有少跟有多
                        if (listSourcelusthids == null)
                        { }

                        //這邊是沒比對到的，所以沒MATCH且目的文件多的才繼續
                        if (!isMatch && tmpSize.Count > 0)
                        {
                            DataTable dtUA_SizeTableSource_Header = new DataTable();
                            string lusthid_compare = string.Join("','", listSourcelusthids);

                            sSql = @"select * 
                            from PDFTAG.dbo.UA_SizeTable_Header a
                            where a.lusthid in ('" + lusthid_compare + "') ";
                            Response.Write("<!--" + sSql + "-->");
                            cm.CommandText = sSql;

                            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                            {
                                da.Fill(dtUA_SizeTableSource_Header);
                            }

                            StringBuilder sbNotExitSizeTmpTable = new StringBuilder();
                            if (initFlag)
                            {
                                sbNotExitSizeHeader.Append("<h4>尺寸</h4>");
                                sbNotExitSizeHeader.Append("<h4>Size Table</h4>");
                                initFlag = false;
                            }
                            bool sizeTableFlag = false;
                            sbNotExitSizeTmpTable.Append("<table class='table table-hover'>");
                            sbNotExitSizeTmpTable.Append("<tr>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>POM</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Description</th>");
                            //sbNotExitSizeTmpTable.Append(" <th scope='col'>AddlComments</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Tol(-)</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Tol(+)</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Variation</th>");

                            foreach (var size in tmpSize)
                            {
                                sbNotExitSizeTmpTable.Append(" <th scope='col' style='background-color:red'>" + size + "</th>");
                            }

                            sbNotExitSizeTmpTable.Append("</tr>");

                            //DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
                            DataRow[] drCompareSizeTables = dtCompare.Select("lusthid='" + lusthidCompare + "'", "rowid asc");

                            for (int s = 0; s < drCompareSizeTables.Length; s++)
                            {
                                string lustid = "";
                                string org_lustid = "";

                                string POM = "";
                                string description = "";
                                string tolA = "";
                                string tolB = "";
                                string AddlComments = "";
                                string Variation = "";

                                POM = drCompareSizeTables[s]["POM"].ToString();
                                description = drCompareSizeTables[s]["Description"].ToString();
                                AddlComments = drCompareSizeTables[s]["AddlComments"].ToString();
                                tolA = drCompareSizeTables[s]["TolA"].ToString();
                                tolB = drCompareSizeTables[s]["TolB"].ToString();
                                Variation = drCompareSizeTables[s]["Variation"].ToString();

                                sbNotExitSizeTableBody.Append("<tr data-rowid='" + drCompareSizeTables[s]["rowid"].ToString() + "'>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='POM' >" + POM + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' >" + description + "</td>");
                                //sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='AddlComments' >" + AddlComments + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' >" + tolA + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' >" + tolB + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Variation' >" + Variation + "</td>");

                                bool bodyFlag = false;
                                for (int i = 1; i <= 15; i++)
                                {
                                    string compareHeader = drCompareSizeTables[s]["H" + i].ToString();
                                    if (string.IsNullOrEmpty(compareHeader)) { break; }
                                    else if (tmpSize.Contains(compareHeader))
                                    {
                                        sizeTableFlag = true;
                                        bodyFlag = true;
                                        sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='A" + i + "' >" + drCompareSizeTables[s]["A" + i].ToString() + "</td>");
                                    }
                                }
                                sbNotExitSizeTableBody.Append("</tr>");
                                if (!bodyFlag) { sbNotExitSizeTableBody.Clear(); }
                                sbNotExitSizeTmpTable.Append(sbNotExitSizeTableBody.ToString());
                                sbNotExitSizeTableBody.Clear();
                            }
                            sbNotExitSizeTmpTable.Append("</table>");
                            if (!sizeTableFlag) { sbNotExitSizeTmpTable.Clear(); }
                            sbNotExitSizeTableHeader.Append(sbNotExitSizeTmpTable.ToString());
                            sbNotExitSizeTmpTable.Clear();
                        }
                    }
                }
                if (sbNotExitSizeTableHeader.Length == 0) sbNotExitSizeHeader.Clear();
                sbNotExit.Append(sbNotExitSizeHeader);
                sbNotExit.Append(sbNotExitSizeTableHeader);
                #endregion
                #region 資料列
                //如果有代表有沒比對到的，沒有就不跑
                initFlag = true;

                Response.Write("<!--沒比對到的 資料列SizeTable=" + getSizeTable.Rows.Count + "-->");

                if (getSizeTable.Rows.Count > 0)
                {
                    List<List<string>> getHeader = new List<List<string>>();

                    foreach (DataRow getdata in getSizeTable.Rows)
                    {
                        List<string> sizeHeader = new List<string>();

                        for (int i = 1; i <= 15; i++)
                        {
                            string sH = getdata["H" + i].ToString();
                            if (!string.IsNullOrEmpty(sH))
                            {
                                sizeHeader.Add(sH);
                            }
                        }
                        if (!(getHeader.Any(x => x.SequenceEqual(sizeHeader))))
                        {
                            if (initFlag)
                            {
                                sbNotExit.Append("<h4>資料列</h4>");
                                sbNotExit.Append("<h4>Size Table</h4>");
                                initFlag = false;
                            }
                            else if (!initFlag) sbNotExit.Append("</table>");
                            sbNotExit.Append("<table class='table table-hover'>");
                            sbNotExit.Append("<tr>");
                            sbNotExit.Append(" <th scope='col'>POM</th>");
                            sbNotExit.Append(" <th scope='col'>Description</th>");
                            //sbNotExit.Append(" <th scope='col'>AddlComments</th>");
                            sbNotExit.Append(" <th scope='col'>Tol(-)</th>");
                            sbNotExit.Append(" <th scope='col'>Tol(+)</th>");
                            sbNotExit.Append(" <th scope='col'>Variation</th>");

                            for (int i = 1; i <= 15; i++)
                            {
                                string sH = getdata["H" + i].ToString();
                                if (!string.IsNullOrEmpty(sH))
                                {
                                    if (listSourceSizeTableHeader.Any(x => x == sH))
                                    {
                                        sbNotExit.Append("  <th scope='col'>" + sH + "</th>");
                                    }
                                    else
                                    {
                                        sbNotExit.Append("  <th scope='col' style='background-color:red'>" + sH + "</th>");
                                    }
                                }
                            }
                            getHeader.Add(sizeHeader);
                        }

                        sbNotExit.Append("<tr data-rowid='" + getdata["rowid"].ToString() + "'>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='POM' >" + getdata["POM"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='Description' >" + getdata["description"].ToString() + "</td>");
                        //sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='AddlComments' >" + getdata["AddlComments"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='TolA' >" + getdata["TolA"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='TolB' >" + getdata["TolB"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='Variation' >" + getdata["Variation"].ToString() + "</td>");
                        for (int i = 1; i <= 15; i++)
                        {
                            //if ((string.IsNullOrEmpty(getdata["A" + i].ToString()) || getdata["A" + i].ToString() == "----") && getdata["H" + i].ToString().Replace(" ", "") != "Comments") break;
                            sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='A" + i + "' >" + getdata["A" + i].ToString() + "</td>");
                        }
                        sbNotExit.Append("</tr>");
                    }
                    if (getSizeTable.Rows.Count > 0) sbNotExit.Append("</table>");
                }
                #endregion
                #endregion

                #endregion

            }

            divHeader.InnerHtml = sb.ToString();
            divHeaderNotCompare.InnerHtml = sbNotExit.ToString();
            script = "onShow(1);";

        }
        catch (Exception err)
        {
            script = "alert('比對失敗!')";
            Response.Write("<!--比對失敗:" + err.ToString() + "-->");
            LogFile.Logger.LogException(err);
        }
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

    protected void btnQuery_Click(object sender, EventArgs e)
    {
        DataBind();
    }


}