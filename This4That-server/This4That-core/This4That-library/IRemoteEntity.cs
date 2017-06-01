using System;
using System.Collections.Generic;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;

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
        bool CreateTask(CSTaskDTO task, string userID, out string taskID);
    }

    public interface ITaskDistributor : IRemoteEntity
    {
        List<GetTasksDTO> GetTasksByTopicName(string topicName);
        List<String> GetTopics();
        List<CSTaskDTO> GetUserTasks(string userID);
        List<CSTaskDTO> GetUserSubscribedTasks(string userID);
        bool SubscribeTopic(string userId, string topic, ref string errorMessage);
        List<CSTaskDTO> GetSubscribedTasksByTopicName(string userID, string topicName, ref string errorMessage);
        bool ExecuteTask(string userID, string taskId);
    }

    public interface IReportAggregator : IRemoteEntity
    {
        bool SaveReport(ReportDTO reportTask);

    }

    public interface IIncentiveEngine : IRemoteEntity
    {
        bool CalcTaskCost(CSTaskDTO taskSpec, string userID, out object incentiveValue);
        bool PayTask(string userId, object inventiveValue, out string transactionId);
        bool RewardUser(string userId, out string transactionId, out object rewardObj);
        bool RegisterUser(out string userId, out string userMultichainAddress);
        bool GetUserTransactions(string userId, out List<Transaction> transactions);
        bool AddNodeToChain(string userID, string multichainAddress, ref string message);
    }

    public interface IRepository : IRemoteEntity
    {
        bool RegisterTask(CSTaskDTO topic, string userID, out string taskID);
        bool GetTasksByTopicName(out List<GetTasksDTO> listTaskDTO, string topicName);
        List<String> GetTopicsFromRepository();
        string RegisterUser(Incentive incentive);
        object GetUserBalance(string userID);
        List<CSTaskDTO> GetTasksByUserID(string userID);
        List<CSTaskDTO> GetSubscribedTasksbyUserID(string userID);
        List<CSTaskDTO> GetSubscribedTasksbyTopic(string userID, string topicName, ref string errorMessage);
        bool SubscribeTopic(string userId, string topicName, ref string errorMessage);
        bool AddMultiChainAddressToUser(string userId, string userAddress);
        bool SaveReportInRepository(ReportDTO report);
        bool ExecuteTask(string userID, string taskId);
        bool CreateTransactionCentralized(string senderID, string receiverID, object incentiveValue, out string transactionId);
        bool ExecuteTransactionCentralized(string senderId, string receiverId, Incentive incentive, object incentiveValue, string txId);
        List<Transaction> GetUserTransactionsCentralized(string userId);
        bool AddNodeToUserWalletDescentralized(string userId, string nodeAddress);
    }

    public interface ITransactionNode : IRemoteEntity
    {
        Transaction GetTransactionById(string txId);
        bool CreateTransaction(string sender, string receiver, object value, out string transactionID);

    }
}
