using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace RaspberryPi
{
    internal class SerialComms
    {
        private SerialDevice _serialPort;
        private DataReader _dataReaderObject;

        #region Public Event
        public delegate void UartEventDelegate(object sender, UartEventArgs e);

        public event UartEventDelegate UartEvent;

        public class UartEventArgs : EventArgs
        {
            public string Data { get; set; }
        }

        private void OnUartEvent(UartEventArgs e)
        {
            UartEvent?.Invoke(this, e);
        }
        #endregion

        public SerialComms()
        {
            InitSerial();
        }

        private async void InitSerial()
        {
            string aqs = SerialDevice.GetDeviceSelector("UART0");
            var myDevices = await DeviceInformation.FindAllAsync(aqs);
            if (myDevices.Count == 0)
            {
                Debug.WriteLine("ERROR - CANNOT Get rpi UART Info");
            }
            else
            {
                DeviceInformation entry = myDevices[0];
                try
                {
                    _serialPort = await SerialDevice.FromIdAsync(entry.Id);
                    if (_serialPort == null)
                    {
                        Debug.WriteLine("Serial Port is null");
                        return;
                    }

                    Debug.WriteLine(_serialPort.PortName + " is Ready");
                    _serialPort.WriteTimeout = TimeSpan.FromMilliseconds(200);
                    _serialPort.ReadTimeout = TimeSpan.FromMilliseconds(200);
                    _serialPort.BaudRate = 9600;
                    _serialPort.Parity = SerialParity.None;
                    _serialPort.StopBits = SerialStopBitCount.One;
                    _serialPort.DataBits = 8;
                    _serialPort.Handshake = SerialHandshake.None;

                    Listen();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception in InitSerial: " + ex.Message);
                }
            }
        }

        private async void Listen()
        {
            try
            {
                if (_serialPort != null)
                {
                    _dataReaderObject = new DataReader(_serialPort.InputStream);

                    while (true)
                    {
                        await ReadAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EXCEPTION in Listen()");
                Debug.WriteLine("EXCEPTION : " + ex.Message);
            }
            finally
            {
                // Cleanup once complete
                if (_dataReaderObject != null)
                {
                    _dataReaderObject.DetachStream();
                    _dataReaderObject = null;
                }
            }
        }

        private async Task ReadAsync()
        {
            const uint readBufferLength = 1024;

            _dataReaderObject.InputStreamOptions = InputStreamOptions.Partial;
            var loadAsyncTask = _dataReaderObject.LoadAsync(readBufferLength).AsTask();

            var bytesRead = await loadAsyncTask;
            if (bytesRead > 0)
            {
                string strMsgRec = _dataReaderObject.ReadString(bytesRead);
                strMsgRec = strMsgRec.Substring(0, 13);
                var args = new UartEventArgs {Data = strMsgRec};
                OnUartEvent(args);
            }
        }
    }
}
