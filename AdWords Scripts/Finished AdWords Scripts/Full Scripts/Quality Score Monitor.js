/**********************************
 *
 * Quality Score Monitor
 * Created by Josh DeGraw
 *
 ***********************************/
var REPORT_NAME = ['Quality', 'Score', 'Monitor'];
var EMAILS = [
    "joshd@sewelldirect.com",
    'paul@sewelldirect.com'

];
var IS_PREVIEW = AdWordsApp.getExecutionInfo().isPreview();

var TITLES = ['\nCampaign', 'AdGroup', 'Keyword', 'MatchType', 'QS', 'Cost', 'ConvValue', 'NetProfit', 'Conversions', 'Impressions', 'Clicks', 'MaxCPC', 'AvgCPC', 'KeywordID'];

var PAUSED_LIST = [
    ['Paused'], TITLES
];
var CHECKED_LIST = [
    ['Checked'], TITLES
];

var PausedNum = 0;
var CheckedNum = 0;

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
		CompletedReport = true;

		EmailReportResults(EMAILS, REPORT_NAME, emailMessage(), emailAttachment(), IS_PREVIEW);
	} catch (e) {
		print(e);
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
		.forDateRange(dateRange.fromStr, dateRange.toStr);
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
				Logger.log('Error in CheckOrPause for kw ' + kw + ': ' + e);
			}

		}
	}

	Logger.log('Finished.');
}

function pauseKeyword(kw, msg) {
	PausedNum++;
	kw.pause();
	Logger.log("Pausing " + msg.join());
	PAUSED_LIST = PAUSED_LIST.concat('\n' + msg);
	addToPausedSpreadsheet(msg);
	kw.applyLabel(LABEL);
}

function checkedKeyword(kw, msg) {
	CheckedNum++;
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
			List: function () {
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

	print('Updating Conv. Report: ' + updating);

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
	if (PausedNum > 0) {
		attachment = PAUSED_LIST.join();
	}
	if (CheckedNum > 0) {
		if (attachment != '') {
			attachment += '\n\n';
		}
		attachment += CHECKED_LIST.join();
	}
	return attachment;

}

function emailMessage() {
	var message = '';
	if (PausedNum > 0) {
		message += PausedNum + ' keywords were paused due to  having a QS below ' + MIN_QS + '.';
	}
	if (CheckedNum > 0) {
		if (message != '') {
			message += '\n\n';
		}
		message += CheckedNum + ' keywords have a QS of ' + MED_QS + '.';
	}
	return message + 'This script Pauses keywords below QS of ' + MIN_QS + ' that also are not profitable for: ' + DATE_RANGE.string + '\nKeywords that have been paused by this script can be seen at: ' + LOW_QS_LOG_URL + ', along with the date of the change.';
}

// Minified Helper Functions
function _getDateTime(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r),o=AM_PM(t),n={day:a,time:o};return n}catch(t){throw error("_getDateTime()",t)}}function AM_PM(t){try{var e=t.getHours()+1,r=t.getMinutes(),a=e>=12?"pm":"am";e%=12,e=e?e:12,r=r<10?"0"+r:r;var o=e+":"+r+" "+a;return o}catch(e){throw error("AM_PM(date: "+t+")",e)}}function CustomDateRange(t,e,r){try{null!==t&&void 0!==t||(t=91),null!==e&&void 0!==e||(e=0),void 0!==r&&""!==r&&null!==r||(r="YYYYMMdd");var a=_daysAgo(t),o=_daysAgo(e),n=_daysAgo(t,r).toString(),i=_daysAgo(e,r).toString(),s=n+","+i,c={fromStr:n,toStr:i,fromObj:a,toObj:o,dateObj:[a,o],string:s};return c}catch(a){throw error("CustomDateRange(fromDaysAgo: "+t+", tillDate: "+e+", format: "+r+")",a)}}function _daysAgo(t,e){try{var r=new Date;r.setDate(r.getDate()-t);var a;if(void 0!=e&&""!=e&&null!=e){var o=AdWordsApp.currentAccount().getTimeZone();a=Utilities.formatDate(r,o,e)}else a={year:r.getYear(),month:r.getMonth(),day:r.getDate()};return a}catch(r){throw error("_daysAgo(num: "+t+", format: "+e+")",r)}}function _today(t){try{var e,r=new Date,a=AdWordsApp.currentAccount().getTimeZone();return e=void 0!=t&&""!=t&&null!=t?Utilities.formatDate(r,a,t):{day:r.getDate(),month:r.getMonth(),year:r.getYear(),time:r.getTime()}}catch(e){throw error("_today(format: "+t+")",e)}}function _getDateString(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r);return a}catch(t){throw error("_getDateString()",t)}}function _todayIsMonday(){try{var t=36e5,e=new Date,r=new Date(e.getTime()+t),a=(r.getTime(),r.getDay());return Logger.log("today: "+r+"\nday: "+a),1===a}catch(t){throw error("todayIsMonday",t)}}function _rolling13Week(t){try{void 0!==t&&""!==t&&null!==t||(t="YYYYMMdd");var e=CustomDateRange(98,8,t),r=CustomDateRange(91,1,t),a=e.string+" - "+r.string,o={from:e,to:r,string:a};return o}catch(e){throw error("Rolling13Week(format: "+t+")",e)}}function formatKeyword(t){try{return t=t.replace(/[^a-zA-Z0-9 ]/g,"")}catch(e){throw error("formatKeyword(keyword: "+t+")",e)}}function round(t){try{var e=Math.pow(10,DECIMAL_PLACES);return Math.round(t*e)/e}catch(e){throw error("round(value: "+t+")",e)}}function getStandardDev(t,e,r){try{var a=0;for(var o in t)a+=Math.pow(t[o].stats[r]-e,2);return 0==Math.sqrt(t.length-1)?0:round(Math.sqrt(a)/Math.sqrt(t.length-1))}catch(a){throw error("getStandardDev(entites: "+t+", mean: "+e+", stat_key: "+r+")",a)}}function getMean(t,e){try{var r=0;for(var a in t)r+=t[a].stats[e];return 0==t.length?0:round(r/t.length)}catch(r){throw error("getMean(entites: "+t+", stat_key: "+e+")",r)}}function createLabelIfNeeded(t){try{AdWordsApp.labels().withCondition("Name = '"+t+"'").get().hasNext()||AdWordsApp.createLabel(t)}catch(e){throw error("createLabelIfNeeded(name: "+t+")",e)}}function EmailErrorReport(t,e,r,a,o){var n="AdWords Alert: Error in "+t+", script "+(o?"did execute correctly ":"did not execute ")+" correctly.",i="Error on line "+a.lineNumber+":\n"+a.message+EMAIL_SIGNATURE,s=emailAttachment(),c=_getDateString()+"_"+t,l=r?e[0]:e.join();PreviewMsg=r?"Preview; No changes actually made.\n":"",""!=i&&MailApp.sendEmail({to:l,subject:n,body:PreviewMsg+i,attachments:[{fileName:c+".csv",mimeType:"text/csv",content:s}]}),print("Email sent to: "+l)}function sendResultsViaEmail(t,e){try{var r,a=t.match(/\n/g).length-1,o=_getDateTime().day,n="AdWords Alert: "+REPORT_NAME.join(" ")+" "+_titleCase(e)+"s Report - "+day,i="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",s=emailMessage(a)+i,c=REPORT_NAME.join("_")+o,l="";0!=a&&(AdWordsApp.getExecutionInfo().isPreview()?(r=EMAILS[0],l="Preview; No changes actually made.\n"):r=EMAILS.join(),MailApp.sendEmail({to:r,subject:n,body:l+s,attachments:[Utilities.newBlob(t,"text/csv",c+o+".csv")]}),Logger.log("Email sent to: "+r))}catch(r){throw error("sendResultsViaEmail(report: "+t+", level: "+e+")",r)}}function _titleCase(t){try{return t.replace(/(?:^|\s)\S/g,function(t){return t.toUpperCase()})}catch(e){throw error("_titleCase(str: "+t+")",e)}}function EmailResults(t){try{var e,r=EMAILS,a="AdWords Alert: "+t.join(" "),o=emailMessage()+EMAIL_SIGNATURE,n=emailAttachment(),i=_getDateString()+"_"+t.join("_"),s="";e=r instanceof Array?IS_PREVIEW?r[0]:r.join():r,PreviewMsg=IS_PREVIEW?"Preview; No changes actually made.\n":"",""!=o&&MailApp.sendEmail({to:e,subject:a,body:s+o,attachments:[{fileName:i+".csv",mimeType:"text/csv",content:n}]}),print("Email sent to: "+e)}catch(e){throw error("EmailResults(ReportName: "+t.join(" ")+")",e)}}function EmailReportResults(t,e,r,a,o){try{var n,i="AdWords Alert: "+e.join(" "),s=_getDateString()+"_"+e.join("_");n=t instanceof Array?o?t[0]:t.join(","):t,a=a instanceof Array?a.join(","):a,PreviewMsg=o?"Preview; No changes actually made.\n":"",""!=r&&MailApp.sendEmail({to:n,subject:i,body:PreviewMsg+r+EMAIL_SIGNATURE,attachments:[{fileName:s+".csv",mimeType:"text/csv",content:a}]}),print("Email sent to: "+n)}catch(n){error("EmailReportResults(_emails: "+t+", _reportName: "+e+", _message: "+r+", _attachment: "+a+", isPreview: "+o+"),\n"+n)}}function info(t){Logger.log(t)}function print(t){Logger.log(t)}function error(t,e){var r="";return r=e instanceof Error?e.name+" in "+t+" at line "+e.lineNumber+": "+e.message:"Error in : "+t+":\n"+e,Logger.log(r),r}function warn(t){Logger.log("WARNING: "+t)}function isNumber(t){try{return t.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===t}catch(e){throw error("isNumber(obj: "+t+")",e)}}function hasLabelAlready(t,e){try{return t.labels().withCondition("Name = '"+e+"'").get().hasNext()}catch(r){throw error("hasLabelAlready(entity: "+t+", label"+e+")",r)}}var PreviewMsg="",EMAIL_SIGNATURE="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",IS_PREVIEW=AdWordsApp.getExecutionInfo().isPreview();