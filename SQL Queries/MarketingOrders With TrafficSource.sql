-- MarketingOrders
SELECT 	mo.OrderDetailId, 
	mo.OrderId, 
	mo.OrderDate, 
	mo.CustomerId, 
	mo.CustomerUniquePurchase, 
	mo.OrderEmail, 
	mo.OrderTotalRevenue,
	mo.OrderTotalCost,
	mo.OrderTotalRevenue - mo.OrderTotalCost AS OrderTotalGP,
	mo.OrderQTY,
	mo.TrafficSourceId,
	mo.ProductId, 
	mo.AnalyticsMedium,
	mo.AnalyticsSource,
	CAST(	
		CASE WHEN mo.OrderTotalRevenue > 
		(SELECT 
			AVG(m.OrderTotalRevenue) + STDEV(m.OrderTotalRevenue) * 2
			FROM (SELECT 
					RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned
					AS OrderTotalRevenue
					FROM Sewell.dbo.MarketingOrders
					WHERE CustomerUniquePurchase > 1) 
				AS m)
		THEN 1 ELSE 0 END 
	AS BIT)  AS MoIsRevOutlier,
	CAST(	
		CASE WHEN OrderGP > 
		(SELECT 
			AVG(m.OrderTotalGP) + STDEV(m.OrderTotalGP) * 2
			FROM (SELECT 
					(RevenueOfGoodsShipped + ShippingRevenue - RevenueOfGoodsReturned)  
					- (CostOfGoodsShipped + TransactionCosts + ShippingCost - CostOfGoodsReturned) 
						AS OrderTotalGP
					FROM Sewell.dbo.MarketingOrders
					WHERE CustomerUniquePurchase > 1)
			AS m)
		THEN 1 ELSE 0 END 
	AS BIT)  AS MoIsGpOutlier,
	CAST(	
		CASE WHEN mo.OrderTotalCost > 
		(SELECT 
			AVG(m.OrderTotalCost) + STDEV(m.OrderTotalCost) * 2
			FROM (SELECT 
						CostOfGoodsShipped 
						+ TransactionCosts 
						+ ShippingCost 
						- CostOfGoodsReturned
						AS OrderTotalCost
					FROM Sewell.dbo.MarketingOrders
					WHERE CustomerUniquePurchase > 1) AS m)
		THEN 1 ELSE 0 END 
	AS BIT)  AS MoIsCostOutlier
FROM (
	SELECT 	
		OrderDetailId, 
		OrderId, 
		OrderDate, 
		CustomerId, 
		CustomerUniquePurchase, 
		OrderQuantity AS OrderQTY,
		ProductId, ProductName, ProductPartNumber, ProductFamily, ProductBrand,
		OrderEmail, 
		(RevenueOfGoodsShipped 
			+ ShippingRevenue 
			- RevenueOfGoodsReturned) 
			AS OrderTotalRevenue,
		(CostOfGoodsShipped 
			+ TransactionCosts 
			+ ShippingCost 
			- CostOfGoodsReturned)
		AS OrderTotalCost,
		(RevenueOfGoodsShipped 
			+ ShippingRevenue 
			- RevenueOfGoodsReturned) 
		-(CostOfGoodsShipped 
			+ TransactionCosts 
			+ ShippingCost 
			- CostOfGoodsReturned)
		AS OrderGP,
		(CASE 
			WHEN 
				(AnalyticsSource LIKE '%google%' OR 
				AnalyticsSource LIKE '%_d_ords%')
			THEN 1
			WHEN 
				AnalyticsSource LIKE '%amazon%'
			THEN 54
			WHEN 
				AnalyticsSource LIKE '%[Bb]ing%'  
			THEN 2
			WHEN
				AnalyticsSource LIKE '%[Yy]ahoo%'
			THEN 4
			ELSE 0 
		END) 
		AS TrafficSourceId,
		AnalyticsMedium,
		AnalyticsSource
	FROM Sewell.dbo.MarketingOrders as m)
 AS mo 

