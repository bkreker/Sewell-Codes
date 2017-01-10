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
    class Program
    {
        static XNamespace C_XMLNS = "http://base.google.com/cns/1.0";
        static XNamespace G_XMLNS = "http://base.google.com/ns/1.0";
        static XName item = $"item";
        static XName title = $"title";
        static XName link = $"link";
        static XName description = $"description";
        static XName g_id = $"{{{G_XMLNS}}}id";
        static XName g_condition = $"{{{G_XMLNS}}}condition";
        static XName g_price = $"{{{G_XMLNS}}}price";
        static XName g_availability = $"{{{G_XMLNS}}}availability";
        static XName g_online_only = $"{{{G_XMLNS}}}online_only";
        static XName g_shipping = $"{{{G_XMLNS}}}shipping";
        static XName g_country = $"{{{G_XMLNS}}}country";
        static XName g_google_product_category = $"{{{G_XMLNS}}}google_product_category";
        static XName g_product_type = $"{{{G_XMLNS}}}product_type";
        static XName g_custom_label_0 = $"{{{G_XMLNS}}}custom_label_0";
        static XName g_custom_label_1 = $"{{{G_XMLNS}}}custom_label_1";
        static XName g_custom_label_2 = $"{{{G_XMLNS}}}custom_label_2";
        static XName g_custom_label_3 = $"{{{G_XMLNS}}}custom_label_3";
        static XName g_custom_label_4 = $"{{{G_XMLNS}}}custom_label_4";
        static XName g_brand = $"{{{G_XMLNS}}}brand";
        static XName g_item_group_id = $"{{{G_XMLNS}}}item_group_id";
        static XName g_color = $"{{{G_XMLNS}}}color";
        static XName g_gtin = $"{{{G_XMLNS}}}gtin";
        static XName g_image_link = $"{{{G_XMLNS}}}image_link";
        static XName g_mpn = $"{{{G_XMLNS}}}mpn";
        static XName g_shipping_weight = $"{{{G_XMLNS}}}shipping_weight";
        static XName c_length = $"{{{C_XMLNS}}}length";
        static XName c_certification = $"{{{C_XMLNS}}}certification";
        static XName c_color = $"{{{C_XMLNS}}}color";
        static XName c_cable_style = $"{{{C_XMLNS}}}cable_style";
        static XName c_rating = $"{{{C_XMLNS}}}rating";


        const string XML_FILE_URL = @"http://sewelldirect.com/feeds/sewelldirect_google.xml";
        static XDocument ShoppingFile;
        static void Main(string[] args)
        {
            Console.WriteLine("Getting file...");
            try
            {
                LoadXML();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Console.WriteLine(ex.Message);
                }
            }
            Console.WriteLine($"Finished. Saved at {OUT_FILE}");
            Console.ReadLine();
        }
        const string OUT_FILE = "outfile.txt";
        private static async void LoadXML()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    using (var stream = await client.GetStreamAsync(XML_FILE_URL))
                    {
                        ShoppingFile = XDocument.Load(stream);
                    }
                }

                var items = (from x in ShoppingFile.Descendants()
                             where 
                                x.Name == item
                                && x.Element(g_image_link) == null
                             select x).ToList();
                using (var writer = new StreamWriter(OUT_FILE))
                {

                    foreach (var x in items)
                    {
                        string line = $"{x.Element(g_id).Value},{x.Element(title).Value}";
                        Console.WriteLine(line);
                        writer.WriteLine(line);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    Console.WriteLine(ex.Message);
                }
            }

        }
    }
}
