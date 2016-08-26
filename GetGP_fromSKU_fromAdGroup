var GP_SHEET_NAME = "Price_GP";
var GP_SPREADSHEET_URL = "https://docs.google.com/spreadsheets/d/1OhCVslUmFzwJsT3-tYg2Xee_sDtMu3dC3u_L_BbMfSc/edit?usp=sharing";
var DEFAULT_PRICE = 29.95;
var DEFAULT_GP = 10;
var DEFAULT_MAX_BID = 0.50;

function main() {
  var ad_groups = getAdGroups();
  
  // for each ad group
  while(ad_groups.hasNext()){
    var ag = ad_groups.next();
    var adGroup = ag.getName();
    
    var sku = getSKU(adGroup);
    var item = getItem(adGroup, sku);
    print(item.list());
  }
}

function getSKU(adGroup){
  var reg = /(((SW-)|(sw-))+[SW0-9]+[\S]+[A-z0-9])/g;
  var _sku = adGroup.match(reg);
  return _sku;
}

function getItem(adGroup, sku){
  var result;
  
  var ss = SpreadsheetApp.openByUrl(SPREADSHEET_URL);
  var sheet = ss.getSheetByName(SHEET_NAME);
  
  ss.getRangeByName("Selected_SKU").setValue(sku);
  
  var price = ss.getRangeByName("Selected_Price").getValue();
  var gp = ss.getRangeByName("Selected_GP").getValue();
  var maxBid = ss.getRangeByName("Selected_MaxBid").getValue();
  var reg = /[A-z #\/]/g;
  
  if (price .toString().match(reg)) {price = DEFAULT_PRICE;}
  if (gp    .toString().match(reg)) {gp = DEFAULT_GP;}
  if (maxBid.toString().match(reg)) {maxBid = DEFAULT_MAX_BID;}
  
  var result = {
    sku: sku,
    adGroup: adGroup,
    price: price, 
    gp: gp, 
    maxBid: maxBid,
    list: function(){return 'SKU: ' + this.sku +' AdGroup: ' + this.adGroup + ' Price: '+ this.price+ ' GP: '+ this.gp + ' maxBid: '+ this.maxBid;}
  };  
  
  return result;   
  
}

function getAdGroups(){
  var _ad_groups = AdWordsApp.adGroups()
  .withCondition("CampaignStatus = ENABLED")
  .withCondition("Status = ENABLED")
  .get();
  
  return _ad_groups;
}

function print(msg){
  Logger.log(msg); 
}
