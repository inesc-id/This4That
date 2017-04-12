package pt.ulisboa.tecnico.this4that_client.Domain;

import java.net.UnknownServiceException;

/**
 * Created by Calado on 11-04-2017.
 */

public class UserInfo {
    private String userId;

    public UserInfo(String userId){
        this.userId = userId;
    }

    public String getUserId() {
        return userId;
    }

    public void setUserId(String userId) {
        this.userId = userId;
    }
}
