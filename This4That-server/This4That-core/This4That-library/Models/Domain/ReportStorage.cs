using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Integration.ReportDTO;

namespace This4That_library.Models.Domain
{
    public class ReportStorage
    {
        private Dictionary<string, Report> reports = new Dictionary<string, Report>();

        public Dictionary<string, Report> Reports
        {
            get
            {
                return reports;
            }
        }

        public bool SaveReport(ref ReportDTO reportDTO, out Report report)
        {
            string reportId = Guid.NewGuid().ToString().Substring(0, 8);
            report = null;
            
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
                lock (Reports)
                {
                    while (Reports.ContainsKey(reportId))
                    {
                        reportId = Guid.NewGuid().ToString().Substring(0, 8);
                    }
                    report.ReportID = reportId;
                    Reports.Add(reportId, report);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }            
        }

        public void AssociateReportReward(string reportId, Dictionary<string, string> reward, string txId)
        {
            ReportReward reportReward = new ReportReward();

            reportReward.IncentiveName = reward["incentive"];
            reportReward.IncentiveValue = reward["quantity"];
            reportReward.TransactionId = txId;

            this.Reports[reportId].ReportReward = reportReward;
        }
    }
}
