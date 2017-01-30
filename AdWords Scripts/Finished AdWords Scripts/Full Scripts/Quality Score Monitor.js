/**********************************
 *
 * Quality Score Monitor
 * Created by Josh DeGraw
 *
 ***********************************/
var REPORT_NAME = ['Quality', 'Score', 'Monitor'];
var EMAILS = [
    "joshd@sewelldirect.com",

];
var IS_PREVIEW = AdWordsApp.getExecutionInfo().isPreview();

var TITLES = ['\nCampaign', 'AdGroup', 'Keyword', 'MatchType', 'QS', 'Cost', 'ConvValue', 'NetProfit', 'Conversions', 'Impressions', 'Clicks', 'MaxCPC', 'AvgCPC', 'KeywordID'];

var PAUSED_LIST = [
    ['Paused'], TITLES
];
var CHECKED_LIST = [
    ['Checked'], TITLES
];

var pausedNum = 0;
var checkedNum = 0;

var MIN_QS = 4;
var MED_QS = 5;

var LABEL = "Low_QS";
var EXCEPTION_LABEL = "Low_QS_Exception";
var DATE_RANGE = CustomDateRange();
var DATE_OBJECT = DATE_RANGE.dateObj;
var DATE_STRING = DATE_RANGE.string;

//info for the sheet that will hold the Conversion Values
var CONV_SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1-dyzDaFZ8mQvHGidP6MP1P-EXNVFRzJyTxbyi4sHnFg/edit?usp=sharing';
var CONV_SHEET_NAME = 'CONV_VALUE_REPORT';
var ADGRP_CONV_SHEET_NAME = 'AdGroupConv';
var LOW_QS_LOG_URL = 'https://docs.google.com/spreadsheets/d/143g_NYaLyQqNMnocHCku4u9EP0OEPRBYhYvTuIsRn1Y/edit?usp=sharing';
var LOW_QS_LOG_SHEET_NAME = 'Low QS Keywords Paused';

var CompletedReport = false;

function main() {
    try {
        createLabelIfNeeded(LABEL);

        updateConvValReport(CONV_SPREADSHEET_URL, CONV_SHEET_NAME, DATE_RANGE.string, true);
        CheckOrPause(DATE_RANGE);

        EmailResults(REPORT_NAME);
    } catch (e) {
        EmailErrorReport(REPORT_NAME.join(' '), EMAILS, IS_PREVIEW, e, CompletedReport);
    }
}


function isException(kw) {
    var labels = kw.labels().get();
    var qs = kw.getQualityScore();
    var result = false;

    if (qs === null || !isNumber(qs)) {
        print("Null qs for " + kw.getText());
        return true;
    } else {
        while (labels.hasNext()) {
            var label = labels.next();
            if (label.getName() === EXCEPTION_LABEL) {
                return true;
            }
        }
    }
    return result;
}

function CheckOrPause(dateRange) {
    Logger.log('Starting.');
    var keywordSelector = AdWordsApp.keywords()
        .withCondition("CampaignStatus = ENABLED")
        .withCondition("AdGroupStatus = ENABLED")
        .withCondition("Status = ENABLED")
        .withCondition("QualityScore <= " + MED_QS)
        .forDateRange(dateRange.fromObj, dateRange.toObj);
    var i = 0;
    var keywordIterator = keywordSelector.get();

    while (keywordIterator.hasNext()) {
        var kw = keywordIterator.next();

        if (!isException(kw)) {
            try {
                var kwId = kw.getId();
                var kwStats = kw.getStatsFor(dateRange.fromStr, dateRange.toStr);
                var campaignName = kw.getCampaign().getName();
                var adGroupName = kw.getAdGroup().getName();
                var keyW = kw.getText();
                var keyword = formatKeyword(keyW);

                var valReport = getConvValue(campaignName, adGroupName, kwId);
                var matchType = kw.getMatchType();
                var qs = kw.getQualityScore();
                var cost = valReport.Cost;
                var convVal = valReport.ConvVal;
                var netProfit = valReport.NetProfit;
                var conversions = valReport.Conversions;
                var clicks = kwStats.getClicks();
                var maxCPC = kw.getMaxCpc();
                var avgCpc = valReport.AvgCPC;
                var impressions = kwStats.getImpressions();


                // ['\nCampaign', 'AdGroup', 'Keyword', 'MatchType', 'QS', 'Cost', 'ConvValue', 'NetProfit', 'Conversions', 'Impressions', 'Clicks', 'MaxCPC', 'AvgCPC', 'KeywordID'];
                var msg = [campaignName, adGroupName, keyword, matchType, qs, cost, convVal, netProfit, conversions, impressions, clicks, maxCPC, avgCpc, kwId];

                if (qs <= MIN_QS && netProfit < 0) {
                    pauseKeyword(kw, msg);

                } else {
                    checkedKeyword(kw, msg);
                }
            } catch (e) {
                print(JSON.stringify(e, null, '\t'));
                Logger.log('Error in CheckOrPause for kw ' + kw + ': ' + e);
            }

        }
    }
    CompletedReport = true;
}

function pauseKeyword(kw, msg) {
    pausedNum++;
    kw.pause();
    Logger.log("Pausing " + msg.join());
    PAUSED_LIST = PAUSED_LIST.concat('\n' + msg);
    addToPausedSpreadsheet(msg);
    kw.applyLabel(LABEL);
}

function checkedKeyword(kw, msg) {
    checkedNum++;
    Logger.log('Not Pausing: ' + msg.join());
    CHECKED_LIST = CHECKED_LIST.concat('\n' + msg);

}


// Add the info for paused keywords to a set-aside spreadsheet to keep better track of all of them
function addToPausedSpreadsheet(msg) {
    var ss = SpreadsheetApp.openByUrl(LOW_QS_LOG_URL);
    var sheet = ss.getSheetByName(LOW_QS_LOG_SHEET_NAME);
    var date = _getDateTime().day;

    msg = msg.concat(date);
    sheet.appendRow(msg)

}

function getConvValue(campaign, adGroup, kwId) {
    var logError = 'Error Getting GP for: ' + campaign + ',' + adGroup;
    var alreadyLogged = '';
    var result;
    try {

        var ss = SpreadsheetApp.openByUrl(CONV_SPREADSHEET_URL);
        var sheet = ss.getSheetByName(CONV_SHEET_NAME);

        ss.getRangeByName("Selected_Campaign").setValue(campaign);
        ss.getRangeByName("Selected_AdGroup").setValue(adGroup);
        ss.getRangeByName("Selected_KwId").setValue(kwId);

        var convVal = ss.getRangeByName("Selected_ConvVal").getValue();
        var cpc = ss.getRangeByName("Selected_CPC").getValue();
        var cost = ss.getRangeByName("Selected_Cost").getValue();
        var conversions = ss.getRangeByName("Selected_Conversions").getValue();
        // var numRegex = /(\.*([0-9])*\,*[0-9]\.*)/g;

        if (convVal === "#N/A" || convVal === "" || !isNumber(convVal)) {
            convVal = 0;
        }

        if (cost === "#N/A" || cost === "" || !isNumber(cost)) {
            cost = 0;
        }

        if (cpc === "#N/A" || cpc === "" || !isNumber(cpc)) {
            cpc = 0;
        }

        if (conversions === "#N/A" || conversions === "" || !isNumber(conversions)) {
            conversions = 0;
        }

        var np = convVal - cost;
        if (np === NaN) {
            np = 0 - cost;
        }

        var result = {
            ConvVal: convVal,
            AvgCPC: cpc,
            Cost: cost,
            Conversions: conversions,
            NetProfit: np,
            List: function() {
                return 'NetProfit: ' + this.np + 'ConvVal: ' + this.ConvVal + ' AvgCPC: ' + this.AvgCPC + ' Cost: ' + this.cost + ' Conversions: ' + this.conversions;
            },
        }



    } catch (e) {

        throw e;
    }

    return result;
}

function updateConvValReport(sheetUrl, sheetName, dateRange, isString) {
    var timePeriod;
    isString ? timePeriod = dateRange : timePeriod = dateRange.string;

    var ss = SpreadsheetApp.openByUrl(sheetUrl);
    var sheet = ss.getSheetByName(sheetName);
    var date = _getDateTime();
    var today = date.day;
    var time = date.time;
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var timeCell = ss.getRangeByName('UpdateTime');
    var dayCell = ss.getRangeByName('UpdateDay');
    var dayCellVal = Utilities.formatDate(dayCell.getValue(), timeZone, "MM-dd-yyyy");
    var periodRange = ss.getRangeByName("TimePeriod");
    var periodRangeVal = periodRange.getValue();
    var updateTime = today + ', ' + time;
    var dayCellIsNotToday = (dayCellVal != today);
    var periodRangeIsNotTheSame = (periodRangeVal != timePeriod);
    print('dayCellVal (' + dayCellVal + ') Is Not Today (' + today + ') === ' + dayCellIsNotToday);
    print('periodRange (' + periodRangeVal + ') Is Not The Same as selected (' + timePeriod + ') === ' + periodRangeIsNotTheSame);
    var updating = dayCellIsNotToday || periodRangeIsNotTheSame;
    print('Updating: ' + updating);

    // Only update it a max of once per day
    if (updating) {
        var reportName = 'KEYWORDS_PERFORMANCE_REPORT';
        var statusConditions = 'CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED';
        Logger.log('Updating ConvVal Report for: ' + timePeriod);


        var fields = 'CampaignName, AdGroupName, Id, ConversionValue, AverageCpc, Cost, Conversions ';
        var startRange = 'A';
        var endRange = 'H';

        var report = getAdWordsReport(fields, reportName, statusConditions + ' AND BiddingStrategyType = MANUAL_CPC', timePeriod);
        var report2 = getAdWordsReport(fields, reportName, statusConditions + ' AND BiddingStrategyType = ENHANCED_CPC', timePeriod);

        var array = report.rows();
        var array2 = report2.rows();
        clearConvSheet(ss);
        var i = addRowsToSheet(array, sheet, startRange, endRange, 2);
        var endi = addRowsToSheet(array2, sheet, startRange, endRange, i);

        var lastRow = sheet.getLastRow();
        var range = sheet.getRange(startRange + '2:' + endRange + lastRow);

        range.sort([1, 2]);
        info('Date: ' + updateTime);
        periodRange.setValue("'" + timePeriod.toString());
        dayCell.setValue(today);
        print('Finished Updating ConvVal Report');
    }
}

function getAdWordsReport(fields, reportName, conditions, timePeriod) {
    var query = 'SELECT ' + fields.trim() +
        ' FROM ' + reportName.trim();
    if (conditions != '' && conditions != null && conditions != undefined) {
        query += ' WHERE ' + conditions.trim();
    }
    if (timePeriod != '' && conditions != null && conditions != undefined) {
        query += ' DURING ' + timePeriod.trim();
    }
    //print(query);
    return AdWordsApp.report(query);
}

function addRowsToSheet(array, sheet, startRange, endRange, i) {
    print('Adding Rows to sheet');
    while (array.hasNext()) {
        var range = sheet.getRange(startRange + i + ":" + endRange + i);
        var rowTotal = array.next();
        var cpa;
        rowTotal.Conversions === 0 ? cpa = '-' : cpa = (rowTotal.Cost / rowTotal.Conversions);

        var row = [
            [
                rowTotal.CampaignName,
                rowTotal.AdGroupName,
                rowTotal.Id,
                rowTotal.ConversionValue,
                rowTotal.AverageCpc,
                rowTotal.Cost,
                rowTotal.Conversions,
                cpa
            ]
        ];

        range.setValues(row);

        i++;
        //print(i);
    }
    return i;
}


function clearConvSheet(ss) {
    var campRange = ss.getRangeByName('CampaignName');
    var adGrpRange = ss.getRangeByName('AdGroupName');
    var kwIdRange = ss.getRangeByName('KwId');
    var convRange = ss.getRangeByName('Conversions');
    var costRange = ss.getRangeByName('Cost');
    var convValRange = ss.getRangeByName('ConversionValue');
    var cpcRange = ss.getRangeByName('AverageCpc');
    var cpaRange = ss.getRangeByName('CPA');


    campRange.clear({ contentsOnly: true });
    adGrpRange.clear({ contentsOnly: true });
    kwIdRange.clear({ contentsOnly: true });
    convRange.clear({ contentsOnly: true });
    costRange.clear({ contentsOnly: true });
    convValRange.clear({ contentsOnly: true });
    cpcRange.clear({ contentsOnly: true });
    cpaRange.clear({ contentsOnly: true });
}

function emailAttachment() {
    var attachment = '';
    if (pausedNum > 0) {
        attachment = PAUSED_LIST.join();
    }
    if (checkedNum > 0) {
        if (attachment != '') {
            attachment += '\n\n';
        }
        attachment += CHECKED_LIST.join();
    }
    return attachment;

}

function emailMessage() {
    var message = '';
    if (pausedNum > 0) {
        message += pausedNum + ' keywords were paused due to  having a QS below ' + MIN_QS + '.';
    }
    if (checkedNum > 0) {
        if (message != '') {
            message += '\n\n';
        }
        message += checkedNum + ' keywords have a QS of ' + MED_QS + '.';
    }
    return message + 'This script Pauses keywords below QS of ' + MIN_QS + ' that also are not profitable for: ' + DATE_RANGE.string + '\nKeywords that have been paused by this script can be seen at: ' + LOW_QS_LOG_URL + ', along with the date of the change.';
}
