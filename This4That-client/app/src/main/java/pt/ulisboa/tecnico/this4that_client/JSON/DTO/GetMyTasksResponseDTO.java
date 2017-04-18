package pt.ulisboa.tecnico.this4that_client.JSON.DTO;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetMyTasksResponseDTO extends APIResponseDTO{

    private List<CSTask> response;

    public List<CSTask> getResponse() {
        return response;
    }

}
