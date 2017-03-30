package pt.ulisboa.tecnico.this4that_client.serviceLayer;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;

/**
 * Created by Calado on 30-03-2017.
 */

public class ServerAPI {

    private String serverURL;

    public ServerAPI(String serverURL){
        this.serverURL = serverURL;
    }

    public boolean CalcTaskCostAPI(CSTask task){

        String response;

        response = HttpClient.postJSON(serverURL + "calcTask", task.toHashMap());
        return  true;
    }
}
