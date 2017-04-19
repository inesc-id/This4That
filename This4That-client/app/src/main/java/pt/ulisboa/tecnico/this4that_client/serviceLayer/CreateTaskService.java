package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.JSON.DTO.CreateTaskResponseDTO;
import pt.ulisboa.tecnico.this4that_client.activity.CreateTaskActivity;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

/**
 * Created by Calado on 05-04-2017.
 */

public class CreateTaskService extends AsyncTask<String,Integer, String> {

    public CreateTaskActivity context;
    final String TAG = "CalcTaskCostService";
    private Exception ex;

    public CreateTaskService(CreateTaskActivity context) {
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
        CreateTaskResponseDTO createTaskResponse;
        MainActivity activity;

        if (ex != null && ex instanceof SocketTimeoutException){
            Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
            return;
        }
        if (result == null){
            Toast.makeText(context, "Cannot Create Task!", Toast.LENGTH_LONG).show();
            return;
        }
        createTaskResponse = gson.fromJson(result, CreateTaskResponseDTO.class);
        if (createTaskResponse.getErrorCode() != 1){
            Toast.makeText(context
                    , "Cannot create Task!"
                    , Toast.LENGTH_LONG).show();
            return;
        }
        Toast.makeText(context, "Task created with Sucess!", Toast.LENGTH_LONG).show();
    }
}
