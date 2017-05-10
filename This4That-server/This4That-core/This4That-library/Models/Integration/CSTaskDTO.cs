using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace This4That_library.Models.Integration
{
    [Serializable]
    public class CSTaskDTO
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

        [JsonProperty(PropertyName = "interactiveTask")]
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

        [JsonProperty(PropertyName = "sensor", Required = Required.Always)]
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
        private string question;
        private List<TaskAnswer> answers;

        [JsonProperty(PropertyName = "question", Required = Required.Always)]
        public string Question
        {
            get
            {
                return question;
            }

            set
            {
                question = value;
            }
        }

        [JsonProperty(PropertyName = "answers", Required = Required.Always)]
        public List<TaskAnswer> Answers
        {
            get
            {
                return answers;
            }

            set
            {
                answers = value;
            }
        }
    }

    [Serializable]
    public class TaskAnswer
    {        
        private string answer;
        private string answerId;

        [JsonProperty(PropertyName = "answer")]
        public string Answer
        {
            get
            {
                return answer;
            }

            set
            {
                answer = value;
            }
        }

        [JsonProperty(PropertyName = "answerId")]
        public string AnswerId
        {
            get
            {
                return answerId;
            }

            set
            {
                answerId = value;
            }
        }

        public TaskAnswer()
        {
            AnswerId = Guid.NewGuid().ToString().Substring(0, 8);
        }
    }

    

}
