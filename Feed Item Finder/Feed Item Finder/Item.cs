using System;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace FeedItemFinder
{
    public static class Names
    {
        private const string _C_XMLNS = "http://base.google.com/cns/1.0";
        private const string _G_XMLNS = "http://base.google.com/ns/1.0";
        private const string _item = "item";
        private const string _title = "title";
        private const string _link = "link";
        private const string _description = "description";
        private const string _g_id = "{" + _G_XMLNS + "}id";
        private const string _g_condition = "{" + _G_XMLNS + "}condition";
        private const string _g_price = "{" + _G_XMLNS + "}price";
        private const string _g_availability = "{" + _G_XMLNS + "}availability";
        private const string _g_online_only = "{" + _G_XMLNS + "}online_only";
        private const string _g_shipping = "{" + _G_XMLNS + "}shipping";
        private const string _g_country = "{" + _G_XMLNS + "}country";
        private const string _g_google_product_category = "{" + _G_XMLNS + "}google_product_category";
        private const string _g_product_type = "{" + _G_XMLNS + "}product_type";
        private const string _g_custom_label_0 = "{" + _G_XMLNS + "}custom_label_0";
        private const string _g_custom_label_1 = "{" + _G_XMLNS + "}custom_label_1";
        private const string _g_custom_label_2 = "{" + _G_XMLNS + "}custom_label_2";
        private const string _g_custom_label_3 = "{" + _G_XMLNS + "}custom_label_3";
        private const string _g_custom_label_4 = "{" + _G_XMLNS + "}custom_label_4";
        private const string _g_brand = "{" + _G_XMLNS + "}brand";
        private const string _g_item_group_id = "{" + _G_XMLNS + "}item_group_id";
        private const string _g_color = "{" + _G_XMLNS + "}color";
        private const string _g_gtin = "{" + _G_XMLNS + "}gtin";
        private const string _g_image_link = "{" + _G_XMLNS + "}image_link";
        private const string _g_mpn = "{" + _G_XMLNS + "}mpn";
        private const string _g_shipping_weight = "{" + _G_XMLNS + "}shipping_weight";
        private const string _c_length = "{" + _G_XMLNS + "}length";
        private const string _c_certification = "{" + _G_XMLNS + "}certification";
        private const string _c_color = "{" + _G_XMLNS + "}color";
        private const string _c_cable_style = "{" + _G_XMLNS + "}cable_style";
        private const string _c_rating = "{" + _G_XMLNS + "}rating";

        public static XNamespace C_XMLNS { get { return _C_XMLNS; } }
        public static XNamespace G_XMLNS { get { return "http://base.google.com/ns/1.0"; } }
        public static XName Item { get { return _item; } }
        public static XName Title { get { return _title; } }
        public static XName Link { get { return _link; } }
        public static XName Description { get { return _description; } }
        public static XName g_id { get { return _g_id; } }
        public static XName g_condition { get { return _g_condition; } }
        public static XName g_price { get { return _g_price; } }
        public static XName g_availability { get { return _g_availability; } }
        public static XName g_online_only { get { return _g_online_only; } }
        public static XName g_shipping { get { return _g_shipping; } }
        public static XName g_country { get { return _g_country; } }
        public static XName g_google_product_category { get { return _g_google_product_category; } }
        public static XName g_product_type { get { return _g_product_type; } }
        public static XName g_custom_label_0 { get { return _g_custom_label_0; } }
        public static XName g_custom_label_1 { get { return _g_custom_label_1; } }
        public static XName g_custom_label_2 { get { return _g_custom_label_2; } }
        public static XName g_custom_label_3 { get { return _g_custom_label_3; } }
        public static XName g_custom_label_4 { get { return _g_custom_label_4; } }
        public static XName g_brand { get { return _g_brand; } }
        public static XName g_item_group_id { get { return _g_item_group_id; } }
        public static XName g_color { get { return _g_color; } }
        public static XName g_gtin { get { return _g_gtin; } }
        public static XName g_image_link { get { return _g_image_link; } }
        public static XName g_mpn { get { return _g_mpn; } }
        public static XName g_shipping_weight { get { return _g_shipping_weight; } }
        public static XName c_length { get { return _c_length; } }
        public static XName c_certification { get { return _c_certification; } }
        public static XName c_color { get { return _c_color; } }
        public static XName c_cable_style { get { return _c_cable_style; } }
        public static XName c_rating { get { return _c_rating; } }
    }
    public class Item// : XElement
    {
        public XElement XElement { get; set; }

        public string Title { get { return this.XElement.Element(Names.Title).Value; } }
        public string Link { get { return this.XElement.Element(Names.Link).Value; } }
        public string Description { get { return this.XElement.Element(Names.Description).Value; } }
        public string Id { get { return this.XElement.Element(Names.g_id).Value; } }
        public string Condition { get { return this.XElement.Element(Names.g_condition).Value; } }
        public double Price
        {
            get
            {
                var el = this.XElement.Element(Names.g_price);
                double ret = 0;
                if (el != null && double.TryParse(el.Value, out ret))
                {
                    return ret;
                }
                else
                {
                    return -1;
                }

            }
        }
        public string Availability { get { return this.XElement.Element(Names.g_availability).Value; } }
        public string Shipping { get { return this.XElement.Element(Names.g_shipping).Value; } }
        public string Country { get { return this.XElement.Element(Names.g_country).Value; } }
        public string GoogleCategory { get { return this.XElement.Element(Names.g_google_product_category).Value; } }
        public string ProductTypeInternal { get { return this.XElement.Element(Names.g_product_type).Value; } }
        public string CustomLabel0 { get { return this.XElement.Element(Names.g_custom_label_0).Value; } }
        public string CustomLabel1 { get { return this.XElement.Element(Names.g_custom_label_1).Value; } }
        public string CustomLabel2 { get { return this.XElement.Element(Names.g_custom_label_2).Value; } }
        public string CustomLabel3 { get { return this.XElement.Element(Names.g_custom_label_3).Value; } }
        public string CustomLabel4 { get { return this.XElement.Element(Names.g_custom_label_4).Value; } }
        public string Brand { get { return this.XElement.Element(Names.g_brand).Value; } }
        public string ItemGroupId { get { return this.XElement.Element(Names.g_item_group_id).Value; } }
        public string Color_G { get { return this.XElement.Element(Names.g_color).Value; } }
        public string GTIN { get { return this.XElement.Element(Names.g_gtin).Value; } }
        public string ImageLink { get { return this.XElement.Element(Names.g_image_link) != null ? this.XElement.Element(Names.g_image_link).Value : ""; } }
        public string Mpn { get { return this.XElement.Element(Names.g_mpn).Value; } }
        public string ShippingWeight { get { return this.XElement.Element(Names.g_shipping_weight).Value; } }
        public string Length { get { return this.XElement.Element(Names.c_length).Value; } }
        public string Certification { get { return this.XElement.Element(Names.c_certification).Value; } }
        public string Color_C { get { return this.XElement.Element(Names.c_color).Value; } }
        public string CableStyle { get { return this.XElement.Element(Names.c_cable_style).Value; } }
        public string Rating { get { return this.XElement.Element(Names.c_rating).Value; } }

        public Item(XElement x) //: base(x)
        {
            this.XElement = x;
        }

        public static implicit operator Item(XElement x)
        {
            return new Item(x);
        }
    }
    public enum QueryType : int
    {
        Full = 1,
        NoImageLinks = 2,
        BrandIsMRP = 3
    }
    public class ItemList : List<Item>
    {
        private const string OUT_FILE = "outfile.csv";
        private const string XML_FILE_URL = @"http://sewelldirect.com/feeds/sewelldirect_google.xml";
        public XDocument ShoppingFile;
        public ItemList(QueryType type = QueryType.NoImageLinks)
        {
            Load(type);
        }
        public ItemList(int choice = 2)
        {
            Load((QueryType)choice);
        }

        public async void Load(QueryType type = QueryType.NoImageLinks)
        {
            if (this.Count > 0)
                this.Clear();

            using (var client = new HttpClient())
            {
                Task<Stream> streamTask = client.GetStreamAsync(XML_FILE_URL);
                streamTask.Wait();

                using (var stream = streamTask.Result)
                {
                    this.ShoppingFile = XDocument.Load(stream);
                }
            }

            IEnumerable<Item> items;
            if (type == QueryType.NoImageLinks)
            {
                items = (from x in ShoppingFile.Descendants()
                         where
                            x.Name == Names.Item
                            && x.Element(Names.g_image_link) == null
                         select (Item)x);
            }
            else if (type == QueryType.BrandIsMRP)
            {
                items = (from x in ShoppingFile.Descendants()
                         where
                            x.Name == Names.Item
                            && x.Element(Names.g_brand).Value == "MRP"
                         select (Item)x);

            }
            else 
            {
                items = (from x in ShoppingFile.Descendants()
                         where
                            x.Name == Names.Item
                         select (Item)x);
            }
            this.AddRange(items);

        }

        public void Save()
        {
            try
            {
                using (var writer = new StreamWriter(OUT_FILE))
                {
                    writer.WriteLine("g_id,title,g_brand,link,description,image_link");
                    foreach (var x in (IEnumerable<Item>)this)
                    {
                        string line = $"{x.Id},{x.Title},{x.Brand},{x.Link},{x.Description},{x.ImageLink}";
                        //  Console.WriteLine(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error Saving file: {ex.Message}");
            }

        }
    }
}
