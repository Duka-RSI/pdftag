using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// PublicFunction 的摘要描述
/// </summary>
public static class PublicFunction
{
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