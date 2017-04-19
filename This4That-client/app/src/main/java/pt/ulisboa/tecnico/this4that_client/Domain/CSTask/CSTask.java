package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.text.SimpleDateFormat;
import java.util.Calendar;
import java.util.Date;
import java.util.HashMap;

/**
 * Created by Calado on 30-03-2017.
 */

public class CSTask {

    private String name;
    private Date expirationDate;
    private String topic;
    private SensingTask sensingTask;
    private String taskID;
    private InteractiveTask interactiveTask;

    public String getTaskID() {
        return taskID;
    }

    public void setTaskID(String taskID) {
        this.taskID = taskID;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public Date getExpirationDate() {
        return expirationDate;
    }

    public void setExpirationDate(Date expirationDate) {

        Calendar cal = Calendar.getInstance();
        cal.setTime(expirationDate);
        //add one week
        cal.add(Calendar.DATE, 7);//7 days
        this.expirationDate = cal.getTime();
    }

    public String getTopic() {
        return topic;
    }

    public void setTopic(String topic) {
        this.topic = topic;
    }

    public SensingTask getSensingTask() {
        return sensingTask;
    }

    public void setSensingTask(SensingTask sensingTask) {
        this.sensingTask = sensingTask;
    }

    public InteractiveTask getInteractiveTask() {
        return interactiveTask;
    }

    public void setInteractiveTask(InteractiveTask interactiveTask) {
        this.interactiveTask = interactiveTask;
    }

    public HashMap toHashMap() {
        HashMap<String, Object> jsonElements = new HashMap<>();

        jsonElements.put("name", name);
        jsonElements.put("expDate", new SimpleDateFormat("yyyy-MM-dd HH:mm:ss").format(expirationDate));
        jsonElements.put("topic", topic);
        jsonElements.put("interactiveTask", interactiveTask.toHashMap());

        return jsonElements;
    }
}
