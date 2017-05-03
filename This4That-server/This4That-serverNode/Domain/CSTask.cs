using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration;

namespace This4That_serverNode.Domain
{
    public class CSTask
    {
        private string taskID;
        private string name;
        private DateTime expirationDate;
        private string topic;
        private SensingTask sensingTask;
        private InteractiveTask interactiveTask;
        private List<string> reportsID = new List<string>();

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

        public List<string> ReportsID
        {
            get
            {
                return reportsID;
            }

            set
            {
                reportsID = value;
            }
        }

        public CSTask(CSTaskDTO taskDTO)
        {
            this.TaskID = taskDTO.TaskID;
            this.Name = taskDTO.Name;
            this.ExpirationDate = taskDTO.ExpirationDate;
            this.Topic = taskDTO.TopicName;
            this.SensingTask = taskDTO.SensingTask;
            this.InteractiveTask = taskDTO.InteractiveTask;
        }

        internal CSTaskDTO ToDTO()
        {
            CSTaskDTO taskDTO = new CSTaskDTO();
            try
            {
                taskDTO.TaskID = this.TaskID;
                taskDTO.Name = this.Name;
                taskDTO.ExpirationDate = this.ExpirationDate;
                taskDTO.TopicName = this.Topic;
                taskDTO.SensingTask = this.SensingTask;
                taskDTO.InteractiveTask = this.InteractiveTask;
                return taskDTO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
