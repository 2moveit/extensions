﻿var SF = SF || {};

SF.Auth = (function () {
    var coloredRadios = function ($ctx) {
        var updateBackground = function () {
            var $this = $(this);
            $this.toggleClass("sf-auth-chooser-disabled", !$this.find(":radio").attr("checked"));
        };

        var $links = $ctx.find(".sf-auth-rules .sf-auth-chooser");

        $links.find(":radio").hide();
        $links.each(updateBackground);

        $links.click(function () {
            var $this = $(this);
            var $tr = $this.closest("tr");
            $tr.find(".sf-auth-chooser :radio").attr("checked", false);
            var $radio = $this.find(":radio");
            $radio.attr("checked", true);
            $tr.find(".sf-auth-overriden").attr("checked", $radio.val() != $tr.find("input[name$=Base]").val());
            $tr.find(".sf-auth-chooser").each(updateBackground);
        });
    };

    var multiSelRadios = function ($ctx) {
        var updateBackground = function () {
            var $this = $(this);
            $this.toggleClass("sf-auth-chooser-disabled", !$this.find(":checkbox").attr("checked"));
        };

        var $links = $ctx.find(".sf-auth-chooser");

        $links.find(":checkbox").hide();
        $links.each(updateBackground);

        $links.bind("mousedown", function () {
            this.onselectstart = function () { return false; };
        });
        $links.click(function (e) {
            var $this = $(this);
            var $tr = $this.closest("tr");
            var $cb = $this.find(":checkbox");

            if (!e.shiftKey) {
                $tr.find(".sf-auth-chooser :checkbox").attr("checked", false);
                $cb.attr("checked", true);
            } else {
                var num = $tr.find(".sf-auth-chooser :checkbox:checked").length;
                if (!$cb.attr("checked") && num == 1) {
                    $cb.attr("checked", true);
                }
                else if ($cb.attr("checked") && num >= 2) {
                    $cb.attr("checked", false);
                }
            }

            var total = $.map($tr.find(".sf-auth-chooser :checkbox:checked"), function (a) { return $(a).attr("data-tag"); }).join(",");

            $tr.find(".sf-auth-overriden").attr("checked", total != $tr.find("input[name$=Base]").val());
            $tr.find(".sf-auth-chooser").each(updateBackground);
        });
    };

    var treeView = function () {
        $(".sf-auth-namespace").live("click", function (e) {
            e.preventDefault();
            var $this = $(this);
            $this.find(".sf-auth-expanded-last,.sf-auth-closed-last").toggleClass("sf-auth-expanded-last").toggleClass("sf-auth-closed-last");
            $this.find(".sf-auth-expanded,.sf-auth-closed").toggleClass("sf-auth-expanded").toggleClass("sf-auth-closed");
            var ns = $this.find(".sf-auth-namespace-name").html();
            $(".sf-auth-rules tr").filter(function () {
                return $(this).attr("data-ns") == ns;
            }).toggle();
        });
    };

    var openDialog = function (e) {
        e.preventDefault();
        var $this = $(this);
        var navigator = new SF.ViewNavigator({
            controllerUrl: $this.attr("href"),
            requestExtraJsonData: null,
            type: 'PropertyRulePack',
            onCancelled: function () {
                $this.closest("div").css("opacity", 0.5);
                $this.find(".sf-auth-thumb").css("opacity", 0.5);
            },
            onLoaded: function (divId) {
                SF.Auth.coloredRadios($("#" + divId));
            }
        });
        navigator.createSave();
    };

    var postDialog = function (controllerUrl, prefix) {
        new SF.PartialValidator({ controllerUrl: controllerUrl, prefix: prefix }).trySave();
    };

    var findTrsInGroup = function (ns, type) {
        return $(".sf-auth-rules tr[data-ns='" + ns + "'][data-type='" + type + "']");
    };

    var removeCondition = function (e) {
        e.preventDefault();
        var $this = $(this);
        var $tr = $this.closest("tr");
        var $trsInGroup = findTrsInGroup($tr.attr("data-ns"), $tr.attr("data-type"));

        var $typeTr = $trsInGroup.filter(".sf-auth-type");
        $typeTr.find(".sf-create").show();
        $tr.remove();

        $trsInGroup = findTrsInGroup($tr.attr("data-ns"), $tr.attr("data-type")); //reevaluate after delete
        var $lastConditionTr = $trsInGroup.filter(":last").not(".sf-auth-type");
        $lastConditionTr.find(".sf-auth-tree:eq(2)").removeClass().addClass("sf-auth-tree sf-auth-leaf-last");
    };

    var chooseConditionToAdd = function ($sender, url, title) {
        var $tr = $sender.closest("tr");
        var ns = $tr.attr("data-ns");
        var type = $tr.attr("data-type");
        var conditions = $tr.find(".sf-auth-available-conditions").val().split(",");
        var $rules = $(".sf-auth-rules tr");
        var conditionsNotSet = conditions.filter(function (c) { return $rules.filter("[data-ns='" + ns + "'][data-type='" + type + "'][data-condition='" + c + "']").length == 0; });
        SF.openChooser("New", function (chosenCondition) { addCondition($tr, chosenCondition); }, conditionsNotSet, null, { controllerUrl: url, title: title });
    };

    var addCondition = function ($typeTr, condition) {
        var $newTr = $typeTr.clone();
        $newTr.attr("data-condition", condition);
        $newTr.find(".sf-auth-label").html(condition);

        $newTr.removeClass("sf-auth-type").addClass("sf-auth-condition");
        $newTr.find(".sf-auth-available-conditions").remove();
        $newTr.find("td.sf-auth-type-only").html("");

        var $create = $newTr.find(".sf-create");
        $create.prev(".sf-auth-tree").removeClass().addClass("sf-auth-tree sf-auth-blank");
        $create.before($("<div></div>").addClass("sf-auth-tree sf-auth-leaf-last"));
        $create.removeClass("sf-create").addClass("sf-remove");
        $create.find(".ui-icon").removeClass("ui-icon-circle-plus").addClass("ui-icon-circle-close");

        //update indexes
        var $trsInGroup = findTrsInGroup($typeTr.attr("data-ns"), $typeTr.attr("data-type"));
        var $lastTrInGroup = $trsInGroup.filter(":last");
        var lastConditionIndex = $lastTrInGroup.attr("data-index");
        if (typeof lastConditionIndex == "undefined") {
            lastConditionIndex = 0;
        }
        else {
            lastConditionIndex++;
        }
        $newTr.attr("data-index", lastConditionIndex);

        $newTr.find("td:first input[type='hidden']").remove();
        $newTr.find(".sf-auth-chooser input").each(function(i, e){
            var $input = $(e);
            var newId = $input.attr("name").replace("Fallback", "Conditions_" + lastConditionIndex + "_Allowed");
            $input.attr("name", newId);
            if ($input.filter(":checkbox").length > 0) {
                $input.attr("id", newId);
            }
        });
        var fullName = $newTr.find(".sf-auth-chooser:first input").attr("name");
        fullName = fullName.substring(0, fullName.lastIndexOf("Allowed")) + "ConditionName";
        var $conditionName = $("<input>").attr("type", "hidden").val(condition).attr("id", fullName).attr("name", fullName);
        $newTr.find("td:first").append($conditionName);        

        multiSelRadios($newTr);
        $lastTrInGroup.after($newTr);
        $lastTrInGroup.find(".sf-auth-tree:eq(2)").removeClass().addClass("sf-auth-tree sf-auth-leaf");

        if ($typeTr.find(".sf-auth-available-conditions").val().split(",").length == $trsInGroup.length) {
            $typeTr.find(".sf-create").hide();
        }
    };

    return {
        coloredRadios: coloredRadios,
        multiSelRadios: multiSelRadios,
        treeView: treeView,
        openDialog: openDialog,
        postDialog: postDialog,
        removeCondition: removeCondition,
        chooseConditionToAdd: chooseConditionToAdd
    };
})();