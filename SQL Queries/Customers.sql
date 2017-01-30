  -- Customers	
SELECT DISTINCT PrimaryCustomerID AS CustomerID,
	MAX(TotalOrders) AS LifetimeOrderCount,
	MAX(TotalNetRevenue) AS LifeTimeRevenue, 
	MAX(TotalAdjustedMargin) AS LifetimeGP,
  CAST(MAX(BusinessAccount + 0) AS BIT) AS BusinessAccount,
     CAST(MAX(c.Created+ 0) AS BIT) AS CustomerAccountCreated, 
	CAST(	
		CASE WHEN Sum(TotalAdjustedMargin) >= 
			(SELECT AVG(TotalAdjustedMargin) + STDEV(TotalAdjustedMargin) FROM CustomerBios_BI  WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT) 
	AS GPHasHighLifetime,
	CAST(	
		CASE WHEN Sum(TotalNetRevenue) >= 
			(SELECT AVG(TotalNetRevenue) + STDEV(TotalNetRevenue) FROM CustomerBios_BI WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT) 
	AS RevHasHighLifetime,
	CAST(	
		CASE WHEN Sum(TotalOrders) >= 
			(SELECT AVG(TotalOrders) + STDEV(TotalOrders) FROM CustomerBios_BI WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT) 
	AS OrderCountHasHighLifetime,
	CAST(	
		CASE 
			WHEN Sum(TotalNetRevenue) > 
			(SELECT AVG(TotalNetRevenue) + STDEV(TotalNetRevenue) * 2 FROM CustomerBios_BI WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT) 
	AS RevIsOutlier,
	CAST(	
		CASE WHEN Sum(TotalAdjustedMargin) > 
			(SELECT AVG(TotalAdjustedMargin) + STDEV(TotalAdjustedMargin) * 2 FROM CustomerBios_BI WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT) 
	AS GPIsOutlier,
	CAST(	
		CASE WHEN Sum(TotalOrders) > 
			(SELECT AVG(TotalOrders) + STDEV(TotalOrders) * 2 FROM CustomerBios_BI WHERE TotalOrders > 2)
		THEN 1 ELSE 0 END AS BIT)  
		AS OrderCountIsOutlier	,
	MAX(a.FirstName) AS FirstName,
	MAX(a.LastName) AS LastName,
	MAX(a.AddressId) AS AddressId
FROM CustomerBios_BI as b
	JOIN Customers_BI as c ON b.CustomerBioId = c.CustomerBioId
	JOIN CustomerAddresses_BI as a ON a.CustomerId = c.CustomerId
WHERE TotalOrders > 2
GROUP BY PrimaryCustomerID