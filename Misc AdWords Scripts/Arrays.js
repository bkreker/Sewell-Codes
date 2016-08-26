function main(){
  var campaignName = "Extenders and Converters";
  var adGroupName = "SW-29969 -HD-Link 5/20";
  var kw = "+hdmi +extender";
  var convVal = getConvValue(campaignName, adGroupName, kw);
  //Logger.log(convVal); 
}
function getConvValue(campaignName, adGroupName, kw){
  var FIELDS = 'ConversionValue';
  var CONV_TIME_PERIOD = 'LAST_30_DAYS';
  var convVal = 69.420;
  
  var report = AdWordsApp.report(
    'SELECT ' + FIELDS +
    ' FROM   KEYWORDS_PERFORMANCE_REPORT' +
    ' WHERE CampaignName='+ "'"+ campaignName + "' AND AdGroupName='" + adGroupName +"'"+// AND Id='" + kw.getId() + "'" +
    ' DURING ' + CONV_TIME_PERIOD);
  //report = report.withCondition("Conversions > 0");
  var rows = report.rows();
  var conversions = [];
  var avgCount= 0;
  var avgSum = 0.0;
  while (rows.hasNext()){
    var row = rows.next();
    var ConversionVal = row['ConversionValue'];
    var convVal = ConversionVal;
    
    if(ConversionVal != 0){
      avgSum = avgSum + ConversionVal; //conversions.push(ConversionVal);
      avgCount++;
    }
    Logger.log('ConversionVal: '+ ConversionVal);
    //convVal = 
  }
  /*
  for(var i = 0; i < conversions.length; i++){
    avgSum += conversions[i];
  }*/
  var average = avgSum/avgCount;
  Logger.log('Sum: '+ avgSum+' Count: '+ avgCount)
  Logger.log('AverageConvValue: '+average);
  exportReportToSpreadsheet(report);
  return average;
}

function exportReportToSpreadsheet(report) {
  var spreadsheet = SpreadsheetApp.create('CONV_VALUE_REPORT');
  
  report.exportToSheet(spreadsheet.getActiveSheet());
}

/*
function main() {
  var array1 = ['\n"AdGroup1"','"Ad1"'];
  var array2 = ['\n"AdGroup2"','"Ad2"'];
  var array3 = array1.concat(array2);
  var array1 = [
    ['\n"AdGroup1"','"Ad1"'],
    ['\n"AdGroup2"','"Ad2"']
  ];
  array1 = array1.concat(['\n"AdGroup3"','"Ad3"']);
  //var list1 = {one:array1,two:array2};  
  
  var list = array1.join();//.concat(array3);//.join());
  Logger.log(list);
  
}

function allAdsPaused(adGroup){
	var answer = false;
	var ads = adGroup.ads()
    .get();
	
	while(ads.hasNext()){
		var ad = ads.next();
		if(ad.isEnabled()){
			answer = false;
			return false;
		}		
		if (ad.isPaused()){
			answer = true;
		}
	}		
	return answer;
}

function CheckAdGroups(){
	var adGroups = AdWordsApp.adGroups()
	.withCondition('CampaignStatus = ENABLED')
	.withCondition('AdGroupStatus = ENABLED')
	.get();
		
	while (adGroups.hasNext()){
		adGroup = adGroups.next();
      Logger.log('AdGroup: ' + adGroup.getName());
		if (allAdsPaused(adGroup)){
			pauseAdGroup(adGroup);
		}
	}
	
}
*/
/*
function main() {
  var array1 = ['\n"AdGroup1"','"Ad1"'];
  var array2 = ['\n"AdGroup2"','"Ad2"'];
  var array3 = array1.concat(array2);
  var array1 = [
    ['\n"AdGroup1"','"Ad1"'],
    ['\n"AdGroup2"','"Ad2"']
  ];
  array1 = array1.concat(['\n"AdGroup3"','"Ad3"']);
  //var list1 = {one:array1,two:array2};  
  
  var list = array1.join();//.concat(array3);//.join());
  Logger.log(list);
  
}

function allAdsPaused(adGroup){
	var answer = false;
	var ads = adGroup.ads()
    .get();
	
	while(ads.hasNext()){
		var ad = ads.next();
		if(ad.isEnabled()){
			answer = false;
			return false;
		}		
		if (ad.isPaused()){
			answer = true;
		}
	}		
	return answer;
}

function CheckAdGroups(){
	var adGroups = AdWordsApp.adGroups()
	.withCondition('CampaignStatus = ENABLED')
	.withCondition('AdGroupStatus = ENABLED')
	.get();
		
	while (adGroups.hasNext()){
		adGroup = adGroups.next();
      Logger.log('AdGroup: ' + adGroup.getName());
		if (allAdsPaused(adGroup)){
			pauseAdGroup(adGroup);
		}
	}
	
}
*/