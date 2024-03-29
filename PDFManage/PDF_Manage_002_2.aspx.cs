﻿using System;
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
               where a.isShow=0 and a.pver=1 and a.hdid >0
               order by hisversion asc";
            }
            else
            {
                sSql = @"
  select a.pipid,a.ptitle,a.piuploadfile,b.hisversion 
from PDFTAG.dbo.P_inProcess a
  join PDFTAG.dbo.HistoryData b on a.hdid=b.hdid and b.isshow=0 
where a.isShow=0 and a.pver=1 and a.pipid in (select pipid from PDFTAG.dbo.P_inProcess where isShow=0 and gmid='" + gmid + @"')
and a.pipid !='" + hidpipid.Value + @"'
order by hisversion asc";

                //顯示 所有群組內的文件
                sSql = @"
  select a.pipid,a.ptitle,a.piuploadfile,a.hdid
from PDFTAG.dbo.P_inProcess a
where a.isShow=0 and a.pver=1 and a.pipid in (select pipid from PDFTAG.dbo.P_inProcess where isShow=0 and gmid='" + gmid + @"')
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

    public class Lu_Ch_Note
    {
        public long Id { get; set; }
        public string ColName { get; set; }

        public string Note { get; set; }
    }
    public class Lu_Bom
    {
        public int rowId { get; set; }
        public string lubid { get; set; }
        public string org_lubid { get; set; }
        public string StandardPlacement { get; set; }

        public string Placement { get; set; }

        public string SupplierArticle { get; set; }

        public string ColorName { get; set; }

        public string ColorVal { get; set; }

        public string ColorBName { get; set; }


        public string StandardPlacement_org { get; set; }

        public string Placement_org { get; set; }

        public string SupplierArticle_org { get; set; }

        public string Supplier_org { get; set; }

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
    public string FilterNote(List<Lu_Ch_Note> arrData, string id, string colName)
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
            sSql += "from PDFTAG.dbo.Lu_Header a              \n";
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

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", sCompare_pipid);
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                }

                string compare_luhid = dtCompare.Rows[0]["luhid"].ToString();
                string compare_org_luhid = dtCompare.Rows[0]["org_luhid"].ToString();

                #region Lu_Header

                string season_soure = dt.Rows[0]["Season"].ToString();
                string style_soure = dt.Rows[0]["style"].ToString();
                string styledesc_soure = dt.Rows[0]["styledesc"].ToString();
                string productsize_soure = dt.Rows[0]["productsize"].ToString();
                string brand_soure = dt.Rows[0]["brand"].ToString();
                string dividion_soure = dt.Rows[0]["dividion"].ToString();
                string pod_soure = dt.Rows[0]["pod"].ToString();
                string stylestatus_soure = dt.Rows[0]["stylestatus"].ToString();
                string generateddate_soure = dt.Rows[0]["generateddate"].ToString();

                string season = dt.Rows[0]["Season"].ToString();
                string style = dt.Rows[0]["style"].ToString();
                string styledesc = dt.Rows[0]["styledesc"].ToString();
                string productsize = dt.Rows[0]["productsize"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();

                string season_compare = dtCompare.Rows[0]["Season"].ToString();
                string style_compare = dtCompare.Rows[0]["style"].ToString();
                string styledesc_compare = dtCompare.Rows[0]["styledesc"].ToString();
                string productsize_compare = dtCompare.Rows[0]["productsize"].ToString();
                string brand_compare = dtCompare.Rows[0]["brand"].ToString();
                string dividion_compare = dtCompare.Rows[0]["dividion"].ToString();
                string pod_compare = dtCompare.Rows[0]["pod"].ToString();
                string stylestatus_compare = dtCompare.Rows[0]["stylestatus"].ToString();
                string generateddate_compare = dtCompare.Rows[0]["generateddate"].ToString();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.Lu_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", sSource_pipid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                var arrData = dt.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                string season_note = FilterNote(arrData, luhid, "season");
                string style_note = FilterNote(arrData, luhid, "style");
                string styledesc_note = FilterNote(arrData, luhid, "styledesc");
                string productsize_note = FilterNote(arrData, luhid, "productsize");
                string brand_note = FilterNote(arrData, luhid, "brand");
                string dividion_note = FilterNote(arrData, luhid, "dividion");
                string pod_note = FilterNote(arrData, luhid, "pod");
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

                arrData = dtCompare.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                string season_compare_note = FilterNote(arrData, compare_luhid, "season");
                string style_compare_note = FilterNote(arrData, compare_luhid, "style");
                string styledesc_compare_note = FilterNote(arrData, compare_luhid, "styledesc");
                string productsize_compare_note = FilterNote(arrData, compare_luhid, "productsize");
                string brand_compare_note = FilterNote(arrData, compare_luhid, "brand");
                string dividion_compare_note = FilterNote(arrData, compare_luhid, "dividion");
                string pod_compare_note = FilterNote(arrData, compare_luhid, "pod");
                string stylestatus_compare_note = FilterNote(arrData, compare_luhid, "stylestatus");
                string generateddate_compare_note = FilterNote(arrData, compare_luhid, "generateddate");

                if (sType == "2")
                {
                    #region 中文備註


                    season = season_note;
                    style = style_note;
                    styledesc = styledesc_note;
                    productsize = productsize_note;
                    brand = brand_note;
                    dividion = dividion_note;
                    pod = pod_note;
                    stylestatus = stylestatus_note;
                    generateddate = generateddate_note;

                    cm.CommandText = sSql;

                    season_compare = season_compare_note;
                    style_compare = style_compare_note;
                    styledesc_compare = styledesc_compare_note;
                    productsize_compare = productsize_compare_note;
                    brand_compare = brand_compare_note;
                    dividion_compare = dividion_compare_note;
                    pod_compare = pod_compare_note;
                    stylestatus_compare = stylestatus_compare_note;
                    generateddate_compare = generateddate_compare_note;

                    #endregion
                }

                sb.Append("<table class='table table-hover'>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Season</th>");
                sb.Append(" <th scope='col'>Style</th>");
                sb.Append(" <th scope='col'>Style Desc</th>");
                sb.Append(" <th scope='col'>Product Size</th>");
                sb.Append(" <th scope='col'>Brand</th>");
                sb.Append(" <th scope='col'>Dividion</th>");
                sb.Append(" <th scope='col'>Pod</th>");
                sb.Append(" <th scope='col'>Style Status</th>");
                sb.Append(" <th scope='col'>Generated Date</th>");
                sb.Append("</tr>");
                sb.Append("<tr>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='Season'>" + season_soure + (string.IsNullOrEmpty(season_note) ? "" : "<br>中:" + season_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='style'>" + style_soure + (string.IsNullOrEmpty(style_note) ? "" : "<br>中:" + style_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='styledesc'>" + styledesc_soure + (string.IsNullOrEmpty(styledesc_note) ? "" : "<br>中:" + styledesc_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='productsize'>" + productsize_soure + (string.IsNullOrEmpty(productsize_note) ? "" : "<br>中:" + productsize_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='brand'>" + brand_soure + (string.IsNullOrEmpty(brand_note) ? "" : "<br>中:" + brand_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='dividion'>" + dividion_soure + (string.IsNullOrEmpty(dividion_note) ? "" : "<br>中:" + dividion_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='pod'>" + pod_soure + (string.IsNullOrEmpty(pod_note) ? "" : "<br>中:" + pod_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='stylestatus'>" + stylestatus_soure + (string.IsNullOrEmpty(stylestatus_note) ? "" : "<br>中:" + stylestatus_note) + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "' data-col='generateddate'>" + generateddate_soure + (string.IsNullOrEmpty(generateddate_note) ? "" : "<br>中:" + generateddate_note) + "</td>");
                sb.Append("</tr>");


                sb.Append("<tr class='rowCompare'>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='Season'>" + Compare(sType, season, season_compare, season_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='style'>" + Compare(sType, style, style_compare, style_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='styledesc'>" + Compare(sType, styledesc, styledesc_compare, styledesc_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='productsize'>" + Compare(sType, productsize, productsize_compare, productsize_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='brand'>" + Compare(sType, brand, brand_compare, brand_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='dividion'>" + Compare(sType, dividion, dividion_compare, dividion_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='pod'>" + Compare(sType, pod, pod_compare, pod_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='stylestatus'>" + Compare(sType, stylestatus, stylestatus_compare, stylestatus_compare_note) + "</td>");
                sb.Append(" <td scope='col' data-luhid='" + compare_luhid + "' data-org_luhid='" + compare_org_luhid + "' data-col='generateddate'>" + Compare(sType, generateddate, generateddate_compare, generateddate_compare_note) + "</td>");
                sb.Append("</tr>");


                sb.Append("</table>");

                #endregion

                #region Lu_BOM

                sSql = "select a.*,A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,A16,A17,A18,A19,A20,A21,A22,A23,A24,A25,A26,A27,A28,A29,A30,A31,A32,A33,A34,A35,A36,A37,A38,A39,A40,A41,A42,A43,A44,A45,A46,A47,A48,A49,A50,'0' as isExist \n";
                sSql += "from PDFTAG.dbo.Lu_BOM a              \n";
                sSql += "join  PDFTAG.dbo.Lu_BOMGarmentcolor b on a.lubcid=b.lubcid               \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid = @luhid   \n";
                //sSql += " order by lubid,rowid asc    \n";
                sSql += "  order by (CASE WHEN org_lubid is null THEN lubid ELSE org_lubid END) asc ,rowid asc    \n";
                Response.Write("<!--" + sSql.Replace("@luhid", luhid) + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                Response.Write("<!--" + sSql.Replace("@luhid", compare_luhid) + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", compare_luhid);
                dtCompare = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                }

                sSql = "select distinct A1,A2,A3,A4,A5,A6,A7,A8,A9,A10,A11,A12,A13,A14,A15,A16,A17,A18,A19,A20,A21,A22,A23,A24,A25,A26,A27,A28,A29,A30,A31,A32,A33,A34,A35,A36,A37,A38,A39,A40,A41,A42,A43,A44,A45,A46,A47,A48,A49,A50 \n";
                sSql += "from PDFTAG.dbo.Lu_BOMGarmentcolor a              \n";
                sSql += " where a.luhid = @luhid   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                DataTable dtLu_BOMGarmentcolor = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_BOMGarmentcolor);
                }

                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", compare_luhid);
                DataTable dtLu_BOMGarmentcolor_Compare = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_BOMGarmentcolor_Compare);
                }

                List<string> arrLu_BOMGarmentcolorsSource = new List<string>();       //來源的顏色色組
                List<string> arrLu_BOMGarmentcolorsCompare = new List<string>();    //目的的顏色色組
                //List<string> arrColors = new List<string>();
                List<string> arrLu_BOMGarmentcolors = new List<string>();


                foreach (DataRow drColor in dtLu_BOMGarmentcolor.Rows)
                {
                    for (int i = 1; i <= 50; i++)
                    {
                        string sA = drColor["A" + i].ToString();

                        if (string.IsNullOrEmpty(sA))
                        {
                            break;
                        }

                        arrLu_BOMGarmentcolors.Add(sA);
                        arrLu_BOMGarmentcolorsSource.Add(sA);
                    }
                }
                foreach (DataRow drColor in dtLu_BOMGarmentcolor_Compare.Rows)
                {
                    for (int i = 1; i <= 50; i++)
                    {
                        string sA = drColor["A" + i].ToString();

                        if (string.IsNullOrEmpty(sA))
                        {
                            break;
                        }

                        arrLu_BOMGarmentcolors.Add(sA);
                        arrLu_BOMGarmentcolorsCompare.Add(sA);
                    }
                }

                arrLu_BOMGarmentcolors = arrLu_BOMGarmentcolors.Distinct().ToList();

                //foreach (var color in arrColors.Distinct().ToList())
                //{
                //    arrLu_BOMGarmentcolors.Add(new HeaderExist { ColName = color });
                //}



                List<Lu_Ch_Note> arrNotes = new List<Lu_Ch_Note>();
                List<Lu_Ch_Note> arrNotesCompare = new List<Lu_Ch_Note>();
                //if (sType == "2")
                {
                    #region 中文備註

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                    sSql += "where 1=1 \n";
                    sSql += " and a.IdName='lubid'  and a.Id in (select lubid as id from PDFTAG.dbo.Lu_BOM where luhid = @luhid ) \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@luhid", luhid);
                    DataTable dt2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt2);
                    }

                    arrNotes = dt2.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@luhid", compare_luhid);
                    DataTable dtCompare2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtCompare2);
                    }

                    arrNotesCompare = dtCompare2.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    #endregion
                }

                //var arrTypes = dt.AsEnumerable().Select(s => s.Field<string>("type")).Distinct().ToList();
                var arrTypes = dt.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type") }).Distinct().ToList();

                List<string> arrExistTypes = new List<string>();

                foreach (var itemType in arrTypes)
                {
                    if (!arrExistTypes.Contains(itemType.type))
                        sb.Append("<h4>" + itemType.type + "</h4>");

                    arrExistTypes.Add(itemType.type);


                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>Standard Placement</th>");
                    sb.Append(" <th scope='col'>Placement</th>");
                    sb.Append(" <th scope='col'>Supplier Article</th>");
                    sb.Append(" <th scope='col'>Supplier</th>");

                    List<bool> arrCompareHeaderResult = new List<bool>();
                    List<string> arrBomHeader = new List<string>();
                    List<string> arrBomHeaderCompare = new List<string>();



                    #region lubcid

                    int iColorCnt = 0;
                    //int iColorMoreCnt = 0;

                    //20220419
                    //	來源文件 與 比對目的 都有的欄位 ，欄位底色顯示水藍色 #00FFFF
                    //	來源文件 有的欄位，但 比對目的 沒有的欄位，欄位底色 顯示 橘色 	#FF9224
                    //	來源文件 沒有的欄位，但 比對目的 有的欄位，欄位底色 顯示 紅色 #FF0000



                    //foreach (var color in arrLu_BOMGarmentcolors)
                    //{
                    //    var isExistSource = arrLu_BOMGarmentcolorsSource.Any(x => x == color);
                    //    var isExistCompare = arrLu_BOMGarmentcolorsCompare.Any(x => x == color);



                    //    if (isExistSource && isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#00FFFF'> " + color + "</th>");
                    //    else if (isExistSource && !isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#FF9224'> " + color + "</th>");
                    //    else if (!isExistSource && isExistCompare)
                    //        sb.Append(" <th scope='col' style='background-color:#FF0000'> " + color + "</th>");


                    //}

                    //20220425 以原文件欄位不動,去B找資料
                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_BOMGarmentcolor a              \n";
                    sSql += " where a.lubcid = '" + itemType.lubcid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    dtLu_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_BOMGarmentcolor);
                    }
                    arrLu_BOMGarmentcolors = new List<string>();
                    foreach (DataRow drColor in dtLu_BOMGarmentcolor.Rows)
                    {
                        for (int i = 1; i <= 50; i++)
                        {
                            string sA = drColor["A" + i].ToString();

                            if (string.IsNullOrEmpty(sA))
                            {
                                break;
                            }

                            var isExistCompare = arrLu_BOMGarmentcolorsCompare.Any(x => x == sA);

                            if (!isExistCompare)
                                sb.Append(" <th scope='col' style='background-color:#FF9224'>" + sA + "</th>");
                            else
                                sb.Append(" <th scope='col' style='background-color:#00FFFF'>" + sA + "</th>");
                            arrLu_BOMGarmentcolors.Add(sA);
                        }
                    }

                    sb.Append("</tr>");


                    DataRow[] drBoms = dt.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "' ", "org_lubid asc");
                    DataRow[] drComareBoms = dtCompare.Select("type='" + itemType.type + "'", "org_lubid asc");

                    List<Lu_Bom> arrCompareData = new List<Lu_Bom>();

                    int iRowId = 1;
                    foreach (DataRow drBomMain in drComareBoms)
                    {
                        for (int i = 1; i <= 50; i++)
                        {
                            string sColorName = drBomMain["A" + i].ToString();
                            string sColorVal = drBomMain["B" + i].ToString();

                            if (string.IsNullOrEmpty(sColorName))
                            {
                                break;
                            }

                            arrCompareData.Add(new Lu_Bom()
                            {
                                rowId = iRowId,
                                lubid = drBomMain["lubid"].ToString(),
                                org_lubid = drBomMain["org_lubid"].ToString(),
                                StandardPlacement = drBomMain["StandardPlacement"].ToString().Replace(" ", string.Empty),
                                Placement = drBomMain["Placement"].ToString().Replace(" ", string.Empty),
                                SupplierArticle = drBomMain["SupplierArticle"].ToString().Replace(" ", string.Empty),

                                StandardPlacement_org = drBomMain["StandardPlacement"].ToString(),
                                Placement_org = drBomMain["Placement"].ToString(),
                                SupplierArticle_org = drBomMain["SupplierArticle"].ToString(),
                                Supplier_org = drBomMain["Supplier"].ToString(),

                                ColorName = sColorName,
                                ColorVal = sColorVal,
                                ColorBName = "B" + i,
                                isExistA = false
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
                        string supplierArticle_source = "";
                        string supplier_source = "";

                        string standardPlacement = "";
                        string placement = "";
                        string supplierArticle = "";
                        string supplier = "";


                        string standardPlacement_note = "";
                        string placement_note = "";
                        string supplierArticle_note = "";
                        string supplier_note = "";


                        try
                        {
                            lubid = drBoms[b]["lubid"].ToString();
                            org_lubid = drBoms[b]["org_lubid"].ToString();

                            standardPlacement_source = drBoms[b]["StandardPlacement"].ToString();
                            placement_source = drBoms[b]["Placement"].ToString();
                            supplierArticle_source = drBoms[b]["SupplierArticle"].ToString();
                            supplier_source = drBoms[b]["Supplier"].ToString();

                            standardPlacement = drBoms[b]["StandardPlacement"].ToString();
                            placement = drBoms[b]["Placement"].ToString();
                            supplierArticle = drBoms[b]["SupplierArticle"].ToString();
                            supplier = drBoms[b]["Supplier"].ToString();

                            standardPlacement_note = FilterNote(arrNotes, lubid, "StandardPlacement");
                            placement_note = FilterNote(arrNotes, lubid, "Placement");
                            supplierArticle_note = FilterNote(arrNotes, lubid, "SupplierArticle");
                            supplier_note = FilterNote(arrNotes, lubid, "Supplier");
                        }
                        catch (Exception ex)
                        {
                            //Response.Write("<!--" + ex.ToString() + "-->");
                        }

                        string lubid_compare = "";
                        string lubid_org_compare = "";
                        string standardPlacement_compare = "";
                        string placement_compare = "";
                        string supplierArticle_compare = "";
                        string supplier_compare = "";

                        string standardPlacement_compare_note = "";
                        string placement_compare_note = "";
                        string supplierArticle_compare_note = "";
                        string supplier_compare_note = "";

                        try
                        {
                            var tmpCompareData = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                    && x.Placement == placement && x.SupplierArticle == supplierArticle);
                            lubid_compare = tmpCompareData.lubid;
                            lubid_org_compare = tmpCompareData.org_lubid;
                            standardPlacement_compare = tmpCompareData.StandardPlacement;
                            placement_compare = tmpCompareData.Placement;
                            supplierArticle_compare = tmpCompareData.SupplierArticle;
                            supplier_compare = tmpCompareData.Supplier_org;
                            //lubid_compare = drComareBoms[b]["lubid"].ToString();
                            //lubid_org_compare = drComareBoms[b]["org_lubid"].ToString();
                            //standardPlacement_compare = drComareBoms[b]["StandardPlacement"].ToString();
                            //placement_compare = drComareBoms[b]["Placement"].ToString();
                            //supplierArticle_compare = drComareBoms[b]["SupplierArticle"].ToString();
                            //supplier_compare = drComareBoms[b]["Supplier"].ToString();

                            standardPlacement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "StandardPlacement");
                            placement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Placement");
                            supplierArticle_compare_note = FilterNote(arrNotesCompare, lubid_compare, "SupplierArticle");
                            supplier_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Supplier");
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
                                supplierArticle = supplierArticle_note;
                                supplier = supplier_note;


                                standardPlacement_compare = standardPlacement_compare_note;
                                placement_compare = placement_compare_note;
                                supplierArticle_compare = supplierArticle_compare_note;
                                supplier_compare = supplier_compare_note;

                            }

                            sb.Append("<tr data-rowid='" + drBoms[b]["rowid"].ToString() + "'>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement'>" + standardPlacement_source + (string.IsNullOrEmpty(standardPlacement_note) ? "" : "<br>中:" + standardPlacement_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Placement'>" + placement_source + (string.IsNullOrEmpty(placement_note) ? "" : "<br>中:" + placement_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='SupplierArticle'>" + supplierArticle_source + (string.IsNullOrEmpty(supplierArticle_note) ? "" : "<br>中:" + supplierArticle_note) + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier'>" + supplier_source + (string.IsNullOrEmpty(supplier_note) ? "" : "<br>中:" + supplier_note) + "</td>");

                            List<string> arrColorNameSource = new List<string>();
                            List<string> arrColorValSource = new List<string>();

                            foreach (var color in arrLu_BOMGarmentcolors)
                            {
                                bool isFind = false;
                                for (int i = 1; i <= 50; i++)
                                {
                                    string colorName = drBoms[b]["A" + i].ToString();
                                    if (color == colorName)
                                    {
                                        string colorVal = drBoms[b]["B" + i].ToString();
                                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='B" + i + "' >" + colorVal + "</td>");
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
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' data-col='StandardPlacement' >" + (isCompare ? Compare(sType, standardPlacement, standardPlacement_compare, standardPlacement_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' data-col='Placement' >" + (isCompare ? Compare(sType, placement, placement_compare, placement_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' data-col='SupplierArticle' >" + (isCompare ? Compare(sType, supplierArticle, supplierArticle_compare, supplierArticle_compare_note) : "") + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' data-col='Supplier' >" + (isCompare ? Compare(sType, supplier, supplier_compare, supplier_compare_note) : "") + "</td>");

                            isCompare = true;

                            int iIndexSource = 0;
                            foreach (var color in arrLu_BOMGarmentcolors)
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
                                    supplierArticle = supplierArticle.Replace(" ", string.Empty);

                                    //     var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                    //&& x.Placement == placement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    //20220524
                                    //var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    //20220719
                                    var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                    && x.Placement == placement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                    if (resCompare != null)
                                    {
                                        isFind = true;
                                        colorValCompare = resCompare.ColorVal;
                                        resCompare.isExistA = true;
                                    }

                                    if (isFind)
                                    {
                                        sb.Append(" <td scope='col' data-lubid='" + lubid_compare + "' data-org_lubid='" + lubid_org_compare + "' data-col='" + resCompare.ColorBName + "' compare>" + (isCompare ? Compare(sType, colorValSource, colorValCompare, colorNoteValCompare) : "") + "</td>");
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
                }
                //沒比對到的
                #region  沒比對到的BOM
                var arrTypes_notCompare = dtCompare.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type") }).Distinct().ToList();

                List<string> arrExistTypes_notCompare = new List<string>();
                List<Lu_Bom> arrNotCompareData = new List<Lu_Bom>();
                StringBuilder sbNotExit_header = new StringBuilder();
                StringBuilder sbNotExit_tmptable = new StringBuilder();                
                StringBuilder sbNotExit_itemtype = new StringBuilder();
                StringBuilder sbNotExit_table = new StringBuilder();
                StringBuilder sbNotExit_tablebody = new StringBuilder();
                #region  色組
                sbNotExit.Append("<h4>色組</h4>");
                foreach (var itemType in arrTypes_notCompare)
                {
                    if (!arrExistTypes_notCompare.Contains(itemType.type))
                    {
                        sbNotExit.Append("<h4>" + itemType.type + "</h4>");
                        arrExistTypes_notCompare.Add(itemType.type);

                        sbNotExit.Append("<table class='table table-hover'>");
                        sbNotExit.Append("<tr>");
                        sbNotExit.Append(" <th scope='col'>Standard Placement</th>");
                        sbNotExit.Append(" <th scope='col'>Placement</th>");
                        sbNotExit.Append(" <th scope='col'>Supplier Article</th>");
                        sbNotExit.Append(" <th scope='col'>Supplier</th>");

                        List<string> curBomStyleColor = new List<string>();
                        foreach (DataRow drColor in dtLu_BOMGarmentcolor.Rows)
                        {
                            for (int i = 1; i <=50; i++)
                            {
                                string sA = drColor["A" + i].ToString();

                                if (string.IsNullOrEmpty(sA))
                                {
                                    break;
                                }
                                curBomStyleColor.Add(sA);
                            }
                        }
                        var isNewAtCompare = arrLu_BOMGarmentcolorsCompare.Where(x => !curBomStyleColor.Contains(x));
                        //20220718 顯示不存在來源的
                        bool isHasDifferent = false;
                        var colorList = new List<string>();
                        foreach (var color in arrLu_BOMGarmentcolorsCompare)
                        {
                            var isExistSource = arrLu_BOMGarmentcolorsSource.Any(x => x == color);

                            Response.Write("<!--color=" + color + " =" + isExistSource + "-->");

                            if (!isExistSource)
                            {
                                sbNotExit.Append(" <th scope='col' style='background-color:red'>" + color + "</th>");
                                isHasDifferent = true;
                                colorList.Add(color);
                            }
                        }

                        sbNotExit.Append("</tr>");

                        DataRow[] drBoms = dt.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "' ", "org_lubid asc");
                        DataRow[] drComareBoms = dtCompare.Select("type='" + itemType.type + "'", "org_lubid asc");

                        List<Lu_Bom> arrCompareData = new List<Lu_Bom>();

                        int iRowId = 1;
                        foreach (DataRow drBomMain in drComareBoms)
                        {
                            for (int i = 1; i <= 50; i++)
                            {
                                string sColorName = drBomMain["A" + i].ToString();
                                string sColorVal = drBomMain["B" + i].ToString();

                                if (string.IsNullOrEmpty(sColorName))
                                {
                                    break;
                                }

                                arrCompareData.Add(new Lu_Bom()
                                {
                                    rowId = iRowId,
                                    StandardPlacement = drBomMain["StandardPlacement"].ToString().Replace(" ", string.Empty),
                                    Placement = drBomMain["Placement"].ToString().Replace(" ", string.Empty),
                                    SupplierArticle = drBomMain["SupplierArticle"].ToString().Replace(" ", string.Empty),

                                    StandardPlacement_org = drBomMain["StandardPlacement"].ToString(),
                                    Placement_org = drBomMain["Placement"].ToString(),
                                    SupplierArticle_org = drBomMain["SupplierArticle"].ToString(),
                                    Supplier_org = drBomMain["Supplier"].ToString(),

                                    ColorName = sColorName,
                                    ColorVal = sColorVal,
                                    ColorBName = "B" + i,
                                    isExistA = false
                                });
                            }
                            iRowId++;
                        }
                        var arrRowIds = arrCompareData.Where(w => !w.isExistA && colorList.Contains(w.ColorName)).Select(s => s.rowId).Distinct().OrderBy(o => o).ToList();
                        var checkRepeat = new List<string>();
                        foreach (var rowId in arrRowIds)
                        {
                            var resItem = arrCompareData.FirstOrDefault(x => x.rowId == rowId && colorList.Contains(x.ColorName));
                            if (checkRepeat.Contains(resItem.StandardPlacement_org + "; " + resItem.Placement_org + "; " + resItem.SupplierArticle_org + "; " + resItem.Supplier_org))
                                continue;
                            checkRepeat.Add(resItem.StandardPlacement_org + "; " + resItem.Placement_org + "; " + resItem.SupplierArticle_org + "; " + resItem.Supplier_org);
                            sbNotExit.Append("<tr>");
                            sbNotExit.Append(" <td scope='col'>" + resItem.StandardPlacement_org + "</td>");
                            sbNotExit.Append(" <td scope='col'>" + resItem.Placement_org + "</td>");
                            sbNotExit.Append(" <td scope='col'>" + resItem.SupplierArticle_org + "</td>");
                            sbNotExit.Append(" <td scope='col'>" + resItem.Supplier_org + "</td>");

                            foreach (var color in colorList)
                            {
                                var resColor = arrCompareData.Where(x => x.ColorName == color
                                                                                                && x.StandardPlacement_org == resItem.StandardPlacement_org
                                                                                                && x.Placement_org == resItem.Placement_org
                                                                                                && x.SupplierArticle_org == resItem.SupplierArticle_org
                                                                                                && x.Supplier_org == resItem.Supplier_org).Select(s => s.ColorVal).Distinct().ToList();
                                foreach (var bColor in resColor)
                                {
                                    sbNotExit.Append(" <td scope='col'>" + bColor + "</td>");
                                }
                            }
                            sbNotExit.Append("</tr>");
                        }
                        sbNotExit.Append("</table>");
                        if (!isHasDifferent)
                            sbNotExit.Length = 0;
                    }
                }
                #endregion

                #region  資料列
                sbNotExit_header.Append("<h4>資料列</h4>");
                //bool headerFlag = true;
                arrExistTypes_notCompare = new List<string>();
                foreach (var itemType in arrTypes_notCompare)
                {
                    if (!arrExistTypes_notCompare.Contains(itemType.type))
                    {
                        sbNotExit_itemtype.Append("<h4>" + itemType.type + "</h4>");
                        arrExistTypes_notCompare.Add(itemType.type);
                    }
                    sbNotExit_table.Append("<table class='table table-hover'>");
                    sbNotExit_table.Append("<tr>");
                    sbNotExit_table.Append(" <th scope='col'>Standard Placement</th>");
                    sbNotExit_table.Append(" <th scope='col'>Placement</th>");
                    sbNotExit_table.Append(" <th scope='col'>Supplier Article</th>");
                    sbNotExit_table.Append(" <th scope='col'>Supplier</th>");

                    List<bool> arrCompareHeaderResult = new List<bool>();
                    List<string> arrBomHeader = new List<string>();
                    List<string> arrBomHeaderCompare = new List<string>();

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_BOMGarmentcolor a              \n";
                    sSql += " where a.lubcid = '" + itemType.lubcid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    dtLu_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_BOMGarmentcolor);
                    }
                    arrLu_BOMGarmentcolors = new List<string>();
                    foreach (DataRow drColor in dtLu_BOMGarmentcolor.Rows)
                    {
                        for (int i = 1; i <= 50; i++)
                        {
                            string sA = drColor["A" + i].ToString();

                            if (string.IsNullOrEmpty(sA))
                            {
                                break;
                            }

                            var isExistCompare = arrLu_BOMGarmentcolorsSource.Any(x => x == sA);

                            if (!isExistCompare)
                            {
                                sbNotExit_table.Append(" <th scope='col' style='background-color:red'>" + sA + "</th>");
                            }
                            else
                                sbNotExit_table.Append(" <th scope='col'>" + sA + "</th>");
                            arrLu_BOMGarmentcolors.Add(sA);
                        }
                    }

                    sbNotExit_table.Append("</tr>");


                    DataRow[] drBoms = dtCompare.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "' ", "org_lubid asc");
                    DataRow[] drComareBoms = dt.Select("type='" + itemType.type + "'", "org_lubid asc");

                    List<Lu_Bom> arrCompareData = new List<Lu_Bom>();

                    int iRowId = 1;
                    foreach (DataRow drBomMain in drComareBoms)
                    {
                        for (int i = 1; i <=50; i++)
                        {
                            string sColorName = drBomMain["A" + i].ToString();
                            string sColorVal = drBomMain["B" + i].ToString();

                            if (string.IsNullOrEmpty(sColorName))
                            {
                                break;
                            }

                            arrCompareData.Add(new Lu_Bom()
                            {
                                rowId = iRowId,
                                StandardPlacement = drBomMain["StandardPlacement"].ToString().Replace(" ", string.Empty),
                                Placement = drBomMain["Placement"].ToString().Replace(" ", string.Empty),
                                SupplierArticle = drBomMain["SupplierArticle"].ToString().Replace(" ", string.Empty),

                                StandardPlacement_org = drBomMain["StandardPlacement"].ToString(),
                                Placement_org = drBomMain["Placement"].ToString(),
                                SupplierArticle_org = drBomMain["SupplierArticle"].ToString(),
                                Supplier_org = drBomMain["Supplier"].ToString(),

                                ColorName = sColorName,
                                ColorVal = sColorVal,
                                ColorBName = "B" + i,
                                isExistA = false
                            });

                        }

                        iRowId++;
                    }
                    StringBuilder tmpBody = new StringBuilder();
                    //20220419 原始不動,去比對找資料
                    for (int b = 0; b < drBoms.Length; b++)
                    {
                        string lubid = "";
                        string org_lubid = "";


                        string standardPlacement_source = "";
                        string placement_source = "";
                        string supplierArticle_source = "";
                        string supplier_source = "";

                        string standardPlacement = "";
                        string placement = "";
                        string supplierArticle = "";
                        string supplier = "";


                        string standardPlacement_note = "";
                        string placement_note = "";
                        string supplierArticle_note = "";
                        string supplier_note = "";

                        lubid = drBoms[b]["lubid"].ToString();
                        org_lubid = drBoms[b]["org_lubid"].ToString();

                        standardPlacement_source = drBoms[b]["StandardPlacement"].ToString();
                        placement_source = drBoms[b]["Placement"].ToString();
                        supplierArticle_source = drBoms[b]["SupplierArticle"].ToString();
                        supplier_source = drBoms[b]["Supplier"].ToString();

                        standardPlacement = drBoms[b]["StandardPlacement"].ToString();
                        placement = drBoms[b]["Placement"].ToString();
                        supplierArticle = drBoms[b]["SupplierArticle"].ToString();
                        supplier = drBoms[b]["Supplier"].ToString();

                        standardPlacement_note = FilterNote(arrNotes, lubid, "StandardPlacement");
                        placement_note = FilterNote(arrNotes, lubid, "Placement");
                        supplierArticle_note = FilterNote(arrNotes, lubid, "SupplierArticle");
                        supplier_note = FilterNote(arrNotes, lubid, "Supplier");

                        //string lubid_compare = "";
                        //string lubid_org_compare = "";
                        //string standardPlacement_compare = "";
                        //string placement_compare = "";
                        //string supplierArticle_compare = "";
                        //string supplier_compare = "";

                        //string standardPlacement_compare_note = "";
                        //string placement_compare_note = "";
                        //string supplierArticle_compare_note = "";
                        //string supplier_compare_note = "";
                        //if (drComareBoms.Length < drBoms.Length && drComareBoms.Length < b)
                        //{ }
                        //else
                        //{
                        //    lubid_compare = drComareBoms[b]["lubid"].ToString();
                        //    lubid_org_compare = drComareBoms[b]["org_lubid"].ToString();
                        //    standardPlacement_compare = drComareBoms[b]["StandardPlacement"].ToString();
                        //    placement_compare = drComareBoms[b]["Placement"].ToString();
                        //    supplierArticle_compare = drComareBoms[b]["SupplierArticle"].ToString();
                        //    supplier_compare = drComareBoms[b]["Supplier"].ToString();

                        //    standardPlacement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "StandardPlacement");
                        //    placement_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Placement");
                        //    supplierArticle_compare_note = FilterNote(arrNotesCompare, lubid_compare, "SupplierArticle");
                        //    supplier_compare_note = FilterNote(arrNotesCompare, lubid_compare, "Supplier");
                        //}

                        #region         

                        //if (sType == "2")
                        //{
                        //    standardPlacement = standardPlacement_note;
                        //    placement = placement_note;
                        //    supplierArticle = supplierArticle_note;
                        //    supplier = supplier_note;


                        //    standardPlacement_compare = standardPlacement_compare_note;
                        //    placement_compare = placement_compare_note;
                        //    supplierArticle_compare = supplierArticle_compare_note;
                        //    supplier_compare = supplier_compare_note;

                        //}

                        sbNotExit_tablebody.Append("<tr data-rowid='" + drBoms[b]["rowid"].ToString() + "'>");
                        sbNotExit_tablebody.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement'>" + standardPlacement_source + (string.IsNullOrEmpty(standardPlacement_note) ? "" : "<br>中:" + standardPlacement_note) + "</td>");
                        sbNotExit_tablebody.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Placement'>" + placement_source + (string.IsNullOrEmpty(placement_note) ? "" : "<br>中:" + placement_note) + "</td>");
                        sbNotExit_tablebody.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='SupplierArticle'>" + supplierArticle_source + (string.IsNullOrEmpty(supplierArticle_note) ? "" : "<br>中:" + supplierArticle_note) + "</td>");
                        sbNotExit_tablebody.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier'>" + supplier_source + (string.IsNullOrEmpty(supplier_note) ? "" : "<br>中:" + supplier_note) + "</td>");

                        List<string> arrColorNameSource = new List<string>();
                        List<string> arrColorValSource = new List<string>();
                        foreach (var color in arrLu_BOMGarmentcolors)
                        {
                            bool isFind = false;
                            for (int i = 1; i <= 50; i++)
                            {
                                string colorName = drBoms[b]["A" + i].ToString();
                                if (color == colorName)
                                {
                                    string colorVal = drBoms[b]["B" + i].ToString();
                                    sbNotExit_tablebody.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='B" + i + "' >" + colorVal + "</td>");
                                    isFind = true;

                                    arrColorNameSource.Add(colorName);
                                    arrColorValSource.Add(colorVal);
                                    break;
                                }
                            }
                            if (!isFind)
                            {
                                sbNotExit_tablebody.Append(" <td scope='col'></td>");
                                arrColorValSource.Add("");
                            }
                        }
                        sbNotExit_tablebody.Append("</tr>");
                        #region 比對結果

                        int iIndexSource = 0;
                        foreach (var color in arrLu_BOMGarmentcolors)
                        {
                            string colorValSource = arrColorValSource[iIndexSource];
                            string colorValCompare = "";

                            bool isFind = false;

                            if (arrColorNameSource.Contains(color))
                            {
                                standardPlacement = standardPlacement.Replace(" ", string.Empty);
                                placement = placement.Replace(" ", string.Empty);
                                supplierArticle = supplierArticle.Replace(" ", string.Empty);

                                var resCompare = arrCompareData.FirstOrDefault(x => x.StandardPlacement == standardPlacement
                                && x.Placement == placement && x.SupplierArticle == supplierArticle && x.ColorName == color);

                                if (resCompare != null)
                                {
                                    isFind = true;
                                    colorValCompare = resCompare.ColorVal;
                                    resCompare.isExistA = true;
                                }

                                if (!isFind)
                                {}
                                else
                                {
                                    sbNotExit_tablebody.Clear();
                                }
                            }
                            iIndexSource++;
                        }

                        #endregion
                        #endregion
                        sbNotExit_table.Append(sbNotExit_tablebody.ToString());
                        tmpBody.Append(sbNotExit_tablebody.ToString());                        
                        sbNotExit_tablebody.Clear();
                    }//end for bom
                    sbNotExit_table.Append(sbNotExit_tablebody.ToString());
                    sbNotExit_table.Append("</table>");
                    if (tmpBody.Length == 0)
                    {
                        sbNotExit_table.Clear();
                    }
                    if (sbNotExit_table.Length == 0)
                    {
                        sbNotExit_itemtype.Clear();
                    }
                    //if (sbNotExit_itemtype.Length == 0)
                    //{
                    //    sbNotExit_header.Clear();
                    //}
                    //if (headerFlag && sbNotExit_header.Length > 0) { sbNotExit.Append(sbNotExit_header.ToString()); headerFlag = false; }
                    sbNotExit_tmptable.Append(sbNotExit_itemtype.ToString());
                    sbNotExit_tmptable.Append(sbNotExit_table.ToString());
                    //sbNotExit.Append(sbNotExit_itemtype.ToString());
                    //sbNotExit.Append(sbNotExit_table.ToString());
                    sbNotExit_itemtype.Clear();
                    sbNotExit_table.Clear();
                    sbNotExit_tablebody.Clear();
                }//end foreach itemtype
                if (sbNotExit_tmptable.Length != 0)
                {
                    sbNotExit.Append(sbNotExit_header.ToString());
                    sbNotExit.Append(sbNotExit_tmptable.ToString());
                }
                #endregion
                #endregion


                #endregion

                #region Lu_SizeTable                
                List<string> listSourceSizeTableHeader = new List<string>();       //來源的尺寸
                List<string> listCompareSizeTableHeader = new List<string>();    //目的的尺寸
                //List<string> listSizeTableHeader = new List<string>();

                //撈出全部的尺寸表
                sSql = @"select a.*,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15
                    from PDFTAG.dbo.Lu_SizeTable a
                    join  PDFTAG.dbo.Lu_SizeTable_Header b on a.lusthid=b.lusthid
                    where 1=1
                    and a.luhid =@luhid
                    --and a.lusthid in ('" + hid_Source_lusthid.Value.Replace(",", "','") + @"')--直接全抓
                    and b.SAMPLE = ''     --排除樣品尺寸表
                    order by lusthid,rowid asc ";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                //這邊找出有幾頁，並撈出樣衣尺寸
                var arr_lusthids = dt.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().OrderBy(o => o).ToList();

                Dictionary<string, List<string>> dicHeardersSource = new Dictionary<string, List<string>>();
                //List<string> arrHeardersSource = new List<string>();

                for (int h = 0; h < arr_lusthids.Count; h++)
                {
                    string lusthid = arr_lusthids[h].ToString();
                    List<string> tmpSize = new List<string>();
                       
                    sSql = @"select *
                        from PDFTAG.dbo.Lu_SizeTable_Header a
                        where a.lusthid in ('" + lusthid + "') ";
                    cm.CommandText = sSql;
                    DataTable dtLu_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_SizeTable_Header);
                    }
                    for (int i = 1; i <= 15; i++)
                    {
                        string sH = dtLu_SizeTable_Header.Rows[0]["H" + i].ToString();
                        if (string.IsNullOrEmpty(sH)) { dicHeardersSource.Add(lusthid, tmpSize); break; }
                        if (!tmpSize.Contains(sH)) { tmpSize.Add(sH); listSourceSizeTableHeader.Add(sH); }
                        //if(!arrHeardersSource.Contains(sH)) arrHeardersSource.Add(sH);
                    }
                }

                //撈出全部的尺寸表
                sSql = @"select a.*,H1,H2,H3,H4,H5,H6,H7,H8,H9,H10,H11,H12,H13,H14,H15
                    from PDFTAG.dbo.Lu_SizeTable a
                    join  PDFTAG.dbo.Lu_SizeTable_Header b on a.lusthid=b.lusthid
                    where 1=1
                    and a.luhid =@luhid
                    --and a.lusthid in ('" + hid_Compare_lusthid.Value.Replace(",", "','") + @"')
                    and b.SAMPLE = ''     --排除樣品尺寸表
                    order by lusthid,rowid asc ";

                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", compare_luhid);
                dtCompare = new DataTable();
                DataTable getSizeTable = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtCompare);
                    //複製一份Compare的，比對時找codeid的第一筆然後刪掉
                    da.Fill(getSizeTable);
                }

                //這邊找出有幾頁，並撈出樣衣尺寸
                var arr_Comparelusthids = dtCompare.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().OrderBy(o => o).ToList();
                Response.Write("<!--luhid=" + luhid + ";compare_luhid=" + compare_luhid + "-->");

                Dictionary<string, List<string>> dicHeardersCompare = new Dictionary<string, List<string>>();
                //List<string> arrHeardersCompare = new List<string>();

                for (int h = 0; h < arr_Comparelusthids.Count; h++)
                {
                    string lusthidCompare = arr_Comparelusthids[h].ToString();
                    List<string> tmpSize = new List<string>();

                    sSql = @"select *
                        from PDFTAG.dbo.Lu_SizeTable_Header a
                        where a.lusthid in ('" + lusthidCompare + "') ";
                    cm.CommandText = sSql;
                    DataTable dtLu_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtLu_SizeTable_Header);
                    }
                    for (int i = 1; i <= 15; i++)
                    {
                        string sH = dtLu_SizeTable_Header.Rows[0]["H" + i].ToString();
                        if (string.IsNullOrEmpty(sH)) { dicHeardersCompare.Add(lusthidCompare, tmpSize); break; }
                        if (!tmpSize.Contains(sH)) { tmpSize.Add(sH); listCompareSizeTableHeader.Add(sH); }
                        //if (!arrHeardersCompare.Contains(sH)) arrHeardersCompare.Add(sH);
                    }
                }


                arrNotes = new List<Lu_Ch_Note>();
                arrNotesCompare = new List<Lu_Ch_Note>();
                //if (sType == "2")
                {
                    #region 中文備註

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.Lu_Ch_Note  a             \n";
                    sSql += "where 1=1 \n";
                    sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.Lu_SizeTable where luhid = @luhid ) \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@luhid", luhid);
                    DataTable dt2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dt2);
                    }

                    arrNotes = dt2.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                    cm.CommandText = sSql;
                    cm.Parameters.Clear();
                    cm.Parameters.AddWithValue("@luhid", compare_luhid);
                    DataTable dtCompare2 = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtCompare2);
                    }

                    arrNotesCompare = dtCompare2.AsEnumerable().Select(s => new Lu_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

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

                        DataTable dtLu_SizeTableCompare_Header = new DataTable();
                        string lusthid_compare = string.Join("','", listComparelusthids);

                        sSql = @"select * 
                            from PDFTAG.dbo.Lu_SizeTable_Header a
                            where a.lusthid in ('" + lusthid_compare + "') ";
                        Response.Write("<!--" + sSql + "-->");
                        cm.CommandText = sSql;

                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                        {
                            da.Fill(dtLu_SizeTableCompare_Header);
                        }

                        if (h == 0)
                        {
                            sb.Append("<h4>Size Table</h4>");
                        }
                        sb.Append("<table class='table table-hover'>");
                        sb.Append("<tr>");
                        sb.Append(" <th scope='col'>#</th>");
                        sb.Append(" <th scope='col'>Name</th>");
                        sb.Append(" <th scope='col'>Criticality</th>");
                        sb.Append(" <th scope='col'>Tol(-)</th>");
                        sb.Append(" <th scope='col'>Tol(+)</th>");
                        sb.Append(" <th scope='col'>HTM Instruction</th>");

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
                            //找compare codeid符合的第一筆
                            var tmpSizetable = getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("codeid").ToString().Replace(" ", "") == drSizeTables[s]["codeid"].ToString().Replace(" ", "")
                                && x.Field<string>("H1") == drSizeTables[s]["H1"].ToString());

                            string lustid = "";
                            string org_lustid = "";

                            string codeid_source = "";
                            string name_source = "";
                            string criticality_source = "";
                            string tolA_source = "";
                            string tolB_source = "";
                            string hTMInstruction_source = "";

                            string codeid = "";
                            string name = "";
                            string criticality = "";
                            string tolA = "";
                            string tolB = "";
                            string hTMInstruction = "";

                            string codeid_note = "";
                            string name_note = "";
                            string criticality_note = "";
                            string tolA_note = "";
                            string tolB_note = "";
                            string hTMInstruction_note = "";


                            string lustid_compare = "";
                            string lustid_org_compare = "";
                            string codeid_compare = "";
                            string name_compare = "";
                            string criticality_compare = "";
                            string tolA_compare = "";
                            string tolB_compare = "";
                            string hTMInstruction_compare = "";

                            string codeid_compare_note = "";
                            string name_compare_note = "";
                            string criticality_compare_note = "";
                            string tolA_compare_note = "";
                            string tolB_compare_note = "";
                            string hTMInstruction_compare_note = "";

                            try
                            {
                                lustid = drSizeTables[s]["lustid"].ToString();
                                org_lustid = drSizeTables[s]["org_lustid"].ToString();

                                codeid_source = drSizeTables[s]["codeid"].ToString();
                                name_source = drSizeTables[s]["Name"].ToString();
                                criticality_source = drSizeTables[s]["Criticality"].ToString();
                                tolA_source = drSizeTables[s]["TolA"].ToString();
                                tolB_source = drSizeTables[s]["TolB"].ToString();
                                hTMInstruction_source = drSizeTables[s]["HTMInstruction"].ToString();

                                codeid = drSizeTables[s]["codeid"].ToString();
                                name = drSizeTables[s]["Name"].ToString();
                                criticality = drSizeTables[s]["Criticality"].ToString();
                                tolA = drSizeTables[s]["TolA"].ToString();
                                tolB = drSizeTables[s]["TolB"].ToString();
                                hTMInstruction = drSizeTables[s]["HTMInstruction"].ToString();

                                codeid_note = FilterNote(arrNotes, lustid, "codeid");
                                name_note = FilterNote(arrNotes, lustid, "name");
                                criticality_note = FilterNote(arrNotes, lustid, "criticality");
                                tolA_note = FilterNote(arrNotes, lustid, "tolA");
                                tolB_note = FilterNote(arrNotes, lustid, "tolB");
                                hTMInstruction_note = FilterNote(arrNotes, lustid, "hTMInstruction");
                            }
                            catch (Exception ex) { }

                            try
                            {
                                lustid_compare = tmpSizetable["lustid"].ToString();
                                lustid_org_compare = tmpSizetable["org_lustid"].ToString();
                                codeid_compare = tmpSizetable["codeid"].ToString();
                                name_compare = tmpSizetable["Name"].ToString();
                                criticality_compare = tmpSizetable["Criticality"].ToString();
                                tolA_compare = tmpSizetable["TolA"].ToString();
                                tolB_compare = tmpSizetable["TolB"].ToString();
                                hTMInstruction_compare = tmpSizetable["HTMInstruction"].ToString();

                                codeid_compare_note = FilterNote(arrNotesCompare, lustid_compare, "codeid");
                                name_compare_note = FilterNote(arrNotesCompare, lustid_compare, "name");
                                criticality_compare_note = FilterNote(arrNotesCompare, lustid_compare, "criticality");
                                tolA_compare_note = FilterNote(arrNotesCompare, lustid_compare, "tolA");
                                tolB_compare_note = FilterNote(arrNotesCompare, lustid_compare, "tolB");
                                hTMInstruction_compare_note = FilterNote(arrNotesCompare, lustid_compare, "hTMInstruction");
                            }
                            catch (Exception ex) { }

                            try
                            {
                                #region 
                                    
                                if (sType == "2")
                                {

                                    codeid = codeid_note;
                                    name = name_note;
                                    criticality = criticality_note;
                                    tolA = tolA_note;
                                    tolB = tolB_note;
                                    hTMInstruction = hTMInstruction_note;


                                    codeid_compare = codeid_compare_note;
                                    name_compare = name_compare_note;
                                    criticality_compare = criticality_compare_note;
                                    tolA_compare = tolA_compare_note;
                                    tolB_compare = tolB_compare_note;
                                    hTMInstruction_compare = hTMInstruction_compare_note;
                                }

                                sb.Append("<tr data-rowid='" + drSizeTables[s]["rowid"].ToString() + "'>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='codeid' >" + codeid_source + (string.IsNullOrEmpty(codeid_note) ? "" : "<br>中:" + codeid_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Name' >" + name_source + (string.IsNullOrEmpty(name_note) ? "" : "<br>中:" + name_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' >" + criticality_source + (string.IsNullOrEmpty(criticality_note) ? "" : "<br>中:" + criticality_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' >" + tolA_source + (string.IsNullOrEmpty(tolA_note) ? "" : "<br>中:" + tolA_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' >" + tolB_source + (string.IsNullOrEmpty(tolB_note) ? "" : "<br>中:" + tolB_note) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='HTMInstruction' >" + hTMInstruction_source + (string.IsNullOrEmpty(hTMInstruction_note) ? "" : "<br>中:" + hTMInstruction_note) + "</td>");
                                    
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
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='codeid' >" + (string.IsNullOrEmpty(codeid_compare) ? "" : Compare(sType, codeid, codeid_compare, codeid_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='Name' >" + (string.IsNullOrEmpty(name_compare) ? "" : Compare(sType, name, name_compare, name_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='Criticality' >" + (string.IsNullOrEmpty(criticality_compare) ? "" : Compare(sType, criticality, criticality_compare, criticality_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='TolA' >" + (string.IsNullOrEmpty(tolA_compare) ? "" : Compare(sType, tolA, tolA_compare, tolA_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='TolB' >" + (string.IsNullOrEmpty(tolB_compare) ? "" : Compare(sType, tolB, tolB_compare, tolB_compare_note)) + "</td>");
                                sb.Append(" <td scope='col' data-lustid='" + lustid_compare + "' data-org_lustid='" + lustid_org_compare + "' data-col='HTMInstruction' >" + (string.IsNullOrEmpty(hTMInstruction_compare) ? "" : Compare(sType, hTMInstruction, hTMInstruction_compare, hTMInstruction_compare_note)) + "</td>");
                                    
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
                            if (!string.IsNullOrEmpty(codeid_compare))
                                getSizeTable.Rows.Remove(getSizeTable.AsEnumerable().FirstOrDefault(x => x.Field<string>("codeid").ToString().Replace(" ", "") == drSizeTables[s]["codeid"].ToString().Replace(" ", "")
                                        && x.Field<string>("H1") == drSizeTables[s]["H1"].ToString()));

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
                            DataTable dtLu_SizeTableSource_Header = new DataTable();
                            string lusthid_compare = string.Join("','", listSourcelusthids);

                            sSql = @"select * 
                            from PDFTAG.dbo.Lu_SizeTable_Header a
                            where a.lusthid in ('" + lusthid_compare + "') ";
                            Response.Write("<!--" + sSql + "-->");
                            cm.CommandText = sSql;

                            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                            {
                                da.Fill(dtLu_SizeTableSource_Header);
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
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>#</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Name</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Criticality</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Tol(-)</th>");
                            sbNotExitSizeTmpTable.Append(" <th scope='col'>Tol(+)</th>");

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
                                    
                                string codeid = "";
                                string name = "";
                                string criticality = "";
                                string tolA = "";
                                string tolB = "";
                                string hTMInstruction = "";

                                codeid = drCompareSizeTables[s]["codeid"].ToString();
                                name = drCompareSizeTables[s]["Name"].ToString();
                                criticality = drCompareSizeTables[s]["Criticality"].ToString();
                                tolA = drCompareSizeTables[s]["TolA"].ToString();
                                tolB = drCompareSizeTables[s]["TolB"].ToString();
                                hTMInstruction = drCompareSizeTables[s]["HTMInstruction"].ToString();

                                sbNotExitSizeTableBody.Append("<tr data-rowid='" + drCompareSizeTables[s]["rowid"].ToString() + "'>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='codeid' >" + codeid + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Name' >" + name + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' >" + criticality + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' >" + tolA + "</td>");
                                sbNotExitSizeTableBody.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' >" + tolB + "</td>");
                                    
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
                            sbNotExit.Append(" <th scope='col'>#</th>");
                            sbNotExit.Append(" <th scope='col'>Name</th>");
                            sbNotExit.Append(" <th scope='col'>Criticality</th>");
                            sbNotExit.Append(" <th scope='col'>Tol(-)</th>");
                            sbNotExit.Append(" <th scope='col'>Tol(+)</th>");

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
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='codeid' >" + getdata["codeid"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='Name' >" + getdata["Name"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='Criticality' >" + getdata["Criticality"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='TolA' >" + getdata["TolA"].ToString() + "</td>");
                        sbNotExit.Append(" <td scope='col' data-lustid='" + getdata["lustid"].ToString() + "' data-org_lustid='" + getdata["org_lustid"].ToString() + "' data-col='TolB' >" + getdata["TolB"].ToString() + "</td>");
                        for (int i = 1; i <= 15; i++)
                        {
                            if ((string.IsNullOrEmpty(getdata["A" + i].ToString()) || getdata["A" + i].ToString() == "----") && getdata["H" + i].ToString().Replace(" ", "") != "Comments") break;
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