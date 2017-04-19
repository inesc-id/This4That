package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import android.text.AndroidCharacter;
import android.util.Pair;

import java.util.ArrayList;
import java.util.HashMap;

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

    public HashMap<String, Object> toHashMap() {

        HashMap<String, Object> hashInteractiveTask = new HashMap<>();
        ArrayList<HashMap<String, String>> jsonAnswers;
        HashMap<String, String> jsonAnswer;

        hashInteractiveTask.put("question", question);
        hashInteractiveTask.put("answers", new ArrayList<Pair<String, String>>());
        for (Answer answer : answers) {
            jsonAnswers = (ArrayList)hashInteractiveTask.get("answers");
            jsonAnswer = new HashMap<>();
            jsonAnswer.put("answer", answer.getAnswer());
            jsonAnswers.add(jsonAnswer);
        }
        return hashInteractiveTask;
    }
}
