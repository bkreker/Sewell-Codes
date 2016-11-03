--PPC Totals Last Week by Source
SELECT 
'PPC Total' AS Source, concat(dbo.DaysAgo(7), ' - ', dbo.Today()) AS OrderDateRange, sum(TotalPrice) AS TotalPrice, count(DISTINCT OrderId) AS OrderCount, sum(OrderQuantity) AS OrderQuantity, sum(TotalMaterialCost) AS TotalMaterialCost, sum(TotalPrice - TotalMaterialCost) AS TotalGP, count(DISTINCT ProductPartNumber) AS ProductsOrdered, count(DISTINCT CustomerId) AS CustomerCount
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(7) and dbo.Today() 
AND GoogleAnalyticsMedium LIKE '%cpc%'
UNION 
SELECT
'Google', concat(dbo.DaysAgo(7), ' - ', dbo.Today()) AS OrderDateRange, sum(TotalPrice) AS TotalPrice,count(DISTINCT OrderId) AS OrderCount,  sum(OrderQuantity) AS OrderQuantity, sum(TotalMaterialCost) AS TotalMaterialCost, sum(TotalPrice - TotalMaterialCost) AS TotalGP, count(DISTINCT ProductPartNumber) AS ProductsOrdered, count(DISTINCT CustomerId) AS CustomerCount
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(7) and dbo.Today()
AND GoogleAnalyticsSource LIKE '%Google%'
AND GoogleAnalyticsMedium LIKE '%cpc%'
UNION 
SELECT
'Bing', concat(dbo.DaysAgo(7), ' - ', dbo.Today()) AS OrderDateRange, sum(TotalPrice) AS TotalPrice,count(DISTINCT OrderId) AS OrderCount,  sum(OrderQuantity) AS OrderQuantity, sum(TotalMaterialCost) AS TotalMaterialCost, sum(TotalPrice - TotalMaterialCost) AS TotalGP, count(DISTINCT ProductPartNumber) AS ProductsOrdered, count(DISTINCT CustomerId) AS CustomerCount
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(7) and dbo.Today()
AND GoogleAnalyticsSource LIKE '%Bing%'
AND GoogleAnalyticsMedium LIKE '%cpc%'

-- AdWords Order Totals Last 100 Days
SELECT 
count(distinct OrderId) AS OrderCount, concat(dbo.DaysAgo(100), ' - ', dbo.Today()) AS OrderDateRange, sum(TotalPrice) AS TotalPrice, sum(OrderQuantity) AS OrderQuantity, sum(TotalMaterialCost) AS TotalMaterialCost, sum(TotalPrice - TotalMaterialCost) AS TotalGP, count(DISTINCT ProductPartNumber) AS ProductsOrdered, count(DISTINCT ProductName) AS ProductNames, 'google' AS GoogleAnalyticsSource, 'cpc' AS GoogleAnalyticsMedium, count(DISTINCT CustomerId) AS CustomerCount
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(100) and dbo.Today()
AND GoogleAnalyticsSource LIKE '%Google%' 
AND GoogleAnalyticsMedium LIKE '%cpc%'

--AdWords Orders Last 100 Days
SELECT OrderId, OrderDate, TotalPrice, OrderQuantity, TotalMaterialCost, TotalPrice - TotalMaterialCost AS TotalGP, ProductPartNumber, ProductName, GoogleAnalyticsSource, GoogleAnalyticsMedium, CustomerId
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(100) and dbo.Today()
AND GoogleAnalyticsSource LIKE '%Google%' 
AND GoogleAnalyticsMedium LIKE '%cpc%'

--Order Totals By IDs
SELECT 
   count(distinct OrderId) AS OrderCount, concat(dbo.DaysAgo(100), ' - ', dbo.Today()) AS OrderDateRange, sum(TotalPrice) AS TotalPrice, sum(OrderQuantity) AS OrderQuantity, sum(TotalMaterialCost) AS TotalMaterialCost,     
   sum(TotalPrice - TotalMaterialCost) AS TotalGP, count(DISTINCT ProductPartNumber) AS ProductsOrdered, count(DISTINCT CustomerId) AS CustomerCount
FROM ProductSalesSince2008
WHERE OrderId IN (8735980, 8716177, 8719879, 8721675, 8743319, 8735980, 8735980, 8705747, 8720395, 8729743, 8729743, 8723166, 8720976, 8742894)
