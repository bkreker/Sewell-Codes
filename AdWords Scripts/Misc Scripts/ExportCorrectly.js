function getConvValue(){
  //var spreadsheet = SpreadsheetApp.create('CONV_VALUE_REPORT');
 
  var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1-dyzDaFZ8mQvHGidP6MP1P-EXNVFRzJyTxbyi4sHnFg/edit?usp=sharing';
  var SHEET_NAME = 'CONV_VALUE_REPORT';
  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  
  var report = AdWordsApp.report(
    'SELECT CampaignName, AdGroupName, ConversionValue ' +
    'FROM   ADGROUP_PERFORMANCE_REPORT ' +
    'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED ' +
    'DURING LAST_30_DAYS');
  sheet.clearContents();
  report.exportToSheet(sheet);
}

/*
var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1CnzIzsKOBmCL8C7bmqM2oBvPFWqbQrkyLMSnbS-V2tU/edit#gid=0';
var SHEET_NAME = 'CONV_VALUE_REPORT';
  	

 // Log the last cell with data in it, and its co-ordinates.
  var lastRow = sheet.getLastRow();
  var lastColumn = sheet.getLastColumn();
  var lastCell = sheet.getRange(lastRow, lastColumn);
  Logger.log('Last cell is at (%s,%s) and has value "%s".', lastRow, lastColumn,
  lastCell.getValue());
 
function main(){
	
  var campaignName = "Extenders and Converters";
  var adGroupName = "SW-29969 -HD-Link 5/20";
  var kw = "+hdmi +extender";
  // For each entity, get conv Value
  var convVal = getConvValue(campaignName, adGroupName, kw);
  
}
  
	// for each ad group, 	
	function openSpreadsheet(adGroup){
	var SPREADSHEET_URL =	'https://docs.google.com/spreadsheets/d/1oQGfRa2YjB1SeJF5-ZI_H2ewe97eQTtPIUdk0c1tr0I/edit?usp=sharing';
		//open the spreadsheet, 		
			var sheet = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
	
			//find the appropriate cell and return the data for the max CPA
			var convVal = findCell(sheet, campaign, adGroup);
			
			// For each keyword, 
			adjustBid(kw);
			
	}
	
	
	
  function updateSheet(){
	// The size of the two-dimensional array must match the size of the range.
	var values = [
		['Conversions', 'TotalConvVal', 'AvgConvVal', 'RawAvg']
	];
	var range = sheet.getRange('B1:E1');
	
	range.setValues(values);
	
	// Sets formula for cell B2 to sum of conversions
	setCellFormula(sheet, 'B2', '=COUNTIF(A:A,">0")');
	// Sets formula for cell C2 to Total Conversion value
	setCellFormula(sheet, 'C2', '=SUM(A2:A)');
	// Sets formula for cell D2 to AvgConvVal
	setCellFormula(sheet, 'D2', '=AVERAGE(A:A)');
	// Sets formula for cell E2 to RawAvg
	setCellFormula(sheet, 'E2', '=C2/B2');
	  
	  //This should get the correct value
	  var convValCell = 'D2';
	  var convVal = convValCell.getValue();
  }
  

function getConvValue(campaignName, adGroupName, kw){
  var FIELDS = 'ConversionValue';
  var CONV_TIME_PERIOD = 'LAST_30_DAYS';
  var convVal = 69.420;
  
  var report = AdWordsApp.report(
    'SELECT ' + FIELDS +
    ' FROM   KEYWORDS_PERFORMANCE_REPORT' +
    ' WHERE CampaignName='+ "'"+ campaignName + "' AND AdGroupName='" + adGroupName +"'"+
    ' DURING ' + CONV_TIME_PERIOD);
	
	conv val = updateSheet();
    
  }  
  return report;
}

function setCellFormula(sheet, cell, formula) {
  var cell = sheet.getRange(cell);
  cell.setFormula(formula);
}


*/
