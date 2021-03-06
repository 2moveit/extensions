/// <reference path="../../../../Framework/Signum.Web/Signum/Scripts/globals.ts"/>
define(["require", "exports", "Framework/Signum.Web/Signum/Scripts/Entities", "Framework/Signum.Web/Signum/Scripts/Validator", "Framework/Signum.Web/Signum/Scripts/Navigator"], function(require, exports, Entities, Validator, Navigator) {
    function updateColoredBackground() {
        var $this = $(this);
        $this.toggleClass("sf-auth-chooser-disabled", !$this.find(":radio").prop("checked"));
    }

    function coloredRadios($ctx) {
        var $links = $ctx.find(".sf-auth-rules .sf-auth-chooser");

        $links.find(":radio").hide();
        $links.each(updateColoredBackground);

        $links.click(function () {
            var $this = $(this);
            var $tr = $this.closest("tr");
            $tr.find(".sf-auth-chooser :radio").prop("checked", false);
            var $radio = $this.find(":radio");
            $radio.prop("checked", true);
            $tr.find(".sf-auth-overriden").prop("checked", $radio.val() != $tr.find("input[name$=AllowedBase]").val());
            $tr.find(".sf-auth-chooser").each(updateColoredBackground);
        });
    }
    exports.coloredRadios = coloredRadios;

    function updateMultiSelBackground() {
        var $this = $(this);
        $this.toggleClass("sf-auth-chooser-disabled", !$this.find(":checkbox").prop("checked"));
    }

    function multiSelToStringParts($tr) {
        return $.map($tr.find(".sf-auth-chooser :checkbox:checked"), function (a) {
            return $(a).attr("data-tag");
        }).join(",");
    }
    ;

    function multiSelRadios($ctx) {
        var $links = $ctx.find(".sf-auth-chooser");

        $links.find(":checkbox").hide();
        $links.each(updateMultiSelBackground);

        $links.bind("mousedown", function () {
            this.onselectstart = function () {
                return false;
            };
        });
        $links.click(function (e) {
            var $this = $(this);
            var $tr = $this.closest("tr");
            var $cb = $this.find(":checkbox");

            if (!e.shiftKey) {
                $tr.find(".sf-auth-chooser :checkbox").prop("checked", false);
                $cb.prop("checked", true);
            } else {
                var num = $tr.find(".sf-auth-chooser :checkbox:checked").length;
                if (!$cb.prop("checked") && num == 1) {
                    $cb.prop("checked", true);
                } else if ($cb.prop("checked") && num >= 2) {
                    $cb.prop("checked", false);
                }
            }

            var total = "";
            var type = $tr.attr("data-type");
            if (typeof type == "undefined") {
                total = multiSelToStringParts($tr);
                $tr.find(".sf-auth-overriden").prop("checked", total != $tr.find("input[name$=AllowedBase]").val());
            } else {
                var $groupTrs = findTrsInGroup($tr.attr("data-ns"), type);
                var $typeTr = $groupTrs.filter(".sf-auth-type");
                total = multiSelToStringParts($typeTr);
                var $conditionTrs = $groupTrs.not(".sf-auth-type");
                $conditionTrs.each(function (i, e) {
                    var $ctr = $(e);
                    total += ";" + $ctr.attr("data-condition") + "-" + multiSelToStringParts($ctr);
                });
                $typeTr.find(".sf-auth-overriden").prop("checked", total != $typeTr.find("input[name$=AllowedBase]").val());
            }

            $tr.find(".sf-auth-chooser").each(updateMultiSelBackground);
        });
    }
    exports.multiSelRadios = multiSelRadios;

    function treeView() {
        $(document).on("click", ".sf-auth-namespace", function (e) {
            e.preventDefault();
            var $this = $(this);
            $this.find(".sf-auth-expanded-last,.sf-auth-closed-last").toggleClass("sf-auth-expanded-last").toggleClass("sf-auth-closed-last");
            $this.find(".sf-auth-expanded,.sf-auth-closed").toggleClass("sf-auth-expanded").toggleClass("sf-auth-closed");
            var ns = $this.find(".sf-auth-namespace-name").html();
            $(".sf-auth-rules tr").filter(function () {
                return $(this).attr("data-ns") == ns;
            }).toggle();
        });
    }
    exports.treeView = treeView;

    function openDialog(e) {
        e.preventDefault();
        var $this = $(this);
        Navigator.navigatePopup(Entities.EntityHtml.withoutType("New"), {
            controllerUrl: $this.attr("href"),
            onPopupLoaded: function (div) {
                return exports.coloredRadios(div);
            }
        }).then(function () {
            $this.closest("div").css("opacity", 0.5);
            $this.find(".sf-auth-thumb").css("opacity", 0.5);
        });
    }
    exports.openDialog = openDialog;

    function postDialog(controllerUrl, prefix) {
        Validator.validate({ controllerUrl: controllerUrl, prefix: prefix }).then(function (vr) {
            if (vr.isValid) {
                $(".sf-main-control[data-prefix='" + prefix + "']").removeClass("sf-changed");
            }
        });
    }
    exports.postDialog = postDialog;

    function submitPage(controllerUrl, prefix) {
        SF.submit(controllerUrl, null, null);
    }
    exports.submitPage = submitPage;

    function findTrsInGroup(ns, type) {
        return $(".sf-auth-rules tr[data-ns='" + ns + "'][data-type='" + type + "']");
    }

    function removeCondition(e) {
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
    }
    exports.removeCondition = removeCondition;

    function chooseConditionToAdd($sender, title) {
        var $tr = $sender.closest("tr");
        var ns = $tr.attr("data-ns");
        var type = $tr.attr("data-type");
        var conditions = $tr.find(".sf-auth-available-conditions").val().split(",");
        var $rules = $(".sf-auth-rules tr");
        var conditionsNotSet = conditions.filter(function (c) {
            return $rules.filter("[data-ns='" + ns + "'][data-type='" + type + "'][data-condition='" + c.before("|") + "']").length == 0;
        }).map(function (c) {
            return ({ id: c.before("|"), text: c.after("|") });
        });

        Navigator.chooser("New", title, conditionsNotSet).then(function (c) {
            return addCondition($tr, c.id);
        });
    }
    exports.chooseConditionToAdd = chooseConditionToAdd;
    ;

    function addCondition($typeTr, condition) {
        var $newTr = $typeTr.clone();
        $newTr.attr("data-condition", condition);

        var conditionNiceName = $typeTr.find(".sf-auth-available-conditions").val().split(",").filter(function (c) {
            return c.split("|")[0] == condition;
        })[0].split("|")[1];
        $newTr.find(".sf-auth-label").html(conditionNiceName);

        $newTr.removeClass("sf-auth-type").addClass("sf-auth-condition");
        $newTr.find(".sf-auth-available-conditions").remove();
        $newTr.find("td.sf-auth-type-only").html("");

        var $create = $newTr.find(".sf-create");
        $create.prev(".sf-auth-tree").removeClass().addClass($typeTr.find(".sf-create").prev(".sf-auth-tree").hasClass("sf-auth-leaf") ? "sf-auth-tree sf-auth-line" : "sf-auth-tree sf-auth-blank");
        $create.before($("<div></div>").addClass("sf-auth-tree sf-auth-leaf-last"));
        $create.removeClass("sf-create").addClass("sf-remove");
        $create.find(".glyphicon.glyphicon-plus").removeClass("glyphicon-plus").addClass("glyphicon-remove");

        //update indexes
        var $trsInGroup = findTrsInGroup($typeTr.attr("data-ns"), $typeTr.attr("data-type"));
        var $lastTrInGroup = $trsInGroup.filter(":last");
        var lastConditionIndex = $lastTrInGroup.attr("data-index") == undefined ? 0 : (parseInt($lastTrInGroup.attr("data-index")) + 1);
        $newTr.attr("data-index", lastConditionIndex);

        $newTr.find("td:first input[type='hidden']").remove();
        $newTr.find(".sf-auth-chooser input").each(function (i, e) {
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

        exports.multiSelRadios($newTr);
        $lastTrInGroup.after($newTr);
        $lastTrInGroup.find(".sf-auth-tree:eq(2)").removeClass().addClass("sf-auth-tree sf-auth-leaf");

        if ($typeTr.find(".sf-auth-available-conditions").val().split(",").length == $trsInGroup.length) {
            $typeTr.find(".sf-create").hide();
        }
    }
});
//# sourceMappingURL=AuthAdmin.js.map
