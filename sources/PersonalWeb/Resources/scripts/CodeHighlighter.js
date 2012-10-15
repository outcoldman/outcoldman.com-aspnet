$(function () {
	$("#tryButton").click(function () {
		$("#notification").hide("fast");
		$("#notification > h4")[0].innerHTML = "";

		var inputcode = $("#inputcode")[0].value;
		var codetype = $("#codetype")[0].value;

		var model = { inputcode: inputcode, type: codetype };
		$("#tryButton")[0].disabled = true;
		$("#busyIndicator").show();
		$.post(CodeHighlighterCallbackUrl, model, callbackMethod, "json");
	});
});


function callbackMethod(data, textStatus, XMLHttpRequest) {
	if (XMLHttpRequest.status == 200 && data.res) {
		$("#result")[0].innerHTML = data.formattedCodeEscaped;
		$("#resultExample")[0].innerHTML = data.formattedCode;
	} else {
		$("#notification > h4")[0].innerHTML = CouldntExecuteMessage + '<br/>' + data.errorinfo;
	}
	$("#notification").show("fast");
	$("#tryButton")[0].disabled = false;
	$("#busyIndicator").hide();
}