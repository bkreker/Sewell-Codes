
-- AdWords
SELECT 
	AnalyticsMedium, 
	AnalyticsSource, 
	ProductPartNumber AS ProductSKU,
	ProductName,
	ProductBrand,
	ProductFamily,
	m.CustomerBioId, 
	CustomerId, 
	CustomerPriceTier, 
	CustomerUniquePurchase, 
	ExtendedWarrantyRevenue, 
	FirstReferer, 
	FirstShipDate, 
	LastReferer, 
	OrderDate, 
	OrderDetailId, 
	OrderEmail, 
	OrderId, 
	OrderPaid, 
	OrderPhone, 
	OrderQuantity, 
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned) AS OrderTotalRevenue,
	(CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalCost,
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned)  - (CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalGP,
	SalesChannel, 
	c.TotalOrders AS CustLifeTimeOrderCount, 
	c.TotalNetRevenue AS CustLifetimeRevenue, 
	c.TotalAdjustedMargin AS CustLifetimeGP,
	1 AS AdvertisingChannelId
FROM 
	MarketingOrders as m JOIN CustomerBios_BI as c 
	on m.CustomerBioId = c.CustomerBioId
WHERE 
	(OrderEmail <> '' AND	
	OrderEmail IS NOT NULL) AND
	
	AnalyticsMedium IN('cpc','cpm')	AND	
	(AnalyticsSource LIKE '%google%' OR 
	AnalyticsSource LIKE '%_d_ords%')

	
UNION
-- Amazon PaidAds
SELECT 
	AnalyticsMedium, 
	AnalyticsSource, 
	ProductPartNumber AS ProductSKU,
	ProductName,
	ProductBrand,
	ProductFamily,
	m.CustomerBioId, 
	CustomerId, 
	CustomerPriceTier, 
	CustomerUniquePurchase, 
	ExtendedWarrantyRevenue, 
	FirstReferer, 
	FirstShipDate, 
	LastReferer, 
	OrderDate, 
	OrderDetailId, 
	OrderEmail, 
	OrderId, 
	OrderPaid, 
	OrderPhone, 
	OrderQuantity, 
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned) AS OrderTotalRevenue,
	(CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalCost,
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned)  - (CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalGP,
	SalesChannel, 
	c.TotalOrders AS CustLifeTimeOrderCount, 
	c.TotalNetRevenue AS CustLifetimeRevenue, 
	c.TotalAdjustedMargin AS CustLifetimeGP,
	7 AS AdvertisingChannelId
FROM 
	MarketingOrders as m JOIN CustomerBios_BI as c 
	on m.CustomerBioId = c.CustomerBioId
WHERE 
	(OrderEmail <> '' AND	
	OrderEmail IS NOT NULL) AND	
	
	(AnalyticsMedium LIKE 'cp%' OR AnalyticsMedium LIKE 'pla')	AND	
	(AnalyticsSource LIKE '%amazon%')
	
UNION
-- Other
SELECT 
	AnalyticsMedium, 
	AnalyticsSource, 
	ProductPartNumber AS ProductSKU,
	ProductName,
	ProductBrand,
	ProductFamily,
	m.CustomerBioId, 
	CustomerId, 
	CustomerPriceTier, 
	CustomerUniquePurchase, 
	ExtendedWarrantyRevenue, 
	FirstReferer, 
	FirstShipDate, 
	LastReferer, 
	OrderDate, 
	OrderDetailId, 
	OrderEmail, 
	OrderId, 
	OrderPaid, 
	OrderPhone, 
	OrderQuantity, 
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned) AS OrderTotalRevenue,
	(CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalCost,
	(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned)  - (CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) AS OrderTotalGP,
	SalesChannel, 
	c.TotalOrders AS CustLifeTimeOrderCount, 
	c.TotalNetRevenue AS CustLifetimeRevenue, 
	c.TotalAdjustedMargin AS CustLifetimeGP,
	0 AS AdvertisingChannelId
FROM 
	MarketingOrders as m JOIN CustomerBios_BI as c 
	on m.CustomerBioId = c.CustomerBioId
WHERE 
	(OrderEmail <> '' AND	
	OrderEmail IS NOT NULL) AND
	
	(AnalyticsSource NOT LIKE '%amazon%') AND
	(AnalyticsSource NOT LIKE '%google%' AND
	AnalyticsSource NOT LIKE '%_d_ords%') AND
	
	(AnalyticsMedium IS NOT NULL AND
	AnalyticsMedium NOT IN ('(not set)','(none)','')) AND	
	(AnalyticsSource IS NOT NULL AND
	AnalyticsSource <> '')
