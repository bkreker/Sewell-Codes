SELECT ProductPartNumber, ProductName, ProductFamily, ProductBrand, p.WebPage
FROM ProductWebPageLog AS l
	JOIN WebPages as p ON p.WebPageId = l.WebPageId
	JOIN (SELECT DISTINCT mo.ProductPartNumber, mo.ProductName, f.Name AS ProductFamily,f.Id AS ProductFamilyId, p.ProductId, ProductBrand
		FROM MarketingOrders AS mo 
		JOIN Sewell_Products.dbo.ProductFamilyMembers as m ON m.ProductId = mo.ProductId
		JOIN Sewell_Products.dbo.ProductFamilies as f ON m.ProductFamilyId = f.Id
		JOIN Sewell_Products.dbo.Products as p ON p.ProductId = m.ProductId) AS pr ON pr.ProductId = l.ProductId