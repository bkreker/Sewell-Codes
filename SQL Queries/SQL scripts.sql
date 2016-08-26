GP_Score: (((First([Product_Price_Margin].[AvgProductGP])-First([PPMMaxAndMin].[MinGP]))/(First([PPMMaxAndMin].[MaxGP])-First([PPMMaxAndMin].[MinGP])))*100)
ConvRate_Score: (((First([Product_Price_Margin].[ConversionRate])-First([PPMMaxAndMin].[MinConvRate]))/(First([PPMMaxAndMin].[MaxConvRate])-First([PPMMaxAndMin].[MinConvRate])))*100)
CPConv_Score: ((((First([Product_Price_Margin].[ClicksPerConv])-First([PPMMaxAndMin].[MinCPA]))/(First([PPMMaxAndMin].[MaxCPA])-First([PPMMaxAndMin].[MinCPA])))*-100)+100)
MaxBid_Score: (((First([Product_Price_Margin].[MaxBid])-First([PPMMaxAndMin].[MinBid]))/(First([PPMMaxAndMin].[MaxBid])-First([PPMMaxAndMin].[MinBid])))*100)
Viability_Score: ((([RawScore]-First([RawTotals].[Min_RawScore]))/ 
						(First([RawTotals].[Max_RawScore]) - First([RawTotals].[Min_RawScore])))*100)
Viability_Score: ((([RawScore] -First([RawTotals].[Min_RawScore]))/(First([RawTotals].[Max_RawScore])-First([RawTotals].[Min_RawScore])))*100)
SELECT Max(Viability_Scores.RawScore) AS Max_RawScore, Min(Viability_Scores.RawScore) AS Min_RawScore
FROM Viability_Scores;
Viability_Score: ([RawScore]-Min([RawScore])/(Max([RawScore])-Min([RawScore]))*100)
ANet_Margin: (([Product_Price_Margin].[Net_Margin]-DMin("[Net_Margin]","[Product_Price_Margin]")/(DMax("[Net_Margin]","[Product_Price_Margin]")-DMin("[Net_Margin]","[Product_Price_Margin]")))*100)
VolumeScore: (((First([Product_Price_Margin].[AvgClicks]))+First([Product_Price_Margin].[Net_Margin])+First([Product_Price_Margin].[AvgWeeklyShipped])-First([PPMMaxAndMin].[MinVolume]))/(First([PPMMaxAndMin].[MaxVolume])-First([PPMMaxAndMin].[MinVolume]))*100)
AvgProductGP: IIf([GP_Report].[Avg_Product_GP] >0,  [GP_Report].[Avg_Product_GP], IIf([COGS_PerUnit] >0, [Price]-[COGS_PerUnit], [Price] /2))