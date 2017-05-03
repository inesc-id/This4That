package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

/**
 * Created by Calado on 18-04-2017.
 */

public class Answer {
    private String answer;
    private String answerId;

    public Answer(String answer){
        this.answer = answer;
    }

    public String getAnswer() {
        return answer;
    }

    public String getAnswerId() {
        return answerId;
    }
}