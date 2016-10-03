/**********************************
 * Auto-Maintenance
 * Loosely based on template Created By Russ Savage
 * Customized by Josh DeGraw
 ***********************************/
var REPORT_NAME = ['Auto', 'Maintenance'];

var EMAILS = ['joshd@sewelldirect.com', 'cameronp@sewelldirect.com'];

//info for the sheet that will hold the Conversion Values
var CONV_SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1-dyzDaFZ8mQvHGidP6MP1P-EXNVFRzJyTxbyi4sHnFg/edit?usp=sharing';
var CONV_SHEET_NAME = 'CONV_VALUE_REPORT';

// Info for the sheet that holds the Max GPs
var GP_SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1oQGfRa2YjB1SeJF5-ZI_H2ewe97eQTtPIUdk0c1tr0I/edit?usp=sharing';
var GP_SHEET_NAME = 'GPs';

var DECIMAL_PLACES = 2;


var TIME_PERIOD ={
	min: _daysAgo(91),
	max: _today(),
	array: function(){return [this.min, this.max];	},
	string: function(){return this.array().join()},
	text: 'LAST_QUARTER'
};

var DEFAULT_CONV_VAL = 10;
var DEFAULT_COST = 0;
var DEFAULT_CONVERSIONS = 0;
var DEFAULT_GP = 10;
var DEFAULT_PRICE;
var DEFAULT_MAX_CPA;
var DEFAULT_MAX_BID;

var CAMPAIGN_EXCLUSIONS = ['Sewell Terms', 'Sewell Terms - Remarketing']

var TITLES = ['\nSKU', 'Campaign', 'AdGroup', 'Keyword', 'MatchType', 'QS', 'OldBid', 'NewBid', 'BidChange', 'KwBidMax', 'AvgCPC', 'Cost', 'ConversionValue', 'NetProfit', 'Conversions', 'NP_PerConv', 'CPA', 'MaxCPA', 'kwNum', 'ROI', 'Criteria', 'LifeTimeProfit'];

var reducedBids = [
    ['Reduced Bids:'], TITLES
];
var reducedNum = 0;
var increasedBids = [
    ['Increased Bids:'], TITLES
];
var increasedNum = 0;
var errorNum = 0;
var ERROR_LOG = [
    ['Errors:'],
    ['\nCampaign', 'AdGroup', 'Keyword', 'MatchType']
];
var pausedKWs = [
    ['Paused Keywords:'], TITLES
];
var pausedNum = 0;

var PAUSED_LABEL = 'Auto-Paused';
var REDUCED_LABEL = 'Auto-Reduced';
var INCREASED_LABEL = 'Auto-Increased';
var MAINTENANCE_LABELS = [
    PAUSED_LABEL,
    REDUCED_LABEL,
    INCREASED_LABEL
];

function main() {
    // Update the spreadsheet,
	//TIME_PERIOD = [_today(), _daysAgo(91)];
	info(TIME_PERIOD.string());
     getDefaults();
     updateConvValReport();
     CheckItAll();
     emailResults();
}

function getDefaults() {
    var ss = SpreadsheetApp.openByUrl(GP_SPREADSHEET_URL);
    var sheet = ss.getSheetByName(GP_SHEET_NAME);
    DEFAULT_PRICE = ss.getRangeByName('Average_Price').getValue();
    DEFAULT_GP = ss.getRangeByName('Average_GP').getValue();
    DEVAULT_MAX_CPA = DEFAULT_GP;
    DEFAULT_MAX_BID = ss.getRangeByName('Average_MaxBid').getValue();
}


function CheckItAll() {
    info("Beginning.\nUsing Data from " + TIME_PERIOD.text + '\n' + TITLES);
    var kw_iter = buildSelector();
    var alreadyLogged = '';
    var logError;
    var i = 0;
    while (kw_iter.hasNext()) {

        var kw = kw_iter.next();
        var kw_stats = kw.getStatsFor(TIME_PERIOD);
        var bidType = kw.getCampaign().getBiddingStrategyType();
        var campaign = kw.getCampaign().getName();
        if (bidStrategy(bidType) && isIncluded(campaign)) {
            i++;
            var matchType = kw.getMatchType();
            var keyW = kw.getText();
            var kwId = kw.getId();
            var keyword = formatKeyword(keyW);
            var ag = kw.getAdGroup();
            var adGroup = ag.getName();
            try {
                var kwNum = kw.getAdGroup().keywords().withCondition('Status = ENABLED').get().totalNumEntities();
                var gpReport = getMaxGP(campaign, adGroup);
                var sku = gpReport.SKU;
                var maxCPA = gpReport.GP;
                var qs = kw.getQualityScore();
                var oldBid = kw.getMaxCpc();
                var kwBidMax = maxCPA / kwNum;

                // ConvVal: convVal, 	  AvgCPC: cpc, 	  Cost: cost,	  Conversions: conversions, list: all listed out
                var convReport = getConvValue(campaign, adGroup, kwId, keyW);
                //var cost = convReport.Cost;
                //var conversions = convReport.Conversions;	
                var cost = kw_stats.getCost();
                var conversions = kw_stats.getConversions();
                var avgCPC = convReport.AvgCPC;
                var convVal = convReport.ConvVal;
                var ltProfit = getLifeTimeProfit(ag, kw);
                var cpa;
                var netProfit;
                var npPerConv;
                var roi;
                var reg = /(((SW-)|(sw-))+[SW0-9]+[\S]+[A-z0-9])/g;
                if (!sku.toString().match(reg)) {
                    sku = adGroup.match(reg);
                }
                var msg = [kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit];

                if (conversions === 0) {
                    cpa = cost;
                    netProfit = cost * -1;
                    npPerConv = netProfit;
                    roi = netProfit / cost;
                    // If it has no conversions, check it out
                    noConversions(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit);
                } else {
                    netProfit = convVal - cost;
                    cpa = (cost / (conversions * 1.0));
                    npPerConv = netProfit / conversions;
                    roi = netProfit / cost;

                    if (npPerConv < 0) {
                        // If CPA is greater than convVal per Conversion, reduce it accordingly
                        reduceHighCPCbids(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit);
                    } else {
                        // Otherwise if it's net positive, help it out
                        increaseCheapConversions(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit);
                    }
                }
            } catch (e) {
                info('Error checking ' + campaign + ' ' + adGroup + ' ' + keyword + '\n' + e);
            }

        }
    }
    info('Done.\n' + i + ' keywords checked, \n\t' + reducedNum + ' reduced, \n\t' + increasedNum + ' increased, \n\t' + pausedNum + ' paused.')

}

//-----------------------------------
// Reduce Bids on High Cost per Conversion Keywords
//-----------------------------------
function reduceHighCPCbids(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit) {

    //Let's reduce keywords with a GP 5% greater than maxGP by 20%
    var WAY_TOO_HIGH_CPA = maxCPA * 3;
    var WAY_TOO_HIGH_BID_REDUCTION_AMOUNT = .05;

    //And keywords with GP between maxGP and maxGP * 1.5 by 10%
    var TOO_HIGH_CPA = maxCPA * 2;
    var TOO_HIGH_BID_REDUCTION_AMOUNT = .03;

    var HIGH_CPA = maxCPA;
    var HIGH_BID_REDUCTION_AMOUNT = .01;

    var cpaDiff = cpa - maxCPA;
    var criteria = 'HIGH_CPA';
    var action = 'Reduced';
    var newBid = oldBid;

    // If it's pretty high, reduce it a bit
    if (cpa >= WAY_TOO_HIGH_CPA) {
        newBid = oldBid * (1 - WAY_TOO_HIGH_BID_REDUCTION_AMOUNT);
        criteria = 'WAY_TOO_HIGH_CPA';
    }
    // If it's really high, reduce it a lot
    else if (cpa >= TOO_HIGH_CPA) {
        newBid = oldBid * (1 - TOO_HIGH_BID_REDUCTION_AMOUNT);
        criteria = 'TOO_HIGH_CPA';
    } else {
        newBid = oldBid * (1 - HIGH_BID_REDUCTION_AMOUNT);
        criteria = 'HIGH_CPA';
    }
    kw.setMaxCpc(newBid);

    addLabel(kw, REDUCED_LABEL);
    var bidChange = round(newBid - oldBid);
    var message = ['\n' + sku, campaign, adGroup, keyword, matchType, qs, oldBid, round(newBid), bidChange, round(kwBidMax), avgCPC, cost, convVal, round(netProfit), conversions, round(npPerConv), round(cpa), maxCPA, kwNum, round(roi), criteria, ltProfit];
    infoReduced(message);
    reducedNum++;


}
//-----------------------------------
// Increase Bids Cheap Conversion Keywords
// This should be if ConvVal is greater than Cost AND cost is less than GP
//-----------------------------------
function increaseCheapConversions(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit) {

    //For keywords with less than 1/4 maxGP, let's pump those bids up by 15%
    var AMAZING_CPA = maxCPA / 4;
    var AMAZING_BID_INCREASE_AMOUNT = .05;

    //For keywords with between 1/2 and 1/4 maxGP, we will only increase the bids by 10%
    var GREAT_CPA = maxCPA / 2;
    var GREAT_BID_INCREASE_AMOUNT = .02;

    var newBid = oldBid;
    var action = 'Increased';
    var criteria = 'GOOD_CPA';
    if (cpa <= maxCPA) {
        // This should be if ConvVal is greater than Cost AND cpa is less than GP    
        if (cpa <= AMAZING_CPA) {
            newBid = oldBid * (1 + AMAZING_BID_INCREASE_AMOUNT);
            criteria = 'AMAZING_CPA';
        } else if (cpa <= GREAT_CPA) {
            newBid = oldBid * (1 + GREAT_BID_INCREASE_AMOUNT);
            criteria = 'GREAT_CPA';
        } else {
            return;
        }

        kw.setMaxCpc(newBid);
        addLabel(kw, INCREASED_LABEL);
        var bidChange = round(newBid - oldBid);
        var message = ['\n' + sku, campaign, adGroup, keyword, matchType, qs, oldBid, round(newBid), bidChange, round(kwBidMax), avgCPC, cost, convVal, round(netProfit), conversions, round(npPerConv), round(cpa), maxCPA, kwNum, round(roi), criteria, ltProfit];

        infoIncreased(message);
        increasedNum++;
    }
}

//-----------------------------------
// Pause Keywords That Are Not Performing
//-----------------------------------
function noConversions(kw, sku, campaign, adGroup, keyword, matchType, qs, oldBid, kwBidMax, avgCPC, cost, convVal, netProfit, conversions, npPerConv, cpa, maxCPA, kwNum, roi, ltProfit) {
    var DEFAULT_VALUE_OF_ONE_CONVERSION = 10;
    var WAY_TOO_HIGH_COST = maxCPA * 4;
    var PRETTY_HIGH_COST = maxCPA * 2;
    var STILL_TOO_HIGH_COST = maxCPA * 1.2;
    var message;
    var PRETTY_HIGH_BID_REDUCTION_AMOUNT = .05;
    var STILL_TOO_HIGH_BID_REDUCTION_AMOUNT = .03;
    var newBid = oldBid;
    var action = '';
    var criteria = 'noConversions';

    if (cost >= WAY_TOO_HIGH_COST) {
        action = 'Paused';
        criteria = 'WAY_TOO_HIGH_COST';
        kw.pause();
        addLabel(kw, PAUSED_LABEL);
        var bidChange = 0;
        var message = ['\n' + sku, campaign, adGroup, keyword, matchType, qs, oldBid, round(newBid), bidChange, round(kwBidMax), avgCPC, cost, convVal, round(netProfit), conversions, round(npPerConv), round(cpa), maxCPA, kwNum, round(roi), criteria, ltProfit];
        infoPaused(message);
        pausedNum++;
    } else {
        action = 'Reduced';
        var reduced = false;
        if (cost >= PRETTY_HIGH_COST) {
            newBid = oldBid * (1 - PRETTY_HIGH_BID_REDUCTION_AMOUNT);
            reduced = true;
            criteria = 'PRETTY_HIGH_COST'
        } else if (cost >= STILL_TOO_HIGH_COST) {
            newBid = oldBid * (1 - STILL_TOO_HIGH_BID_REDUCTION_AMOUNT);
            reduced = true;
            criteria = 'TOO_HIGH_COST';
        }

        if (reduced) {
            kw.setMaxCpc(newBid);

            addLabel(kw, REDUCED_LABEL);
            var bidChange = round(newBid - oldBid);
            var message = ['\n' + sku, campaign, adGroup, keyword, matchType, qs, oldBid, round(newBid), bidChange, round(kwBidMax), avgCPC, cost, convVal, round(netProfit), conversions, round(npPerConv), round(cpa), maxCPA, kwNum, round(roi), criteria, ltProfit];
            infoReduced(message);
            reducedNum++;
        }
    }
}

function isIncluded(campaign) {
    var result = true;
    for (var i = 0; i < CAMPAIGN_EXCLUSIONS.length; i++) {
        var excl = CAMPAIGN_EXCLUSIONS[i];
        if (campaign === excl) {
            //info(campaign + ' === ' + excl +': '+ (campaign === excl));
            return false;
        }
    }
    return result;
}

function addLabel(kw, label) {
    createLabelIfNeeded(label);
    // Skip adding the label if it already has it
    if (!hasLabelAlready(kw, label)) {
        kw.applyLabel(label);
    }
    var others = hasDifferentLabel(kw, label);
    if (others.truth) {
        for (var i in others.existingLabels) {
            kw.removeLabel(i);
        }
    }
}

//
// Get the max GP as MaxCPA from the MaxCPA sheet
//
function getMaxGP(campaign, adGroup) {
    var logError = 'Error Getting GP for: ' + campaign + ',' + adGroup;
    var alreadyLogged = '';

    var ss = SpreadsheetApp.openByUrl(GP_SPREADSHEET_URL);
    var sheet = ss.getSheetByName(GP_SHEET_NAME);
    var lastRow = sheet.getLastRow();

    var sel_camp = ss.getRangeByName('Selected_Campaign');
    var sel_adGroup = ss.getRangeByName('Selected_AdGroup');
    //Logger.log(sel_camp.getValue()+ ' ' + sel_adGroup.getValue());

    sel_camp.setValue(campaign);
    sel_adGroup.setValue(adGroup);

    //Logger.log(sel_camp.getValue()+ ' ' + sel_adGroup.getValue());

    var GP = ss.getRangeByName('Selected_GP').getValue();
    var price = ss.getRangeByName('Selected_Price').getValue();
    var maxBid = ss.getRangeByName('Selected_MaxBid').getValue();
    var SKU = ss.getRangeByName('Selected_SKU').getValue();
    var reg = /[A-z #\/]/g;

    if (GP.toString().match(reg)) {
        GP = DEFAULT_GP;
    }
    if (price.toString().match(reg)) {
        price = DEFAULT_PRICE;
    }
    if (maxBid.toString().match(reg)) {
        maxBid = DEFAULT_MAX_BID;
    }

    var result = {
        GP: GP,
        price: price,
        maxBid: maxBid,
        SKU: SKU,
        list: function() {
            return 'SKU: ' + this.SKU + ', GP: ' + this.GP + ', MaxBid: ' + this.maxBid + ', Price: ' + this.price;
        }
    };

    //Logger.log('GP: ' + GP);
    return result;
}

function getLifeTimeProfit(ag, kw) {
    var kwId = kw.getId();
    var agId = ag.getId();
    //var kwId = '940210125';
    //var agId = '1878916803';

    var report = AdWordsApp.report(
        'SELECT Cost, ConversionValue ' +
        'FROM KEYWORDS_PERFORMANCE_REPORT ' +
        'WHERE AdGroupId = ' + agId + ' AND Id = ' + kwId
    );

    var rows = report.rows();
    var profit = 0.0;
    var cost = 0.0;
    var conVal = 0.0;

    var regex = /,/g;
    while (rows.hasNext()) {
        var row = rows.next();
        var costSheet = row.Cost;
        var convSheet = row.ConversionValue;

        cost = costSheet.replace(regex, '');
        conVal = convSheet.replace(regex, '');
        profit = conVal - cost;

    }

    profit = round(profit);
    //info(agId + ' ' + kwId + ' Profit = $'+ profit + ' ($' + conVal + ' - $' + cost + ')');

    return profit;
}

//
// Get the ConvVal from the report
//
function getConvValue(campaign, adGroup, kwId, keyW) {
    var logError = 'Error Getting GP for: ' + campaign + ',' + adGroup;
    var result;
    try {
        var ss = SpreadsheetApp.openByUrl(CONV_SPREADSHEET_URL);
        var sheet = ss.getSheetByName(CONV_SHEET_NAME);

        ss.getRangeByName("Selected_Campaign").setValue(campaign);
        ss.getRangeByName("Selected_AdGroup").setValue(adGroup);
        ss.getRangeByName("Selected_KwId").setValue(kwId);

        var convVal = ss.getRangeByName("Selected_ConvVal").getValue();
        var cpc = ss.getRangeByName("Selected_CPC").getValue();
        var cost = ss.getRangeByName("Selected_Cost").getValue();
        var conversions = ss.getRangeByName("Selected_Conversions").getValue();
        var reg = /[A-z #]/g;
        if (convVal.toString().match(reg)) {

            convVal = DEFAULT_CONV_VAL;
        }
        if (cost.toString().match(reg)) {
            cost = GetStat(campaign, adGroup, kwId, 'Cost');
        }
        if (conversions.toString().match(reg)) {
            conversions = GetStat(campaign, adGroup, kwId, 'Conversions');
        }
        if (cpc.toString().match(reg)) {
            cpc = GetStat(campaign, adGroup, kwId, 'AvgCPC');
        }

        var result = {
            ConvVal: convVal,
            AvgCPC: cpc,
            Cost: cost,
            Conversions: conversions,
            List: function() {
                return 'ConvVal: ' + this.ConvVal + ' AvgCPC: ' + this.AvgCPC + ' Cost: ' + this.cost + ' Conversions: ' + this.conversions;
            }
        };


    } catch (e) {
        errorNum++;

        ERROR_LOG = ERROR_LOG.concat('\n' + e + campaign, adGroup, keyW);
        convVal = 10;
    }

    return result;
}

function GetStat(campaign, adGroup, kwId, _stat) {
    var report = AdWordsApp.keywords()
        .forDateRange(TIME_PERIOD)
        .withCondition("Campaign = " + campaign)
        .withCondition("AdGroup = " + adGroup)
        .withCondition("Id = " + kwId)
        .get();
    var Stat = 0;
    if (_stat === 'Cost') {
        Stat = report.next().getCost();
    }
    if (_stat === 'AvgCPC') {
        Stat = report.next().getAvgCPC();
    }
    if (_stat === 'Conversions') {
        Stat = report.next().getConversions();
    }
    return Stat;
}

//
//  Update the Sheet to contain the most accurate conversion data
// 
function updateConvValReport() {

    var ss = SpreadsheetApp.openByUrl(CONV_SPREADSHEET_URL);
    var sheet = ss.getSheetByName(CONV_SHEET_NAME);
    var today = _getDateString();
    var time = _getTimeString();
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var timeCell = ss.getRangeByName('UpdateTime');
    var dayCell = ss.getRangeByName('UpdateDay');
    var dayCellVal = Utilities.formatDate(dayCell.getValue(), timeZone, "MM-dd-yyyy");
    var periodRange = ss.getRangeByName("TimePeriod");
    var updateTime = today + ', ' + time;
    info('Date: ' + updateTime);
    var DATE = updateTime + ',' + TIME_PERIOD.max + '\n\n';

    // Only update it a max of once per day
    if (dayCellVal != today || periodRange.getValue() != TIME_PERIOD.text) {
        Logger.log('Updating ConvVal Report');
        periodRange.setValue(TIME_PERIOD);
        var fields = 'CampaignName, AdGroupName, Id, Cost, ConversionValue, AverageCpc, Cost, Conversions ';
        var startRange = 'A';
        var endRange = 'H';

        var report = AdWordsApp.report(
            'SELECT  ' + fields +
            'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
            'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = MANUAL_CPC ' +
            'DURING ' + TIME_PERIOD.string()
        );

        // Two reports since OR operator doesn't exist in AWQL
        var report2 = AdWordsApp.report(
            'SELECT  ' + fields +
            'FROM  KEYWORDS_PERFORMANCE_REPORT ' +
            'WHERE CampaignStatus = ENABLED AND AdGroupStatus = ENABLED AND Status = ENABLED AND BiddingStrategyType = ENHANCED_CPC ' +
            'DURING ' + TIME_PERIOD.string()
        );

        var array = report.rows();
        var array2 = report2.rows();
        clearSheet(ss);
        var i = 2;
        while (array.hasNext()) {
            var range = sheet.getRange(startRange + i + ':' + endRange + i);
            var rowTotal = array.next();
            var cpa = 0;
            if (rowTotal.Conversions != 0) {
                cpa = rowTotal.Cost / rowTotal.Conversions;
            }

            var row = [
                [rowTotal.CampaignName,
                    rowTotal.AdGroupName,
                    rowTotal.Id,
                    rowTotal.ConversionValue,
                    rowTotal.AverageCpc,
                    rowTotal.Cost,
                    rowTotal.Conversions,
                    cpa
                ]
            ];

            range.setValues(row);

            i++;
        }

        while (array2.hasNext()) {
            var range = sheet.getRange(startRange + i + ':' + endRange + i);
            var rowTotal = array2.next();
            var cpa;
            rowTotal.Conversions === 0 ? cpa = '-' : cpa = (rowTotal.Cost / rowTotal.Conversions);

            var row = [
                [rowTotal.CampaignName,
                    rowTotal.AdGroupName,
                    rowTotal.Id,
                    rowTotal.ConversionValue,
                    rowTotal.AverageCpc,
                    rowTotal.Cost,
                    rowTotal.Conversions,
                    cpa
                ]
            ];
            Logger.log(row.join());
            range.setValues(row);
            range.setValue(row);
            i++;
        }

        var lastRow = sheet.getLastRow();
        var range = sheet.getRange(startRange + '2:' + endRange + lastRow);

        range.sort([1, 2]);

        timeCell.setValue(_getTimeString());
        dayCell.setValue(today);
    }
}

//
// Clear the named ranges
//
function clearSheet(ss) {
    var campRange = ss.getRangeByName('CampaignName');
    var adGrpRange = ss.getRangeByName('AdGroupName');
    var kwIdRange = ss.getRangeByName('KwId');
    var convRange = ss.getRangeByName('Conversions');
    var costRange = ss.getRangeByName('Cost');
    var convValRange = ss.getRangeByName('ConversionValue');
    var cpcRange = ss.getRangeByName('AverageCpc');
    var cpaRange = ss.getRangeByName('CPA');


    campRange.clear({
        contentsOnly: true
    });
    convRange.clear({
        contentsOnly: true
    });
    adGrpRange.clear({
        contentsOnly: true
    });
    costRange.clear({
        contentsOnly: true
    });
    convValRange.clear({
        contentsOnly: true
    });
    cpaRange.clear({
        contentsOnly: true
    });
    cpcRange.clear({
        contentsOnly: true
    });
    kwIdRange.clear({
        contentsOnly: true
    });
    convRange.clear({
        contentsOnly: true
    });
}

function emailResults() {
    var Subject = 'AdWords Alert: ' + REPORT_NAME.join(' ');
    var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
    var Message = emailMessage() + signature;
    var Attachment = emailAttachment();
    var file_name = _getDateString() + '_' + REPORT_NAME.join('_') + TIME_PERIOD.text;
    var To;
    var isPreview = '';

    if (AdWordsApp.getExecutionInfo().isPreview()) {
        To = EMAILS[0]
        isPreview = 'Preview; No changes actually made.\n';
    } else {
        To = EMAILS.join();
    }

    if (Message != '') {
        MailApp.sendEmail({
            to: To,
            subject: Subject,
            body: isPreview + Message,
            attachments: [{
                fileName: file_name + '.csv',
                mimeType: 'text/csv',
                content: file_name + '\n' + Attachment
            }]
        });
        info('Email sent to: ' + To);
    }
}

function emailMessage() {
    var message = '';
    if (reducedNum > 0) {
        message += reducedNum + ' bid(s) auto-reduced.\n';
    }
    if (increasedNum > 0) {
        message += increasedNum + ' bid(s) auto-increased.\n';
    }
    if (pausedNum > 0) {
        message += pausedNum + ' keyword(s) auto-paused.';
    }
    return message;
}

function emailAttachment() {
    var attachment = '';
    if (reducedNum > 0) {
        attachment += reducedNum + ' ' + reducedBids.join();
    }

    if (increasedNum > 0) {
        if (attachment != '') {
            attachment += '\n\n';
        }
        attachment += increasedNum + ' ' + increasedBids.join();
    }

    if (pausedNum > 0) {
        if (attachment != '') {
            attachment += '\n\n';
        }
        attachment += pausedNum + ' ' + pausedKWs.join();
    }

    if (errorNum > 0) {
        if (attachment != '') {
            attachment += '\n\n';
        }
        attachment += errorNum + ' ' + ERROR_LOG.join();
    }

    return attachment;
}

function type(item) {
    var result = '"';
    var i = 0;
    while (i < item.length) {
        result += '"' + item[i] + '"\n';
        i++;
    }
    return result.join('","') + '"\n';
}

function bidStrategy(bidType) {
    //if( bidType != "TARGET_GP" && bidType != "CONVERSION_OPTIMIZER"){
    if (bidType == "MANUAL_CPC" || bidType == "MANUAL_CPM") {
        return true;
    } else {
        return false;
    }
}

function buildSelector() {
    var kw_iter = AdWordsApp.keywords()
        .forDateRange(TIME_PERIOD.string())
        .withCondition("Status = ENABLED")
        .withCondition("AdGroupStatus = ENABLED")
        .withCondition("CampaignStatus = ENABLED")
        .withCondition("Cost > 0")
        .withCondition("Impressions > 0")
        .get();

    return kw_iter;

}

function infoReduced(item) {
    var toLog = item.join().replace('\n', '');
    info('Reduced,' + toLog);
    reducedBids = reducedBids.concat(item);
}

function infoIncreased(item) {
    var toLog = item.join().replace('\n', '');
    info('Increased,' + toLog);
    increasedBids = increasedBids.concat(item);
}

function infoPaused(item) {
    var toLog = item.join().replace('\n', '');
    info('Paused,' + toLog);
    pausedKWs = pausedKWs.concat(item);
}

function info(item) {
    Logger.log(item);
}

function formatKeyword(keyword) {
    keyword = keyword.replace(/[^a-zA-Z0-9 ]/g, '');
    return keyword;
}

// A helper function to make rounding a little easier
function round(value) {
    var decimals = Math.pow(10, DECIMAL_PLACES);
    return Math.round(value * decimals) / decimals;
}

//Helper function to format today's date
function _getDateString() {
    var today = new Date();
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var format = "MM-dd-yyyy";
    var date = Utilities.formatDate(today, timeZone, format);
    return date;

}

function hasLabelAlready(kw, label) {
    if (kw.labels().withCondition("Name = '" + label + "'").get().hasNext()) {
        return true
    } else {
        return false;
    }
}

function hasDifferentLabel(kw, label) {
    var truth = false;
    var existingLabels = [];
    for (var i in MAINTENANCE_LABELS) {
        if (i != label) {
            if (kw.labels().withCondition("Name = '" + i + "'").get().hasNext()) {
                truth = true;
                existingLabels.push(labels.next());
            }
        }
    }
    return {
        truth: truth,
        existingLabels: existingLabels
    };
}

//Helper function to format todays date
function _getTimeString() {
    var today = new Date();
    var timeZone = AdWordsApp.currentAccount().getTimeZone();
    var format = "HH:mm";
    var time = Utilities.formatDate(today, timeZone, format);
    return time;
}

function createLabelIfNeeded(name) {
    if (!AdWordsApp.labels().withCondition("Name = '" + name + "'").get().hasNext()) {
        AdWordsApp.createLabel(name);
    }
}

//Helper function to format todays date
function _today() {
  var today = new Date();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();  
  var dayFormat = "YYYYMMDD";  
  var day = Utilities.formatDate(today, timeZone , dayFormat);
 
  return day;  
} 

// Helper function to get a date a certain number of days ago (one quarter (13 weeks) ago is 91 days)
function _daysAgo(num){  	
  var today = new Date();
  today.setDate(today.getDate() - num);
  
  var timeZone = AdWordsApp.currentAccount().getTimeZone();  
  var dayFormat = "YYYYMMDD";  
  var day = Utilities.formatDate(today, timeZone , dayFormat);
  
  return day;
}