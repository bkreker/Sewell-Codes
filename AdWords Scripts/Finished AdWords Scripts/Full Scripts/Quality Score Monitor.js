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

function main() {
    createLabelIfNeeded(LABEL);

    updateConvValReport(CONV_SPREADSHEET_URL, CONV_SHEET_NAME, DATE_RANGE.string, true);
    CheckOrPause(DATE_RANGE);

    EmailResults(REPORT_NAME);
}

function isException(kw) {
    var labels = kw.labels().get();
    var qs = kw.getQualityScore();
    var result = false;

    if (qs === null || !isNumber(qs)) {
        print("Null qs for " + kw.getText());
        return true;
    } 
	else 
	{
        while (labels.hasNext()) 
		{
            var label = labels.next();
            if (label.getName() === EXCEPTION_LABEL) 
			{
                return true;
            }
        }
    }
    return result;
}

function CheckOrPause(dateRange) {
    Logger.log('Starting.');
    var keywordSelector = AdWordsApp
        .keywords()
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
			try{
				var kwId = kw.getId();
				var kwStats = kw.getStatsFor(dateRange.fromObj, dateRange.toObj);
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
				var msg = [campaignName, adGroupName, keyword, matchType, qs, cost, convVal, netProfit, conversions,impressions, clicks, maxCPC, avgCpc, kwId];

				if (qs <= MIN_QS && netProfit < 0) {
					pauseKeyword(kw, msg);

				} else {
					checkedKeyword(kw, msg);
				}
			}
			catch(e)
			{
				Logger.log('Error in CheckOrPause for kw ' + kw + ': ' + e);
			}
			
        }
    }
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
		
		if (convVal === "#N/A" || convVal === ""|| !isNumber(convVal)) {
            convVal = 0;
        }
		
		if (cost === "#N/A" || cost === ""|| !isNumber(cost)) {
            cost = 0;
        }
		
        if (cpc === "#N/A" || cpc === ""|| !isNumber(cpc)) {
            cpc = 0;
        }
		
		if (conversions === "#N/A" || conversions === ""|| !isNumber(conversions)) {
            conversions = 0;
        }
		
		var np = convVal - cost;
		if(np === NaN){
			np = 0 - cost;
		}
		
        var result = {
            ConvVal: convVal,
            AvgCPC: cpc,
            Cost: cost,
            Conversions: conversions,
			NetProfit: np,
            List: function() {
                return 'NetProfit: ' + this.np+'ConvVal: ' + this.ConvVal + ' AvgCPC: ' + this.AvgCPC + ' Cost: ' + this.cost + ' Conversions: ' + this.conversions;
            },
		}
        


    } catch (e) {
       
		throw e;
    }

    return result;
}

function updateConvValReport(sheetUrl, sheetName, dateRange, isString) {
    var timePeriod;
    isString ? timePeriod= dateRange : timePeriod = dateRange.string;
 
	var ss = SpreadsheetApp.openByUrl(sheetUrl);
    var sheet = ss.getSheetByName(sheetName);
    var date = _getDateTime();
	var today =date.day;
    var time = date.time;
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var timeCell = ss.getRangeByName('UpdateTime');
    var dayCell = ss.getRangeByName('UpdateDay');
    var dayCellVal = Utilities.formatDate(dayCell.getValue(), timeZone, "MM-dd-yyyy");
    var periodRange = ss.getRangeByName("TimePeriod");
	var periodRangeVal = periodRange.getValue();
    var updateTime = today + ', ' + time;
	print('dayCellVal '+dayCellVal + '!= today ' + today +' === '+(dayCellVal != today));
	print('periodRangeVal '+ periodRangeVal + '!= timePeriod ' + timePeriod + '=== ' +(periodRangeVal != timePeriod));
	
    // Only update it a max of once per day
    if (dayCellVal != today || periodRangeVal != timePeriod) 
	//if(true)
	{  
		Logger.log('Updating ConvVal Report for: ' + timePeriod);
		info('Date: ' + updateTime);   
        periodRange.setValue(timePeriod.toString());
		dayCell.setValue(today);
		
		var fields = 'CampaignName, AdGroupName, Id, ConversionValue, AverageCpc, Cost, Conversions ';
		var startRange = 'A';
		var endRange = 'H';
		var report = AdWordsApp.report(
			'SELECT  ' + fields +
			'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
			'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = MANUAL_CPC ' +
			'DURING ' + timePeriod
		);

		// Two reports since OR operator doesn't exist in AWQL
		var report2 = AdWordsApp.report(
			'SELECT  ' + fields +
			'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
			'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = ENHANCED_CPC ' +
			'DURING ' + timePeriod
		);

		var array = report.rows();
		var array2 = report2.rows();
		clearConvSheet(ss);
		var i = 2;
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
		}

		while (array2.hasNext()) {
			var range = sheet.getRange(startRange + i + ':' + endRange + i);
			var rowTotal = array2.next();
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
			Logger.log(row.join());
			range.setValues(row);
			range.setValue(row);
			i++;
		}

		var lastRow = sheet.getLastRow();
		var range = sheet.getRange(startRange + '2:' + endRange + lastRow);

		range.sort([1, 2]);
		print('Finished Updating ConvVal Report');
	}
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


    campRange.clear({contentsOnly: true});
    adGrpRange.clear({contentsOnly: true});
    kwIdRange.clear({contentsOnly: true});
    convRange.clear({contentsOnly: true});
    costRange.clear({contentsOnly: true});
    convValRange.clear({contentsOnly: true});
    cpcRange.clear({contentsOnly: true});
    cpaRange.clear({contentsOnly: true});
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
    return message +'This script Pauses keywords below QS of '+MIN_QS + ' that also are not profitable for: '+ DATE_RANGE+'\nKeywords that have been paused by this script can be seen at: ' + LOW_QS_LOG_URL + ', along with the date of the change.';
}


//Minified Helper Functions:
function _getDateTime(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c),e=AM_PM(a),f={day:d,time:e};return f}catch(a){throw error("_getDateTime()",a)}}function AM_PM(a){try{var b=a.getHours()+1,c=a.getMinutes(),d=b>=12?"pm":"am";b%=12,b=b?b:12,c=c<10?"0"+c:c;var e=b+":"+c+" "+d;return e}catch(b){throw error("AM_PM(date: "+a+")",b)}}function CustomDateRange(a,b,c){try{null!==a&&void 0!==a||(a=91),null!==b&&void 0!==b||(b=0),void 0!==c&&""!==c&&null!==c||(c="YYYYMMdd");var d=_daysAgo(a),e=_daysAgo(b),f=_daysAgo(a,c).toString(),g=_daysAgo(b,c).toString(),h=[d,e],i=f+","+g,j={fromStr:f,toStr:g,fromObj:d,toObj:e,dateObj:h,string:i};return j}catch(d){throw error("CustomDateRange(fromDaysAgo: "+a+", tillDate: "+b+", format: "+c+")",d)}}function _daysAgo(a,b){try{var c=new Date;c.setDate(c.getDate()-a);var d;if(void 0!=b&&""!=b&&null!=b){var e=AdWordsApp.currentAccount().getTimeZone();d=Utilities.formatDate(c,e,b)}else d={day:c.getDate(),month:c.getMonth(),year:c.getYear()};return d}catch(c){throw error("_daysAgo(num: "+a+", format: "+b+")",c)}}function _today(a){try{var d,b=new Date,c=AdWordsApp.currentAccount().getTimeZone();return d=void 0!=a&&""!=a&&null!=a?Utilities.formatDate(b,c,a):{day:b.getDate(),month:b.getMonth(),year:b.getYear(),time:b.getTime()}}catch(b){throw error("_today(format: "+a+")",b)}}function _getDateString(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c);return d}catch(a){throw error("_getDateString()",a)}}function _todayIsMonday(){try{var a=36e5,b=new Date,c=new Date(b.getTime()+a),e=(c.getTime(),c.getDay());return Logger.log("today: "+c+"\nday: "+e),1===e}catch(a){throw error("todayIsMonday",a)}}function _rolling13Week(a){try{void 0!==a&&""!==a&&null!==a||(a="YYYYMMdd");var b=CustomDateRange(98,8,a),c=CustomDateRange(91,1,a),d=b.string+" - "+c.string,e={from:b,to:c,string:d};return e}catch(b){throw error("Rolling13Week(format: "+a+")",b)}}function formatKeyword(a){try{return a=a.replace(/[^a-zA-Z0-9 ]/g,"")}catch(b){throw error("formatKeyword(keyword: "+a+")",b)}}function round(a){try{var b=Math.pow(10,DECIMAL_PLACES);return Math.round(a*b)/b}catch(b){throw error("round(value: "+a+")",b)}}function getStandardDev(a,b,c){try{var d=0;for(var e in a)d+=Math.pow(a[e].stats[c]-b,2);return 0==Math.sqrt(a.length-1)?0:round(Math.sqrt(d)/Math.sqrt(a.length-1))}catch(d){throw error("getStandardDev(entites: "+a+", mean: "+b+", stat_key: "+c+")",d)}}function getMean(a,b){try{var c=0;for(var d in a)c+=a[d].stats[b];return 0==a.length?0:round(c/a.length)}catch(c){throw error("getMean(entites: "+a+", stat_key: "+b+")",c)}}function createLabelIfNeeded(a){try{AdWordsApp.labels().withCondition("Name = '"+a+"'").get().hasNext()||AdWordsApp.createLabel(a)}catch(b){throw error("createLabelIfNeeded(name: "+a+")",b)}}function sendResultsViaEmail(a,b){try{var i,c=a.match(/\n/g).length-1,d=_getDateTime().day,e="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_titleCase(b)+"s Report - "+day,f="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",g=emailMessage(c)+f,h=SCRIPT_NAME.join("_")+d,j="";0!=c&&(AdWordsApp.getExecutionInfo().isPreview()?(i=EMAILS[0],j="Preview; No changes actually made.\n"):i=EMAILS.join(),MailApp.sendEmail({to:i,subject:e,body:j+g,attachments:[Utilities.newBlob(a,"text/csv",h+d+".csv")]}),Logger.log("Email sent to: "+i))}catch(c){throw error("sendResultsViaEmail(report: "+a+", level: "+b+")",c)}}function _titleCase(a){try{return a.replace(/(?:^|\s)\S/g,function(a){return a.toUpperCase()})}catch(b){throw error("_titleCase(str: "+a+")",b)}}function EmailResults(a){try{var g,b="AdWords Alert: "+a.join(" "),c="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",d=emailMessage()+c,e=emailAttachment(),f=_getDateString()+"_"+a.join("_"),h="";AdWordsApp.getExecutionInfo().isPreview()?(g=EMAILS[0],h="Preview; No changes actually made.\n"):g=EMAILS.join(),""!=d&&MailApp.sendEmail({to:g,subject:b,body:d,attachments:[{fileName:f+".csv",mimeType:"text/csv",content:e}]}),Logger.log("Email sent to: "+g)}catch(b){throw error("EmailResults(ReportName: "+a+")",b)}}function info(a){Logger.log(a)}function print(a){Logger.log(a)}function error(a,b){var c="ERROR in "+a+": "+b;return Logger.log(c),c}function warn(a){Logger.log("WARNING: "+a)}function isNumber(a){try{return a.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===a}catch(b){throw error("isNumber(obj: "+a+")",b)}}function hasLabelAlready(a,b){try{return a.labels().withCondition("Name = '"+b+"'").get().hasNext()}catch(c){throw error("hasLabelAlready(entity: "+a+", label"+b+")",c)}}

