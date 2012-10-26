function utcToLocal(u) {
    var l = new Date(u.substring(6, 10), u.substring(3, 5) - 1, u.substring(0, 2), u.substring(11, 13), u.substring(14, 16));
    var d = new Date(l.getTime() + (-l.getTimezoneOffset() * 60 * 1000));
    return wZ(d.getDate()) + '.' + wZ(d.getMonth() + 1) + '.' + d.getFullYear() + ' ' + wZ(d.getHours()) + ':' + wZ(d.getMinutes());
}

function wZ(x) {
    if (x <= 9) return '0' + x;
    return x;
}

function doCurrentDate() {
    $("time").each(function() {
        var content = this.innerHTML;
        if (content.toUpperCase().indexOf("UTC") >= 0) {
            this.innerHTML = utcToLocal(content);
        }
    });
}

$(document).ready(function() {
    doCurrentDate();
});