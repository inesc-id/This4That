using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        bool CreateTask(string encryptedTask, out string taskID);
    }

    public interface ITaskDistributor : IRemoteEntity
    {

    }

    public interface IReportAggregator : IRemoteEntity
    {
        bool CreateReport(string jsonBody);

    }

    public interface IIncentiveEngine : IRemoteEntity
    {
        bool CalcTaskCost(string taskSpec, out object incentiveValue);
        bool IsTaskPaid(string transactionId);
    }

    public interface IRepository : IRemoteEntity
    {
        bool AuthtenticateUser(string userID);
    }
}
