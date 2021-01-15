using System;

namespace TSensor.Web.Models.Controller
{
    public class LatestControllerCommand
    {
        public string Command { get; set; }
        public Guid CCGuid { get; set; }
        public int izkNumber { get; set; }
        
    }
}