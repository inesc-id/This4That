package pt.ulisboa.tecnico.this4that_client.fragment;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v4.widget.SwipeRefreshLayout;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;

/**
 * Created by Calado on 11-04-2017.
 */

public class SearchTopicsFragment extends Fragment {
    //layout
    private RecyclerView recyclerView;
    private LinearLayoutManager linearLayoutManager;
    private SwipeRefreshLayout swipeLayout;
    //global
    private static GlobalApp globalApp;

    public SearchTopicsFragment() {

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
        View view;
        // Inflate the layout for this fragment
        view = inflater.inflate(R.layout.fragment_search_topics, container, false);
        swipeLayout = (SwipeRefreshLayout) view.findViewById(R.id.refreshSearchTopics);
        swipeLayout.setOnRefreshListener(new SwipeRefreshLayout.OnRefreshListener() {
            @Override
            public void onRefresh() {
                getTopics();
            }
        });
        recyclerView = (RecyclerView) view.findViewById(R.id.searchTasksRecycleView);
        linearLayoutManager = new LinearLayoutManager(getActivity());
        recyclerView.setLayoutManager(linearLayoutManager);
        getTopics();
        return view;
    }

    public RecyclerView getRecyclerView() {
        return recyclerView;
    }

    public SwipeRefreshLayout getSwipeLayout() {
        return swipeLayout;
    }

    public void getTopics(){
        globalApp.getServerAPI().getTopics(this);
    }

}
