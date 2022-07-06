using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Newtonsoft.Json;
namespace IDC850_Simulator
{
    public class IDCData
    {
        public string Ip { get; set; }
        public int Port { get; set; }
        public int SlaveId { get; set; }
        public int AntennaId { get; set; }
        public int DriverId { get; set; }
        public int RouteId { get; set; }
        public int TrainID1 { get; set; }
        public int TrainID2 { get; set; }

    }
}
