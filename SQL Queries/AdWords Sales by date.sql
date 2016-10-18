SELECT 
	datepart(Year, OrderDate) AS 'Year', 
	datepart(Month, OrderDate) AS 'Month', 
	datepart(Week, OrderDate) AS 'Week', 
	--datepart(Day, OrderDate) AS 'Day', 
	sum(TotalPrice) AS 'Revenue', 
	sum(TotalMaterialCost) AS 'COGS', 
	sum(TotalPrice)-avg(TotalMaterialCost) AS 'GP'
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(365) and dbo.Today()
	AND GoogleAnalyticsSource LIKE '%Google%' 
	AND GoogleAnalyticsMedium LIKE '%cpc%'
GROUP BY 
	datepart(Year, OrderDate), 
	datepart(Month, OrderDate), 
	datepart(week, OrderDate)
	--,datepart(Day, OrderDate) 
ORDER BY 
	datepart(Year, OrderDate), 
	datepart(Month, OrderDate), 
	datepart(week, OrderDate)
	--,datepart(Day, OrderDate) 
	
	SELECT OrderId, OrderDate, TotalPrice, OrderQuantity, TotalMaterialCost, ProductPartNumber, ProductName, GoogleAnalyticsSource,GoogleAnalyticsMedium,CustomerId
FROM ProductSalesSince2008
WHERE OrderDate BETWEEN dbo.DaysAgo(100) and dbo.Today()
AND (GoogleAnalyticsSource LIKE '%google%' OR GoogleAnalyticsSource LIKE '%bing%' )
AND GoogleAnalyticsMedium LIKE '%cpc%'