using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NModbus;
using System.Net.Sockets;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
namespace IDC850_Simulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<IDC850Device> devices = new List<IDC850Device>();
        private int selectedDeviceId;
        public MainWindow()
        {
            InitializeComponent();
            cmbPorts.IsDropDownOpen = false;
            cmbAntennaId.IsDropDownOpen = false;
            cmbPorts.ItemsSource = devices;
            lstConnectedDevices.ItemsSource = devices;
            for (int i = 1; i <= 3; i++)
            {
                cmbAntennaId.Items.Add(i.ToString());
            }
        }

        private void btnPortCount_Click(object sender, RoutedEventArgs e)
        {
            int.TryParse(txtPortCount.Text, out int port);
            try
            {
                IDC850Device device = new IDC850Device(txtIpAddress.Text, port);
                if (!CheckExistIpPort(device, devices))
                {
                    devices.Add(device);
                    cmbPorts.Items.Refresh();
                    lstConnectedDevices.Items.Refresh();
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void cmbPorts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int.TryParse(cmbPorts.SelectedIndex.ToString(), out  selectedDeviceId);
        }
        private void WriteInputs()
        {
            int antenna = cmbAntennaId.SelectedIndex+1;
            try
            {
                devices[selectedDeviceId].WriteMultipleData(antenna, txtRouteId.Text, txtDriverId.Text, txtTrain1Id.Text, txtTrain2Id.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Write Exception: " + ex.Message);
            }
            
        }

        private void btnSendData_Click(object sender, RoutedEventArgs e)
        {
            WriteInputs();
        }

        private void ReadDataFromJson(string fileNamePath)
        {
            List<IDCData> data = new List<IDCData>();
            using (StreamReader r = new StreamReader(fileNamePath))
            {
                string json =r.ReadToEnd();
                if (json!=String.Empty)
                {
                    data = JsonConvert.DeserializeObject<List<IDCData>>(json);  
                }
            }
            for(int i = 0; i < data.Count; i++)
            {
                //Thread.Sleep(1000);
                if(data[i] != null) { 
                try {
                    IDC850Device dev = new IDC850Device(data[i].Ip, data[i].Port);
                    if (!CheckExistIpPort(dev, devices))
                    {
                        devices.Add(dev);
                    }
                    try
                    {
                       dev.WriteMultipleData(data[i].AntennaId, data[i].RouteId.ToString(), data[i].DriverId.ToString(), data[i].TrainID1.ToString(), data[i].TrainID2.ToString());
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Write Exception:"+ex.Message);
                    }
                   
                }catch(Exception ex)
                {
                    MessageBox.Show("Device Exception:"+ex.Message);
                }
                cmbPorts.Items.Refresh();
                lstConnectedDevices.Items.Refresh();
                }
            }
        }

        private void lstConnectedDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IDC850Device? selectedDevice = lstConnectedDevices.SelectedItem as IDC850Device;
            if (selectedDevice != null)
            {
                txtDeviceInfoIp.Text = selectedDevice.ip;
                txtDeviceInfoPort.Text = selectedDevice.port.ToString();
            }

        }

        private bool CheckExistIpPort(IDC850Device Adddevice,List<IDC850Device> deviceList)
        {
            for(int i = 0; i < deviceList.Count; i++)
            {
                if(Adddevice.ip==deviceList[i].ip && Adddevice.port == deviceList[i].port)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnReadJson_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
            openFileDlg.Filter = "Json Files (.json)|*.json";

            Nullable<bool> result = openFileDlg.ShowDialog();
            if(result == true)
            {
                devices.Clear();
                cmbPorts.Items.Refresh();
                lstConnectedDevices.Items.Refresh();
                ReadDataFromJson(openFileDlg.FileName);
            }
        }

    }
}
