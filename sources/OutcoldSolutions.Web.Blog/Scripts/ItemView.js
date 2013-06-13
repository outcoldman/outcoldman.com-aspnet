$(function () {
    $("#btnAddComment").click(function () {
        var nHeader = $("#notification > h4");
        hideHeader();
        var comment = getCommentData();
        var isValid = true;

        if (comment.body == "") {
            nHeader.append(fillBodyComment);
            blink($("#commentBody"));
            isValid = false;
        }

        if (comment.fInform && comment.email == "") {
            nHeader.append(fillEmailComment);
            blink($("#Email"));
            blink($("#commentNeedInform"));
            isValid = false;
        }

        if (isValid) {
            startRequest();
            $.post(commentAddUrl, comment, commentAdded, "json");
            saveCookie(comment);
        } else {
            $("#notification").show("fast");
        }
    });

    $("#Email").keydown(function (event) {
        $("#commentNeedInform").prop('checked', true);
    });

    $("#btnShowPreview").click(function () {
        hideHeader();
        startRequest();
        $.post(commentPreviewUrl, getCommentData(), commentPreviewed, "json");
    });

    try {
        setSavedValue(commentAddUrl + "_commentBody", "#commentBody");
        setSavedValue("userName", "#Name");
        setSavedValue("userEmail", "#Email");
        setSavedValue("userSite", "#WebSite");

        if ($.cookie("userfInform")) {
            $("#commentNeedInform")[0].checked = $.cookie("userfInform");
        } else if (Modernizr.localstorage) {
            $("#commentNeedInform")[0].checked = localStorage["userfInform"] == "true";
        }

        if (Modernizr.localstorage) {
            $("#commentBody").bind('keyup', function () {
                localStorage[commentAddUrl + "_commentBody"] = this.value;
            });
            $("#Name").bind('keyup', function () {
                localStorage["userName"] = this.value;
            });
            $("#Email").bind('keyup', function () {
                localStorage["userEmail"] = this.value;
            });
            $("#WebSite").bind('keyup', function () {
                localStorage["userSite"] = this.value;
            });
            $("#commentNeedInform").click(function () {
                localStorage["userfInform"] = this.checked;
            });
        }
    } catch (e) { }
});

function commentAdded(data, textStatus, XMLHttpRequest) {
    if (XMLHttpRequest.status == 200) {
        $("#notification > h4").html(data.resMessage);
        if (data.result) {
            $("#commentBody").val("");
            showComment(data);
            if (Modernizr.localstorage) {
                localStorage.removeItem(commentAddUrl + "_commentBody");
            }
        }
    } else {
        $("#notification > h4").html(couldntAddComment);
    }
    $("#notification").show("fast");
    endRequest();
}

function commentPreviewed(data, textStatus, XMLHttpRequest) {
    if (XMLHttpRequest.status == 200) {
        showComment(data);
    } else {
        $("#notification > h4")[0].innerHTML = couldntShowPreview;
    }
    $("#notification").show("fast");
    endRequest();
}

function showComment(data) {
    $("#preview").remove();
    $("#comments").append(data.commentView);
    doCurrentDate();
}

function getCommentData() {
    return {
        name: $("#Name").val(),
        email: $("#Email").val(),
        body:  $("#commentBody").val(),
        fInform: $("#commentNeedInform").val(),
        site: $("#WebSite").val()
    };
}

function setSavedValue(savedValueId, elementId) {
    var saved = $.cookie(savedValueId);

    if (!saved && Modernizr.localstorage) {
        saved = localStorage[savedValueId];
    }

    if (saved != null) {
        $(elementId).val(saved);
    }
}

function saveCookie(comment) {
    try {
        $.cookie("userName", comment.name, { expires: 2880 });
        $.cookie("userEmail", comment.email, { expires: 2880 });
        $.cookie("userSite", comment.site, { expires: 2880 });
        $.cookie("userfInform", comment.fInform, { expires: 2880 });
    } catch (e) { }
}

function blink(el) {
    $(el).fadeTo(250, 0.5, function () { $(el).fadeTo(250, 1); });
}

function startRequest() {
    $("#btnAddComment").disabled();
    $("#btnShowPreview").disabled();
    $("#busyIndicator").show();
}

function endRequest() {
    $("#btnAddComment").enabled();
    $("#btnShowPreview").enabled();
    $("#busyIndicator").hide();
}

function hideHeader() {
    $("#notification").hide("fast");
    $("#notification > h4").empty();
}