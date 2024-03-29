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
using Aspose.Pdf;

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


        sSql += "select c.pipid,c.gaptype,b.pipid as copy_pipid,b.ptitle,b.pidate,b.piuploadfile,b.pver,b.creator,a.hdid,a.hisversion,a.editdate \n";
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

            hidgaptype.Value = dt.Rows[0]["gaptype"].ToString();
        }

    }



    private void DataBind()
    {
        bool _AllowPaging = true;
        SQLHelper sql = new SQLHelper();
        DataTable dt = new DataTable();
        string sSql = "";

        StringBuilder sb = new StringBuilder();

        string gaptype = hidgaptype.Value;

        try
        {
            sSql = "select * \n";
            sSql += "from PDFTAG.dbo.GAP_Header a              \n";
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
                sSql += "from PDFTAG.dbo.GAP_Header a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid in (select org_luhid as luhid from  PDFTAG.dbo.GAP_Header where pipid = '" + hidpipid.Value + "' )   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtGAP_Header_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_Header_Org);
                }


                List<GAP_Ch_Note> arrNotes = new List<GAP_Ch_Note>();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='luhid'  and a.Id in (select luhid as id from PDFTAG.dbo.GAP_Header where isshow=0 and pipid = @pipid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@pipid", hidpipid.Value);
                DataTable dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                int iSeq = 1;

                #region GAP_Header

                string season = dt.Rows[0]["Season"].ToString();
                string style = dt.Rows[0]["style"].ToString();
                string styledesc = dt.Rows[0]["styledesc"].ToString();
                string productsize = dt.Rows[0]["productsize"].ToString();
                string ProductDescription = dt.Rows[0]["ProductDescription"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string strClass = dt.Rows[0]["class"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();
                bool isEditHeader = (dt.Rows[0]["isEdit"].ToString().ToLower() == "true" ? true : false);

                hidStyle.Value = style;

                DataRow[] drOrgHeades = dtGAP_Header_Org.Select("luhid='" + org_luhid + "'");

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
                sb.Append(" <th scope='col'>Style</th>");
                sb.Append(" <th scope='col'>Style Desc</th>");
                sb.Append(" <th scope='col'>Product Description</th>");
                sb.Append(" <th scope='col'>Brand</th>");
                sb.Append(" <th scope='col'>Class</th>");
                sb.Append(" <th scope='col'>Style Status</th>");
                sb.Append(" <th scope='col'>Generated Date</th>");
                sb.Append(" <th scope='col'></th>");
                sb.Append("</tr>");
                sb.Append("<tr id='row" + iSeq + "'>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='Season' onclick='editHeader(this)'>" + season + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='style' onclick='editHeader(this)'>" + style + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='styledesc' onclick='editHeader(this)'>" + styledesc + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='productdescription' onclick='editHeader(this)'>" + ProductDescription + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='brand' onclick='editHeader(this)'>" + brand + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='class' onclick='editHeader(this)'>" + strClass + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='stylestatus' onclick='editHeader(this)'>" + stylestatus + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='generateddate' onclick='editHeader(this)'>" + generateddate + "</td>");

                sb.Append(" <td scope='col'>");
                sb.Append("   <input typ='button' class='btn btn-danger btn-sm' value='整筆刪除' onclick='deleteHeader(" + luhid + "," + iSeq + ")' />");
                sb.Append("   <input typ='button' class='btn btn-secondary btn-sm' value='整筆新增'  onclick='copyHeader(" + luhid + ")'/>");
                sb.Append(" </td>");
                sb.Append("</tr>");
                sb.Append("</table>");

                #endregion

                #region GAP_BOM

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='lubid'  and a.Id in (select lubid as id from PDFTAG.dbo.GAP_BOM where luhid = @luhid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();


                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_BOM a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid = '" + luhid + "'   \n";
                //sSql += " order by lubid,rowid asc    \n";
                sSql += "  order by (CASE WHEN org_lubid is null THEN lubid ELSE org_lubid END) asc ,rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                dt = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dt);
                }

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_BOM a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lubid in (select org_lubid as lubid from  PDFTAG.dbo.GAP_BOM where luhid = '" + luhid + "' )   \n";
                sSql += " order by lubid,rowid asc    \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                DataTable dtGAP_BOM_Org = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_BOM_Org);
                }

                //sSql = "select * \n";
                //sSql += "from PDFTAG.dbo.GAP_BOMGarmentcolor a              \n";
                //sSql += " where a.luhid = '" + luhid + "'   \n";
                //Response.Write("<!--" + sSql + "-->");
                //cm.CommandText = sSql;
                //DataTable dtGAP_BOMGarmentcolor = new DataTable();
                //using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                //{
                //    da.Fill(dtGAP_BOMGarmentcolor);
                //}

                var arrTypes = dt.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type") }).Distinct().ToList();

                foreach (var itemType in arrTypes)
                {
                    StringBuilder sbHeader = new StringBuilder();

                    sbHeader.Append("<h4>" + itemType.type + "</h4>");
                    sbHeader.Append("<table class='table table-hover'>");
                    sbHeader.Append("<tr>");

                    if (gaptype == "2")
                    {

                        sbHeader.Append(" <th scope='col'>Product</th>");
                        sbHeader.Append(" <th scope='col'>Usage</th>");
                        sbHeader.Append(" <th scope='col'>Supplier Article Number</th>");
                        sbHeader.Append(" <th scope='col'>Quality Details</th>");
                        sbHeader.Append(" <th scope='col'>Supplier[Allocate]</th>");
                    }
                    else
                    {
                        sbHeader.Append(" <th scope='col'>Standard Placement</th>");
                        sbHeader.Append(" <th scope='col'>Placement</th>");
                        sbHeader.Append(" <th scope='col'>Supplier / Supplier Article</th>");
                    }

                    sSql = "select * \n";
                    sSql += "from PDFTAG.dbo.GAP_BOMGarmentcolor a              \n";
                    sSql += " where a.lubcid = '" + itemType.lubcid + "'   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtGAP_BOMGarmentcolor = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtGAP_BOMGarmentcolor);
                    }
                    List<string> arrColorHeader = new List<string>();
                    int iColorCnt = 0;
                    if (dtGAP_BOMGarmentcolor.Rows.Count > 0)
                    {

                        for (int i = 1; i <= 10; i++)
                        {
                            string sA = dtGAP_BOMGarmentcolor.Rows[0]["A" + i].ToString();
                            if (!string.IsNullOrEmpty(sA))
                            {
                                sbHeader.Append(" <th scope='col'>" + sA + "</th>");
                                arrColorHeader.Add(sA);
                                iColorCnt++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    sbHeader.Append(" <th scope='col'></th>");
                    sbHeader.Append("</tr>");

                    if (iColorCnt == 0)
                        continue;

                    sb.Append(sbHeader);

                    DataRow[] drBoms = dt.Select("type='" + itemType.type + "' and lubcid='" + itemType.lubcid + "'", "org_lubid asc");
                    int idx = 0;
                    foreach (DataRow drBom in drBoms)
                    {
                        string lubid = drBom["lubid"].ToString();
                        string org_lubid = drBom["org_lubid"].ToString();
                        bool isEdit = (drBom["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string standardPlacement = drBom["StandardPlacement"].ToString();//Product
                        string usage = drBom["Usage"].ToString();
                        string supplierArticle = drBom["SupplierArticle"].ToString();
                        //bool IsMappingSupplierArticle = (drBom["IsMappingSupplierArticle"].ToString().ToLower() == "true" ? true : false);
                        bool IsMappingSupplierArticle = false;
                        string supplier = drBom["Supplier"].ToString();
                        string QualityDetails = drBom["QualityDetails"].ToString();


                        string PARTS_TYPE = drBom["PARTS_TYPE"].ToString();
                        string PARTS_CODE = drBom["PARTS_CODE"].ToString();
                        string PARTS_DESC = drBom["PARTS_DESC"].ToString();
                        string MAT_ID = drBom["MAT_ID"].ToString();

                        DataRow[] drOrgBoms = dtGAP_BOM_Org.Select("lubid='" + org_lubid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgBoms.Length > 0)
                            {
                                DataRow drOrgBom = drOrgBoms[0];

                                standardPlacement = Compare(drOrgBom["StandardPlacement"].ToString(), standardPlacement, FilterNote(arrNotes, lubid, "StandardPlacement"));
                                usage = Compare(drOrgBom["usage"].ToString(), usage, FilterNote(arrNotes, lubid, "usage"));
                                supplierArticle = Compare(drOrgBom["supplierArticle"].ToString(), supplierArticle, FilterNote(arrNotes, lubid, "supplierArticle"));
                                supplier = Compare(drOrgBom["supplier"].ToString(), supplier, FilterNote(arrNotes, lubid, "supplier"));
                                QualityDetails = Compare(drOrgBom["QualityDetails"].ToString(), QualityDetails, FilterNote(arrNotes, lubid, "QualityDetails"));
                            }

                        }


                        if (!isEdit && !IsMappingSupplierArticle)
                        {
                            //Response.Write("<!--supplierArticle=" + supplierArticle + " IsMappingSupplierArticle=" + IsMappingSupplierArticle + "-->");

                            sSql = @"select distinct MAT_NO,MAT_NAME from [RD_SAMPLE].[dbo].[SAM_MAT_DEF] where UPPER(MAT_NO)=@MAT_NO";
                            Response.Write("<!--" + sSql + "-->");
                            cm.CommandText = sSql;
                            cm.Parameters.Clear();
                            cm.Parameters.AddWithValue("@MAT_NO", supplierArticle);
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
                                //20230130 有對應到的MAT_NO料號(不用顯示)。沒有找到MAT_NO料號，需顯示 MAT_NO:無對應
                                supplierArticle += " <br><font color='red' data-id='2'>MAT_NO:無對應</font>";
                            }
                        }

                        string parts = " data-PARTS_TYPE='" + PARTS_TYPE + "' data-PARTS_CODE='" + PARTS_CODE + "' data-PARTS_DESC='" + PARTS_DESC + "'  data-MAT_ID='" + MAT_ID + "' ";

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drBom["rowid"].ToString() + "' id='row" + iSeq + "'>");

                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement' onclick='editBom(this)'>" + standardPlacement + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='usage' onclick='editBom(this)'>" + usage + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='SupplierArticle' onclick='editBom(this)'>" + supplierArticle + "</td>");

                        if (gaptype == "2")
                        {
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='QualityDetails' onclick='editBom(this)'>" + QualityDetails + "</td>");
                            sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='supplier' onclick='editBom(this)'>" + supplier + "</td>");

                        }





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

                #region GAP_SizeTable

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_Ch_Note  a             \n";
                sSql += "where 1=1 \n";
                sSql += " and a.IdName='lustid'  and a.Id in (select lustid as id from PDFTAG.dbo.GAP_SizeTable where luhid = @luhid ) \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                cm.Parameters.AddWithValue("@luhid", luhid);
                dtNote = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtNote);
                }

                arrNotes = dtNote.AsEnumerable().Select(s => new GAP_Ch_Note { Id = s.Field<long>("Id"), ColName = s.Field<string>("ColName"), Note = s.Field<string>("Note") }).ToList();

                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_SizeTable a              \n";
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
                sSql += "from PDFTAG.dbo.GAP_SizeTable a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lustid in (select org_lustid as lustid from  PDFTAG.dbo.GAP_SizeTable where luhid = '" + luhid + "' )   \n";
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
                    sSql += "from PDFTAG.dbo.GAP_SizeTable_Header a              \n";
                    sSql += " where a.lusthid in ('" + lusthid + "')   \n";
                    Response.Write("<!--" + sSql + "-->");
                    cm.CommandText = sSql;
                    DataTable dtGAP_SizeTable_Header = new DataTable();
                    using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                    {
                        da.Fill(dtGAP_SizeTable_Header);
                    }

                    sb.Append("<h4>Size Table</h4>");
                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>POM</th>");
                    sb.Append(" <th scope='col'>Description</th>");
                    //sb.Append(" <th scope='col'>AddlComments</th>");
                    sb.Append(" <th scope='col'>POM Variation</th>");
                    sb.Append(" <th scope='col'>Tol(-)</th>");
                    sb.Append(" <th scope='col'>Tol(+)</th>");
                    //sb.Append(" <th scope='col'>Variation</th>");

                    int iOtherCnt = 0;
                    if (dtGAP_SizeTable_Header.Rows.Count > 0)
                    {
                        for (int i = 1; i <= 15; i++)
                        {
                            string sH = dtGAP_SizeTable_Header.Rows[0]["H" + i].ToString();
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

                    DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
                    int idx = 0;
                    foreach (DataRow drSizeTable in drSizeTables)
                    {
                        string lustid = drSizeTable["lustid"].ToString();
                        string org_lustid = drSizeTable["org_lustid"].ToString();
                        bool isEdit = (drSizeTable["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string POM = drSizeTable["POM"].ToString();
                        string POMNote = drSizeTable["POMNote"].ToString();
                        string Description = drSizeTable["Description"].ToString();
                        string AddlComments = drSizeTable["AddlComments"].ToString();
                        string tolA = drSizeTable["TolA"].ToString();
                        string tolB = drSizeTable["TolB"].ToString();
                        string Variation = drSizeTable["Variation"].ToString();

                        DataRow[] drOrgSizeTables = dtSizeTable_Org.Select("lustid='" + org_lustid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgSizeTables.Length > 0)
                            {
                                DataRow drOrgSizeTable = drOrgSizeTables[0];

                                POM = Compare(drOrgSizeTable["POM"].ToString(), POM, FilterNote(arrNotes, lustid, "POM"));
                                Description = Compare(drOrgSizeTable["Description"].ToString(), Description, FilterNote(arrNotes, lustid, "Description"));
                                AddlComments = Compare(drOrgSizeTable["AddlComments"].ToString(), AddlComments, FilterNote(arrNotes, lustid, "AddlComments"));
                                tolA = Compare(drOrgSizeTable["tolA"].ToString(), tolA, FilterNote(arrNotes, lustid, "tolA"));
                                tolB = Compare(drOrgSizeTable["tolB"].ToString(), tolB, FilterNote(arrNotes, lustid, "tolB"));
                                Variation = Compare(drOrgSizeTable["Variation"].ToString(), Variation, FilterNote(arrNotes, lustid, "Variation"));
                            }

                        }

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drSizeTable["rowid"].ToString() + "' id='row" + iSeq + "'>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='POM' onclick='editSizeTable(this)'>" + POM + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' onclick='editSizeTable(this)'>" + Description + "</td>");
                        //sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' onclick='editSizeTable(this)'>" + AddlComments + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Variation' onclick='editSizeTable(this)'>" + Variation + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' onclick='editSizeTable(this)'>" + tolA + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' onclick='editSizeTable(this)'>" + tolB + "</td>");
                        //sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Variation' onclick='editSizeTable(this)'>" + Variation + "</td>");

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

                        if (!string.IsNullOrEmpty(POMNote))
                        {
                            sb.Append("<tr>");
                            sb.Append("<td colspan='" + (5 + iOtherCnt) + "'>   -> " + POMNote + "</td>");
                            sb.Append("</tr>");
                        }


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
    public class GAP_Ch_Note
    {
        public long Id { get; set; }
        public string ColName { get; set; }

        public string Note { get; set; }
    }
    public string FilterNote(List<GAP_Ch_Note> arrData, string id, string colName)
    {
        var res = arrData.FirstOrDefault(w => w.Id == long.Parse(id) && w.ColName.ToLower() == colName.ToLower());
        if (res == null) return "";

        return res.Note;
    }
    public string Compare(string org, string newText, string note, bool IsMapping = true)
    {
        if (org == newText)
        {
            if (string.IsNullOrEmpty(note))
                return org;
            else
                return org + "<br><font color='blue'>中:" + note + "</font>";
        }
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

            List<GAP_LearnmgrItemDto> arrGAP_LearnmgrItemDto = new List<GAP_LearnmgrItemDto>();



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.GAP_Header a              \n";
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
                    sSql = "select * from PDftag.dbo.GAP_LearnmgrItem where style=@style  \n";

                    Response.Write("<!--btnLearn_Click:" + sSql.Replace("@style", "'" + style + "'") + "-->");

                    arrGAP_LearnmgrItemDto = cn.Query<GAP_LearnmgrItemDto>(sSql, new { style = style }).ToList();

                    //LogFile.Logger.Log(Newtonsoft.Json.JsonConvert.SerializeObject(arrGAP_LearnmgrItemDto));
                }

                #region GAP_BOM

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.GAP_BOM a              \n";
                sSql += " join PDFTAG.dbo.GAP_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.GAP_Header where isshow=0 and pipid='" + hidpipid.Value + "')   \n";
                Response.Write("<!--btnLearn_Click:" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtGAP_BOM = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_BOM);
                }

                foreach (DataRow drBom in dtGAP_BOM.Rows)
                {
                    string lubid = drBom["lubid"].ToString();
                    string StandardPlacement = drBom["StandardPlacement"].ToString();
                    string Usage = drBom["Usage"].ToString();
                    string SupplierArticle = drBom["SupplierArticle"].ToString();
                    string Supplier = drBom["Supplier"].ToString();

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

                    var res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                     && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        StandardPlacement = res.Termname;
                        InsertGapNote(cm, "lubid", lubid, "StandardPlacement", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Usage"
                     && x.Termname_org == Usage.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Usage = res.Termname;
                        InsertGapNote(cm, "lubid", lubid, "Usage", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                    && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        SupplierArticle = res.Termname;
                        InsertGapNote(cm, "lubid", lubid, "SupplierArticle", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
                  && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Supplier = res.Termname;
                        InsertGapNote(cm, "lubid", lubid, "Supplier", res.Ctermname);
                        isUpdate = true;
                    }


                    for (int a = 1; a <= 10; a++)
                    {

                        switch (a)
                        {
                            case 1:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                && x.ColorName == A1
              && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B1 = res.Termname;
                                    //Response.Write("<!--btnLearn_Click: lubid=" + lubid + "  " + res.Ctermname + "-->");
                                    InsertGapNote(cm, "lubid", lubid, "B1", res.Ctermname);
                                    isUpdate = true;
                                }
                                //if (lubid == "2359")
                                //{
                                //    Response.Write("<!--btnLearn_Click:lubid=" + lubid + " B1=" + B1 + "SupplierArticle=" + SupplierArticle + "Usage=" + Usage + " ColorName='" + A1 + "' B1=" + B1.Trim().Replace(" ", "").ToLower() + " res=" + (res == null) + "-->");

                                //    var arr = arrGAP_LearnmgrItemDto.Where(x => x.ColSource == "BOM"
                                //&& x.ColName == "GarmentColor").ToList();
                                //    LogFile.Logger.Log(Newtonsoft.Json.JsonConvert.SerializeObject(arr));
                                //    LogFile.Logger.Log("SupplierArticle='" + SupplierArticle + "' Usage='" + Usage + "' ColorName='" + A1 + "' Termname_org='" + B1.Trim().Replace(" ", "").ToLower() + "'");
                                //}


                                break;

                            case 2:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                && x.ColorName == A2
              && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B2 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B2", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                            case 3:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                && x.ColorName == A3
              && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B3 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B3", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                            case 4:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A4
              && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B4 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B4", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 5:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A5
              && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B5 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B5", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 6:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A6
              && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B6 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B6", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 7:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A7
              && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B7 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B7", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 8:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A8
              && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B8 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B8", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 9:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A9
              && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B9 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B9", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;
                            case 10:
                                res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM"
                                && x.ColName == "GarmentColor"
                                 && x.SupplierArticleNumber == SupplierArticle
                                && x.Usage == Usage
                                 && x.ColorName == A10
              && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B10 = res.Termname;
                                    InsertGapNote(cm, "lubid", lubid, "B10", res.Ctermname);
                                    isUpdate = true;
                                }
                                break;

                        }

                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.GAP_BOM \n";
                        sSql += "set  StandardPlacement=@StandardPlacement,Usage=@Usage,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                        sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=1           \n";
                        sSql += "where lubid=@lubid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                        cm.Parameters.AddWithValue("@Usage", Usage);
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

                #region GAP_SizeTable

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.GAP_SizeTable a              \n";
                sSql += " join PDFTAG.dbo.GAP_SizeTable_Header b on a.lusthid=b.lusthid              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.luhid=(select luhid from PDFTAG.dbo.GAP_Header where isshow=0 and pipid='" + hidpipid.Value + "')   \n";
                Response.Write("<!--" + sSql + "-->");
                cm.CommandText = sSql;
                cm.Parameters.Clear();
                DataTable dtGAP_SizeTable = new DataTable();
                using (System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter(cm))
                {
                    da.Fill(dtGAP_SizeTable);
                }

                foreach (DataRow drGAP_SizeTable in dtGAP_SizeTable.Rows)
                {
                    string lustid = drGAP_SizeTable["lustid"].ToString();
                    string POM = drGAP_SizeTable["POM"].ToString();
                    string Description = drGAP_SizeTable["Description"].ToString();

                    string A1 = drGAP_SizeTable["A1"].ToString();
                    string A2 = drGAP_SizeTable["A2"].ToString();
                    string A3 = drGAP_SizeTable["A3"].ToString();
                    string A4 = drGAP_SizeTable["A4"].ToString();
                    string A5 = drGAP_SizeTable["A5"].ToString();
                    string A6 = drGAP_SizeTable["A6"].ToString();
                    string A7 = drGAP_SizeTable["A7"].ToString();
                    string A8 = drGAP_SizeTable["A8"].ToString();
                    string A9 = drGAP_SizeTable["A9"].ToString();
                    string A10 = drGAP_SizeTable["A10"].ToString();
                    string A11 = drGAP_SizeTable["A11"].ToString();
                    string A12 = drGAP_SizeTable["A12"].ToString();
                    string A13 = drGAP_SizeTable["A13"].ToString();
                    string A14 = drGAP_SizeTable["A14"].ToString();
                    string A15 = drGAP_SizeTable["A15"].ToString();

                    string H1 = drGAP_SizeTable["H1"].ToString();
                    string H2 = drGAP_SizeTable["H2"].ToString();
                    string H3 = drGAP_SizeTable["H3"].ToString();
                    string H4 = drGAP_SizeTable["H4"].ToString();
                    string H5 = drGAP_SizeTable["H5"].ToString();
                    string H6 = drGAP_SizeTable["H6"].ToString();
                    string H7 = drGAP_SizeTable["H7"].ToString();
                    string H8 = drGAP_SizeTable["H8"].ToString();
                    string H9 = drGAP_SizeTable["H9"].ToString();
                    string H10 = drGAP_SizeTable["H10"].ToString();
                    string H11 = drGAP_SizeTable["H11"].ToString();
                    string H12 = drGAP_SizeTable["H12"].ToString();
                    string H13 = drGAP_SizeTable["H13"].ToString();
                    string H14 = drGAP_SizeTable["H14"].ToString();
                    string H15 = drGAP_SizeTable["H15"].ToString();

                    bool isUpdate = false;

                    #region 比對詞彙

                    var res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "POM"
                                  && x.Pom == POM
                                  && x.Termname_org == POM.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        POM = res.Termname;
                        InsertGapNote(cm, "lustid", lustid, "POM", res.Ctermname);
                        isUpdate = true;
                    }

                    res = arrGAP_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Description"
                     && x.Pom == POM
                     && x.Termname_org == Description.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Description = res.Termname;
                        InsertGapNote(cm, "lustid", lustid, "Description", res.Ctermname);
                        isUpdate = true;
                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.GAP_SizeTable \n";
                        sSql += "set  POM=@POM,Description=@Description,isEdit=1          \n";
                        sSql += "where lustid=@lustid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lustid", lustid);
                        cm.Parameters.AddWithValue("@POM", POM);
                        cm.Parameters.AddWithValue("@Description", Description);
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
    public void InsertGapNote(System.Data.SqlClient.SqlCommand cm, string IdName, string lubid, string ColName, string note)
    {
        if (string.IsNullOrEmpty(note))
            return;
        string sSql = @"IF NOT Exists (select * from PDFTAG.dbo.GAP_Ch_Note where IdName=@IdName and id=@id and ColName=@ColName )
                             begin
                                  insert into PDFTAG.dbo.GAP_Ch_Note
                                   (IdName,id,ColName,note,creator,createordate)
                                    values 
                              (@IdName,@id,@ColName,@note,@creator,@createordate)
                              end 
                             else 
                                begin
                                update PDFTAG.dbo.GAP_Ch_Note
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