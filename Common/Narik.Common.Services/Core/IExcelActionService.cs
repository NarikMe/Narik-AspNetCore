using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Narik.Common.Services.Core
{
    public interface  IExcelActionService
    {

        string UploadFile(byte[] data);

        string UploadFile(Stream data);


        bool DeleteFile(string name);
       

        List<string> GetColumns(string name, string sheetName);
     

        List<string> GetSheets(string name);

        DataTable GetData(string name, string sheetName);
       

        IEnumerable<Dictionary<string, object>> GetDataAsDictionary(string name, string sheetName,
            Dictionary<string, string> mappedColumns);
       

        IEnumerable<Dictionary<string, object>> GetDataAsDictionary(string name, string sheetName);
       
        

        //

        DataTable GetData(string name, string sheetName, Dictionary<string, string> mappedColumns);

        DataTable ExportToDataTable(string name, string sheetName);

        MemoryStream ExportAsExcel(List<DataTable> dataList);

    }
}
