package pt.ulisboa.tecnico.this4that_client.fragment;

import android.content.Intent;
import android.os.Bundle;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.app.Fragment;
import android.support.v7.app.ActionBar;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.activity.CreateTaskActivity;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;
import pt.ulisboa.tecnico.this4that_client.serviceLayer.ServerAPI;

public class MyTasksFragment extends Fragment {
    //layout
    private RecyclerView recyclerView;
    private LinearLayoutManager linearLayoutManager;
    private FloatingActionButton btnCreateTask;
    //global
    private static GlobalApp globalApp;

    public MyTasksFragment() {

        // Required empty public constructor
    }
    public MainActivity getParentActivity(){
        return (MainActivity) getActivity();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        this.globalApp = (GlobalApp) getParentActivity().getApplicationContext();
        //FIXME: remove
        this.globalApp.setServerAPI(new ServerAPI());
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_my_tasks, container, false);
        recyclerView = (RecyclerView) view.findViewById(R.id.myTasksRecycleView);
        linearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerView.setLayoutManager(linearLayoutManager);
        btnCreateTask = (FloatingActionButton) view.findViewById(R.id.btnCreateTask);
        btnCreateTask.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent intent = new Intent(getParentActivity(), CreateTaskActivity.class);
                startActivity(intent);
            }
        });

        this.globalApp.getServerAPI().getMyTasks(this.globalApp.getUserInfo().getUserId()
                                                 , this);
        return view;
    }

    public RecyclerView getRecyclerView() {
        return recyclerView;
    }

    public void setRecyclerView(RecyclerView recyclerView) {
        this.recyclerView = recyclerView;
    }

    public LinearLayoutManager getLinearLayoutManager() {
        return linearLayoutManager;
    }

    public void setLinearLayoutManager(LinearLayoutManager linearLayoutManager) {
        this.linearLayoutManager = linearLayoutManager;
    }
}
