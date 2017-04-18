package pt.ulisboa.tecnico.this4that_client.JSON.DTO;

import java.util.List;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetTopicsResponseDTO extends APIResponseDTO{

    private List<String> response;

    public List<String> getResponse() {
        return response;
    }
}
