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
                string productsize = dt.Rows[0]["productsize"].ToString();
                string productfamily = dt.Rows[0]["class"].ToString();
                string brand = dt.Rows[0]["brand"].ToString();
                string dividion = dt.Rows[0]["dividion"].ToString();
                string pod = dt.Rows[0]["pod"].ToString();
                string stylestatus = dt.Rows[0]["stylestatus"].ToString();
                string generateddate = dt.Rows[0]["generateddate"].ToString();
                bool isEditHeader = (dt.Rows[0]["isEdit"].ToString().ToLower() == "true" ? true : false);

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
                sb.Append(" <th scope='col'>Product Size</th>");
                sb.Append(" <th scope='col'>Product Family</th>");
                sb.Append(" <th scope='col'>Style Status</th>");
                sb.Append(" <th scope='col'></th>");
                sb.Append("</tr>");
                sb.Append("<tr id='row" + iSeq + "'>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='Season' onclick='editHeader(this)'>" + season + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='style' onclick='editHeader(this)'>" + style + "</td>");
                sb.Append(" <td scope='col'  data-luhid='" + luhid + "' data-org_luhid='" + org_luhid + "'data-col='styledesc' onclick='editHeader(this)'>" + styledesc + "</td>");
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


                sSql = "select * \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
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
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " where 1=1   \n";
                sSql += " and a.lubid in (select org_lubid as lubid from  PDFTAG.dbo.UA_BOM where luhid = '" + luhid + "' )   \n";
                sSql += " order by lubid,rowid asc    \n";
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

                var arrTypes = dt.AsEnumerable().Select(s => new { lubcid = s.Field<long>("lubcid"), type = s.Field<string>("type") }).Distinct().ToList();

                foreach (var itemType in arrTypes)
                {
                    sb.Append("<h4>" + itemType.type + "</h4>");
                    sb.Append("<table class='table table-hover'>");
                    sb.Append("<tr>");
                    sb.Append(" <th scope='col'>Standard Placement</th>");
                    sb.Append(" <th scope='col'>Usage</th>");
                    sb.Append(" <th scope='col'>Supplier Article</th>");
                    sb.Append(" <th scope='col'>Supplier</th>");

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

                        DataRow[] drUA_BOMGarmentcolor = dtUA_BOMGarmentcolor.Select("lubcid='" + itemType.lubcid + "'");
                        if (drUA_BOMGarmentcolor.Length > 0)
                        {
                            for (int i = 1; i <= 10; i++)
                            {
                                string sA = drUA_BOMGarmentcolor[0]["A" + i].ToString();
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

                        string standardPlacement = drBom["StandardPlacement"].ToString();
                        string usage = drBom["Usage"].ToString();
                        string supplierArticle = drBom["SupplierArticle"].ToString();
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

                                standardPlacement = Compare(drOrgBom["StandardPlacement"].ToString(), standardPlacement, FilterNote(arrNotes, lubid, "StandardPlacement"));
                                usage = Compare(drOrgBom["usage"].ToString(), usage, FilterNote(arrNotes, lubid, "usage"));
                                supplierArticle = Compare(drOrgBom["supplierArticle"].ToString(), supplierArticle, FilterNote(arrNotes, lubid, "supplierArticle"), IsMappingSupplierArticle);
                                supplier = Compare(drOrgBom["supplier"].ToString(), supplier, FilterNote(arrNotes, lubid, "supplier"));
                            }

                        }


                        if (!IsMappingSupplierArticle)
                        {
                            Response.Write("<!--supplierArticle=" + supplierArticle + " IsMappingSupplierArticle=" + IsMappingSupplierArticle + "-->");

                            if (drOrgBoms.Length > 0)
                            {
                                DataRow drOrgBom = drOrgBoms[0];

                                supplierArticle += "<br><font color='red'>修:無對應</font>"; ;
                            }

                        }

                        string parts = " data-PARTS_TYPE='" + PARTS_TYPE + "' data-PARTS_CODE='" + PARTS_CODE + "' data-PARTS_DESC='" + PARTS_DESC + "'  data-MAT_ID='" + MAT_ID + "' ";

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drBom["rowid"].ToString() + "' id='row" + iSeq + "'>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='StandardPlacement' onclick='editBom(this)'>" + standardPlacement + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='usage' onclick='editBom(this)'>" + usage + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' " + parts + " data-col='SupplierArticle' onclick='editBom(this)'>" + supplierArticle + "</td>");
                        sb.Append(" <td scope='col' data-lubid='" + lubid + "' data-org_lubid='" + org_lubid + "' data-col='Supplier' onclick='editBom(this)'>" + supplier + "</td>");

                        for (int i = 1; i <= iColorCnt; i++)
                        {
                            string colorHeader = arrColorHeader[iColorCnt - 1];
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
                    sb.Append(" <th scope='col'>#</th>");
                    sb.Append(" <th scope='col'>Description</th>");
                    sb.Append(" <th scope='col'>Criticality</th>");
                    sb.Append(" <th scope='col'>Tol(-)</th>");
                    sb.Append(" <th scope='col'>Tol(+)</th>");
                    sb.Append(" <th scope='col'>HTM Instruction</th>");

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

                    DataRow[] drSizeTables = dt.Select("lusthid='" + lusthid + "'", "rowid asc");
                    int idx = 0;
                    foreach (DataRow drSizeTable in drSizeTables)
                    {
                        string lustid = drSizeTable["lustid"].ToString();
                        string org_lustid = drSizeTable["org_lustid"].ToString();
                        bool isEdit = (drSizeTable["isEdit"].ToString().ToLower() == "true" ? true : false);

                        string Code = drSizeTable["Code"].ToString();
                        string Description = drSizeTable["Description"].ToString();
                        string criticality = drSizeTable["Criticality"].ToString();
                        string tolA = drSizeTable["TolA"].ToString();
                        string tolB = drSizeTable["TolB"].ToString();
                        string hTMInstruction = drSizeTable["HTMInstruction"].ToString();

                        DataRow[] drOrgSizeTables = dtSizeTable_Org.Select("lustid='" + org_lustid + "'");

                        if (isEdit)
                        {
                            Response.Write("<!--isEdit=" + isEdit + "-->");

                            if (drOrgSizeTables.Length > 0)
                            {
                                DataRow drOrgSizeTable = drOrgSizeTables[0];

                                Code = Compare(drOrgSizeTable["Code"].ToString(), Code, FilterNote(arrNotes, lustid, "Code"));
                                Description = Compare(drOrgSizeTable["Description"].ToString(), Description, FilterNote(arrNotes, lustid, "Description"));
                                criticality = Compare(drOrgSizeTable["criticality"].ToString(), criticality, FilterNote(arrNotes, lustid, "criticality"));
                                tolA = Compare(drOrgSizeTable["tolA"].ToString(), tolA, FilterNote(arrNotes, lustid, "tolA"));
                                tolB = Compare(drOrgSizeTable["tolB"].ToString(), tolB, FilterNote(arrNotes, lustid, "tolB"));
                                hTMInstruction = Compare(drOrgSizeTable["hTMInstruction"].ToString(), hTMInstruction, FilterNote(arrNotes, lustid, "hTMInstruction"));
                            }

                        }

                        iSeq++;
                        sb.Append("<tr data-rowid='" + drSizeTable["rowid"].ToString() + "' id='row" + iSeq + "'>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Code' onclick='editSizeTable(this)'>" + Code + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Description' onclick='editSizeTable(this)'>" + Description + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='Criticality' onclick='editSizeTable(this)'>" + criticality + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolA' onclick='editSizeTable(this)'>" + tolA + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='TolB' onclick='editSizeTable(this)'>" + tolB + "</td>");
                        sb.Append(" <td scope='col' data-lustid='" + lustid + "' data-org_lustid='" + org_lustid + "' data-col='HTMInstruction' onclick='editSizeTable(this)'>" + hTMInstruction + "</td>");

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
            return org;
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

            using (var cn = SqlMapperUtil.GetOpenConnection("DB"))
            {
                sSql = "select * from PDftag.dbo.UA_LearnmgrItem  \n";

                arrUA_LearnmgrItemDto = cn.Query<UA_LearnmgrItemDto>(sSql, new { }).ToList();
            }



            using (System.Data.SqlClient.SqlCommand cm = new System.Data.SqlClient.SqlCommand(sSql, sql.getDbcn()))
            {
                #region UA_BOM

                sSql = "select a.*,b.* \n";
                sSql += "from PDFTAG.dbo.UA_BOM a              \n";
                sSql += " join PDFTAG.dbo.UA_BOMGarmentcolor b on a.lubcid=b.lubcid              \n";
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
                    string Placement = drBom["Placement"].ToString();
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

                    var res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "StandardPlacement"
                     && x.Termname_org == StandardPlacement.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        StandardPlacement = res.Termname;
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Placement"
                     && x.Termname_org == Placement.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Placement = res.Termname;
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "SupplierArticle"
                    && x.Termname_org == SupplierArticle.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        SupplierArticle = res.Termname;
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == "Supplier"
                  && x.Termname_org == Supplier.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Supplier = res.Termname;
                        isUpdate = true;
                    }


                    for (int a = 1; a <= 10; a++)
                    {
                        switch (a)
                        {
                            case 1:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A1
              && x.Termname_org == B1.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B1 = res.Termname;
                                    isUpdate = true;
                                }
                                break;

                            case 2:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A2
              && x.Termname_org == B2.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B2 = res.Termname;
                                    isUpdate = true;
                                }
                                break;

                            case 3:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A3
              && x.Termname_org == B3.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B3 = res.Termname;
                                    isUpdate = true;
                                }
                                break;

                            case 4:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A4
              && x.Termname_org == B4.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B4 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 5:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A5
              && x.Termname_org == B5.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B5 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 6:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A6
              && x.Termname_org == B6.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B6 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 7:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A7
              && x.Termname_org == B7.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B7 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 8:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A8
              && x.Termname_org == B8.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B8 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 9:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A9
              && x.Termname_org == B9.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B9 = res.Termname;
                                    isUpdate = true;
                                }
                                break;
                            case 10:
                                res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "BOM" && x.ColName == A10
              && x.Termname_org == B10.Trim().Replace(" ", "").ToLower());

                                if (res != null)
                                {
                                    B10 = res.Termname;
                                    isUpdate = true;
                                }
                                break;

                        }

                    }

                    #endregion

                    if (isUpdate)
                    {
                        sSql = "update PDFTAG.dbo.UA_BOM \n";
                        sSql += "set  StandardPlacement=@StandardPlacement,Placement=@Placement,SupplierArticle=@SupplierArticle,Supplier=@Supplier           \n";
                        sSql += " ,B1=@B1,B2=@B2,B3=@B3,B4=@B4,B5=@B5,B6=@B6,B7=@B7,B8=@B8,B9=@B9,B10=@B10,isEdit=1           \n";
                        sSql += "where lubid=@lubid\n";
                        cm.CommandText = sSql;
                        cm.Parameters.Clear();
                        cm.Parameters.AddWithValue("@lubid", lubid);
                        cm.Parameters.AddWithValue("@StandardPlacement", StandardPlacement);
                        cm.Parameters.AddWithValue("@Placement", Placement);
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
                                  && x.Termname_org == Description.Trim().Replace(" ", "").ToLower());

                    if (res != null)
                    {
                        Description = res.Termname;
                        isUpdate = true;
                    }

                    res = arrUA_LearnmgrItemDto.FirstOrDefault(x => x.ColSource == "Size" && x.ColName == "Criticality"
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


}