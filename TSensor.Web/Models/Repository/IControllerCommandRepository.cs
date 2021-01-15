using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TSensor.Web.Models.Controller;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IControllerCommandRepository
    {
        public Task<LatestControllerCommand> GetLastCommand(string deviceGuid);
        public Task<ControllerCommand> SetFailReason(string commandGuid, string failReason);
        public Task<ControllerCommand> SetCompleteState(string commandGuid);

        public IEnumerable<ControllerCommand> GetAllCommand();
        public Guid? UploadCommand(string command, string deviceGuid, int izkNumber);

       
        public Task<string> UploadDeviceStatus(string deviceGuid, string state);
    }
}