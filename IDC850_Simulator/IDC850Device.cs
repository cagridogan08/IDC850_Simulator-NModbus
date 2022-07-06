using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NModbus;
using System.Net.Sockets;
namespace IDC850_Simulator
{
    public class IDC850Device
    {
        ModbusFactory? factory = null;
        TcpClient? client = null;
        IModbusMaster? master = null;
        public byte deviceId=1;
        public string ip { get; set; }
        public int port { get; set; }
        public IDC850Device(string ip,int port)
        {
            this.ip = ip;
            this.port = port;
            client = new TcpClient(ip, port);
            factory = new ModbusFactory();
            master = factory.CreateMaster(client);
        }

        public void WriteRegisters(int SelectedAntenna,string data)
        {
            ushort startingRegister = 8;
            if(SelectedAntenna == 2)
            {
                startingRegister = 22;
            }
            else if(SelectedAntenna == 3)
            {
                startingRegister = 36;
            }
            byte registerCount = 14;
            ushort [] Senddata = new ushort [registerCount];
            int.TryParse(data, out int input);
            for(int i = 0; i < registerCount; i++)
            {
                Senddata [i] = (ushort)input;
            }
            master.WriteMultipleRegisters(deviceId, startingRegister, Senddata);
        }
        public override string ToString()
        {
            return this.ip+":"+this.port.ToString();
        }
        public void WriteMultipleData(int SelectedAntenna, string route,string driver,string train_1,string train_2)
        {
            ushort startingRegisterRoute = 17;
           
            if (SelectedAntenna == 2)
            {
                startingRegisterRoute = 31;
            }
            else if (SelectedAntenna == 3)
            {
                startingRegisterRoute = 45;
            }
            byte dataSize = 4;
            ushort [] Senddata = new ushort [dataSize];
            int.TryParse(route, out int input);
            int.TryParse(driver, out int input_2);
            int.TryParse(train_1, out int input_3);
            int.TryParse (train_2, out int input_4);
            Senddata[0] = (ushort)input;
            Senddata[1] = (ushort)input_2;
            Senddata[2] = (ushort)(input_3);
            Senddata[3] = (ushort)(input_4);
            //master.ReadWriteMultipleRegisters(SlaveId, startingRegisterRoute, startingRegisterRoute,startingRegisterRoute, Senddata);
            if (master != null)
            {
                master.WriteMultipleRegisters(deviceId, startingRegisterRoute, Senddata);
            }
        }
    }
}
