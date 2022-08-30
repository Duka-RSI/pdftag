<%@ WebHandler Language="C#" Class="dlPdf" %>

using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;

public class dlPdf : IHttpHandler
{
    public int pdf_ex1 = 1;
    public int pdf_ex2 = 2;
    public int pdf_ex3 = 3;
    public int pdf_ex4 = 4;
    public int pdf_ex5 = 5;
    public int pdf_ex6 = 6;
    public int pdf_ex7 = 7;
    public int pdf_ex8 = 8;
    public int pdf_ex9 = 9;

    public int reason_on = 1;
    public int reason_off = 0;



    public string doc_OfficeHour = "~/doc/ex1.pdf";
    public string doc_SeminarOrder = "~/doc/ex2.pdf";
    public string doc_SeminarReport = "~/doc/ex3.pdf";
    //public string doc_SeminarApply = "~/doc/ex4.pdf";
    public string doc_ProposalApply = "~/doc/ex5.pdf";
    public string doc_SeminarRecorder = "~/doc/ex9.pdf";
    public string doc_MidTerm = "~/doc/ex8.pdf";
    public string doc_TeachDemons = "~/doc/ex7.pdf";
    public string doc_CoachRecord = "~/doc/ex6.pdf";
    public BaseFont bfChineseFont()
    {
        BaseFont bf = null;
        bf = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\kaiu.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        return bf;
    }

    public void ProcessRequest(HttpContext context)
    {
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex1 && Int32.Parse(context.Request.Params["type"]) == 1)
        {
            this.ex1(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex1 && Int32.Parse(context.Request.Params["type"]) == 2)
        {
            this.ex1_2(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex2)
        {
            this.ex2(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex3)
        {
            this.ex3(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex4)
        {
            //this.ex4(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex5)
        {
            this.ex5(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex6)
        {
            this.ex6(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex7)
        {
            this.ex7(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex8)
        {
            this.ex8(context);
        }
        if (Int32.Parse(context.Request.Params["ex"]) == pdf_ex9)
        {
            this.ex9(context);
        }
    }
    private void ex1(HttpContext context)
    {
        List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_OfficeHour));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            for (int i = 0; i < list.Count; i++)
            {
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }
            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex1_2(HttpContext context)
    {
        List<OfficeHours> list = OfficeHoursDao.listOfficeHours((LoginUser.role == LoginUser.ROLE_ADMIN ? "" : LoginUser.PK), "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 2);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_OfficeHour));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            for (int i = 0; i < list.Count; i++)
            {
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }
            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex2(HttpContext context)
    {
        /*
         * 教師研討申請表
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        SeminarOrder list = SeminarOrderDao.getSeminarOrder(id);
        AccountObj account = AccountDao.getAccount(list.account);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_SeminarOrder));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());


            fields.SetField("fill_1", "護理系"); //單位
            fields.SetField("fill_2", ""); //申請日期
            fields.SetField("fill_6", list.starttime.ToString("yyyy-MM-dd HH:mm") + " ~ " + list.endtime.ToString("yyyy-MM-dd HH:mm")); //時間
            fields.SetField("fill_7", list.location); //地點
            fields.SetField("fill_8", account.Name); //參加人員
            fields.SetField("fill_9", list.org); //主辦單位
            fields.SetField("fill_10", list.subject); //主題

            fields.SetField("fill_11", list.subject); //其他
            fields.SetField("fill_12", list.allowance.ToString()); //補助費用
            if (list.fileTimeList) //學制
            {
                fields.SetField("toggle_1", "On");
            }
            if (list.fileDoc)
            {
                fields.SetField("toggle_2", "On");
            }
            if (list.fileOther)
            {
                fields.SetField("toggle_3", "On");
            }
            if (list.appluPublicHolidays)
            {
                fields.SetField("toggle_4", "On");
            }
            if (list.applyAllowance)
            {
                fields.SetField("toggle_5", "On");
            }


            fields.SetField("fill_5", account.Name); //申請人

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex3(HttpContext context)
    {
        /*
         * 教師研討申請表
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        SeminarOrder list = SeminarOrderDao.getSeminarOrder(id);
        AccountObj account = AccountDao.getAccount(list.account);
        TeacherInfo teacher = TeacherInfoDao.getTeacherInfo(list.account);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_SeminarReport));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());


            fields.SetField("fill_8", list.reportDate.ToString("yyyy-MM-dd")); //填表日期
            fields.SetField("fill_4", account.Name); //報告人
            fields.SetField("fill_5", teacher.departmentName); //單位

            fields.SetField("fill_1", list.starttime.ToString("yyyy-MM-dd HH:mm") + " ~ " + list.endtime.ToString("yyyy-MM-dd HH:mm")); //時間
            fields.SetField("fill_2", list.location); //地點
            fields.SetField("fill_3", list.subject); //主題

            fields.SetField("fill_6", list.report); //報告內容
            if (list.lv != null)
            {
                if (list.lv == "教授") //學制
                {
                    fields.SetField("toggle_1", "On");
                }
                if (list.lv == "副教授")
                {
                    fields.SetField("toggle_2", "On");
                }
                if (list.lv == "助理教授")
                {
                    fields.SetField("toggle_3", "On");
                }
                if (list.lv == "講師")
                {
                    fields.SetField("toggle_4", "On");
                }
                if (list.lv != "講師" && list.lv != "教授" && list.lv != "副教授" && list.lv != "助理教授")
                {
                    fields.SetField("toggle_5", "On");
                    fields.SetField("fill_7", list.lv); //其他人員
                }
            }
            else { }


            fields.SetField("fill_9", account.Name); //申請人

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex5(HttpContext context)
    {
        /*
         * 教師研討申請表
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        Proposal list = ProposalDao.getProposal(id);
        AccountObj account = AccountDao.getAccount(list.account);
        TeacherInfo teacher = TeacherInfoDao.getTeacherInfo(list.account);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_ProposalApply));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());


            fields.SetField("fill_1", list.schoolYear); //學年度
            fields.SetField("fill_29", account.Name); //申請教師
            fields.SetField("fill_30", ""); //職級
            fields.SetField("fill_31", ""); //申請日期

            if (list.teacherGroup != null)
            {
                if (list.teacherGroup == "基本護理") //教研會
                {
                    fields.SetField("toggle_1", "On");
                }
                if (list.teacherGroup == "內外科護理")
                {
                    fields.SetField("toggle_2", "On");
                }
                if (list.teacherGroup == "產科護理")
                {
                    fields.SetField("toggle_3", "On");
                }
                if (list.teacherGroup == "兒科護理")
                {
                    fields.SetField("toggle_4", "On");
                }
                if (list.teacherGroup == "精神科護理") //教研會
                {
                    fields.SetField("toggle_5", "On");
                }
                if (list.teacherGroup == "社區衛生護理")
                {
                    fields.SetField("toggle_6", "On");
                }
                if (list.teacherGroup == "急重症護理")
                {
                    fields.SetField("toggle_7", "On");
                }
                if (list.teacherGroup == "長期照護")
                {
                    fields.SetField("toggle_8", "On");
                }
            }
            else { }
            fields.SetField("fill_32", ""); //教授課程
            fields.SetField("fill_33", list.startTime.ToString("yyyy-MM-dd HH:mm")); //時間
            fields.SetField("fill_34", list.endTime.ToString("yyyy-MM-dd HH:mm")); //時間
            TimeSpan total = list.endTime.Subtract(list.startTime);
            fields.SetField("fill_35", total.Hours.ToString()); //小時

            fields.SetField("fill_36", list.unitName); //機構
            fields.SetField("fill_37", list.unitZone); //院區
            fields.SetField("fill_38", list.unitPart); //單位
            fields.SetField("fill_39", list.unitContect); //聯絡人
            fields.SetField("fill_40", list.unitTel); //電話
            //學歷
            fields.SetField("fill_2", list.gradeSchool1); //學校
            fields.SetField("fill_3", list.nation1); //國別
            fields.SetField("fill_4", list.master1); //主修
            fields.SetField("fill_5", list.degree1); //學位
            fields.SetField("fill_6", list.startEnd1); //起迄
            fields.SetField("fill_7", list.gradeSchool2); //
            fields.SetField("fill_8", list.nation2); //
            fields.SetField("fill_9", list.master2); //
            fields.SetField("fill_10", list.degree2); //
            fields.SetField("fill_11", list.startEnd2); //
            /*
            fields.SetField("fill_12", list.subject); //
            fields.SetField("fill_13", list.subject); //
            fields.SetField("fill_14", list.subject); //
            fields.SetField("fill_15", list.subject); //
            fields.SetField("fill_16", list.subject); //
             */
            //現職與專長
            fields.SetField("fill_17", list.workPlace1); //服務機關
            fields.SetField("fill_18", list.workDepart1); //服務部門
            fields.SetField("fill_19", list.workTitle1); //職稱
            fields.SetField("fill_20", list.wstartEnd1); //起迄
            fields.SetField("fill_21", list.workPlace2); //
            fields.SetField("fill_22", list.workDepart2); //
            fields.SetField("fill_23", list.workTitle2); //
            fields.SetField("fill_24", list.wstartEnd2); //
            fields.SetField("fill_25", list.workPlace3); //
            fields.SetField("fill_26", list.workDepart3); //
            fields.SetField("fill_27", list.workTitle3); //
            fields.SetField("fill_28", list.wstartEnd3); //

            fields.SetField("fill_41", account.Name); //申請人

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex6(HttpContext context)
    {
        /*
         * 學生預警輔導紀錄
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        CoachRecord list = CoachRecordDao.get(id);

        var studentInfos = StudentInfoDao.listStudentInfo(list.student.Split(','));

        if (studentInfos != null && studentInfos.Count > 0)
        {
            list.eduSystem = studentInfos.First().eduSystem;
            list.class1 = studentInfos.First().class1.ToString();
            list.class2 = studentInfos.First().class2.ToString();
            list.name = studentInfos.Select(a => string.Format("{0}/{1}", a.StudentNo, a.name)).Aggregate((a, b) => string.Format("{0}, {1}", a, b));
        }

        //2015/01/24 洪儀竣，取得老師資料
        TeacherInfo teacher1 = TeacherInfoDao.getTeacherInfo(list.teacher1);
        TeacherInfo teacher2 = TeacherInfoDao.getTeacherInfo(list.teacher2);
        //2015/01/24 洪儀竣，取得老師資料
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_CoachRecord));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            /*for (int i = 0; i < list.Count; i++)
            //{
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }*/
            if (list.eduSystem == "五專") //學制
            {
                fields.SetField("toggle_1", eduSystem(list.eduSystem));
            }
            if (list.eduSystem == "四技")
            {
                fields.SetField("toggle_2", eduSystem(list.eduSystem));
            }
            if (list.eduSystem == "二技")
            {
                fields.SetField("toggle_3", eduSystem(list.eduSystem));
            }
            if (list.eduSystem == "進修部")
            {
                fields.SetField("toggle_4", eduSystem(list.eduSystem));
            }
            //2015/01/24 洪儀竣，各項匯出資料
            fields.SetField("fill_1", list.year);//學年
            //fields.SetField("fill_2", list.period.ToString());//學期
            fields.SetField("fill_3", list.class3.ToString()); //年級
            fields.SetField("fill_4", list.class4); //班級
            fields.SetField("fill_5", list.StudentNo); //座號
            fields.SetField("fill_students", list.name); //姓名
            fields.SetField("fill_7", list.className); //課程
            //輔導原因:
            if (list.reson1)
            {
                fields.SetField("toggle_5", "On");
            }
            if (list.reson2)
            {
                fields.SetField("toggle_6", "On");
            }
            fields.SetField("fill_8", list.midtest.ToString()); //期中成績
            fields.SetField("fill_9", list.smalltest.ToString()); //小考成績
            //不理想原因:
            if (list.why1)
            {
                fields.SetField("toggle_7", "On");
            }
            if (list.why2)
            {
                fields.SetField("toggle_8", "On");
            }
            if (list.why3)
            {
                fields.SetField("toggle_9", "On");
            }
            if (list.why4)
            {
                fields.SetField("toggle_10", "On");
            }
            if (list.why5)
            {
                fields.SetField("toggle_11", "On");
            }
            if (list.why6)
            {
                //fields.SetField("toggle_12", "On");
            }
            fields.SetField("fill_reason", list.whyNote); //不想念原因
            fields.SetField("fill_other", list.other);//其他原因
            //輔導日期:
            fields.SetField("fill_12", list.startTime.Year.ToString()); //年
            fields.SetField("fill_13", list.startTime.Month.ToString()); //月
            fields.SetField("fill_14", list.startTime.Day.ToString()); //日
            fields.SetField("fill_15", list.startTime.Hour.ToString("") + ":" + list.startTime.Minute.ToString(""));//時間，始
            fields.SetField("fill_endtime", list.endTime.Hour.ToString("") + ":" + list.endTime.Minute.ToString(""));//時間，終
            //fields.SetField("fill_12", list.startTime.Hour.ToString()+":"+list.startTime.Minute.ToString()); //起
            //fields.SetField("fill_11", list.startTime.ToString("HH:mm")+"~"+list.endTime.ToString("HH:mm")); //迄
            //輔導內容:
            if (list.content1)
            {
                fields.SetField("toggle_13", "On");
            }
            if (list.content2)
            {
                fields.SetField("toggle_14", "On");
            }
            if (list.content3)
            {
                fields.SetField("toggle_15", "On");
            }
            if (list.content4)
            {
                fields.SetField("toggle_16", "On");
            }
            if (list.content5)
            {
                fields.SetField("toggle_17", "On");
            }
            fields.SetField("fill_19", list.contentNote);//簡述
            fields.SetField("fill_20", list.plan);//計畫1
            fields.SetField("fill_21", list.plan2);//計畫2
            //成效追蹤:
            if (list.content6)
            {
                fields.SetField("toggle_18", "On");
            }
            if (list.content7)
            {
                fields.SetField("toggle_19", "On");
            }
            fields.SetField("fill_22", list.effect);//成效說明
            fields.SetField("fill_17", teacher1.name);//輔導教師
            fields.SetField("fill_18", teacher2.name);//導師
            //2015/01/24 洪儀竣，各項匯出資料

            if (list.isRead == 1)
                fields.SetField("fill_33", "管理者已讀取");

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex7(HttpContext context)
    {
        /*
         * 教學觀摩紀錄
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        TeachingDemonstration list = TeachingDemonstrationDao.get(id);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_TeachDemons));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            /*for (int i = 0; i < list.Count; i++)
            //{
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }*/
            //2015/01/24 洪儀竣，各項匯出資料
            fields.SetField("fill_20", list.year);//學年
            fields.SetField("fill_21", list.period.ToString());//學期
            if (list.edusystemName == "五專") //學制
            {
                fields.SetField("toggle_1", "On");
            }
            if (list.edusystemName == "四技")
            {
                fields.SetField("toggle_2", "On");
            }
            if (list.edusystemName == "二技")
            {
                fields.SetField("toggle_3", "On");
            }
            fields.SetField("fill_1", list.time.ToString("yyyy-MM-dd")); //觀摩日期
            fields.SetField("fill_4", list.classNo); //堂次
            fields.SetField("fill_5", list.className); //課程名稱
            fields.SetField("fill_6", list.unit); //單元
            fields.SetField("fill_7", list.teacherName); //授課教師
            fields.SetField("fill_13", list.classInfo); //班級經營
            //核心能力情形
            if (list.option01)
            {
                fields.SetField("toggle_4", "On");
            }
            if (list.option02)
            {
                fields.SetField("toggle_5", "On");
            }
            if (list.option03)
            {
                fields.SetField("toggle_6", "On");
            }
            if (list.option04)
            {
                fields.SetField("toggle_7", "On");
            }
            if (list.option05)
            {
                fields.SetField("toggle_8", "On");
            }
            if (list.option06)
            {
                fields.SetField("toggle_9", "On");
            }
            if (list.option07)
            {
                fields.SetField("toggle_10", "On");
            }
            if (list.option08)
            {
                fields.SetField("toggle_11", "On");
            }
            if (list.option09)
            {
                fields.SetField("toggle_12", "On");
            }
            if (list.option10)
            {
                fields.SetField("toggle_13", "On");
            }
            fields.SetField("fill_14", list.techStrategy); //教學策略
            fields.SetField("fill_15", list.superiority); //優勢
            fields.SetField("fill_16", list.proposal); //建議
            fields.SetField("fill_8", list.accountName); //觀摩教師    
            //2015/01/24 洪儀竣，各項匯出資料 

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex8(HttpContext context)
    {
        /*
         * 期中教學評量
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        TeachingEvaluation list = TeachingEvaluationDao.get(id);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_MidTerm));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            /*for (int i = 0; i < list.Count; i++)
            //{
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }*/
            if (list.edusystemName == "五專") //學制
            {
                fields.SetField("toggle_1", "On");
            }
            if (list.edusystemName == "四技")
            {
                fields.SetField("toggle_2", "On");
            }
            if (list.edusystemName == "二技")
            {
                fields.SetField("toggle_3", "On");
            }
            if (list.edusystemName == "進修部")
            {
                fields.SetField("toggle_4", "On");
            }
            fields.SetField("fill_0", list.className); //課程名稱
            fields.SetField("fill_1", list.year); //課程名稱
            fields.SetField("fill_2", list.period.ToString()); //課程名稱
            fields.SetField("fill_3", list.class1); //年
            fields.SetField("fill_4", list.class2); //班
            fields.SetField("fill_5", list.answer.ToString());
            fields.SetField("fill_t", list.teacherName); //授課教師
            fields.SetField("fill_t2", list.teacherName2); //授課教師
            fields.SetField("fill_t3", list.teacherName3); //授課教師
            fields.SetField("fill_t4", list.teacherName4); //授課教師
            fields.SetField("fill_t5", list.teacherName5); //授課教師
            var classTeacherName = "";
            if (!string.IsNullOrEmpty(list.classteacher))
            {
                classTeacherName = TeacherInfoDao.getTeacherInfo(list.classteacher).name;
            }
            fields.SetField("fill_t1", classTeacherName); //課程負責人
            fields.SetField("fill_6", list.startdate.ToString("yyyy-MM-dd"));
            fields.SetField("fill_7", list.enddate.ToString("yyyy-MM-dd"));

            //授課教師評分一覽
            fields.SetField("fill_8", list.avgscro01); //
            fields.SetField("fill_9", list.avgscro02); //
            fields.SetField("fill_10", list.avgscro03); //
            fields.SetField("fill_11", list.avgscro04); //
            fields.SetField("fill_12", list.avgscro05); //
            fields.SetField("fill_13", list.avgscro06); //
            fields.SetField("fill_14", list.avgscro07); //
            fields.SetField("fill_15", list.avgscro08); //
            fields.SetField("fill_16", list.avgscro09); //
            fields.SetField("fill_17", list.avgscro10); //
            fields.SetField("fill_18", list.avgscro11); //
            fields.SetField("fill_19", list.avgscro12); //
            fields.SetField("fill_20", list.avg); //平均

            fields.SetField("fill_2_1", list.opinion1); //授課教師意見
            fields.SetField("fill_2_3", list.opinion2); //授課時數調整
            fields.SetField("fill_2_5", list.opinion3); //教學活動
            fields.SetField("fill_2_7", list.opinion4); //其他
            fields.SetField("fill_2_2", list.opinion5); //授課教師意見
            fields.SetField("fill_2_4", list.opinion6); //授課時數調整
            fields.SetField("fill_2_6", list.opinion7); //教學活動
            fields.SetField("fill_2_8", list.opinion8); //其他

            if (list.isRead == 1)
                fields.SetField("fill_33", "管理者已讀取");

            fields.SetField("fill_2_9", list.accountName); //彙總教師
            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void ex9(HttpContext context)
    {
        /*
         * 教師研討記錄單
         */
        string s_id = context.Request.Params["id"];
        int id = Int32.Parse(s_id);
        TeacherSeminarRecord list = TeacherSeminarRecordDao.get(id);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours(LoginUser.PK, "", 1, 1);
        //List<OfficeHours> list = OfficeHoursDao.listOfficeHours("9999", "101", 1, 1);
        PdfReader reader = new PdfReader(context.Server.MapPath(doc_SeminarRecorder));
        using (MemoryStream ms = new MemoryStream())
        {
            PdfStamper stamper = new PdfStamper(reader, ms);
            AcroFields fields = stamper.AcroFields;
            fields.AddSubstitutionFont(bfChineseFont());
            /*for (int i = 0; i < list.Count; i++)
            //{
                fields.SetField("fill_a" + (i + 1).ToString(), list[i].schoolYear);
                fields.SetField("fill_b" + (i + 1).ToString(), list[i].depName);
                fields.SetField("fill_c" + (i + 1).ToString(), list[i].account);
                fields.SetField("fill_d" + (i + 1).ToString(), list[i].name);
                fields.SetField("fill_e" + (i + 1).ToString(), list[i].date.ToShortDateString() + "  " + OfficeHoursDao.timeBlockToStr(list[i].timeBlock));
            }*/

            //fields.SetField("fill_1", ""); //學年
            //fields.SetField("fill_2", ""); //學制
            fields.SetField("fill_31", list.unitName); //課程
            fields.SetField("fill_32", list.unitName); //單元名稱
            fields.SetField("fill_33", list.personInChargeName); //負責老師
            fields.SetField("fill_34", list.teacherName); //授課老師
            fields.SetField("fill_35", list.time.ToString("yyyy-MM-dd HH:mm")); //討論時間

            fields.SetField("fill_36", list.content); //決議要項
            fields.SetField("fill_37", list.recommendMedia); //書目與視聽媒體
            fields.SetField("fill_38", list.other); //追蹤事項

            fields.SetField("fill_4", list.accountName); //紀錄人
            fields.SetField("fill_5", list.teacherName); //課程負責人

            stamper.FormFlattening = true;
            stamper.Close();
            DownloadAsPDF(ms, context);
        }
    }
    private void DownloadAsPDF(MemoryStream ms, HttpContext context)
    {
        context.Response.Clear();
        context.Response.ClearContent();
        context.Response.ClearHeaders();
        context.Response.ContentType = "application/pdf";
        context.Response.AppendHeader("Content-Disposition", "attachment;filename=pdf.pdf");
        context.Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        context.Response.OutputStream.Flush();
        context.Response.OutputStream.Close();
        context.Response.End();
        ms.Close();
    }
    public string eduSystem(string edu_system)
    {
        string check = null;
        if (edu_system == "五專")
        {
            check = "On";
        }
        if (edu_system == "四技")
        {
            check = "On";
        }
        if (edu_system == "二技")
        {
            check = "On";
        }
        else { }
        return check;
    }


    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}