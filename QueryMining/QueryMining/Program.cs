using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QueryMining
{
    static class Program
    {
        public static bool AvgAll { get; set; }
        public static bool Processing { get; set; }
        public static bool OperationCancelled { get; set; }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Processing = false;
            OperationCancelled = false;
            AvgAll = true;
            Application.Run(new MainForm());
        }
    }
}
