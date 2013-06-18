
$(document).ready(function () {
    Init();
});


function Init() {
    //console.log("Init");

    // initialize elements
    $('.tabs').tabs();
    $('.ui-accordion').accordion({ autoHeight: false, height: 600 });
    $('.ui-button').button();
    $('.datePicker').datepicker();

    // initialize events


    if (window.location.hash != '') {

        var key = window.location.hash.replace('#', '');

        $('#key').html(key);
        $('#KeyToken').val(key);
        $('#KeyTokenIdentifier').val(key);

        //$('#send-messages').dialog({
        //    width: 500, title: "Send Secure Message/File to " + key,
        //    close: function () {
        //        //$('#send-messages').show();
        //    }
        //});
        $('#instructions').show('slow');
    }

    // Initialize File Uplaod

    var bar = $('.bar');
    var percent = $('.percent');
    var status = $('#SendMessageStatus');

    $('#SendMessageForm').ajaxForm({

        beforeSend: function () {
            status.empty();
            var percentVal = '0%';
            bar.width(percentVal);
            percent.html(percentVal);
        },
        uploadProgress: function (event, position, total, percentComplete) {
            var percentVal = percentComplete + '%';
            bar.width(percentVal);
            percent.html(percentVal);
        },
        complete: function (xhr) {

            status.html(xhr.responseText);
            $('#MessageText').val('');
            status.dialog({ title: "Send Message Request" });
        },
        error: function (xhr) {
            OnError(xhr.responseText);
        }
    });
}

function ShowMoreOptions() {

    $('#AdvancedOptions').toggle();
    
    if ($('#AdvancedOptions').is(":visible")) {
        $('#btnMore span').text('(less)');
    } else {
        $('#btnMore span').text('(more)');
    }
}


function CreateNewRequest() {
    //console.log("CreateRequest");

    var params = { releaseDate: $('#releaseDate').val() };

    $.post('/Service/Create', params, function (data) {

        $('#response').text($(data).find('response').text());

        $('#token').val($(data).find('KeyToken').text());

    });

    return false;
}




function OnError(error) {

    if (error == "" || error.responseText == undefined) {
        // don't show empty error
        error.responseText == "Unknown error!  Please report this as a bug."
    } else {
        $('#ErrorDialog').html(error.responseText);
        $('#ErrorDialog').dialog({ minWidth: 500, title: "Error" });
    }
}

function OnSendMessage() {
    $('#MessageText').val('Success');
    $('#SendMessageResult').dialog({ title: "Message/File Sent" });
}

function ShowKeyDetailWindow(parameters) {
    $('#KeyDetails').dialog({ minWidth: 500, title: "Key Created" });
}

function ShowMyMessages(data, XMLHttpRequest, ajaxOptions) {

    $('#messageDetails').html(data);
    $('#messageDetails').dialog({ minWidth: 500, title: "Messages" });

    //if (ajaxOptions.getResponseHeader("content-disposition") == null)
    //{
    //    // if no file was sent
    //    $('#messageDetails').html(data);
    //    $('#messageDetails').dialog({ minWidth: 500 });
    //}
}

function GetFile(fileId) {
    post('/Home/GetFile/', { fileId: fileId });
}

// for self-destructing messages
function GetAttachment(fileId) {
    post('/SelfDestruct/GetAttachment/', { fileId: fileId });
}


// Post to the provided URL with the specified parameters.
function post(path, parameters) {
    var form = $('<form></form>');

    form.attr("method", "post");
    form.attr("action", path);

    $.each(parameters, function (key, value) {
        var field = $('<input></input>');

        field.attr("type", "hidden");
        field.attr("name", key);
        field.attr("value", value);

        form.append(field);
    });

    // The form needs to be apart of the document in
    // order for us to be able to submit it.
    $(document.body).append(form);
    form.submit();
}


function ShowMessageSent(response) {

    $('#ErrorDialog').html(response);
    $('#ErrorDialog').dialog({ title: "Message Sent" });
}
