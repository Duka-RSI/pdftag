using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for DataConfigTable
/// </summary>
public class DataConfigTable
{
    public long ID { get; set; }
    public int PVER { get; set; }
    public string DataName { get; set; }
    public string DataValue { get; set; }
    public string DataType { get; set; }
    public bool Enable { get; set; }

}