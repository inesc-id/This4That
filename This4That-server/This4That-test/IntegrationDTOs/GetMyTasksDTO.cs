using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace This4That_test.IntegrationDTOs
{
    public class GetMyTasksDTO
    {
        private int errorCode;
        private string errorMessage;
        private List<CSTaskDTO> response;

        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        public List<CSTaskDTO> Response
        {
            get
            {
                return response;
            }

            set
            {
                response = value;
            }
        }
    }

    public class CSTaskDTO
    {
        [JsonProperty(PropertyName = "taskId")]
        private string taskID;
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        private string name;
        [JsonProperty(PropertyName = "expirationDate", Required = Required.Always)]
        private DateTime expirationDate;
        [JsonProperty(PropertyName = "topic", Required = Required.Always)]
        private string topic;
        [JsonProperty(PropertyName = "sensingTask", NullValueHandling = NullValueHandling.Ignore)]
        private SensingTask sensingTask;
        [JsonProperty(PropertyName = "interactiveTask", NullValueHandling = NullValueHandling.Ignore)]
        private InteractiveTask interactiveTask;

        #region PROPERTIES


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

            if (InteractiveTask != null)
                result += $"InteractiveTask: {interactiveTask.ToString()} ";

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

        public override string ToString()
        {
            string result = null;

            result += $"Question: {Question} ";
            foreach (TaskAnswer answer in Answers)
            {
                result += Environment.NewLine + "Answer: [" + answer.Answer + "] Generated ID: [" + answer.AnswerId + "]";
            }
            return result;
        }
    }

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
