var DATE_RANGE = 'TODAY';
var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1-aTJRZvTg_bh92zPUnSykm2EPpgPPnaeCf15osUYNiE/edit?usp=sharing';
var SHEET_NAME = 'AG_ConvVals';
var CRITERIA = 'CampaignStatus = ENABLED AND AdGroupStatus = ENABLED ';

var INCLUDE_LABEL = 'AdGroup_Budgeting'
var FIELDS = 'CampaignName, AdGroupName, ConversionValue, AverageCpc, Cost, Conversions ';

var PAUSED_ADGROUPS = [['Ad Groups paused:']];

function main(){
  PAUSED_ADGROUPS = PAUSED_ADGROUPS.concat('\n'+FIELDS);
  
  //createLabelIfNeeded(INCLUDE_LABEL);
  updateConvValReport();
  
  // Several Times every day, check if ad groups are spending too much for the campaign
  checkGroups();  
  
}

function checkGroups(){  
  // first get the campaigns 

  var campaigns = campaignSelector();
  
  while (campaigns.hasNext()){
    var campaign = campaigns.next();
    
    var campStats = campaign.getStatsFor(DATE_RANGE);
    var budget = campaign.getBudget();
    
    // Then get the ad groups for that campaign
    var adGroups = adGroupSelector(campaign);
    var agNum = countAdGroups(campaign);
    var agBudget = budget / agNum;
    
    while (adGroups.hasNext()){      
      var adGroup = adGroups.next(); 
      var agStats = adGroup.getStatsFor(DATE_RANGE);
      var convReport = getConvVal(campaign, adGroup);
      
      // then see how much the ad group has spent
      var conversions = convReport.Conversions;
      var msg = 'Campaign Budget: '+ budget + ', AdGroupNum: ' + agNum+ ' AdGroup Budget: '+ agBudget;
      // if it's net positive, let it be
      if(convReport.ConvVal > convReport.Cost){
        continue;
      }
      else{
        // but otherwise if it's more than it's share of the budget, pause it,  
        if(convReport.Cost >= agBudget){
          msg += '\nPause: '+ convReport.List();
          pauseAdGroup(adGroup, msg);     
        }
      }  
    }
  }
}

function pauseAdGroup(adGroup, msg){  
  print(msg);
  PAUSED_ADGROUPS = PAUSED_ADGROUPS.concat(msg);
  adGroup.pause();     
}

function campaignSelector(){
  var campaigns = AdWordsApp.campaigns()
  .withCondition('Status = ENABLED')
  .withCondition('LabelNames CONTAINS_ANY['+INCLUDE_LABEL+']')
  .get();
  return campaigns;
}

function countAdGroups(campaign){
  var adGrps = adGroupSelector(campaign);
  var adGrpNum = 0;
  while (adGrps.hasNext()){
    var j = adGrps.next();
    adGrpNum++;
  }
  return adGrpNum;
}

function adGroupSelector(campaign){  
  var campName = campaign.getName();
  //print('agSelector ' + campName);
  var adGroups = AdWordsApp.adGroups()
  .withCondition("CampaignName = '" + campName +"'")
  .withCondition('Status = ENABLED')
  .get();
  
  return adGroups;
}
///
///  Update the Sheet to contain the most accurate conversion data
/// 
function updateConvValReport() {  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  var today = _getDateString();
  var time = _getTimeString();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();
  var timeCell = ss.getRangeByName('UpdateTime');
  var dayCell = ss.getRangeByName('UpdateDay');
  var dayCellVal = Utilities.formatDate(dayCell.getValue(), timeZone, "MM-dd-yyyy");
  var periodRange = ss.getRangeByName("TimePeriod");
  var updateTime = 'Date: ' + today + ' ' + time;
  var DATE;
  var fields = 'CampaignName, AdGroupName, Cost, ConversionValue ';
  print(updateTime);
  DATE = updateTime + '' + DATE_RANGE + '\n';
  
  // Only update it a max of once per day
  if (dayCellVal != today || periodRange.getValue() != DATE_RANGE)
  {
    print('Updating ConvVal Report');
    periodRange.setValue(DATE_RANGE);
    //var fields = 'CampaignName, AdGroupName, Cost, ConversionValue';
    var startRange = 'A';
    var endRange = 'G';
    
    var report = AdWordsApp.report(
      'SELECT  '+ fields +
      'FROM  ADGROUP_PERFORMANCE_REPORT ' +
      'WHERE '+ CRITERIA +
      'DURING ' + DATE_RANGE
    );
    
    var array = report.rows();
    clearSheet(ss);
    var i= 2;
    while(array.hasNext()){
      var range = sheet.getRange(startRange+i+':' + endRange + i);
      var rowTotal = array.next();   
      var cpa = 0;
      if (rowTotal.Conversions != 0){      
        cpa = rowTotal.Cost / rowTotal.Conversions;
      }else{
        cpa = rowTotal.Cost;
      }
      
      var row = [
        [rowTotal.CampaignName, 
         rowTotal.AdGroupName, 
         rowTotal.ConversionValue, 
         rowTotal.AverageCpc, 
         rowTotal.Cost,
         rowTotal.Conversions,
         cpa]
      ];
      
      range.setValues(row);
      
      i++;
    }
    
    var lastRow = sheet.getLastRow();
    var range = sheet.getRange(startRange + '2:' + endRange + lastRow);
    
    range.sort([1,2]);
    
    timeCell.setValue(_getTimeString());
    dayCell.setValue(today);
  }
}

///
/// Clear the named ranges
///
function clearSheet(ss){
  var campRange = ss.getRangeByName('Campaign');
  var adGrpRange = ss.getRangeByName('AdGroup');
  var convRange = ss.getRangeByName('Conversions');
  var costRange = ss.getRangeByName('Cost');
  var convValRange = ss.getRangeByName('ConversionValue');
  var cpcRange = ss.getRangeByName('AverageCpc');
  var cpaRange = ss.getRangeByName('CPA');
  
  
  campRange.clear({contentsOnly: true});
  convRange.clear({contentsOnly: true});
  adGrpRange.clear({contentsOnly: true});
  costRange.clear({contentsOnly: true});
  convValRange.clear({contentsOnly: true});
  cpaRange.clear({contentsOnly: true});
  cpcRange.clear({contentsOnly: true});
  convRange.clear({contentsOnly: true});
}


function getConvVal(campObject, agObject){
  var campaign = campObject.getName();
  var adGroup = agObject.getName();
  var logError = 'Error Getting GP for: '+ campaign + ','+ adGroup;
  var result;
  
  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  
  ss.getRangeByName("Selected_Campaign").setValue(campaign);
  ss.getRangeByName("Selected_AdGroup").setValue(adGroup);
  
  var convVal = ss.getRangeByName("Selected_ConvVal").getValue();
  var cpc = ss.getRangeByName("Selected_CPC").getValue();
  var cost = ss.getRangeByName("Selected_Cost").getValue();
  var conversions = ss.getRangeByName("Selected_Conversions").getValue();
  /*
  if (convVal === "#N/A" || convVal === "" ){
  convVal = 0; 
  }
  if (cost === "#N/A" || cost === "" ){
  cost = GetStat(campaign, adGroup, 'Cost');       
  }
  if (conversions === "#N/A" || conversions === "" ){
  conversions =  GetStat(campaign, adGroup, 'Conversions');
  }
  if (cpc === "#N/A" || cpc === "" ){
  cpc =  GetStat(campaign, adGroup, 'AvgCPC');
  }   */
  
  var result = {
    Campaign: campaign,
    AdGroup: adGroup,
    ConvVal: convVal, 
    AvgCPC: cpc, 
    Cost: cost,
    Conversions: conversions,
    List: function(){return this.Campaign+ ',' + this.AdGroup + ' ConvVal: '+ this.ConvVal+ ' AvgCPC: '+ this.AvgCPC + ' Cost: '+ this.Cost + ' Conversions: '+ this.Conversions;}
  };  
  return result;   
}

function print(msg){
  Logger.log(msg);
}

// A helper function to make rounding a little easier
function round(value) {
  var decimals = Math.pow(10,DECIMAL_PLACES);
  return Math.round(value*decimals)/decimals;
}

//Helper function to format todays date
function _getDateString() {
  var today = new Date();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();
  var format = "MM-dd-yyyy";
  var date = Utilities.formatDate(today, timeZone , format);
  return date;
  
}

//Helper function to format todays date
function _getTimeString() {
  var today = new Date();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();
  var format = "HH:mm";
  var time = Utilities.formatDate(today, timeZone , format);
  return time;
  
}

function createLabelIfNeeded(name) {
  if(!AdWordsApp.labels().withCondition("Name = '"+name+"'").get().hasNext()) {
    AdWordsApp.createLabel(name);
  }
}
