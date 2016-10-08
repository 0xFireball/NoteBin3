/* jshint esversion: 6 */
/*
    Copyright 2016 0xFireball. All Rights Reserved.

    https://github.com/0xFireball
    https://0xfireball.me

    This file is part of NoteBin3
*/

let codeMirrorOpts = {
    mode: "gfm",
    matchBrackets: true,
    theme: "3024-day",
    lineNumbers: true,
    scrollbarStyle: "simple",
    extraKeys: {
        "Alt-F": "findPersistent"
    },
    viewportMargin: Infinity
};

let previewEnabled = $("#show-preview-cb").prop("checked");

// Initialize CodeMirror editor
let mdEditor = CodeMirror.fromTextArea($("#md-editor")[0], codeMirrorOpts);


mdEditor.on("change", function(cm, change) {
    //Editor modified event
    localStorage.setItem("savedFile", mdEditor.getValue());
    if (previewEnabled) {
        updatePreview(mdEditor.getValue());
    }
    //send save request to server 
});

function restoreSavedSession() {
    let savedFileCont = localStorage.getItem("savedFile");
    if (savedFileCont) {
        mdEditor.setValue(savedFileCont);
    }
}

function updatePreview(inputMarkdown) {
    $("#rendered-md").html(marked(inputMarkdown, {
        sanitize: true
    }));
}

function bootstrapEditor() {
    //attempt to recover data
    restoreSavedSession();
}

bootstrapEditor();

$("#show-preview-cb").on("change", () => {
    previewEnabled = $("#show-preview-cb").prop("checked");
    if (previewEnabled) {
        $("#preview-container").show();
        $("#editor-container")
            .removeClass("l12")
            .addClass("l6");
        updatePreview(mdEditor.getValue());
    } else {
        $("#preview-container").hide();
        $("#editor-container")
            .removeClass("l6")
            .addClass("l12");
    }
});

$("#show-editor-cb").on("change", () => {
    let showEditor = $("#show-editor-cb").prop("checked");
    if (showEditor) {
        $("#preview-container")
            .removeClass("l12")
            .addClass("l6");
        $("#editor-container")
            .show();
    } else {
        $("#preview-container")
            .removeClass("l6")
            .addClass("l12");
        $("#editor-container")
            .hide();
    }
});