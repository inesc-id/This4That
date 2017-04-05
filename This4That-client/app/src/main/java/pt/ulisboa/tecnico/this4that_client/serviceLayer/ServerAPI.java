package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.util.Log;

import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import static android.content.ContentValues.TAG;

/**
 * Created by Calado on 30-03-2017.
 */

public class ServerAPI {

    private String serverURL;
    private String userId;

    private final String CALCTASK = "task/cost";
    private final String CREATETASK = "task";
    private String GETTOPICS = "topic/";

    public ServerAPI(String serverURL, String userId) {
        this.serverURL = serverURL;
        this.userId = userId;
    }

    public boolean CalcTaskCostAPI(CSTask task, Context ctx) {

        CalcTaskCostService calcTaskCostService;
        String postBody;
        HashMap<String, Object> postParams = new HashMap<>();
        try {
            calcTaskCostService = new CalcTaskCostService(ctx);
            postParams.put("userId", userId);
            postParams.put("task", task.toHashMap());
            postBody = HttpClient.convertToJSON(postParams).toString();
            calcTaskCostService.execute(this.serverURL + CALCTASK, postBody);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }

    public boolean CreateCSTask(CSTask csTask, Context ctx, String refToPay) {

        CreateTaskService createTaskService;
        String postBody;
        HashMap<String, Object> postParams = new HashMap<>();
        try {
            createTaskService = new CreateTaskService(ctx);
            postParams.put("userId", userId);
            postParams.put("refToPay", refToPay);
            postParams.put("task", csTask.toHashMap());
            postBody = HttpClient.convertToJSON(postParams).toString();
            createTaskService.execute(this.serverURL + CREATETASK, postBody);
            return true;

        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return false;
        }
    }
/*
    public String GetTopics(Context ctx) {

        HttpClient clientHttp;

        /*
        try {
            clientHttp = new HttpClient(ctx);
            return clientHttp.getRequestFromServer(serverURL + GETTOPICS);
        } catch (Exception ex) {
            Log.d(TAG, ex.getMessage());
            return  null;
        }
    }*/


}
