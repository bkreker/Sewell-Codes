using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq.Mapping;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Serialization.Advanced;
using System.Xml.Serialization.Configuration;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Resolvers;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XmlConfiguration;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;

namespace GoogleTaxonomyViewer
{

    public class TaxonomyList<T> : LinkedList<T>
    {
        public TaxonomyList() : base()
        {
        }

        public TaxonomyList(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                this.Push(item);
            }
        }

        public void AddRange(IEnumerable<T> range)
        {
            foreach (var item in range)
            {
                this.Push(item);
            }
        }
        /// <summary>
        /// Adds the new object to the End of the list
        /// </summary>
        /// <param name="newVal">The object to push onto the Stack. The value can be null for reference types.</param>            
        public void Push(T newVal)
        {
            base.AddLast(newVal);
        }

        /// <summary>
        /// Removes and returns the Last Object in the list
        /// </summary>
        /// <returns>The object removed from the top of the Stack</returns>
        /// <exception cref="InvalidOperationException">The Stack is empty.</exception>
        public T Pop_Last()
        {
            var last = base.Last.Value;
            base.RemoveLast();
            return last;
        }


        /// <summary>
        /// Removes and returns the First object in the list
        /// </summary>
        /// <returns>
        /// The object that is removed from the beginning of the Queue
        /// </returns>
        /// <exception cref="InvalidOperationException">The Queue is empty.</exception>
        public T Pop_First()
        {
            var first = base.First.Value;
            base.RemoveFirst();
            return first;
        }

    }

}
