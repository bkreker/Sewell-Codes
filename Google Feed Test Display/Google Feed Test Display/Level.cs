using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleTaxonomyViewer
{
    public class Level
    {
        public TaxonomyList<string> Parents { get; set; }
        public string Name { get; set; }
        public string Ancestor { get; set; }
        public int Id { get; set; }
        public string FullName { get; set; }
        public string ToolTipText
        {
            get
            {
                return $"{this.FullName}";
            }
        }

        public Level()
        {
            this.Name = "";
            this.Id = -1;
            this.Ancestor = "";
            this.Parents = new TaxonomyList<string>();
            this.FullName = "";

        }
        public Level(string name, int id, string ancestor, TaxonomyList<string> parents, string fullName)
        {
            this.Name = name;
            this.Id = id;
            this.Ancestor = ancestor;
            this.Parents = parents;
            this.FullName = fullName;
        }


        public override string ToString()
        {
            return $"{this.Name} ({this.Id})";
        }
    }

}
