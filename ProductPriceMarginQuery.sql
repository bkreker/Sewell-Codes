SELECT 
ActiveProducts_with_Pricing.ProductName, 
SKUsWithMargins.[SW-#s], 
Avg(ActiveProducts_with_Pricing.Price) AS Price, 
Avg(SKUsWithMargins.COGS_PerUnit) AS COGS_PerUnit, 
Avg(GP_Report.[Avg Product GP]) AS AvgProductGP, 
Avg(([GP_Report].[Shipped Units 52]/52)+([GP_Report].[Shipped Units 13]/13)+([GP_Report].[Shipped Units 4]/4)+([GP_Report].[Shipped Units 1])) AS AvgWeeklyShipped, 
Sum(GP_Report.[GP 52]) AS [GP 52], 
Sum(SKUsWithMargins.Net_Margin) AS Net_Margin, 
[AvgProductGP]/[Price] AS ProfitPercent, 
Sum(ContentDrilldown.Sessions) AS Sessions, 
Sum(ContentDrilldown.UniquePageviews) AS UniquePageviews, 
([Sessions]+[UniquePageviews])/2 AS AvgClicks, 
Sum(ProductPerformance.Unique_Purchases) AS Unique_Purchases, 
[Unique_Purchases]/52 AS UniquePurch_PerWeek, 
[AvgClicks]/[Unique_Purchases] AS ClicksPerConv,
[Unique_Purchases]/[UniquePageviews] AS ConversionRate, 
[AvgProductGP]/[ClicksPerConv] AS MaxBid, 
Avg(ProductPerformance.Quantity) AS Quantity, 
Avg(ProductPerformance.Average_QTY) AS Average_QTY_PerOrder, 
ActiveProducts_with_Pricing.FinalUrl 
INTO Product_Price_Margin
FROM ((ActiveProducts_with_Pricing 
INNER JOIN SKUsWithMargins ON ActiveProducts_with_Pricing.SKU = SKUsWithMargins.[SW-#s]) 
INNER JOIN GP_Report ON ActiveProducts_with_Pricing.SKU = GP_Report.[SW-#]) 
INNER JOIN (ProductPerformance 
INNER JOIN ContentDrilldown ON ProductPerformance.Page = ContentDrilldown.Page) ON SKUsWithMargins.[SW-#s] = ProductPerformance.Product_SKU
WHERE (((SKUsWithMargins.Shipped_Units)>0))
GROUP BY ActiveProducts_with_Pricing.ProductName, SKUsWithMargins.[SW-#s], ActiveProducts_with_Pricing.FinalUrl;
