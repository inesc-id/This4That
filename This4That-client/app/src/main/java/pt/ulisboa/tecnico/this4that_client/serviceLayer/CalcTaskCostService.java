package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.Domain.DTO.TaskCostResponseDTO;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

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
        MainActivity activity;
        TaskCostResponseDTO taskCostResponseDTO;
        if (ex != null && ex instanceof SocketTimeoutException){
            Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
            return;
        }
        if (result == null){
            Toast.makeText(context, "Cannot calculate Task Cost!", Toast.LENGTH_LONG).show();
            return;
        }
        taskCostResponseDTO = gson.fromJson(result, TaskCostResponseDTO.class);
        activity = (MainActivity) context;
        activity.getTxtRefToPay().setText(taskCostResponseDTO.getResponse().getRefToPay());
        activity.getTxtValToPay().setText(taskCostResponseDTO.getResponse().getValToPay());
    }
}
