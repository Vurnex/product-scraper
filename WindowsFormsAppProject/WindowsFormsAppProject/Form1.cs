using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WindowsFormsAppProject
{
    public partial class Form1 : Form
    {
        List<NeweggItems> neweggItems = new List<NeweggItems>();
        List<EbayItems> ebayItems = new List<EbayItems>();

        public string selectedBrand;
        public string selectedType;

        string neweggResults;
        string ebayResults;

        public Form1()
        {
            InitializeComponent();

            button3.Enabled = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {

            //Get data from server and display as a JSON object

            richTextBox1.Text = "";
            richTextBox2.Text = "";

            ServiceReference1.Service1Client MyClient = new ServiceReference1.Service1Client();

            if (selectedBrand == null || selectedType == null)
            {
                label4.Visible = true;
                return;
            }

            label4.Visible = false;

            //Query Newegg Table

            neweggResults = 
                JsonConvert.SerializeObject(MyClient.GetProducts(selectedBrand, selectedType, "Newegg"), Formatting.Indented);

            richTextBox1.Text = neweggResults;

            //Query Ebay Table

            ebayResults =
                JsonConvert.SerializeObject(MyClient.GetProducts(selectedBrand, selectedType, "Ebay"), Formatting.Indented);

            richTextBox2.Text = ebayResults;

            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            JArray jArray = JArray.Parse(neweggResults);

            JObject convNeweggResults = JObject.Parse(jArray[0].ToString());

            using (StreamWriter file = File.CreateText(@"C:\Users\Laquon\Documents\Homework\ITS-462\Project\newjson.json"))
            using (JsonTextWriter writer = new JsonTextWriter(file))
            {
                for (int i = 0; i < jArray.Count; i++)
                {
                    convNeweggResults = JObject.Parse(jArray[i].ToString());
                    convNeweggResults.WriteTo(writer);

                }
            }

            label7.Visible = true;

        }

        private async void button3_Click(object sender, EventArgs e)
        {
            //Call the server and tell it to scrape data
            ServiceReference1.Service1Client MyClient = new ServiceReference1.Service1Client();

            MyClient.ScrapeAllData();

            button3.Enabled = false;
            button3.Text = "Refreshing...";

            await Task.Delay(7000);

            button3.Enabled = true;
            button3.Text = "Refresh Data";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBrand = comboBox1.Text;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedType = comboBox2.Text;
        }
    }

    public class NeweggItems
    {
        public string itemTitle;
        public string itemPrice;
    };

    public class EbayItems
    {
        public string itemTitle;
        public string itemPrice;
    }

}
