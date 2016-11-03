var SCRIPT_NAME = ['Auto','Maintenance'];
var EMAILS = ['joshd@sewelldirect.com'];

function emailResults() {
  var message  = emailMessage();
  var subject =  'AdWords Alert: ' +SCRIPT_NAME.join(' ');
  var message = emailMessage();
  var attachment = emailAttachment();
  var file_name = _getDateString() + '_' + SCRIPT_NAME.join('_');
  var To;
  var isPreview = '';
  
  // If it's a preview, only send it to me, and mention that no changes were made
  if(AdWordsApp.getExecutionInfo().isPreview()){ 
    To = EMAILS[0] 
    isPreview = 'Preview; No changes actually made\n';
  }
  else{
    To = EMAILS.join();
  }
  
  if (message != '') {
    MailApp.sendEmail( {
      to: To,
      subject: subject,
      body: isPreview + message,
      attachments:[{fileName: file_name+ '.csv', mimeType: 'text/csv', content: file_name + '\n' + attachment}]
    });
    info('Email sent to: '+ To);
  }
}

function emailMessage(){
  var message = '';
  if(pausedNum !=0){
    message += pausedNum + ' ads auto-paused due to lack of stock. ';
  }
  return message;
  
}

function emailAttachment(){
  var attachment = '';
  
  return attachment;
}

//Helper function to format todays date
function _getDateString() {
  var today = new Date();
  var timeZone = AdWordsApp.currentAccount().getTimeZone();
  var format = "MM-dd-yyyy";
  var date = Utilities.formatDate(today, timeZone , format);
  return date;  
}