/************************************
 * Customizer Automtion
 * Created By: Josh DeGraw
 ************************************/
var SCRIPT_NAME = ['Ad', 'Customizer'];
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

var REGEX_SKU_CODE = /(itemprop="mpn">)[sS][wW]-[0-9]+-*[A-z0-9]*/g
var REGEX_SKU_FORMAT = /([sS][wW]-[0-9]+-*[A-z0-9]*)/g
var REGEX_PRICE_FORMAT = /(\d*\.)\d+/g;
var REGEX_PRICE_CODE = [/(itemprop="price">\$\d*\.)\d+(<\/div>)/g, /(class="price belowstrike">\$\d*\.)\d+(<\/h2>)/g];
var REGEX_URL = /(http)s?:\/\//g;

function main() {
    var date = _getDate();
    var day = date.day;
    var time = date.time;
    EMAIL_LIST = [
        ['SKU', 'Promotion', 'Price', 'RoundedPrice', 'Campaign', 'AdGroup', 'Url', 'Last Updated: ', day, time]
    ];
    getPrices();
    updateReport();
    EmailResults();
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

function print(msg) {
    Logger.log(msg);
}

// A helper function to make rounding a little easier
function round(value) {
    // var decimals = Math.pow(10,DECIMAL_PLACES);
    // return Math.round(value*decimals)/decimals;
    return Math.floor(value);
}

//Helper function to format todays date
function _getDate() {
    var today = new Date();
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var dayFormat = "MM-dd-yyyy";
    var day = Utilities.formatDate(today, timeZone, dayFormat);
    var time = AM_PM(today);

    var date = {
        day: day,
        time: time
    };

    return date;
}

function AM_PM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function EmailResults() {
    var date = _getDate().day;
    var attachment = EMAIL_LIST.join();
    var subject = 'AdWords Alert: ' + SCRIPT_NAME.join('_');

    var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
    var message = emailMessage() + signature;
    var file_name = SCRIPT_NAME.join('_') + '_' + date;
    var To;
    var isPreview = '';

    if (AdWordsApp.getExecutionInfo().isPreview()) {
        To = EMAILS[0]
        isPreview = 'Preview; No changes actually made to AdWords account.\n';
    } else {
        To = EMAILS.join();
    }

    MailApp.sendEmail({
        to: To,
        subject: subject,
        body: isPreview + message,
        attachments: [{
            fileName: file_name + '.csv',
            mimeType: 'text/csv',
            content: attachment
        }]
    });

    print('Email sent to ' + To);

}

function emailMessage() {
    var date = _getDate();
    var day = date.day;
    var time = date.time;
    var message = 'Ad Customizer Updated ' + day + ' ' + time + '\n' + PriceNum + ' prices counted.';
    return message;

}