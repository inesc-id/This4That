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
import pt.ulisboa.tecnico.this4that_client.adapters.MyTasksAdapter;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.fragment.MyTasksFragment;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetMyTasksService extends AsyncTask<String,Integer, String> {

    private MyTasksFragment myTasksFragment;
    final String TAG = "GetMyTasksService";
    private Exception ex;

    public GetMyTasksService(MyTasksFragment fragment){
        this.myTasksFragment = fragment;
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
        GetMyTasksResponseDTO responseDTO;
        List<CSTask> myTasks;
        GlobalApp globalApp;
        try{
            if (ex != null && ex instanceof SocketTimeoutException){
                Toast.makeText(myTasksFragment.getContext(), "Cannot connect to server!", Toast.LENGTH_LONG).show();
                return;
            }
            if (result == null){
                Toast.makeText(myTasksFragment.getContext(), "Cannot obtain my Tasks!", Toast.LENGTH_LONG).show();
                return;
            }
            //get gson object with the date serializer
            gson = HttpClient.getGsonAPI();
            responseDTO = gson.fromJson(result, GetMyTasksResponseDTO.class);

            if (responseDTO.getErrorCode() != 1){
                Toast.makeText(myTasksFragment.getContext()
                               , "Cannot obtain my Tasks!"
                               , Toast.LENGTH_LONG).show();
                return;
            }
            globalApp = (GlobalApp) myTasksFragment.getParentActivity().getApplicationContext();
            myTasks = responseDTO.getResponse();
            globalApp.setMyTasks(myTasks);
            MyTasksAdapter tasksAdapter = new MyTasksAdapter(myTasks, myTasksFragment);
            //new tasks list
            this.myTasksFragment.getRecyclerView().setAdapter(tasksAdapter);

        }catch (Exception ex){
            Log.d(TAG, ex.getMessage());
        }

    }
}
