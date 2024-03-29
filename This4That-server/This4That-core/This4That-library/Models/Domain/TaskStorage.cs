﻿using System;
using System.Collections.Generic;
using This4That_library.Integration;
using This4That_library.Models.Integration;

namespace This4That_library.Models.Domain
{
    public class TaskStorage
    {
        private Dictionary<string, CSTask> tasks = new Dictionary<string, CSTask>();

        public Dictionary<string, CSTask> Tasks
        {
            get
            {
                return tasks;
            }

            set
            {
                tasks = value;
            }
        }



        public bool CreateTask(CSTaskDTO taskDTO)
        {
            string taskID = null;
            CSTask task;
            try
            {
                //save task
                taskID = Guid.NewGuid().ToString();
                while (Tasks.ContainsKey(taskID))
                {
                    taskID = Guid.NewGuid().ToString();
                }
                taskDTO.TaskID = taskID;
                task = new CSTask(taskDTO);
                Tasks.Add(taskID, task);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public CSTask GetTaskByID(string taskID)
        {
            CSTask task;

            if (!Tasks.TryGetValue(taskID, out task))
                return null;

            return task;
        }

        public bool AssociateReport(string userID, string reportID, string taskId)
        {
            CSTask task;

            if (!Tasks.TryGetValue(taskId, out task))
                return false;

            if (!task.ReportsID.ContainsKey(userID))
                task.ReportsID.Add(userID, reportID);
            return true;
        }
    }
}
