package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;
import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.JSON.DTO.GetMyTasksResponseDTO;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.adapters.SubscribedTasksAdapter;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.fragment.SubscribedTasksFragment;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetSubscribedTasksService extends AsyncTask<String, Integer, String>{

    private SubscribedTasksFragment fragment;
    private Exception ex;

    public GetSubscribedTasksService(SubscribedTasksFragment fragment){
        this.fragment = fragment;
    }

    @Override
    protected String doInBackground(String... params) {
        String url = params[0];
        try{
            return HttpClient.makeGETRequest(url);
        }catch (Exception ex){
            this.ex = ex;
            Log.d("This4That", ex.getMessage());
            return null;
        }
    }

    @Override
    protected void onPostExecute(String result){
        Gson gson;
        GetMyTasksResponseDTO responseDTO;
        List<CSTask> subscribedTasks;
        SubscribedTasksAdapter subscribedTasksAdapter;
        GlobalApp globalApp;
        try{
            if (ex != null && ex instanceof SocketTimeoutException){
                Toast.makeText(fragment.getContext(), "Cannot connect to server!", Toast.LENGTH_LONG).show();
                return;
            }
            if (result == null){
                Toast.makeText(fragment.getContext(), "Cannot obtain my Tasks!", Toast.LENGTH_LONG).show();
                return;
            }
            //get gson object with the date serializer
            gson = HttpClient.getGsonAPI();
            responseDTO = gson.fromJson(result, GetMyTasksResponseDTO.class);
            if (responseDTO.getErrorCode() != 1){
                Toast.makeText(fragment.getContext(), "Cannot obtain my Tasks! \n" + responseDTO.getErrorMessage()
                               , Toast.LENGTH_LONG).show();
                return;
            }

            globalApp = (GlobalApp) fragment.getParentActivity().getApplicationContext();
            subscribedTasks = responseDTO.getResponse();
            globalApp.setSubscribedTasks(subscribedTasks);
            subscribedTasksAdapter = new SubscribedTasksAdapter(subscribedTasks);
            this.fragment.getRecyclerView().setAdapter(subscribedTasksAdapter);

        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
        }

    }
}
