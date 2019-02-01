using GrovePi;
using GrovePi.Sensors;
using InfluxDB.LineProtocol.Client;
using InfluxDB.LineProtocol.Payload;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using PubnubApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Security.Cryptography;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RaspberryPi
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static HubConnection _connection;

        private static readonly HttpClient HttpClient = new HttpClient();
        private const string Url = "https://aubergine.site";

        // Sensor Pins
        private readonly IUltrasonicRangerSensor
            _distSensorA = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin2); // Ultrasonic Sensor A  — Pin D2

        private readonly IUltrasonicRangerSensor
            _distSensorB = DeviceFactory.Build.UltraSonicSensor(Pin.DigitalPin3); // Ultrasonic Sensor B  — Pin D3

        private readonly ILed _redLed = DeviceFactory.Build.Led(Pin.DigitalPin6); // Red LED              — Pin D6
        private readonly ILed _greenLed = DeviceFactory.Build.Led(Pin.DigitalPin7); // Green LED            — Pin D7

        private readonly IButtonSensor
            _requestButton = DeviceFactory.Build.ButtonSensor(Pin.DigitalPin4); // Button               — Pin D4

        private readonly IGasSensorMQ2
            _gasSensor = DeviceFactory.Build.GasSensorMQ2(Pin.AnalogPin0); // Gas Sensor           — Pin A0

        private readonly IRelay _relay = DeviceFactory.Build.Relay(Pin.DigitalPin5); // Relay                — Pin D5

        private static readonly IBuzzer
            Buzzer = DeviceFactory.Build.Buzzer(Pin.DigitalPin8); // Buzzer               — Pin D8

        private DateTime _distATriggered = DateTime.Now.AddDays(-1);
        private DateTime _distBTriggered = DateTime.Now.AddDays(-1);

        private bool _peoplePresent;
        private int _currentUsers;
        private static int _totalUsers;
        private static int _cleaningRequests;
        private int _distAValue;
        private int _distBValue;
        private int _gasValue;
        private string _fanMode;
        private int _fanThreshold;
        private int _updateFrequency;

        private static SerialComms _uartComms;
        private static string _detectedRfidString = "";

        private readonly Pubnub _pubNub = new Pubnub(new PNConfiguration
        {
            PublishKey = "[REDACTED]",
            SubscribeKey = "[REDACTED]",
            Uuid = "RPi_01",
            Secure = true
        });

        private static int _toiletId;
        private string _rpiSensorId;

        private static DateTime _rfidRead;
        private static DateTime _timeRead;
        private static bool _updateTimeRead;
        private static string _rfid;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var id = SystemIdentification.GetSystemIdForPublisher().Id;
            _rpiSensorId = CryptographicBuffer.EncodeToBase64String(id);

            var deviceInfo = new EasClientDeviceInformation();
            var friendlyName = deviceInfo.FriendlyName;

            // Get the toilet settings for the specific toilet.
            ToiletSettings toiletSettings = GetToiletSettings(_rpiSensorId, friendlyName);

            _toiletId = toiletSettings.ToiletId;
            _updateFrequency = toiletSettings.UpdateFrequency;
            _fanMode = toiletSettings.FanMode;
            _fanThreshold = toiletSettings.FanThreshold;

            _connection = new HubConnectionBuilder()
                .WithUrl("[REDACTED]")
                .Build();

            _connection.Closed += async error =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };

            Connect();

            StartUart();

            Task.Run(new Action(SendDataToPubHub));
            Task.Run(new Action(UploadSensorValuesToInfluxDb));
            while (true)
            {
                try
                {
                    // Read the sensors values.
                    ReadDistanceSensors();
                    ProcessDistanceReadings();
                    CheckButtonPressed();
                    ReadGasValue();
                    ControlFan();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("An error occurred while reading the sensors values: " + ex.Message);
                }
            }
        }

        #region helpers

        private static ToiletSettings GetToiletSettings(string sensorId, string friendlyName)
        {
            var toiletSettings = new ToiletSettings();
            var uri = new Uri(
                $"{Url}/api/ToiletSettings/GetToiletSettings?sensorId={System.Web.HttpUtility.UrlEncode(sensorId)}&name={friendlyName}");
            Debug.WriteLine($"Attempting to fetch toilet settings from {uri.AbsoluteUri}");
            try
            {
                var response = HttpClient.GetStringAsync(uri).Result;
                Debug.WriteLine($"Response from URL:\n{response}");
                toiletSettings = JsonConvert.DeserializeObject<ToiletSettings>(response);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
            }

            return toiletSettings;
        }

        private async void Connect()
        {
            _connection.On<int, string, int, int, int, int>("ReceiveMessage",
                (updateFrequency, fanMode, fanThreshold, userThreshold, gasValueThreshold, requestThreshold) =>
                {
                    _updateFrequency = updateFrequency;
                    _fanMode = fanMode;
                    _fanThreshold = fanThreshold;
                    Debug.WriteLine("Toilet settings updated");
                });

            try
            {
                await _connection.StartAsync();
                Debug.WriteLine("Connection started");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private static void StartUart()
        {
            _uartComms = new SerialComms();
            _uartComms.UartEvent += UartDataHandler;
        }

        private static async void UartDataHandler(object sender, SerialComms.UartEventArgs e)
        {
            _detectedRfidString = e.Data;
            _rfidRead = DateTime.Now;

            if (!_updateTimeRead)
            {
                _timeRead = DateTime.Now;
                _updateTimeRead = true;
                await SoundBuzzer();
                Debug.WriteLine(
                    $"Clock-in detected - Date Clocked: {_timeRead:MMM dd, yyyy} Time Clocked: {_timeRead:h:mm tt}");

                _rfid = _detectedRfidString.Substring(1);
                bool result = await UpdateCleanerStatus(_rfid);
                if (result)
                {
                    Debug.WriteLine("Updated cleaner's status successfully.");
                    await UpdateTable();
                }
                else
                {
                    Debug.WriteLine("Unable to update cleaner's status. Please check your code.");
                }
            }
            else
            {
                if (_timeRead.AddSeconds(5) > _rfidRead)
                {
                    Debug.WriteLine("Please wait at least five minutes before tapping your RFID card again.");
                }
                else
                {
                    await SoundBuzzer();
                    Debug.WriteLine(
                        $"Clock-out detected - Date Clocked: {_rfidRead:MMM dd, yyyy} Time Clocked: {_rfidRead:h:mm tt}");

                    _rfidRead = DateTime.Now;
                    var duration = (_rfidRead - _timeRead).TotalSeconds;
                    var toiletLog = new CustomModel
                    {
                        ToiletId = _toiletId,
                        Rfid = _rfid,
                        StartDate = _timeRead,
                        EndDate = _rfidRead,
                        Duration = (int) duration
                    };

                    Task<Uri> createToiletLogTask = CreateToiletLogAsync(toiletLog);
                    Task<bool> updateCleanerStatusAndCleanedCountTask = UpdateCleanerStatusAndCleanedCountAsync(_rfid);
                    Task<bool> updateToiletLastCleanedTask = UpdateToiletLastCleanedAsync(_toiletId, _rfidRead);

                    await createToiletLogTask.ContinueWith(task =>
                    {
                        if (task == null)
                        {
                            Debug.WriteLine("Unable to create a toilet log. Please check your code.");
                        }
                        else
                        {
                            Debug.WriteLine("A toilet log has been created successfully.");
                            _cleaningRequests = 0;
                            _totalUsers = 0;
                            _updateTimeRead = false;
                        }
                    });

                    await updateCleanerStatusAndCleanedCountTask.ContinueWith(task =>
                    {
                        Debug.WriteLine(task.Result
                            ? "Updated cleaner's status and cleaned count successfully."
                            : "Unable to update cleaner's status and cleaned count. Please check your code.");
                    });

                    await updateToiletLastCleanedTask.ContinueWith(task =>
                    {
                        Debug.WriteLine(task.Result
                            ? "Updated toilet's last cleaned date successfully."
                            : "Unable to update toilet's last cleaned date. Please check your code.");
                    });

                    await UpdateTable();
                }
            }
        }

        private static async Task SoundBuzzer()
        {
            Buzzer.ChangeState(SensorStatus.On);
            await Task.Delay(400).ContinueWith(_ => Buzzer.ChangeState(SensorStatus.Off));
        }

        private static async Task<bool> UpdateCleanerStatus(string rfid)
        {
            HttpResponseMessage response = null;
            var uri = new Uri($"{Url}/api/Cleaners/UpdateCleanerStatus/{rfid}");
            Debug.WriteLine($"Attempting to update cleaner's status at {uri.AbsoluteUri}");
            try
            {
                HttpContent content = new StringContent(rfid, Encoding.UTF8, "application/json");
                response = await HttpClient.PatchAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
            }

            return response != null && response.IsSuccessStatusCode;
        }

        private static async Task UpdateTable()
        {
            #region snippet_ErrorHandling

            try
            {
                #region snippet_InvokeAsync

                await _connection.InvokeAsync("UpdateTable");
                Debug.WriteLine("Table updated on server-side");

                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            #endregion
        }

        private static async Task<Uri> CreateToiletLogAsync(CustomModel customModel)
        {
            HttpResponseMessage response = null;
            var uri = new Uri($"{Url}/api/ToiletLogs");
            Debug.WriteLine($"Attempting to create a toilet log at {uri.AbsoluteUri}");
            try
            {
                response = await HttpClient.PostAsJsonAsync(uri, customModel);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
            }

            // Return the URI of the created resource.
            return response?.Headers.Location;
        }

        private static async Task<bool> UpdateCleanerStatusAndCleanedCountAsync(string rfid)
        {
            HttpResponseMessage response = null;
            var uri = new Uri($"{Url}/api/Cleaners/{rfid}");
            Debug.WriteLine($"Attempting to update cleaner's status and cleaned count at {uri.AbsoluteUri}");
            try
            {
                HttpContent content = new StringContent(rfid, Encoding.UTF8, "application/json");
                response = await HttpClient.PatchAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
            }

            return response != null && response.IsSuccessStatusCode;
        }

        private static async Task<bool> UpdateToiletLastCleanedAsync(int id, DateTime lastCleaned)
        {
            HttpResponseMessage response = null;
            var uri = new Uri($"{Url}/api/Toilets");
            Debug.WriteLine($"Attempting to update toilet's last cleaned date at {uri.AbsoluteUri}");
            try
            {
                dynamic toilet = new ExpandoObject();
                toilet.id = id;
                toilet.lastCleaned = lastCleaned;
                HttpContent content = new StringContent(JsonConvert.SerializeObject(toilet), Encoding.UTF8,
                    "application/json");
                response = await HttpClient.PatchAsync(uri, content);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception: {e.Message}");
                Debug.WriteLine($"Stack Trace: {e.StackTrace}");
            }

            return response != null && response.IsSuccessStatusCode;
        }

        private void SendDataToPubHub()
        {
            while (true)
            {
                //Debug.WriteLine("Attempting to publish sensor values to PubNub");
                var wrapper = new
                {
                    type = "sensor_data",
                    sensor_values = new
                    {
                        gas_value = _gasValue,
                        current_users = _currentUsers,
                        total_users = _totalUsers,
                        requests = _cleaningRequests
                    }
                };
                _pubNub.Publish()
                    .Channel(_rpiSensorId)
                    .Message(JsonConvert.SerializeObject(wrapper))
                    .Async(new PNPublishResultExt(
                        (result, status) =>
                        {
                            //Debug.WriteLine(status.StatusCode == 200
                            //    ? "Successfully published sensor values to PubNub"
                            //    : $"Error publishing to PubNub: {status.StatusCode}, {status.Error}");
                        }
                    ));
                Task.Delay(_updateFrequency).Wait();
            }
        }

        private async void UploadSensorValuesToInfluxDb()
        {
            while (true)
            {
                var sensorValues = new LineProtocolPoint(
                    "1",
                    new Dictionary<string, object>
                    {
                        {"GasValue", _gasValue},
                        {"TotalUsers", _totalUsers},
                        {"CurrentUsers", _currentUsers},
                        {"Requests", _cleaningRequests}
                    },
                    new Dictionary<string, string>());

                var payload = new LineProtocolPayload();
                payload.Add(sensorValues);

                var client = new LineProtocolClient(new Uri("[REDACTED]"), "SensorValues",
                    "[REDACTED]", "[REDACTED]");
                await client.WriteAsync(payload);
                Task.Delay(60000).Wait();
            }
        }

        //Read values of Ultrasonic Sensors
        public void ReadDistanceSensors()
        {
            _distAValue = _distSensorA.MeasureInCentimeters();
            _distBValue = _distSensorB.MeasureInCentimeters();
        }

        //Process Ultrasonic Sensor readings and calculate people
        public void ProcessDistanceReadings()
        {
            if (_distAValue < 0 || _distBValue < 0) return;
            if (_distAValue < 30)
            {
                _distATriggered = DateTime.Now;
                _greenLed.ChangeState(SensorStatus.On);
                if (_distBTriggered > DateTime.Now.AddSeconds(-2) && !_peoplePresent)
                {
                    _currentUsers++;
                    _totalUsers++;
                    _peoplePresent = true;
                    _distATriggered = DateTime.Now.AddDays(-1);
                    _distBTriggered = DateTime.Now.AddDays(-1);
                }
            }
            else
            {
                _greenLed.ChangeState(SensorStatus.Off);
                _peoplePresent = false;
            }

            if (_distBValue < 30)
            {
                _distBTriggered = DateTime.Now;
                _redLed.ChangeState(SensorStatus.On);
                if (_distATriggered > DateTime.Now.AddSeconds(-2) && !_peoplePresent)
                {
                    _peoplePresent = true;
                    if (_currentUsers > 0)
                    {
                        _currentUsers--;
                    }

                    _distATriggered = DateTime.Now.AddDays(-1);
                    _distBTriggered = DateTime.Now.AddDays(-1);
                }
            }
            else
            {
                _redLed.ChangeState(SensorStatus.Off);
                _peoplePresent = false;
            }
        }

        //Check if User Feedback Button is pressed
        public void CheckButtonPressed()
        {
            if (_requestButton.CurrentState.ToString().Equals("On", StringComparison.OrdinalIgnoreCase))
            {
                _cleaningRequests++;
            }
        }

        //Read gas value from Gas Sensor
        private void ReadGasValue()
        {
            int tempGasValue = _gasSensor.SensorValue();

            if (tempGasValue >= 0 && tempGasValue <= 1023)
            {
                _gasValue = tempGasValue * 100 / 1023;
            }
            else
            {
                Debug.WriteLine("WARNING: Gas value ignored, out of range: " + tempGasValue);
            }
        }

        private void ControlFan()
        {
            switch (_fanMode)
            {
                case "Auto" when _gasValue > _fanThreshold:
                    _relay.ChangeState(SensorStatus.On);
                    break;
                case "Auto":
                {
                    if (_gasValue < _fanThreshold - 16)
                    {
                        _relay.ChangeState(SensorStatus.Off);
                    }

                    break;
                }
                case "On":
                    _relay.ChangeState(SensorStatus.On);
                    break;
                case "Off":
                    _relay.ChangeState(SensorStatus.Off);
                    break;
            }
        }

        #endregion
    }

    public sealed class ToiletSettings
    {
        public int ToiletId { get; set; }

        public int UpdateFrequency { get; set; }

        public string FanMode { get; set; }

        public int FanThreshold { get; set; }
    }

    public sealed class CustomModel
    {
        public int ToiletId { get; set; }

        public string Rfid { get; set; }

        public DateTimeOffset StartDate { get; set; }

        public DateTimeOffset EndDate { get; set; }

        public int Duration { get; set; }
    }
}