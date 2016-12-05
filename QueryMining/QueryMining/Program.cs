using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using QueryMining.Forms;

namespace QueryMining
{
    static class Program
    {
        public static bool AvgAll { get; set; }
        public static bool Processing { get; set; }
        public static bool OperationCancelled { get; set; }
        public static MineType MineType { get; set; }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                Processing = false;
                OperationCancelled = false;
                AvgAll = true;
                Application.Run(new MainForm());
            }
            catch (Exception ex)
            {
                MessageBox.Show($" Message: {ex.Message}\nSource: {ex.Source}\nTarget Site: {ex.TargetSite}\nStackTrace: {ex.StackTrace}","Fatal Error Running application");
            }
        }
    }

    public enum MineType { One, Two, Three }
}
