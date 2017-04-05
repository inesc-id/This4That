package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.Domain.DTO.CreateTaskResponseDTO;
import pt.ulisboa.tecnico.this4that_client.Domain.DTO.TaskCostResponseDTO;
import pt.ulisboa.tecnico.this4that_client.MainActivity;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

/**
 * Created by Calado on 05-04-2017.
 */

public class CreateTaskService extends AsyncTask<String,Integer, String> {

    public Context context;
    final String TAG = "CalcTaskCostService";
    private Exception ex;

    public CreateTaskService(Context context) {
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

        if (ex != null && ex instanceof SocketTimeoutException)
            Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();

        createTaskResponse = gson.fromJson(result, CreateTaskResponseDTO.class);
        //Toast.makeText(context, "Task ID: " + createTaskResponse.getResponse().getTaskId(), Toast.LENGTH_LONG).show();
        activity = (MainActivity) context;
        activity.getTxtTaskID().setText(createTaskResponse.getResponse().getTaskId());
    }
}
