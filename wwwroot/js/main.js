var Modal = function (type, header, message) {
    $(
        '<div class="modal fade" tabindex="-1"><div class="modal-dialog"><div class="modal-content">' +
        '<div class="modal-header ' + type + '"><h4 class="modal-title">' + header + '</h4></div>' +
        '<div class="modal-body"><p>' + message + '</p></div>' +
        '<div class="modal-footer"><button type="button" class="btn btn-default default-button" data-dismiss="modal">Закрыть</button></div>' +
        '</div></div></div>').appendTo($('body'));

    this.show = function () {
        $('.modal').modal();
    };

    return this;
};

var ErrorModal = function (message) {
    return new Modal('alert-error', 'Ошибка', message);
};

var SuccessModal = function (message) {
    return new Modal('alert-success', 'Успешно', message);
};

var ConfirmModal = function (message, callback) {
    function generateId() {
        return Math.random().toString().substr(2, 8);
    };

    var modalId = generateId();
    var successId = generateId();
    $(
        '<div id="modal' + modalId + '" class="modal fade" tabindex="-1"><div class="modal-dialog"><div class="modal-content">' +
        '<div class="modal-header alert-info"><h4 class="modal-title">Подтверждение</h4></div>' +
        '<div class="modal-body"><p>' + message + '</p></div>' +
        '<div class="modal-footer"><button type="button" class="btn btn-default pull-left default-button" data-dismiss="modal">Отмена</button>' +
        '<button id="success' + successId + '" type="button" class="btn btn-default bg-blue default-button" data-dismiss="modal">OK</button></div>' +
        '</div></div></div>').appendTo($('body'));

    this.show = function () {
        $('#modal' + modalId).modal();
    };

    $('#success' + successId).click(function () {
        if (callback) {
            callback();
        }
    });

    return this;
};