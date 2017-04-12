package pt.ulisboa.tecnico.this4that_client.Domain.DTO;

import java.util.List;

/**
 * Created by Calado on 11-04-2017.
 */

public class GetTopicsResponseDTO {

    private int errorCode;
    private List<String> response;


    public List<String> getResponse() {
        return response;
    }
}
