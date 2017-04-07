package pt.ulisboa.tecnico.this4that_client;

import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.design.widget.Snackbar;
import android.util.Log;
import android.view.View;
import android.support.design.widget.NavigationView;
import android.support.v4.view.GravityCompat;
import android.support.v4.widget.DrawerLayout;
import android.support.v7.app.ActionBarDrawerToggle;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.Toolbar;
import android.view.Menu;
import android.view.MenuItem;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import org.w3c.dom.Text;

import java.util.Date;


import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.SensingTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.TriggerSensor;
import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;
import pt.ulisboa.tecnico.this4that_client.applicationLayer.HttpClient;
import pt.ulisboa.tecnico.this4that_client.serviceLayer.ServerAPI;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private Button btnCalcTaskCost;
    private Button btnCreateTask;
    private TextView txtRefToPay;
    private TextView txtValToPay;
    private TextView txtTaskID;
    private TextView txtTxID;
    private ServerAPI serverAPI;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        btnCalcTaskCost = (Button) findViewById(R.id.btnCalcTaskCost);
        btnCreateTask = (Button) findViewById(R.id.btnCreateTask);
        txtRefToPay = (TextView) findViewById(R.id.txtRefToPayFill);
        txtValToPay = (TextView) findViewById(R.id.txtValToPayFill);
        txtTaskID = (TextView) findViewById(R.id.txtTaskIDFill);
        txtTxID = (TextView) findViewById(R.id.txtTxFill);

        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);

        FloatingActionButton fab = (FloatingActionButton) findViewById(R.id.fab);
        fab.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Snackbar.make(view, "Replace with your own action", Snackbar.LENGTH_LONG)
                        .setAction("Action", null).show();
            }
        });

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.setDrawerListener(toggle);
        toggle.syncState();

        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);
        serverAPI = new ServerAPI("http://http://194.210.232.1:58949/api/", "1234");


        btnCalcTaskCost.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                CSTask task = CreateDummyTask();
                serverAPI.CalcTaskCostAPI(task, MainActivity.this);
            }
        });
        btnCreateTask.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                CSTask task = CreateDummyTask();
                serverAPI.CreateCSTask(task, MainActivity.this, getTxtRefToPay().getText().toString());
            }
        });
    }

    public CSTask CreateDummyTask(){
        CSTask csTask = new CSTask();
        SensingTask sensingTask = new SensingTask();
        sensingTask.setSensor(SensorType.TEMPERATURE);
        TriggerSensor triggerSensor = new TriggerSensor();
        triggerSensor.setType(SensorType.GPS);
        triggerSensor.setParam1("50.2");
        triggerSensor.setParam2("10");

        csTask.setExpirationDate(new Date());
        csTask.setName("TaskTeste");
        csTask.setTopic("temperature");
        csTask.setSensingTask(sensingTask);
        csTask.setTrigger(triggerSensor);
        return  csTask;
    }


    @Override
    public void onBackPressed() {
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        if (id == R.id.nav_camera) {
            // Handle the camera action
        } else if (id == R.id.nav_gallery) {

        } else if (id == R.id.nav_slideshow) {

        } else if (id == R.id.nav_manage) {

        } else if (id == R.id.nav_share) {

        } else if (id == R.id.nav_send) {

        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }

    public Button getBtnCalcTaskCost() {
        return btnCalcTaskCost;
    }

    public void setBtnCalcTaskCost(Button btnCalcTaskCost) {
        this.btnCalcTaskCost = btnCalcTaskCost;
    }

    public Button getBtnCreateTask() {
        return btnCreateTask;
    }

    public void setBtnCreateTask(Button btnCreateTask) {
        this.btnCreateTask = btnCreateTask;
    }

    public TextView getTxtRefToPay() {
        return txtRefToPay;
    }

    public void setTxtRefToPay(TextView txtRefToPay) {
        this.txtRefToPay = txtRefToPay;
    }

    public TextView getTxtValToPay() {
        return txtValToPay;
    }

    public void setTxtValToPay(TextView txtValToPay) {
        this.txtValToPay = txtValToPay;
    }

    public TextView getTxtTaskID() {
        return txtTaskID;
    }

    public void setTxtTaskID(TextView txtTaskID) {
        this.txtTaskID = txtTaskID;
    }

    public TextView getTxtTxID() {
        return txtTxID;
    }
}
