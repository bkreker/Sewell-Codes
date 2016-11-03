using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMining
{
    public class Stat
    {
        public string Name { get; set; }
        public double? Value { get; set; }
        public StatType? StatType { get; set; }

        public Stat()
        {
            Name = "Unassigned";
            Value = null;
            StatType = null;
        }

        public Stat(List<string> row)
        {

        }

        public Stat(string value, StatType statType)
        {
            double temp;
            if (double.TryParse(value, out temp))
            {
                this.Value = temp;
            }
            this.Name = statType.ToString();
            this.StatType = statType;
        }
    }
}