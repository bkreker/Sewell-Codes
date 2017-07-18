SELECT PS.DateReported, P.ProductName,  
	SUM(PS.Users) AS Users, SUM(PS.NewUsers) AS NewUsers, SUM(PS.Sessions) AS Sessions, AVG(PS.PageValue) AS PageValue
FROM GoogleAnalyticsPageStatistics PS
	JOIN GoogleAnalyticsDomains D ON PS.DomainId = D.DomainId
	JOIN GoogleAnalyticsPagePaths PP ON PP.PathId = PS.PathId
	JOIN GoogleAnalyticsMediums M ON M.MediumId = PS.MediumId
	JOIN GoogleAnalyticsSources S ON S.SourceId = PS.SourceId
	LEFT JOIN WebPages W ON W.WebPageId = PP.WebPageId
	LEFT JOIN Products P ON PP.ProductId = P.ProductId
GROUP BY PS.DateReported, P.ProductName
ORDER BY PS.DateReported, P.ProductName