/**
*
* Search Query Mining Tool
*
* This script calculates the contribution of each word found in the search query report
* and outputs a report into a Google Doc spreadsheet.
*
* Version: 1.0
* Google Apps Script maintained on brainlabsdigital.com
*
**/
// Options  
var START_DATE = "2015-04-01";
var END_DATE = "2016-09-01";
// The start and end date of the date range for your search query data
// Format is yyyy-mm-dd

var CURRENCY_SYMBOL = "$";
// The currency symbol used for formatting. For example "£", "$" or "€".

var CAMP_NAME_CONTAINS = "HDMI Shopping";
// Use this if you only want to look at some campaigns
// such as campaigns with names containing 'Brand' or 'Shopping'.
// Leave as "" if not wanted.

var spreadsheetUrl = "https://docs.google.com/spreadsheets/d/1aSOeW0R6LEmp8RwwLQOj-ZWF6pUS4VZtp0ZGuheCA-Q/edit?usp=sharing";
// The URL of the Google Doc the results will be put into.

// Thresholds

var IMP_THRESHOLD = 10;
var CLICK_THRESHOLD = 0;
var COST_THRESHOLD = 0;
var CONV_THRESHOLD = 0;
// Words will be ignored if their statistics are lower than any of these thresholds

// Find the negative keywords

var NEGS_BY_GROUP = [];
var NEGS_BY_CAMPAIGN = [];
var SHARED_SET_DATA = [];
var SHARED_SET_NAMES = [];
var SHARED_SET_CAMPAIGNS = [];
var DATE_RANGE = START_DATE.replace(/-/g, "") + "," + END_DATE.replace(/-/g, "");
var ACTIVE_CAMP_IDS = [];
var keywordReport;

function main() {
  // Gather ad group level negative keywords
  var keywordParam = "Id";
  var keywordReport = getKeywordReport();  
  
  print(ACTIVE_CAMP_IDS.join(', '));
  print(keywordReport.rows().hasNext());
  print("line 55 CAMP_NAME_CONTAINS: " + CAMP_NAME_CONTAINS);
  
  var keywordRows = keywordReport.rows();
  while (keywordRows.hasNext()) {
    var keywordRow = keywordRows.next();
    
    if (NEGS_BY_GROUP[keywordRow["AdGroupId"]] == undefined) {
      var thing = [
        [keywordRow[keywordParam].toLowerCase(), keywordRow["KeywordMatchType"].toLowerCase()]
      ];
      NEGS_BY_GROUP[keywordRow["AdGroupId"]] = thing;
      print('line 66 thing: ' + thing);
      
    } else {
      NEGS_BY_GROUP[keywordRow["AdGroupId"]].push([keywordRow[keywordParam].toLowerCase(), keywordRow["KeywordMatchType"].toLowerCase()]);
    }
    print('Line 71: ACTIVE_CAMP_IDS.indexOf(keywordRow["CampaignId"]): ' + ACTIVE_CAMP_IDS.indexOf(keywordRow["CampaignId"]));
    if (ACTIVE_CAMP_IDS.indexOf(keywordRow["CampaignId"]) < 0) {
      ACTIVE_CAMP_IDS.push(keywordRow["CampaignId"]);
    }
  } //end while
  
  if (ACTIVE_CAMP_IDS.length < 0) {
    for (var camp in CAMP_NAME_CONTAINS) {
      var name = AdWordsApp.campaigns()
      .withCondition('Name CONTAINS_IGNORE_CASE "' + camp + '"')
      .get();
      
      if (name.hasNext()) {
        var id = name.next().getId();
        ACTIVE_CAMP_IDS.push(id);
      }
      print(ACTIVE_CAMP_IDS.join(', '));
    }
  }
  print("Line 103" + ACTIVE_CAMP_IDS.join(","));
  
  // Gather campaign level negative keywords
  var campaignNegReport = getCampNegReport(ACTIVE_CAMP_IDS);
  
  var campaignNegativeRows = campaignNegReport.rows();
  while (campaignNegativeRows.hasNext()) {
    var campaignNegativeRow = campaignNegativeRows.next();
    
    if (NEGS_BY_CAMPAIGN[campaignNegativeRow["CampaignId"]] == undefined) {
      NEGS_BY_CAMPAIGN[campaignNegativeRow["CampaignId"]] = [
        [campaignNegativeRow[keywordParam].toLowerCase(), campaignNegativeRow["KeywordMatchType"].toLowerCase()]
      ];
    } else {
      
      NEGS_BY_CAMPAIGN[campaignNegativeRow["CampaignId"]].push([campaignNegativeRow[keywordParam].toLowerCase(), campaignNegativeRow["KeywordMatchType"].toLowerCase()]);
    }
  } //end while
  
  // Find which campaigns use shared negative keyword sets
  
  var campaignSharedReport = getCampSharedReport();
  
  var campaignSharedRows = campaignSharedReport.rows();
  while (campaignSharedRows.hasNext()) {
    var campaignSharedRow = campaignSharedRows.next();
    
    if (SHARED_SET_CAMPAIGNS[campaignSharedRow["SharedSetName"]] == undefined) {
      SHARED_SET_CAMPAIGNS[campaignSharedRow["SharedSetName"]] = [campaignSharedRow["CampaignId"]];
    } else {
      
      SHARED_SET_CAMPAIGNS[campaignSharedRow["SharedSetName"]].push(campaignSharedRow["CampaignId"]);
    }
  } //end while
  
  // Map the shared sets' IDs (used in the criteria report below)
  // to their names (used in the campaign report above)
  
  var sharedSetReport = AdWordsApp.report(
    "SELECT Name, SharedSetId, MemberCount, ReferenceCount, Type " +
    "FROM   SHARED_SET_REPORT " +
    "WHERE ReferenceCount > 0 AND Type = NEGATIVE_KEYWORDS ");
  var sharedSetRows = sharedSetReport.rows();
  while (sharedSetRows.hasNext()) {
    var sharedSetRow = sharedSetRows.next();
    SHARED_SET_NAMES[sharedSetRow["SharedSetId"]] = sharedSetRow["Name"];
  } //end while
  
  // Collect the negative keyword text from the sets,
  // and record it as a campaign level negative in the campaigns that use the set
  
  var sharedSetReport = AdWordsApp.report(
    "SELECT SharedSetId, KeywordMatchType, Id " +
    "FROM   SHARED_SET_CRITERIA_REPORT ");
  var sharedSetRows = sharedSetReport.rows();
  while (sharedSetRows.hasNext()) {
    var sharedSetRow = sharedSetRows.next();
    var setName = SHARED_SET_NAMES[sharedSetRow["SharedSetId"]];
    if (SHARED_SET_CAMPAIGNS[setName] != undefined) {
      for (var i = 0; i < SHARED_SET_CAMPAIGNS[setName].length; i++) {
        var campaignId = SHARED_SET_CAMPAIGNS[setName][i];
        if (NEGS_BY_CAMPAIGN[campaignId] == undefined) {
          NEGS_BY_CAMPAIGN[campaignId] = [
            [sharedSetRow[keywordParam].toLowerCase(), sharedSetRow["KeywordMatchType"].toLowerCase()]
          ];
        } else {
          
          NEGS_BY_CAMPAIGN[campaignId].push([sharedSetRow[keywordParam].toLowerCase(), sharedSetRow["KeywordMatchType"].toLowerCase()]);
        }
      }
    }
  } //end while
  
  print("Finished negative keyword lists.");
  
  // Defines the statistics to download or calculate, and their formatting
  
  var statColumns = ["Clicks", "Impressions", "Cost", "ConvertedClicks", "ConversionValue"];
  var calculatedStats = [
    ["CTR", "Clicks", "Impressions"],
    ["CPC", "Cost", "Clicks"],
    ["Conv. Rate", "ConvertedClicks", "Clicks"],
    ["Cost / conv.", "Cost", "ConvertedClicks"],
    ["Conv. value/cost", "ConversionValue", "Cost"]
  ]
  var currencyFormat = CURRENCY_SYMBOL + "#,##0.00";
  var formatting = ["#,##0", "#,##0", currencyFormat, "#,##0", currencyFormat, "0.00%", currencyFormat, "0.00%", currencyFormat, "0.00%"];
  
  
  // Go through the search query report, remove searches already excluded by negatives
  // record the performance of each word in each remaining query
  doQueryReport();
  
  
  
  // Output the data into the spreadsheet
  
  var campaignSearchWordsOutput = [];
  var campaignSearchWordsFormat = [];
  var totalSearchWordsOutput = [];
  var totalSearchWordsFormat = [];
  var wordLengthOutput = [];
  var wordLengthFormat = [];
  
  // Add headers
  
  var calcStatNames = [];
  for (var s = 0; s < calculatedStats.length; s++) {
    calcStatNames.push(calculatedStats[s][0]);
  }
  var statNames = statColumns.concat(calcStatNames);
  campaignSearchWordsOutput.push(["Campaign", "Word", "FullQuery"].concat(statNames));
  totalSearchWordsOutput.push(["Word", "FullQuery"].concat(statNames));
  wordLengthOutput.push(["Word count"].concat(statNames));
  
  // Output the campaign level stats
  
  for (var campaign in campaignSearchWords) {
    for (var word in campaignSearchWords[campaign]) {
      
      if (campaignSearchWords[campaign][word]["Impressions\n"] < IMP_THRESHOLD) {
        continue;
      }
      if (campaignSearchWords[campaign][word]["Clicks"] < CLICK_THRESHOLD) {
        continue;
      }
      if (campaignSearchWords[campaign][word]["Cost"] < COST_THRESHOLD) {
        continue;
      }
      if (campaignSearchWords[campaign][word]["ConvertedClicks"] < CONV_THRESHOLD) {
        continue;
      }
      
      // skips words under the thresholds
      
      var printline = [campaign, word];
      
      for (var s = 0; s < statColumns.length; s++) {
        printline.push(campaignSearchWords[campaign][word][statColumns[s]]);
      }
      
      for (var s = 0; s < calculatedStats.length; s++) {
        var multiplier = calculatedStats[s][1];
        var divisor = calculatedStats[s][2];
        if (campaignSearchWords[campaign][word][divisor] > 0) {
          printline.push(campaignSearchWords[campaign][word][multiplier] / campaignSearchWords[campaign][word][divisor]);
        } else {
          printline.push("-");
        }
      }
      
      campaignSearchWordsOutput.push(printline);
      campaignSearchWordsFormat.push(formatting);
    }
  } // end for
  
  
  totalSearchWordsKeys.sort(function(a, b) {
    return totalSearchWords[b]["Cost"] - totalSearchWords[a]["Cost"];
  });
  
  for (var i = 0; i < totalSearchWordsKeys.length; i++) {
    var word = totalSearchWordsKeys[i];
    
    if (totalSearchWords[word]["Impressions"] < IMP_THRESHOLD) {
      continue;
    }
    if (totalSearchWords[word]["Clicks"] < CLICK_THRESHOLD) {
      continue;
    }
    if (totalSearchWords[word]["Cost"] < COST_THRESHOLD) {
      continue;
    }
    if (totalSearchWords[word]["ConvertedClicks"] < CONV_THRESHOLD) {
      continue;
    }
    
    // skips words under the thresholds
    
    var printline = [word];
    
    for (var s = 0; s < statColumns.length; s++) {
      printline.push(totalSearchWords[word][statColumns[s]]);
    }
    
    for (var s = 0; s < calculatedStats.length; s++) {
      var multiplier = calculatedStats[s][1];
      var divisor = calculatedStats[s][2];
      if (totalSearchWords[word][divisor] > 0) {
        printline.push(totalSearchWords[word][multiplier] / totalSearchWords[word][divisor]);
      } else {
        printline.push("-");
      }
    }
    
    totalSearchWordsOutput.push(printline);
    totalSearchWordsFormat.push(formatting);
  } // end for
  
  for (var i = 1; i < 8; i++) {
    if (i < 7) {
      var wordLength = i;
    } else {
      var wordLength = "7+";
    }
    
    var printline = [wordLength];
    
    if (numberOfWords[wordLength] == undefined) {
      printline.push([0, 0, 0, 0, "-", "-", "-", "-"]);
    } else {
      for (var s = 0; s < statColumns.length; s++) {
        printline.push(numberOfWords[wordLength][statColumns[s]]);
      }
      
      for (var s = 0; s < calculatedStats.length; s++) {
        var multiplier = calculatedStats[s][1];
        var divisor = calculatedStats[s][2];
        if (numberOfWords[wordLength][divisor] > 0) {
          printline.push(numberOfWords[wordLength][multiplier] / numberOfWords[wordLength][divisor]);
        } else {
          printline.push("-");
        }
      }
    }
    
    wordLengthOutput.push(printline);
    wordLengthFormat.push(formatting);
  } // end for
  
  // Finds available names for the new sheets
  
  var campaignWordName = "Campaign Word Analysis";
  var totalWordName = "Total Word Analysis";
  var wordCountName = "Word Count Analysis";
  var campaignWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(campaignWordName);
  var totalWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(totalWordName);
  var wordCountSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(wordCountName);
  var i = 1;
  while (campaignWordSheet != null || wordCountSheet != null || totalWordSheet != null) {
    campaignWordName = "Campaign Word Analysis " + i;
    totalWordName = "Total Word Analysis " + i;
    wordCountName = "Word Count Analysis " + i;
    campaignWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(campaignWordName);
    totalWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(totalWordName);
    wordCountSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).getSheetByName(wordCountName);
    i++;
    
  }
  campaignWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).insertSheet(campaignWordName);
  totalWordSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).insertSheet(totalWordName);
  wordCountSheet = SpreadsheetApp.openByUrl(spreadsheetUrl).insertSheet(wordCountName);
  
  campaignWordSheet.getRange("R1C1").setValue("Analysis of Words in Search Query Report, By Campaign");
  wordCountSheet.getRange("R1C1").setValue("Analysis of Search Query Performance by Words Count");
  
  if (CAMP_NAME_CONTAINS == "") {
    totalWordSheet.getRange("R1C1").setValue("Analysis of Words in Search Query Report, By Account");
  } else {
    totalWordSheet.getRange("R1C1").setValue("Analysis of Words in Search Query Report, Over All Campaigns Containing '" + CAMP_NAME_CONTAINS + "'");
  }
  
  campaignWordSheet.getRange("R2C1:R" + (campaignSearchWordsOutput.length + 1) + "C" + campaignSearchWordsOutput[0].length).setValues(campaignSearchWordsOutput);
  campaignWordSheet.getRange("R3C3:R" + (campaignSearchWordsOutput.length + 1) + "C" + (formatting.length + 2)).setNumberFormats(campaignSearchWordsFormat);
  totalWordSheet.getRange("R2C1:R" + (totalSearchWordsOutput.length + 1) + "C" + totalSearchWordsOutput[0].length).setValues(totalSearchWordsOutput);
  totalWordSheet.getRange("R3C2:R" + (totalSearchWordsOutput.length + 1) + "C" + (formatting.length + 1)).setNumberFormats(totalSearchWordsFormat);
  wordCountSheet.getRange("R2C1:R" + (wordLengthOutput.length + 1) + "C" + wordLengthOutput[0].length).setValues(wordLengthOutput);
  wordCountSheet.getRange("R3C2:R" + (wordLengthOutput.length + 1) + "C" + (formatting.length + 1)).setNumberFormats(wordLengthFormat);
  
  print("Finished writing to spreadsheet.");
}

function doQueryReport(){	
  print("Going through the search query report, removing searches already excluded by negatives, and recording the performance of each word in each remaining query...");
  var queryReport = getqueryReport(statColumns);
  
  var campaignSearchWords = [];
  var totalSearchWords = [];
  var totalSearchWordsKeys = [];
  var numberOfWords = [];
  
  var queryRows = queryReport.rows();
  while (queryRows.hasNext()) {
    var queryRow = queryRows.next();
    var searchIsExcluded = false;
    
    // Checks if the query is excluded by an ad group level negative
    
    if (NEGS_BY_GROUP[queryRow["AdGroupId"]] !== undefined) {
      for (var i = 0; i < NEGS_BY_GROUP[queryRow["AdGroupId"]].length; i++) {
        if ((NEGS_BY_GROUP[queryRow["AdGroupId"]][i][1] == "exact" &&
             queryRow["Query"] == NEGS_BY_GROUP[queryRow["AdGroupId"]][i][0]) ||
            (NEGS_BY_GROUP[queryRow["AdGroupId"]][i][1] != "exact" &&
            (" " + queryRow["Query"] + " ").indexOf(" " + NEGS_BY_GROUP[queryRow["AdGroupId"]][i][0] + " ") > -1)) {
          searchIsExcluded = true;
          break;
        }
      }
    }
    
    // Checks if the query is excluded by a campaign level negative
    
    if (!searchIsExcluded && NEGS_BY_CAMPAIGN[queryRow["CampaignId"]] !== undefined) {
      for (var i = 0; i < NEGS_BY_CAMPAIGN[queryRow["CampaignId"]].length; i++) {
        if ((NEGS_BY_CAMPAIGN[queryRow["CampaignId"]][i][1] == "exact" &&
             queryRow["Query"] == NEGS_BY_CAMPAIGN[queryRow["CampaignId"]][i][0]) ||
            (NEGS_BY_CAMPAIGN[queryRow["CampaignId"]][i][1] != "exact" &&
            (" " + queryRow["Query"] + " ").indexOf(" " + NEGS_BY_CAMPAIGN[queryRow["CampaignId"]][i][0] + " ") > -1)) {
          searchIsExcluded = true;
          break;
        }
      }
    }
    
    if (searchIsExcluded) {
      continue;
    }
    // if the search is already excluded by the current negatives,
    // we ignore it and go on to the next query
    
    var currentWords = queryRow["Query"].split(" ");
    var searchQuery = currentWords.join(' ');
    var doneWords = [];
    
    if (campaignSearchWords[queryRow["CampaignName"]] == undefined) {
      campaignSearchWords[queryRow["CampaignName"]] = [searchQuery];
    }
    
    var wordLength = currentWords.length;
    if (wordLength > 6) {
      wordLength = "7+";
    }
    if (numberOfWords[wordLength] == undefined) {
      numberOfWords[wordLength] = [];
    }
    for (var i = 0; i < statColumns.length; i++) {
      if (numberOfWords[wordLength][statColumns[i]] > 0) {
        numberOfWords[wordLength][statColumns[i]] += parseFloat(queryRow[statColumns[i]].replace(/,/g, ""));
      } else {
        numberOfWords[wordLength][statColumns[i]] = parseFloat(queryRow[statColumns[i]].replace(/,/g, ""));
      }
    }
    
    
    // Splits the query into words and records the stats for each
    
    for (var w = 0; w < currentWords.length; w++) {
      if (doneWords.indexOf(currentWords[w]) < 0) { //if this word hasn't been in the query yet
        print("Line 256; query: " + searchQuery);
        if (campaignSearchWords[queryRow["CampaignName"]][currentWords[w]] == undefined) {
          campaignSearchWords[queryRow["CampaignName"]][currentWords[w]] = [searchQuery];
        }
        if (totalSearchWords[currentWords[w]] == undefined) {
          totalSearchWords[currentWords[w]] = [];
          totalSearchWordsKeys.push(currentWords[w]);
        }
        
        for (var i = 0; i < statColumns.length; i++) {
          var stat = parseFloat(queryRow[statColumns[i]].replace(/,/g, ""));
          if (campaignSearchWords[queryRow["CampaignName"]][currentWords[w]][statColumns[i]] > 0) {
            campaignSearchWords[queryRow["CampaignName"]][currentWords[w]][statColumns[i]] += stat;
          } else {
            campaignSearchWords[queryRow["CampaignName"]][currentWords[w]][statColumns[i]] = stat;
          }
          if (totalSearchWords[currentWords[w]][statColumns[i]] > 0) {
            totalSearchWords[currentWords[w]][statColumns[i]] += stat;
          } else {
            totalSearchWords[currentWords[w]][statColumns[i]] = stat;
          }
        }
        
        doneWords.push(currentWords[w]);
      } //end if
    } //end for
  } //end while
  
  print("Finished analysing queries.");
  
}

function getCampNegReport() {
  var report = AdWordsApp.report(
    "SELECT CampaignId, Id, KeywordMatchType " +
    "FROM   CAMPAIGN_NEGATIVE_KEYWORDS_PERFORMANCE_REPORT " +
    "WHERE  IsNegative = TRUE " +
    "AND CampaignId IN [" + ACTIVE_CAMP_IDS.join(",") + "]"
  );
  return report;
}
function getCampSharedReport(){
  var report = AdWordsApp.report(
    "SELECT CampaignName, CampaignId, SharedSetName, SharedSetType, Status " +
    "FROM   CAMPAIGN_SHARED_SET_REPORT " +
    "WHERE SharedSetType = NEGATIVE_KEYWORDS " +
    "AND CampaignName CONTAINS_IGNORE_CASE '" + CAMP_NAME_CONTAINS + "'");
  return report;
}
function getqueryReport(statColumns){
  var report = AdWordsApp.report(
    "SELECT CampaignName, CampaignId, AdGroupId, AdGroupName, Query, " + statColumns.join(", ") + " " +
    "FROM SEARCH_QUERY_PERFORMANCE_REPORT " +
      "WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED " +
        "AND CampaignName CONTAINS_IGNORE_CASE '" + CAMP_NAME_CONTAINS + "' " +
          "DURING " + DATE_RANGE);
  return report;
}

function getKeywordReport(){
  var report;
  if (CAMP_NAME_CONTAINS === "") {
    report = AdWordsApp.report(
      "SELECT CampaignId, AdGroupId, Id, KeywordMatchType " +
      "FROM   NEGATIVE_KEYWORDS_PERFORMANCE_REPORT " +
      "WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND IsNegative = TRUE " +
      "DURING " + DATE_RANGE);
  } 
  else {
    report = AdWordsApp.report(
      "SELECT CampaignId, AdGroupId, Id, KeywordMatchType " +
      "FROM   KEYWORDS_PERFORMANCE_REPORT " +
      "WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND IsNegative = TRUE " +
      "AND CampaignName CONTAINS_IGNORE_CASE '" + CAMP_NAME_CONTAINS + "' " +
      "DURING " + DATE_RANGE);
  }
  return report;
}

function print(msg) {
  Logger.log(msg);
}
