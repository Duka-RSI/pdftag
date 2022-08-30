using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
/// <summary>
/// DbUtil 的摘要描述
/// </summary>
public class PageUtil<T>
{
    private PageUtil()
	{
	}


    /// <summary>
    /// 資料分頁，頁數由0開始
    /// </summary>
    public static List<T> getPageData(List<T> list, int pagesize, int page)
    {
        if (list==null || list.Count == 0)
        {
            return list;
        }

        if(pagesize<=0){
            pagesize = 1;
        }

        int maxPage = list.Count / pagesize + (list.Count % pagesize == 0 ? 0 : 1);
        if (page < 1)
        {
            page = 1;
        }
        if (page > maxPage)
        {
            page = maxPage ;
        }

        int start = (page-1)*pagesize;
        int end = page*pagesize;
        if(end>=list.Count){
            end = list.Count;
        }

        return list.GetRange(start, end - start);
    }

    public static int getTotalPages(int count, int pagesize)
    {
        int totalPages = count / pagesize;
        totalPages = totalPages + ((count % pagesize == 0) ? 0 : 1);
        return totalPages;
    }

}