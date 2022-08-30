using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// AccessTokenClass 的摘要描述
/// </summary>
public class AccessTokenClass
{
    public AccessTokenClass()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    public bool IsAccess(string sToken)
    {
        try
        {
            //解開base64編碼
            sToken = Encoding.UTF8.GetString(Convert.FromBase64String(sToken));
            string user = Encoding.UTF8.GetString(Convert.FromBase64String(sToken.Split('|')[1].ToString()));
            string sMD5 = sToken.Split('|')[0].ToString();

            if (bHashEqual(sMD5, user, 0))
            {
                if (autoLogin(user) == true)
                return true;
            }

        }
        catch (Exception ex)
        {
            return false;
        }

        return false;
    }

    public string Hash(string ToHash)
    {
        //convert the string into bytes,
        Encoder enc = System.Text.Encoding.ASCII.GetEncoder();

        // Create a buffer large enough to hold the string
        byte[] data = new byte[ToHash.Length];
        enc.GetBytes(ToHash.ToCharArray(), 0, ToHash.Length, data, 0, true);

        // This is one implementation of the abstract class MD5.
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(data);

        return BitConverter.ToString(result).Replace("-", "").ToLower();
    }

    public bool bHashEqual(string HashStr,
             string sAccount, int minutes)
    {
        string ToHash = "";
        try
        {
            DateTime dt = DateTime.Now;
            System.TimeSpan minute = new System.TimeSpan(0, 0, minutes, 0, 0);
            dt = dt - minute;

            //YYYYMMDD|account|HHMM
            ToHash = dt.ToString("yyyyMMdd") + "|" + sAccount + "|" + dt.ToString("HHmm");

            ToHash = Hash(ToHash);

            if ((ToHash == HashStr))
                return true;
            else
                if (minutes == 0) // allowed max 2 minutes - 1
                    // second to call web service
                    return bHashEqual(HashStr, sAccount, 1);
                else
                    return false;
        }
        catch
        {
            return false;
        }
    }


    private bool autoLogin(string account)
    {


      
        return false;
    }

}