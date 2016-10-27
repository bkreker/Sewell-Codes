var Helpers = {
		//Helper function to format todays date and time
		_getDateTime: function() {
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
		},

		// Helper function to get the time in am/pm
		AM_PM: function(date) {
				var hours = date.getHours();
				var minutes = date.getMinutes();
				var ampm = hours >= 12 ? 'pm' : 'am';
				hours = hours % 12;
				hours = hours ? hours : 12; // the hour '0' should be '12'
				minutes = minutes < 10 ? '0' + minutes : minutes;
				var strTime = hours + ':' + minutes + ' ' + ampm;
				return strTime;
		},

		// Helper function to get custom date range, defaults to one quarter (13 weeks) ago is 91 days and 'YYYYMMdd' date format
		CustomDateRange: function(days, format) {
				if (days === null || days === undefined) {
						days = 91;
				}
				if (format === undefined || format === '' || format === null) {
						format = 'YYYYMMdd';
				}
				var fromS = _daysAgo(days, format).toString();
				var toS = _today(format).toString();
				var to = _today();
				var from = _daysAgo(days);
				var result = {
						fromStr: fromS,
						toStr: toS,
						fromObj: from,
						toObj: to,
						string: function() {
								return fromS + ',' + toS;
						}
				}
				return result;
		},
		// Helper function to get a date a certain number of days ago (one quarter (13 weeks) ago is 91 days)
		_daysAgo: function(num, format) {
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
		},
		//Helper function to format todays date
		_today: function(format) {
				var newDate = new Date();
				var timeZone = AdWordsApp.currentAccount().getTimeZone();
				var day;
				if (format != undefined && format != '' && format != null) {
						day = Utilities.formatDate(newDate, timeZone, format);
				} else {
						day = {
								day: newDate.getDate(),
								month: newDate.getMonth(),
								year: newDate.getYear()
						};
				}
				return day;
		},
		_getDateString: function() {
						var today = new Date();
						var timeZone = AdWordsApp.currentAccount().getTimeZone();
						var dayFormat = "MM-dd-yyyy";
						var day = Utilities.formatDate(today, timeZone, dayFormat);

						return day;
				}
				// Function to get date and return true if it's monday
				// Days: 0: sun, 1: mon, 2: tue, 3: wed, 4: thu, 5: fri, 6: sat
				,
		todayIsMonday: function() {
				var DATE_OFFSET = 3600000;
				var date = new Date();
				var today = new Date(date.getTime() + DATE_OFFSET);
				var time = today.getTime();
				var day = today.getDay();
				Logger.log('today: ' + today + '\nday: ' + day);
				return (day === 1);
		}

		,
		Rolling13Week: function() {
				var format = 'MM/dd/YYYY';
				var p = _daysAgo(98, format) + ' - ' + _daysAgo(7, format);
				var n = _daysAgo(91, format) + ' - ' + _today(format)
				var result = {
						from: p,
						to: n,
						string: function() {
								return this.p + ' - ' + this.n;
						}
				}
				return result;
		},
		Rolling13Week: function(format) {
				var p = _daysAgo(98, format) + ' - ' + _daysAgo(7, format);
				var n = _daysAgo(91, format) + ' - ' + _today(format)
				var result = {
						from: p,
						to: n,
						string: function() {
								return this.p + ' - ' + this.n;
						}
				}
				return result;
		},
		formatKeyword: function(keyword) {
				keyword = keyword.replace(/[^a-zA-Z0-9 ]/g, '');
				return keyword;
		},
		// A helper function to make rounding a little easier
		round: function(value) {
				var decimals = Math.pow(10, DECIMAL_PLACES);
				return Math.round(value * decimals) / decimals;
		},
		//This function returns the standard deviation for a set of entities
		//The stat key determines which stat to calculate it for
		getStandardDev: function(entites, mean, stat_key) {
				var total = 0;
				for (var i in entites) {
						total += Math.pow(entites[i]['stats'][stat_key] - mean, 2);
				}
				if (Math.sqrt(entites.length - 1) == 0) {
						return 0;
				}
				return round(Math.sqrt(total) / Math.sqrt(entites.length - 1));
		},
		//Returns the mean (average) for the set of entities
		//Again, stat key determines which stat to calculate this for
		getMean: function(entites, stat_key) {
				var total = 0;
				for (var i in entites) {
						total += entites[i]['stats'][stat_key];
				}
				if (entites.length == 0) {
						return 0;
				}
				return round(total / entites.length);
		},
		//This is a helper function to create the label if it does not already exist
		createLabelIfNeeded: function(name) {
				if (!AdWordsApp.labels().withCondition("Name = '" + name + "'").get().hasNext()) {
						AdWordsApp.createLabel(name);
				}
		},
		//Takes a report and the level of reporting and sends and email
		//with the report as an attachment.
		sendResultsViaEmail: function(report, level) {
				var rows = report.match(/\n/g).length - 1;
				var date = _getDateTime().day;

				var subject = 'AdWords Alert: ' + SCRIPT_NAME.join(' ') + ' ' + _initCap(level) + 's Report - ' + day;

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
		},
		EmailResults: function() {
				var Subject = 'AdWords Alert: ' + REPORT_NAME.join(' ');
				var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
				var Message = emailMessage() + signature;
				var Attachment = emailAttachment();
				var file_name = _getDateString() + '_' + REPORT_NAME.join('_');
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
		},
		EmailResults: function(ReportName) {
				var Subject = 'AdWords Alert: ' + ReportName.join(' ');
				var signature = '\n\nThis report was created by an automatic script by Josh DeGraw. If there are any errors or questions about this report, please inform me as soon as possible.';
				var Message = emailMessage() + signature;
				var Attachment = emailAttachment();
				var file_name = _getDateString() + '_' + ReportName.join('_');
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
		},
		info: function(msg) {
				Logger.log(msg);
		},
		print: function(msg) {
				Logger.log(msg);
		},
		warn: function(msg) {
				Logger.log('WARNING: ' + msg);
		},
		// Returns bool representing if obj is a number
		isNumber: function(obj) {
				return ((obj.toString().match(/(\.*([0-9])*\,*[0-9]\.*)/g)) || (obj === NaN));
		},
		// returns bool representing if an entity has a given keyword
		hasLabelAlready: function(entity, label) {
				return (entity.labels().withCondition("Name = '" + label + "'").get().hasNext());
		}
}
//Minified:
