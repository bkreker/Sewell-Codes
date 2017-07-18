/**********************************
 * Item Stock Checker
 * Loosely based on template Created By Russ Savage
 * Customized by Josh DeGraw
 ***********************************/
var REPORT_NAME = ['Stock', 'Checker'];
var URL_LEVEL = 'Ad'; // or Keyword
var ONLY_ACTIVE = true; // set to false to check keywords or ads in all campaigns (paused and active)
var CAMPAIGN_LABEL = ''; // set this if you want to only check campaigns with this label
var STRIP_QUERY_STRING = true; // set this to false if the stuff that comes after the question mark is important
var WRAPPED_URLS = false; // set this to true if you use a 3rd party like Marin or Kenshoo for managing you account

// Email addresses to send the report to. The first email is for who debugs the code
var EMAILS = [
    "joshd@sewelldirect.com",
    'paul@sewelldirect.com',
    "jarom@sewelldirect.com",
    "sean@sewelldirect.com"
];

var OUT_OF_STOCK_LABEL = "Out_of_Stock"; // The label that is added to newly paused ads
var IN_STOCK_LABEL = "Now_In_Stock"; // The label that is added to newly enabled ads
var OUT_OF_STOCK_LABEL_ID = AdWordsApp.labels()
    .withCondition('Name = "' + OUT_OF_STOCK_LABEL + '"')
    .get().next().getId();

var IN_STOCK_LABEL_ID = AdWordsApp.labels()
    .withCondition('Name = "' + IN_STOCK_LABEL + '"')
    .get().next().getId();

var PAUSED_SKUS = ['SKUs Paused: '];
var ENABLED_SKUS = ['SKUs Enabled: '];

var PausedNum = 0;
var EnabledNum = 0;
var PausedAdGrpNum = 0;
var EnabledAdGrpNum = 0;
var ChangedNum = function() { return PausedNum + EnabledNum; }

// Array to hold all newly paused urls
var PausedUrls = [
    ['Ads Paused: '],
    ['\nCampaign', 'AdGroup', 'Ad', 'URL']
];
var PausedAdGroups = [
    ['Ad Groups Paused: '],
    ['\nCampaign', 'AdGroup']
];
// Array to hold all  enabled urls
var EnabledUrls = [
    ['Ads Enabled: '],
    ['\nCampaign', 'AdGroup', 'Ad', 'URL']
];
var EenabledAdGroups = [
    ['Ad Groups Enabled: '],
    ['\nCampaign', 'AdGroup']
];
var CompletedReport = false;


// This is the specific text (or texts) to search for 
// on the page that indicates the item 
// is out of stock. If ANY of these match the html
// on the page, the item is considered "out of stock"
var OUT_OF_STOCK_TEXTS = [
    '<h4 id="stockStatus" itemprop="availability">Out of Stock</h4>',
    '<h4 id="stockStatus" itemprop="availability">Discontinued</h4>'
];

var SKU_TEXT = ['<span itemprop="mpn">', '</span>'];

// Same, but for being in stock
var IN_STOCK_TEXTS = [
    '<h4 id="stockStatus" itemprop="availability">In Stock</h4>'
];
// Helper Functions
function main() {
    // Create the labels if needed
    try {
        createLabelIfNeeded(OUT_OF_STOCK_LABEL);
        createLabelIfNeeded(IN_STOCK_LABEL);

        // Pause ads that need pausing
        pauseURLs();
        pauseOutOfStockAdGroups();

        // Enable ads that need enabling
        enableURLs();

        if (PausedNum > 0) {
            print(PausedUrls.join());
        }
        if (EnabledNum > 0) {
            print(EnabledUrls.join());
        }
        CompletedReport = true;
      print('Changes made: ' + ChangedNum());
        //Send an email summarizing the changes
        if (ChangedNum() == 0) {
          print('No stock changes made.');
            EMAILS = [EMAILS[0]];
            
        }
        EmailReportResults(EMAILS, REPORT_NAME, emailMessage(), emailAttachment(), IS_PREVIEW);

    } catch (e) {
        error('main', e);
        EmailErrorReport(REPORT_NAME.join(' '), EMAILS, IS_PREVIEW, e, CompletedReport);
    }
}

function enableURLs() {
    var alreadyCheckedUrls = {};
    var iter = buildSelectorEnable().get();
    print('Enabling URLs.');
    while (iter.hasNext()) {
        var entity = iter.next();
        var adGroup = entity.getAdGroup();
        var urls = [];

        // get the Urls
        if (entity.urls().getFinalUrl()) {
            urls.push(entity.urls().getFinalUrl());
        }
        if (entity.urls().getMobileFinalUrl()) {
            urls.push(entity.urls().getMobileFinalUrl());
        }

        for (var i in urls) {
            // remove tracking info
            var url = cleanUrl(urls[i]);

            if (alreadyCheckedUrls[url]) {
                if (alreadyCheckedUrls[url] === 'in stock') {
                    // if it's already been checked and it's in stock, enable it
                    Enable(entity, url);

                    // If the ad group is paused, re-enable it
                    if (adGroup.isPaused()) {
                        EnableAdGroup(adGroup);
                    }
                }
            } else {
                var htmlCode;
                try {
                    htmlCode = UrlFetchApp.fetch(url).getContentText();
                } catch (e) {
                    Logger.log('There was an issue checking:' + url + ', Skipping.');
                    continue;
                }

                var did_enable = false;

                // Check if the ad is now in stock.
                for (var x in IN_STOCK_TEXTS) {

                    // If this finds the IN_STOCK_TEXTS anywhere on the page, it will enable the ad.
                    if (htmlCode.indexOf(IN_STOCK_TEXTS[x]) >= 0) {
                        alreadyCheckedUrls[url] = 'in stock';

                        Enable(entity);
                        // if the ad group is paused, re-enable it.
                        if (adGroup.isPaused()) {
                            EnableAdGroup(adGroup);
                        }
                        // Flag it as enabled
                        did_enable = true;
                        Logger.log('Url: ' + url + ' is ' + alreadyCheckedUrls[url]);
                        break;
                    }
                }

                // If it wasn't re-enabled, make sure that it's still paused.
                if (!did_enable) {
                    alreadyCheckedUrls[url] = 'out of stock';
                    entity.pause();
                }
            }
        }
    }

  print('Finished. Enabled: '+ EnabledNum +' URls');
}

function Enable(entity, url) {
    var campaignName = entity.getCampaign().getName();
    var adGroupName = entity.getAdGroup().getName();
    var headline = entity.getHeadline();
    var msg = ['\n' + campaignName, adGroupName, headline, url];

    entity.enable();
    entity.removeLabel(OUT_OF_STOCK_LABEL);
    entity.applyLabel(IN_STOCK_LABEL);

    // Add this to the list of enabled urls
    EnabledUrls = EnabledUrls.concat(msg);

    Logger.log('Ads for: ' + entity + ': ' + url + ' are now enabled.');
    EnabledNum++;
}

function EnableAdGroup(adGroup) {
    var adGrpName = adGroup.getName();
    var campaignName = adGroup.getCampaign().getName();
    var msg = ['\n' + campaignName, adGrpName];
    adGroup.enable();
    adGroup.removeLabel(OUT_OF_STOCK_LABEL);
    adGroup.applyLabel(IN_STOCK_LABEL);

    // Add this to the list of enabled ad groups
    EenabledAdGroups = EenabledAdGroups.concat(msg);
    Logger.log('AdGroup: ' + adGrpName + ' is now enabled.');
    EnabledAdGrpNum++;
}

function pauseURLs() {
    print("Pausing URLs.");
    var alreadyCheckedUrls = {};
    var iter = buildSelectorPause().get();
    while (iter.hasNext()) {
        var entity = iter.next();
        var adGroup = entity.getAdGroup();
        var urls = [];
        // Get the Urls
        if (entity.urls().getFinalUrl()) {
            urls.push(entity.urls().getFinalUrl());
        }
        if (entity.urls().getMobileFinalUrl()) {
            urls.push(entity.urls().getMobileFinalUrl());
        }

        for (var i in urls) {
            // Remove tracking info from URL
            var url = cleanUrl(urls[i]);

            // If it's been checked already in a different ad and it's out of stock, pause it
            if (alreadyCheckedUrls[url]) {
                if (alreadyCheckedUrls[url] === 'out of stock') {
                    Pause(entity, url);
                }

            } else {
                var htmlCode;
                try {
                    // This pulls all the html from the URL of the ad
                    htmlCode = UrlFetchApp.fetch(url).getContentText();
                } catch (e) {
                    Logger.log('There was an issue checking: ' + entity + ' ' + url + ', Skipping.');
                    continue;
                }

                // Flag for paused status
                var did_pause = false;

                for (var j in OUT_OF_STOCK_TEXTS) {
                    // if the OUT_OF_STOCK text code from above is found anywhere on the page, pause the ad
                    if (htmlCode.indexOf(OUT_OF_STOCK_TEXTS[j]) >= 0) {
                        alreadyCheckedUrls[url] = 'out of stock';
                        Pause(entity, url);
                        did_pause = true;
                        break;
                    }
                }
                // If the above did not pause it, make sure it doesn't get paused by mistake and mark it as in stock
                if (!did_pause) {
                    alreadyCheckedUrls[url] = 'in stock';
                    entity.enable();
                }
            }
        }
    }

    print('Finished. Paused ' + PausedNum + ' URLs.');
}

function Pause(entity, url) {
    var campaignName = entity.getCampaign().getName();
    var adGroupName = entity.getAdGroup().getName();
    var headline = entity.getHeadline();
    var msg = ['\n' + campaignName, adGroupName, headline, url];
    entity.pause();
    entity.applyLabel(OUT_OF_STOCK_LABEL);
    removeInStockLabel(entity);

    // Add this to the list of paused urls
    PausedUrls = PausedUrls.concat(msg);
    Logger.log('Ads for: ' + adGroupName + ': ' + entity.getHeadline() + ': ' + url + ' are now paused.');
    PausedNum++;
}

function pauseOutOfStockAdGroups() {
    var adGroups = AdWordsApp.adGroups()
        .withCondition('CampaignStatus = ENABLED')
        .withCondition('AdGroupStatus = ENABLED')
        .get();

    while (adGroups.hasNext()) {
        adGroup = adGroups.next();
        // If all ads in the ad group are paused, and at least one ad has the out of stock Label, pause the ad group and add the out of stock label to the ad group
        if (allAdsPaused(adGroup) && isOutOfStock(adGroup)) {
            PauseAdGroup(adGroup);
        }
    }
}

function PauseAdGroup(adGroup) {
    var adGroupName = adGroup.getName();
    var campaignName = adGroup.getCampaign().getName();

    adGroup.pause();
    adGroup.applyLabel(OUT_OF_STOCK_LABEL);
    removeInStockLabel(adGroup);

    // Add this to the list of paused urls
    PausedAdGroups = PausedAdGroups.concat(['\n' + campaignName, adGroupName]);
    Logger.log('AdGroup: ' + adGroupName + ' is now paused.');
    PausedAdGrpNum++;
}

function allAdsPaused(adGroup) {
    var ads = adGroup.ads()
        .withCondition('Status = ENABLED')
        .get();

    if (ads.hasNext()) {
        return false;
    } else {
        return true;
    }
}

function isOutOfStock(adGroup) {
    var ads = adGroup.ads()
        .withCondition('LabelNames CONTAINS_ANY[' + OUT_OF_STOCK_LABEL + ']')
        .get();
    Logger.log('Ads with Label: ' + ads);
    if (ads.hasNext()) {
        Logger.log('Ads with Label: ' + ads.next());
        return true;
    } else {
        return false;
    }
}

function removeInStockLabel(entity) {
    try {
        entity.removeLabel(IN_STOCK_LABEL);
    } catch (e) {
        return;
    }

}

function cleanUrl(url) {
    if (WRAPPED_URLS) {
        url = url.substr(url.lastIndexOf('http'));
        if (decodeURIComponent(url) !== url) {
            url = decodeURIComponent(url);
        }
    }
    if (STRIP_QUERY_STRING) {
        if (url.indexOf('?') >= 0) {
            url = url.split('?')[0];
        }
    }
    if (url.indexOf('{') >= 0) {
        //Let's remove the value track parameters
        url = url.replace(/\{[0-9a-zA-Z]+\}/g, '');
    }
    return url;
}

function emailMessage() {
    var message = "";
    if (PausedNum === 0 && EnabledNum === 0) {
        message = 'No major stock changes were detected; no changes were made.';
    } else {
        if (PausedNum != 0) {
            message += PausedNum + ' ads auto-paused due to lack of stock. ';
        }
        if (EnabledNum != 0) {
            if (message === '') {
                message += '\n';
            }
            message += EnabledNum + ' ads re-enabled due to restock.';
        }
    }
    return message;
}

function emailAttachment() {
    var attachment = '';

    if (PausedNum != 0) {
        if (attachment === '') {
            attachment += '\n\n';
        }
        attachment += PausedNum + ' ' + PausedUrls.join();
    }

    if (EnabledNum != 0) {
        if (attachment === '') {
            attachment += '\n\n';
        }
        attachment += '\n\n' + EnabledNum + ' ' + EnabledUrls.join();
    }

    if (PausedAdGrpNum != 0) {
        attachment += '\n\n' + PausedAdGrpNum + ' ' + PausedAdGroups.join();
    }

    if (EnabledAdGrpNum != 0) {
        attachment += '\n\n' + EnabledAdGrpNum + ' ' + EenabledAdGroups.join();
    }

    return attachment;
}

// Conditions for pausing ads
function buildSelectorPause() {
    var selector = (URL_LEVEL === 'Ad') ? AdWordsApp.ads() : AdWordsApp.keywords();

    selector = selector
        .withCondition('CampaignStatus != DELETED')
        .withCondition('AdGroupStatus != DELETED')
        .withCondition("Labels CONTAINS_NONE [" + OUT_OF_STOCK_LABEL_ID + "]");;

    if (ONLY_ACTIVE) {
        selector = selector
            .withCondition('CampaignStatus = ENABLED');

        if (URL_LEVEL !== 'Ad') {
            selector = selector
                .withCondition('AdGroupStatus = ENABLED');
        }
    }
    return selector;
}

// Conditions for enabling ads
function buildSelectorEnable() {
    var selector = (URL_LEVEL === 'Ad') ? AdWordsApp.ads() : AdWordsApp.keywords();

    selector = selector
        .withCondition('CampaignStatus != DELETED')
        .withCondition('AdGroupStatus != DELETED')
        .withCondition("Labels CONTAINS_ANY [" + OUT_OF_STOCK_LABEL_ID + "]");

    if (ONLY_ACTIVE) {
        selector = selector
            .withCondition('CampaignStatus = ENABLED')

        if (URL_LEVEL !== 'Ad') {
            selector = selector
                .withCondition('AdGroupStatus = PAUSED')
        }
    }
    return selector;
}

// Minified Helper Functions
function _getDateTime(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r),o=AM_PM(t),n={day:a,time:o};return n}catch(t){throw error("_getDateTime()",t)}}function AM_PM(t){try{var e=t.getHours()+1,r=t.getMinutes(),a=e>=12?"pm":"am";e%=12,e=e?e:12,r=r<10?"0"+r:r;var o=e+":"+r+" "+a;return o}catch(e){throw error("AM_PM(date: "+t+")",e)}}function CustomDateRange(t,e,r){try{null!==t&&void 0!==t||(t=91),null!==e&&void 0!==e||(e=0),void 0!==r&&""!==r&&null!==r||(r="YYYYMMdd");var a=_daysAgo(t),o=_daysAgo(e),n=_daysAgo(t,r).toString(),i=_daysAgo(e,r).toString(),s=n+","+i,c={fromStr:n,toStr:i,fromObj:a,toObj:o,dateObj:[a,o],string:s};return c}catch(a){throw error("CustomDateRange(fromDaysAgo: "+t+", tillDate: "+e+", format: "+r+")",a)}}function _daysAgo(t,e){try{var r=new Date;r.setDate(r.getDate()-t);var a;if(void 0!=e&&""!=e&&null!=e){var o=AdWordsApp.currentAccount().getTimeZone();a=Utilities.formatDate(r,o,e)}else a={year:r.getYear(),month:r.getMonth(),day:r.getDate()};return a}catch(r){throw error("_daysAgo(num: "+t+", format: "+e+")",r)}}function _today(t){try{var e,r=new Date,a=AdWordsApp.currentAccount().getTimeZone();return e=void 0!=t&&""!=t&&null!=t?Utilities.formatDate(r,a,t):{day:r.getDate(),month:r.getMonth(),year:r.getYear(),time:r.getTime()}}catch(e){throw error("_today(format: "+t+")",e)}}function _getDateString(){try{var t=new Date,e=AdWordsApp.currentAccount().getTimeZone(),r="MM-dd-yyyy",a=Utilities.formatDate(t,e,r);return a}catch(t){throw error("_getDateString()",t)}}function _todayIsMonday(){try{var t=36e5,e=new Date,r=new Date(e.getTime()+t),a=(r.getTime(),r.getDay());return Logger.log("today: "+r+"\nday: "+a),1===a}catch(t){throw error("todayIsMonday",t)}}function _rolling13Week(t){try{void 0!==t&&""!==t&&null!==t||(t="YYYYMMdd");var e=CustomDateRange(98,8,t),r=CustomDateRange(91,1,t),a=e.string+" - "+r.string,o={from:e,to:r,string:a};return o}catch(e){throw error("Rolling13Week(format: "+t+")",e)}}function formatKeyword(t){try{return t=t.replace(/[^a-zA-Z0-9 ]/g,"")}catch(e){throw error("formatKeyword(keyword: "+t+")",e)}}function round(t){try{var e=Math.pow(10,DECIMAL_PLACES);return Math.round(t*e)/e}catch(e){throw error("round(value: "+t+")",e)}}function getStandardDev(t,e,r){try{var a=0;for(var o in t)a+=Math.pow(t[o].stats[r]-e,2);return 0==Math.sqrt(t.length-1)?0:round(Math.sqrt(a)/Math.sqrt(t.length-1))}catch(a){throw error("getStandardDev(entites: "+t+", mean: "+e+", stat_key: "+r+")",a)}}function getMean(t,e){try{var r=0;for(var a in t)r+=t[a].stats[e];return 0==t.length?0:round(r/t.length)}catch(r){throw error("getMean(entites: "+t+", stat_key: "+e+")",r)}}function createLabelIfNeeded(t){try{AdWordsApp.labels().withCondition("Name = '"+t+"'").get().hasNext()||AdWordsApp.createLabel(t)}catch(e){throw error("createLabelIfNeeded(name: "+t+")",e)}}function EmailErrorReport(t,e,r,a,o){var n="AdWords Alert: Error in "+t+", script "+(o?"did execute correctly ":"did not execute ")+" correctly.",i="Error on line "+a.lineNumber+":\n"+a.message+EMAIL_SIGNATURE,s=emailAttachment(),c=_getDateString()+"_"+t,l=r?e[0]:e.join();PreviewMsg=r?"Preview; No changes actually made.\n":"",""!=i&&MailApp.sendEmail({to:l,subject:n,body:PreviewMsg+i,attachments:[{fileName:c+".csv",mimeType:"text/csv",content:s}]}),print("Email sent to: "+l)}function sendResultsViaEmail(t,e){try{var r,a=t.match(/\n/g).length-1,o=_getDateTime().day,n="AdWords Alert: "+REPORT_NAME.join(" ")+" "+_titleCase(e)+"s Report - "+day,i="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",s=emailMessage(a)+i,c=REPORT_NAME.join("_")+o,l="";0!=a&&(AdWordsApp.getExecutionInfo().isPreview()?(r=EMAILS[0],l="Preview; No changes actually made.\n"):r=EMAILS.join(),MailApp.sendEmail({to:r,subject:n,body:l+s,attachments:[Utilities.newBlob(t,"text/csv",c+o+".csv")]}),Logger.log("Email sent to: "+r))}catch(r){throw error("sendResultsViaEmail(report: "+t+", level: "+e+")",r)}}function _titleCase(t){try{return t.replace(/(?:^|\s)\S/g,function(t){return t.toUpperCase()})}catch(e){throw error("_titleCase(str: "+t+")",e)}}function EmailResults(t){try{var e,r=EMAILS,a="AdWords Alert: "+t.join(" "),o=emailMessage()+EMAIL_SIGNATURE,n=emailAttachment(),i=_getDateString()+"_"+t.join("_"),s="";e=r instanceof Array?IS_PREVIEW?r[0]:r.join():r,PreviewMsg=IS_PREVIEW?"Preview; No changes actually made.\n":"",""!=o&&MailApp.sendEmail({to:e,subject:a,body:s+o,attachments:[{fileName:i+".csv",mimeType:"text/csv",content:n}]}),print("Email sent to: "+e)}catch(e){throw error("EmailResults(ReportName: "+t.join(" ")+")",e)}}function EmailReportResults(t,e,r,a,o){try{var n,i="AdWords Alert: "+e.join(" "),s=_getDateString()+"_"+e.join("_");n=t instanceof Array?o?t[0]:t.join(","):t,a=a instanceof Array?a.join(","):a,PreviewMsg=o?"Preview; No changes actually made.\n":"",""!=r&&MailApp.sendEmail({to:n,subject:i,body:PreviewMsg+r+EMAIL_SIGNATURE,attachments:[{fileName:s+".csv",mimeType:"text/csv",content:a}]}),print("Email sent to: "+n)}catch(n){error("EmailReportResults(_emails: "+t+", _reportName: "+e+", _message: "+r+", _attachment: "+a+", isPreview: "+o+"),\n"+n)}}function info(t){Logger.log(t)}function print(t){Logger.log(t)}function error(t,e){var r="";return r=e instanceof Error?e.name+" in "+t+" at line "+e.lineNumber+": "+e.message:"Error in : "+t+":\n"+e,Logger.log(r),r}function warn(t){Logger.log("WARNING: "+t)}function isNumber(t){try{return t.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===t}catch(e){throw error("isNumber(obj: "+t+")",e)}}function hasLabelAlready(t,e){try{return t.labels().withCondition("Name = '"+e+"'").get().hasNext()}catch(r){throw error("hasLabelAlready(entity: "+t+", label"+e+")",r)}}var PreviewMsg="",EMAIL_SIGNATURE="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",IS_PREVIEW=AdWordsApp.getExecutionInfo().isPreview();