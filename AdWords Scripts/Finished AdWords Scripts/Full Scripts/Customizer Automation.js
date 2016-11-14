/************************************
 * Customizer Automtion
 * Created By: Josh DeGraw
 ************************************/
var REPORT_NAME = ['Ad', 'Customizer'];
var PRICE_TEXT = ['itemprop="price">', '</div>']
var TITLES;

var SHEET_NAME = 'Customizer';
var EMAIL_LIST;
var CUSTOMIZER_LIST = [
    ['SKU (text)', 'Promotion (text)', 'Price (price)', 'RoundedPrice (price)', 'Target campaign', 'Target ad group', 'Target keyword']
];
var DECIMAL_PLACES = 0;
var PriceNum = 0;
var EMAILS = ['joshd@sewelldirect.com'];

var CUSTOMIZER_FILE_NAME = 'Ad Customizer';
var ACTUAL_FILE_ID = '1FCp4YgZJ9pLAuWUZGGQjIg4G_bL4yBZX7lDqT3fXyP4';
var FOLDER_ID = '0B2YtZPyR5eEcNnRRZWFJblZUTms';

var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1FCp4YgZJ9pLAuWUZGGQjIg4G_bL4yBZX7lDqT3fXyP4/edit#gid=0';

var alreadyCheckedUrls = {};
var alreadyAddedPrices = {};

var REGEX_SKU_CODE = /(itemprop="mpn">)SW-[0-9]+-*[A-z0-9]*/gi; 
var REGEX_SKU_FORMAT = /SW-[0-9]+-*[A-z0-9]*/gi;
var REGEX_PRICE_FORMAT = /(\d*\.)\d+/g;
var REGEX_PRICE_CODE = [/(itemprop="price">\$\d*\.)\d+(<\/div>)/g, /(class="price belowstrike">\$\d*\.)\d+(<\/h2>)/g];
var REGEX_URL = /(http)s?:\/\//g;

function main() {
    var date = _getDateTime();
    var day = date.day;
    var time = date.time;
    EMAIL_LIST = [
        ['SKU', 'Promotion', 'Price', 'RoundedPrice', 'Campaign', 'AdGroup', 'Url', 'Last Updated: ', day, time]
    ];
    getPrices();
    updateReport();
    EmailResults(REPORT_NAME);
}

function getFileFromDrive() {
    var filesIterator = DriveApp.getFilesByName(CUSTOMIZER_FILE_NAME);
    while (filesIterator.hasNext()) {
        var file = filesIterator.next();
        Logger.log(file.getAs(MimeType.HTML).getDataAsString());
    }
}

function updateReport() {
    print('updating report');
    var folder = DriveApp.getFolderById(FOLDER_ID);
    var files = folder.getFilesByName(CUSTOMIZER_FILE_NAME);
    while (files.hasNext()) {
        var file = files.next();
        file.setContent(CUSTOMIZER_LIST);
    }

    var url = file.getUrl();
    var ss = SpreadsheetApp.openByUrl(url);
    var sheet = ss.getActiveSheet();
    var startRange = 'A';
    var endRange = 'G';
    var lastRow = sheet.getLastRow();

    var range = sheet.getRange(startRange + '2:' + endRange + lastRow);
    var priceRange = sheet.getRange('C2:C' + lastRow);
    var roundRange = sheet.getRange('D2:D' + lastRow);

    var roundFormat = '"$"#,##0';
    var priceFormat = '"$"#,##0.00';

    roundRange.setNumberFormat(roundFormat);
    priceRange.setNumberFormat(priceFormat);
    range.sort([5, 6]);
    print('Report Updated');
}


function getPrices() {
    print('Getting Prices.');
    // Get adgroups
    var objList = [];
    var _ad_groups = getAdGroups();
    while (_ad_groups.hasNext()) {
        var ag = _ad_groups.next();
        var adGroup = ag.getName();
        var agStatus = ag.isEnabled();
        var camp = ag.getCampaign();
        var campaign = camp.getName();
        //print('Ag: '+ ag.getName());
        var _ads = getAds(ag);

        while (_ads.hasNext()) {
            // for each ad, get the url and get the price from it          
            var ad = _ads.next();
            var rawUrl = ad.urls().getFinalUrl()
            try {
                var rawUrl = ad.urls().getFinalUrl()
                var url = cleanURL(rawUrl);
                var price;
                var unique = [campaign, adGroup, url];
                var adStatus = ad.isEnabled();
                var sku;
                var added = '';
                var msg;

                // if alreadyCheckedUrls has a value for this url and if that value is a valid price, add it to the customizer
                if (alreadyCheckedUrls[url]) {
                    // 
                    if (alreadyCheckedUrls[url].priceString().match(REGEX_PRICE_FORMAT)) {
                        price = alreadyCheckedUrls[url].price;
                        sku = alreadyCheckedUrls[url].sku;
                        added = ' Already matched';
                        addToLists(sku, getPromotion(price), price, round(price), campaign, adGroup, url, unique);
                    }
                } else {
                    var item = getPriceAndSku(adGroup, rawUrl);
                    price = item.price;
                    sku = item.sku;
                    alreadyCheckedUrls[url] = {
                        price: price,
                        sku: sku,
                        priceString: function() {
                            return this.price.toString();
                        }
                    };

                    if (price.toString().match(REGEX_PRICE_FORMAT)) {
                        addToLists(sku, getPromotion(price), price, round(price), campaign, adGroup, url, unique);

                        msg = 'adGroup ' + adGroup + ', Url: ' + url + ', Price: ' + price + ', SKU: ' + sku;
                        print(msg + added);
                    }
                }
            } catch (e) {
                var errObj = [campaign, adGroup, rawUrl];
                print('Error checking ' + errObj.join(', ') + ', skipping.\n' + e);
            }
        }
    }
}

function addToLists(sku, promotion, price, roundPrice, campaign, adGroup, url, unique) {
    // Only add it if it hasn't been added already. One per unique combo
    if (!alreadyAddedPrices[unique]) {
        var priceRow = [sku, promotion, price, roundPrice, campaign, adGroup, url];
        var customizerRow = [sku, getPromotion(price), price, round(price), campaign, adGroup];
        PriceNum++;
        alreadyAddedPrices[unique] = true;

        EMAIL_LIST = EMAIL_LIST.concat('\n' + priceRow);
        CUSTOMIZER_LIST = CUSTOMIZER_LIST.concat('\n' + customizerRow);
    }
}

function getAdGroups() {
    var selector = AdWordsApp.adGroups()
        .withCondition("CampaignName DOES_NOT_CONTAIN_IGNORE_CASE 'Display'")
        .withCondition("CampaignName DOES_NOT_CONTAIN_IGNORE_CASE 'Shopping'")
        .withCondition("CampaignStatus = ENABLED")
        .withCondition("Status != DELETED")
        .withCondition("Status != PAUSED")
        .get();
    return selector;
}

function cleanURL(url) {
    var _url = url.replace(REGEX_URL, '');
    _url = _url.toLowerCase();
    return _url;
}

function getAds(ag) {
    var selector = ag.ads()
        //.withCondition('Type=TEXT_AD')
        .withCondition('Status = ENABLED')
        .get();
    return selector;
}

function getPromotion(price) {
    if (price > 100) {
        return 'Free Shipping!';
    } else {
        return '';
    }
}

function getPriceAndSku(adGroup, url) {
    var price;
    var sku;
    try {
        var htmlCode = UrlFetchApp.fetch(url).getContentText();
        var skuCode = htmlCode.match(REGEX_SKU_CODE);

        var priceCode;
        // This checks each possible price code. If it finds one, it stops, so put the more important/common one first
        for (var i = 0; i < REGEX_PRICE_CODE.length; i++) {
            if (htmlCode.match(REGEX_PRICE_CODE[i])) {
                priceCode = htmlCode.match(REGEX_PRICE_CODE[i]);
                break;
            }
        }

        if (htmlCode.indexOf(skuCode) >= 0) {
            sku = skuCode.toString().match(REGEX_SKU_FORMAT);
        } else if (adGroup.match(REGEX_SKU_FORMAT)) {
            sku = adGroup.match(REGEX_SKU_FORMAT);
        } else {
            sku = 'N/A';
        }

        if (htmlCode.indexOf(priceCode) >= 0) {
            price = priceCode.toString().match(REGEX_PRICE_FORMAT);
        } else {
            price = 'N/A';
        }
    } catch (e) {
        print('There was an issue checking ' + url + ', skipping.');
        price = 'N/A';
        sku = 'N/A';
    }
    var result = {
        price: price,
        sku: sku
    };
    return result;
}


function emailMessage() {
    var date = _getDateTime();
    var day = date.day;
    var time = date.time;
    var message = 'Ad Customizer Updated ' + day + ' ' + time + '\n' + PriceNum + ' prices counted.';
    return message;

}

function emailAttachment(){
	
	return EMAIL_LIST.join();
}

//Minified Helper Functions:
function _getDateTime(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c),e=AM_PM(a),f={day:d,time:e};return f}catch(a){throw error("_getDateTime()",a)}}function AM_PM(a){try{var b=a.getHours()+1,c=a.getMinutes(),d=b>=12?"pm":"am";b%=12,b=b?b:12,c=c<10?"0"+c:c;var e=b+":"+c+" "+d;return e}catch(b){throw error("AM_PM(date: "+a+")",b)}}function CustomDateRange(a,b,c){try{null!==a&&void 0!==a||(a=91),null!==b&&void 0!==b||(b=0),void 0!==c&&""!==c&&null!==c||(c="YYYYMMdd");var d=_daysAgo(a),e=_daysAgo(b),f=_daysAgo(a,c).toString(),g=_daysAgo(b,c).toString(),h=[d,e],i=f+","+g,j={fromStr:f,toStr:g,fromObj:d,toObj:e,dateObj:h,string:i};return j}catch(d){throw error("CustomDateRange(fromDaysAgo: "+a+", tillDate: "+b+", format: "+c+")",d)}}function _daysAgo(a,b){try{var c=new Date;c.setDate(c.getDate()-a);var d;if(void 0!=b&&""!=b&&null!=b){var e=AdWordsApp.currentAccount().getTimeZone();d=Utilities.formatDate(c,e,b)}else d={day:c.getDate(),month:c.getMonth(),year:c.getYear()};return d}catch(c){throw error("_daysAgo(num: "+a+", format: "+b+")",c)}}function _today(a){try{var d,b=new Date,c=AdWordsApp.currentAccount().getTimeZone();return d=void 0!=a&&""!=a&&null!=a?Utilities.formatDate(b,c,a):{day:b.getDate(),month:b.getMonth(),year:b.getYear(),time:b.getTime()}}catch(b){throw error("_today(format: "+a+")",b)}}function _getDateString(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c);return d}catch(a){throw error("_getDateString()",a)}}function _todayIsMonday(){try{var a=36e5,b=new Date,c=new Date(b.getTime()+a),e=(c.getTime(),c.getDay());return Logger.log("today: "+c+"\nday: "+e),1===e}catch(a){throw error("todayIsMonday",a)}}function _rolling13Week(a){try{void 0!==a&&""!==a&&null!==a||(a="YYYYMMdd");var b=CustomDateRange(98,8,a),c=CustomDateRange(91,1,a),d=b.string+" - "+c.string,e={from:b,to:c,string:d};return e}catch(b){throw error("Rolling13Week(format: "+a+")",b)}}function formatKeyword(a){try{return a=a.replace(/[^a-zA-Z0-9 ]/g,"")}catch(b){throw error("formatKeyword(keyword: "+a+")",b)}}function round(a){try{var b=Math.pow(10,DECIMAL_PLACES);return Math.round(a*b)/b}catch(b){throw error("round(value: "+a+")",b)}}function getStandardDev(a,b,c){try{var d=0;for(var e in a)d+=Math.pow(a[e].stats[c]-b,2);return 0==Math.sqrt(a.length-1)?0:round(Math.sqrt(d)/Math.sqrt(a.length-1))}catch(d){throw error("getStandardDev(entites: "+a+", mean: "+b+", stat_key: "+c+")",d)}}function getMean(a,b){try{var c=0;for(var d in a)c+=a[d].stats[b];return 0==a.length?0:round(c/a.length)}catch(c){throw error("getMean(entites: "+a+", stat_key: "+b+")",c)}}function createLabelIfNeeded(a){try{AdWordsApp.labels().withCondition("Name = '"+a+"'").get().hasNext()||AdWordsApp.createLabel(a)}catch(b){throw error("createLabelIfNeeded(name: "+a+")",b)}}function sendResultsViaEmail(a,b){try{var i,c=a.match(/\n/g).length-1,d=_getDateTime().day,e="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_titleCase(b)+"s Report - "+day,f="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",g=emailMessage(c)+f,h=SCRIPT_NAME.join("_")+d,j="";0!=c&&(AdWordsApp.getExecutionInfo().isPreview()?(i=EMAILS[0],j="Preview; No changes actually made.\n"):i=EMAILS.join(),MailApp.sendEmail({to:i,subject:e,body:j+g,attachments:[Utilities.newBlob(a,"text/csv",h+d+".csv")]}),Logger.log("Email sent to: "+i))}catch(c){throw error("sendResultsViaEmail(report: "+a+", level: "+b+")",c)}}function _titleCase(a){try{return a.replace(/(?:^|\s)\S/g,function(a){return a.toUpperCase()})}catch(b){throw error("_titleCase(str: "+a+")",b)}}function EmailResults(a){try{var g,b="AdWords Alert: "+a.join(" "),c="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",d=emailMessage()+c,e=emailAttachment(),f=_getDateString()+"_"+a.join("_"),h="";AdWordsApp.getExecutionInfo().isPreview()?(g=EMAILS[0],h="Preview; No changes actually made.\n"):g=EMAILS.join(),""!=d&&MailApp.sendEmail({to:g,subject:b,body:d,attachments:[{fileName:f+".csv",mimeType:"text/csv",content:e}]}),Logger.log("Email sent to: "+g)}catch(b){throw error("EmailResults(ReportName: "+a+")",b)}}function info(a){Logger.log(a)}function print(a){Logger.log(a)}function error(a,b){var c="ERROR in "+a+": "+b;return Logger.log(c),c}function warn(a){Logger.log("WARNING: "+a)}function isNumber(a){try{return a.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===a}catch(b){throw error("isNumber(obj: "+a+")",b)}}function hasLabelAlready(a,b){try{return a.labels().withCondition("Name = '"+b+"'").get().hasNext()}catch(c){throw error("hasLabelAlready(entity: "+a+", label"+b+")",c)}}
