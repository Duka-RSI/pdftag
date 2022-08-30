using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

/// <summary>
/// ExcelUtils 的摘要描述
/// </summary>
public class ExcelUtils
{
	public ExcelUtils()
	{
		
	}
    public static Stream RenderDataTableToExcel(DataTable SourceTable)
    {
        HSSFWorkbook workbook = new HSSFWorkbook();
        MemoryStream ms = new MemoryStream();
        HSSFSheet sheet = workbook.CreateSheet();
        HSSFRow headerRow = sheet.CreateRow(0);

        // handling header. 
        foreach (DataColumn column in SourceTable.Columns)
            headerRow.CreateCell(column.Ordinal).SetCellValue(column.ColumnName);

        // handling value. 
        int rowIndex = 1;

        foreach (DataRow row in SourceTable.Rows)
        {
            HSSFRow dataRow = sheet.CreateRow(rowIndex);

            foreach (DataColumn column in SourceTable.Columns)
            {
                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
            }

            rowIndex++;
        }

        workbook.Write(ms);
        ms.Flush();
        ms.Position = 0;

        sheet = null;
        headerRow = null;
        workbook = null;

        return ms;
    }
}