package pt.ulisboa.tecnico.this4that_client.applicationLayer;

import android.content.Context;
import android.os.AsyncTask;
import android.util.Log;
import android.widget.Toast;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.SocketTimeoutException;
import java.net.URL;
import java.util.HashMap;

import static android.content.ContentValues.TAG;

/**
 * Created by Calado on 30-03-2017.
 */

public class HttpClient {

    private Context context = null;

    public HttpClient(Context context){
        this.context = context;
    }
    private class PostJSON extends AsyncTask<String,Integer, String>{

        private Exception ex;
        @Override
        protected String doInBackground(String... params) {
            URL url;
            HttpURLConnection connection = null;
            String endpointURL = null;
            String postBody = null;
            DataOutputStream outStream;
            InputStream inStream;
            BufferedReader bufReader;
            StringBuilder response;
            String line;

            try{
                response = null;
                if (params[1] != null)
                    postBody = params[1];
                //prepare request
                if (params[0] != null)
                    endpointURL = params[0];
                url = new URL(endpointURL);
                connection = (HttpURLConnection) url.openConnection();
                connection.setRequestMethod("POST");
                connection.setRequestProperty("Content-Type", "application/json");
                connection.setRequestProperty("Content-Length", Integer.toString(postBody.getBytes().length));
                connection.setUseCaches(false);
                connection.setConnectTimeout(2000);
                //send the request
                outStream = new DataOutputStream(connection.getOutputStream());
                outStream.writeBytes(postBody);
                outStream.close();

                if (connection.getResponseCode() != 200)
                    inStream = connection.getErrorStream();
                else
                    inStream = connection.getInputStream();
                bufReader = new BufferedReader(new InputStreamReader(inStream));
                response = new StringBuilder();
                while ((line = bufReader.readLine()) != null){
                    response.append(line);
                    response.append('\n');
                }
                bufReader.close();
                return response.toString();
            }catch (Exception ex){
                Log.d(TAG, "doInBackground: " + ex.getMessage());
                this.ex = ex;
                return null;
            }
            finally {
                if (connection != null)
                    connection.disconnect();
            }
        }

        @Override
        protected void onPostExecute(String result){
            if (ex != null && ex instanceof SocketTimeoutException)
                Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
            else
                Toast.makeText(context, result, Toast.LENGTH_LONG).show();
        }
    }



    public boolean postJSON(String endpointURL, String postBody){
        try{
            new PostJSON().execute(endpointURL, postBody);
            return true;
        }catch(Exception ex){
            Log.d(TAG, "postJSON: " + ex.getMessage());
            Toast.makeText(context, ex.getMessage(), Toast.LENGTH_LONG).show();
            return  false;
        }
    }

    private class GetRequest extends AsyncTask<String,Integer, String>{

        private Exception ex;

        @Override
        protected String doInBackground(String... params) {
            URL url;
            HttpURLConnection connection = null;
            String endpointURL = null;
            InputStream inStream;
            BufferedReader bufReader;
            StringBuilder response = null;
            String line;

            try{
                //prepare request
                if (params[0] != null)
                    endpointURL = params[0];
                url = new URL(endpointURL);
                connection = (HttpURLConnection) url.openConnection();
                connection.setRequestMethod("GET");
                connection.setUseCaches(false);
                connection.setConnectTimeout(2000);
                if (connection.getResponseCode() != 200)
                    inStream = connection.getErrorStream();
                else
                    inStream = connection.getInputStream();
                bufReader = new BufferedReader(new InputStreamReader(inStream));
                response = new StringBuilder();
                while ((line = bufReader.readLine()) != null){
                    response.append(line);
                    response.append('\n');
                }
                bufReader.close();
                return response.toString();
            }catch (Exception ex){
                Log.d(TAG, "doInBackground: " + ex.getMessage());
                this.ex = ex;
                return null;
            }
            finally {
                if (connection != null)
                    connection.disconnect();
            }
        }

        @Override
        protected void onPostExecute(String result){
            if (ex != null && ex instanceof SocketTimeoutException)
                Toast.makeText(context, "Cannot connect to server!", Toast.LENGTH_LONG).show();
            else
                Toast.makeText(context, result, Toast.LENGTH_LONG).show();
        }
    }



    public String getRequestFromServer(String endpointURL){
        try{
            return new GetRequest().execute(endpointURL).get();
        }catch(Exception ex){
            Log.d(TAG, "getRequestFromServer: " + ex.getMessage());
            Toast.makeText(context, ex.getMessage(), Toast.LENGTH_LONG).show();
            return null;
        }
    }

    public static JSONObject convertToJSON(HashMap<String, Object> elements){

        try{
            return new JSONObject(elements);
        }catch (Exception ex){
            return null;
        }
    }
}
