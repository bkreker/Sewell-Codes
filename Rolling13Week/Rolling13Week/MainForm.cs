using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rolling13Week
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            TimeSpan _13_Weeks = new TimeSpan(91, 0, 0, 0, 0);
            DateTime
                today = DateTime.Today.Subtract(new TimeSpan(1, 0, 0, 0)).Date,
                _13WeeksAgo = today.Subtract(_13_Weeks).Date;
            // 13 weeks is 91 days

            calCurrent.SelectionStart = _13WeeksAgo;
            calCurrent.SelectionEnd = today;

            DateTime
                lastWeek = DateTime.Today.Subtract(new TimeSpan(7, 0, 0, 0)).Date,
                _14WeeksAgo = lastWeek.Subtract(_13_Weeks).Date;

            calPrev.SelectionStart = _14WeeksAgo;
            calPrev.SelectionEnd = lastWeek;
            gbCurrent.Text = _13WeeksAgo.ToShortDateString() + " - " + today.ToShortDateString();
            gbPast.Text = _14WeeksAgo.ToShortDateString() + " - " + lastWeek.ToShortDateString();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            DateTime d = new DateTime(2016, 11, 21, 0, 0, 0, 0, DateTimeKind.Utc);
            d.AddHours(7);
            string timeStr = d.ToString("yyyy-MM-ddTHH:mm:ss") + "+07:00";
            string str = $"{d.ToString("yyyy-MM-dd")}T{d.ToString("HH:mm:ss")}+07:00";

            Console.WriteLine(timeStr);
            Console.WriteLine(str);




        }
    }
}
