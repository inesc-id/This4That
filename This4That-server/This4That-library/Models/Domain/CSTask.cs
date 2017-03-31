using Newtonsoft.Json;
using System;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class CSTask
    {
        private string taskID;
        private string name;
        private DateTime expirationDate;
        private string topic;
        private Trigger trigger;
        private SensingTask sensingTask;

        public override string ToString()
        {
            string result = null;

            result = $"Name: {name} Exp: {expirationDate.ToString()} Topic: {topic} " + Environment.NewLine;
            if (Trigger != null)
                result += $"Trigger: {trigger.ToString()} " + Environment.NewLine;
            else
                result += $"Trigger: NULL ";
            if (SensingTask != null)
                result += $"SensingTask: {sensingTask.ToString()} ";
            else
                result += $"SensingTask: NULL ";
            return result;
        }
        #region PROPERTIES
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        [JsonProperty(PropertyName = "expDate")]
        public DateTime ExpirationDate
        {
            get
            {
                return expirationDate;
            }

            set
            {
                expirationDate = value;
            }
        }

        [JsonProperty(PropertyName = "topic")]
        public string Topic
        {
            get
            {
                return topic;
            }

            set
            {
                topic = value;
            }
        }

        [JsonProperty(PropertyName = "trigger")]
        public Trigger Trigger
        {
            get
            {
                return trigger;
            }

            set
            {
                trigger = value;
            }
        }

        [JsonProperty(PropertyName = "sensingTask")]
        public SensingTask SensingTask
        {
            get
            {
                return sensingTask;
            }

            set
            {
                sensingTask = value;
            }
        }

        public string TaskID
        {
            get
            {
                return taskID;
            }

            set
            {
                taskID = value;
            }
        }
        #endregion
    }

    [Serializable]
    public class Trigger
    {
        private SensorType sensor;
        private string param1;
        private string triggerInfo2;

        [JsonProperty(PropertyName = "sensor")]
        public SensorType Sensor
        {
            get
            {
                return sensor;
            }

            set
            {
                sensor = value;
            }
        }

        [JsonProperty(PropertyName = "param1")]
        public string Param1
        {
            get
            {
                return param1;
            }

            set
            {
                param1 = value;
            }
        }

        [JsonProperty(PropertyName = "param2")]
        public string Param2
        {
            get
            {
                return triggerInfo2;
            }

            set
            {
                triggerInfo2 = value;
            }
        }

        public override string ToString()
        {
            String result = null;

            result += $"Sensor: {Sensor.ToString()} ";
            if (Param1 != null)
                result += $"Param1: {Param1.ToString()} ";
            if (Param2 != null)
                result += $"Param2: {Param2.ToString()}";

            return result;
        }
    }

    public enum SensorType
    {
        NONE,
        GPS,
        TEMPERATURE
    }

    [Serializable]
    public class SensingTask
    {
        private SensorType sensor;

        [JsonProperty(PropertyName = "sensor")]
        public SensorType Sensor
        {
            get
            {
                return sensor;
            }

            set
            {
                sensor = value;
            }
        }

        public override string ToString()
        {
            return "Sensor: " + Sensor.ToString();
        }
    }

    

}
