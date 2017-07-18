/************************************
 * AdWords Account Audit Checklist
 * Created By: Russ Savage
 * Customized By: Josh DeGraw
 ************************************/
var REPORT_NAME = ['Account', 'Audit'];
var EMAILS = [
	'joshd@sewelldirect.com',
	'paul@sewelldirect.com'
];
var MAX_KEYWORD_NUM = 15; // <-- this is the max number of keywords you want in an AdGroup
var NUMBER_OF_ADS = 1; // <-- this is the minimum number of ads you want in an AdGroup
var MAX_NUM_OF_ADS = 4; // <-- this is the maximum number of ads you want in an AdGroup
var TARGET_LOCATIONS = ['United States']; // <-- the list of places your campaigns should be targeting
var SITE_LINK_MIN = 4; //<-- this is the minimum number of site links you want in a campaign
//var excludedLocList = ['Europe']; // <-- the list of places your campaigns should be excluding

var MATCH_TYPES = [];
var AdNumObj = {
	Count: 0,
	List: [
		['AdGroups with irregular number of ads (max recommended: ' + MAX_NUM_OF_ADS + '):'],
		['\nCampaign,AdGroup,Ads']
	]
};

var KeywordsObj = {
	Count: 0,
	List: [
		['AdGroups with too many keywords (max recommended: ' + MAX_KEYWORD_NUM + '):'],
		['\nCampaign,AdGroup,Keywords']
	]
};

var NegativeKeywordsObj = {
	Count: 0,
	List: [
		['AdGroups negative keywords (no max recommended):'],
		['\nCampaign,AdGroup,NegKeywords']
	]
};

var PhoneObj = {
	Count: 0,
	List: [
		['Campaigns without Phone Extensions:'],
		['\nCampaign,']
	]
};

var MobileModObj = {
	Count: 0,
	List: [
		['Campaigns without mobile modifiers:'],
		['\nCampaign']
	]
};

var linkNum = 0;
var LinkObj = {
	Count: 0,
	List: [
		['Campaigns without recommended number of sitelinks (' + SITE_LINK_MIN + '):'],
		['\nCampaign']
	]
};

function main() {
	try {
		//1. Campaigns
		//  a. Target the right locations. 
		verifyTargetedLocations(TARGET_LOCATIONS);

		//verifyExcludedLocations(excludedLocList);

		//  b. Language - Can't be done using scripts yet :(

		//  c. Search vs Display
		verifySearchAndDisplay();

		//  d. Check Mobile Strategy
		verifyMobileModifiers();

		//2. AdGroups
		//  a. Check for AdGroups with more than 20-30 keywords
		verifyKeywordNum();
		verifyNegKeywordNum();

		//  c. Check for ads
		verifyAdNum();

		//3. Keywords
		//  a. Check for MatchTypes
		printMatchTypes();

		//4. Other
		//  a. Conversion Tracking
		verifyConversionTracking();

		//  b. AdExtensions
		verifyAdExtensions();

		EmailResults(REPORT_NAME);
	} catch (e) {
		print('Error Occured: e');
		print(JSON.stringify(e, null, '\t'));
		EmailErrorReport(REPORT_NAME.join(' '), EMAILS, IS_PREVIEW, e, CompletedReport);
	}
}


function emailMessage() {
	return 'Attached are the results of the account audit.';
}

function emailAttachment() {
	var attachment = '';
	info('adNum: ' + AdNumObj.Count);
	if (AdNumObj.Count > 0) {
		attachment += AdNumObj.List.join();
	}
	info('kwNum: ' + KeywordsObj.Count);
	if (KeywordsObj.Count > 0) {
		if (attachment != '') {
			attachment += '\n\n'
		}
		attachment += KeywordsObj.List.join();
	}
	info('negKwNum: ' + NegativeKeywordsObj.Count);
	if (NegativeKeywordsObj.Count > 0) {
		if (attachment != '') {
			attachment += '\n\n'
		}
		attachment += NegativeKeywordsObj.List.join();
	}
	info('ModNum: ' + MobileModObj.Count);
	if (MobileModObj.Count > 0) {
		if (attachment != '') {
			attachment += '\n\n'
		}
		attachment += MobileModObj.List.join();
	}
	info('phoneNum: ' + PhoneObj.Count);
	if (PhoneObj.Count > 0) {
		if (attachment != '') {
			attachment += '\n\n'
		}
		attachment += PhoneObj.List.join();
	}
	info('linkNum: ' + LinkObj.Count);
	if (LinkObj.Count > 0) {
		if (attachment != '') {
			attachment += '\n\n'
		}
		attachment += LinkObj.List.join();
	}
	if (attachment != '') {
		attachment += '\n\n'
	}
	attachment += MATCH_TYPES.join();

	return attachment;
}

function verifyAdScheduling() {
	var AD_SCHEDULES = {
		TotalGoodWeekendCount: 0,
		TotalBadWeekendCount: 0,
		BadCampCount: 0,
		Campaigns: []

	};
	var schedules;
	var s;
	var row;
	var campaigns = AdWordsApp.campaigns()
		.withCondition('Status = ENABLED')
		.get();

	while (campaigns.hasNext()) {
		var camp = campaigns.next();
		AD_SCHEDULES.Campaigns[camp] = {
			Rows: [],
			GoodWeekendCount: 0,
			BadWeekendCount: 0
		};
		schedules = camp.targeting().adSchedules().get();

		if (schedules.totalNumEntities() > 0) {
			print(camp.getName() + ': ');

			while (schedules.hasNext()) {
				s = schedules.next();
				row = s.getDayOfWeek() + '\t' + s.getStartHour() + '\t' + s.getEndHour() + '\t' + s.getBidModifier();
				print('\t' + row);
				if (s.getDayOfWeek() === 'SATURDAY' || s.getDayOfWeek() === 'SUNDAY') {
					if (s.getBidModifier() < 1) {

						AD_SCHEDULES.Campaigns[camp].GoodWeekendCount++;
						AD_SCHEDULES.TotalGoodWeekendCount++;
					} else {
						AD_SCHEDULES.Campaigns[camp].BadWeekendCount++;
						AD_SCHEDULES.TotalBadWeekendCount++;
					}
				}
				AD_SCHEDULES.Campaigns[camp].Rows.push(row);

			} // end while

			print(AD_SCHEDULES.Campaigns[camp].GoodWeekendCount);
			if (AD_SCHEDULES.Campaigns[camp].GoodWeekendCount === 0.0) {
				AD_SCHEDULES.BadCampCount++;
			}

		} else {
			print(camp.getName() + ' has no ad schedules.');
		}
	}
	print('Total: ' + AD_SCHEDULES.TotalBadWeekendCount);
	print('Without: ' + AD_SCHEDULES.BadCampCount);
}

function verifyConversionTracking() {
	//Assume that if the account has not had a conversion in 7 days, something is wrong.
	var campsWithConversions = AdWordsApp.campaigns()
		.withCondition('Status = ENABLED')
		.forDateRange('LAST_7_DAYS')
		.withCondition('Conversions > 0')
		.get().totalNumEntities();
	if (campsWithConversions == 0) {
		warn('Campaign has not had any conversions in the last week.');
	}
}

function verifyAdExtensions() {
	var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();

	while (campIter.hasNext()) {
		var camp = campIter.next();
		var campName = camp.getName();
		var phoneNumExtCount = camp.extensions().phoneNumbers().get().totalNumEntities();
		// Phone number extensions
		if (phoneNumExtCount == 0) {
			var msg = 'Campaign: "' + campName + '" is missing phone number extensions.';
			var phoneParams = [campName];
			addToList(PhoneObj, phoneParams, msg);
		}

		// Check how many sitelinks a campaign has
		var siteLinksExtCount = camp.extensions().sitelinks().get().totalNumEntities();
		if (siteLinksExtCount < SITE_LINK_MIN) {
			var msg = 'Campaign: "' + campName + '" could use more site links. Currently has: ' + siteLinksExtCount;
			var linkParams = [campName];
			addToList(LinkObj, linkParams, msg);
		}
		/*
		// we don't need mobile app extensions
		var mobileAppsExtCount = camp.extensions().mobileApps().get().totalNumEntities();
		if(mobileAppsExtCount == 0) {
		warn('Campaign: "'+camp.getName()+'" is missing mobile apps extension.');
		}*/
	}
}

function printMatchTypes() {
	var broadKws = AdWordsApp.keywords()
		.withCondition('Status = ENABLED')
		.withCondition('AdGroupStatus = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.withCondition('KeywordMatchType = BROAD')
		.get();
	var numBroad = broadKws.totalNumEntities();

	var numBroadMod = 0;
	while (broadKws.hasNext()) {
		if (broadKws.next().getText().match(/\+/)) {
			numBroadMod++;
		}
	}
	numBroad -= numBroadMod;

	var numPhrase = AdWordsApp.keywords()
		.withCondition('Status = ENABLED')
		.withCondition('AdGroupStatus = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.withCondition('KeywordMatchType = PHRASE')
		.get().totalNumEntities();

	var numExact = AdWordsApp.keywords()
		.withCondition('Status = ENABLED')
		.withCondition('AdGroupStatus = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.withCondition('KeywordMatchType = EXACT')
		.get().totalNumEntities();

	var total = numBroad + numBroadMod + numPhrase + numExact;
	var percBroad = Math.round(numBroad / total * 100);
	var percBroadMod = Math.round(numBroadMod / total * 100);
	var percPhrase = Math.round(numPhrase / total * 100);
	var percExact = Math.round(numExact / total * 100);

	MATCH_TYPES = MATCH_TYPES.concat(['\nOut of a total of: ' + total + ' active keywords in your account:'], ['\nMatch Type', 'Number', 'Percent']);
	MATCH_TYPES = MATCH_TYPES.concat(['\nBroad', numBroad, percBroad + '%']);
	MATCH_TYPES = MATCH_TYPES.concat(['\nBroad Mod (+)', numBroadMod, percBroadMod + '%']);
	MATCH_TYPES = MATCH_TYPES.concat(['\nPhrase', numPhrase, percPhrase + '%']);
	MATCH_TYPES = MATCH_TYPES.concat(['\nExact', numExact, percExact + '%']);
}

// Verify the number of ads per ad group
function verifyAdNum() {
	var agIter = AdWordsApp.adGroups()
		.withCondition('Status = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.get();
	var level = 'ad';
	while (agIter.hasNext()) {
		var ag = agIter.next();
		var campaign = ag.getCampaign().getName();
		var adGroup = ag.getName();
		var adCount = ag.ads().withCondition('Status = ENABLED').get().totalNumEntities();
		var adNumParams = [campaign, adGroup, adCount];

		if (adCount < NUMBER_OF_ADS) {
			var msg = 'Warning: Campaign: "' + campaign + '" AdGroup: "' + adGroup + '" does not have enough ads: ' + adCount;
			//addToList(KeywordsObj, adNumParams, msg);
			//addToList(adNumParams, AD_NUM_LIST, adNum, msg)
			addToList(AdNumObj, adNumParams, msg);
		}
		if (adCount > (MAX_NUM_OF_ADS)) {
			var msg = 'Warning: Campaign: "' + campaign + '" AdGroup: "' + adGroup + '" has too many ads: ' + adCount;
			addToList(AdNumObj, adNumParams, msg);
		}
	}
}

function verifyKeywordNum() {
	var agIter = AdWordsApp.adGroups()
		.withCondition('Status = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.get();

	while (agIter.hasNext()) {
		var ag = agIter.next();
		var kwSize = ag.keywords().withCondition('Status = ENABLED').get().totalNumEntities();
		if (kwSize >= MAX_KEYWORD_NUM) {
			var campaignName = ag.getCampaign().getName();
			var adGroupName = ag.getName();
			var msg = 'Warning: Campaign: "' + campaignName + '" AdGroup: "' + adGroupName + '" has too many keywords: ' + kwSize;
			var kwParams = [campaignName, adGroupName, kwSize];
			//addToList(kwParams, KEYWORD_LIST, kwNum, msg);
			addToList(KeywordsObj, kwParams, msg);
		}
	}
}

function verifyNegKeywordNum() {
	var agIter = AdWordsApp.adGroups()
		.withCondition('Status = ENABLED')
		.withCondition('CampaignStatus = ENABLED')
		.get();
	while (agIter.hasNext()) {
		var ag = agIter.next();
		var kwSize = ag.negativeKeywords() /*.withCondition('Status = ENABLED')*/ .get().totalNumEntities();
		if (kwSize > 0) {
			var campaignName = ag.getCampaign().getName();
			var adGroupName = ag.getName();

			var msg = 'Campaign: "' + campaignName + '" AdGroup: "' + adGroupName + '" has negative keywords: ' + kwSize;
			var negKwParams = [campaignName, adGroupName, kwSize];

			// addToList(negKwParams, NEG_KEYWORD_LIST, negKwNum, msg);

			addToList(NegativeKeywordsObj, negKwParams, msg);
		}
	}
}

function verifyMobileModifiers() {
	var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
	while (campIter.hasNext()) {
		var camp = campIter.next();
		var desktop = camp.targeting().platforms().desktop().get().next();
		//var tablet = camp.targeting().platforms().tablet().get().next();
		var mobile = camp.targeting().platforms().mobile().get().next();
		//check for mobile modifiers
		if (desktop.getBidModifier() == 1 && mobile.getBidModifier() == 1) {
			var campName = camp.getName();
			var msg = 'Warning: Campaign: "' + campName + '" has no mobile modifier set.';
			var modParams = [campName];
			addToList(MobileModObj, modParams, msg)
		}
	}
}

function verifyTargetedLocations(locList) {
	var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
	while (campIter.hasNext()) {
		var camp = campIter.next();
		var locIter = camp.targeting().targetedLocations().get();
		reportOnLocations(camp, locIter, locList);
	}
}
/*
function verifyExcludedLocations(locList) {
var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
while(campIter.hasNext()) {
var camp = campIter.next();
var locIter = camp.targeting().excludedLocations().get();
reportOnLocations(camp,locIter,locList);
}
}
*/
function reportOnLocations(camp, locIter, locList) {
	var campLocList = [];
	while (locIter.hasNext()) {
		var loc = locIter.next();
		campLocList.push(loc.getName());
		if (!locList) {
			warn('Campaign: "' + camp.getName() + '" targeting: "' + loc.getName() + '"');
		}
	}
	if (locList && campLocList.sort() != locList.sort()) {
		for (var i in campLocList) {
			if (locList.indexOf(campLocList[i]) == -1) {
				warn('Campaign: "' + camp.getName() + '" incorrectly targeting: "' + campLocList[i] + '"');
			}
		}
		for (var i in locList) {
			if (campLocList.indexOf(locList[i]) == -1) {
				warn('Campaign: "' + camp.getName() + '" not targeting: "' + locList[i] + '"');
			}
		}
	}
}

function verifySearchAndDisplay() {
	var API_VERSION = {
		includeZeroImpressions: false
	};
	var cols = ['CampaignId', 'CampaignName', 'AdNetworkType1', 'Impressions'];
	var report = 'CAMPAIGN_PERFORMANCE_REPORT';
	var query = ['select', cols.join(','), 'from', report, 'during', 'LAST_30_DAYS'].join(' ');
	var results = {}; // { campId : { agId : [ row, ... ], ... }, ... }
	var reportIter = AdWordsApp.report(query, API_VERSION).rows();
	while (reportIter.hasNext()) {
		var row = reportIter.next();
		if (results[row.CampaignId]) {
			warn('Campaign: "' + row.CampaignName + '" is targeting the Display and Search networks.');
		} else {
			results[row.CampaignId] = row;
		}
	}
	return results;

}


function addToList(obj, params, msg) {
	//info(obj.List);
	obj.List = obj.List.concat(['\n' + params.join()]);
	obj.Count++;
	info(msg);
}

//Minified Helper Functions:
function _getDateTime(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r),o=AM_PM(t),n={day:a,time:o};return n}catch(t){throw error("_getDateTime()",t)}}function AM_PM(t){try{var e=t.getHours()+1,r=t.getMinutes(),a=e>=12?"pm":"am";e%=12,e=e?e:12,r=r<10?"0"+r:r;var o=e+":"+r+" "+a;return o}catch(e){throw error("AM_PM(date: "+t+")",e)}}function CustomDateRange(t,e,r){try{null!==t&&void 0!==t||(t=91),null!==e&&void 0!==e||(e=0),void 0!==r&&""!==r&&null!==r||(r="YYYYMMdd");var a=_daysAgo(t),o=_daysAgo(e),n=_daysAgo(t,r).toString(),i=_daysAgo(e,r).toString(),s=n+","+i,c={fromStr:n,toStr:i,fromObj:a,toObj:o,dateObj:[a,o],string:s};return c}catch(a){throw error("CustomDateRange(fromDaysAgo: "+t+", tillDate: "+e+", format: "+r+")",a)}}function _daysAgo(t,e){try{var r=new Date;r.setDate(r.getDate()-t);var a;if(void 0!=e&&""!=e&&null!=e){var o=AdWordsApp.currentAccount().getTimeZone();a=Utilities.formatDate(r,o,e)}else a={year:r.getYear(),month:r.getMonth(),day:r.getDate()};return a}catch(r){throw error("_daysAgo(num: "+t+", format: "+e+")",r)}}function _today(t){try{var e,r=new Date,a=AdWordsApp.currentAccount().getTimeZone();return e=void 0!=t&&""!=t&&null!=t?Utilities.formatDate(r,a,t):{day:r.getDate(),month:r.getMonth(),year:r.getYear(),time:r.getTime()}}catch(e){throw error("_today(format: "+t+")",e)}}function _getDateString(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r);return a}catch(t){throw error("_getDateString()",t)}}function _todayIsMonday(){try{var t=36e5,e=new Date,r=new Date(e.getTime()+t),a=(r.getTime(),r.getDay());return Logger.log("today: "+r+"\nday: "+a),1===a}catch(t){throw error("todayIsMonday",t)}}function _rolling13Week(t){try{void 0!==t&&""!==t&&null!==t||(t="YYYYMMdd");var e=CustomDateRange(98,8,t),r=CustomDateRange(91,1,t),a=e.string+" - "+r.string,o={from:e,to:r,string:a};return o}catch(e){throw error("Rolling13Week(format: "+t+")",e)}}function formatKeyword(t){try{return t=t.replace(/[^a-zA-Z0-9 ]/g,"")}catch(e){throw error("formatKeyword(keyword: "+t+")",e)}}function round(t){try{var e=Math.pow(10,DECIMAL_PLACES);return Math.round(t*e)/e}catch(e){throw error("round(value: "+t+")",e)}}function getStandardDev(t,e,r){try{var a=0;for(var o in t)a+=Math.pow(t[o].stats[r]-e,2);return 0==Math.sqrt(t.length-1)?0:round(Math.sqrt(a)/Math.sqrt(t.length-1))}catch(a){throw error("getStandardDev(entites: "+t+", mean: "+e+", stat_key: "+r+")",a)}}function getMean(t,e){try{var r=0;for(var a in t)r+=t[a].stats[e];return 0==t.length?0:round(r/t.length)}catch(r){throw error("getMean(entites: "+t+", stat_key: "+e+")",r)}}function createLabelIfNeeded(t){try{AdWordsApp.labels().withCondition("Name = '"+t+"'").get().hasNext()||AdWordsApp.createLabel(t)}catch(e){throw error("createLabelIfNeeded(name: "+t+")",e)}}function EmailErrorReport(t,e,r,a,o){var n="AdWords Alert: Error in "+t+", script "+(o?"did execute correctly ":"did not execute ")+" correctly.",i="Error on line "+a.lineNumber+":\n"+a.message+EMAIL_SIGNATURE,s=emailAttachment(),c=_getDateString()+"_"+t,l=r?e[0]:e.join();PreviewMsg=r?"Preview; No changes actually made.\n":"",""!=i&&MailApp.sendEmail({to:l,subject:n,body:PreviewMsg+i,attachments:[{fileName:c+".csv",mimeType:"text/csv",content:s}]}),print("Email sent to: "+l)}function sendResultsViaEmail(t,e){try{var r,a=t.match(/\n/g).length-1,o=_getDateTime().day,n="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_titleCase(e)+"s Report - "+day,i="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",s=emailMessage(a)+i,c=SCRIPT_NAME.join("_")+o,l="";0!=a&&(AdWordsApp.getExecutionInfo().isPreview()?(r=EMAILS[0],l="Preview; No changes actually made.\n"):r=EMAILS.join(),MailApp.sendEmail({to:r,subject:n,body:l+s,attachments:[Utilities.newBlob(t,"text/csv",c+o+".csv")]}),Logger.log("Email sent to: "+r))}catch(r){throw error("sendResultsViaEmail(report: "+t+", level: "+e+")",r)}}function _titleCase(t){try{return t.replace(/(?:^|\s)\S/g,function(t){return t.toUpperCase()})}catch(e){throw error("_titleCase(str: "+t+")",e)}}function EmailResults(t){try{var e,r=EMAILS,a="AdWords Alert: "+t.join(" "),o=emailMessage()+EMAIL_SIGNATURE,n=emailAttachment(),i=_getDateString()+"_"+t.join("_"),s="";e=IS_PREVIEW?r[0]:r.join(),PreviewMsg=IS_PREVIEW?"Preview; No changes actually made.\n":"",""!=o&&MailApp.sendEmail({to:e,subject:a,body:s+o,attachments:[{fileName:i+".csv",mimeType:"text/csv",content:n}]}),print("Email sent to: "+e)}catch(e){throw error("EmailResults(ReportName: "+t.join(" ")+")",e)}}function EmailReportResults(t,e,r,a){try{var o,n="AdWords Alert: "+e.join(" "),i=_getDateString()+"_"+e.join("_");o=IS_PREVIEW?t[0]:t.join(),PreviewMsg=IS_PREVIEW?"Preview; No changes actually made.\n":"",""!=r&&MailApp.sendEmail({to:o,subject:n,body:PreviewMsg+r+EMAIL_SIGNATURE,attachments:[{fileName:i+".csv",mimeType:"text/csv",content:a.join(",")}]}),Logger.log("Email sent to: "+To)}catch(r){print(a.join()),error("EmailReportResults(_emails: "+t.join()+", _reportName:"+e.join()+", _message, _attachment),\n"+r)}}function info(t){Logger.log(t)}function print(t){Logger.log(t)}function error(t,e){var r=e.name+" in "+t+" at line "+e.lineNumber+": "+e.message;return Logger.log(r),r}function warn(t){Logger.log("WARNING: "+t)}function isNumber(t){try{return t.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===t}catch(e){throw error("isNumber(obj: "+t+")",e)}}function hasLabelAlready(t,e){try{return t.labels().withCondition("Name = '"+e+"'").get().hasNext()}catch(r){throw error("hasLabelAlready(entity: "+t+", label"+e+")",r)}}var PreviewMsg="",EMAIL_SIGNATURE="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",IS_PREVIEW=AdWordsApp.getExecutionInfo().isPreview();
