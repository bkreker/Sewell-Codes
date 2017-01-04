using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductNavigator
{
    public partial class Form1 : Form
    {
        string ProductsQuery = @"SELECT DISTINCT p.ProductId,p.ProductPartNumber, MAX(ProductName) AS ProductName, MAX(ProductFamily) AS ProductFamily, MAX(ProductBrand) AS ProductBrand, MAX(w.WebPage) AS ProductPage
                                FROM ProductWebPageLog AS l
	                                JOIN WebPages as w ON w.WebPageId = l.WebPageId
	                                JOIN (SELECT DISTINCT mo.ProductPartNumber, mo.ProductName, f.Name AS ProductFamily,f.Id AS ProductFamilyId, p.ProductId, ProductBrand
		                                FROM MarketingOrders AS mo 
		                                JOIN Sewell_Products.dbo.ProductFamilyMembers as m ON m.ProductId = mo.ProductId
		                                JOIN Sewell_Products.dbo.ProductFamilies as f ON m.ProductFamilyId = f.Id
		                                JOIN Sewell_Products.dbo.Products as p ON p.ProductId = m.ProductId) AS p ON p.ProductId = l.ProductId
                                GROUP BY p.ProductId,p.ProductPartNumber";

        public Form1()
        {
            InitializeComponent();
        }
    }
}
