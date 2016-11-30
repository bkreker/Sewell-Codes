
//-----------------------------------
// Delete Ads That Are Disapproved
// Created By: Russ Savage
// FreeAdWordsScripts.com
//-----------------------------------

var LABEL_NAME = 'Disapproved';
var REPORT_NAME = ['Pause','Disapproved'];
var pausedNum = 0;
var ATTACHMENT = [['Campaign','AdGroup','Ad','Headline','Url','Reason']];
function main() {
  createLabelIfNeeded(LABEL_NAME);
  pauseDisapproved();
  EmailReportResults(['joshd@sewelldirect.com'], REPORT_NAME, pausedNum + ' ads paused due to dissapproval.', ATTACHMENT);
  //pauseLowVolume();
}
function emailMessage(){
  return pausedNum + ' ads paused due to dissapproval.';
}
function emailAttachment(){
  return ATTACHMENT;
}
function pauseDisapproved(){
  // Let's start by getting all of the ad that are disapproved
  var ad_iter = AdWordsApp.ads()
  .withCondition("ApprovalStatus = DISAPPROVED")
  .withCondition("AdGroupStatus = ENABLED")
  .withCondition("CampaignStatus = ENABLED")
  .get(); 
  
  // Then we will go through each one
  while (ad_iter.hasNext()) {
    var ad = ad_iter.next();
    var camp = ad.getCampaign().getName();
    var adGrp = ad.getAdGroup().getName();
    var headline = ad.getHeadline();
    var url = ad.urls().getFinalUrl();
    var reasons = ad.getDisapprovalReasons();
    // now we delete the ad
    ATTACHMENT = ATTACHMENT.concat(['\n' + camp, adGrp, ad, headline, url,  reasons]);
    Logger.log("Pausing ad: "+  camp + ' ' + adGrp + ' ' + ad + ' '+ headline + ' '+ url + ' ' +'\nfor ' + reasons);
  
    ad.applyLabel(LABEL_NAME);
    ad.pause();
    pausedNum++;
  }
}

function pauseLowVolume(){
  var ad_iter = AdWordsApp.ads()
  .withCondition("ApprovalStatus = DISAPPROVED")
  .get();
  
    while (ad_iter.hasNext()) {
    var ad = ad_iter.next();
    // now we pause the ad
    Logger.log("Pausing ad: " + ad.getHeadline());
    ad.pause();
  }
}

function _getDateTime(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c),e=AM_PM(a),f={day:d,time:e};return f}catch(a){throw error("_getDateTime()",a)}}function AM_PM(a){try{var b=a.getHours()+1,c=a.getMinutes(),d=b>=12?"pm":"am";b%=12,b=b?b:12,c=c<10?"0"+c:c;var e=b+":"+c+" "+d;return e}catch(b){throw error("AM_PM(date: "+a+")",b)}}function CustomDateRange(a,b,c){try{null!==a&&void 0!==a||(a=91),null!==b&&void 0!==b||(b=0),void 0!==c&&""!==c&&null!==c||(c="YYYYMMdd");var d=_daysAgo(a),e=_daysAgo(b),f=_daysAgo(a,c).toString(),g=_daysAgo(b,c).toString(),h=[d,e],i=f+","+g,j={fromStr:f,toStr:g,fromObj:d,toObj:e,dateObj:h,string:i};return j}catch(d){throw error("CustomDateRange(fromDaysAgo: "+a+", tillDate: "+b+", format: "+c+")",d)}}function _daysAgo(a,b){try{var c=new Date;c.setDate(c.getDate()-a);var d;if(void 0!=b&&""!=b&&null!=b){var e=AdWordsApp.currentAccount().getTimeZone();d=Utilities.formatDate(c,e,b)}else d={day:c.getDate(),month:c.getMonth(),year:c.getYear()};return d}catch(c){throw error("_daysAgo(num: "+a+", format: "+b+")",c)}}function _today(a){try{var d,b=new Date,c=AdWordsApp.currentAccount().getTimeZone();return d=void 0!=a&&""!=a&&null!=a?Utilities.formatDate(b,c,a):{day:b.getDate(),month:b.getMonth(),year:b.getYear(),time:b.getTime()}}catch(b){throw error("_today(format: "+a+")",b)}}function _getDateString(){try{var a=new Date,b=AdWordsApp.currentAccount().getTimeZone(),c="MM-dd-yyyy",d=Utilities.formatDate(a,b,c);return d}catch(a){throw error("_getDateString()",a)}}function _todayIsMonday(){try{var a=36e5,b=new Date,c=new Date(b.getTime()+a),e=(c.getTime(),c.getDay());return Logger.log("today: "+c+"\nday: "+e),1===e}catch(a){throw error("todayIsMonday",a)}}function _rolling13Week(a){try{void 0!==a&&""!==a&&null!==a||(a="YYYYMMdd");var b=CustomDateRange(98,8,a),c=CustomDateRange(91,1,a),d=b.string+" - "+c.string,e={from:b,to:c,string:d};return e}catch(b){throw error("Rolling13Week(format: "+a+")",b)}}function formatKeyword(a){try{return a=a.replace(/[^a-zA-Z0-9 ]/g,"")}catch(b){throw error("formatKeyword(keyword: "+a+")",b)}}function round(a){try{var b=Math.pow(10,DECIMAL_PLACES);return Math.round(a*b)/b}catch(b){throw error("round(value: "+a+")",b)}}function getStandardDev(a,b,c){try{var d=0;for(var e in a)d+=Math.pow(a[e].stats[c]-b,2);return 0==Math.sqrt(a.length-1)?0:round(Math.sqrt(d)/Math.sqrt(a.length-1))}catch(d){throw error("getStandardDev(entites: "+a+", mean: "+b+", stat_key: "+c+")",d)}}function getMean(a,b){try{var c=0;for(var d in a)c+=a[d].stats[b];return 0==a.length?0:round(c/a.length)}catch(c){throw error("getMean(entites: "+a+", stat_key: "+b+")",c)}}function createLabelIfNeeded(a){try{AdWordsApp.labels().withCondition("Name = '"+a+"'").get().hasNext()||AdWordsApp.createLabel(a)}catch(b){throw error("createLabelIfNeeded(name: "+a+")",b)}}function sendResultsViaEmail(a,b){try{var i,c=a.match(/\n/g).length-1,d=_getDateTime().day,e="AdWords Alert: "+SCRIPT_NAME.join(" ")+" "+_titleCase(b)+"s Report - "+day,f="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",g=emailMessage(c)+f,h=SCRIPT_NAME.join("_")+d,j="";0!=c&&(AdWordsApp.getExecutionInfo().isPreview()?(i=EMAILS[0],j="Preview; No changes actually made.\n"):i=EMAILS.join(),MailApp.sendEmail({to:i,subject:e,body:j+g,attachments:[Utilities.newBlob(a,"text/csv",h+d+".csv")]}),Logger.log("Email sent to: "+i))}catch(c){throw error("sendResultsViaEmail(report: "+a+", level: "+b+")",c)}}function _titleCase(a){try{return a.replace(/(?:^|\s)\S/g,function(a){return a.toUpperCase()})}catch(b){throw error("_titleCase(str: "+a+")",b)}}function EmailResults(a){try{var g,b="AdWords Alert: "+a.join(" "),c="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.",d=emailMessage()+c,e=emailAttachment(),f=_getDateString()+"_"+a.join("_"),h="";AdWordsApp.getExecutionInfo().isPreview()?(g=EMAILS[0],h="Preview; No changes actually made.\n"):g=EMAILS.join(),""!=d&&MailApp.sendEmail({to:g,subject:b,body:d,attachments:[{fileName:f+".csv",mimeType:"text/csv",content:e}]}),Logger.log("Email sent to: "+g)}catch(b){throw error("EmailResults(ReportName: "+a+")",b)}}function EmailReportResults(a,b,c,d){try{var g,e="AdWords Alert: "+b.join(" "),f=_getDateString()+"_"+b.join("_"),h="";AdWordsApp.getExecutionInfo().isPreview()?(g=a[0],h="Preview; No changes actually made.\n"):g=a.join(),""!=c&&MailApp.sendEmail({to:g,subject:e,body:h+c+EMAIL_SIGNATURE,attachments:[{fileName:f+".csv",mimeType:"text/csv",content:d.join(",")}]}),Logger.log("Email sent to: "+g)}catch(c){print(d.join()),error("EmailReportResults(_emails: "+a.join()+", _reportName:"+b.join()+", _message, _attachment),\n"+c)}}function info(a){Logger.log(a)}function print(a){Logger.log(a)}function error(a,b){var c="ERROR in "+a+": "+b;return Logger.log(c),c}function warn(a){Logger.log("WARNING: "+a)}function isNumber(a){try{return a.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)||NaN===a}catch(b){throw error("isNumber(obj: "+a+")",b)}}function hasLabelAlready(a,b){try{return a.labels().withCondition("Name = '"+b+"'").get().hasNext()}catch(c){throw error("hasLabelAlready(entity: "+a+", label"+b+")",c)}}var EMAIL_SIGNATURE="\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.";