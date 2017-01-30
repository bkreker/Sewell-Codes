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

var pausedNum = 0;
var enabledNum = 0;
var pausedAdGrpNum = 0;
var enabledAdGrpNum = 0;

// Array to hold all newly paused urls
var pausedUrls = [
    ['Ads Paused: '],
    ['\nCampaign', 'AdGroup', 'Ad', 'URL']
];
var pausedAdGroups = [
    ['Ad Groups Paused: '],
    ['\nCampaign', 'AdGroup']
];

// Array to hold all  enabled urls
var enabledUrls = [
    ['Ads Enabled: '],
    ['\nCampaign', 'AdGroup', 'Ad', 'URL']
];
var enabledAdGroups = [
    ['Ad Groups Enabled: '],
    ['\nCampaign', 'AdGroup']
];


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

        if (pausedNum > 0) {
            Logger.log(pausedUrls.join());
        }
        if (enabledNum > 0) {
            Logger.log(enabledUrls.join());
        }

        //Send an email summarizing the changes
        EmailResults();
    } catch (e) {
        print('Error Occured: e');
        print(JSON.stringify(e, null, '\t'));
        EmailErrorReport(REPORT_NAME.join(' '), EMAILS, IS_PREVIEW, e, CompletedReport);
    }
}

function enableURLs() {
    var alreadyCheckedUrls = {};
    var iter = buildSelectorEnable().get();
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
    enabledUrls = enabledUrls.concat(msg);

    Logger.log('Ads for: ' + entity + ': ' + url + ' are now enabled.');
    enabledNum++;
}

function EnableAdGroup(adGroup) {
    var adGrpName = adGroup.getName();
    var campaignName = adGroup.getCampaign().getName();
    var msg = ['\n' + campaignName, adGrpName];
    adGroup.enable();
    adGroup.removeLabel(OUT_OF_STOCK_LABEL);
    adGroup.applyLabel(IN_STOCK_LABEL);

    // Add this to the list of enabled ad groups
    enabledAdGroups = enabledAdGroups.concat(msg);
    Logger.log('AdGroup: ' + adGrpName + ' is now enabled.');
    enabledAdGrpNum++;
}

function pauseURLs() {
    var alreadyCheckedUrls = {};
    var iter = buildSelectorPause().get();
    print("Starting Pause Function.");
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
    pausedUrls = pausedUrls.concat(msg);
    Logger.log('Ads for: ' + adGroupName + ': ' + entity.getHeadline() + ': ' + url + ' are now paused.');
    pausedNum++;
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
    pausedAdGroups = pausedAdGroups.concat(['\n' + campaignName, adGroupName]);
    Logger.log('AdGroup: ' + adGroupName + ' is now paused.');
    pausedAdGrpNum++;
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
    if (pausedNum === 0 && enabledNum === 0) {
        message = 'No major stock changes were detected; no changes were made.';
    } else {
        if (pausedNum != 0) {
            message += pausedNum + ' ads auto-paused due to lack of stock. ';
        }
        if (enabledNum != 0) {
            if (message === '') {
                message += '\n';
            }
            message += enabledNum + ' ads re-enabled due to restock.';
        }
    }
    return message;
}

function emailAttachment() {
    var attachment = '';

    if (pausedNum != 0) {
        if (attachment === '') {
            attachment += '\n\n';
        }
        attachment += pausedNum + ' ' + pausedUrls.join();
    }

    if (enabledNum != 0) {
        if (attachment === '') {
            attachment += '\n\n';
        }
        attachment += '\n\n' + enabledNum + ' ' + enabledUrls.join();
    }

    if (pausedAdGrpNum != 0) {
        attachment += '\n\n' + pausedAdGrpNum + ' ' + pausedAdGroups.join();
    }

    if (enabledAdGrpNum != 0) {
        attachment += '\n\n' + enabledAdGrpNum + ' ' + enabledAdGroups.join();
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

// Minified Helper Functions: