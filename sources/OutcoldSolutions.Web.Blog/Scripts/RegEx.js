$(function () {
	$("#tryButton").click(function () {
		$("#notification").hide("fast");
		$("#notification > h4")[0].innerHTML = "";

		var regex = $("#regex")[0].value;
		var checkstring = $("#checkstring")[0].value;

		var model = { regex: regex, checkstring: checkstring };
		$("#tryButton")[0].disabled = true;
		$("#busyIndicator").show();
		$.post(RegExCallbackUrl, model, callbackMethod, "json");
	});
});


function callbackMethod(data, textStatus, XMLHttpRequest)	{
	if (XMLHttpRequest.status == 200 && data.res) {
		$("#notification > h4")[0].innerHTML = data.matches;
	} else {
		$("#notification > h4")[0].innerHTML = CouldntExecuteMessage + '<br/>' + data.errorinfo;
	}
	$("#notification").show("fast");
	$("#tryButton")[0].disabled = false;
	$("#busyIndicator").hide();
}