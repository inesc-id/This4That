using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Domain;

namespace This4That_library
{
    public interface IRemoteEntity
    {
    }

    public interface IServerManager : IRemoteEntity
    {
        bool RegisterTaskCreatorNode(string tcpUrl);
        bool RegisterTaskDistributorNode(string tcpUrl);
        bool RegisterReportAggregatorNode(string tcpUrl);
        bool RegisterIncentiveEngineNode(string tcpUrl);
        bool RegisterRepositoryNode(string tcpUrl);
    }

    public interface ITaskCreator : IRemoteEntity
    {
        bool CreateTask(CSTask task, out string taskID);
    }

    public interface ITaskDistributor : IRemoteEntity
    {
        bool ReceiveTask(CSTask task);

    }

    public interface IReportAggregator : IRemoteEntity
    {
        bool CreateReport(string jsonBody);

    }

    public interface IIncentiveEngine : IRemoteEntity
    {
        bool CalcTaskCost(CSTask taskSpec, out object incentiveValue);
        bool IsTaskPaid(string transactionId);
    }

    public interface IRepository : IRemoteEntity
    {
        bool AuthenticateUser(string userID);
    }
}
