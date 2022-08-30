using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Javascript 的摘要描述
/// </summary>
public class Javascript
{
    #region 私有屬性
    /// <summary>
    /// 取得表示目前正在執行處理常式的 Web Form 網頁 物件。
    /// </summary>
    private static System.Web.UI.Page page
    {
        get { return (System.Web.UI.Page)System.Web.HttpContext.Current.CurrentHandler; }
    }
    #endregion

    #region 私有函式
    /// <returns>置換特殊字元之後的字串</returns>
    /// <remarks></remarks>
    private static string JSStringEscape(string raw, bool inHtmlAttribute)
    {
        raw = raw.Replace("\r\n", "\\n").Replace("\r", "").Replace("\n", "\\n");
        if (inHtmlAttribute)
            raw = raw.Replace("\"", "&quot;").Replace("'", "\\'");
        else
            raw = raw.Replace("'", "\\'").Replace("\"", "\\\"");
        return raw;
    }
    #endregion

    #region 公用函式
    public static string Generate(string ScriptText)
    {
        System.Text.StringBuilder script = new System.Text.StringBuilder();
        script.AppendLine("<script language=\'JavaScript\'>");
        script.AppendLine(ScriptText);
        script.AppendLine("</script>");
        return script.ToString();
    }
    public static string Generate(string ScriptText, string eventSource, string eventType)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendFormat("<script language=\'JavaScript\' for=\'{0}\' event=\'{1}\'>", eventSource, eventType);
        stringBuilder.Append(ScriptText);
        stringBuilder.Append("</script>");
        return stringBuilder.ToString();
    }
    #region 跳出Alert視窗
    /// <summary>
    /// 跳出Alert視窗
    /// </summary>
    /// <param name="message">訊息</param>
    public static void Alert(string message)
    {
        string script = "window.onload=function(){ \n";
        script += string.Format(" alert(\"{0}\"); \n", JSStringEscape(message, false));
        script += " }";
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "alert", Generate(script));
    }
    /// <summary>
    /// 跳出Alert視窗then重新整理
    /// </summary>
    /// <param name="message"></param>
    public static void AlertThenReload(string message)
    {
        string script = string.Format(" alert(\"{0}\");", JSStringEscape(message, false));
        script += string.Format(" window.location = window.location;");
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "alert", Generate(script));
    }
    public static void AlertThenTopReload(string message)
    {
        string script = string.Format(" alert(\"{0}\");", JSStringEscape(message, false));
        script += string.Format(" window.parent.location = window.parent.location;");
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "alert", Generate(script));
    }
    public static void AlertThenToRedirect(string message,string url)
    {
        string script = string.Format(" alert(\"{0}\");", JSStringEscape(message, false));
        script += string.Format(" window.location.href =\"{0}\";",url);
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "alert", Generate(script));
    }
    /// <summary>
    /// 重新整理
    /// </summary>
    /// <param name="message"></param>
    public static void Reload()
    {
        string script = "window.opener.location.reload();";
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "reload", Generate(script));
    }

    /// <summary>
    /// 關閉目前視窗
    /// </summary>
    /// <param name="message"></param>
    public static void WindowClose()
    {
        string script = "window.close(); ";
        page.ClientScript.RegisterStartupScript(typeof(System.Web.UI.Page), "close", Generate(script));
    }

    #endregion
    #endregion
}
