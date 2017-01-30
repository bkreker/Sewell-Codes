// Helper Functions
// Public variables
var PreviewMsg = '';
var EMAIL_SIGNATURE = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
var IS_PREVIEW = AdWordsApp.getExecutionInfo().isPreview();


//Helper function to format todays date and time
function _getDateTime() {
    try {
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
    } catch (e) {
        throw error('_getDateTime()', e);
    }
}

// Helper function to get the time in am/pm
function AM_PM(date) {
    try {
        var hours = date.getHours() + 1;
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'pm' : 'am';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + ':' + minutes + ' ' + ampm;
        return strTime;
    } catch (e) {
        throw error('AM_PM(date: ' + date + ')', e);
    }
}

// Helper function to get custom date range, defaults to one quarter (13 weeks) ago is 91 days and 'YYYYMMdd' date format
function CustomDateRange(fromDaysAgo, tillDate, format) {
    try {
        //print('CustomDateRange(fromDaysAgo: '+fromDaysAgo+', tillDate: '+tillDate+ ', format: '+format+')');

        if (fromDaysAgo === null || fromDaysAgo === undefined) {
            fromDaysAgo = 91;
        }
        if (tillDate === null || tillDate === undefined) {
            tillDate = 0;
        }
        if (format === undefined || format === '' || format === null) {
            format = 'YYYYMMdd';
        }
        var from = _daysAgo(fromDaysAgo);
        var to = _daysAgo(tillDate);
        var fromS = _daysAgo(fromDaysAgo, format).toString();
        var toS = _daysAgo(tillDate, format).toString();
        var dateArr = [from, to];
        var str = fromS + ',' + toS;

        var result = {
            fromStr: fromS,
            toStr: toS,
            fromObj: from,
            toObj: to,
            dateObj: [from, to],
            string: str
        }
        return result;
    } catch (e) {
        throw error('CustomDateRange(fromDaysAgo: ' + fromDaysAgo + ', tillDate: ' + tillDate + ', format: ' + format + ')', e);
    }
}

// Helper function to get a date a certain number of days ago (one quarter (13 weeks) ago is 91 days)
function _daysAgo(num, format) {
    try {
        var newDate = new Date();
        newDate.setDate(newDate.getDate() - num);
        var date;
        if (format != undefined && format != '' && format != null) {
            var timeZone = AdWordsApp.currentAccount().getTimeZone();
            date = Utilities.formatDate(newDate, timeZone, format);
        } else {
            date = {
                year: newDate.getYear(),
                month: newDate.getMonth(),
                day: newDate.getDate()
            };
        }
        return date;
    } catch (e) {
        throw error('_daysAgo(num: ' + num + ', format: ' + format + ')', e);
    }
}

//Helper function to format todays date
function _today(format) {
    try {
        var newDate = new Date();
        var timeZone = AdWordsApp.currentAccount().getTimeZone();
        var today;
        if (format != undefined && format != '' && format != null) {
            today = Utilities.formatDate(newDate, timeZone, format);
        } else {
            today = {
                day: newDate.getDate(),
                month: newDate.getMonth(),
                year: newDate.getYear(),
                time: newDate.getTime()
            };
        }
        return today;
    } catch (e) {
        throw error('_today(format: ' + format + ')', e);
    }
}

function _getDateString() {
    try {
        var today = new Date();
        var timeZone = AdWordsApp.currentAccount().getTimeZone();
        var dayFormat = "MM-dd-yyyy";
        var day = Utilities.formatDate(today, timeZone, dayFormat);

        return day;
    } catch (e) {
        throw error('_getDateString()', e);
    }
}

// Function to get date and return true if it's monday
// Days: 0: sun, 1: mon, 2: tue, 3: wed, 4: thu, 5: fri, 6: sat
function _todayIsMonday() {
    try {
        var DATE_OFFSET = 3600000;
        var date = new Date();
        var today = new Date(date.getTime() + DATE_OFFSET);
        var time = today.getTime();
        var day = today.getDay();
        Logger.log('today: ' + today + '\nday: ' + day);
        return (day === 1);
    } catch (e) {
        throw error('todayIsMonday', e);
    }
}


function _rolling13Week(format) {
    try {
        if (format === undefined || format === '' || format === null) {
            format = 'YYYYMMdd';
        }
        var p = CustomDateRange(98, 8, format);
        var n = CustomDateRange(91, 1, format);
        var str = p.string + ' - ' + n.string;
        var result = {
            from: p,
            to: n,
            string: str
        }
        return result;
    } catch (e) {
        throw error('Rolling13Week(format: ' + format + ')', e);
    }
}

function formatKeyword(keyword) {
    try {
        keyword = keyword.replace(/[^a-zA-Z0-9 ]/g, '');
        return keyword;
    } catch (e) {
        throw error('formatKeyword(keyword: ' + keyword + ')', e);
    }
}

// A helper function to make rounding a little easier
function round(value) {
    try {
        var decimals = Math.pow(10, DECIMAL_PLACES);
        return Math.round(value * decimals) / decimals;
    } catch (e) {
        throw error('round(value: ' + value + ')', e);
    }
}

//This function returns the standard deviation for a set of entities
//The stat key determines which stat to calculate it for
function getStandardDev(entites, mean, stat_key) {
    try {
        var total = 0;
        for (var i in entites) {
            total += Math.pow(entites[i]['stats'][stat_key] - mean, 2);
        }
        if (Math.sqrt(entites.length - 1) == 0) {
            return 0;
        }
        return round(Math.sqrt(total) / Math.sqrt(entites.length - 1));
    } catch (e) {
        throw error('getStandardDev(entites: ' + entites + ', mean: ' + mean + ', stat_key: ' + stat_key + ')', e);
    }
}

//Returns the mean (average) for the set of entities
//Again, stat key determines which stat to calculate this for
function getMean(entites, stat_key) {
    try {
        var total = 0;
        for (var i in entites) {
            total += entites[i]['stats'][stat_key];
        }
        if (entites.length == 0) {
            return 0;
        }
        return round(total / entites.length);
    } catch (e) {
        throw error('getMean(entites: ' + entites + ', stat_key: ' + stat_key + ')', e);
    }
}

//This is a helper function to create the label if it does not already exist
function createLabelIfNeeded(name) {
    try {
        if (!AdWordsApp.labels().withCondition("Name = '" + name + "'").get().hasNext()) {
            AdWordsApp.createLabel(name);
        }
    } catch (e) {
        throw error('createLabelIfNeeded(name: ' + name + ')', e);
    }
}

function EmailErrorReport(reportName, emails, isPreview, ex, completedReport) {
    var _subject = 'AdWords Alert: Error in ' + reportName + ', script ' + (completedReport ? 'did execute correctly ' : 'did not execute ') + ' correctly.';
    var _message = "Error on line " + ex.lineNumber + ":\n" + ex.message + EMAIL_SIGNATURE;
    var _attachment = emailAttachment();
    var _fileName = _getDateString() + '_' + reportName;
    var _to = isPreview ? emails[0] : emails.join();
    PreviewMsg = isPreview ? 'Preview; No changes actually made.\n' : '';


    if (_message != '') {
        MailApp.sendEmail({
            to: _to,
            subject: _subject,
            body: PreviewMsg + _message,
            attachments: [{
                fileName: _fileName + '.csv',
                mimeType: 'text/csv',
                content: _attachment
            }]
        });
    }

    print('Email sent to: ' + _to);

}

//Takes a report and the level of reporting and sends and email
//with the report as an attachment.
function sendResultsViaEmail(report, level) {
    try {
        var rows = report.match(/\n/g).length - 1;
        var date = _getDateTime().day;

        var subject = 'AdWords Alert: ' + SCRIPT_NAME.join(' ') + ' ' + _titleCase(level) + 's Report - ' + day;

        var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
        var message = emailMessage(rows) + signature;
        var file_name = SCRIPT_NAME.join('_') + date;
        var To;
        var isPreview = '';

        if (rows != 0) {
            // If it's a preview, only send it to me, and mention that no changes were made
            if (AdWordsApp.getExecutionInfo().isPreview()) {
                To = EMAILS[0]
                isPreview = 'Preview; No changes actually made.\n';
            } else {
                To = EMAILS.join();
            }

            MailApp.sendEmail({
                to: To,
                subject: subject,
                body: isPreview + message,
                attachments: [Utilities.newBlob(report, 'text/csv', file_name + date + '.csv')]
            });

            Logger.log('Email sent to: ' + To);

        }
    } catch (e) {
        throw error('sendResultsViaEmail(report: ' + report + ', level: ' + level + ')', e);
    }
}
//Helper function to capitalize the first letter of a string.
function _titleCase(str) {
    try {
        return str.replace(/(?:^|\s)\S/g, function(a) {
            return a.toUpperCase();
        });
    } catch (e) {
        throw error('_titleCase(str: ' + str + ')', e);
    }
}

function EmailResults(ReportName) {
    try {
        var _emails = EMAILS;
        var Subject = 'AdWords Alert: ' + ReportName.join(' ');
        var Message = emailMessage() + EMAIL_SIGNATURE;
        var Attachment = emailAttachment();
        var file_name = _getDateString() + '_' + ReportName.join('_');
        var _to;
        var previewMsg = '';

        _to = IS_PREVIEW ? _emails[0] : _emails.join();
        PreviewMsg = IS_PREVIEW ? 'Preview; No changes actually made.\n' : '';


        if (Message != '') {
            MailApp.sendEmail({
                to: _to,
                subject: Subject,
                body: previewMsg + Message,
                attachments: [{
                    fileName: file_name + '.csv',
                    mimeType: 'text/csv',
                    content: Attachment
                }]
            });

        }

        print('Email sent to: ' + _to);
    } catch (e) {
        throw error('EmailResults(ReportName: ' + ReportName.join(' ') + ')', e);
    }
}

function EmailReportResults(_emails, _reportName, _message, _attachment) {
    try {
        var Subject = 'AdWords Alert: ' + _reportName.join(' ');

        var file_name = _getDateString() + '_' + _reportName.join('_');
        var _to;

        _to = IS_PREVIEW ? _emails[0] : _emails.join();
        PreviewMsg = IS_PREVIEW ? 'Preview; No changes actually made.\n' : '';


        if (_message != '') {
            MailApp.sendEmail({
                to: _to,
                subject: Subject,
                body: PreviewMsg + _message + EMAIL_SIGNATURE,
                attachments: [{
                    fileName: file_name + '.csv',
                    mimeType: 'text/csv',
                    content: _attachment.join(',')
                }]
            });

        }

        Logger.log('Email sent to: ' + To);
    } catch (e) {
        print(_attachment.join());
        error('EmailReportResults(_emails: ' + _emails.join() + ', _reportName:' + _reportName.join() + ', _message, _attachment),\n' + e);
    }
}

function info(msg) {
    Logger.log(msg);
}

function print(msg) {
    Logger.log(msg);
}

function error(funcName, e) {
    var warning = e.name + ' in ' + funcName + ' at line ' + e.lineNumber + ': ' + e.message;
    Logger.log(warning);
    return warning;
}

function warn(msg) {
    Logger.log('WARNING: ' + msg);
}

// Returns bool representing if obj is a number
function isNumber(obj) {
    try {
        return ((obj.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)) || (obj === NaN));
    } catch (e) {
        throw error('isNumber(obj: ' + obj + ')', e);
    }
}

// returns bool representing if an entity has a given keyword
function hasLabelAlready(entity, label) {
    try {
        return (entity.labels().withCondition("Name = '" + label + "'").get().hasNext());
    } catch (e) {
        throw error('hasLabelAlready(entity: ' + entity + ', label' + label + ')', e);
    }
}