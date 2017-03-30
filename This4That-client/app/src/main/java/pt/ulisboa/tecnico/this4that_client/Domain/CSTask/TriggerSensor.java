package pt.ulisboa.tecnico.this4that_client.Domain.CSTask;

import java.util.HashMap;

import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;

/**
 * Created by Calado on 30-03-2017.
 */

public class TriggerSensor
{
    private SensorType type;
    private String param1;
    private String param2;

    public SensorType getType() {
        return type;
    }

    public void setType(SensorType type) {
        this.type = type;
    }


    public String getParam1() {
        return param1;
    }

    public void setParam1(String param1) {
        this.param1 = param1;
    }

    public String getParam2() {
        return param2;
    }

    public void setParam2(String param2) {
        this.param2 = param2;
    }

    public Object toJSON()
    {
        HashMap<String, String> triggerElems = new HashMap<>();

        triggerElems.put("sensor", type.toString());
        triggerElems.put("param1", (param1 == null ? null : param1));
        triggerElems.put("param2", (param2 == null ? null : param2));

        return  triggerElems;
    }
}
