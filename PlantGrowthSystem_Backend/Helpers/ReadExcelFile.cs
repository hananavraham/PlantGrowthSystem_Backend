using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Specialized;
using PlantGrowthSystem_Backend.Models;

namespace PlantGrowthSystem_Backend.Helpers
{
    public class ReadExcelFile
    {
        public List<FullPlant> ParseExcel(string file)
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkbook = xlApp.Workbooks.Open(file);
            ListDictionary plantIntervals = new ListDictionary();

            List<FullPlant> plants = new List<FullPlant>();
            for (int x = 1; x <= xlWorkbook.Sheets.Count; x++)
            {
                Excel._Worksheet xlWorksheet = xlWorkbook.Sheets[x];

                List<Intervals> intervals = new List<Intervals>();
                // reading the plant ip number 
                string ip = xlWorksheet.Name;


                Excel.Range xlRange = xlWorksheet.UsedRange;

                int rowCount = xlRange.Rows.Count;
                int colCount = xlRange.Columns.Count;

                FullPlant plant  = new FullPlant
                {
                    Env_control_address = xlRange.Cells[1, 2].Value2.ToString(),
                    Growth_control_address = xlRange.Cells[2, 2].Value2.ToString(),
                    Frequency_of_measurement = Int32.Parse(xlRange.Cells[3, 2].Value2.ToString()),
                    Frequency_of_upload = Int32.Parse(xlRange.Cells[4, 2].Value2.ToString()),
                    Status = "Pending"
       
                };
                
                
                //iterate over the rows and columns
                //excel is not zero based!!
                for (int i = 6; i <= rowCount; i++)
                {
                    Intervals interval = new Intervals();
                    try
                    {
                        interval.date = DateTime.FromOADate(Convert.ToDouble(xlRange.Cells[i, 1].Value2)).ToString("MM/dd/yyyy");
                        interval.min_Humidity = Int32.Parse(xlRange.Cells[i, 2].Value2.ToString());
                        interval.max_Humidity = Int32.Parse(xlRange.Cells[i, 3].Value2.ToString());
                        //interval.min_Temperature = Int32.Parse(xlRange.Cells[i, 4].Value2.ToString());
                        //interval.max_Temperature = Int32.Parse(xlRange.Cells[i, 5].Value2.ToString());
                        interval.light_Per_Day = Int32.Parse(xlRange.Cells[i, 6].Value2.ToString());
                        //interval.min_Light = Int32.Parse(xlRange.Cells[i, 6].Value2.ToString());
                        //interval.max_Light = Int32.Parse(xlRange.Cells[i, 7].Value2.ToString());

                        intervals.Add(interval);
                    }

                    catch { }
                }

                plant.Intervals = intervals;

                plants.Add(plant);
            }

            

            return plants;
        }
    }

}