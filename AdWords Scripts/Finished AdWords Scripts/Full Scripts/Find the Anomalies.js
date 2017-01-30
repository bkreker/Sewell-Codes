/**************************************
 * Find the Anomalies
 * Created By: Russ Savage
 * Version: 1.2
 * Changelog v1.2
 *  - Fixed divide by 0 errors
 *  - Changed SIG_FIGS to DECIMAL_PLACES
 * Changelog v1.1
 *  - Added ability to tag ad anomalies as well
 * FreeAdWordsScripts.com
 **************************************/
var DATE_RANGE = 'LAST_30_DAYS';
var DECIMAL_PLACES = 3;
var STANDARD_DEVIATIONS = 2;
var EMAILS = ['joshd@sewelldirect.com'];
var REPORT_NAME = ['Anomaly', 'Tagger'];

function main() {
    // This will add labels to and send emails about adgroups, keywords and ads. Remove any if you like.
    var levels_to_tag = ['adgroup', 'keyword' /*,'ad'*/ ];
    for (var x in levels_to_tag) {
        var report = getContentRows(levels_to_tag[x]);
        var entity_map = buildEntityMap(levels_to_tag[x]);
        for (var parent_id in entity_map) {
            var child_list = entity_map[parent_id];
            var stats_list = Object.keys(child_list[0].stats);
            for (var i in stats_list) {
                var mean = getMean(child_list, stats_list[i]);
                var stand_dev = getStandardDev(child_list, mean, stats_list[i]);
                var label_name = stats_list[i];
                report += addLabelToAnomalies(child_list, mean, stand_dev, stats_list[i], label_name, levels_to_tag[x]);
            }
        }
        sendResultsViaEmail(report, levels_to_tag[x]);
    }
}

function emailMessage(rows) {
    return "There are " + rows + " " + level + "s that have abnormal performance. See attachment for details.";
}

//Helper function to return a single row of the report formatted correctly
function toReportRow(entity, level, label_name) {
    var _row = [entity.getCampaign().getName()];

    _row.push((level == 'adgroup') ? entity.getName() : entity.getAdGroup().getName());

    if (level == 'keyword') {
        _row = _row.concat([entity.getText(), entity.getMatchType()]);
    } else if (level == 'ad') {
        _row = _row.concat([entity.getHeadline(), entity.getDescription1(), entity.getDescription2(), entity.getDisplayUrl()]);
    }
    _row.push(label_name);

    return '"' + _row.join('","') + '"\n';
}

//Helper function to return the column headings for the report
function getContentRows(level) {
    var ret_val = ['CampaignName', 'AdGroupName'];
    if (level == 'keyword') {
        ret_val = ret_val.concat(['KeywordText', 'MatchType']);
    } else if (level == 'ad') {
        ret_val = ret_val.concat(['Headline', 'Description1', 'Description2', 'DisplayUrl']);
    }
    ret_val.push('LabelName');
    return '"' + ret_val.join('","') + '"\n';
}

//Function to add the labels to the entities based on the standard deviation and mean.
//It returns a csv formatted string for reporting
function addLabelToAnomalies(entites, mean, sd, stat_key, label, level) {
    var report = '';
    var label_name = label;
    try {
        for (var i in entites) {
            var entity = entites[i]['entity'];
            //print('addLabelToAnomalies: ' + entity);
            var stat = entites[i]['stats'][stat_key];
            var rawDev = stat - mean;

            var highLabel = setLabel(label, 'high');
            var lowLabel = setLabel(label, 'low');

            var deviation = Math.abs(rawDev);
            if (sd != 0 && deviation / sd >= STANDARD_DEVIATIONS && rawDev != 0) {

                if (rawDev > 0) {
                    label_name = highLabel;
                } else if (rawDev < 0) {
                    label_name = lowLabel;
                }
                if (shouldBeLabeled(label_name)) {
                    entity.applyLabel(label_name);
                    report += toReportRow(entity, level, label_name);
                    print(entity + ' has ' + label_name + ': ' + round(stat) + ', deviation: ' + round(deviation) + ', sd: ' + round(sd) + ', rawDev: ' + round(rawDev) + ', mean: ' + round(mean));
                }

            } else {
                removeLabels(entity, highLabel, lowLabel);
            }
        }
    } catch (f) {
        Logger.log(f.message);
    }
    return report;
}

function shouldBeLabeled(label) {
    // Comment out labels that shouldn't be applied
    var possibles = [
        // highs to check
        'high_cpc',
        // 'high_cpm',
        'high_page_views',
        'high_pos',
        'high_time_on_site',
        'high_bounce',
        'high_clicks',
        // 'high_cv',
        // 'high_conv',
        'high_cost',
        'high_ctr',
        'high_imps',
        // lows to check
        'low_cpc',
        // 'low_cpm',
        'low_page_views',
        // 'low_pos',
        'low_time_on_site',
        // 'low_bounce',
        'low_clicks',
        'low_cv',
        'low_conv',
        'low_cost',
        'low_ctr',
        'low_imps'
    ];
    //print('Index of label: ' + possibles.indexOf(label));
    if (possibles.indexOf(label) >= 0) {
        return true;
    } else {
        return false;
    }
}

function setLabel(lblName, type) {
    var result = lblName;
    if (lblName.indexOf('low') < 0 && lblName.indexOf('high') < 0) {
        result = type + '_' + lblName;
    }
    createLabelIfNeeded(result);
    return result;
}

function hasLabel(entity, label) {
    var possibles = [label, 'high_' + label, 'low_' + label];
    var lblStack = entity.labels()
        .withCondition("LabelNames CONTAINS_ANY " + possibles + "")
        .get();

    // print(possibles.join());
    if (lblStack.hasNext()) {

        print('lblStack.hasNext === true ');
        return true;
    } else {
        print('lblStack.hasNext === false ');
        return false;
    }
}
// function hasLabel(entity, label){
// var lblStack = entity.labels()
// .withCondition("LabelNames CONTAINS_ANY "+ [label] +"")
// .get();

// // print(possibles.join());
// if(lblStack.hasNext()){

// print('lblStack.hasNext === true ');
// return true;
// }
// else {
// return false;
// }

// }
function removeLabels(entity, highLabel, lowLabel) {

    try {
        entity.removeLabel(highLabel);
    } catch (h) {
        print('removeLabels ' + h.message);
    }
    try {
        entity.removeLabel(lowLabel);
    } catch (l) {
        print('removeLabels: ' + l.message);
    }
}

//This function returns the standard deviation for a set of entities
//The stat key determines which stat to calculate it for
function getStandardDev(entites, mean, stat_key) {
    var total = 0;
    for (var i in entites) {
        total += Math.pow(entites[i]['stats'][stat_key] - mean, 2);
    }
    if (Math.sqrt(entites.length - 1) == 0) {
        return 0;
    }
    return round(Math.sqrt(total) / Math.sqrt(entites.length - 1));
}

//Returns the mean (average) for the set of entities
//Again, stat key determines which stat to calculate this for
function getMean(entites, stat_key) {
    var total = 0;
    for (var i in entites) {
        total += entites[i]['stats'][stat_key];
    }
    if (entites.length == 0) {
        return 0;
    }
    return round(total / entites.length);
}

//This function returns a map of the entities that I am processing.
//The format for the map can be found on the first line.
//It is meant to work on AdGroups and Keywords
function buildEntityMap(entity_type) {
    var map = {}; // { parent_id : [ { entity : entity, stats : entity_stats } ], ... }
    var iter = getIterator(entity_type);
    while (iter.hasNext()) {
        var entity = iter.next();
        var stats = entity.getStatsFor(DATE_RANGE);
        var stats_map = getStatsMap(stats);
        var parent_id = getParentId(entity_type, entity);
        if (map[parent_id]) {
            map[parent_id].push({ entity: entity, stats: stats_map });
        } else {
            map[parent_id] = [{ entity: entity, stats: stats_map }];
        }
    }
    return map;
}

//Given an entity type (adgroup or keyword) this will return the parent id
function getParentId(entity_type, entity) {
    switch (entity_type) {
        case 'adgroup':
            return entity.getCampaign().getId();
        case 'keyword':
            return entity.getAdGroup().getId();
        case 'ad':
            return entity.getAdGroup().getId();
    }
}

//Given an entity type this will return the iterator for that.
function getIterator(entity_type) {
    switch (entity_type) {
        case 'adgroup':
            return AdWordsApp.adGroups().forDateRange(DATE_RANGE)
                .withCondition("Impressions > 0")
                .withCondition("CampaignStatus = ENABLED")
                .withCondition("Status = ENABLED")
                .get();
        case 'keyword':
            return AdWordsApp.keywords().forDateRange(DATE_RANGE)
                .withCondition("Impressions > 0")
                .withCondition("AdGroupStatus = ENABLED")
                .withCondition("CampaignStatus = ENABLED")
                .withCondition("Status = ENABLED")
                .get();
        case 'ad':
            return AdWordsApp.ads().forDateRange(DATE_RANGE)
                .withCondition("Impressions > 0")
                .withCondition("AdGroupStatus = ENABLED")
                .withCondition("CampaignStatus = ENABLED")
                .withCondition("Status = ENABLED")
                .get();
    }
}

//This returns a map of all the stats for a given entity.
//You can comment out the things you don't really care about.
function getStatsMap(stats) {
    return { // You can comment these out as needed.
        cpc: stats.getAverageCpc(),
        // cpm : stats.getAverageCpm(),
        page_views: stats.getAveragePageviews(),
        // pos : stats.getAveragePosition(),
        time_on_site: stats.getAverageTimeOnSite(),
        bounce: stats.getBounceRate(),
        clicks: stats.getClicks(),
        cv: stats.getConversionRate(),
        conv: stats.getConversions(),
        cost: stats.getCost(),
        ctr: stats.getCtr(),
        imps: stats.getImpressions()
    };
}

// Helper Functions