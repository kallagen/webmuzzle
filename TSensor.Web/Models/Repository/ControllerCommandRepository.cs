using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TSensor.Web.Models.Controller;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ControllerCommandRepository: RepositoryBase, IControllerCommandRepository
    {
        public ControllerCommandRepository(string connectionString) : base(connectionString){ }

        public Task<LatestControllerCommand> GetLastCommand(string deviceGuid)
        {
            return QueryFirstAsync<LatestControllerCommand>(
                @"SELECT TOP 1 Command, CCGuid, izkNumber
                FROM ControllerCommands
                WHERE State = 0 AND DeviceGuid = @deviceGuid
                order by Date DESC", new {deviceGuid});
        }

        public Task<ControllerCommand> SetFailReason(string commandGuid, string failReason)
        {
            return QueryFirstAsync<ControllerCommand>(
                @"
                UPDATE TSensor.dbo.ControllerCommands
                SET State=2, FailReason=@failReason
                WHERE CCGuid = @commandGuid;

                SELECT TOP 1 Command, Date, State, FailReason
                FROM ControllerCommands
                WHERE CCGuid = @commandGuid AND State = 2",
                new {failReason, commandGuid });
        }
        
        public Task<ControllerCommand> SetCompleteState(string commandGuid)
        {
            return QueryFirstAsync<ControllerCommand>(
                @"
                UPDATE TSensor.dbo.ControllerCommands
                SET State=1
                WHERE CCGuid = @commandGuid;
                
                SELECT TOP 1 Command, Date, State, FailReason
                FROM ControllerCommands
                WHERE CCGuid = @commandGuid AND State = 1",
                new { commandGuid });
        }

        public IEnumerable<ControllerCommand> GetAllCommand()
        {
            throw new System.NotImplementedException();
        }
        
        public Guid? UploadCommand(string command, string deviceGuid, int izkNumber)
        {
            var date = DateTime.Now;
            
            return QueryFirst<Guid?>(@"
            DECLARE @guid UNIQUEIDENTIFIER = NEWID()
    
            INSERT INTO ControllerCommands
            (Command, [Date], State, CCGuid, DeviceGuid, izkNumber)
            VALUES(@command, @date, 0, @guid, @deviceGuid, @izkNumber);
            
            SELECT CCGuid FROM ControllerCommands WHERE CCGuid = @guid", 
                new {command, date, deviceGuid, izkNumber}
            );
        }

        public Task<string> UploadDeviceStatus(string deviceGuid, string state)
        {
            var date = DateTime.Now;
            
            return QueryFirstAsync<string>(@"          
            UPDATE TSensor.dbo.ControllerSettings
            SET State=@state, DateChanged=@date
            WHERE DeviceGuid = @deviceGuid;

            
            SELECT DeviceGuid FROM ControllerSettings WHERE DeviceGuid = @deviceGuid", 
                new { state, date, deviceGuid }
            );
        }
    }
}