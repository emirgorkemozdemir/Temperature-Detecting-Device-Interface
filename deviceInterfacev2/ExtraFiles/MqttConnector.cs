using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer.Concrete;
using deviceInterfacev2.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace deviceInterfacev2.ExtraFiles
{
    public class MqttConnector
    {
        DeviceBL device_manager = new DeviceBL();
        TMBL tm_manager = new TMBL();
        MqttClient mqttClient;
        public void MqttConnectionLoad()
        {
            Task.Run(() =>
            {
                mqttClient = new MqttClient("broker.emqx.io");
                mqttClient.MqttMsgPublishReceived += MqttClient_MqttMsgPublishReceived;
                mqttClient.Subscribe(new string[] { "pharmacy/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE });
                Random random = new Random();
                var random_code = random.Next(1000, 9999);
                mqttClient.Connect("Application1asasdfhga"+ random_code);
            });
        }

        public void sendRandomCodeToDevice(string deviceKey,string random_code)
        {
            Task.Run(() =>
            {
                mqttClient = new MqttClient("broker.emqx.io");
                Random random = new Random();
                var random_code2 = random.Next(1000, 9999);
                mqttClient.Connect("Application1asasdfhga" + random_code2);
                if (mqttClient.IsConnected)
                {
                    mqttClient.Publish($"pharmacy/{deviceKey}/link", Encoding.UTF8.GetBytes(random_code));
                }
                else
                {

                }
            });

        }

        private void MqttClient_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {

            var message = Encoding.UTF8.GetString(e.Message);
            string topic = e.Topic;

            List<string> topicParts = topic.Split('/').ToList(); // 1. pharmacy 2. unique key 3. tli değer

            string unique_device_key = topicParts[1].ToString(); //topic içinde unique key i al

            var selected_device = device_manager.findDevicebyUniqeKey(unique_device_key); // buraya unique_device_key atanacak id gelince

            var last_date = device_manager.getDeviceLastTM(selected_device.DeviceId);

            TableTm tm = new TableTm();
            tm.TmdeviceId = selected_device.DeviceId;

            if (topicParts[2]=="t1")
            {
                tm.Temp1 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "t2")
            {
                tm.Temp2 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "t3")
            {
                tm.Temp3 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "t4")
            {
                tm.Temp4 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "t5")
            {
                tm.Temp5 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "t6")
            {
                tm.Temp6 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "h1")
            {
                tm.Moisture1 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "h2")
            {
                tm.Moisture2 = float.Parse(message,
             System.Globalization.CultureInfo.InvariantCulture);
            }
            else if (topicParts[2] == "limit")
            {
                var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                selected_device.DeviceMaxMoisture = Convert.ToDouble(values["hh"]);
                selected_device.DeviceMinMoisture = Convert.ToDouble(values["hl"]);
                selected_device.DeviceMaxTemp = Convert.ToDouble(values["th"]);
                selected_device.DeviceMinTemp = Convert.ToDouble(values["tl"]);
                device_manager.Edit(selected_device);
               
            }

            var first_check = device_manager.GetTMValues(selected_device.DeviceId, topicParts[2]);

            if (last_date.Value.Hour != DateTime.Now.Hour)
            {
                tm_manager.Add(tm);
            }
            else if (first_check.Count() == 0)
            {
                tm_manager.Add(tm);
            }
            else if ((topicParts[2] == "t1" || topicParts[2] == "t2" || topicParts[2] == "t3" || topicParts[2] == "t4" || topicParts[2] == "t5" || topicParts[2] == "t6") && float.Parse(message, System.Globalization.CultureInfo.InvariantCulture) > selected_device.DeviceMaxTemp)
            {
                tm_manager.Add(tm);
            }
            else if ((topicParts[2] == "t1" || topicParts[2] == "t2" || topicParts[2] == "t3" || topicParts[2] == "t4" || topicParts[2] == "t5" || topicParts[2] == "t6") && float.Parse(message, System.Globalization.CultureInfo.InvariantCulture) < selected_device.DeviceMinTemp)
            {
                tm_manager.Add(tm);
            }
            else if ((topicParts[2] == "h1" || topicParts[2] == "h2") && float.Parse(message, System.Globalization.CultureInfo.InvariantCulture) < selected_device.DeviceMinMoisture)
            {
                tm_manager.Add(tm);
            }
            else if ((topicParts[2] == "h1" || topicParts[2] == "h2") && float.Parse(message, System.Globalization.CultureInfo.InvariantCulture) > selected_device.DeviceMaxMoisture)
            {
                tm_manager.Add(tm);
            }



        }
    }
}
