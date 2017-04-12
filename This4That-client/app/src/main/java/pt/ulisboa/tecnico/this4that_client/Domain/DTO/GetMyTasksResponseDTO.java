package pt.ulisboa.tecnico.this4that_client.Domain.DTO;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetMyTasksResponseDTO {

    private int errorCode;
    private List<CSTask> response;

    public int getErrorCode() {
        return errorCode;
    }

    public void setErrorCode(int errorCode) {
        this.errorCode = errorCode;
    }

    public List<CSTask> getResponse() {
        return response;
    }

    public void setResponse(List<CSTask> response) {
        this.response = response;
    }
}
