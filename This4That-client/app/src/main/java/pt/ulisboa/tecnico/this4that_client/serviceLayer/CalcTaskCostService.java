package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.JSON.DTO.TaskCostResponseDTO;
import pt.ulisboa.tecnico.this4that_client.activity.CreateTaskActivity;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.fragment.ConfirmTaskCreation;

/**
 * Created by Calado on 05-04-2017.
 */

public class CalcTaskCostService extends AsyncTask<String,Integer, String>{

    public Context context;
    final String TAG = "CalcTaskCostService";
    private Exception ex;

    public CalcTaskCostService(Context context){
        this.context = context;
    }


    @Override
    protected String doInBackground(String... params) {
        String url = params[0];
        String postBody = params[1];
        try{
            return HttpClient.makePOSTRequest(postBody, url);
        }catch (Exception ex){
            this.ex = ex;
            Log.d(TAG, ex.getMessage());
            return null;
        }
    }

    @Override
    protected void onPostExecute(String result){
        Gson gson = new Gson();
        CreateTaskActivity activity;
        TaskCostResponseDTO taskCostResponseDTO;
        ConfirmTaskCreation confirmTaskCreation;

        if (ex != null && ex instanceof SocketTimeoutException){
            Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
            return;
        }
        if (result == null){
            Toast.makeText(context, "Cannot calculate Task Cost!", Toast.LENGTH_LONG).show();
            return;
        }
        taskCostResponseDTO = gson.fromJson(result, TaskCostResponseDTO.class);
        activity = (CreateTaskActivity) context;

        confirmTaskCreation = new ConfirmTaskCreation();
        confirmTaskCreation.show(activity.getSupportFragmentManager(), "confirmTaskDialog");
    }
}
