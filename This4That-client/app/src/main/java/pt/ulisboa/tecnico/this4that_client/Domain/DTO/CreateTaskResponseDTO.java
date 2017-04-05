package pt.ulisboa.tecnico.this4that_client.Domain.DTO;

/**
 * Created by Calado on 05-04-2017.
 */

public class CreateTaskResponseDTO {
    private int errorCode;
    private CreateTaskResponse response;


    public int getErrorCode() {
        return errorCode;
    }

    public CreateTaskResponse getResponse() {
        return response;
    }
    //nested class
    public class CreateTaskResponse{
        private String taskId;

        public String getTaskId() {
            return taskId;
        }
    }
}
