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

var InformModal = function (message) {
    return new Modal('alert-info', 'Информация', message);
}

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

var TogglePasswordButton = function (selector) {
    var TogglePasswordControl = function (el) {
        var that = this;
        that.el = $(el);
        that.parentName = that.el.data('for');
        that.parent = $('input[name="' + that.parentName + '"]');

        that.state = 'hidden';

        that.el.click(function () {
            if (that.state === 'hidden') {
                that.parent.prop('type', 'text');
                that.state = 'visible';
            } else {
                that.parent.attr('type', 'password');
                that.state = 'hidden';
            }

            that.el.toggleClass('glyphicon glyphicon-eye-open');
            that.el.toggleClass('glyphicon glyphicon-eye-close');
        });
    };

    $(selector).each(function (i, el) {
        new TogglePasswordControl(el);
    });
};

var DatePicker = function (selector, options) {
    var that = this;

    options = options || {};

    var format = options.allowTime === true ? 'DD.MM.YYYY HH:mm' : 'DD.MM.YYYY';

    this._control = new Pikaday({
        field: document.getElementById(selector),
        firstDay: 1,
        yearRange: [2016, 2026],
        format: format,
        i18n: {
            previousMonth: 'Previous Month',
            nextMonth: 'Next Month',
            months: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
            weekdays: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
            weekdaysShort: ['вс', 'пн', 'вт', 'ср', 'чт', 'пт', 'сб']
        },
        theme: 'as-pikadate-theme',
        onSelect: function (e) {
            if (options.onSelectEvent && that.initFinished) {
                options.onSelectEvent(e);
            }
        }
    });

    if (options.defaultDate) {
        this.initFinished = false;
        this._control.setMoment(moment(options.defaultDate, format));
    }

    this.initFinished = true;
};