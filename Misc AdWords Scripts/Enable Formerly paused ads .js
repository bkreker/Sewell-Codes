	/************************************
	* Item Out Of Stock Checker
	* Version 1.2
	* ChangeLog v1.2
	*  - ONLY_ACTIVE is used to filter campaigns and adgroups only. All Keywords and Ads in the AdGroups will
	*    be checked which solves the "once disabled, always disabled" issue.
	*  - Updated call to get the Final Urls. Now calls getFinalUrl and getMobileFinalUrl instead of getDestinationUrl
	*  - OUT_OF_STOCK_TEXTS can now contain multiple things to check for.
	*  - If the CAMPAIGN_LABEL does not exist, it is ignored with a warning.
	* ChangeLog v1.1 - Filtered out deleted Campaigns and AdGroups
	* Created By: Russ Savage
	* FreeAdWordsScripts.com
	***********************************/
	var URL_LEVEL = 'Ad'; // or Keyword
	var ONLY_ACTIVE = true; // set to false to check keywords or ads in all campaigns (paused and active)
	var CAMPAIGN_LABEL = ''; // set this if you want to only check campaigns with this label
	var STRIP_QUERY_STRING = true; // set this to false if the stuff that comes after the question mark is important
	var WRAPPED_URLS = false; // set this to true if you use a 3rd party like Marin or Kenshoo for managing you account
	var EMAIL_ADDRESS = "joshd@sewelldirect.com";
	var LABEL = "Out_of_Stock";

	// This is the specific text (or texts) to search for 
	// on the page that indicates the item 
	// is out of stock. If ANY of these match the html
	// on the page, the item is considered "out of stock"
	var OUT_OF_STOCK_TEXTS = [
	  'Out of Stock',
	  'Preorder'
	];

	function main() 
	{
		var alreadyCheckedUrls = {};
		var iter = buildSelector().get();

		// Array to hold all newly paused urls
		var pausedUrls = [];  

		while(iter.hasNext()) 
		{	    
				var entity = iter.next();
				var urls = [];

				if(entity.urls().getFinalUrl())       
				{
					urls.push(entity.urls().getFinalUrl());
				}

				if(entity.urls().getMobileFinalUrl()) 
				{
					urls.push(entity.urls().getMobileFinalUrl());
				}

				for(var i in urls)
			{      
				var url = cleanUrl(urls[i]);
				if(alreadyCheckedUrls[url]) 
				{
					if(alreadyCheckedUrls[url] === 'out of stock')
					{  
						
						entity.pause();
						
						entity.applyLabel(LABEL);
						
						// Add this to the list of paused urls
						pausedUrls.push('\nAds for '+entity+': '+url+' are now paused.\n');
						
						Logger.log('Ads for: '+entity+' are now paused.');
						//Logger.log(entity.Label.getName(LABEL));
						
					} 
					else 
					{
						//if(entity.Label.getName() === "Out_of_Stock"){            
						entity.enable();
						
						//}
					}
				} 
				else
				{
					var htmlCode;
					try 
					{
						htmlCode = UrlFetchApp.fetch(url).getContentText();
					} 
					catch(e) 
					{
					Logger.log('There was an issue checking:'+url+', Skipping.');
					continue;
					}
				
					var did_pause = false;
					for(var x in OUT_OF_STOCK_TEXTS) 
					{						
						if(htmlCode.indexOf(OUT_OF_STOCK_TEXTS[x]) >= 0)  
						{
							alreadyCheckedUrls[url] = 'out of stock';
							entity.pause();
							did_pause = true;
							
							Logger.log('Url: '+url+' is '+alreadyCheckedUrls[url]);
							
							// Add this to the list of paused urls
							pausedUrls.push('\nAds for '+entity+': '+url+' are now paused.\n');
							
							Logger.log('Ads for: '+entity+' are now paused.');
							
							break;
						}
					}  
					
					if(!did_pause) { 
						alreadyCheckedUrls[url] = 'in stock';
						entity.enable();
					}
				}
			}
		} 
	  
	  Logger.log(pausedUrls.length+' Ads Paused Because Out of Stock: \n'+pausedUrls.toString());
	  
	  //MailApp.sendEmail(EMAIL_ADDRESSs, "Quality Score Tracker for Keywords", "You can see the keywords on the following url\n\n"+workbook.getUrl());
	  EmailResults(pausedUrls);
	}

	function cleanUrl(url) 
	{
		if(WRAPPED_URLS) 
		{
			url = url.substr(url.lastIndexOf('http'));
			if(decodeURIComponent(url) !== url) 
			{
				url = decodeURIComponent(url);
			}
		}
		if(STRIP_QUERY_STRING) 
		{
			if(url.indexOf('?')>=0) 
			{
				url = url.split('?')[0];
			}
		}
		if(url.indexOf('{') >= 0) 
		{
			//Let's remove the value track parameters
			url = url.replace(/\{[0-9a-zA-Z]+\}/g,'');
		}
		return url;
	}

	function EmailResults(pausedUrls) {
	  MailApp.sendEmail(EMAIL_ADDRESS,
						'AdWords Alert: Items out of stock; Ads paused',
						pausedUrls.length+' Ads Paused Because Out of Stock:\n'+pausedUrls.toString());
	}

	function previouslyOutOfStockNowInStock(){
		
	}
	
	
	function buildSelector() 
	{
	  var selector = (URL_LEVEL === 'Ad') ? AdWordsApp.ads() : AdWordsApp.keywords();
	  selector = selector.withCondition('CampaignStatus != DELETED').withCondition('AdGroupStatus != DELETED');
	  
	  if(ONLY_ACTIVE) 
	  {
			selector = selector.withCondition('CampaignStatus = ENABLED');
			
		if(URL_LEVEL !== 'Ad') 
		{
			selector = selector.withCondition('AdGroupStatus = ENABLED');
		}
	  }
	  if(CAMPAIGN_LABEL) {
		if(AdWordsApp.labels().withCondition("Name = '"+CAMPAIGN_LABEL+"'").get().hasNext()) {
		  var label = AdWordsApp.labels().withCondition("Name = '"+CAMPAIGN_LABEL+"'").get().next();
		  var campIter = label.campaigns().get();
		  var campaignNames = [];
		  while(campIter.hasNext()) {
			campaignNames.push(campIter.next().getName());
		  }
		  selector = selector.withCondition("CampaignName IN ['"+campaignNames.join("','")+"']");
		} else {
		  Logger.log('WARNING: Campaign label does not exist: '+CAMPAIGN_LABEL);
		}
	  }
	  return selector;
	}
