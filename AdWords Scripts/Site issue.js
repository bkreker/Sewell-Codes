Bootstrapper.bindDependencyDOMParsed(

function () {
    var Bootstrapper = window["Bootstrapper"];
    var ensightenOptions = Bootstrapper.ensightenOptions;
    Bootstrapper.on("mousedown", "#submitReport, #createReportDownloadUri", function () {
        try {
            var eventType = "link";
            var path = window.location.pathname;
            var id = this.id;
            if (id !== undefined)id = id.replace(/[0-9]/g, "X");
            var name = this.name;
            if (name !== undefined)name = name.replace(/[0-9]/g, "X");
            var result = $(this).val();
            var finalResult = (eventType + "|" + path + "|" + name + "|" + id + "|" + result).replace(/undefined|null/g, "").replace(/\|+/g, "|").replace(/\|+$/, "");
            ensightensc.products = ";" + finalResult;
            if (id === "submitReport")ensightensc.events = "event63,event3,prodView";
            else if (id === "createReportDownloadUri")ensightensc.events = "event64,event3,prodView";
            else ensightensc.events = "event3,prodView";
            ensightensc.pageName = Bootstrapper.data.resolve("11652");
            ensightensc.eVar69 = result;
            var reportType = "";
            try {
                reportType = $("#ReportSubjectSelector").val();
            }
            catch (e) {
            }
            switch (reportType) {
                case "s:kywd:actv:sum":
                    reportType = "Keyword";
                    break;
                case "s:search:query":
                    reportType = "Search term";
                    break;
                case "s:disp:ad:perf":
                    reportType = "Ad";
                    break;
                case "s:acct:actv":
                    reportType = "Account";
                    break;
                case "s:cmp:actv":
                    reportType = "Campaign";
                    break;
                case "s:ord:sum":
                    reportType = "Ad group";
                    break;
                case "s:site:bidding:perf":
                    reportType = "Website placement";
                    break;
                case "s:dest:url:actv":
                    reportType = "Destination URL";
                    break;
                case "s:ad:parm:sum":
                    reportType = "Ad dynamic text";
                    break;
                case "s:pub:plc:perf":
                    reportType = "Website URL (publisher)";
                    break;
                case "s:rich:ad:cmpnt:perf":
                    reportType = "Rich ad component";
                    break;
                case "s:kywd:auct:sum":
                    reportType = "Share of voice";
                    break;
                case "s:ad:ext:kywd":
                    reportType = "Ad extension by keyword";
                    break;
                case "s:ad:ext:ads":
                    reportType = "Ad extension by ad";
                    break;
                case "s:ad:extension:item:activity":
                    reportType = "Ad extension details";
                    break;
                case "s:prdct:target:actv:sum":
                    reportType = "Product target";
                    break;
                case "s:pla:offer:perf":
                    reportType = "Product offer";
                    break;
                case "s:call:details":
                    reportType = "Call forwarding detail";
                    break;
                case "s:demog:ageor:gender:sum":
                    reportType = "Age and gender";
                    break;
                case "s:demog:major:city:sum":
                    reportType = "Geo location (Old version)";
                    break;
                case "s:demog:location:sum":
                    reportType = "Geo location (New version)";
                    break;
                case "s:neg:kywd:conflicts":
                    reportType = "Negative keyword conflicts";
                    break;
                case "s:cnv:perf":
                    reportType = "Conversions";
                    break;
                case "s:goals:funnels:sum":
                    reportType = "Goals";
                    break;
                case "s:traffic:sum":
                    reportType = "Traffic sources";
                    break;
                case "s:segmentation:sum":
                    reportType = "Segments";
                    break;
                case "s:tactic:chnl:sum":
                    reportType = "Tactics and channels";
                    break;
                case "s:adv:budget:sum":
                    reportType = "Budget";
                    break;
                case "s:bill":
                    reportType = "Customer invoice billing";
                    break;
                case "s:inv:cust:bill":
                    reportType = "Billing statement";
                    break;
                    default : reportType = reportType;
                    break;
            }
            var reportGrain = "";
            try {
                reportGrain = $("#ReportTimeGrainSelector").val();
            }
            catch (e) {
            }
            var reportRange = "";
            try {
                reportRange = $("#ReportRelativeTimeRangeSelector").val();
                reportRange = reportRange.replace(/tr:/, "");
            }
            catch (e) {
            }
            var reportZone = "";
            try {
                reportZone = $("#ReportTimeZoneSelectList").val();
            }
            catch (e) {
            }
            var reportFormat = "";
            try {
                reportFormat = $("#ReportOutputFormatSelector").val();
                reportFormat = reportFormat.replace(/opt:/, "");
            }
            catch (e) {
            }
            var reportAccounts = "";
            try {
                if ($("#accountTreeClose").attr("checked"))reportAccounts = "All accounts";
                else if($("#accountTreeOpen").attr("checked"))reportAccounts = "Specific accounts";
                else reportAccount = "error";
            }
            catch (e) {
            }
            var reportColumns = "";
            try {
                $(".columnChooserText").each(function () {
                    reportColumns = reportColumns + "," + $(this).text();
                }
                );
                reportColumns = reportColumns.replace(/\,/, "");
            }
            catch (e) {
            }
            var reportFilter = "";
            try {
                $("#report_filters input[type\x3dcheckbox]").each(
				function () {
                    if ($(this).attr("checked"))reportFilter = reportFilter + "," + $(this).next().text();
                }
                );
                if (reportFilter === "")reportFilter = "none";
                else reportFilter = reportFilter.replace(/\,/, "");
            }
            catch (e) {
            }
            var reportSettings = "";
            try {
                $("#reportSchedule input[type\x3dcheckbox]").each(function () {
                    if ($(this).attr("checked"))reportSettings = reportSettings + "," + $(this).parent().next().text();
                }
                );
                if (reportSettings === "")reportSettings = "none";
                else reportSettings = reportSettings.replace(/\,/, "");
            }
            catch (e) {
            }
            ensightensc.prop56 = reportGrain;
            ensightensc.prop57 = reportRange;
            ensightensc.prop58 = reportZone;
            ensightensc.prop63 = reportFormat;
            ensightensc.prop51 = reportType;
            ensightensc.prop52 = reportAccounts;
            ensightensc.prop53 = reportColumns;
            ensightensc.prop54 = reportFilter;
            ensightensc.prop55 = reportSettings;
            ensightensc.linkTrackVars = "events,prop51,prop52,prop53,prop54,prop55,prop56,prop57,prop58,prop63,eVar69,products,pageName";
            ensightensc.linkTrackEvents = ensightensc.events || "None";
            $(this).data("TrackingSkipAuto", 1);
            ensightensc.tl(this, "o", finalResult);
            try {
                console.log("Ensighten deployment #307254 Reporting: " + finalResult);
            }
            catch (e) {
            }
        }
        catch (e) {
        }
    }
    );
	
    Bootstrapper.on("mousedown", "#navPanelWunderbar a", function () {
        try {
            var eventType = "link";
            var path = window.location.pathname;
            var id = this.id;
            if (id !== undefined)id = id.replace(/[0-9]/g, "X");
            var name = this.name;
            if (name !== undefined)name = name.replace(/[0-9]/g, "X");
            var result = $(this).text().trim().substring(0, 50);
            if (result !== undefined)result = result.replace(/[0-9]/g, "X");
            var finalResult = (eventType + "|" + path + "|" + name + "|" + id + "|" + result).replace(/undefined|null/g, "").replace(/\|+/g, "|").replace(/\|+$/, "");
            ensightensc.products = ";" + finalResult;
            ensightensc.pageName = Bootstrapper.data.resolve("11652");
            ensightensc.eVar69 = result;
            ensightensc.events = "";
            ensightensc.events = "event83,event3,prodView";
            ensightensc.linkTrackVars = "events,eVar69,products,pageName";
            ensightensc.linkTrackEvents = ensightensc.events || "None";
            $(this).data("TrackingSkipAuto", 1);
            ensightensc.tl(this, "o", finalResult);
            try {
                console.log("Ensighten deployment #307254 Reporting: " + finalResult);
            }
            catch (e) {
            }
        }
        catch (e) {
        }
    }
    );
}
, 882959, [1344507], 307254, [136938]);
