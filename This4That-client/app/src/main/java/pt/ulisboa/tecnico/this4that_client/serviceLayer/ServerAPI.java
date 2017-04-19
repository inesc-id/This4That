package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.util.Log;

import java.net.URL;
import java.net.URLEncoder;
import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.activity.CreateTaskActivity;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.fragment.MyTasksFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.SearchTopicsFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.SubscribedTasksFragment;

import static android.content.ContentValues.TAG;

/**
 * Created by Calado on 30-03-2017.
 */

public class ServerAPI {

    private String serverURL;

    private final String CALCTASK = "task/cost/";
    private final String CREATETASK = "task/";
    private String GETTOPICS = "topic/";

    public ServerAPI() {
        this.serverURL = "http://192.168.1.111:58949/api/";
    }

    public boolean calcTaskCostAPI(CSTask task, CreateTaskActivity createTaskActivity) {

        CalcTaskCostService calcTaskCostService;
        String postBody;
        HashMap<String, Object> postParams = new HashMap<>();

        try {
            postParams.put("userId", createTaskActivity.getGlobalApp().getUserInfo().getUserId());
            postParams.put("task", task.toHashMap());
            postBody = HttpClient.convertToJSON(postParams).toString();
            calcTaskCostService = new CalcTaskCostService(createTaskActivity, postBody);
            calcTaskCostService.execute(this.serverURL + CALCTASK, postBody);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean createCSTask(CSTask csTask, CreateTaskActivity createTaskActivity) {

        CreateTaskService createTaskService;
        String postBody;
        HashMap<String, Object> postParams = new HashMap<>();
        try {
            createTaskService = new CreateTaskService(createTaskActivity);
            postParams.put("userId", createTaskActivity.getGlobalApp().getUserInfo().getUserId());
            postParams.put("task", csTask.toHashMap());
            postBody = HttpClient.convertToJSON(postParams).toString();
            createTaskService.execute(this.serverURL + CREATETASK, postBody);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean createCSTask(String postBody, CreateTaskActivity createTaskActivity) {

        CreateTaskService createTaskService;

        try {
            createTaskService = new CreateTaskService(createTaskActivity);
            createTaskService.execute(this.serverURL + CREATETASK, postBody);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean loginUser(String userID) {
        return  true;
    }

    public boolean getTopics(SearchTopicsFragment fragment) {

        SearchTopicsService topicsService;
        try {
            topicsService = new SearchTopicsService(fragment);
            topicsService.execute(this.serverURL + GETTOPICS);
            return true;
        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return  false;
        }
    }

    public boolean getMyTasks(String userID, MyTasksFragment fragment){
        GetMyTasksService myTasksService;
        String myTasksURL = "user/" + userID + "/task/";
        try {
            myTasksService = new GetMyTasksService(fragment);
            myTasksService.execute(this.serverURL + myTasksURL);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean getSubscribedTasks(String userID, SubscribedTasksFragment fragment) {
        GetSubscribedTasksService subscribedTasks;
        String subTasksURL = "user/" + userID + "/subscribedtasks/";
        try {
            subscribedTasks = new GetSubscribedTasksService(fragment);
            subscribedTasks.execute(this.serverURL + subTasksURL);
            return true;
        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean subscribeTopic(Context context, String topicName, String userID){
        SubscribeTopicService subscribeTopicService;
        String subTasksURL = null;
        try {
            subTasksURL = "user/" + userID + "/topic/" + topicName + "/subscribe";
            subscribeTopicService = new SubscribeTopicService(context);
            subscribeTopicService.execute(this.serverURL + subTasksURL);
            return true;
        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }


}
