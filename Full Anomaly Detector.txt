// Copyright 2015, Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

/**
 * @name Account Anomaly Detector
 *
 * @overview The Account Anomaly Detector alerts the advertiser whenever an
 *     advertiser account is suddenly behaving too differently from what's
 *     historically observed. See
 *     https://developers.google.com/adwords/scripts/docs/solutions/account-anomaly-detector
 *     for more details.
 *
 * @author AdWords Scripts Team [adwords-scripts@googlegroups.com]
 *
 * @version 1.0
 * @changelog
 * - version 1.0
 *   - Released initial version.
 */

 // URL of MaxCPAs
var SPREADSHEET_URL = 'https://docs.google.com/spreadsheets/d/1oQGfRa2YjB1SeJF5-ZI_H2ewe97eQTtPIUdk0c1tr0I/edit?usp=sharing';

var DAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday',
            'Saturday'];

/**
 * Configuration to be used for running reports.
 */
var REPORTING_OPTIONS = {
  // Comment out the following line to default to the latest reporting version.
  apiVersion: 'v201601'
};

function main() {
  Logger.log('Using spreadsheet - %s.', SPREADSHEET_URL);
  var spreadsheet = SpreadsheetApp.openByUrl(SPREADSHEET_URL);

  spreadsheet.getRangeByName('date').setValue(new Date());
  spreadsheet.getRangeByName('account_id').setValue(
      AdWordsApp.currentAccount().getCustomerId());
  var impressionsThreshold = parseField(spreadsheet.
      getRangeByName('impressions').getValue());
  var clicksThreshold = parseField(spreadsheet.getRangeByName('clicks').
      getValue());
  var costThreshold = parseField(spreadsheet.getRangeByName('cost').getValue());
  var weeksStr = spreadsheet.getRangeByName('weeks').getValue();
  var weeks = parseInt(weeksStr.substring(0, weeksStr.indexOf(' ')));
  var email = spreadsheet.getRangeByName('email').getValue();

  var now = new Date(Utilities.formatDate(new Date(),
      AdWordsApp.currentAccount().getTimeZone(), 'MMM dd,yyyy HH:mm:ss'));

  var currentDate = now.getDate();
  now.setTime(now.getTime() - 3 * 3600 * 1000);
  var adjustedDate = now.getDate();

  var hours = now.getHours();
  if (hours == 0) {
    hours = 24;
  }
  if (hours == 1) {
    // first run of the day, kill existing alerts
    spreadsheet.getRangeByName('clicks_alert').clearContent();
    spreadsheet.getRangeByName('impressions_alert').clearContent();
    spreadsheet.getRangeByName('cost_alert').clearContent();
  }
  var dayToCheck;
  if (currentDate != adjustedDate) {
    dayToCheck = 1;
  } else {
    dayToCheck = 0;
  }
  var dateRangeToCheck = getDateInThePast(dayToCheck);
  var dateRangeToEnd = getDateInThePast(dayToCheck + 1);
  var dateRangeToStart = getDateInThePast(dayToCheck + 1 + weeks * 7);
  var fields = 'HourOfDay,DayOfWeek,Clicks,Impressions,Cost';

  var today = AdWordsApp.report('SELECT ' + fields +
      ' FROM ACCOUNT_PERFORMANCE_REPORT DURING ' + dateRangeToCheck + ',' +
      dateRangeToCheck, REPORTING_OPTIONS);
  var past = AdWordsApp.report('SELECT ' + fields +
      ' FROM ACCOUNT_PERFORMANCE_REPORT WHERE DayOfWeek=' +
      DAYS[now.getDay()].toUpperCase() +
      ' DURING ' + dateRangeToStart + ',' + dateRangeToEnd, REPORTING_OPTIONS);

  var todayStats = accumulateRows(today.rows(), hours, 1);
  var pastStats = accumulateRows(past.rows(), hours, weeks);

  var alertText = [];
  if (impressionsThreshold &&
      todayStats.impressions < pastStats.impressions * impressionsThreshold) {
    var range = spreadsheet.getRangeByName('impressions_alert');
    if (!range.getValue() || range.getValue().length == 0) {
      alertText.push('    Impressions are too low: ' + todayStats.impressions +
          ' impressions by ' + hours + ':00, expecting at least ' +
          parseInt(pastStats.impressions * impressionsThreshold));
      range.setValue('Alerting ' + hours + ':00');
    }
  }
  if (clicksThreshold &&
      todayStats.clicks < pastStats.clicks * clicksThreshold) {
    var range = spreadsheet.getRangeByName('clicks_alert');
    if (!range.getValue() || range.getValue().length == 0) {
      alertText.push('    Clicks are too low: ' + todayStats.clicks +
          ' clicks by ' + hours + ':00, expecting at least ' +
          (pastStats.clicks * clicksThreshold).toFixed(1));
      range.setValue('Alerting ' + hours + ':00');
    }
  }
  if (costThreshold && todayStats.cost > pastStats.cost * costThreshold) {
    var range = spreadsheet.getRangeByName('cost_alert');
    if (!range.getValue() || range.getValue().length == 0) {
      alertText.push('    Cost is too high: ' + todayStats.cost + ' ' +
          AdWordsApp.currentAccount().getCurrencyCode() + ' by ' + hours +
          ':00, expecting at most ' +
          (pastStats.cost * costThreshold).toFixed(2));
      range.setValue('Alerting ' + hours + ':00');
    }
  }
  if (alertText.length > 0 && email && email.length > 0) {
    MailApp.sendEmail(email,
        'AdWords Account ' + AdWordsApp.currentAccount().getCustomerId() +
        ' misbehaved.',
        'Your account ' + AdWordsApp.currentAccount().getCustomerId() +
        ' is not performing as expected today: \n\n' + alertText.join('\n') +
        '\n\nLog into AdWords and take a look.\n\nAlerts dashboard: ' +
        SPREADSHEET_URL);
  }
  spreadsheet.getRangeByName('date').setValue(new Date());
  spreadsheet.getRangeByName('account_id').setValue(
      AdWordsApp.currentAccount().getCustomerId());
  spreadsheet.getRangeByName('timestamp').setValue(
      DAYS[now.getDay()] + ', ' + hours + ':00');

  var dataRows = [
    [todayStats.impressions, pastStats.impressions.toFixed(0)],
    [todayStats.clicks, pastStats.clicks.toFixed(1)],
    [todayStats.cost, pastStats.cost.toFixed(2)]
  ];
  spreadsheet.getRangeByName('data').setValues(dataRows);
}

function toFloat(value) {
  value = value.toString().replace(/,/g, '');
  return parseFloat(value);
}

function parseField(value) {
  if (value == 'No alert') {
    return null;
  } else {
    return toFloat(value);
  }
}

function accumulateRows(rows, hours, weeks) {
  var row;
  var result;

  while (rows.hasNext()) {
    var row = rows.next();
    var hour = row['HourOfDay'];

    if (hour < hours) {
      result = addRow(row, result, 1 / weeks);
    }
  }

  return result;
}

function addRow(row, previous, coefficient) {
  if (!coefficient) {
    coefficient = 1;
  }
  if (row == null) {
    row = {Clicks: 0, Impressions: 0, Cost: 0};
  }
  if (!previous) {
    return {
      clicks: parseInt(row['Clicks']) * coefficient,
      impressions: parseInt(row['Impressions']) * coefficient,
      cost: toFloat(row['Cost']) * coefficient,      
      //conversions: toFloat(row['Conversions']) * coefficient
    };
  } else {
    return {
      clicks: parseInt(row['Clicks']) * coefficient + previous.clicks,
      impressions: parseInt(row['Impressions']) * coefficient +
          previous.impressions,
      cost: toFloat(row['Cost']) * coefficient + previous.cost,
      //conversions: toFloat(row['Conversions']) * coefficient + previous.conversions
    };
  }
}

function checkInRange(today, yesterday, coefficient, field) {
  var yesterdayValue = yesterday[field] * coefficient;
  if (today[field] > yesterdayValue * 2) {
    Logger.log('' + field + ' too much');
  } else if (today[field] < yesterdayValue / 2) {
    Logger.log('' + field + ' too little');
  }
}

// Returns YYYYMMDD-formatted date.
function getDateInThePast(numDays) {
  var today = new Date();
  today.setDate(today.getDate() - numDays);
  return Utilities.formatDate(today, 'PST', 'yyyyMMdd');
}