package pt.ulisboa.tecnico.this4that_client.applicationLayer;

import org.json.JSONObject;

import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;
import java.util.HashMap;

/**
 * Created by Calado on 30-03-2017.
 */

public class HttpClient {

    public static String postJSON(String endpointURL, HashMap<String, Object> jsonBody){

        URL url;
        HttpURLConnection connection = null;
        String postBody = null;
        DataOutputStream outStream;
        InputStream inStream;
        BufferedReader bufReader;
        StringBuilder response;
        String line;

        try{
            postBody = convertToJSON(jsonBody).toString();
            //prepare request
            url = new URL(endpointURL);
            connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("POST");
            connection.setRequestProperty("Content-Type", "application/json");
            connection.setRequestProperty("Content-Length", Integer.toString(postBody.getBytes().length));
            connection.setUseCaches(false);
            connection.setDoOutput(true);
            //send the request
            outStream = new DataOutputStream(connection.getOutputStream());
            outStream.writeBytes(postBody);
            outStream.close();
            //get Response
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
            return null;
        }
        finally {
            if (connection != null)
                connection.disconnect();
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
