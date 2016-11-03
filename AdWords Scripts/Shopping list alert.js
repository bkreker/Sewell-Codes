/* AdWords Account Management  -- Review Google Shopping Products for sharp changes in 
* Product level Cost and Product level Avg CPC (200%+ or -200%)
* Will e-mail any offending products for review.
* Version 1.1
* Created By: Derek Martin
* DerekMartinLA.com
****************************************************************************************/
var EMAIL_ADDRESS = "joshd@sewelldirect.com";

function main() {
    var clientName = AdWordsApp.currentAccount().getName().split("-")[0];
    var offendingProducts = []; // will hold list of product items that need review
    var products = [];
     
	products = runShoppingReport();
	offendingProducts = analyzeShoppingResults(products);
	
  if (offendingProducts.length > 0) {
    var file = createSpreadsheet(clientName,offendingProducts);
    sendAnEmail(clientName[0], offendingProducts.toString(), file);
  } // end of if statement
	
} // end of main function

function runShoppingReport() {

    // var reportFormat = _.str.quote(camp.getName());
    var listOfProducts = [];
  
 	var report = AdWordsApp.report(
     'SELECT Date, Brand, OfferId, AverageCpc, Cost ' +
     'FROM   SHOPPING_PERFORMANCE_REPORT ' +
      'DURING LAST_7_DAYS');
    
     var rows = report.rows();

	 while (rows.hasNext()) {
		   var row = rows.next();
       
           var brand = row['Brand'];
		   var offerId= row['OfferId'];
           var date = row['Date'];   
		   var averageCpc = row['AverageCpc'];
		   var cost = row['Cost'];
		              
           var productResult = new productData(brand, offerId, date, averageCpc, cost);
       
           listOfProducts.push(productResult);
           
	 }  // end of report run
    
	 return listOfProducts;
     
} 

function productData(brand, offerId, date, averageCpc, totalCost) {
	this.brand = brand;
	this.offerId = offerId;
	this.date = date;
	this.averageCpc = averageCpc;
	this.totalCost = totalCost;
} // end of productData

function analyzeShoppingResults (products) {
  var listOfProducts = products;
  var listOfResults = [];
 
  listOfProducts = _.uniq(listOfProducts);
  listOfProducts.sort(); // sort list to keep things clean
  
  var i = 0;
  for each (offerId in listOfProducts) {
   
    var currentOffer = _.where(listOfProducts, {offerId: listOfProducts[i].offerId});
    if (currentOffer.length > 1) { // check if there are multiple dates at play, this way we can calculate min and max-
     
      var oldestAvgCpc = parseFloat(currentOffer[0].averageCpc);
      var oldestCost = parseFloat(currentOffer[0].totalCost);
      var newestAvgCpc = parseFloat(currentOffer[currentOffer.length - 1].averageCpc);
      var newestCost = parseFloat(currentOffer[currentOffer.length - 1].totalCost);
      
      var cpcChange = parseFloat((newestAvgCpc - oldestAvgCpc) / oldestAvgCpc).toFixed(2);
      var costChange = parseFloat((newestCost - oldestCost) / oldestCost).toFixed(2);

      var offerResult = new productResult(listOfProducts[i].brand, listOfProducts[i].offerId, oldestAvgCpc, newestAvgCpc, cpcChange, oldestCost, newestCost, costChange);
  
      listOfResults.push(offerResult);
    } // end of length if statement
    i++;
  } // end of for each
   
   listOfResults = _.uniq(listOfResults);
  
   listOfResults.sort();
   
   var uniqueList = _.uniq(listOfResults, function(item, key, productId) { 
    return item.productId;
   } );
   
  uniqueList.sort();         
   
  var offendingProducts = _.filter (uniqueList, function(product) {
    return  product.cpcDelta <= -2 || product.cpcDelta >=2 || product.costDelta >= 2 || product.costDelta <= -2 ;
  });
    
 return offendingProducts;
  
} // end of analyzeShoppingResults


function productResult (brand, id, oldCpc, newCpc, cpcDelta, oldCost, newCost, costDelta) {
  this.brand = brand;
  this.productId = id;
  this.oldCpc = oldCpc;
  this.newCpc = newCpc
  this.cpcDelta = cpcDelta;
  this.oldCost = oldCost;
  this.newCost = newCost;
  this.costDelta = costDelta;
    
} // end of productResult

function createSpreadsheet(client,results) {
  
  var productResults = results;
  var clientName = client;
  var spreadsheetName = clientName + '-shoppingreport';
  
  var newSS = SpreadsheetApp.create(spreadsheetName, results.length, 26);
  
  var sheet = newSS.getActiveSheet();
  
  var columnNames = ["ProductId", "Brand", "Old AvgCpc", "New AvgCpc", "AvgCpc Delta", "Old Daily Cost", "New Daily Cost", "Cost Delta"];
  
  var headersRange = sheet.getRange(1, 1, 1, columnNames.length);

  for (i = 0; i < productResults.length; i++) {
    
    headersRange.setValues([columnNames]);
 
     var product;
     product = productResults[i].productId;
     var productBrand = productResults[i].brand;
     var oldAvgCpc = productResults[i].oldCpc;
     var newAvgCpc = productResults[i].newCpc;
     var cpcDelta = productResults[i].cpcDelta;
     var oldCost = productResults[i].oldCost;
     var newCost = productResults[i].newCost
     var costDelta = productResults[i].costDelta;
     
    sheet.appendRow([product, productBrand, oldAvgCpc, newAvgCpc, cpcDelta, oldCost, newCost, costDelta]);
    
    // Sets the first column to a width which fits the text
    sheet.setColumnWidth(1, 300);
    
    var range = sheet.getRange(sheet.getMaxColumns(), sheet.getMaxRows());

    range.setFontFamily("Helvetica");
    range.setFontSize(30);

  }
  
  return newSS.getUrl();
  
}

function sendAnEmail (clientName, results, fileUrl) {
 
  var data = Utilities.parseCsv(results, '\t');
  var today = new Date();
  today = today.getMonth() + today.getDate() + today.getFullYear();
  
  var filename = clientName + 'search-results' + today;
  
  // Send an email with Search list attachment
   var blob = Utilities.newBlob(results, 'text/html', '');
    
  MailApp.sendEmail(EMAIL_ADDRESS, clientName + ' -  Google Shopping Alert Results ', 'There are Google Shopping Products that need your attention. You can find the results at the following URL:\n\n' + fileUrl, {
     name: 'Google Shopping Alert'
 });
  
} // end of sendAnEmail function
/* HELPER FUNCTIONS */

function warn(msg) {
  Logger.log('WARNING: '+msg);
}
 
function info(msg) {
  Logger.log(msg);
}