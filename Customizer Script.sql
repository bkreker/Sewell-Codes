SELECT ActiveProducts_with_Pricing.SKU AS [SKU (text)], ActiveProducts_with_Pricing.Price AS [Price (price)], CombinedIDs.Adgroup AS [Target ad group], CombinedIDs.Campaign AS [Target campaign]
FROM 
(AdsWithURLs INNER JOIN CombinedIDs ON AdsWithURLs.AdGroupID = CombinedIDs.AdGroupID) 
INNER JOIN (ActiveProducts_with_Pricing 
INNER JOIN item ON ActiveProducts_with_Pricing.SKU = item.id) ON AdsWithURLs.FinalURL = item.link
GROUP BY ActiveProducts_with_Pricing.SKU, ActiveProducts_with_Pricing.Price, CombinedIDs.Adgroup, CombinedIDs.Campaign, CombinedIDs.Status
HAVING (((CombinedIDs.Status)<>"removed"));
