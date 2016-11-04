using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryMining
{
    public abstract class TopStat
    {
        public abstract string getName();
        public abstract object getValue();
        public abstract StatType? getType();
    }

    public class Stat<T> : TopStat
    {
        public string Name { get; set; }
        public T Value { get; set; }
        public StatType? StatType { get; set; }

        public Stat()
        {
            Name = "-";
            Value = default(T);
            StatType = null;
        }
        public Stat(T value, StatType statType)
        {
            Name = statType.ToString();
            Value = value;
            StatType = statType;
        }

        public override object getValue()
        {
            return this.Value;
        }
        public override string getName()
        {
            return this.Name;
        }
        public override StatType? getType()
        {
            return this.StatType;
        }
        
       

    }
}