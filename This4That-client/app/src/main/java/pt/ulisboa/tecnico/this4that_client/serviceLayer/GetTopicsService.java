package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Intent;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;
import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.DTO.GetMyTasksResponseDTO;
import pt.ulisboa.tecnico.this4that_client.Domain.DTO.GetTopicsResponseDTO;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.adapters.MyTasksAdapter;
import pt.ulisboa.tecnico.this4that_client.adapters.TopicsAdapter;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.fragment.MyTasksFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.SearchTasksFragment;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetTopicsService extends AsyncTask<String, Integer, String>{
    private SearchTasksFragment fragment;
    final String TAG = "GetMyTasksService";
    private Exception ex;

    public GetTopicsService(SearchTasksFragment fragment){
        this.fragment = fragment;
    }

    @Override
    protected String doInBackground(String... params) {
        String url = params[0];
        try{
            return HttpClient.makeGETRequest(url);
        }catch (Exception ex){
            this.ex = ex;
            Log.d(TAG, ex.getMessage());
            return null;
        }
    }

    @Override
    protected void onPostExecute(String result){
        Gson gson;
        GetTopicsResponseDTO responseDTO;
        TopicsAdapter topicsAdapter;
        List<String> topics;
        GlobalApp globalApp;
        try{
            if (ex != null && ex instanceof SocketTimeoutException){
                Toast.makeText(fragment.getContext(), "Cannot connect to server!", Toast.LENGTH_LONG).show();
                return;
            }
            if (result == null){
                Toast.makeText(fragment.getContext(), "Cannot obtain topics!", Toast.LENGTH_LONG).show();
                return;
            }
            //get gson object with the date serializer
            gson = HttpClient.getGsonAPI();
            responseDTO = gson.fromJson(result, GetTopicsResponseDTO.class);

            topics = responseDTO.getResponse();
            topicsAdapter = new TopicsAdapter(topics);
            this.fragment.getRecyclerView().setAdapter(topicsAdapter);

        }catch (Exception ex){
            Log.d(TAG, ex.getMessage());
        }

    }

}
