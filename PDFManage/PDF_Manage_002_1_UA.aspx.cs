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
using Dapper;

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
            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.UA_Header a              \n";
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

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_Header a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid in (select org_luhid as luhid from  PDFTAG.dbo.UA_Header where pipid = '" + hidpipid.Value + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtUA_Header_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtUA_Header_Org);
                }


                List<UA_Ch_Note> arrNotes = new List<UA_Ch_Note>();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.UA_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                DataTable dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new UA_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                int iSeq = 1;

                #region UA_Header

                string season = dt.Rows[0]["Season"].ToString();
                string style = dt.Rows[0]["style"].ToString();
                string styledesc = dt.Rows[0]["styledesc"].ToString();
                string sFitType = dt.Rows[0]["FitType"].ToString();
                string sGearLine = dt.Rows[0]["GearLine"].ToString();
                string productsize = dt.Rows[0]["productsize"].ToString();
                string productfamily = dt.Rows[0]["class"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();
                bool isEditHeader = (dt.Rows[0]["isEdit"].ToString().ToLower() == "true" ? true : false);

                hidStyle.Value = style;

                DataRow[] drOrgHeades = dtUA_Header_Org.Select("luhid='" + org_luhid + "'");

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
                        productfamily = Compare(drOrgHeader["class"].ToString(), productfamily, FilterNote(arrNotes, luhid, "productfamily"));
                        //brand = Compare(drOrgHeader["brand"].ToString(), brand, FilterNote(arrNotes, luhid, "brand"));
                        //dividion = Compare(drOrgHeader["dividion"].ToString(), dividion, FilterNote(arrNotes, luhid, "dividion"));
                        //pod = Compare(drOrgHeader["pod"].ToString(), pod, FilterNote(arrNotes, luhid, "pod"));
                        stylestatus = Compare(drOrgHeader["stylestatus"].ToString(), stylestatus, FilterNote(arrNotes, luhid, "stylestatus"));
                        //generateddate = Compare(drOrgHeader["generateddate"].ToString(), generateddate, FilterNote(arrNotes, luhid, "generateddate"));
                    }

                }

                sb.Append("<table class='table table-hover'>");
                sb.Append("<tr>");
                sb.Append(" <th scope='col'>Season</th>");
                sb.Append(" <th scope='col'>Style</th>");
                sb.Append(" <th scope='col'>Style Desc</th>");
                sb.Append(" <th scope='col'>Fit Type</th>");
                sb.Append(" <th scope='col'>Gear Line</th>");
                sb.Append(" <th scope='col'>Size Scale</th>");
                sb.Append(" <th scope='col'>Product Family</th>");
                sb.Append(" <th scope='col'>Sample Status</th>");
                sb.Append(" <th scope='col'></th>");
                sb.Append("</tr>");
                sb.Append("<tr id='row" + iSeq + "'>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='Season' onclick='editHeader(this)'>" + season + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='style' onclick='editHeader(this)'>" + style + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='styledesc' onclick='editHeader(this)'>" + styledesc + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='FitType' onclick='editHeader(this)'>" + sFitType + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='GearLine' onclick='editHeader(this)'>" + sGearLine + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='productsize' onclick='editHeader(this)'>" + productsize + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='class' onclick='editHeader(this)'>" + productfamily + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='stylestatus' onclick='editHeader(this)'>" + stylestatus + "</td>");

                sb.Append(" <td scope='col'>");
                sb.Append("   <input typ='button' class='btn btn-danger btn-sm' value='整筆刪除' onclick='deleteHeader(" + luhid + "," + iSeq + ")' />");
                sb.Append("   <input typ='button' class='btn btn-secondary btn-sm' value='整筆新增'  onclick='copyHeader(" + luhid + ")'/>");
                sb.Append(" </td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                #region UA_BOM

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='lubid'  and a.Id in (select lubid as id from PDFTAG.dbo.UA_BOM where luhid = @luhid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new UA_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();


                sSql = "select a.* \n";
                sSql += ",(CASE WHEN c.EW1 is null or c.EW1='' THEN c.W1 ELSE c.EW1 End) as W1              \n";
                sSql += ",(CASE WHEN c.EW2 is null or c.EW2='' THEN c.W2 ELSE c.EW2 End) as W2              \n";
                sSql += ",(CASE WHEN c.EW3 is null or c.EW3='' THEN c.W3 ELSE c.EW3 End) as W3              \n";
                sSql += ",(CASE WHEN c.EW4 is null or c.EW4='' THEN c.W4 ELSE c.EW4 End) as W4              \n";
                sSql += ",(CASE WHEN c.EW5 is null or c.EW5='' THEN c.W5 ELSE c.EW5 End) as W5              \n";
                sSql += ",(CASE WHEN c.EW6 is null or c.EW6='' THEN c.W6 ELSE c.EW6 End) as W6              \n";
                sSql += ",(CASE WHEN c.EW7 is null or c.EW7='' THEN c.W7 ELSE c.EW7 End) as W7              \n";
                sSql += ",(CASE WHEN c.EW8 is null or c.EW8='' THEN c.W8 ELSE c.EW8 End) as W8              \n";
                sSql += ",c.EW4              \n";
                sSql += ",c.EW1_note              \n";
                sSql += ",c.EW2_note              \n";
                sSql += ",c.EW3_note              \n";
                sSql += ",c.EW4_note              \n";
                sSql += ",c.EW5_note              \n";
                sSql += ",c.EW6_note              \n";
                sSql += ",c.EW7_note              \n";
                sSql += ",c.EW8_note              \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " left join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid = '" + luhid + "'   \n";
                //sSql += " order by lubid,rowid asc    \n";
                sSql += "  order by (CASE WHEN a.org_lubid is null THEN a.lubid ELSE org_lubid END) asc ,rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                sSql = "select a.* \n";
                sSql += ",(CASE WHEN c.EW1 is null or c.EW1='' THEN c.W1 ELSE c.EW1 End) as W1              \n";
                sSql += ",(CASE WHEN c.EW2 is null or c.EW2='' THEN c.W2 ELSE c.EW2 End) as W2              \n";
                sSql += ",(CASE WHEN c.EW3 is null or c.EW3='' THEN c.W3 ELSE c.EW3 End) as W3              \n";
                sSql += ",(CASE WHEN c.EW4 is null or c.EW4='' THEN c.W4 ELSE c.EW4 End) as W4              \n";
                sSql += ",(CASE WHEN c.EW5 is null or c.EW5='' THEN c.W5 ELSE c.EW5 End) as W5              \n";
                sSql += ",(CASE WHEN c.EW6 is null or c.EW6='' THEN c.W6 ELSE c.EW6 End) as W6              \n";
                sSql += ",(CASE WHEN c.EW7 is null or c.EW7='' THEN c.W7 ELSE c.EW7 End) as W7              \n";
                sSql += ",(CASE WHEN c.EW8 is null or c.EW8='' THEN c.W8 ELSE c.EW8 End) as W8              \n";
                sSql += ",c.EW4              \n";
                sSql += ",c.EW1_note              \n";
                sSql += ",c.EW2_note              \n";
                sSql += ",c.EW3_note              \n";
                sSql += ",c.EW4_note              \n";
                sSql += ",c.EW5_note              \n";
                sSql += ",c.EW6_note              \n";
                sSql += ",c.EW7_note              \n";
                sSql += ",c.EW8_note              \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " left join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lubid in (select org_lubid as lubid from  PDFTAG.dbo.UA_BOM where luhid = '" + luhid + "' )   \n";
                sSql += " order by a.lubid,a.rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtUA_BOM_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtUA_BOM_Org);
                }

                //sSql = "select * \n";
                //sSql += "from PDFTAG.dbo.UA_BOMGarmentcolor a              \n";
                //sSql += " where a.luhid = '" + luhid + "'   \n";
                //Response.Write("<!--" + sSql + "-->");
                //cm.CommandText = sSql;
                //DataTable dtUA_BOMGarmentcolor = new DataTable();
                //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                //{
                //    da.Fill(dtUA_BOMGarmentcolor);
                //}

                var arrTypes = dt.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type"), COLOR_SET = s.Field<string>("COLOR_SET") }).Distinct().ToList();
                List<string> checkColorSet = new List<string>();

                foreach (var itemType in arrTypes)
                {
                    //sb.Append("<h4>" + itemType.type + "</h4>");
                    sb.Append("<table class='table table-hover'>");
                    if (!checkColorSet.Contains(itemType.COLOR_SET))
                    {
                        sb.Append("<tr>");
                        sb.Append(" <th scope='col'>" + itemType.COLOR_SET + "</th>");
                        sb.Append("</tr>");
                        checkColorSet.Add(itemType.COLOR_SET);
                    }
                    sb.Append("<tr>");
                    //sb.Append(" <th scope='col'>Standard Placement</th>");
                    sb.Append(" <th scope='col'>" + itemType.type + "</th>");
                    sb.Append(" <th scope='col'>Usage</th>");
                    sb.Append(" <th scope='col'>QTY</th>");
                    //sb.Append(" <th scope='col'>Supplier Article</th>");
                    //sb.Append(" <th scope='col'>Supplier</th>");

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.UA_BOMGarmentcolor a              \n";
                    sSql += " where a.lubcid = '" + itemType.lubcid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtUA_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtUA_BOMGarmentcolor);
                    }
                    List<string> arrColorHeader = new List<string>();
                    int iColorCnt = 0;
                    if (dtUA_BOMGarmentcolor.Rows.Count > 0)
                    {

                        for (int i = 1; i <= 10; i++)
                        {
                            string sA = dtUA_BOMGarmentcolor.Rows[0]["A" + i].ToString();
                            if (!string.IsNullOrEmpty(sA))
                            {
                                sb.Append(" <th scope='col'>" + sA + "</th>");
                                arrColorHeader.Add(sA);
                                iColorCnt++;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    sb.Append(" <th scope='col'></th>");
                    sb.Append("</tr>");

                    DataRow[] drBoms = dt.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "'", "org_lubid asc");
                    int idx = 0;
                    foreach (DataRow drBom in drBoms)
                    {
                        string lubid = drBom["lubid"].ToString();
                        string org_lubid = drBom["org_lubid"].ToString();
                        bool isEdit = (drBom["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string type = drBom["type"].ToString();
                        string standardPlacement = drBom["StandardPlacement"].ToString();
                        string usage = drBom["Usage"].ToString();
                        string supplierArticle = drBom["SupplierArticle"].ToString();
                        string supplierArticle2 = "";
                        string QTY = drBom["QTY"].ToString();

                        if (type == "Fabric")
                            supplierArticle2 = drBom["W4"].ToString();
                        else
                            supplierArticle2 = drBom["W1"].ToString();


                        //bool IsMappingSupplierArticle = (drBom["IsMappingSupplierArticle"].ToString().ToLower() == "true" ? true : false);
                        bool IsMappingSupplierArticle = false;
                        string supplier = drBom["Supplier"].ToString();

                        string PARTS_TYPE = drBom["PARTS_TYPE"].ToString();
                        string PARTS_CODE = drBom["PARTS_CODE"].ToString();
                        string PARTS_DESC = drBom["PARTS_DESC"].ToString();
                        string MAT_ID = drBom["MAT_ID"].ToString();

                        DataRow[] drOrgBoms = dtUA_BOM_Org.Select("lubid='" + org_lubid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgBoms.Length > 0)
                            {
                                DataRow drOrgBom = drOrgBoms[0];

                                supplierArticle = drOrgBom["supplierArticle"].ToString();
                                standardPlacement = Compare(drOrgBom["StandardPlacement"].ToString(), standardPlacement, FilterNote(arrNotes, lubid, "StandardPlacement"));
                                usage = Compare(drOrgBom["usage"].ToString(), usage, FilterNote(arrNotes, lubid, "usage"));
                                QTY = Compare(drOrgBom["QTY"].ToString(), QTY, FilterNote(arrNotes, lubid, "QTY"));

                                string org_supplierArticle2 = "";
                                if (drOrgBom["type"].ToString() == "Fabric")
                                    org_supplierArticle2 = drOrgBom["W4"].ToString();
                                else
                                    org_supplierArticle2 = drOrgBom["W1"].ToString();

                                if (false && string.IsNullOrEmpty(drBom["EW4"].ToString()))
                                {

                                    //if (drOrgBom["supplierArticle"].ToString() == supplierArticle)
                                    if (org_supplierArticle2 == supplierArticle2)
                                    {
                                        sSql = @"select distinct MAT_NO,MAT_NAME from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] where UPPER(MAT_NO)=@MAT_NO";
                                        Response.Write("<!--" + sSql + "-->");
                                        cm.CommandText = sSql;
                                        cm.Parameters.Clear();
                                        cm.Parameters.AddWithValue("@MAT_NO", supplierArticle2);
                                        DataTable dtSAM_MAT_DEF = new DataTable();
                                        using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                                        {
                                            da.Fill(dtSAM_MAT_DEF);
                                        }

                                        //20240219
                                        supplierArticle = Compare(org_supplierArticle2, supplierArticle2, FilterNote(arrNotes, lubid, "supplierArticle"));

                                        if (dtSAM_MAT_DEF.Rows.Count > 0)
                                        {
                                            //supplierArticle = Compare(drOrgBom["supplierArticle"].ToString(), supplierArticle, FilterNote(arrNotes, lubid, "supplierArticle"));
                                            if (string.IsNullOrEmpty(drOrgBom["EW4"].ToString()))
                                                supplierArticle = supplierArticle + "<br><font color='red' data-id='1'>修:" + dtSAM_MAT_DEF.Rows[0]["MAT_NO"].ToString() + "</font>";
                                        }
                                        else
                                            supplierArticle = supplierArticle + "<br><font color='red' data-id='1'>MAT_NO:無對應</font>";
                                    }
                                    else
                                    {
                                        //supplierArticle = Compare(org_supplierArticle2, supplierArticle2, FilterNote(arrNotes, lubid, "supplierArticle, IsMappingSupplierArticle"));

                                        string newText = supplierArticle2;

                                        supplierArticle = "<font>原:" + supplierArticle + "</font><br><font color='red'>修:" + newText + "</font><br><font color='blue'>中:" + FilterNote(arrNotes, lubid, "supplierArticle, IsMappingSupplierArticle") + "</font>";
                                    }
                                }
                                else
                                {
                                    #region 表示有修改 UA_TagData

                                    List<string> arrText = new List<string>();
                                    List<string> arrNote = new List<string>();
                                    int iWCnt = 8;
                                    if (type == "Fabric" || type == "Hangtag") iWCnt = 6;
                                    else if (type == "Embellishment") iWCnt = 7;

                                    for (int i = 1; i <= iWCnt; i++)
                                    {
                                        string sW = drBom["W" + i].ToString();
                                        string sEWNote = drBom["EW" + i + "_note"].ToString();
                                        string sCW = drOrgBom["W" + i].ToString();

                                        if (sW.Trim().Replace(" ", "") != sCW.Trim().Replace(" ", ""))
                                        {
                                            arrText.Add("<font color='red'>" + sW + "</font>");
                                        }
                                        else
                                            arrText.Add(sW);

                                        if (!string.IsNullOrEmpty(sEWNote) && sEWNote != "null")
                                            arrNote.Add(sEWNote);
                                    }
                                    if (arrText.Any(x => x.Contains("color='red'")))
                                    {
                                        supplierArticle = supplierArticle + "<br><font color='red' data-id='3'>修:</font>" + string.Join(" / ", arrText) + "";
                                        //sb.Append("<div style='width:250px'><span class='span_w'>" + string.Join(" / ", arrText) + "</span></div>");

                                        //20240201中文備註改成多筆
                                    }
                                    if (arrNote.Any())
                                        supplierArticle += "<br><font color='blue' data-id='3'>中:" + string.Join(" / ", arrNote) + "</font>";

                                    #endregion
                                }




                                //supplierArticle = Compare(drOrgBom["supplierArticle"].ToString(), supplierArticle, FilterNote(arrNotes, lubid, "supplierArticle"), IsMappingSupplierArticle);
                                supplier = Compare(drOrgBom["supplier"].ToString(), supplier, FilterNote(arrNotes, lubid, "supplier"));
                            }

                        }


                        if (!isEdit && !IsMappingSupplierArticle)
                        {
                            //Response.Write("<!--supplierArticle=" + supplierArticle + " IsMappingSupplierArticle=" + IsMappingSupplierArticle + "-->");

                            sSql = @"select distinct MAT_NO,MAT_NAME from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] where UPPER(MAT_NO)=@MAT_NO";
                            Response.Write("<!--" + sSql + "-->");
                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@MAT_NO", supplierArticle2);
                            DataTable dtSAM_MAT_DEF = new DataTable();
                            using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                            {
                                da.Fill(dtSAM_MAT_DEF);
                            }
                            if (dtSAM_MAT_DEF.Rows.Count > 0)
                            {
                                //supplierArticle += " <br><font color='red' data-id='2'>MAT_NO :" + dtSAM_MAT_DEF.Rows[0]["MAT_NO"].ToString() + "</font>";
                            }
                            else
                            {
                                //20230117 有對應到的MAT_NO料號(不用顯示)。沒有找到MAT_NO料號，需顯示 MAT_NO:無對應
                                supplierArticle += " <br><font color='red' data-id='2'>MAT_NO:無對應</font>";
                            }

                            //if (drOrgBoms.Length > 0)
                            //{
                            //	DataRow drOrgBom = drOrgBoms[0];

                            //	supplierArticle += "<br><font color='red' data-id='2'>修:無對應</font>"; ;
                            //}

                        }

                        string parts = " data-type='" + itemType.type + "' data-PARTS_TYPE='" + PARTS_TYPE + "' data-PARTS_CODE='" + PARTS_CODE + "' data-PARTS_DESC='" + PARTS_DESC + "'  data-MAT_ID='" + MAT_ID + "' ";

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drBom["rowid"].ToString() + "' id='row" + iSeq + "'>");
                        //sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement' onclick='editBom(this)'>" + standardPlacement + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='SupplierArticle' onclick='editBom(this)'>" + supplierArticle + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='Usage' onclick='editBom(this)'>" + usage + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='QTY' onclick='editBom(this)'>" + QTY + "</td>");
                        //sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier' onclick='editBom(this)'>" + supplier + "</td>");

                        for (int i = 1; i <= iColorCnt; i++)
                        {
                            string colorHeader = arrColorHeader[i - 1];
                            string color = drBom["B" + i].ToString();
                            if (isEdit)
                            {
                                if (drOrgBoms.Length > 0)
                                {
                                    DataRow drOrgBom = drOrgBoms[0];
                                    color = Compare(drOrgBom["B" + i].ToString(), color, FilterNote(arrNotes, lubid, "B" + i));
                                }
                            }
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='B" + i + "' data-colheader='" + colorHeader + "' onclick='editBom(this)'>" + color + "</td>");
                        }

                        sb.Append(" <td scope='col'>");
                        sb.Append("   <input typ='button' class='btn btn-danger btn-sm' value='整筆刪除' onclick='deleteBom(" + lubid + "," + iSeq + ")' />");

                        //if (idx == drBoms.Length - 1)
                        sb.Append("   <input typ='button' class='btn btn-secondary btn-sm' value='整筆新增' onclick='copyBom(" + lubid + ")'/>");
                        sb.Append(" </td>");
                        sb.Append("</tr>");

                        if (!string.IsNullOrEmpty(PARTS_TYPE))
                        {
                            sb.Append("<tr data-rowid='" + drBom["rowid"].ToString() + "'>");
                            sb.Append(" <td scope='col'><font color='green'>" + PARTS_CODE + "</font></td>");
                            sb.Append(" <td scope='col'><font color='green'>" + PARTS_DESC + "</font></td>");
                            sb.Append(" <td scope='col'><font color='green'>" + MAT_ID + "</font></td>");
                            sb.Append(" <td scope='col'></td>");

                            for (int i = 1; i <= iColorCnt; i++)
                            {
                                sb.Append(" <td scope='col'></td>");
                            }
                            sb.Append(" <td scope='col'></td>");
                            sb.Append("</tr>");
                        }

                        idx++;
                    }


                    sb.Append("</table>");

                }



                #endregion

                #region UA_SizeTable

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.UA_SizeTable where luhid = @luhid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new UA_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid = '" + luhid + "'   \n";
                sSql += " order by lusthid,rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lustid in (select org_lustid as lustid from  PDFTAG.dbo.UA_SizeTable where luhid = '" + luhid + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtSizeTable_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtSizeTable_Org);
                }

                var arr_lusthids = dt.AsEnumerable().Select(s => s.Field<long>("lusthid")).Distinct().ToList();

                foreach (var lusthid in arr_lusthids)
                {
                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.UA_SizeTable_Header a              \n";
                    sSql += " where a.lusthid in ('" + lusthid + "')   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtUA_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtUA_SizeTable_Header);
                    }

                    sb.Append("<h4>Size Table</h4>");
                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>Code</th>");
                    sb.Append(" <th scope='col'>Description</th>");
                    //sb.Append(" <th scope='col'>Criticality</th>");
                    sb.Append(" <th scope='col'>Tol(-)</th>");
                    sb.Append(" <th scope='col'>Tol(+)</th>");
                    //sb.Append(" <th scope='col'>HTM Instruction</th>");

                    int iOtherCnt = 0;
                    if (dtUA_SizeTable_Header.Rows.Count > 0)
                    {
                        for (int i = 1; i <= 15; i++)
                        {
                            string sH = dtUA_SizeTable_Header.Rows[0]["H" + i].ToString();
                            if (!string.IsNullOrEmpty(sH))
                            {
                                sb.Append(" <th scope='col'>" + sH + "</th>");
                                iOtherCnt++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    sb.Append(" <th scope='col'></th>");
                    sb.Append("</tr>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col' colspan='" + (4 + iOtherCnt) + "'>" + dtUA_SizeTable_Header.Rows[0]["HeaderDesc"].ToString() + "</th>");
                    sb.Append("</tr>");

                    DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
                    int idx = 0;
                    foreach (DataRow drSizeTable in drSizeTables)
                    {
                        string lustid = drSizeTable["lustid"].ToString();
                        string org_lustid = drSizeTable["org_lustid"].ToString();
                        bool isEdit = (drSizeTable["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string Code = drSizeTable["Code"].ToString();
                        string Description = drSizeTable["Description"].ToString();
                        //string criticality = drSizeTable["Criticality"].ToString();
                        string tolA = drSizeTable["TolA"].ToString();
                        string tolB = drSizeTable["TolB"].ToString();
                        //string hTMInstruction = drSizeTable["HTMInstruction"].ToString();

                        DataRow[] drOrgSizeTables = dtSizeTable_Org.Select("lustid='" + org_lustid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgSizeTables.Length > 0)
                            {
                                DataRow drOrgSizeTable = drOrgSizeTables[0];

                                Code = Compare(drOrgSizeTable["Code"].ToString(), Code, FilterNote(arrNotes, lustid, "Code"));
                                Description = Compare(drOrgSizeTable["Description"].ToString(), Description, FilterNote(arrNotes, lustid, "Description"));
                                //criticality = Compare(drOrgSizeTable["criticality"].ToString(), criticality, FilterNote(arrNotes, lustid, "criticality"));
                                tolA = Compare(drOrgSizeTable["tolA"].ToString(), tolA, FilterNote(arrNotes, lustid, "tolA"));
                                tolB = Compare(drOrgSizeTable["tolB"].ToString(), tolB, FilterNote(arrNotes, lustid, "tolB"));
                                //hTMInstruction = Compare(drOrgSizeTable["hTMInstruction"].ToString(), hTMInstruction, FilterNote(arrNotes, lustid, "hTMInstruction"));
                            }

                        }

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drSizeTable["rowid"].ToString() + "' id='row" + iSeq + "'>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Code' onclick='editSizeTable(this)'>" + Code + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' onclick='editSizeTable(this)'>" + Description + "</td>");
                        //sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' onclick='editSizeTable(this)'>" + criticality + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' onclick='editSizeTable(this)'>" + tolA + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' onclick='editSizeTable(this)'>" + tolB + "</td>");
                        //sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='HTMInstruction' onclick='editSizeTable(this)'>" + hTMInstruction + "</td>");

                        for (int i = 1; i <= iOtherCnt; i++)
                        {
                            string other = drSizeTable["A" + i].ToString();
                            if (isEdit)
                            {
                                if (drOrgSizeTables.Length > 0)
                                {
                                    DataRow drOrgSizeTable = drOrgSizeTables[0];
                                    other = Compare(drOrgSizeTable["A" + i].ToString(), other, FilterNote(arrNotes, lustid, "A" + i));
                                }
                            }
                            double d = 0;
                            if (other.Contains("/") && Double.TryParse(other.Split('/')[1].Trim(), out d)) other = other.Replace(" ", "-");

                            sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='A" + i + "' onclick='editSizeTable(this)'>"
                                + other + "</td>");
                        }

                        sb.Append(" <td scope='col'>");
                        sb.Append("   <input typ='button' class='btn btn-danger btn-sm' value='整筆刪除' onclick='deleteSizeTable(" + lustid + "," + iSeq + ")' />");

                        //if (idx == drSizeTables.Length - 1)
                        sb.Append("   <input typ='button' class='btn btn-secondary btn-sm' value='整筆新增'  onclick='copySizeTable(" + lustid + ")'/>");
                        sb.Append(" </td>");
                        sb.Append("</tr>");

                        idx++;
                    }


                    sb.Append("</table>");

                }



                #endregion

            }

            divHeader.InnerHtml = sb.ToString();

        }
        catch (Exception err)
        {
            Response.Write("<!--" + err.ToString() + "-->");
        }
    }
    public class UA_Ch_Note
    {
        public long Id { get; set; }
        public string ColName { get; set; }

        public string Note { get; set; }
    }
    public string FilterNote(List<UA_Ch_Note> arrData, string id, string colName)
    {
        var res = arrData.FirstOrDefault(w => w.Id == long.Parse(id) && w.ColName.ToLower() == colName.ToLower());
        if (res == null) return "";

        return res.Note;
    }
    public string Compare(string org, string newText, string note, bool IsMapping = true)
    {
        if (org == newText)
            if (string.IsNullOrEmpty(note))
                return org;
            else
                return "<font>原:" + org + "</font><br><font color='blue'>中:" + note + "</font>";
        else
        {
            if (IsMapping)
                return "<font>原:" + org + "</font><br><font color='red'>修:" + newText + "</font><br><font color='blue'>中:" + note + "</font>";
            else
                return "<font color='red'>修:無對應</font>";
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



            sSql = "update PDFTAG.dbo.P_inProcess    \n";
            sSql += " set ptitle=@ptitle,pver=@pver \n";
            if (!string.IsNullOrEmpty(sFilePath))
            {
                sSql += " ,piuploadfile=@piuploadfile \n";
            }


            sSql += "where pipid=@pipid                          \n";


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                //cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                //cm.Parameters.AddWithValue("@ptitle", ptitle.Text);
                //cm.Parameters.AddWithValue("@pidate", dtNow);
                //if (!string.IsNullOrEmpty(sFilePath))
                //    cm.Parameters.AddWithValue("@piuploadfile", sFilePath);
                //cm.Parameters.AddWithValue("@pver", ddlpver.SelectedItem.Value);
                //cm.ExecuteNonQuery();




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

    protected void btnLearn_Click(object sender, EventArgs e)
    {
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        try
        {
            DateTime dtNow = DateTime.Now;
            string sFilePath = "";

            List<UA_LearnmgrItemDto> arrUA_LearnmgrItemDto = new List<UA_LearnmgrItemDto>();


            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_Header a              \n";
                sSql += " where 1=1 and a.isshow=0   \n";
                sSql += " and a.pipid = '" + hidpipid.Value + "'   \n";
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtLu_Header = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtLu_Header);
                }

                string style = dtLu_Header.Rows[0]["style"].ToString();

                using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
                {
                    sSql = "select * from PDftag.dbo.UA_LearnmgrItem where style=@style  \n";

                    arrUA_LearnmgrItemDto = cn.Query<UA_LearnmgrItemDto>(sSql, new { style = style }).ToList();
                }

                #region UA_TagData

                sSql = "select c.* \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " join PDFTAG.dbo.UA_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
                sSql += " join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + hidpipid.Value + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtUA_TagData = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtUA_TagData);
                }

                foreach (DataRow drUA_TagData in dtUA_TagData.Rows)
                {
                    string lubid = drUA_TagData["lubid"].ToString();
                    string utdid = drUA_TagData["utdid"].ToString();

                    string sEW = "";


                    for (int i = 1; i <= 8; i++)
                    {
                        string SubColName = "W" + i;
                        string sW = drUA_TagData[SubColName].ToString();

                        #region 比對詞彙

                        var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle" && x.SubColName == SubColName
                     && x.Termname_org.Trim() == sW.Trim().Replace(" ", "").ToLower());

                        if (res != null)
                        {
                            sEW = res.Termname;
                            string sEW_note = res.Ctermname;

                            sSql = "update PDFTAG.dbo.UA_TagData  set E" + SubColName + "=@E" + SubColName + " ";
                            if (!string.IsNullOrEmpty(sEW_note))
                                sSql += " ,E" + SubColName + "_note=@note ";
                            sSql += " where utdid=@utdid";

                            Response.Write("<!--20240221 " + sSql.Replace("@utdid", utdid) + "-->");
                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@utdid", utdid);
                            cm.Parameters.AddWithValue("@E" + SubColName, sEW);
                            cm.Parameters.AddWithValue("@note", sEW_note);
                            cm.ExecuteNonQuery();

                            sSql = "update PDFTAG.dbo.UA_BOM \n";
                            sSql += "set  isEdit=1           \n";
                            sSql += "where lubid=@lubid\n";
                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@lubid", lubid);
                            cm.ExecuteNonQuery();
                        }
                    }



                    #endregion
                }


                #endregion


                #region UA_BOM

                sSql = "select a.*,b.* \n";
                sSql += ",c.W1              \n";
                sSql += ",c.W2              \n";
                sSql += ",c.W3              \n";
                sSql += ",c.W4              \n";
                sSql += ",c.W5              \n";
                sSql += ",c.W6              \n";
                sSql += ",c.W7              \n";
                sSql += ",c.W8              \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " join PDFTAG.dbo.UA_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
                sSql += " left join PDFTAG.dbo.UA_TagData c on a.lubid=c.lubid               \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + hidpipid.Value + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtUA_BOM = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtUA_BOM);
                }

                foreach (DataRow drBom in dtUA_BOM.Rows)
                {
                    string lubid = drBom["lubid"].ToString();
                    string StandardPlacement = drBom["StandardPlacement"].ToString();
                    string Usage = drBom["Usage"].ToString();
                    string SupplierArticle = drBom["SupplierArticle"].ToString();
                    string Supplier = drBom["Supplier"].ToString();
                    string W1 = drBom["W1"].ToString();

                    string B1 = drBom["B1"].ToString();
                    string B2 = drBom["B2"].ToString();
                    string B3 = drBom["B3"].ToString();
                    string B4 = drBom["B4"].ToString();
                    string B5 = drBom["B5"].ToString();
                    string B6 = drBom["B6"].ToString();
                    string B7 = drBom["B7"].ToString();
                    string B8 = drBom["B8"].ToString();
                    string B9 = drBom["B9"].ToString();
                    string B10 = drBom["B10"].ToString();

                    string A1 = drBom["A1"].ToString();
                    string A2 = drBom["A2"].ToString();
                    string A3 = drBom["A3"].ToString();
                    string A4 = drBom["A4"].ToString();
                    string A5 = drBom["A5"].ToString();
                    string A6 = drBom["A6"].ToString();
                    string A7 = drBom["A7"].ToString();
                    string A8 = drBom["A8"].ToString();
                    string A9 = drBom["A9"].ToString();
                    string A10 = drBom["A10"].ToString();

                    bool isUpdate = false;

                    #region 比對詞彙

                    var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                     && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        StandardPlacement = res.Termname;
                        InsertUANote(cm, "lubid", lubid, "StandardPlacement", res.Ctermname);
                        isUpdate = true;
                    }

                    //res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                    // && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                    //if (res != null)
                    //{
                    //    Placement = res.Termname;
                    //    isUpdate = true;
                    //}

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                    && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        SupplierArticle = res.Termname;
                        InsertUANote(cm, "lubid", lubid, "SupplierArticle", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
                  && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Supplier = res.Termname;
                        InsertUANote(cm, "lubid", lubid, "Supplier", res.Ctermname);
                        isUpdate = true;
                    }


                    for (int a = 1; a <= 10; a++)
                    {
                        switch (a)
                        {
                            case 1:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A1
              && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B1 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B1", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                            case 2:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A2
              && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B2 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B2", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                            case 3:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A3
              && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B3 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B3", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                            case 4:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A4
              && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B4 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B4", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 5:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A5
              && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B5 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B5", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 6:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A6
              && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B6 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B6", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 7:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A7
              && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B7 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B7", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 8:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A8
              && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B8 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B8", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 9:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A9
              && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B9 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B9", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 10:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "GarmentColor"
                                 && x.Usage == Usage
                            && x.W1 == W1
                            && x.ColorName == A10
              && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B10 = res.Termname;
                                    InsertUANote(cm, "lubid", lubid, "B10", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                        }

                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.UA_BOM \n";
                        sSql += "set  StandardPlacement=@StandardPlacement,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                        sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=1           \n";
                        sSql += "where lubid=@lubid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                        //cm.Parameters.AddWithValue("@Placement", Placement);
                        cm.Parameters.AddWithValue("@SupplierArticle", SupplierArticle);
                        cm.Parameters.AddWithValue("@Supplier", Supplier);
                        cm.Parameters.AddWithValue("@B1", B1);
                        cm.Parameters.AddWithValue("@B2", B2);
                        cm.Parameters.AddWithValue("@B3", B3);
                        cm.Parameters.AddWithValue("@B4", B4);
                        cm.Parameters.AddWithValue("@B5", B5);
                        cm.Parameters.AddWithValue("@B6", B6);
                        cm.Parameters.AddWithValue("@B7", B7);
                        cm.Parameters.AddWithValue("@B8", B8);
                        cm.Parameters.AddWithValue("@B9", B9);
                        cm.Parameters.AddWithValue("@B10", B10);
                        cm.ExecuteNonQuery();
                    }
                }

                #endregion

                #region UA_SizeTable

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.UA_SizeTable a              \n";
                sSql += " join PDFTAG.dbo.UA_SizeTable_Header b on a.lusthid=b.lusthid              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.UA_Header where isshow=0 and pipid='" + hidpipid.Value + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtUA_SizeTable = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtUA_SizeTable);
                }

                foreach (DataRow drUA_SizeTable in dtUA_SizeTable.Rows)
                {
                    string lustid = drUA_SizeTable["lustid"].ToString();
                    string Code = drUA_SizeTable["Code"].ToString();
                    string Description = drUA_SizeTable["Description"].ToString();
                    string Criticality = drUA_SizeTable["Criticality"].ToString();

                    string A1 = drUA_SizeTable["A1"].ToString();
                    string A2 = drUA_SizeTable["A2"].ToString();
                    string A3 = drUA_SizeTable["A3"].ToString();
                    string A4 = drUA_SizeTable["A4"].ToString();
                    string A5 = drUA_SizeTable["A5"].ToString();
                    string A6 = drUA_SizeTable["A6"].ToString();
                    string A7 = drUA_SizeTable["A7"].ToString();
                    string A8 = drUA_SizeTable["A8"].ToString();
                    string A9 = drUA_SizeTable["A9"].ToString();
                    string A10 = drUA_SizeTable["A10"].ToString();
                    string A11 = drUA_SizeTable["A11"].ToString();
                    string A12 = drUA_SizeTable["A12"].ToString();
                    string A13 = drUA_SizeTable["A13"].ToString();
                    string A14 = drUA_SizeTable["A14"].ToString();
                    string A15 = drUA_SizeTable["A15"].ToString();

                    string H1 = drUA_SizeTable["H1"].ToString();
                    string H2 = drUA_SizeTable["H2"].ToString();
                    string H3 = drUA_SizeTable["H3"].ToString();
                    string H4 = drUA_SizeTable["H4"].ToString();
                    string H5 = drUA_SizeTable["H5"].ToString();
                    string H6 = drUA_SizeTable["H6"].ToString();
                    string H7 = drUA_SizeTable["H7"].ToString();
                    string H8 = drUA_SizeTable["H8"].ToString();
                    string H9 = drUA_SizeTable["H9"].ToString();
                    string H10 = drUA_SizeTable["H10"].ToString();
                    string H11 = drUA_SizeTable["H11"].ToString();
                    string H12 = drUA_SizeTable["H12"].ToString();
                    string H13 = drUA_SizeTable["H13"].ToString();
                    string H14 = drUA_SizeTable["H14"].ToString();
                    string H15 = drUA_SizeTable["H15"].ToString();

                    bool isUpdate = false;

                    #region 比對詞彙

                    var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Description"
                    && x.Code == Code
                                  && x.Termname_org == Description.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Description = res.Termname;
                        InsertUANote(cm, "lustid", lustid, "Description", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Criticality"
                    && x.Code == Code
                     && x.Termname_org == Criticality.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Criticality = res.Termname;
                        isUpdate = true;
                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.UA_SizeTable \n";
                        sSql += "set  Description=@Description,Criticality=@Criticality          \n";
                        sSql += "where lustid=@lustid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lustid", lustid);
                        cm.Parameters.AddWithValue("@Description", Description);
                        cm.Parameters.AddWithValue("@Criticality", Criticality);
                        cm.ExecuteNonQuery();
                    }
                }

                #endregion
            }

            script = "alert('完成!')";
        }
        catch (Exception err)
        {
            Response.Write("btnAdd_Click:" + err.ToString());
        }


        DataBind();
    }
    public void InsertUANote(System.Data.SqlClient.SqlCommand cm, string IdName, string lubid, string ColName, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;
        string sSql = @"IF NOT Exists (select * from PDFTAG.dbo.UA_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.UA_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.UA_Ch_Note
                                set note=@note
                             where IdName=@IdName and id=@id and ColName=@ColName
                             end ";

        cm.CommandText = sSql;
        cm.Parameters.Clear();
        cm.Parameters.AddWithValue("@IdName", IdName);
        cm.Parameters.AddWithValue("@id", lubid);
        cm.Parameters.AddWithValue("@ColName", ColName);
        cm.Parameters.AddWithValue("@note", note);
        cm.Parameters.AddWithValue("@creator", LoginUser.PK);
        cm.Parameters.AddWithValue("@createordate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
        cm.ExecuteNonQuery();
    }

}