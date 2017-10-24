﻿using System;
using System.Collections.Generic;
using This4That_library.Models.Integration;
using This4That_library.Models.Integration.GetTasksByTopicDTO;
using This4That_library.Models.Integration.ReportDTO;
using This4That_library.Models.IncentiveModels;
using This4That_library.Models.Domain;
using This4That_library.Models.Incentives;
using This4That_library.Integration;

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
        bool CalcTaskCost(CSTaskDTO taskSpec, string userID, out IncentiveAssigned incentive);
        bool PayTask(string userId, out string transactionId, out bool hasfunds);
        bool RewardUser(string userId, string taskId, out string transactionId, out object response);
        bool RegisterUser(out string userId, out string userMultichainAddress);
        bool GetUserTransactions(string userId, out List<Transaction> transactions);
        bool AddNodeToChain(string userID, string multichainAddress, ref string message);
    }

    public interface IRepository : IRemoteEntity
    {
        bool RegisterTask(CSTaskDTO topic, string userID, out string taskID);
        bool GetTasksByTopicName(out List<GetTasksDTO> listTaskDTO, string topicName);
        List<String> GetTopicsFromRepository();
        bool RegisterUser(string userAddress, Incentive incentive);
        List<CSTaskDTO> GetTasksByUserID(string userID);
        List<CSTaskDTO> GetSubscribedTasksbyUserID(string userID);
        List<CSTaskDTO> GetSubscribedTasksbyTopic(string userID, string topicName, ref string errorMessage);
        bool SubscribeTopic(string userId, string topicName, ref string errorMessage);
        bool SaveReportInRepository(ReportDTO report);
        bool ExecuteTask(string userID, string taskId);
        bool AddUserMultichainNode(string userId, string nodeAddress);
        List<string> GetUserMultichainNodes(string userId);
        List<CSTask> GetCSTasks();
        List<CSTask> GetCSTasksToValidate();
        InteractiveReport GetInteractiveReportsByID(string reportID);
        void SaveReportReward(string taskId, string reportId, Dictionary<string, string> reward, string txId);
        string GetUserReportByTaskId(string taskId, string userId);

    }

    public interface ITransactionNode : IRemoteEntity
    {
        Transaction GetTransactionById(string txId);
        bool CreateTransaction(string sender, string receiver, IncentiveAssigned incentiveAssigned, out string transactionID);
        bool GetUserWallet(string userId, out Wallet wallet);
        bool CreateUserWallet(string userAddress, Incentive incentive);
        bool IssueMoreIncentives(string managerAddress, IncentiveAssigned incentiveAssigned);
    }
}
