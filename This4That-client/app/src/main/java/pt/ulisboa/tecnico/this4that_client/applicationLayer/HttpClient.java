package pt.ulisboa.tecnico.this4that_client.applicationLayer;

import android.util.Log;

import com.google.gson.Gson;
import com.google.gson.GsonBuilder;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.Date;
import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.JSON.DateDeserializer;

import static android.content.ContentValues.TAG;

/**
 * Created by Calado on 05-04-2017.
 */

public class HttpClient {

    public static String makePOSTRequest(String postBody, String endpointURL) throws Exception {
        URL url;
        HttpURLConnection connection = null;
        DataOutputStream outStream;
        InputStream inStream;
        BufferedReader bufReader;
        StringBuilder response;
        String line;

        try {
            response = null;
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
            while ((line = bufReader.readLine()) != null) {
                response.append(line);
                response.append('\n');
            }
            bufReader.close();
            return response.toString();
        } catch (Exception ex) {
            Log.d(TAG, "makePOSTRequest: " + ex.getMessage());
            throw ex;
        } finally {
            if (connection != null)
                connection.disconnect();
        }
    }

    public static String makeGETRequest(String endpointURL) throws Exception {
        URL url;
        HttpURLConnection connection = null;
        InputStream inStream;
        BufferedReader bufReader;
        StringBuilder response = null;
        String line;

        try {
            //prepare request
            url = new URL(endpointURL);
            connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("GET");
            connection.setUseCaches(false);
            connection.setConnectTimeout(5000);
            if (connection.getResponseCode() != 200)
                inStream = connection.getErrorStream();
            else
                inStream = connection.getInputStream();
            bufReader = new BufferedReader(new InputStreamReader(inStream));
            response = new StringBuilder();
            while ((line = bufReader.readLine()) != null) {
                response.append(line);
                response.append('\n');
            }
            bufReader.close();
            return response.toString();
        } catch (Exception ex) {
            Log.d(TAG, "makeGETRequest: " + ex.getMessage());
            throw ex;
        } finally {
            if (connection != null)
                connection.disconnect();
        }
    }

    public static Gson getGsonAPI(){
        GsonBuilder builder = new GsonBuilder().registerTypeAdapter(Date.class, new DateDeserializer());
        return builder.create();
    }

    public static JSONObject convertToJSON(HashMap<String, Object> elements) {

        try {
            return new JSONObject(elements);
        } catch (Exception ex) {
            return null;
        }

    }
}
