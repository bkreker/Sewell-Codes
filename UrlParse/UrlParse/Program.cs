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

            Console.ReadLine();
            string fileName = defaultLocation;
            StreamReader inputFile = File.OpenText(fileName);

            inputFile.ReadLine();
            while (!inputFile.EndOfStream)
            {
                LandingPage page = new LandingPage();
                string row = inputFile.ReadLine();
                string url = row.Split('?')[0];
                string parameters = row.Split('?')[1];
                var thing = parameters.Split('&');
                for (int i = 0; i < thing.Length; i++)
                {
                    string param = thing[i];
                    string title = thing[i].Split('=')[0];
                    string value = thing[i].Split('=')[1];


                }

                page.url = url;

                lstUrls.Add(page);
            }
        }
    }

}
