using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace UrlParse
{
    class Program
    {
        static void Main(string[] args)
        {
            List<LandingPage> lstUrls = new List<LandingPage>();

            //
            string defaultLocation = "C: \\Users\\jdegr_000\\OneDrive\\Work Files\\Analysis\\Google Analytics\\landing page report.csv";
            Console.WriteLine("Enter the full path of the csv file to be parsed");
            Console.Write(">");

            Console.ReadLine();
            string fileName = defaultLocation;
            StreamReader inputFile = File.OpenText(fileName);

            string titles = inputFile.ReadLine();
            while (!inputFile.EndOfStream)
            {
                LandingPage page = new LandingPage();
                string row = inputFile.ReadLine();
                string url = row.Split('?')[0];
                string urlString = "";
                try
                {
                    urlString = row.Split('?')[1];
                }
                catch
                {
                    urlString = row.Split(',')[0];
                }
                var paramsAndValues = urlString.Split(',');
                string parameters = paramsAndValues[0];
                string values = "";

                for (int i = 1; i < paramsAndValues.Length; i++)
                {
                    values += paramsAndValues[i] + ",";
                }

                var information = parameters.Split('&');

                for (int i = 0; i < information.Length; i++)
                {
                    string param = information[i];
                    string title = param.Split('=')[0];
                    try
                    {
                        string paramValue = param.Split('=')[1];
                        AddParam(ref title, ref paramValue, ref page);
                    }
                    catch
                    {
                        break;
                    }
                }

                page.url = url;
                page.values = values;
                lstUrls.Add(page);
            }

            inputFile.Close();

            try
            {

                Console.WriteLine("Write the folderName to save the new file.");
                Console.Write(">");
                string outputFileName = Console.ReadLine();
                StreamWriter outputFile = File.CreateText(outputFileName);
                outputFile.WriteLine(titles);
                for (int i = 0; i < lstUrls.Count; i++)
                {
                    outputFile.WriteLine(lstUrls[i].Concatenate());
                }
                outputFile.Close();
                Console.WriteLine("File written at " + outputFileName);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private static void AddParam(ref string title, ref string paramValue, ref LandingPage page)
        {
            switch (title)
            {
                case "stm_type":
                    page.stm_type = paramValue;
                    break;
                case "stm_source":
                    page.stm_source = paramValue;
                    break;
                case "stm_sku":
                    page.stm_sku = paramValue;
                    break;
                case "product_id":
                    page.product_id = paramValue;
                    break;
                case "matchtype":
                    page.matchtype = paramValue;
                    break;
                case "targetid":
                    page.targetid = paramValue;
                    break;
                case "feeditemid":
                    page.feeditemid = paramValue;
                    break;
                case "creative":
                    page.creative = paramValue;
                    break;
                default:
                    break;
            }
        }
    }

}
