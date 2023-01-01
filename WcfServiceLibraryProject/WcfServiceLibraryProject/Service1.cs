using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using HtmlAgilityPack;
using Newtonsoft.Json;
using MySqlConnector;
using System.Threading.Tasks;
using System.Threading;

namespace WcfServiceLibraryProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class Service1 : IService1
    {
        MySqlConnection conn;

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public List<Products> GetProducts(string brand, string type, string website)
        {
            List<Products> products = new List<Products>();

            System.Diagnostics.Debug.WriteLine(brand);
            System.Diagnostics.Debug.WriteLine(type);
            System.Diagnostics.Debug.WriteLine(website);

            string currentBrand;
            string currentDesc;
            string currentType;
            string currentPrice;

            conn = DBUtils.CreateConnection();
            conn.Open();

            MySqlCommand prod = new MySqlCommand();

            if (website == "Newegg")
            {
                prod.CommandText = @"SELECT * FROM Database2.NeweggTable
                                     WHERE Brand = @Brand AND Type = @Type";

                prod.Parameters.AddWithValue("@Brand", brand);
                prod.Parameters.AddWithValue("@Type", type);
            }
            else if (website == "Ebay")
            {
                prod.CommandText = @"SELECT * FROM Database2.EBayTable
                                     WHERE Brand = @Brand AND Type = @Type";

                prod.Parameters.AddWithValue("@Brand", brand);
                prod.Parameters.AddWithValue("@Type", type);
            }

            prod.Connection = conn;

            MySqlDataReader selectProd = prod.ExecuteReader();

            while (selectProd.Read())
            {
                currentBrand = (string)selectProd["Brand"];
                currentDesc = (string)selectProd["Description"];
                currentType = (string)selectProd["Type"];
                currentPrice = (string)selectProd["Price"];

                products.Add(new Products()
                {
                    Brand = currentBrand,
                    Description = currentDesc,
                    Type = currentType,
                    Price = currentPrice,
                });
            }

            conn.Close();


            return products;
        } 

        public void ScrapeAllData()
        {
            refreshData();

            //await Task.Delay(5000); //wait for 5 seconds between each request.

            //Apple
            ScrapeAppleDesktops();

            Thread.Sleep(3000);

            ScrapeAppleLaptops();

            Thread.Sleep(3000);

            ScrapeAppleTablets();

            Thread.Sleep(3000);

            ScrapeApplePhones();

            Thread.Sleep(3000);

            //Samsung

            ScrapeSamsungLaptops();

            Thread.Sleep(3000);

            ScrapeSamsungTablets();

            Thread.Sleep(3000);

            ScrapeSamsungPhones();

            Thread.Sleep(3000);

            //Google

            ScrapeGoogleLaptops();

            Thread.Sleep(3000);

            ScrapeGooglePhones();

            //Remove redundant data

            MySqlCommand rm = new MySqlCommand();
            rm.Connection = conn;
            conn.Open();

            rm.CommandText = @"SET SQL_SAFE_UPDATES = 0;
                               DELETE FROM Database2.EBayTable
                               WHERE Description = 'Shop on eBay';
                               SET SQL_SAFE_UPDATES = 1;";

            rm.ExecuteNonQuery();

            conn.Close();

        }

        public void refreshData()
        {
            //Database
            conn = DBUtils.CreateConnection();

            //Refresh Data in Table

            MySqlCommand refresh = new MySqlCommand();
            refresh.Connection = conn;
            conn.Open();

            refresh.CommandText = @"TRUNCATE TABLE Database2.NeweggTable;
                                    TRUNCATE TABLE Database2.EBayTable;";
            refresh.ExecuteNonQuery();

            conn.Close();
        }

        public void ScrapeAppleDesktops()
        {
            //Desktops
            var html = @"https://www.newegg.com/p/pl?d=apple+imac";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=apple+imac&_sacat=0&LH_TitleDesc=0&_odkw=apple+imac&_osacat=0";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Apple");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Desktop");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Apple");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Desktop");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeAppleLaptops()
        {
            //Laptops
            var html = @"https://www.newegg.com/p/pl?d=apple+macbook";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=apple+macbook&_sacat=0&LH_TitleDesc=0&_odkw=apple+imac&_osacat=0";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Apple");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Laptop");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Apple");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Laptop");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeAppleTablets()
        {
            //Tablets
            var html = @"https://www.newegg.com/p/pl?d=apple+ipad";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=apple+ipad&_sacat=0&LH_TitleDesc=0&_odkw=apple+ipad&_osacat=0";
            
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Apple");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Tablet");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Apple");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Tablet");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeApplePhones()
        {
            //Phone
            var html = @"https://www.newegg.com/p/pl?d=apple+iphone";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=apple+iphone&_sacat=0&LH_TitleDesc=0&_odkw=apple+iphone&_osacat=0";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Apple");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Phone");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Apple");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Phone");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        
        //Samsung
        public void ScrapeSamsungLaptops()
        {
            //Laptops
            var html = @"https://www.newegg.com/p/pl?d=samsung+laptop";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=samsung+galaxy+book&_sacat=0&LH_TitleDesc=0&_odkw=samsung+galaxy+book&_osacat=0";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Samsung");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Laptop");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Samsung");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Laptop");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeSamsungTablets()
        {
            //Tablets
            var html = @"https://www.newegg.com/p/pl?d=samsung+tablet";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=samsung+tablet&_sacat=0&LH_TitleDesc=0&_odkw=samsung+tablet&_osacat=171485";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Samsung");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Tablet");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Samsung");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Tablet");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeSamsungPhones()
        {
            //Phone
            var html = @"https://www.newegg.com/p/pl?d=samsung+phone";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=samsung+phone&_sacat=0&LH_TitleDesc=0&_odkw=samsung+phone&_osacat=9355";
            
            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Samsung");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Phone");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Samsung");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Phone");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        //Google
        public void ScrapeGoogleLaptops()
        {
            //Laptops
            var html = @"https://www.newegg.com/p/pl?d=google+pixelbook";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=google+pixelbook&_sacat=0&LH_TitleDesc=0&_odkw=google+pixelbook&_osacat=0";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Google");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Laptop");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Google");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Laptop");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }

        public void ScrapeGooglePhones()
        {
            //Phone
            var html = @"https://www.newegg.com/p/pl?d=google+pixel";
            var html2 = @"https://www.ebay.com/sch/i.html?_from=R40&_trksid=p2334524.m570.l1313&_nkw=google+pixel&_sacat=0&LH_TitleDesc=0&_odkw=google+pixel&_osacat=9355";

            HtmlWeb web = new HtmlWeb();
            var htmlDoc = web.Load(html);

            var titles = htmlDoc.DocumentNode.SelectNodes("//a[@class='item-title']");
            var prices = htmlDoc.DocumentNode.SelectNodes("//li[@class='price-current']/strong");

            int count = 0;

            string title;
            string price;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData = new MySqlCommand();
                    newData.Connection = conn;
                    conn.Open();

                    newData.CommandText = @"INSERT INTO NeweggTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData.Parameters.AddWithValue("@Brand", "Google");
                    newData.Parameters.AddWithValue("@Description", title);
                    newData.Parameters.AddWithValue("@Type", "Phone");
                    newData.Parameters.AddWithValue("@Price", price);
                    newData.Parameters.AddWithValue("@Website", "Newegg");

                    newData.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            //Ebay

            htmlDoc = web.Load(html2);

            titles = htmlDoc.DocumentNode.SelectNodes("//div[@class='s-item__title']/span");
            prices = htmlDoc.DocumentNode.SelectNodes("//span[@class='s-item__price']");

            count = 0;

            try
            {
                foreach (var item in titles)
                {
                    if (count == 11)
                    {
                        break;
                    }

                    title = titles[count].InnerText;
                    price = prices[count].InnerText;

                    //Database

                    MySqlCommand newData2 = new MySqlCommand();
                    newData2.Connection = conn;
                    conn.Open();

                    newData2.CommandText = @"INSERT INTO EBayTable
                                    (Brand, Description, Type, Price, Website)
                                    VALUES (@Brand, @Description, @Type, @Price, @Website)";

                    newData2.Parameters.AddWithValue("@Brand", "Google");
                    newData2.Parameters.AddWithValue("@Description", title);
                    newData2.Parameters.AddWithValue("@Type", "Phone");
                    newData2.Parameters.AddWithValue("@Price", price);
                    newData2.Parameters.AddWithValue("@Website", "Ebay");

                    newData2.ExecuteNonQuery();

                    conn.Close();

                    count++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error executing the queries.", e);
            }
            finally
            {
                conn.Close();
            }

            return;
        }


        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
