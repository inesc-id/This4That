using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_library.Domain
{
    public abstract class Report
    {
        private string userID;
        private string taskId;
        private DateTime timestamp;
        private string reportId;

        public string UserID
        {
            get
            {
                return userID;
            }

            set
            {
                userID = value;
            }
        }

        public string TaskId
        {
            get
            {
                return taskId;
            }

            set
            {
                taskId = value;
            }
        }

        public DateTime Timestamp
        {
            get
            {
                return timestamp;
            }

            set
            {
                timestamp = value;
            }
        }

        public string ReportID
        {
            get
            {
                return reportId;
            }

            set
            {
                reportId = value;
            }
        }
    }



    public class SensingReport : Report
    {
        private SensingTaskResult result;

        public SensingReport(ReportDTO reportDTO)
        {
            this.TaskId = reportDTO.TaskId;
            this.Timestamp = reportDTO.Timestamp;
            this.UserID = reportDTO.UserID;
            this.Result = ((SensingTaskReportDTO)reportDTO).Result;
        }

        public SensingTaskResult Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }



    public class InteractiveReport : Report
    {
        private InteractiveTaskResult result;

        public InteractiveReport(ReportDTO reportDTO)
        {
            this.TaskId = reportDTO.TaskId;
            this.Timestamp = reportDTO.Timestamp;
            this.UserID = reportDTO.UserID;
            this.Result = ((InteractiveTaskReportDTO)reportDTO).Result;
        }

        public InteractiveTaskResult Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }
    }
}
