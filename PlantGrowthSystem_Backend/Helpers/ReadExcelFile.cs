using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Specialized;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class ReadExcelFile
    {
        public ListDictionary ParseExcel(string file)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(file);
            ListDictionary plantIntervals = new ListDictionary();
            List<Intervals> intervals = new List<Intervals>();
            for (int x = 1; x <= xlWorkbook.Sheets.Count; x++)
            {
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[x];

                // reading the plant ip number 
                string ip = xlWorksheet.Name;


                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;
                
                //iterate over the rows and columns
                //excel is not zero based!!
                for (int i = 2; i <= rowCount; i++)
                {
                    Intervals interval = new Intervals();
                    try
                    {
                        interval.date = DateTime.FromOADate(Convert.ToDouble(xlRange.Cells[i, 1].Value2)).ToString("MM/dd/yyyy");
                        interval.min_Humidity = Int32.Parse(xlRange.Cells[i, 2].Value2.ToString());
                        interval.max_Humidity = Int32.Parse(xlRange.Cells[i, 3].Value2.ToString());
                        interval.min_Temperature = Int32.Parse(xlRange.Cells[i, 4].Value2.ToString());
                        interval.max_Temperature = Int32.Parse(xlRange.Cells[i, 5].Value2.ToString());
                        interval.min_Light = Int32.Parse(xlRange.Cells[i, 6].Value2.ToString());
                        interval.max_Light = Int32.Parse(xlRange.Cells[i, 7].Value2.ToString());

                        intervals.Add(interval);
                    }

                    catch { }
                }

                plantIntervals.Add(ip, intervals);
            }

            

            return plantIntervals;
        }
    }

}