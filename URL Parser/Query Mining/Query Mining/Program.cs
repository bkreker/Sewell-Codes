using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Design;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Collections;
using System.Windows.Markup;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace Query_Mining
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Console.WriteLine("Select the csv file to upload.");            
            Application.Run(new Form1());

        }
    }
}
