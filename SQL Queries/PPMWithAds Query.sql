SELECT 
LifeTimeConversionData.Status, 
CombinedIDs.Campaign, 
CombinedIDs.AdGroup, 
Product_Price_Margin.[SW-#s], 
Product_Price_Margin.ProductName, 
Product_Price_Margin.FinalURL, 
Avg(LifeTimeConversionData.Clicks) AS Clicks, 
Avg(LifeTimeConversionData.CTR) AS CTR, 
Avg(LifeTimeConversionData.Conversions) AS Conversions, 
Avg(LifeTimeConversionData.ConvRate) AS ConvRate, 
Avg(LifeTimeConversionData.CostPerConv) AS CostPerConv, 
[Clicks]*[ConvRate] AS ClicksPerConv, 
[Avg_Product_GP]/[ClicksPerConv] AS MaxBid, 
Sum(Product_Price_Margin.Net_Margin) AS Net_Margin, 
Sum(Product_Price_Margin.[GP 52]) AS GP_52, 
[Gp_52]/52 AS GP1, 
Avg(Product_Price_Margin.ProfitPercent) AS ProfitPercent, 
Avg(Product_Price_Margin.AvgProductGP) AS Avg_Product_GP, 
Avg(([GP_Report].[Shipped Units 52]/52)+([GP_Report].[Shipped Units 13]/13)+([GP_Report].[Shipped Units 4]/4)+([GP_Report].[Shipped Units 1])) AS AvgWeeklyShipped
INTO PPM_WithAds

FROM ((Product_Price_Margin 
INNER JOIN LifeTimeConversionData ON Product_Price_Margin.FinalURL = LifeTimeConversionData.ActualURL) 
INNER JOIN CombinedIDs ON LifeTimeConversionData.AdGroupID = CombinedIDs.AdGroupID) 
INNER JOIN GP_Report ON Product_Price_Margin.[SW-#s] = GP_Report.[SW-#]
GROUP BY LifeTimeConversionData.Status, CombinedIDs.Campaign, CombinedIDs.AdGroup, Product_Price_Margin.[SW-#s], Product_Price_Margin.ProductName, Product_Price_Margin.FinalURL;