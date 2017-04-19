package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;

/**
 * Created by Calado on 30-03-2017.
 */

public class SensingTask {
    private SensorType sensor = null;

    public SensorType getSensor() {
        return sensor;
    }

    public void setSensor(SensorType sensor) {
        this.sensor = sensor;
    }


    public HashMap<String, String> toHashMap(){
        HashMap<String, String> hashSensingTask = new HashMap<>();

        hashSensingTask.put("sensor", (sensor == null ? null : sensor.toString()));
        return  hashSensingTask;
    }
}
