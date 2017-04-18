package pt.ulisboa.tecnico.this4that_client.JSON.DTO;

/**
 * Created by Calado on 05-04-2017.
 */

public class CreateTaskResponseDTO extends APIResponseDTO{

    private CreateTaskResponse response;

    public CreateTaskResponse getResponse() {
        return response;
    }
    //nested class
    public class CreateTaskResponse{
        private String taskId;
        private String transactionId;

        public String getTaskId() {
            return taskId;
        }

        public String getTransactionId() {
            return transactionId;
        }
    }
}
