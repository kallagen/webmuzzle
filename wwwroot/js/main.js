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

function sensorUpdate(data, date) {
    for (sensorGuid in listenSensors) {
        var container = $('.t-cell[data-sensorguid="' + sensorGuid + '"],.t-cell[data-secondsensorguid="' + sensorGuid + '"]');
        var point = $('i[data-sensorguid="' + sensorGuid + '"]');

        if (data && data[sensorGuid] && data[sensorGuid].insertDate !== listenSensors[sensorGuid].updateDate) {

            var val = data[sensorGuid];

            if (point.hasClass('text-red')) {
                point.removeClass('text-red');
            }
            if (point.hasClass('text-yellow')) {
                point.removeClass('text-yellow');
            }

            if (container.hasClass('t-error') && !container.find('.text-red').length) {
                container.removeClass('t-error');
            }
            if (container.hasClass('t-warning') && !container.find('.text-yellow').length) {
                container.removeClass('t-warning');
            }

            container.find('span[data-sensorguid="' + sensorGuid + '"]').html(val.insertDateStr);

            if (listenSensors[sensorGuid].isSecond === 0 || listenSensors[sensorGuid].isSecond === -1) {
                container.find('.t-liquidEnvironmentLevel').html(val.liquidEnvironmentLevel);
                container.find('.t-environmentVolume').html(val.environmentVolume);
                container.find('.t-liquidDensity').html(val.liquidDensity);
                container.find('.t-avgT').html(val.avgT);
                container.find('.t-environmentLevel').html(val.environmentLevel);
            }

            if (listenSensors[sensorGuid].isSecond === 1 || listenSensors[sensorGuid].isSecond === -1) {
                container.find('.t-t1').html(val.t1.toFixed(1).replace('.', ','));
                container.find('.t-t2').html(val.t2.toFixed(1).replace('.', ','));
                container.find('.t-t3').html(val.t3.toFixed(1).replace('.', ','));
                container.find('.t-t4').html(val.t4.toFixed(1).replace('.', ','));
                container.find('.t-t5').html(val.t5.toFixed(1).replace('.', ','));
                container.find('.t-t6').html(val.t6.toFixed(1).replace('.', ','));
                container.find('.t-avgT').html(val.avgT.toFixed(1).replace('.', ','));
            }

            listenSensors[sensorGuid].updateDate = data[sensorGuid].insertDate;
            listenSensors[sensorGuid].warningDate = date + 600000;
        }

        if (!point.hasClass('text-yellow') && date > listenSensors[sensorGuid].warningDate) {
            point.addClass('text-yellow');

            if (!container.hasClass('t-error') && !container.hasClass('t-warning')) {
                container.addClass('t-warning');
            }
        }

        if ($('.text-red').length) {
            if (errorBlock.hasClass('hidden')) {
                errorBlock.removeClass('hidden');
            }
        } else {
            if (!errorBlock.hasClass('hidden')) {
                errorBlock.addClass('hidden');
            }
        }

        if ($('.text-yellow').length) {
            if (warningBlock.hasClass('hidden')) {
                warningBlock.removeClass('hidden');
            }
        }
        else {
            if (!warningBlock.hasClass('hidden')) {
                warningBlock.addClass('hidden');
            }
        }
    }
}

function notAssignedSensorUpdate(data) {
    for (sensorGuid in listenSensors) {
        var container = $('.t-cell[data-sensorguid="' + sensorGuid + '"]');

        if (data && data[sensorGuid] && data[sensorGuid].insertDate !== listenSensors[sensorGuid]) {

            var val = data[sensorGuid];

            container.find('.t-date').html(val.insertDateStr);
            container.find('.t-liquidEnvironmentLevel').html(val.liquidEnvironmentLevel);
            container.find('.t-environmentVolume').html(val.environmentVolume);
            container.find('.t-liquidDensity').html(val.liquidDensity);
            container.find('.t-avgT').html(val.avgT);
            container.find('.t-environmentLevel').html(val.environmentLevel);

            listenSensors[sensorGuid] = data[sensorGuid].insertDate;
        }
    }
}
