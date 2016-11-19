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
var SCRIPT_NAME = ['Anomaly','Tagger'];

function main() {
  // This will add labels to and send emails about adgroups, keywords and ads. Remove any if you like.
  var levels_to_tag = ['adgroup','keyword'/*,'ad'*/];
  for(var x in levels_to_tag) {
    var report = getContentRows(levels_to_tag[x]);
    var entity_map = buildEntityMap(levels_to_tag[x]);
    for(var parent_id in entity_map) {
      var child_list = entity_map[parent_id];
      var stats_list = Object.keys(child_list[0].stats);
      for(var i in stats_list) {
        var mean = getMean(child_list,stats_list[i]);
        var stand_dev = getStandardDev(child_list,mean,stats_list[i]);
        var label_name = stats_list[i];
        report += addLabelToAnomalies(child_list,mean,stand_dev,stats_list[i],label_name,levels_to_tag[x]);
      }
    }
    sendResultsViaEmail(report,levels_to_tag[x]);
  }
}

function emailMessage(rows){
	return "There are " + rows + " " + level + "s that have abnormal performance. See attachment for details.";
}

//Helper function to return a single row of the report formatted correctly
function toReportRow(entity,level,label_name) {
  var _row = [entity.getCampaign().getName()];
  
  _row.push( (level == 'adgroup') ? entity.getName() : entity.getAdGroup().getName() );
  
  if(level == 'keyword') {
    _row = _row.concat([entity.getText(),entity.getMatchType()]); 
  } else if(level == 'ad') {
    _row = _row.concat([entity.getHeadline(),entity.getDescription1(),entity.getDescription2(),entity.getDisplayUrl()]); 
  }
  _row.push(label_name);
  
  return '"' + _row.join('","') + '"\n';
}

//Helper function to return the column headings for the report
function getContentRows(level) {
  var ret_val = ['CampaignName','AdGroupName'];
  if(level == 'keyword') {
    ret_val = ret_val.concat(['KeywordText','MatchType']); 
  } else if(level == 'ad') {
    ret_val = ret_val.concat(['Headline','Description1','Description2','DisplayUrl']);
  }
  ret_val.push('LabelName');
  return '"' + ret_val.join('","') + '"\n';
}

//Function to add the labels to the entities based on the standard deviation and mean.
//It returns a csv formatted string for reporting
function addLabelToAnomalies(entites, mean, sd, stat_key, label, level) {
  var report = '';
  var label_name = label;
  try{
    for(var i in entites) {
      var entity = entites[i]['entity'];
      //print('addLabelToAnomalies: ' + entity);
      var stat = entites[i]['stats'][stat_key];	
      var rawDev =  stat - mean;
      
      var highLabel = setLabel(label, 'high');
      var lowLabel = setLabel(label, 'low');
      
      var deviation = Math.abs(rawDev);
      if(sd != 0 && deviation/sd >= STANDARD_DEVIATIONS && rawDev != 0){
        
        if(rawDev > 0){
          label_name = highLabel;
        }
        else if (rawDev < 0){
          label_name = lowLabel;
        }		
        if (shouldBeLabeled(label_name)){
          entity.applyLabel(label_name);
          report += toReportRow(entity,level,label_name);
          print(entity +' has ' + label_name + ': ' + round(stat) + ', deviation: ' + round(deviation)+ ', sd: '+ round(sd) + ', rawDev: '+ round(rawDev) +', mean: '+ round(mean));
        }
        
      }	
      else {
        removeLabels(entity, highLabel, lowLabel);
      }
    }	
  }
  catch(f){    
    Logger.log(f.message);
  }
  return report;
}

function shouldBeLabeled(label){  
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
  if (possibles.indexOf(label) >= 0){
    return true;
  }
  else{
    return false;
  }
}

function setLabel(lblName, type){
  var result = lblName;
  if (lblName.indexOf('low') < 0 && lblName.indexOf('high') < 0){
    result = type +'_' + lblName;
  }
  createLabelIfNeeded(result);
  return result;
}

function hasLabel(entity, label){
  var possibles = [label, 'high_'+label, 'low_'+label];
  var lblStack = entity.labels()
  .withCondition("LabelNames CONTAINS_ANY "+ possibles +"")
  .get();
  
  // print(possibles.join());
  if(lblStack.hasNext()){
    
    print('lblStack.hasNext === true ');
    return true;
  }
  else {
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
function removeLabels(entity, highLabel, lowLabel){    
  
  try{
    entity.removeLabel(highLabel);
  }
  catch(h){
    print('removeLabels ' + h.message);
  }
  try{
    entity.removeLabel(lowLabel);
  }
  catch(l){
    print('removeLabels: ' + l.message);   
  } 
}

//This function returns the standard deviation for a set of entities
//The stat key determines which stat to calculate it for
function getStandardDev(entites,mean,stat_key) {
  var total = 0;
  for(var i in entites) {
    total += Math.pow(entites[i]['stats'][stat_key] - mean,2);
  }
  if(Math.sqrt(entites.length-1) == 0) {
    return 0;
  }
  return round(Math.sqrt(total)/Math.sqrt(entites.length-1));
}

//Returns the mean (average) for the set of entities
//Again, stat key determines which stat to calculate this for
function getMean(entites,stat_key) {
  var total = 0;
  for(var i in entites) {
    total += entites[i]['stats'][stat_key];
  }
  if(entites.length == 0) {
    return 0;
  }
  return round(total/entites.length);
}

//This function returns a map of the entities that I am processing.
//The format for the map can be found on the first line.
//It is meant to work on AdGroups and Keywords
function buildEntityMap(entity_type) {
  var map = {}; // { parent_id : [ { entity : entity, stats : entity_stats } ], ... }
  var iter = getIterator(entity_type);
  while(iter.hasNext()) {
    var entity = iter.next();
    var stats = entity.getStatsFor(DATE_RANGE);
    var stats_map = getStatsMap(stats);
    var parent_id = getParentId(entity_type,entity);
    if(map[parent_id]) { 
      map[parent_id].push({entity : entity, stats : stats_map});
    } else {
      map[parent_id] = [{entity : entity, stats : stats_map}];
    }
  }
  return map;
}

//Given an entity type (adgroup or keyword) this will return the parent id
function getParentId(entity_type,entity) {
  switch(entity_type) {
    case 'adgroup' :
      return entity.getCampaign().getId();
    case 'keyword':
      return entity.getAdGroup().getId();
    case 'ad':
      return entity.getAdGroup().getId();
  }
}

//Given an entity type this will return the iterator for that.
function getIterator(entity_type) {
  switch(entity_type) {
    case 'adgroup' :
      return AdWordsApp.adGroups().forDateRange(DATE_RANGE)
      .withCondition("Impressions > 0")
      .withCondition("CampaignStatus = ENABLED")
      .withCondition("Status = ENABLED")
      .get();
    case 'keyword' :
      return AdWordsApp.keywords().forDateRange(DATE_RANGE)
      .withCondition("Impressions > 0")      
      .withCondition("AdGroupStatus = ENABLED")
      .withCondition("CampaignStatus = ENABLED")
      .withCondition("Status = ENABLED")
      .get();
    case 'ad' :
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
    cpc : stats.getAverageCpc(),
    // cpm : stats.getAverageCpm(),
    page_views : stats.getAveragePageviews(),
    // pos : stats.getAveragePosition(),
    time_on_site : stats.getAverageTimeOnSite(),
    bounce : stats.getBounceRate(),
    clicks : stats.getClicks(),
    cv : stats.getConversionRate(),
    conv : stats.getConversions(),
    cost : stats.getCost(),
    ctr : stats.getCtr(),
    imps : stats.getImpressions()
  };
}



function _getDateTime(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c),e=AM_PM(a),f={day:d,time:e};return f}catch(a){throw error("_getDateTime()",a)}}function AM_PM(a){try{var b=a.getHours()+1,c=a.getMinutes(),d=b>=12?"pm":"am";b%=12,b=b?b:12,c=c<10?"0"+c:c;var e=b+":"+c+" "+d;return e}catch(b){throw error("AM_PM(date: "+a+")",b)}}function CustomDateRange(a,b,c){try{null!==a&&void 0!==a||(a=91),null!==b&&void 0!==b||(b=0),void 0!==c&&""!==c&&null!==c||(c="YYYYMMdd");var d=_daysAgo(a),e=_daysAgo(b),f=_daysAgo(a,c).toString(),g=_daysAgo(b,c).toString(),h=[d,e],i=f+","+g,j={fromStr:f,toStr:g,fromObj:d,toObj:e,dateObj:h,string:i};return j}catch(d){throw error("CustomDateRange(fromDaysAgo: "+a+", tillDate: "+b+", format: "+c+")",d)}}function _daysAgo(a,b){try{var c=new Date;c.setDate(c.getDate()-a);var d;if(void 0!=b&&""!=b&&null!=b){var e=AdWordsApp.currentAccount().getTimeZone();d=Utilities.formatDate(c,e,b)}else d={day:c.getDate(),month:c.getMonth(),year:c.getYear()};return d}catch(c){throw error("_daysAgo(num: "+a+", format: "+b+")",c)}}function _today(a){try{var d,b=new Date,c=AdWordsApp.currentAccount().getTimeZone();return d=void 0!=a&&""!=a&&null!=a?Utilities.formatDate(b,c,a):{day:b.getDate(),month:b.getMonth(),year:b.getYear(),time:b.getTime()}}catch(b){throw error("_today(format: "+a+")",b)}}function _getDateString(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c);return d}catch(a){throw error("_getDateString()",a)}}function _todayIsMonday(){try{var a=36e5,b=new Date,c=new Date(b.getTime()+a),e=(c.getTime(),c.getDay());return Logger.log("today: "+c+"\nday: "+e),1===e}catch(a){throw error("todayIsMonday",a)}}function _rolling13Week(a){try{void 0!==a&&""!==a&&null!==a||(a="YYYYMMdd");var b=CustomDateRange(98,8,a),c=CustomDateRange(91,1,a),d=b.string+" - "+c.string,e={from:b,to:c,string:d};return e}catch(b){throw error("Rolling13Week(format: "+a+")",b)}}function formatKeyword(a){try{return a=a.replace(/[^a-zA-Z0-9 ]/g,"")}catch(b){throw error("formatKeyword(keyword: "+a+")",b)}}function round(a){try{var b=Math.pow(10,DECIMAL_PLACES);return Math.round(a*b)/b}catch(b){throw error("round(value: "+a+")",b)}}function getStandardDev(a,b,c){try{var d=0;for(var e in a)d+=Math.pow(a[e].stats[c]-b,2);return 0==Math.sqrt(a.length-1)?0:round(Math.sqrt(d)/Math.sqrt(a.length-1))}catch(d){throw error("getStandardDev(entites: "+a+", mean: "+b+", stat_key: "+c+")",d)}}function getMean(a,b){try{var c=0;for(var d in a)c+=a[d].stats[b];return 0==a.length?0:round(c/a.length)}catch(c){throw error("getMean(entites: "+a+", stat_key: "+b+")",c)}}function createLabelIfNeeded(a){try{AdWordsApp.labels().withCondition("Name = '"+a+"'").get().hasNext()||AdWordsApp.createLabel(a)}catch(b){throw error("createLabelIfNeeded(name: "+a+")",b)}}function sendResultsViaEmail(a,b){try{var i,c=a.match(/\n/g).length-1,d=_getDateTime().day,e="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_titleCase(b)+"s Report - "+day,f="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",g=emailMessage(c)+f,h=SCRIPT_NAME.join("_")+d,j="";0!=c&&(AdWordsApp.getExecutionInfo().isPreview()?(i=EMAILS[0],j="Preview; No changes actually made.\n"):i=EMAILS.join(),MailApp.sendEmail({to:i,subject:e,body:j+g,attachments:[Utilities.newBlob(a,"text/csv",h+d+".csv")]}),Logger.log("Email sent to: "+i))}catch(c){throw error("sendResultsViaEmail(report: "+a+", level: "+b+")",c)}}function _titleCase(a){try{return a.replace(/(?:^|\s)\S/g,function(a){return a.toUpperCase()})}catch(b){throw error("_titleCase(str: "+a+")",b)}}function EmailResults(a){try{var g,b="AdWords Alert: "+a.join(" "),c="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",d=emailMessage()+c,e=emailAttachment(),f=_getDateString()+"_"+a.join("_"),h="";AdWordsApp.getExecutionInfo().isPreview()?(g=EMAILS[0],h="Preview; No changes actually made.\n"):g=EMAILS.join(),""!=d&&MailApp.sendEmail({to:g,subject:b,body:d,attachments:[{fileName:f+".csv",mimeType:"text/csv",content:e}]}),Logger.log("Email sent to: "+g)}catch(b){throw error("EmailResults(ReportName: "+a+")",b)}}function info(a){Logger.log(a)}function print(a){Logger.log(a)}function error(a,b){var c="ERROR in "+a+": "+b;return Logger.log(c),c}function warn(a){Logger.log("WARNING: "+a)}function isNumber(a){try{return a.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===a}catch(b){throw error("isNumber(obj: "+a+")",b)}}function hasLabelAlready(a,b){try{return a.labels().withCondition("Name = '"+b+"'").get().hasNext()}catch(c){throw error("hasLabelAlready(entity: "+a+", label"+b+")",c)}}
