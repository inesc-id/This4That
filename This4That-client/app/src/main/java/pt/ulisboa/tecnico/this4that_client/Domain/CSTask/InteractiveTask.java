package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.util.List;

/**
 * Created by Calado on 11-04-2017.
 */

public class InteractiveTask {

    private String question;
    private List<Answers> answers;

    public InteractiveTask() {
    }

    public class Answers{
        private String answer;
    }
}
