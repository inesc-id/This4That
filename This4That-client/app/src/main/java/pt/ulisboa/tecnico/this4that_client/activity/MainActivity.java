package pt.ulisboa.tecnico.this4that_client.activity;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.app.FragmentManager;
import android.support.v4.app.FragmentTransaction;
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

import java.util.Date;


import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.SensingTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.TriggerSensor;
import pt.ulisboa.tecnico.this4that_client.Domain.UserInfo;
import pt.ulisboa.tecnico.this4that_client.Enums.SensorType;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.fragment.MyTasksFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.SearchTopicsFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.SubscribedTasksFragment;
import pt.ulisboa.tecnico.this4that_client.serviceLayer.ServerAPI;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener {

    private Button btnCalcTaskCost;
    private Button btnCreateTask;
    private TextView txtRefToPay;
    private TextView txtValToPay;
    private TextView txtTaskID;
    private TextView txtTxID;
    //global
    private static GlobalApp globalApp;



    @Override
    protected void onCreate(Bundle savedInstanceState){
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        //toolbar
        Toolbar toolbar = (Toolbar) findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        //navigationView
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.mainDrawerLayout);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.setDrawerListener(toggle);
        toggle.syncState();
        NavigationView navigationView = (NavigationView) findViewById(R.id.nav_view);
        navigationView.setNavigationItemSelectedListener(this);
        this.globalApp = (GlobalApp) getApplicationContext();
        //FIXME: remove
        this.globalApp.setServerAPI(new ServerAPI());
        this.globalApp.setUserInfo(new UserInfo("1234"));
    }


    /**
     * Loads new fragment
     *
     * @param fragment - fragment to be showed
     */
    private void replaceFragment(Fragment fragment, boolean explicitReplace, boolean addToBackStack){
        String backStateName =  fragment.getClass().getName();
        String fragmentTag = backStateName;
        boolean fragmentPopped = false;

        FragmentManager manager = getSupportFragmentManager();

        if(!explicitReplace){
            fragmentPopped = manager.popBackStackImmediate (backStateName, 0);
        }

        if(!fragmentPopped && manager.findFragmentByTag(fragmentTag) == null){ //fragment not in back stack, create it.

            FragmentTransaction ft = manager.beginTransaction();
            ft.replace(R.id.content_frame, fragment, fragmentTag);
            if(addToBackStack) {
                ft.addToBackStack(backStateName);
            }
            ft.commit();
        }
        else if(explicitReplace){

            manager.popBackStack();
            FragmentTransaction ft = manager.beginTransaction();
            ft.replace(R.id.content_frame, fragment, fragmentTag);
            ft.addToBackStack(backStateName);
            ft.commit();
        }
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
        return  csTask;
    }


    @Override
    public void onBackPressed() {
        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.mainDrawerLayout);
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

        if (id == R.id.myTasksitem) {
            Fragment fragment = new MyTasksFragment();
            replaceFragment(fragment, false, false);
        } else if (id == R.id.subscribedTasks){
            Fragment fragment = new SubscribedTasksFragment();
            replaceFragment(fragment, false, false);

        }else if (id == R.id.searchTasks) {
            Fragment fragment = new SearchTopicsFragment();
            replaceFragment(fragment, false, false);
        }else if (id == R.id.nav_share) {

        } else if (id == R.id.nav_send) {

        }

        DrawerLayout drawer = (DrawerLayout) findViewById(R.id.mainDrawerLayout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }


    public TextView getTxtValToPay() {
        return txtValToPay;
    }

}
