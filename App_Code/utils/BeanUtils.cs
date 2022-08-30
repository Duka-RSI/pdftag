using System;
using System.Collections.Generic;
using System.Web;
using System.Reflection;

/// <summary>
/// BeanUtils 的摘要描述
/// </summary>
public class BeanUtils
{
	private BeanUtils()
	{
	}

    public static void CopyProperties(object pobSrc, object pobDest, OptionTyp penOpt)
    {
        SetProperties(GetProperties(pobSrc), pobDest, penOpt);
    }

    public static void CopyPropertiesWithMap(object pobSrc, object pobDest, Dictionary<string, string> pdiMap, OptionTyp penOpt)
    {
        List<string> strSrc = new List<string>();
        List<string> strDest = new List<string>();
        foreach (KeyValuePair<string, string> pair in pdiMap)
        {
            strSrc.Add(pair.Key);
            strDest.Add(pair.Value);
        }
        CopyPropertiesWithMap(pobSrc, pobDest, strSrc.ToArray(), strDest.ToArray(), penOpt);
    }

    public static void CopyPropertiesWithMap(object pobSrc, object pobDest, string[] pstSrcPropertyNames, string[] pstDestPropertyNames, OptionTyp penOpt)
    {
        if (null == pobSrc || null == pobDest)
        { throw new ArgumentNullException("one of the arguments is null!"); }

        if (pstDestPropertyNames.Length != pstSrcPropertyNames.Length)
            throw new ArgumentException("pstDestPropertyNames & pstSrcPropertyNames must have same length");

        for (int i = 0; i < pstDestPropertyNames.Length; i++)
        {
            CopyProperty(pobSrc, pobDest, pstSrcPropertyNames[i], pstDestPropertyNames[i], penOpt);
        }
    }

    public static T GenernationObject<T>(object pobSrc, OptionTyp penOpt)
    {
        T lobDest = Activator.CreateInstance<T>();
        CopyProperties(pobSrc, lobDest, penOpt);
        return lobDest;
    }

    public static T GenernationObject<T>(Dictionary<string, object> pdiProperties, OptionTyp penOpt)
    {
        T lobDest = Activator.CreateInstance<T>();
        SetProperties(pdiProperties, lobDest, penOpt);

        return lobDest;
    }

    public static Dictionary<string, object> GetProperties(object pobObj)
    {
        Dictionary<string, object> list = new Dictionary<string, object>();
        string name;
        object val;

        if (null == pobObj) { throw new ArgumentNullException("pobObj can't be null"); }

        Type objType = pobObj.GetType();

        PropertyInfo[] objInfo = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        for (int i = 0; i < objInfo.Length; i++)
        {
            name = objInfo[i].Name;
            val = objInfo[i].GetValue(pobObj, null);

            try
            {
                list.Add(name, val);
            }
            catch (Exception ex)
            {
                ;
            }
        }
        return list;
    }

    public static void SetProperties(Dictionary<string, object> pdiProperties, object pobObj, OptionTyp penOpt)
    {
        foreach (KeyValuePair<string, object> pair in pdiProperties)
        {
            try
            {
                SetProperty(pobObj, pair.Key, pair.Value, penOpt);
            }
            catch (MapPropertyException) { }
        }
    }

    public static void CopyProperty(object pobSrc, object pobDest, string pstPropertyName, OptionTyp penOpt)
    {
        CopyProperty(pobSrc, pobDest, pstPropertyName, pstPropertyName, penOpt);
    }

    public static void CopyProperty(object pobSrc, object pobDest, string pstSrcPropertyName, string pstDestPropertyName, OptionTyp penOpt)
    {
        SetProperty(pobDest, pstDestPropertyName, GetProperty(pobSrc, pstSrcPropertyName, penOpt), penOpt);
    }

    public static void SetProperty(object pobObj, string pstPropertyName, object pobValue, OptionTyp penOpt)
    {
        if (null == pobObj || string.IsNullOrEmpty(pstPropertyName))
        {
            throw new ArgumentNullException("one of the arguments is null!");
        }

        bool isIgnoreCase = ((penOpt & OptionTyp.IsIgnoreCase) == OptionTyp.IsIgnoreCase);
        bool isConvert = ((penOpt & OptionTyp.IsConvert) == OptionTyp.IsConvert);
        bool isThrowConvertException = ((penOpt & OptionTyp.IsThrowConvertException) == OptionTyp.IsThrowConvertException);

        Type t = pobObj.GetType();
        PropertyInfo objInfo = null;
        if (isIgnoreCase)
        {
            PropertyInfo[] objInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo p in objInfos)
            {
                if (p.Name.ToUpperInvariant().Equals(pstPropertyName.ToUpperInvariant()))
                {
                    objInfo = p;
                    break;
                }
            }
        }
        else
            objInfo = t.GetProperty(pstPropertyName, BindingFlags.Instance | BindingFlags.Public);

        if (null == objInfo)
            throw new MapPropertyException("none mapping property");

        object descVal = null;
        if (null == pobValue || !isConvert)
            descVal = pobValue;
        else
            descVal = GetDestValue(pobValue.GetType(), objInfo.PropertyType, pobValue, isThrowConvertException);
        objInfo.SetValue(pobObj, descVal, null);
    }

    private static object GetDestValue(Type pobValueType, Type objPropertyType, object pobValue,bool isThrowConvertException)
    {
        object result = null;
        try
        {
            if (pobValue.GetType() == typeof(System.Guid))
            {
                result = pobValue.ToString();
            }
            else if (pobValue.GetType() == typeof(System.DBNull))
            {
                result = null;
            }
            else
            {
                result = Convert.ChangeType(pobValue, objPropertyType);
            }
        }
        catch (ConvertException ex)
        {
            if (isThrowConvertException)
            {
                throw ex;
            }
        }
        return result;
    }

    public static object GetProperty(object pobObj, string pstPropertyName, OptionTyp penOpt)
    {
        if (null == pobObj || string.IsNullOrEmpty(pstPropertyName))
        {
            throw new ArgumentNullException("Argument can't be null!");
        }
        bool isIgnoreCase = ((penOpt & OptionTyp.IsIgnoreCase) == OptionTyp.IsIgnoreCase);
        Type t = pobObj.GetType();
        PropertyInfo objInfo = null;
        if (isIgnoreCase)
        {
            PropertyInfo[] objInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo p in objInfos)
            {
                if (p.Name.ToUpperInvariant().Equals(pstPropertyName.ToUpperInvariant()))
                {
                    objInfo = p;
                    break;
                }
            }
        }
        else
            objInfo = t.GetProperty(pstPropertyName, BindingFlags.Instance | BindingFlags.Public);

        if (null == objInfo)
            throw new MapPropertyException("none mapping property");

        object val = objInfo.GetValue(pobObj, null);
        return val;
    }
}

[Flags, Serializable]
public enum OptionTyp
{
    None = 0,
    IsIgnoreCase = 0x0001,
    IsConvert = 0x0002,
    IsThrowConvertException = 0x0004
}

public class MapPropertyException : Exception
{
    public MapPropertyException(string message) : base(message) { }
}

public class ConvertException : Exception
{
    public ConvertException(string message) : base(message) { }
}