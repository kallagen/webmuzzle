using System;
using System.Collections.Generic;
using TSensor.Web.Models.Entity;

namespace TSensor.Web.Models.Repository
{
    public class ControllerCommandRepository: RepositoryBase, IControllerCommandRepository
    {
        public ControllerCommandRepository(string connectionString) : base(connectionString){ }

        public ControllerCommand GetLastCommand()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<ControllerCommand> GetAllCommand()
        {
            throw new System.NotImplementedException();
        }
        
        public Guid? UploadCommand(string command)
        {
            var date = DateTime.Now;
            
            return QueryFirst<Guid?>(@"
            DECLARE @guid UNIQUEIDENTIFIER = NEWID()
    
            INSERT INTO ControllerCommands
            (Command, [Date], State, CCGuid)
            VALUES(@command, @date, 0, @guid);
            
            SELECT CCGuid FROM ControllerCommands WHERE CCGuid = @guid", 
                new {command, date}
            );
            
        }
    }
}