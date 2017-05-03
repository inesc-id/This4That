package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import com.google.gson.Gson;

import java.net.SocketTimeoutException;

import pt.ulisboa.tecnico.this4that_client.JSON.DTO.ReportResponseDTO;
import pt.ulisboa.tecnico.this4that_client.JSON.DTO.SubscribeTaskResponseDTO;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

/**
 * Created by Calado on 02-05-2017.
 */

public class ReportService extends AsyncTask<String, Integer, String> {

    private Context context;
    private Exception ex;

    public ReportService(Context pContext){
        this.context = pContext;
    }

    @Override
    protected String doInBackground(String... params) {
        String url;
        String postBody;

        try{
            url = params[0];
            postBody = params[1];
            return HttpClient.makePOSTRequest(postBody, url);
        }catch (Exception ex){
            this.ex = ex;
            Log.d("This4That", ex.getMessage());
            return null;
        }
    }

    @Override
    protected void onPostExecute(String result){
        Gson gson;
        ReportResponseDTO responseDTO;

        try{
            if (ex != null && ex instanceof SocketTimeoutException){
                Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
                return;
            }
            if (result == null){
                Toast.makeText(context, "Cannot report information!", Toast.LENGTH_LONG).show();
                return;
            }
            //get gson object with the date serializer
            gson = HttpClient.getGsonAPI();
            responseDTO = gson.fromJson(result, ReportResponseDTO.class);

            if (responseDTO.getErrorCode() != 1)
            {
                Toast.makeText(context, "Cannot Report Information. \n" + responseDTO.getErrorMessage()
                        , Toast.LENGTH_LONG).show();
                return;
            }
            Toast.makeText(context, "Info Reported!", Toast.LENGTH_LONG).show();
        }catch (Exception ex){
            Toast.makeText(context, "Cannot Report Information!", Toast.LENGTH_LONG).show();
            Log.d("This4That", ex.getMessage());
        }

    }
}