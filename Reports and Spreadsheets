  var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1CnzIzsKOBmCL8C7bmqM2oBvPFWqbQrkyLMSnbS-V2tU/edit?usp=sharing';
  var SHEET_NAME = 'CONV_VALUE_REPORT';
  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  
  sheet.clearContents();
  // Appends a new row with 3 columns to the bottom of the
  // spreadsheet containing the values in the array.
  sheet.appendRow(['a man', 'a plan', 'panama']);
  
  
  function runReport() {
 
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var preadsheetsheet = ss.getSheetByName(SHEET_NAME);
  var report = AdWordsApp.report(

  var rows = report.rows();
  while (rows.hasNext()) {
    var row = rows.next();
    var campaignName = row['CampaignName'];
    var clicks = row['Clicks'];
    var impressions = row['Impressions'];
    var cost = row['Cost'];
    Logger.log(campaignName + ',' + clicks + ',' + impressions + ',' + cost);
  }
}

function calculations(){
  // Sets formula for cell B2 to sum of conversions
	setCellFormula('B2', '=COUNTIF(A:A,">0")');
  // Sets formula for cell B2 to sum of conversions
	setCellFormula('C2', '=SUM(A2:A)');
  // Sets formula for cell B2 to sum of conversions
	setCellFormula('D2', '=AVERAGE(A:A)');
  // Sets formula for cell B2 to sum of conversions
	setCellFormula('E2', '=C2/B2');
	
}

function setCellFormula(cell, formula) {
  var cell = sheet.getRange(cell);
  cell.setFormula(formula);
}

function setCellValues() {

  // The size of the two-dimensional array must match the size of the range.
  var values = [
    ['Conversions', 'TotalConvVal', 'AvgConvVal', 'RawAvg']
  ];

  var range = sheet.getRange('A1:E1');
  range.setValues(values);
}

function getNamedRange() {
  // Log the number of columns for the range named 'TaxRates' in the
  // spreadsheet.
  var range = ss.getRangeByName('TaxRates');
  if (range) {
    Logger.log(range.getNumColumns());
  }
}

function exportReportToSpreadsheet(report) {		
  //var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = SpreadsheetApp.create(SHEET_NAME);
	report.exportToSheet(sheet.getActiveSheet());
	setCellValues();
	calculations();
}

function getAllValuesOnSpreadsheet() {

  // This represents ALL the data.
  var range = sheet.getDataRange();
  var values = range.getValues();

  // This logs the spreadsheet in CSV format.
  for (var i = 0; i < values.length; i++) {
    Logger.log(values[i].join(','));
  }
}