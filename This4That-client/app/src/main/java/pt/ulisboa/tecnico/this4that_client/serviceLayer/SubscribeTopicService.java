package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.JSON.DTO.SubscribeTaskResponseDTO;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

/**
 * Created by Calado on 13-04-2017.
 */

public class SubscribeTopicService extends AsyncTask<String, Integer, String> {

    private Context context;
    private Exception ex;

    public SubscribeTopicService(Context pContext){
        this.context = pContext;
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
        SubscribeTaskResponseDTO responseDTO;

        try{
            if (ex != null && ex instanceof SocketTimeoutException){
                Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
                return;
            }
            if (result == null){
                Toast.makeText(context, "Cannot subscribe Task!", Toast.LENGTH_LONG).show();
                return;
            }
            //get gson object with the date serializer
            gson = HttpClient.getGsonAPI();
            responseDTO = gson.fromJson(result, SubscribeTaskResponseDTO.class);

            if (responseDTO.getErrorCode() != 1)
            {
                Toast.makeText(context, "Cannot subscribe Topic. \n" + responseDTO.getErrorMessage()
                               , Toast.LENGTH_LONG).show();
                return;
            }
            Toast.makeText(context, "Topic Subscribed!", Toast.LENGTH_LONG).show();
        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
        }

    }


}
