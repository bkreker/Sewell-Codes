SELECT DISTINCT 
	p.ProductId,
	p.ProductPartNumber, 
	MAX(ProductName) AS ProductName, 
	MAX(ProductFamily) AS ProductFamily, 
	MAX(ProductBrand) AS ProductBrand, 
	MAX(w.WebPage) AS ProductPage
FROM ProductWebPageLog AS l
	JOIN WebPages as w ON w.WebPageId = l.WebPageId
	JOIN (SELECT DISTINCT mo.ProductPartNumber, mo.ProductName, f.Name AS ProductFamily,f.Id AS ProductFamilyId, p.ProductId, ProductBrand
		FROM MarketingOrders AS mo 
		JOIN Sewell_Products.dbo.ProductFamilyMembers as m ON m.ProductId = mo.ProductId
		JOIN Sewell_Products.dbo.ProductFamilies as f ON m.ProductFamilyId = f.Id
		JOIN Sewell_Products.dbo.Products as p ON p.ProductId = m.ProductId) AS p ON p.ProductId = l.ProductId
GROUP BY p.ProductId,p.ProductPartNumber


--SELECT DISTINCT ProductPartNumber, f.Name AS ProductFamily,f.Id AS ProductFamilyId, p.ProductId, ProductBrand
--FROM ProductFamilyMembers as m
--	JOIN ProductFamilies as f ON m.ProductFamilyId = f.Id
--	JOIN Products as p ON p.ProductId = m.ProductId
--	JOIN(SELECT DISTINCT ProductPartNumber,ProductId, ProductBrand
--	FROM Sewell.dbo.MarketingOrders) AS mo ON mo.ProductId = m.ProductId

--SELECT DISTINCT mo.ProductPartNumber, f.Name AS ProductFamily,f.Id AS ProductFamilyId, p.ProductId, ProductBrand
--	FROM MarketingOrders AS mo 
--	JOIN Sewell_Products.dbo.ProductFamilyMembers as m ON m.ProductId = mo.ProductId
--	JOIN Sewell_Products.dbo.ProductFamilies as f ON m.ProductFamilyId = f.Id
--	JOIN Sewell_Products.dbo.Products as p ON p.ProductId = m.ProductId
