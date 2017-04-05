package pt.ulisboa.tecnico.this4that_client.Domain.DTO;

/**
 * Created by Calado on 05-04-2017.
 */

public class TaskCostResponseDTO {

    private int errorCode;
    private TaskCostValues response;


    public int getErrorCode() {
        return errorCode;
    }

    public TaskCostValues getResponse() {
        return response;
    }
    //nested class
    public class TaskCostValues{
        private String refToPay;
        private String valToPay;

        public String getRefToPay() {
            return refToPay;
        }

        public String getValToPay() {
            return valToPay;
        }
    }
}
