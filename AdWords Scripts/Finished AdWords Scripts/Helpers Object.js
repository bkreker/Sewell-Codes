// Regex to switch from function list to member functions: (function) (\w*)(\(\w*(, \w*)*\))
// replace with ,\2: \1\3
var _ = {
        //Helper function to format todays date and time
        getDateTime: function() {
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
                throw error('getDateTime()', e);
            }
        }

        // Helper function to get the time in am/pm
        ,
        AM_PM: function(date) {
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
        ,
        CustomDateRange: function(fromDaysAgo, tillDate, format) {
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
                var from = daysAgo(fromDaysAgo);
                var to = daysAgo(tillDate);
                var fromS = daysAgo(fromDaysAgo, format).toString();
                var toS = daysAgo(tillDate, format).toString();
                var dateArr = [from, to];
                var str = fromS + ',' + toS;

                var result = {
                    fromStr: fromS,
                    toStr: toS,
                    fromObj: from,
                    toObj: to,
                    dateObj: dateArr,
                    string: str
                }
                return result;
            } catch (e) {
                throw error('CustomDateRange(fromDaysAgo: ' + fromDaysAgo + ', tillDate: ' + tillDate + ', format: ' + format + ')', e);
            }
        }

        // Helper function to get a date a certain number of days ago (one quarter (13 weeks) ago is 91 days)
        ,
        daysAgo: function(num, format) {
            try {
                var newDate = new Date();
                newDate.setDate(newDate.getDate() - num);
                var day;
                if (format != undefined && format != '' && format != null) {
                    var timeZone = AdWordsApp.currentAccount().getTimeZone();
                    day = Utilities.formatDate(newDate, timeZone, format);
                } else {
                    day = {
                        day: newDate.getDate(),
                        month: newDate.getMonth(),
                        year: newDate.getYear()
                    };
                }
                return day;
            } catch (e) {
                throw error('daysAgo(num: ' + num + ', format: ' + format + ')', e);
            }
        }

        //Helper function to format todays date
        ,
        today: function(format) {
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
                throw error('today(format: ' + format + ')', e);
            }
        }

        ,
        getDateString: function() {
            try {
                var today = new Date();
                var timeZone = AdWordsApp.currentAccount().getTimeZone();
                var dayFormat = "MM-dd-yyyy";
                var day = Utilities.formatDate(today, timeZone, dayFormat);

                return day;
            } catch (e) {
                throw error('getDateString()', e);
            }
        }

        // Function to get date and return true if it's monday
        // Days: 0: sun, 1: mon, 2: tue, 3: wed, 4: thu, 5: fri, 6: sat
        ,
        todayIsMonday: function() {
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


        ,
        rolling13Week: function(format) {
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

        ,
        formatKeyword: function(keyword) {
            try {
                keyword = keyword.replace(/[^a-zA-Z0-9 ]/g, '');
                return keyword;
            } catch (e) {
                throw error('formatKeyword(keyword: ' + keyword + ')', e);
            }
        }

        // A helper function to make rounding a little easier
        ,
        round: function(value) {
            try {
                var decimals = Math.pow(10, DECIMAL_PLACES);
                return Math.round(value * decimals) / decimals;
            } catch (e) {
                throw error('round(value: ' + value + ')', e);
            }
        }

        //This function returns the standard deviation for a set of entities
        //The stat key determines which stat to calculate it for
        ,
        getStandardDev: function(entites, mean, stat_key) {
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
        ,
        getMean: function(entites, stat_key) {
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
        ,
        createLabelIfNeeded: function(name) {
            try {
                if (!AdWordsApp.labels().withCondition("Name = '" + name + "'").get().hasNext()) {
                    AdWordsApp.createLabel(name);
                }
            } catch (e) {
                throw error('createLabelIfNeeded(name: ' + name + ')', e);
            }
        }

        //Takes a report and the level of reporting and sends and email
        //with the report as an attachment.
        ,
        sendResultsViaEmail: function(report, level) {
                try {
                    var rows = report.match(/\n/g).length - 1;
                    var date = getDateTime().day;

                    var subject = 'AdWords Alert: ' + SCRIPT_NAME.join(' ') + ' ' + titleCase(level) + 's Report - ' + day;

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
            ,
        titleCase: function(str) {
            try {
                return str.replace(/(?:^|\s)\S/g, function(a) {
                    return a.toUpperCase();
                });
            } catch (e) {
                throw error('_titleCase(str: ' + str + ')', e);
            }
        },
        EmailResults: function(ReportName) {
            try {
                var Subject = 'AdWords Alert: ' + ReportName.join(' ');
                var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
                var Message = emailMessage() + signature;
                var Attachment = emailAttachment();
                var file_name = getDateString() + '_' + ReportName.join('_');
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
                        body: Message,
                        attachments: [{
                            fileName: file_name + '.csv',
                            mimeType: 'text/csv',
                            content: Attachment
                        }]
                    });

                }

                Logger.log('Email sent to: ' + To);
            } catch (e) {
                throw error('EmailResults(ReportName: ' + ReportName + ')', e);
            }
        },

        info: function(msg) {
            Logger.log(msg);
        }

        ,
        print: function(msg) {
            Logger.log(msg);
        }

        ,
        error: function(funcName, msg) {
            var warning = 'ERROR in ' + funcName + ': ' + msg;
            Logger.log(warning);
            return warning;
        }

        ,
        warn: function(msg) {
            Logger.log('WARNING: ' + msg);
        }

        // Returns bool representing if obj is a number
        ,
        isNumber: function(obj) {
            try {
                return ((obj.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)) || (obj === NaN));
            } catch (e) {
                throw error('isNumber(obj: ' + obj + ')', e);
            }
        }

        // returns bool representing if an entity has a given keyword
        ,
        hasLabelAlready: function(entity, label) {
            try {
                return (entity.labels().withCondition("Name = '" + label + "'").get().hasNext());
            } catch (e) {
                throw error('hasLabelAlready(entity: ' + entity + ', label' + label + ')', e);
            }
        }
    }
    //Minified: