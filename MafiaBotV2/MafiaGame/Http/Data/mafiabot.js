function addTownRole(name) {
    $("#townRoles").append("<div class=\"role\"><div class=\"roletext\">" + name + "</div><div class=\"rolelink\"><a class=\"removerole\" href=\"#\">Remove</a></div></div>");

    $(".removerole").unbind("click", onRemoveRole);
    $(".removerole").click(onRemoveRole);
}

function addMafiaRole(name) {
    $("#mafiaRoles").append("<div class=\"role\"><div class=\"roletext\">" + name + "</div><div class=\"rolelink\"><a click=\"onRemoveRole\" class=\"removerole\" href=\"#\">Remove</a></div></div>");

    $(".removerole").unbind("click", onRemoveRole);
    $(".removerole").click(onRemoveRole);
}

function addRole(name) {
    $("#townSelect").append("<option>" + name + "</option>");
    $("#mafiaSelect").append("<option>" + name + "</option>");
}

function onAddRole() {
    faction = $(this).attr('name');
    role = ($('#' + faction + 'Select :selected').text())
    $.get("addrole?faction=" + faction + "&role=" + role);

    if (faction == "town") {
        addTownRole(role);
    }
    else if (faction == "mafia") {
        addMafiaRole(role);
    }
}

function onRemoveRole() {
    faction = "unknown";
    if ($(this).parent().parent().parent().is("#townRoles")) {
        faction = "town";
    }
    else if ($(this).parent().parent().parent().is("#mafiaRoles")) {
        faction = "mafia";
    }
    role = $(this).parent().parent().children(".roletext").text();
    $.get("removerole?faction=" + faction + "&role=" + role);

    $(this).parent().parent().detach();
}

$(document).ready(function () {
    $.getScript('init.js');

    $("#nolynch").click(function () {
        $.get("nolynch?value=" + $("#nolynch").attr('checked'))
    });
    $("#cardflip").click(function () {
        $.get("cardflip?value=" + $("#cardflip").attr('checked'))
    });
    $("#night0").click(function () {
        $.get("night0?value=" + $("#night0").attr('checked'))
    });

    $(".addrolebutton").click(onAddRole);
    
});