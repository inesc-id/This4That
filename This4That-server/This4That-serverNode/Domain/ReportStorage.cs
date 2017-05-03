using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_serverNode.Domain
{
    public class ReportStorage
    {
        private Dictionary<string, Report> reports = new Dictionary<string, Report>();


        public bool SaveReport(ReportDTO reportDTO)
        {
            Object lockObj = new object();
            string reportId = Guid.NewGuid().ToString().Substring(0, 8);
            Report report;
            
            try
            {
                //get data from DTO and create objects
                if (reportDTO.GetType() == typeof(SensingTaskReportDTO))
                {
                    report = new SensingReport(reportDTO);
                }
                if (reportDTO.GetType() == typeof(InteractiveTaskReportDTO))
                {
                    report = new InteractiveReport(reportDTO);
                }
                else
                {
                    return false;
                }
                lock (lockObj)
                {
                    while (reports.ContainsKey(reportId))
                    {
                        reportId = Guid.NewGuid().ToString().Substring(0, 8);
                    }
                    report.ReportID = reportId;
                    reports.Add(reportId, report);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }
    }
}
