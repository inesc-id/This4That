package pt.ulisboa.tecnico.this4that_client;

import android.app.Application;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.UserInfo;
import pt.ulisboa.tecnico.this4that_client.serviceLayer.ServerAPI;

/**
 * Created by Calado on 11-04-2017.
 */

public class GlobalApp extends Application {

    private ServerAPI serverAPI;
    private UserInfo userInfo;
    private List<CSTask> myTasks;
    private List<CSTask> subscribedTasks;



    public ServerAPI getServerAPI() {
        return serverAPI;
    }
    public void setServerAPI(ServerAPI serverAPI) {
        this.serverAPI = serverAPI;
    }

    public List<CSTask> getMyTasks() {
        return myTasks;
    }

    public void setMyTasks(List<CSTask> myTasks) {
        this.myTasks = myTasks;
    }

    public List<CSTask> getSubscribedTasks() {
        return subscribedTasks;
    }

    public void setSubscribedTasks(List<CSTask> subscribedTasks) {
        this.subscribedTasks = subscribedTasks;
    }

    public UserInfo getUserInfo() {
        return userInfo;
    }

    public void setUserInfo(UserInfo userInfo) {
        this.userInfo = userInfo;
    }
}
