using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PublicFunction 的摘要描述
/// </summary>
public static class PublicFunction
{
    public static List<string> arrLuBomColors = new List<string>() { "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10","B11","B12","B13","B14","B15","B16","B17","B18","B19","B20","B21","B22","B23","B24","B25","B26","B27","B28","B29","B30","B31","B32","B33","B34","B35","B36","B37","B38","B39","B40","B41","B42","B43","B44","B45","B46","B47","B48","B49","B50" };
    public static List<string> arrGapBomColors = new List<string>() { "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10" };
    public static List<string> arrUABomColors = new List<string>() { "B1", "B2", "B3", "B4", "B5", "B6", "B7", "B8", "B9", "B10" };

    public static string Base64Encode(string plainText)
    {
        try
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        catch (Exception ex)
        {
            LogFile.Logger.LogException(ex, "[Base64Encode] plainText=" + plainText);
        }

        return "";
    }

    public static string Base64Decode(string base64EncodedData)
    {
        try
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        catch (Exception ex)
        {
            LogFile.Logger.LogException(ex, "[Base64Decode] base64EncodedData=" + base64EncodedData);
        }

        return "";
    }
}