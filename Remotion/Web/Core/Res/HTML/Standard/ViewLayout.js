﻿function ViewLayout() {
}

ViewLayout.AdjustWidth = function(contentElement) {
    var children = contentElement.children('div');
    children.each(function(i) {
        $(this).css('position', 'absolute');
        $(this).css('left', '0');
        $(this).css('right', '0');
    });
};

ViewLayout.AdjustHeight = function(contentElement) {
    var margin = contentElement.outerHeight(true) - contentElement.innerHeight();
    contentElement.height(contentElement.parent().height() - margin);
};

ViewLayout.AdjustTop = function(topSibling, elementToAdjust) {
    $(elementToAdjust).css('top', topSibling.position().top + topSibling.outerHeight(true));
};

ViewLayout.AdjustBottom = function(bottomSibling, elementToAdjust) {
    $(elementToAdjust).css('bottom', bottomSibling.parent().height() - bottomSibling.position().top);
};

ViewLayout.AdjustSingleView = function(containerElement) {
    var contentElement = containerElement.children('div:first');

    ViewLayout.AdjustHeight(contentElement);
    ViewLayout.AdjustWidth(contentElement);

    var top = contentElement.children('div:eq(0)');
    var view = contentElement.children('div:eq(1)');
    var bottom = contentElement.children('div:eq(2)');

    top.css('top', '0');
    bottom.css('bottom', '0');
    ViewLayout.AdjustTop(top, view);
    ViewLayout.AdjustBottom(bottom, view);

    ViewLayout.FixIE6(view, bottom);
};

ViewLayout.AdjustTabbedMultiView = function(containerElement) {
    var contentElement = containerElement.children('div:first');

    ViewLayout.AdjustHeight(contentElement);
    ViewLayout.AdjustWidth(contentElement);

    var top = contentElement.children('div:eq(0)');
    var tabs = contentElement.children('div:eq(1)');
    var view = contentElement.children('div:eq(2)');
    var bottom = contentElement.children('div:eq(3)');

    top.css('top', '0');
    bottom.css('bottom', '0');
    ViewLayout.AdjustTop(top, tabs);
    ViewLayout.AdjustTop(tabs, view);
    ViewLayout.AdjustBottom(bottom, view);

    ViewLayout.FixIE6(view, bottom);
};

ViewLayout.FixIE6 = function(view, bottom) {

    if (!jQuery.browser.msie || parseInt(jQuery.browser.version) > 6)
        return;

    view.height(bottom.offset().top - view.offset().top);

    // does not change style, but solves the problem that certain elements didn't show in standard mode
    $('.tabStripTabSeparator').css('display', 'none');
    $('.DropDownMenuContainer').css('display', 'block');
    $('.bocListNavigator').css('display', 'block');
    $('div.bocReferenceValueContent').css('display', 'block');
    $('div.bocAutoCompleteReferenceValueContent').css('display', 'block');
    $('span.bocDateTimeValue').css('display', 'inline-block');
    $('a.DatePickerButton').css('display', 'inline-block');
}
