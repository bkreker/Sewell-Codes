var spreadsheet = SpreadsheetApp.openByUrl('https://docs.google.com/spreadsheets/d/13WuHs2IJ1-iD06LuZfZFQ6ec2QaQP0aTkJv9lzyDbJM/edit?usp=sharing');
	var SHEET_NAME = 'ADGROUP_REPORT';
	var FIELDS = 'ConversionValue';
	var adGroupName = "Sewell Terms - Page";
	var campaignName = "Sewell Terms - Page";
	var keywordText = "";

function main() {  	
	testTheThing();
	Logger.log(convVals);

}
function testTheThing(){
	//+ ' AND ' + 'AdGroupStatus != REMOVED AND CampaignStatus != REMOVED '
	var report = AdWordsApp.report(
	'SELECT ' + FIELDS +
	' FROM   KEYWORDS_PERFORMANCE_REPORT' +
	' WHERE AdGroupName='+ "'"+ adGroupName + "' AND CampaignName='" + campaignName +"' AND Id='" + keyword.getId() + "'";
	' DURING LAST_30_DAYS');
	Logger.log(report.next());  

}

/* function testTheThing(spreadsheet, fields, where1){
	//+ ' AND ' + 'AdGroupStatus != REMOVED AND CampaignStatus != REMOVED '
	var report = AdWordsApp.report(
	'SELECT ' + fields +
	' FROM   ADGROUP_PERFORMANCE_REPORT' +
	' WHERE AdGroupName='+ "'"+ adGroupName + "' AND CampaignName='" + "'"+ campaignName +"' AND KeywordText='" + keywordText
	' DURING LAST_30_DAYS');
	Logger.log('' + report.rows());  
	report.exportToSheet(spreadsheet.getActiveSheet());
	return report.rows();

} */



function getNamedRange(spreadsheet, rangeName) {
	// Log the number of columns for the range named 'TaxRates' in the
	// spreadsheet.
	var range = spreadsheet.getRangeByName('TaxRates');
	Logger.log(range);
	if (range) {
	Logger.log(range.getNumColumns());
	}
}

function getConvValue(spreadsheet, SHEET_NAME, AdGroupName){  

	var sheet = spreadsheet.getSheetByName(SHEET_NAME);

	// This represents ALL the data.
	var range = sheet.getDataRange();
	//Logger.log(range + "range");
	var values = range.getValues();
	//Logger.log("values: " + values);

	// This logs the spreadsheet in CSV format.
	for (var i = 0; i < values.length; i++) {
	Logger.log(values[i].join(','));
	}
}

function exportReportToSpreadsheet(fields, spreadsheet) {
	var report = AdWordsApp.report(
	'SELECT ' + fields +
	' FROM   ADGROUP_PERFORMANCE_REPORT' +
	' WHERE AdGroupName='+ "'"+ where1 + "'"  +
	' DURING LAST_30_DAYS');

	report.exportToSheet(spreadsheet.getActiveSheet());
}
//Returned will be an array of rows (although in this case we'd be expecting only 1) and the conversion value can be accessed by: