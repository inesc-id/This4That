package pt.ulisboa.tecnico.this4that_client.JSON.DTO;

/**
 * Created by Calado on 05-04-2017.
 */

public class TaskCostResponseDTO extends APIResponseDTO{

    private TaskCostValues response;


    public TaskCostValues getResponse() {
        return response;
    }
    //nested class
    public class TaskCostValues{
        private String valToPay;

        public String getValToPay() {
            return valToPay;
        }
    }
}
