using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface ITankRepository
    {
        public IEnumerable<Tank> GetListByPoint(Guid pointGuid);
        public Tank GetByGuid(Guid tankGuid);
        public Guid? Create(Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, int? mainIZKId, int? mainSensorId,
            string secondDeviceGuid, int? secondIZKId, int? secondSensorId, string description,
            decimal? weightChangeDelta, int? weightChangeTimeout);
        public bool Edit(Guid tankGuid, Guid pointGuid, string name, Guid? productGuid, bool dualMode,
            string mainDeviceGuid, int? mainIZKId, int? mainSensorId,
            string secondDeviceGuid, int? secondIZKId, int? secondSensorId, string description,
            decimal? weightChangeDelta, int? weightChangeTimeout);
        public bool Remove(Guid tankGuid, Guid pointGuid);

        public dynamic GetTankActualSensorValues(Guid tankGuid);
        public IEnumerable<ActualSensorValue> GetTankSensorValuesHistory(Guid tankGuid, DateTime dateStart, DateTime dateEnd);

        public IEnumerable<dynamic> GetTankWithoutSensorList();
        public bool SetSensorValue(bool isSecondSensor, Guid tankGuid, string deviceGuid, int izkId, int sensorId);

        #region CalibrationData
        public bool HasTankCalibrationData(Guid tankGuid);
        public Dictionary<int, decimal> GetTankCalibrationData(Guid tankGuid);
        public bool UploadTankCalibrationData(Guid tankGuid, Dictionary<int, decimal> data);
        #endregion
    }
}
