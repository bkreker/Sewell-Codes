using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace Query_Mining
{
    public class DataTable : Dictionary<string[], List<string[]>>
    {
        public string[] Headers { get; set; }
        public DataTable()
        {

        }
    }

}
