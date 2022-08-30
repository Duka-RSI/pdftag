using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Define 的摘要描述
/// </summary>
public static class Define
{
    public static int rowsPrePage = 2;

    public static String GetConfig(String Key)
    {
        return System.Web.Configuration.WebConfigurationManager.AppSettings[Key];
    }


    public static class UserRole
    {
        /// <summary>
        /// 學生
        /// </summary>
        public const int Student = 0;
        /// <summary>
        /// 老師
        /// </summary>
        public const int Teacher = 1;
        /// <summary>
        /// 系統管理者
        /// </summary>
        public const int Admin = 2;
        /// <summary>
        /// 試題管理者
        /// </summary>
        public const int TestAdmin = 3;
    }
}

