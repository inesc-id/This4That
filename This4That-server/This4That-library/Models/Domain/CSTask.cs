using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace This4That_library.Models.Domain
{
    [Serializable]
    public class CSTask
    {
        private string taskID;
        private string name;
        private DateTime expirationDate;
        private string topic;
        private SensingTask sensingTask;
        private InteractiveTask interactiveTask;


        #region PROPERTIES


        [JsonProperty(PropertyName = "taskId")]
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

        [JsonProperty(PropertyName = "name", Required = Required.Always)]
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

        [JsonProperty(PropertyName = "expDate", Required = Required.Always)]
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

        [JsonProperty(PropertyName = "topic", Required = Required.Always)]
        public string TopicName
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

        [JsonProperty(PropertyName = "interactiveTask", Required = Required.Always)]
        public InteractiveTask InteractiveTask
        {
            get
            {
                return interactiveTask;
            }

            set
            {
                interactiveTask = value;
            }
        }
        #endregion

        public override string ToString()
        {
            string result = null;

            result = $"Name: {name} Exp: {expirationDate.ToString()} Topic: {topic} " + Environment.NewLine;
            if (SensingTask != null)
                result += $"SensingTask: {sensingTask.ToString()} ";
            else
                result += $"SensingTask: NULL ";
            return result;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
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

        [JsonProperty(PropertyName = "sensor", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
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

    [Serializable]
    public class InteractiveTask
    {
        [JsonProperty(PropertyName = "question", Required = Required.Always)]
        private string question;

        [JsonProperty(PropertyName ="answers", Required = Required.Always)]
        List<TaskAnswer> answers;
    }

    [Serializable]
    public class TaskAnswer
    {
        [JsonProperty(PropertyName = "answer")]
        private string answer;

    }

    

}
