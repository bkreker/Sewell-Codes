function main() {
  var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1oQGfRa2YjB1SeJF5-ZI_H2ewe97eQTtPIUdk0c1tr0I/edit?usp=sharing';
  var SHEET_NAME = 'MaxCPAs';
  var campaign = 'Banana Plugs ROAS';
  var adGroups = ['Other Banana Plugs',
                 'SW-29863-12 DeadBolt Banana Plugs',
                 'SW-30530-7 - Strike Banana Plugs'];
  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  
  // Log the number of columns for the range named 'TaxRates' in the
  // spreadsheet.
  for (var i = 0; i < adGroups.length;i++){
    var adGroup = adGroups[i];
    var maxCPA = getMaxCPA(ss, sheet, campaign, adGroup);
   Logger.log('Campaign "'+ campaign+ '", AdGroup "' + adGroup+ '" has GP of ' + GP );
  }
}

function getMaxCPA(ss, sheet, campaign, adGroup){ 
  //var campaigns = sheet.getLastRow();
  var lastRow = sheet.getLastRow(); 
  var campaignColumn = 'A';
  var adGroupColumn = 'B';
  var GP_Column = 'D';
  Logger.log(lastRow);
  
  for (var row = 1; row <= lastRow; row++){
    var campaignCell = campaignColumn+row;
    
    var campaignCellValue = sheet.getRange(campaignCell).getValue().toString();
    
    if (campaignCellValue === campaign){
      for (var adGrpRow = row; sheet.getRange(campaignColumn+adGrpRow).getValue().toString() === campaign; adGrpRow++){        
        var adGrpCell = adGroupColumn+adGrpRow;
        
        var adGroupCellValue = sheet.getRange(adGrpCell).getValue().toString();
        //Logger.log('Cell: '+ adGrpCell + ' and ' +campaignCell +' is: ' + campaignCellValue+ ' ' +adGroupCellValue);    
        
        if(campaignCellValue === campaign && adGroupCellValue === adGroup){
          var GP = sheet.getRange(GP_Column+adGrpRow).getValue().toString();          
         // Logger.log('In Loop: Campaign "'+ campaignCellValue+ '", AdGroup "' + adGroupCellValue+ '" has GP of ' + GP );
          return GP;          
        }
        else{continue}
      }
    }else{continue}    
  }
  return 0.69;
}