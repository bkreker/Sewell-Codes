var PRICE_TEXT = ['itemprop="price">','</div>'];

var RESULTS_LIST = [['SKU','Price','Campaign','AdGroup']];
var DECIMAL_PLACES = 0;
var PriceNum = 0;
var EMAILS = ['joshd@sewelldirect.com'];
var FREE_SHIPPING_LABEL = 'Free_Shipping';


var alreadyCheckedUrls = {};  
var alreadyAddedPrices = {};

var REGEX_SKU_CODE = /(itemprop="mpn">)[sS][wW]-[0-9]+-*[A-z0-9]*/g
var REGEX_SKU_FORMAT = /([sS][wW]-[0-9]+-*[A-z0-9]*)/g
var REGEX_PRICE_FORMAT = /(\d*\.)\d+/g;
var REGEX_PRICE_CODE = [/(itemprop="price">\$\d*\.)\d+(<\/div>)/g, /(class="price belowstrike">\$\d*\.)\d+(<\/h2>)/g];
var REGEX_URL = /(http)s?:\/\//g;


function main() {   
  var date = _getDate();
  var day = date.day;
  var time = date.time;
  createLabelIfNeeded(FREE_SHIPPING_LABEL);
  getPrices();
  EmailResults();
}



function getPrices(){  
  print('Getting Prices.');
  // Get adgroups
  var objList = [];
  var _ad_groups = getAdGroups();
  while(_ad_groups.hasNext()){    
    var ag = _ad_groups.next();
    var adGroup = ag.getName();
    var agStatus = ag.isEnabled();
    var camp = ag.getCampaign();
    var campaign = camp.getName();
    //print('Ag: '+ ag.getName());
    var _ads = getAds(ag);
    
    while(_ads.hasNext()){
      
      // for each ad, get the url and get the price from it
      var ad = _ads.next();
      var rawUrl = ad.urls().getFinalUrl()
      var url = cleanURL(rawUrl);
      var unique = [campaign, adGroup, url];
      var adStatus = ad.isEnabled();
      try{
        var price;
        var sku;
        var added = '';
        var msg;
        
        // if alreadyCheckedUrls has a value for this url
        if(alreadyCheckedUrls[url]){
          // if that value is a valid price, add it to the customizer
          if(alreadyCheckedUrls[url].priceString().match(REGEX_PRICE_FORMAT)){            
            price =  alreadyCheckedUrls[url].price;
            sku = alreadyCheckedUrls[url].sku;
            added = ' Already matched';            
            addShippingLabel(sku, price, camp,  ag, unique);
			
          }
        }
        else{          
          var item = getPriceAndSku(adGroup, rawUrl);
          price = item.price;
          sku = item.sku;          
          alreadyCheckedUrls[url] = {
            price: price, 
            sku: sku, 
            priceString: function(){return this.price.toString();} 
          };
          
          if(price.toString().match(REGEX_PRICE_FORMAT)){  
            
			addShippingLabel(sku, price, camp,  ag, unique);
          }            
        }
      }
      catch(e){
        print('Error checking ' + unique.join() + ', skipping.\n' + e.message);
      }
    }
  }    
}

function addShippingLabel(sku, price, camp,  ag, unique){
  var adGroup = ag.getName();
  var campaign = camp.getName();
  
  if(!alreadyAddedPrices[unique]){
    var priceRow = [sku, campaign, adGroup, price];    
    if(price > 100 && RESULTS_LIST.indexOf(adGroup) < 0){
      
      PriceNum++; 
      alreadyAddedPrices[unique] = true;       
      print('Added label to: ' + priceRow.join());
      RESULTS_LIST = RESULTS_LIST.concat('\n' + priceRow);
      ag.applyLabel(FREE_SHIPPING_LABEL);
    }
  }
}



function getAdGroups(){
  var selector = AdWordsApp.adGroups()
  .withCondition("CampaignStatus = ENABLED")
  .withCondition("Status != DELETED")  
  .withCondition("Status != PAUSED")
  .get();  
  return selector;                 
}

function cleanURL(url){
  var _url = url.replace(REGEX_URL, '');
  _url = _url.toLowerCase();
  return _url;
}

function getAds(ag){
  var selector = ag.ads()
  .withCondition('Type=TEXT_AD')
  .withCondition('Status = ENABLED')
  .get(); 
  return selector;
}

function getPriceAndSku(adGroup, url){
  var price;
  var sku;
  try{
    var htmlCode = UrlFetchApp.fetch(url).getContentText();
    var skuCode = htmlCode.match(REGEX_SKU_CODE);
    
    var priceCode;
    // This checks each possible price code. If it finds one, it stops, so put the more important/common one first
    for (var i = 0; i < REGEX_PRICE_CODE.length; i++){
      if(htmlCode.match(REGEX_PRICE_CODE[i])){
        priceCode = htmlCode.match(REGEX_PRICE_CODE[i]);
        break;
      }
    }
    
    if(htmlCode.indexOf(skuCode) >= 0){
      sku = skuCode.toString().match(REGEX_SKU_FORMAT);
    }
    else if (adGroup.match(REGEX_SKU_FORMAT)){
      sku = adGroup.match(REGEX_SKU_FORMAT);
    }
    else{sku = 'N/A';}
    
    if(htmlCode.indexOf(priceCode) >= 0){
      price = priceCode.toString().match(REGEX_PRICE_FORMAT);
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

function createLabelIfNeeded(name) {
  if(!AdWordsApp.labels().withCondition("Name = '"+name+"'").get().hasNext()) {
    AdWordsApp.createLabel(name);
  }
}
// A helper function to make rounding a little easier
function round(value) {
  var decimals = Math.pow(10,DECIMAL_PLACES);
  return Math.round(value*decimals)/decimals;
}

//Helper function to format todays date
function _getDate() {
  var today = new Date();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();  
  var dayFormat = "MM-dd-yyyy";  
  var day = Utilities.formatDate(today, timeZone , dayFormat);
  var time = AM_PM(today);
  
  var date = {
    day: day,
    time: time
  };
  
  return date;  
} 

function AM_PM(date){
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var ampm = hours >= 12 ? 'pm' : 'am';
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? '0'+minutes : minutes;
  var strTime = hours + ':' + minutes + ' ' + ampm;
  return strTime; 
}

function EmailResults() {
  var date = _getDate();
  var day = date.day;
  var time = date.time;
  var To = EMAILS.join();
  var subject =  'AdWords Alert: Free Shipping Labels';	
  var message  =  'Labels added to '+PriceNum + ' AdGroups on ' + day + ' ' + time;
  var attachment = RESULTS_LIST.join();
  
  MailApp.sendEmail({
    to: To,
    subject: subject,
    body: message,
    attachments:[{fileName: day + '_Ad_Customizer.csv', mimeType: 'text/csv', content: attachment}]
  });    
  print('Email sent to '+ To);
  
}
