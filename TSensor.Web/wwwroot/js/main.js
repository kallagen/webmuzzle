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
    listenSensors.forEach(function (sensor) {
        var sensorGuid = sensor.guid;

        var container = $('.t-cell[data-sensorguid="' + sensorGuid + '"],.t-cell[data-secondsensorguid="' + sensorGuid + '"]');
        var point = $('i[data-sensorguid="' + sensorGuid + '"]');

        if (data && data[sensorGuid] && data[sensorGuid].insertDate !== sensor.updateDate) {

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

            if (sensor.isSecond === 0) {
                container.find('.t-liquidEnvironmentLevel').html(val.liquidEnvironmentLevel);
                container.find('.t-environmentVolume').html(val.environmentVolume);
                container.find('.t-liquidDensity').html(val.liquidDensity);
                container.find('.t-avgT').html(val.avgT);
                container.find('.t-environmentLevel').html(val.environmentLevel);

                container.find('.tank-level').css('height', val.percentLevel + '%');

                if (val.liquidEnvironmentLevel > sensor.weight + sensor.weightDelta ||
                    val.liquidEnvironmentLevel < sensor.weight - sensor.weightDelta) {

                    if (val.liquidEnvironmentLevel > sensor.weight + sensor.weightDelta) {
                        container.find('.t-weightdown').addClass('hidden');
                        container.find('.t-weightup').removeClass('hidden');
                    }
                    if (val.liquidEnvironmentLevel < sensor.weight - sensor.weightDelta) {
                        container.find('.t-weightdown').removeClass('hidden');
                        container.find('.t-weightup').addClass('hidden');
                    }

                    sensor.weight = val.liquidEnvironmentLevel;
                    if (sensor.weightTimeout !== 0) {
                        sensor.weightExpire = date + sensor.weightTimeout * 1000;
                    }
                }
            }

            if (sensor.isSecond === 1) {
                container.find('.t-avgT').html(val.avgT.toFixed(1).replace('.', ','));
            }

            sensor.updateDate = data[sensorGuid].insertDate;
            sensor.warningDate = date + 600000;
        }

        if (!point.hasClass('text-yellow') && sensor.warningDate && date > sensor.warningDate) {
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

        if (sensor.weightExpire && sensor.weightExpire < date) {
            container.find('.t-weightdown').addClass('hidden');
            container.find('.t-weightup').addClass('hidden');
        }
    });
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

            container.find('.tank-level').css('height', val.percentLevel + '%');

            listenSensors[sensorGuid] = data[sensorGuid].insertDate;
        }
    }
}

function tankSensorUpdate(data) {
    listenSensors.forEach(function (sensor) {
        var sensorGuid = sensor.guid;
        if (data && data[sensorGuid] && data[sensorGuid].insertDate !== sensor.updateDate) {

            var val = data[sensorGuid];

            $('.izkNumber[data-sensorGuid="' + sensorGuid + '"]').html(val.izkNumber);
            $('.banderolType[data-sensorGuid="' + sensorGuid + '"]').html(val.banderolType);
            $('.sensorSerial[data-sensorGuid="' + sensorGuid + '"]').html(val.sensorSerial);
            $('.sensorChannel[data-sensorGuid="' + sensorGuid + '"]').html(val.sensorChannel);
            $('.pressureAndTempSensorState[data-sensorGuid="' + sensorGuid + '"]').html(val.pressureAndTempSensorState);
            $('.sensorFirmwareVersionAndReserv[data-sensorGuid="' + sensorGuid + '"]').html(val.sensorFirmwareVersionAndReserv);
            $('.alarma[data-sensorGuid="' + sensorGuid + '"]').html(val.alarma);
            $('.environmentLevel[data-sensorGuid="' + sensorGuid + '"]').html(val.environmentLevel);
            $('.pressureFilter[data-sensorGuid="' + sensorGuid + '"]').html(val.pressureFilter);
            $('.pressureMeasuring[data-sensorGuid="' + sensorGuid + '"]').html(val.pressureMeasuring);
            $('.levelInPercent[data-sensorGuid="' + sensorGuid + '"]').html(val.levelInPercent);
            $('.environmentVolume[data-sensorGuid="' + sensorGuid + '"]').html(val.environmentVolume);
            $('.liquidEnvironmentLevel[data-sensorGuid="' + sensorGuid + '"]').html(val.liquidEnvironmentLevel);
            $('.steamMass[data-sensorGuid="' + sensorGuid + '"]').html(val.steamMass);
            $('.liquidDensity[data-sensorGuid="' + sensorGuid + '"]').html(val.liquidDensity);
            $('.steamDensity[data-sensorGuid="' + sensorGuid + '"]').html(val.steamDensity);
            $('.dielectricPermeability[data-sensorGuid="' + sensorGuid + '"]').html(val.dielectricPermeability);
            $('.dielectricPermeability2[data-sensorGuid="' + sensorGuid + '"]').html(val.dielectricPermeability2);
            $('.t1[data-sensorGuid="' + sensorGuid + '"]').html(val.t1);
            $('.t2[data-sensorGuid="' + sensorGuid + '"]').html(val.t2);
            $('.t3[data-sensorGuid="' + sensorGuid + '"]').html(val.t3);
            $('.t4[data-sensorGuid="' + sensorGuid + '"]').html(val.t4);
            $('.t5[data-sensorGuid="' + sensorGuid + '"]').html(val.t5);
            $('.t6[data-sensorGuid="' + sensorGuid + '"]').html(val.t6);
            $('.plateTemp[data-sensorGuid="' + sensorGuid + '"]').html(val.plateTemp);
            $('.period[data-sensorGuid="' + sensorGuid + '"]').html(val.period);
            $('.plateServiceParam[data-sensorGuid="' + sensorGuid + '"]').html(val.plateServiceParam);
            $('.environmentComposition[data-sensorGuid="' + sensorGuid + '"]').html(val.environmentComposition);
            $('.cs1[data-sensorGuid="' + sensorGuid + '"]').html(val.cs1);
            $('.plateServiceParam2[data-sensorGuid="' + sensorGuid + '"]').html(val.plateServiceParam2);
            $('.plateServiceParam3[data-sensorGuid="' + sensorGuid + '"]').html(val.plateServiceParam3);
            $('.sensorWorkMode[data-sensorGuid="' + sensorGuid + '"]').html(val.sensorWorkMode);
            $('.plateServiceParam4[data-sensorGuid="' + sensorGuid + '"]').html(val.plateServiceParam4);
            $('.plateServiceParam5[data-sensorGuid="' + sensorGuid + '"]').html(val.plateServiceParam5);
            $('.crc[data-sensorGuid="' + sensorGuid + '"]').html(val.crc);

            sensor.updateDate = data[sensorGuid].insertDate;
        }
    });
}

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