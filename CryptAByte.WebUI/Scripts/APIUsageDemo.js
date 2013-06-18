$(document).ready(function () {
    ko.applyBindings(new KeyModel());

    $(document).ajaxError(onAjaxError);
});

function onAjaxError(jqXHR, textStatus, errorThrown) {
    $('#error').html(textStatus.responseText);
    $('#error').dialog({ width: 600 });
}

var KeyModel = function () {
    var self = this;

    self.passphrase = ko.observable();
    self.response = ko.observable();
    self.token = ko.observable();
    self.message = ko.observable();
    self.messages = ko.observableArray([]);

    this.validPassphrase = function () {
        return (this.passphrase() != undefined && this.passphrase().length > 0);
    };

    this.validMessage = function () {
        return (this.message() != undefined && this.message().length > 0);
    };

    this.ShowResponse = function (response) {

        self.response(JSON.stringify(response));
    };


    this.CreateKey = function () {
        $.ajax({
            type: "POST",
            url: "/Service",
            data: { passphrase: this.passphrase() },
        }).done(function (response) {
            
            self.token(response.KeyToken);
            self.ShowResponse(response);
        });
    };

    this.DeleteKey = function () {
        $.ajax({
            type: "DELETE",
            url: "/Service",
            data: { token: this.token(), passphrase: this.passphrase() },
        }).done(function (response) {
            self.ShowResponse(response);
        });
    };

    this.GetToken = function () {
        $.ajax({
            type: "GET",
            url: "/Service",
            data: { token: this.token() },
        }).done(function (response) {
            self.token(response.KeyToken);
            self.ShowResponse(response);
        });
    };

    this.SendMessage = function () {
        
        $.ajax({
            type: "PUT",
            url: "/Service",
            data: { token: this.token(), message: this.message() },
        }).done(function (response) {
            self.ShowResponse(response);
        });
    };

    this.GetMessages = function () {
        $.ajax({
            type: "GET",
            url: "/Service",
            data: { token: this.token(), passphrase: this.passphrase() },
        }).done(function (response) {
            self.ShowResponse(response);

            $(response).each(function (index, msg) {
                self.messages.push(msg);
            });
        });
    };
};
