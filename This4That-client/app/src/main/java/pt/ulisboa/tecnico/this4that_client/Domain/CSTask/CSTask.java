package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.util.Date;
import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;

/**
 * Created by Calado on 30-03-2017.
 */

public class CSTask {

    private String name;
    private Date expirationDate;
    private String topic;
    private TriggerSensor trigger;
    private SensingTask sensingTask;

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
        this.expirationDate = expirationDate;
    }

    public String getTopic() {
        return topic;
    }

    public void setTopic(String topic) {
        this.topic = topic;
    }

    public TriggerSensor getTrigger() {
        return trigger;
    }

    public void setTrigger(TriggerSensor trigger) {
        this.trigger = trigger;
    }

    public SensingTask getSensingTask() {
        return sensingTask;
    }

    public void setSensingTask(SensingTask sensingTask) {
        this.sensingTask = sensingTask;
    }

    public HashMap toHashMap() {
        HashMap<String, Object> jsonElements = new HashMap<>();

        jsonElements.put("name", name);
        jsonElements.put("expDate", expirationDate.toString());
        jsonElements.put("topic", topic);
        jsonElements.put("trigger", trigger.toJSON());
        jsonElements.put("sensingTask", sensingTask.toJSON());

        return jsonElements;
    }
}
