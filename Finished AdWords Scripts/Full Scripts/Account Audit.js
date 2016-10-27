/************************************
 * AdWords Account Audit Checklist
 * Created By: Russ Savage
 * Customized By: Josh DeGraw
 ************************************/
var REPORT_NAME = ['Account', 'Audit'];
var EMAILS = [
	'joshd@sewelldirect.com', 
	'cameronp@sewelldirect.com'
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
	List: [	['AdGroups with irregular number of ads (max recommended: ' + MAX_NUM_OF_ADS + '):'],
			['\nCampaign,AdGroup,Ads']]
};

var KeywordsObj = {	
	Count: 0,
	List: [	['AdGroups with too many keywords (max recommended: ' + MAX_KEYWORD_NUM + '):'],
			['\nCampaign,AdGroup,Keywords']]
};

var NegativeKeywordsObj = {	
	Count: 0,
	List: [	['AdGroups negative keywords (no max recommended):'],
			['\nCampaign,AdGroup,NegKeywords']]
};

var PhoneObj = {
	Count: 0,
	List: [	['Campaigns without Phone Extensions:'],
			['\nCampaign,']]
};

var MobileModObj = {
	Count: 0,
	List: [	['Campaigns without mobile modifiers:'],
			['\nCampaign']]
};

var linkNum = 0;
var LinkObj = {
	Count: 0,
	List: [	['Campaigns without recommended number of sitelinks (' + SITE_LINK_MIN + '):'],
			['\nCampaign']]
};

function main() {

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

    EmailResults();

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
	if(MobileModObj.Count > 0){
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
function verifyAdScheduling() 
{
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
  
  while (campaigns.hasNext()) 
  {
    var camp = campaigns.next();
    AD_SCHEDULES.Campaigns[camp] = {
      Rows: [],
      GoodWeekendCount : 0,
      BadWeekendCount: 0
    };
    schedules = camp.targeting().adSchedules().get();
    
    if (schedules.totalNumEntities() > 0) 
    {
      print(camp.getName() + ': ');
      
      while (schedules.hasNext()) 
      {
        s = schedules.next();
        row = s.getDayOfWeek() + '\t' + s.getStartHour() + '\t' + s.getEndHour() + '\t' + s.getBidModifier();
        print('\t' + row);
        if (s.getDayOfWeek() === 'SATURDAY' || s.getDayOfWeek() === 'SUNDAY')
        {
          if(s.getBidModifier() < 1){
            
          AD_SCHEDULES.Campaigns[camp].GoodWeekendCount++;
          AD_SCHEDULES.TotalGoodWeekendCount++;
          }
          else{          
          AD_SCHEDULES.Campaigns[camp].BadWeekendCount++;
          AD_SCHEDULES.TotalBadWeekendCount++;
          }
        }
       AD_SCHEDULES.Campaigns[camp].Rows.push(row);
        
      } // end while
      
      print(AD_SCHEDULES.Campaigns[camp].GoodWeekendCount);
      if(AD_SCHEDULES.Campaigns[camp].GoodWeekendCount === 0.0){
        AD_SCHEDULES.BadCampCount++;
      }           
     
    } 
    else 
    {
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
	while(broadKws.hasNext()){
		if(broadKws.next().getText().match(/\+/)){
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
		
    var total = numBroad + numBroadMod+ numPhrase + numExact;
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
			
			var msg ='Campaign: "' + campaignName + '" AdGroup: "' + adGroupName + '" has negative keywords: ' + kwSize;
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


function addToList(obj, params, msg){
	//info(obj.List);
	obj.List = obj.List.concat(['\n' + params.join()]);
	obj.Count++;
	info(msg);
}

function _getDateTime(){var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c),e=AM_PM(a),f={day:d,time:e};return f}function AM_PM(a){var b=a.getHours(),c=a.getMinutes(),d=b>=12?"pm":"am";b%=12,b=b?b:12,c=c<10?"0"+c:c;var e=b+":"+c+" "+d;return e}function _today(a){var d,b=new Date,c=AdWordsApp.currentAccount().getTimeZone();d=""==a?"MM-dd-yyyy":a;var e=Utilities.formatDate(b,c,d);return e}function _getDateString(){var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c);return d}function todayIsMonday(){var a=36e5,b=new Date,c=new Date(b.getTime()+a),e=(c.getTime(),c.getDay());return Logger.log("today: "+c+"\nday: "+e),1===e}function _daysAgo(a,b){var c=new Date;c.setDate(c.getDate()-a);var d=AdWordsApp.currentAccount().getTimeZone(),e="MM-dd-yyyy";e=""==b?"MM-dd-yyyy":b;var f=Utilities.formatDate(c,d,e);return f}function Rolling13Week(){var a="MM/dd/YYYY",b=_daysAgo(98,a)+" - "+_daysAgo(7,a),c=_daysAgo(91,a)+" - "+_today(a),d={from:b,to:c,string:function(){return this.p+" - "+this.n}};return d}function Rolling13Week(a){var b=_daysAgo(98,a)+" - "+_daysAgo(7,a),c=_daysAgo(91,a)+" - "+_today(a),d={from:b,to:c,string:function(){return this.p+" - "+this.n}};return d}function CustomDateRange(a){var b=_daysAgo(91,a),c={from:b,to:_today(a),string:function(){return this.from+","+this.to}};return c}function CustomDateRange(){var a="yyyyMMdd",b={from:_daysAgo(91,a),to:_today(a),string:function(){return this.from+","+this.to}};return b}function CustomDateRange(a,b){var c=_daysAgo(a,b),d={from:c,to:_today(b),string:function(){return this.from+","+this.to}};return d}function formatKeyword(a){return a=a.replace(/[^a-zA-Z0-9 ]/g,"")}function round(a){var b=Math.pow(10,DECIMAL_PLACES);return Math.round(a*b)/b}function createLabelIfNeeded(a){AdWordsApp.labels().withCondition("Name = '"+a+"'").get().hasNext()||AdWordsApp.createLabel(a)}function sendResultsViaEmail(a,b){var i,c=a.match(/\n/g).length-1,d=_getDateTime().day,e="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_initCap(b)+"s Report - "+day,f="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",g=emailMessage(c)+f,h=SCRIPT_NAME.join("_")+d,j="";0!=c&&(AdWordsApp.getExecutionInfo().isPreview()?(i=EMAILS[0],j="Preview; No changes actually made.\n"):i=EMAILS.join(),MailApp.sendEmail({to:i,subject:e,body:j+g,attachments:[Utilities.newBlob(a,"text/csv",h+d+".csv")]}),Logger.log("Email sent to: "+i))}function EmailResults(){var f,a="AdWords Alert: "+REPORT_NAME.join(" "),b="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",c=emailMessage()+b,d=emailAttachment(),e=_getDateString()+"_"+REPORT_NAME.join("_"),g="";AdWordsApp.getExecutionInfo().isPreview()?(f=EMAILS[0],g="Preview; No changes actually made.\n"):f=EMAILS.join(),""!=c&&MailApp.sendEmail({to:f,subject:a,body:c,attachments:[{fileName:e+".csv",mimeType:"text/csv",content:d}]}),Logger.log("Email sent to: "+f)}function info(a){Logger.log(a)}function print(a){Logger.log(a)}function warn(a) {Logger.log('WARNING: ' + a)}function isNumber(a){return a.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===a}function hasLabelAlready(a,b){return a.labels().withCondition("Name = '"+b+"'").get().hasNext()}