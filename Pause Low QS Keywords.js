var EMAILS = [
  "joshd@sewelldirect.com", 
  "cameronp@sewelldirect.com"
];
var TITLES = ['\nCampaign','AdGroup','Keyword','MatchType','QS','Cost','ConvValue','Conversions','MaxCPC','AvgCPC','KeywordID'];
var PAUSED_LIST = [['Paused'],TITLES];
var pausedNum = 0;
var CHECKED_LIST = [['Checked'],TITLES];
var checkedNum = 0;
var MIN_QS = 4;
var MED_QS = 5;
var LABEL = "Low_QS";
var EXCEPTION_LABEL = "Low_QS_Exception";
var DATE_RANGE = 'LAST_WEEK';

//info for the sheet that will hold the Conversion Values
var CONV_SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1-dyzDaFZ8mQvHGidP6MP1P-EXNVFRzJyTxbyi4sHnFg/edit?usp=sharing';
var CONV_TIME_PERIOD = 'LAST_30_DAYS';
var CONV_SHEET_NAME = 'CONV_VALUE_REPORT';
var ADGRP_CONV_SHEET_NAME = 'AdGroupConv';

function main(){
  createLabelIfNeeded(LABEL);
  
  updateConvValReport();
  CheckOrPause();
  
  EmailResults();
}

function isException(kw){
  var labels = kw.labels().get();
  
  while(labels.hasNext()){
    var label = labels.next();
    if(label.getName() === EXCEPTION_LABEL){
      return true;
    }
    else continue;
  }
  return false;
}

function CheckOrPause(){
  Logger.log('Pausing Kws below 4.');
  var keywordSelector = AdWordsApp
  .keywords()
  .withCondition("CampaignStatus = ENABLED")
  .withCondition("AdGroupStatus = ENABLED")
  .withCondition("Status = ENABLED")
  .withCondition("QualityScore <= "+MED_QS)
  .forDateRange(DATE_RANGE);
  
  var i = 0;
  var keywordIterator = keywordSelector.get();
  while (keywordIterator.hasNext()) {
    var kw = keywordIterator.next();
    if (!isException(kw)){
      var kwId = kw.getId();
      var campaignName = kw.getCampaign().getName();
      var adGroupName = kw.getAdGroup().getName();
      var keyW = kw.getText();
      var keyword = formatKeyword(keyW);
      var qs = kw.getQualityScore();
      var maxCPC = kw.getMaxCpc();
      var matchType = kw.getMatchType();
      var kw_stats = kw.getStatsFor(DATE_RANGE);
      
      var valReport = getConvValue(campaignName, adGroupName, kwId);
      var cost = kw_stats.getCost();
      var conversions = kw_stats.getConversions();
      var convVal = valReport.ConvVal;
      var avgCPC = valReport.AvgCPC;
      
      // [['Campaign','AdGroup','Keyword','MatchType','QS','Cost','ConvValue','Conversions','MaxCPC','AvgCPC','KeywordID']];
      var msg = [campaignName,adGroupName,keyword,matchType,qs, cost, convVal, conversions, maxCPC, avgCPC, kwId];
      
      if(qs <= MIN_QS){
        Logger.log("Pausing " + msg.join());
        kw.pause();
        PAUSED_LIST = PAUSED_LIST.concat('\n' + msg);
        i++;
        pausedNum++;
        kw.applyLabel(LABEL);
      }else{
        Logger.log('Not Pausing: ' + msg.join());
        CHECKED_LIST = CHECKED_LIST.concat('\n'+msg);
        checkedNum++;
      }
    }
  }
  Logger.log('Times Looped to Pause: '+ i);
}

// Function to get date and return true if it's monday
// Days: 0: sun, 1: mon, 2: tue, 3: wed, 4: thu, 5: fri, 6: sat
function todayIsMonday(){
  var DATE_OFFSET = 3600000;
  var date = new Date();
  var today = new Date(date.getTime() + DATE_OFFSET);
  var time = today.getTime();
  var day = today.getDay();
  Logger.log('today: '+ today+ '\nday: ' + day);
  if (day === 1){
    return true; 
  }
  else{
    return false;
  }
}

function formatKeyword(keyW) {
  var keyword = keyW.replace(/[^a-zA-Z0-9 ]/g,'');
  return keyword;
}

function getConvValue(campaign, adGroup, kwId){
  var logError = 'Error Getting GP for: '+ campaign + ','+ adGroup;
  var alreadyLogged = '';
  var result;
  try{   
    
    var ss = SpreadsheetApp.openByUrl(CONV_SPREADSHEET_URL);
    var sheet = ss.getSheetByName(CONV_SHEET_NAME);
    
    ss.getRangeByName("Selected_Campaign").setValue(campaign);
    ss.getRangeByName("Selected_AdGroup").setValue(adGroup);
    ss.getRangeByName("Selected_KwId").setValue(kwId);
    
    var convVal = ss.getRangeByName("Selected_ConvVal").getValue();
    var cpc = ss.getRangeByName("Selected_CPC").getValue();
    var cost = ss.getRangeByName("Selected_Cost").getValue();
    var conversions = ss.getRangeByName("Selected_Conversions").getValue();
    
    if (convVal === "#N/A" || convVal === "" ){
      convVal = 0; 
    }
    
    if (cpc === "#N/A" || cpc === "" ){
      cpc = .50; 
    }
    
    var result = {
      ConvVal: convVal, 
      AvgCPC: cpc, 
      Cost: cost,
      Conversions: conversions,
      List: function(){return 'ConvVal: '+ this.ConvVal+ ' AvgCPC: '+ this.AvgCPC+ ' Cost: '+ this.cost + ' Conversions: '+ this.conversions;}
    };  
    
    
  }
  catch(e){  
    errorNum++;
    ERROR_LOG = ERROR_LOG.concat('\n'+campaign,adGroup, keyW, matchType);
    convVal =  10;    
  }
  
  return result;   
}

function updateConvValReport() {
  Logger.log('Updating ConvVal Report');
  var fields = 'CampaignName, AdGroupName, Id, Cost, ConversionValue, AverageCpc, Cost, Conversions ';
  var startRange = 'A';
  var endRange = 'G';
  
  var ss = SpreadsheetApp.openByUrl(CONV_SPREADSHEET_URL);
  var sheet = ss.getSheetByName(CONV_SHEET_NAME);
  
  var report = AdWordsApp.report(
    'SELECT  '+ fields +
    'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
    'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = MANUAL_CPC ' +
    'DURING ' + CONV_TIME_PERIOD
  );
  
  // Two reports since OR operator doesn't exist in AWQL
  var report2 = AdWordsApp.report(
    'SELECT  '+ fields +
    'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
    'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = ENHANCED_CPC ' +
    'DURING ' +CONV_TIME_PERIOD
  );
  
  var array = report.rows();
  var array2 = report2.rows();
  clearSheet(ss);
  var i= 2;
  while(array.hasNext()){
    var range = sheet.getRange(startRange+i+":"+endRange+i);
    var rowTotal = array.next();
    var row = [
      [rowTotal.CampaignName, 
       rowTotal.AdGroupName, 
       rowTotal.Id,
       rowTotal.ConversionValue, 
       rowTotal.AverageCpc, 
       rowTotal.Cost,
       rowTotal.Conversions	]
    ];
    
    range.setValues(row);
    
    i++;
  }
  
  while(array2.hasNext()){
    var range = sheet.getRange(startRange+i+':'+endRange+i);
    var rowTotal = array2.next();
    var row = [
      [rowTotal.CampaignName, 
       rowTotal.AdGroupName, 
       rowTotal.Id,
       rowTotal.ConversionValue, 
       rowTotal.AverageCpc, 
       rowTotal.Cost,
       rowTotal.Conversions	]
      
    ];
    Logger.log(row.join());
    range.setValues(row);
    range.setValue(row);
    i++;
  }
  
  var lastRow = sheet.getLastRow();
  var range = sheet.getRange(startRange+'2:'+endRange + lastRow);
  
  range.sort([1,2]);
}

function clearSheet(ss){
  var campRange = ss.getRangeByName('CampaignName');
  var adGrpRange = ss.getRangeByName('AdGroupName');
  var kwIdRange = ss.getRangeByName('KwId');
  var convRange = ss.getRangeByName('Conversions');
  var costRange = ss.getRangeByName('Cost');
  var convValRange = ss.getRangeByName('ConversionValue');
  var cpcRange = ss.getRangeByName('AverageCpc');
  
  
  campRange.clear({contentsOnly: true});
  convRange.clear({contentsOnly: true});
  adGrpRange.clear({contentsOnly: true});
  costRange.clear({contentsOnly: true});
  convValRange.clear({contentsOnly: true});
  cpcRange.clear({contentsOnly: true});
  kwIdRange.clear({contentsOnly: true});
  convRange.clear({contentsOnly: true});
}

function createLabelIfNeeded(name) {
  if(!AdWordsApp.labels().withCondition("Name = '"+ name +"'").get().hasNext()) {
    AdWordsApp.createLabel(name);
  }
}

function EmailResults() {
  var Subject =  'AdWords Alert: Quality Score Monitor';
  var Message  = emailMessage();
  var Attachment = emailAttachment();
  var To;
  
  if(AdWordsApp.getExecutionInfo().isPreview()){ 
		To = EMAILS[0] ;
		Message = 'Preview\n'+Message;
	}
  else{
		To = EMAILS.join();
	}
  
  if(Message != ''){   
    MailApp.sendEmail({
      to: To,
      subject: Subject,
      body: Message,
      attachments:[{fileName: 'Low_QS_'+_getDateString()+'.csv', mimeType: 'text/csv', content: Attachment}]
    });
    
  }
}
function emailAttachment(){
  var attachment = '';
  if(pausedNum >0){
    attachment = PAUSED_LIST.join();
  }
  if(checkedNum >0){
    if (attachment != ''){attachment+='\n\n';}
    attachment += CHECKED_LIST.join();
  }
  return attachment;
  
}

function emailMessage(){
  var message = '';
  if (pausedNum > 0){
    message += pausedNum + ' keywords were paused due to  having a QS below ' + MIN_QS + '.';
  }
  if (checkedNum > 0){
    if (message != ''){message+='\n\n';}
    message += checkedNum + ' keywords have a QS of ' + MED_QS + '.';
  }
  return message;
}


//Helper function to format todays date
function _getDateString() {
  var date = Utilities.formatDate((new Date()), AdWordsApp.currentAccount().getTimeZone(), "MM-dd-yyyy");
  return date;
}
