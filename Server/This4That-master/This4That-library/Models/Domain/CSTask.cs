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

        [JsonProperty(PropertyName = "expires")]
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
        private TriggerSensor sensor;

        [JsonProperty(PropertyName = "sensor")]
        public TriggerSensor Sensor
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
            if (Sensor != null)
                return Sensor.ToString();
            else
                return "NULL";
        }
    }

    public enum SensorType
    {
        NONE,
        GPS,
        TEMPERATURE
    }
    [Serializable]
    public class TriggerSensor
    {
        private SensorType type;
        private string triggerInfo1;
        private string triggerInfo2;

        [JsonProperty(PropertyName = "type")]
        public SensorType Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        [JsonProperty(PropertyName = "info1")]
        public string TriggerInfo1
        {
            get
            {
                return triggerInfo1;
            }

            set
            {
                triggerInfo1 = value;
            }
        }

        [JsonProperty(PropertyName = "info2")]
        public string TriggerInfo2
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

            result += $"Sensor: {Type.ToString()} ";
            if (TriggerInfo1 != null)
                result += $"Info1: {TriggerInfo1.ToString()} ";
            if (TriggerInfo2 != null)
                result += $"Info2: {TriggerInfo2.ToString()}";

            return result;
        }
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
