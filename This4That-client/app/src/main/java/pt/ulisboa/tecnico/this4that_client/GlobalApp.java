package pt.ulisboa.tecnico.this4that_client;

import android.app.Activity;
import android.app.Application;
import android.support.v4.app.ActivityCompat;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentActivity;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;

import java.util.HashMap;
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
    private HashMap<String, CSTask> myTasks = new HashMap<>();
    private HashMap<String, CSTask> subscribedTasks = new HashMap<>();



    public ServerAPI getServerAPI() {
        return serverAPI;
    }
    public void setServerAPI(ServerAPI serverAPI) {
        this.serverAPI = serverAPI;
    }

    public HashMap<String, CSTask> getMyTasks() {
        return myTasks;
    }

    public void setMyTasks(List<CSTask> myTasks) {
        for (CSTask task : myTasks) {
            this.myTasks.put(task.getTaskID(), task);
        }
    }

    public HashMap<String, CSTask> getSubscribedTasks() {
        return subscribedTasks;
    }

    public void setSubscribedTasks(List<CSTask> subscribedTasks) {
        for (CSTask task : subscribedTasks) {
            this.subscribedTasks.put(task.getTaskID(), task);
        }
    }

    public UserInfo getUserInfo() {
        return userInfo;
    }
    public void setUserInfo(UserInfo userInfo) {
        this.userInfo = userInfo;
    }

}
