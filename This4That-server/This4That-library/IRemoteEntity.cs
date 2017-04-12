using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using This4That_library.Models.Domain;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_serverNode.IncentiveModels;

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
        bool CreateTask(CSTask task, string userID, out string taskID);
    }

    public interface ITaskDistributor : IRemoteEntity
    {
        List<GetTasksDTO> GetTasksByTopicName(string topicName);
        List<String> GetTopics();
        List<CSTask> GetUserTasks(string userID);
        List<CSTask> GetUserSubscribedTasks(string userID);
        bool SubscribeTopic(string userId, string topic);
        List<CSTask> GetSubscribedTasksByTopicName(string userID, string topicName);
    }

    public interface IReportAggregator : IRemoteEntity
    {
        bool CreateReport(string jsonBody);

    }

    public interface IIncentiveEngine : IRemoteEntity
    {
        bool CalcTaskCost(CSTask taskSpec, string userID, out object incentiveValue);
        bool PayTask(string userId, out string transactionId);
    }

    public interface IRepository : IRemoteEntity
    {
        bool AuthenticateUser(string userID);
        bool GetUserIncentiveMechanism(string userID, out IncentiveSchemeBase incentiveScheme);
        bool RegisterTask(CSTask topic, string userID, out string taskID);
        bool GetTasksByTopicName(out List<GetTasksDTO> listTaskDTO, string topicName);
        List<String> GetTopicsFromRepository();
        string RegisterUser();
        List<CSTask> GetTasksByUserID(string userID);
        List<CSTask> GetSubscribedTasksbyUserID(string userID);
        List<CSTask> GetSubscribedTasksbyTopic(string userID, string topicName);
        bool SubscribeTopic(string userId, string topicName);
    }
}
