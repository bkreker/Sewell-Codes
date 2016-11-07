var TEST_URL = 'https://mosorganizer.com/mos-spring-micro-usb-cable-black-1m';
var PRICE_TEXT = ['itemprop="price">','</div>']
var TITLES;
var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1zPorSi_tYrLKN30u_hONMCvcijJImASdh3PDmZI2z9g/edit?usp=sharing';
var SHEET_NAME = 'Sheet1';
var PRICE_LIST = [['SKU (text)','Promotion (text)','Price (price)','RoundedPrice (price)','Target campaign','Target ad group','Target keyword','Url']];
var alreadyAddedPrices = {};
var DECIMAL_PLACES = 0;
var PriceNum = 0;

var CUSTOMIZER_FILE_NAME = 'Customizer Test File';

var REGEX_PRODUCT_PAGE = /(itemprop="mpn">)[sS][wW]-[0-9]+-*[A-z0-9]*/g
var REGEX_SKU = /([sS][wW]-[0-9]+-*[A-z0-9]*)/g
var REGEX_PRICE = /(\d*\.)\d+/g;
var REGEX_CODE = /(itemprop="price">\$\d*\.)\d+(<\/div>)/g;

function main() {   
  getPrices();
  updateReport();
}

function getFileFromDrive() {
  var filesIterator = DriveApp.getFilesByName(CUSTOMIZER_FILE_NAME);
  while (filesIterator.hasNext()) {
    var file = filesIterator.next();
    Logger.log(file.getAs(MimeType.HTML).getDataAsString());
  }
}
function updateReport(){   
  print('updating report');
 // var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
 // var sheet = ss.getSheetByName(SHEET_NAME);
  var today = _getDateString();
  var time = _getTimeString();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();
  
  DriveApp.createFile(CUSTOMIZER_FILE_NAME, PRICE_LIST.join(), MimeType.CSV);

}

function getPrices(){  
  print('Getting Prices.');
  // Get adgroups
  var alreadyCheckedUrls = {};
  var objList = [];
  var _ad_groups = getAdGroups();
  while(_ad_groups.hasNext()){    
    var ag = _ad_groups.next();
    var adGroup = ag.getName();
    var camp = ag.getCampaign();
    var campaign = camp.getName();
    //print('Ag: '+ ag.getName());
    var _ads = ag.ads().withCondition('Type=TEXT_AD').get();
    
    while(_ads.hasNext()){
      // for each ad, get the url and get the price from it
      var ad = _ads.next();
      var url = ad.urls().getFinalUrl();
      var unique = [campaign, adGroup, url];
      var price;
      var sku;
      var added = '';
      var msg;
      var row;
      
      if(!alreadyAddedPrices[unique]){
        if(alreadyCheckedUrls[url]){
          if(alreadyCheckedUrls[url].toString().match(REGEX_PRICE)){
            
            price =  alreadyCheckedUrls[url].price;
            sku = alreadyCheckedUrls[url].sku;
            added = ' Already matched';
            
            if(!alreadyAddedPrices[unique]){              
              var row = [sku, getPromotion(price), price, round(price), campaign,  adGroup, '', url];
              PRICE_LIST = PRICE_LIST.concat(row);
            }
          }
        }
        else{          
          var item = getPriceAndSku(url);
          price = item.price;
          sku = item.sku;
          PriceNum++;
          alreadyCheckedUrls[url] = {price: price, sku: sku};
        }
        //var TITLES= ['SKU (text)','Promotion (text)','Price (price)','RoundedPrice (price)','Target campaign','Target ad group','Target keyword','Price','Url'];
        var row = [sku, getPromotion(price), price, round(price), campaign,  adGroup, '', url];
        
        alreadyAddedPrices[unique] = true;     
        
        if(price.toString().match(REGEX_PRICE)){          
          PRICE_LIST = PRICE_LIST.concat('\n'+row);
          //print('Object activation: '+ alreadyAddedPrices[unique].row());
          
          // PRICE_LIST = PRICE_LIST.concat(alreadyAddedPrices[unique]);
          msg = 'adGroup '+ adGroup + ', Url: '+ url+ ', Price: '+ price + ', SKU: '+ sku;      
          print(msg+added);
        }
      }
      
    }
  }  
  
}

function getAdGroups(){
  var selector = AdWordsApp.adGroups()
  .withCondition("CampaignStatus = ENABLED")
  .withCondition("Status = ENABLED")
  .get();  
  return selector;                 
}

function getPromotion(price){
  if (price > 100){
    return 'Free Shipping';
  }
  else{
    return '';
  }
}

function getPriceAndSku(url){
  var price;
  var sku;
  try{
    var htmlCode = UrlFetchApp.fetch(url).getContentText();
    var skuCode = htmlCode.match(REGEX_PRODUCT_PAGE);
    var priceCode = htmlCode.match(REGEX_CODE);
    
    if(htmlCode.indexOf(skuCode) >=0){
      sku = skuCode.toString().match(REGEX_SKU);
    }else{sku = 'N/A';}
    
    if(htmlCode.indexOf(priceCode) >=0){
      price = priceCode.toString().match(REGEX_PRICE);
    }else{price = 'N/A';}    
  }
  catch(e){
    print('There was an issue checking ' + url + ', skipping.');
    price = 'N/A';
    sku = 'N/A';
  }
  var result = {price: price, sku: sku};
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
