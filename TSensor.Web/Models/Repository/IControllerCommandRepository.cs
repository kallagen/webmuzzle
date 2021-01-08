using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public interface IControllerCommandRepository
    {
        public ControllerCommand GetLastCommand();
        public IEnumerable<ControllerCommand> GetAllCommand();
        public Guid? UploadCommand(string command);
    }
}