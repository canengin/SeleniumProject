using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Altunbilekler.Service.AddressListModel;

namespace Altunbilekler.Service
{
    public static class CrawlService
    {
        static List<string> catList = new List<string>();


        public static List<WebProduct> GetAllDetails()
        {
            var mainUrl = "http://www.altunbilekleronline.com/";
            List<Uri> uriList = new List<Uri>();

            catList.Add("7e4b754b-5b07-44d2-ad42-6f0db39f83f7");
            catList.Add("188b4cb7-56fb-4029-8c9d-dbc2effecb53");
            catList.Add("125d140f-bd62-4f82-8027-c01fb5196e4a");
            catList.Add("5d727427-549e-4fcb-a237-1eb54f5b0084");
            catList.Add("a9bf3470-26bd-4b06-ae4d-0ba4b59406ee");
            catList.Add("32573c58-141b-49e8-aa0d-0aa92efb7efa");
            catList.Add("21d47506-5117-4445-a383-134d92c9a5ff");
            catList.Add("ae0302c1-d71f-485f-bfb8-a30740a2fe8b");
            catList.Add("f735358c-aeef-4afe-b199-1c5a884535dc");
            catList.Add("7b581c66-6151-4853-8cde-5e22c17bd02b");
            catList.Add("21b48136-5ac1-4c17-b57e-d677f7b24520");
            catList.Add("728f01ff-adda-45a2-867e-91a98944aec1");
            catList.Add("6c43b8b2-66c3-4031-8b10-400ad28f902b");

            Dictionary<string, string> locationList = new Dictionary<string, string>();
            locationList.Add("08dac7a8-c3be-44f9-8988-e0e17538906f", "ANKARA-YENİMAHALLE-OSTİM");

            var dt = new List<WebProduct>();
            foreach (var location in locationList)
            {
                foreach (var category in catList)
                {

                    try
                    {
                        using (var client = new WebClient())
                        {

                            client.Encoding = Encoding.GetEncoding("UTF-8");
                            client.Headers[HttpRequestHeader.UserAgent] =
                                    ("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/106.0.0.0 Safari/537.36");
                            client.Headers.Add("client", "altunbilekler");
                            client.Headers.Add("shopid", location.Key);

                            try
                            {
                               
                                Console.WriteLine("\t\t\t\t altunbilekler Page:  -> Loading -> ");
                                client.Headers[HttpRequestHeader.UserAgent] =
                                    "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36";
                                ServicePointManager.Expect100Continue = true;
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                                var uri = new Uri("https://core.marketyo.net/api/v1/Products/AllByCategory?categoryId=" + category);
                                var sonuc = client.DownloadString(uri);
                                dynamic jsonData = JsonConvert.DeserializeObject<dynamic>(sonuc);
                                dynamic jsonProducts = jsonData.data;

                                foreach (var product in jsonProducts)
                                {
                                    if (product != null)
                                    {
                                        var dr = new WebProduct
                                        {
                                            StoreName = "Altun Bilekler",
                                            StockCount = 0,
                                            InStock = true,
                                            RequestTime = DateTime.Now
                                        };

                                        try
                                        {
                                            
                                            #region Mapping
                                            try
                                            {
                                                var skuDocument = product.name;
                                                dr.Sku = skuDocument ?? "SKU Çekilemedi";
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("SKU YOK!");
                                            }
                                            try
                                            {
                                                var barcodeDocument = product.id;
                                                dr.Barcode = barcodeDocument ?? "SKUCode Çekilemedi";
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("SKU YOK!");
                                            }
                                            try
                                            {
                                                string priceDocument = Convert.ToString(product.price);
                                                priceDocument = priceDocument.Replace('.', ' ');
                                                dr.Price = double.Parse(priceDocument ?? "0");
                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("FİYAT YOK!");
                                            }
                                            try
                                            {
                                                string oPrice = Convert.ToString(product.generalPrice);
                                                var oP = Convert.ToDouble(oPrice.Replace(".", ","));
                                                if (dr.Price == oP)
                                                {
                                                    dr.Price = oP;
                                                    dr.OldPrice = 0;
                                                }
                                                else
                                                {
                                                    dr.OldPrice = oP;
                                                }



                                            }
                                            catch (Exception)
                                            {
                                                dr.OldPrice = 0;
                                            }


                                            try
                                            {
                                                for (int i = 0; i < product.categories.Count; i++)
                                                {
                                                    if (product.categories[i].isVisible == true)
                                                    {
                                                        var categoryDocument = product.categories[i].name;
                                                        dr.Category = categoryDocument ?? "Kategori Çekilemedi";
                                                        break;
                                                    }

                                                }


                                            }
                                            catch (Exception ex)
                                            {
                                                Console.WriteLine("KATEGORİ YOK!");
                                            }
                                            try
                                            {
                                                var brandDocument = product.brand.name;
                                                dr.Brand = brandDocument ?? "Marka Çekilemedi";
                                            }
                                            catch (Exception)
                                            {
                                                Console.WriteLine("MARKA YOK!");
                                            }
                                            try
                                            {
                                                dr.Unit = location.Value.Split('-')[0].ToString() + "-" + location.Value.Split('-')[1].ToString();
                                                dr.Supplier = location.Value.Split('-')[2].ToString();
                                            }
                                            catch (Exception ex)
                                            {
                                            }

                                            try
                                            {
                                                var mainURL = "http://www.altunbilekleronline.com/product/";

                                                string name = dr.Sku;

                                                string n2 = name.ToLower().Replace(" ", "-");

                                                dr.NewUrl = mainURL + n2 + "/" + dr.Barcode;
                                            }
                                            catch (Exception ex)
                                            {
                                            }

                                            #endregion
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Hata Oluştu : " + ex.Message);

                                        }

                                        if (dr.Price > 0)
                                        {
                                            if (dr.OldPrice != 0)
                                            {
                                                dr.CargoDetail02 = "İNDİRİM";
                                            }
                                            else
                                            {
                                                dr.CargoDetail02 = "";
                                            }
                                            dt.Add(CheckTurkishCharacters(dr));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("altunbilekler için sayfada toplanacak url bitti.");
                            }

                            client.Dispose();
                        }
                    }
                    catch (Exception ex)
                    {

                        Console.WriteLine("Kategori bitti, yeni kategoriye geçiliyor.");
                    }

                }
            }
            GC.Collect();
            return dt.ToList();
        }

        private static WebProduct CheckTurkishCharacters(WebProduct model)
        {
            // 1 Characters in text
            WebProduct newModel = new WebProduct(); ;
            var temp = "";
            foreach (PropertyInfo item in model.GetType().GetProperties())
            {
                try
                {
                    if (item.GetValue(model) is string)
                    {
                        temp = item.GetValue(model).ToString();
                        temp = temp.Replace("þ", "ş");
                        temp = temp.Replace("Þ", "Ş");
                        temp = temp.Replace("ý", "ı");
                        temp = temp.Replace("Ý", "İ");
                        temp = temp.Replace("ð", "ğ");
                        temp = temp.Replace("Ð", "Ğ");
                        temp = temp.Replace("&#39;", "'");

                        // Just 2 characters in text;
                        temp = temp.Replace("Ãœ;", "ç");
                        temp = temp.Replace("Ä±", "ı");
                        temp = temp.Replace("ÄŸ", "ğ");
                        temp = temp.Replace("Ã¶", "ö");
                        temp = temp.Replace("ÅŸ", "ş");
                        temp = temp.Replace("Ã¼", "ü");
                        temp = temp.Replace("Ã‡", "Ç");
                        temp = temp.Replace("Ä°", "İ");
                        temp = temp.Replace("ÄŸ", "Ğ");
                        temp = temp.Replace("Ã–", "Ö");
                        temp = temp.Replace("ÅŸ", "Ş");
                        temp = temp.Replace("Ãœ", "Ü");

                        // 5 Characters in text
                        temp = temp.Replace("&#231;", "ç");
                        temp = temp.Replace("&#305;", "ı");
                        temp = temp.Replace("&#287;", "ğ");
                        temp = temp.Replace("&#246;", "ö");
                        temp = temp.Replace("&#351;", "ş");
                        temp = temp.Replace("&#252;", "ü");
                        temp = temp.Replace("&#199;", "Ç");
                        temp = temp.Replace("&#304;", "İ");
                        temp = temp.Replace("&#208;", "Ğ");
                        temp = temp.Replace("&#214;", "Ö");
                        temp = temp.Replace("&#350;", "Ş");
                        temp = temp.Replace("&#220;", "Ü");

                        //Another 5 Characters in text
                        temp = temp.Replace("&Ccedil;", "Ç");
                        temp = temp.Replace("&#286;", "Ğ");
                        temp = temp.Replace("&Ouml;", "Ö");
                        temp = temp.Replace("&Uuml;", "Ü");
                        temp = temp.Replace("&ccedil;", "ç");
                        temp = temp.Replace("&ouml;", "ö");
                        temp = temp.Replace("&uuml;", "ü");
                        temp = temp.Replace("&amp;", "&");
                        //Another 6 Characters in text
                        temp = temp.Replace("&middot;", "-");

                    }
                    else
                    {
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                item.SetValue(model, temp);
            }
            return model;

        }


    }
}
