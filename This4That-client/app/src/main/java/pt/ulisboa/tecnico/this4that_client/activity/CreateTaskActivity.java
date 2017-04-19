package pt.ulisboa.tecnico.this4that_client.activity;

import android.os.Bundle;
import android.support.annotation.IdRes;
import android.support.design.widget.Snackbar;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.util.Log;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.widget.EditText;
import android.widget.RadioButton;
import android.widget.RadioGroup;
import android.widget.RelativeLayout;
import android.widget.Toast;

import java.util.Date;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.Answer;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.InteractiveTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.SensingTask;
import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;

public class CreateTaskActivity extends AppCompatActivity {

    private static GlobalApp globalApp;

    //UI elements
    private EditText txtTaskName;
    private EditText txtTopicName;
    private RadioButton radBtnSensingTask;
    private RadioButton radBtnInteractiveTask;
    private RadioGroup radGroupSensingTask;
    private EditText txtQuestion;
    private EditText txtAnswer1;
    private EditText txtAnswer2;
    private EditText txtAnswer3;
    private EditText txtAnswer4;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_create_task);
        setUIElements();
        globalApp = (GlobalApp) getApplicationContext();

    }

    public void setUIElements(){
        //toolbar
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        getSupportActionBar().setDisplayHomeAsUpEnabled(true);

        //radio buttons
        RadioGroup radioGroup = (RadioGroup) findViewById(R.id.radioGroupTaskType);
        radioGroup.setOnCheckedChangeListener(new RadioGroup.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(RadioGroup radioGroup, @IdRes int i) {
                RelativeLayout layout_InterTask = (RelativeLayout) findViewById(R.id.layout_Interactive_Task);
                RelativeLayout layout_SensTask = (RelativeLayout) findViewById(R.id.layout_SensingTask);

                switch (i){
                    case R.id.radBtnInteractiveTask:
                        layout_InterTask.setVisibility(View.VISIBLE);
                        layout_SensTask.setVisibility(View.GONE);
                        break;

                    case R.id.radBtnSensingTask:
                        layout_InterTask.setVisibility(View.GONE);
                        layout_SensTask.setVisibility(View.VISIBLE);
                        break;
                }
            }
        });
        this.radBtnSensingTask = (RadioButton) findViewById(R.id.radBtnSensingTask);
        this.radBtnInteractiveTask = (RadioButton) findViewById(R.id.radBtnInteractiveTask);
        this.radGroupSensingTask = (RadioGroup) findViewById(R.id.radioGroupSensorType);

        //TextBoxes
        this.txtTaskName = (EditText) findViewById(R.id.txtTaskName);
        this.txtTopicName = (EditText) findViewById(R.id.txtTopicName);
        this.txtQuestion = (EditText) findViewById(R.id.txtQuestion);
        this.txtAnswer1 = (EditText) findViewById(R.id.txtAnswer1);
        this.txtAnswer2 = (EditText) findViewById(R.id.txtAnswer2);
        this.txtAnswer3 = (EditText) findViewById(R.id.txtAnswer3);
        this.txtAnswer4 = (EditText) findViewById(R.id.txtAnswer4);
    }

    @Override
    public boolean onSupportNavigateUp(){
        onBackPressed();
        return true;
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.menu_create_task, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item){

        CSTask task;

        switch (item.getItemId()){
            case R.id.action_create_task:
                /*task = createTask();
                validateTask(task);*/
                task = createDummyTask();
                globalApp.getServerAPI().calcTaskCostAPI(task, this);
                return  true;
            default:
                return super.onOptionsItemSelected(item);
        }
    }

    private boolean validateTask(CSTask task) {

        String topicRegex = "^[a-zA-Z0-9[-]*]*";
        Pattern pattern = Pattern.compile(topicRegex);
        Matcher matcher;
        int emptyResponses = 0;
        try{
            //validate task Name
            if (task.getName().isEmpty() || task.getName().length() > 50){
                txtTaskName.setError("Invalid task name!");
                txtTaskName.requestFocus();
                return false;
            }
            //validate topic name
            matcher = pattern.matcher(task.getTopic());
            if (task.getTopic().isEmpty() || !matcher.find()){
                txtTopicName.setError("Avoid special characters!");
                txtTopicName.requestFocus();
                return false;
            }
            //validate Sensing Task
            if (radBtnSensingTask.isChecked() && task.getSensingTask() == null){
                Toast.makeText(this, "Please choose a sensor!", Toast.LENGTH_SHORT).show();
                return false;
            }
            //validate Interactive Task
            if (radBtnInteractiveTask.isChecked()){
                if (task.getInteractiveTask().getQuestion().isEmpty() ||
                        task.getInteractiveTask().getQuestion().length() > 255){
                    txtQuestion.setError("Invalid question!");
                    txtQuestion.requestFocus();
                    return false;
                }
                for (Answer answer : task.getInteractiveTask().getAnswers()) {
                    if (answer.getAnswer().isEmpty())
                        emptyResponses++;
                }
                if (emptyResponses > 2){
                    Toast.makeText(this, "Write 2 responses at least!", Toast.LENGTH_LONG).show();
                    return false;
                }
            }
            if (!radBtnInteractiveTask.isChecked() && !radBtnSensingTask.isChecked()){
                Toast.makeText(this, "Select one type of task!", Toast.LENGTH_LONG).show();
            }
            return true;

        }catch (Exception ex){
            Toast.makeText(this, "Invalid Task Configuration", Toast.LENGTH_LONG).show();
            return false;
        }
    }

    public static GlobalApp getGlobalApp() {
        return globalApp;
    }

    public CSTask createTask(){

        CSTask task = new CSTask();
        int radBtnID;
        RadioButton radBtnSelected;
        SensingTask sensingTask;
        InteractiveTask interactiveTask;

        try{
            task.setName(this.txtTaskName.getText().toString());
            task.setTopic(this.txtTopicName.getText().toString());
            task.setExpirationDate(new Date());
            radBtnID = this.radGroupSensingTask.getCheckedRadioButtonId();

            //if is a sensing task
            if (this.radBtnSensingTask.isChecked() && radBtnID != -1){
                sensingTask = new SensingTask();
                radBtnSelected = (RadioButton) findViewById(radBtnID);
                sensingTask.setSensor(SensorType.valueOf(radBtnSelected.getText().toString().toUpperCase()));
                task.setSensingTask(sensingTask);
            }
            //if is a interactive task
            if (this.radBtnInteractiveTask.isChecked()){
                interactiveTask = new InteractiveTask();
                interactiveTask.setQuestion(this.txtQuestion.getText().toString());
                interactiveTask.getAnswers().add(new Answer(this.txtAnswer1.getText().toString()));
                interactiveTask.getAnswers().add(new Answer(this.txtAnswer2.getText().toString()));
                interactiveTask.getAnswers().add(new Answer(this.txtAnswer3.getText().toString()));
                interactiveTask.getAnswers().add(new Answer(this.txtAnswer4.getText().toString()));
                task.setInteractiveTask(interactiveTask);
            }
            return task;
        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
            return null;
        }
    }

    public CSTask createDummyTask(){
        CSTask task = new CSTask();
        InteractiveTask interactiveTask;

        try{
            task.setName("Cantina do Social");
            task.setTopic("TecnicoLisboa");
            task.setExpirationDate(new Date());

            interactiveTask = new InteractiveTask();
            interactiveTask.setQuestion("Como está a fila para almoçar?");
            interactiveTask.getAnswers().add(new Answer("Vazia"));
            interactiveTask.getAnswers().add(new Answer("Algumas Pessoas"));
            interactiveTask.getAnswers().add(new Answer("Muitas pessoas"));
            task.setInteractiveTask(interactiveTask);
            return task;
        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
            return null;
        }

    }
}
