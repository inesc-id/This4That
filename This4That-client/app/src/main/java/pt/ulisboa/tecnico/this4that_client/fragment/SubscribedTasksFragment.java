package pt.ulisboa.tecnico.this4that_client.fragment;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;

/**
 * Created by Calado on 11-04-2017.
 */

public class SubscribedTasksFragment extends Fragment {

    //layout
    private RecyclerView recyclerView;
    private LinearLayoutManager linearLayoutManager;
    //global
    private static GlobalApp globalApp;

    public SubscribedTasksFragment() {
        // Required empty public constructor
    }
    public MainActivity getParentActivity(){
        return (MainActivity) getActivity();
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        this.globalApp = (GlobalApp) getParentActivity().getApplicationContext();
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        View view = inflater.inflate(R.layout.fragment_subscribed_tasks, container, false);
        recyclerView = (RecyclerView) view.findViewById(R.id.subTasksRecycleView);
        linearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerView.setLayoutManager(linearLayoutManager);
        this.globalApp.getServerAPI().getSubscribedTasks(this.globalApp.getUserInfo().getUserId(), this);
        return view;
    }

    public RecyclerView getRecyclerView() {
        return recyclerView;
    }
}
