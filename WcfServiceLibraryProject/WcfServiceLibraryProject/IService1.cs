using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using MySqlConnector;

using HtmlAgilityPack;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace WcfServiceLibraryProject
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IService1
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: Add your service operations here

        [OperationContract]
        void ScrapeAllData();

        //Apple
        [OperationContract]
        void ScrapeAppleDesktops();
        [OperationContract]
        void ScrapeAppleLaptops();
        [OperationContract]
        void ScrapeAppleTablets();
        [OperationContract]
        void ScrapeApplePhones();

        //Samsung
        [OperationContract]
        void ScrapeSamsungLaptops();
        [OperationContract]
        void ScrapeSamsungTablets();
        [OperationContract]
        void ScrapeSamsungPhones();

        //Google
        [OperationContract]
        void ScrapeGoogleLaptops();
        [OperationContract]
        void ScrapeGooglePhones();

        //Select Data
        [OperationContract]
        [WebGet(UriTemplate = "GetProducts/{brand}/{type}/{website}")]
        List<Products> GetProducts(string brand, string type, string website);
    }

    [DataContract]
    public class Products
    {
        [DataMember]
        public string Brand { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public string Price { get; set; }

    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "WcfServiceLibraryProject.ContractType".
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
