using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for CommonFunction
/// </summary>
public class CommonFunction
{
    public CommonFunction()
    {
        //
        // TODO: Add constructor logic here
        //

    }

    public List<DataConfigTable> getDataConfigTableAll()
    {
        var sql = new SQLHelper();
        List<DataConfigTable> dataConfigTableList = new List<DataConfigTable>();
        string sSql = @"select [ID], [PVER], [DataName], [DataValue], [DataType], [Enable] from [PDFTAG].[dbo].[DataConfigTable] 
                         where 1=1  order by [PVER],[DataType] ";
        using (var conn = new SqlConnection(sql._Conn))
        {
            dataConfigTableList = conn.Query<DataConfigTable>(sSql).ToList();
        }
        return dataConfigTableList;
    }

    /// <summary>
    /// 當參數為0時，不加入條件
    /// </summary>
    /// <param name="pver"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public List<DataConfigTable> getDataConfigTableByPverDataType(string pver, string dataType)
    {
        var sql = new SQLHelper();
        List<DataConfigTable> dataConfigTableList = new List<DataConfigTable>();
        string sSql = @"select [ID], [PVER], [DataName], [DataValue], [DataType], [Enable] from [PDFTAG].[dbo].[DataConfigTable] 
                         where 1=1 AND [Enable]=1 ";
        if (pver!="0")
            sSql += @"AND [PVER] = '" + pver + "' ";
        if (dataType != "0")
            sSql += @"AND  [DataType] = '" + dataType + "'";

        sSql += @" order by [PVER],[DataType] ";
        using (var conn = new SqlConnection(sql._Conn))
        {
            dataConfigTableList = conn.Query<DataConfigTable>(sSql).ToList();
        }
        return dataConfigTableList;
    }

    /// <summary>
    /// 當參數為0時，不加入條件
    /// </summary>
    /// <param name="pver"></param>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public DataTable getDtDataConfigTableByPverDataType(string pver, string dataType)
    {
        var sql = new SQLHelper();
        var dt = new DataTable();

        string sSql = @"select [ID] ,[PVER],[DataName]  ,[DataValue],[DataType] ,[Enable] from [PDFTAG].[dbo].[DataConfigTable] 
                         where 1=1 ";
        if (pver != "0")
            sSql += @"AND [PVER] = '" + pver + "'";
        if (dataType != "0")
            sSql += @" AND  [DataType] = '" + dataType + "'";
        sSql += @" order by [PVER],[DataType] ";
        using (SqlCommand cm = new SqlCommand(sSql, sql.getDbcn()))
        {
            using (SqlDataAdapter da = new SqlDataAdapter(cm))
            {
                da.Fill(dt);
            }
        }
            return dt;
    }

    public List<Customer_MappingTable> getCustomerMappingTableByPverList(List<string> pverList)
    {
        var sql = new SQLHelper();
        List<Customer_MappingTable> TableList = new List<Customer_MappingTable>();
        string pverStr = "";
        foreach (string pverItem in pverList)
        {
            pverStr += pverItem + ",";
        }

        string sSql = @"select * from [PDFTAG].[dbo].[CUSTOMER_MAPPING] 
                         where 1=1 AND [PVER] in ("+ pverStr.Substring(0,pverStr.Length-1) +")";
        using (var conn = new SqlConnection(sql._Conn))
        {
            TableList = conn.Query<Customer_MappingTable>(sSql).ToList();
        }
        return TableList;
    }
}