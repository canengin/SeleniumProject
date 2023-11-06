using Altunbilekler.Service;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Altunbilekler.Service.AddressListModel;

namespace Altunbilekler
{
    class Program
    {     
        static string PROJECT_NAME = "AltunBilekler";      
        static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\crawlDatas\"; 
        static string path = "";
        static string gpsPath = "";

        static void Main(string[] args)
        {
            List<WebProduct> dcount = new List<WebProduct>();
            dcount = CrawlService.GetAllDetails();           
            ConvertToDatatable(dcount);
        }
        public static void ConvertToDatatable(List<WebProduct> list)
        {
            try
            {
                DataTable dt = new DataTable();

                DateTime date = DateTime.Now;

                dt.Columns.Add("StoreName");
                dt.Columns.Add("Category");
                dt.Columns.Add("SubCategory");
                dt.Columns.Add("Brand");
                dt.Columns.Add("SKU");
                dt.Columns.Add("SKUCode");
                dt.Columns.Add("Barcode");
                dt.Columns.Add("UnitCode");
                dt.Columns.Add("Supplier");
                dt.Columns.Add("SupplierMark");
                dt.Columns.Add("Supplier2");
                dt.Columns.Add("OldPrice");
                dt.Columns.Add("Price");
                dt.Columns.Add("Stock");
                dt.Columns.Add("IsStock");
                dt.Columns.Add("CargoDetail");
                dt.Columns.Add("CargoPrice");
                dt.Columns.Add("URL");
                dt.Columns.Add("DateTime");
                dt.Columns.Add("IsStar");

                string botName = "";



                foreach (var item in list.Distinct().ToList())
                {
                    var row = dt.NewRow();

                    botName = item.StoreName;

                    row["StoreName"] = item.StoreName;
                    row["Category"] = item.Category;
                    row["SubCategory"] = item.SubCategory;
                    row["Brand"] = item.Brand;
                    row["SKU"] = item.Sku;
                    row["SKUCode"] = item.Barcode;
                    row["Barcode"] = "";
                    row["UnitCode"] = item.Unit;
                    row["Supplier"] = item.Supplier;
                    row["SupplierMark"] = "";
                    row["Supplier2"] = item.SubTitle;
                    row["OldPrice"] = item.OldPrice;
                    row["Price"] = item.Price;
                    row["IsStock"] = item.InStock;
                    row["CargoDetail"] = item.CargoDetail02;
                    row["URL"] = item.NewUrl;
                    row["DateTime"] = Convert.ToDateTime(date).ToString();
                    row["Stock"] = item.StockCount;
                    row["IsStar"] = 0;

                    if (item.SupplierMark == null)
                    {
                        row["SupplierMark"] = "";
                    }

                    dt.Rows.Add(row);
                }

                try
                {
                    Random r = new Random();

                    path = folderPath + PROJECT_NAME + " " + gpsPath.Replace("*","") + " " + System.DateTime.Now.ToString("d MMMM yyyy dddd HH.mm.ss") + "-" + r.Next(999, 983948) + ".xlsx";
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ErrorHelper error = new ErrorHelper();
                    error.ErrorWriteFile(ex, folderPath, "excelPathError.txt", PROJECT_NAME);

                }

                bool isSuccess = false;

                #region Excel Çıkarma
                try
                {
                    GC.Collect();
                    string filePath = Pum_Excel_Management.ExportToExcel(path, dt, true, false);
                    //SendMail(filePath);
                    isSuccess = true;
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    ErrorHelper error = new ErrorHelper();
                    error.ErrorWriteFile(ex, folderPath, "excelError.txt", PROJECT_NAME);
                }
                #endregion

                #region DB ye yazma (local den veri çekiminde kapatılmalıdır.)


                //string PROJECT_CONNECTIONSTRING = "Data Source=;Initial Catalog=;user ID=;password=;MultipleActiveResultSets=True;";

                //DataProcess dp = new DataProcess();

                //string tableName = "BotDatas";

                //try
                //{
                //    dp.BulkCopy(dt, tableName, PROJECT_NAME, PROJECT_CONNECTIONSTRING);

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //    ErrorHelper error = new ErrorHelper();
                //    error.ErrorWriteFile(ex, folderPath, "DbError.txt", PROJECT_NAME);

                //}
                //Console.Clear();

                #endregion

                System.Diagnostics.Process.Start(folderPath);
                Console.Clear();
                //clearMemory();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
          
    }
}
