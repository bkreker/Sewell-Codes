"use strict";
exports.__esModule = true;
import {} from '../typings/index'
/**
 * @name Search Query Report
 *
 * @overview The Search Query Report script uses the Search Query Performance
 *     Report to find undesired search terms and add them as negative (or
 *     positive) exact keywords. See
 *     https://developers.google.com/adwords/scripts/docs/solutions/search-query
 *     for more details.
 *
 * @author AdWords Scripts Team [adwords-scripts@googlegroups.com]
 *
 * @version 1.0.1
 *
 * @changelog
 * - version 1.0.1
 *   - Upgrade to API version v201609.
 * - version 1.0
 *   - Released initial version.
 */
// Minimum number of impressions to consider "enough data"
var IMPRESSIONS_THRESHOLD = 100;
// Cost-per-click (in account currency) we consider an expensive keyword.
var AVERAGE_CPC_THRESHOLD = 1; // $1
// Threshold we use to decide if a keyword is a good performer or bad.
var CTR_THRESHOLD = 0.5; // 0.5%
// If ctr is above threshold AND our conversion cost isn’t too high,
// it’ll become a positive keyword.
var COST_PER_CONVERSION_THRESHOLD = 10; // $10
// One currency unit is one million micro amount.
var MICRO_AMOUNT_MULTIPLIER = 1000000;
/**
 * Configuration to be used for running reports.
 */
var REPORTING_OPTIONS = {
    // Comment out the following line to default to the latest reporting version.
    apiVersion: 'v201705'
};
function main() {
    var report = AdWordsApp.report('SELECT Query, Clicks, Cost, Ctr, ConversionRate,' +
        ' CostPerConversion, Conversions, CampaignId, AdGroupId ' +
        ' FROM SEARCH_QUERY_PERFORMANCE_REPORT ' +
        ' WHERE ' +
        ' Conversions > 0' +
        ' AND Impressions > ' + IMPRESSIONS_THRESHOLD +
        ' AND AverageCpc > ' +
        (AVERAGE_CPC_THRESHOLD * MICRO_AMOUNT_MULTIPLIER) +
        ' DURING LAST_7_DAYS', REPORTING_OPTIONS);
    var rows = report.rows();
    var negativeKeywords = {};
    var positiveKeywords = {};
    var allAdGroupIds = {};
    // Iterate through search query and decide whether to
    // add them as positive or negative keywords (or ignore).
    while (rows.hasNext()) {
        var row = rows.next();
        if (parseFloat(row['Ctr']) < CTR_THRESHOLD) {
            addToMultiMap(negativeKeywords, row['AdGroupId'], row['Query']);
            allAdGroupIds[row['AdGroupId']] = true;
        }
        else if (parseFloat(row['CostPerConversion']) < COST_PER_CONVERSION_THRESHOLD) {
            addToMultiMap(positiveKeywords, row['AdGroupId'], row['Query']);
            allAdGroupIds[row['AdGroupId']] = true;
        }
    }
    // Copy all the adGroupIds from the object into an array.
    var adGroupIdList = [];
    for (var adGroupId in allAdGroupIds) {
        adGroupIdList.push(parseFloat(adGroupId));
    }
    // Add the keywords as negative or positive to the applicable ad groups.
    var adGroups = AdWordsApp.adGroups().withIds(adGroupIdList).get();
    while (adGroups.hasNext()) {
        var adGroup = adGroups.next();
        var adGroupId = adGroup.getId().toString();
        var adGroupName = adGroup.getName();
        if (negativeKeywords[adGroupId]) {
            for (var i = 0; i < negativeKeywords[adGroupId].length; i++) {
                var newNeg = negativeKeywords[adGroupId][i];
                Logger.log('Adding -negative keyword: "' + newNeg + '" to AdGroup "' + adGroupName + '"');
                adGroup.createNegativeKeyword('[' + newNeg + ']');
            }
        }
        if (positiveKeywords[adGroupId]) {
            for (var i = 0; i < positiveKeywords[adGroupId].length; i++) {
                var newKw = positiveKeywords[adGroupId][i];
                Logger.log('Adding +positive keyword: "' + newKw + '" to AdGroup "' + adGroupName + '"');
                var keywordOperation = adGroup.newKeywordBuilder()
                    .withText('[' + newKw + ']')
                    .build();
            }
        }
    }
}
function addToMultiMap(map, key, value) {
    if (!map[key]) {
        map[key] = [];
    }
    map[key].push(value);
}
