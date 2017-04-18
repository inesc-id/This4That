package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.util.ArrayList;

/**
 * Created by Calado on 11-04-2017.
 */

public class InteractiveTask {

    private String question;
    private ArrayList<Answer> answers = new ArrayList<>();

    public InteractiveTask() {

    }

    public String getQuestion() {
        return question;
    }

    public void setQuestion(String question) {
        this.question = question;
    }

    public ArrayList<Answer> getAnswers() {
        return answers;
    }

    public void setAnswers(ArrayList<Answer> answers) {
        this.answers = answers;
    }
}
