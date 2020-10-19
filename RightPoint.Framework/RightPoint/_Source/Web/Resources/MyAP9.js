<script language="JavaScript">
var cookieLife = <%=COOKIELIFE%>;	 
var overwriteCookies = 1;
var scriptLoc = ''; //URL to the users Processor.aspx script. 
//Determine http or https 
if (document.URL.indexOf('https:') > -1) 
{scriptLoc = 'https://';} 
else {scriptLoc = 'http://';} 
var domain = scriptLoc + '<%=DOMAINNAME%>/';
//Finish off url 
scriptLoc = domain + 'MAPProc.aspx';	//URL to the users Processor.aspx script. 
//------------------------------------------------------- 
//         Don't edit anything below this line!! 
//------------------------------------------------------- 
var img = new Image(); 
var aImg = new Image();
var kbId = 0; 
var qs = location.search.substring(1); 
//Build the url 
var url = scriptLoc + '?'; 
url += '&curUrl=' + escape(document.URL); 
url += '&refUrl=' + escape(document.referrer); 
url += '&c=' + escape(window.screen.colorDepth) 
url += '&sw=' + escape(self.screen.width); 
url += '&sh=' + escape(self.screen.height); 
url += '&winid=' + escape(window.name); 
url += '&ow=' + overwriteCookies.toString();
var hasCookies = 1;
url += "&cookies=" + hasCookies.toString();
img.src = url; 
kbId = <%=KBID%>;//set on the server side
//----------------
//Action Processor
//----------------
function AProc(aID, profit, id, life)
{
	aImg = new Image();
	var x = Math.round(Math.random()*9999999);
	aImg.src = domain + 'AProc.aspx?aID=' + aID.toString() + '&p=' + profit.toString() + '&id=' + escape(id.toString()) + '&l=' + escape(life) + '&curUrl=' + escape(document.URL) + '&x=' + x.toString();
	pause(500);
}
function pause(ms)
{
	d = new Date(); //today's date
	while (1)
	{
		mill=new Date(); // Date Now
		diff = mill-d; //difference in milliseconds
		if( diff > ms ) {break;}
	}
}
 </script>