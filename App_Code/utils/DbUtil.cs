using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
/// <summary>
/// DbUtil 的摘要描述
/// </summary>
public  class DbUtil
{
	private DbUtil()
	{
	}

    public static void DataRow2Object(DataRow row,object obj)
    {
        object val;
        Dictionary<string,object> properties = BeanUtils.GetProperties(obj);
        foreach (string key in properties.Keys)
        {
            try
            {
                val = row[key];
                if (obj != null)
                {
                    BeanUtils.SetProperty(obj, key, val, OptionTyp.IsConvert);
                }
            }
            catch (Exception ex)
            {
                ;
            }
        }
    }

}