var editorGetHtml = function () {
    return tinymce.activeEditor.getContent();
}

var htmlContent;

var editorSetHtml = function (html) {
    if (tinymce.activeEditor) {
        tinymce.activeEditor.setContent(html);
    }
    else {
        // If not initialized yet, we need to delay it
        htmlContent = html;
    }
}

tinymce.init({
    selector: '#txtContent',
    plugins: [
        "advlist autolink lists link image charmap print preview anchor",
        "searchreplace visualblocks code fullscreen textcolor imagetools",
        "insertdatetime media table contextmenu paste sh4tinymce filemanager"
    ],
    toolbar: "styleselect | bold underline italic | alignleft aligncenter alignright | bullist numlist | forecolor backcolor | link media sh4tinymce | fullscreen code | filemanager",
    style_formats: [
        {
            title: 'Headers', items: [
             { title: 'Header 1', format: 'h1' },
             { title: 'Header 2', format: 'h2' },
             { title: 'Header 3', format: 'h3' },
             { title: 'Header 4', format: 'h4' },
             { title: 'Header 5', format: 'h5' },
             { title: 'Header 6', format: 'h6' }
            ]
        },
        {
            title: 'Inline', items: [
             { title: 'Bold', icon: 'bold', format: 'bold' },
             { title: 'Italic', icon: 'italic', format: 'italic' },
             { title: 'Underline', icon: 'underline', format: 'underline' },
             { title: 'Strikethrough', icon: 'strikethrough', format: 'strikethrough' },
             { title: 'Superscript', icon: 'superscript', format: 'superscript' },
             { title: 'Subscript', icon: 'subscript', format: 'subscript' },
             { title: 'Code', icon: 'code', format: 'code' }
            ]
        },
        {
            title: 'Blocks', items: [
             { title: 'Paragraph', format: 'p' },
             { title: 'Blockquote', format: 'blockquote' },
             { title: 'Div', format: 'div' },
             { title: 'Pre', format: 'pre' }
            ]
        },
        {
            title: 'Alignment', items: [
             { title: 'Left', icon: 'alignleft', format: 'alignleft' },
             { title: 'Center', icon: 'aligncenter', format: 'aligncenter' },
             { title: 'Right', icon: 'alignright', format: 'alignright' },
             { title: 'Justify', icon: 'alignjustify', format: 'alignjustify' }
            ]
        },
        { title: 'Inline Code', icon: 'code', inline: 'span', classes: 'code-segment' }
    ],
    autosave_ask_before_unload: false,
    max_height: 1000,
    min_height: 300,
    height: 500,
    menubar: false,
    relative_urls: false,
    browser_spellcheck: true,
    paste_data_images: true,
    setup: function (editor) {
        editor.on('init', function (e) {
            if (htmlContent) {
                editor.setContent(htmlContent);
            }
        });
    }
});
