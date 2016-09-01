/************************************
* AdWords Account Audit Checklist
* Created By: Russ Savage
* Customized By: Josh DeGraw
************************************/
var REPORT_NAME = ['Account', 'Audit'];
var EMAIL_ADDRESS = 'joshd@sewelldirect.com';
var KEYWORD_NUM = 15; // <-- this is the max number of keywords you want in an AdGroup
var NUMBER_OF_ADS = 1; // <-- this is the minimum number of ads in an AdGroup
var MAX_NUM_OF_ADS = 4;// <-- this is the maximum number of ads in an AdGroup
var SITE_LINK_MIN = 4;
var EMAIL_MESSAGE = [];

var AD_NUM_LIST = [
  ['AdGroups with irregular number of ads (max recommended: '+ MAX_NUM_OF_ADS + '):'],
  ['\nCampaign,AdGroup,Ads']
];
var adNum = 0;

var KEYWORD_LIST = [
  ['AdGroups with too many keywords (max recommended: '+ KEYWORD_NUM +'):'],
  ['\nCampaign,AdGroup,Keywords']
];

var NEG_KEYWORD_LIST = [
  ['AdGroups negative keywords:'],
  ['\nCampaign,AdGroup,NegKeywords']
];

var kwNum = 0;
var negKwNum = 0;
var MATCH_TYPES = [];

var PHONE_LIST = [
  ['Campaigns without Phone Extensions:'],
  ['\nCampaign,']
];
var phoneNum = 0;

var LINK_LIST = [
  ['Campaigns without recommended number of sitelinks ('+ SITE_LINK_MIN + '):'],
  ['\nCampaign']
];
var linkNum = 0;

function main() {
  
  //1. Campaigns
  //  a. Target the right locations. 
  var includedLocList = ['United States']; // <-- the list of places your campaigns should be targeting
  verifyTargetedLocations(includedLocList);
  
  // Commented out by Josh because i don't need this right now
  //var excludedLocList = ['Europe']; // <-- the list of places your campaigns should be excluding
  //verifyExcludedLocations(excludedLocList);
  
  //  b. Language - Can't be done using scripts yet :(
  
  //  c. Search vs Display
  verifySearchAndDisplay();
  
  //  d. Check Mobile Strategy
  //verifyMobileModifiers();
  
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

function EmailResults() {	    
  var Subject =  'AdWords Alert'+REPORT_NAME.join(' ');	
  var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
  var Message  = emailMessage() + signature;
  var Attachment = emailAttachment();
  var file_name = _getDateString()+'_'+REPORT_NAME.join('_');
  var To;   
  var isPreview = '';
  
  
  if(AdWordsApp.getExecutionInfo().isPreview()){ 
    To = EMAILS[0] 
    isPreview = 'Preview; No changes actually made.\n';
  }
  else{
    To = EMAILS.join();
  }
  if(Attachment != ''){
    MailApp.sendEmail({	
      to: EMAIL_ADDRESS,
      subject: Subject,
      body: Message,
      attachments:[{fileName: _getDateString()+'_Account_Audit.csv', mimeType: 'text/csv', content: Attachment}]
    });
  }
}
function emailMessage(){
  return 'Attached are the results of the account audit.';
}
function emailAttachment(){
  var attachment = '';
  info('adNum: ' + adNum);
  if(adNum > 0){
    attachment += AD_NUM_LIST.join();
  }
  info('kwNum: ' + kwNum);
  if(kwNum > 0){
    if (attachment != ''){attachment+='\n\n'}
    attachment += KEYWORD_LIST.join();
  }
  info('negKwNum: ' + negKwNum);
  if(negKwNum > 0){
    if (attachment != ''){attachment+='\n\n'}
    attachment += NEG_KEYWORD_LIST.join();
  }
  info('phoneNum: ' + phoneNum);
  
  if(phoneNum > 0){
    if (attachment != ''){attachment+='\n\n'}
    attachment += PHONE_LIST.join();
  }
  info('linkNum: ' + linkNum);  
  if(linkNum > 0){
    if (attachment != ''){attachment+='\n\n'}
    attachment += LINK_LIST.join();
  }
  if (attachment != ''){attachment+='\n\n'}
  attachment += MATCH_TYPES.join();
  
  return attachment;
}
//Helper function to format todays date
function _getDateString() {
  var date = Utilities.formatDate((new Date()), AdWordsApp.currentAccount().getTimeZone(), "yyyy-MM-dd");
  return date;
}

function verifyConversionTracking() {
  //Assume that if the account has not had a conversion in 7 days, something is wrong.
  var campsWithConversions = AdWordsApp.campaigns()
  .withCondition('Status = ENABLED')
  .forDateRange('LAST_7_DAYS')
  .withCondition('Conversions > 0')
  .get().totalNumEntities();
  if(campsWithConversions == 0) {
    warn('Campaign has not had any conversions in the last week.');
  }
}

function verifyAdExtensions() {
  var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
  
  while(campIter.hasNext()) {
    var camp = campIter.next();
    var phoneNumExtCount = camp.extensions().phoneNumbers().get().totalNumEntities();
    // Phone number extensions
    if(phoneNumExtCount == 0) {
      var msg = 'Campaign: "'+camp.getName()+'" is missing phone number extensions.';
      PHONE_LIST = PHONE_LIST.concat(['\n'+camp.getName()]);
      phoneNum++;
      Log(msg);
    }
    
    // Check how many sitelinks a campaign has
    var siteLinksExtCount = camp.extensions().sitelinks().get().totalNumEntities();
    if(siteLinksExtCount < SITE_LINK_MIN) {
      var msg = 'Campaign: "'+camp.getName()+'" could use more site links. Currently has: '+ siteLinksExtCount;
      LINK_LIST = LINK_LIST.concat(['\n'+camp.getName()]);
      linkNum++;
      Log(msg);
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
  var numBroad = AdWordsApp.keywords()
  .withCondition('Status = ENABLED')
  .withCondition('AdGroupStatus = ENABLED')
  .withCondition('CampaignStatus = ENABLED')
  .withCondition('KeywordMatchType = BROAD')
  .get().totalNumEntities();
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
  var total = numBroad+numPhrase+numExact;
  var percBroad = Math.round(numBroad/total*100);
  var percPhrase = Math.round(numPhrase/total*100);
  var percExact = Math.round(numExact/total*100);
  
  MATCH_TYPES = MATCH_TYPES.concat(['\nOut of a total of: '+total+' active keywords in your account:'],['\nMatch Type', 'Number','Percent']);
  MATCH_TYPES = MATCH_TYPES.concat(['\nBroad',numBroad,percBroad +'%']);
  MATCH_TYPES = MATCH_TYPES.concat(['\nPhrase',numPhrase, percPhrase + '%']);
  MATCH_TYPES = MATCH_TYPES.concat(['\nExact',numExact, percExact + '%']);
}

// Verify the number of ads per ad group
function verifyAdNum() {
  var agIter = AdWordsApp.adGroups()
  .withCondition('Status = ENABLED')
  .withCondition('CampaignStatus = ENABLED')
  .get();
  var level = 'ad';
  while(agIter.hasNext()) {
    var ag = agIter.next();
    var campaign = ag.getCampaign().getName();
    var adGroup = ag.getName();
    var adCount = ag.ads().withCondition('Status = ENABLED').get().totalNumEntities();
    if(adCount < NUMBER_OF_ADS) {
      var msg = 'Campaign: "'+campaign+'" AdGroup: "'+adGroup+'" does not have enough ads: '+adCount;
      warn(msg);
      AD_NUM_LIST= AD_NUM_LIST.concat(['\n'+campaign,adGroup,adCount]);
      adNum++;
    }
    if(adCount > (MAX_NUM_OF_ADS)) {
      var msg = 'Campaign: "'+campaign+'" AdGroup: "'+ adGroup +'" has too many ads: '+ adCount;
      warn(msg);
      AD_NUM_LIST = AD_NUM_LIST.concat(['\n'+campaign,adGroup,adCount]);
      adNum++;
    }
  }
}

function verifyKeywordNum() {	
  var agIter = AdWordsApp.adGroups()
  .withCondition('Status = ENABLED')
  .withCondition('CampaignStatus = ENABLED')
  .get();
  
  while(agIter.hasNext()) {
    var ag = agIter.next();
    var kwSize = ag.keywords().withCondition('Status = ENABLED').get().totalNumEntities();
    if(kwSize >= KEYWORD_NUM) {
      var campaignName= ag.getCampaign().getName();
      var adGroupName = ag.getName();
      warn('Campaign: "'+campaignName+'" AdGroup: "'+adGroupName+'" has too many keywords: ' + kwSize);      
      kwNum++;
      var msg = [campaignName,adGroupName,kwSize]
      KEYWORD_LIST = KEYWORD_LIST.concat(['\n'+msg]);
      
      
    }
  }
}

function verifyNegKeywordNum() {	
  var agIter = AdWordsApp.adGroups()
  .withCondition('Status = ENABLED')
  .withCondition('CampaignStatus = ENABLED')
  .get();
  while(agIter.hasNext()) {
    var ag = agIter.next();
    var kwSize =  ag.negativeKeywords()/*.withCondition('Status = ENABLED')*/.get().totalNumEntities();
    if(kwSize > 0) {
      var campaignName= ag.getCampaign().getName();
      var adGroupName = ag.getName();
      info('Campaign: "'+campaignName+'" AdGroup: "'+adGroupName+'" has negative keywords: ' + kwSize);
      
      var msg = [campaignName,adGroupName,kwSize]
      NEG_KEYWORD_LIST = NEG_KEYWORD_LIST.concat(['\n'+msg]);
      negKwNum++;
      
    }
  }
}

function verifyMobileModifiers() {
  var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
  while(campIter.hasNext()) {
    var camp = campIter.next();
    var desktop = camp.targeting().platforms().desktop().get().next();
    //var tablet = camp.targeting().platforms().tablet().get().next();
    var mobile = camp.targeting().platforms().mobile().get().next();
    //check for mobile modifiers
    if(desktop.getBidModifier() == 1 && mobile.getBidModifier() == 1) {
      warn('Campaign: "'+camp.getName()+'" has no mobile modifier set.');
    }
  }
}

function verifyTargetedLocations(locList) {
  var campIter = AdWordsApp.campaigns().withCondition('Status = ENABLED').get();
  while(campIter.hasNext()) {
    var camp = campIter.next();
    var locIter = camp.targeting().targetedLocations().get();
    reportOnLocations(camp,locIter,locList);
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
function reportOnLocations(camp,locIter,locList) {
  var campLocList = [];
  while(locIter.hasNext()) {
    var loc = locIter.next();
    campLocList.push(loc.getName());
    if(!locList) {
      warn('Campaign: "'+camp.getName()+'" targeting: "'+loc.getName()+'"');
    }
  }
  if(locList && campLocList.sort() != locList.sort()) {
    for(var i in campLocList) {
      if(locList.indexOf(campLocList[i]) == -1) {
        warn('Campaign: "'+camp.getName()+'" incorrectly targeting: "'+campLocList[i]+'"');
      }
    }
    for(var i in locList) {
      if(campLocList.indexOf(locList[i]) == -1) {
        warn('Campaign: "'+camp.getName()+'" not targeting: "'+locList[i]+'"');
      }
    }
  }
}

function verifySearchAndDisplay() {
  var API_VERSION = { includeZeroImpressions : false };
  var cols = ['CampaignId','CampaignName','AdNetworkType1','Impressions'];
  var report = 'CAMPAIGN_PERFORMANCE_REPORT';
  var query = ['select',cols.join(','),'from',report,'during','LAST_30_DAYS'].join(' ');
  var results = {}; // { campId : { agId : [ row, ... ], ... }, ... }
  var reportIter = AdWordsApp.report(query, API_VERSION).rows();
  while(reportIter.hasNext()) {
    var row = reportIter.next();
    if(results[row.CampaignId]) {
      warn('Campaign: "'+row.CampaignName+'" is targeting the Display and Search networks.');
    } else {
      results[row.CampaignId] = row;
    }
  }
  return results;
  
}

function newLine(){
  //EMAIL_MESSAGE.push('\n');
}
function warn(msg, level) {
  Logger.log('WARNING: '+msg);
  //info(msg);//EMAIL_MESSAGE.push('\nWARNING: '+msg);
}

function info(msg, level) {
  Logger.log(msg);
  //EMAIL_MESSAGE.push('\n'+msg);
}

function Log(msg){
  Logger.log(msg);
}

