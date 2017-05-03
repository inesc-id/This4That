package pt.ulisboa.tecnico.this4that_client.fragment;

import android.os.Bundle;
import android.support.design.widget.TabLayout;
import android.support.v4.app.Fragment;
import android.support.v4.view.ViewPager;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.adapters.ReportViewPageAdapter;


public class ReportMainFragment extends Fragment {

    private ViewPager viewPager = null;
    private TabLayout tabLayout = null;
    private int[] tabIcons = {
    };

    private String taskId;


    public ReportMainFragment() {
        // Required empty public constructor
    }

    public static ReportMainFragment newInstance(CSTask task) {
        ReportMainFragment fragment = new ReportMainFragment();
        Bundle args = new Bundle();
        args.putString("taskId", task.getTaskID());
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {

        super.onCreate(savedInstanceState);
        this.taskId = getArguments().getString("taskId");
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        View actualView = null;

        try{
            actualView = inflater.inflate(R.layout.fragment_report_main, container, false);
            //setup Tabs
            viewPager = (ViewPager) actualView.findViewById(R.id.viewpager);
            setupViewPager(viewPager);
            tabLayout = (TabLayout) actualView.findViewById(R.id.tabs);
            tabLayout.setupWithViewPager(viewPager);
            setupTabIcons();
            return actualView;

        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
            return actualView;
        }
    }

    private void setupViewPager(ViewPager viewPager) {
        ReportViewPageAdapter pageAdapter = new ReportViewPageAdapter(getChildFragmentManager());

        pageAdapter.addFragment(ReportDataFragment.newInstance(taskId), "Report");
        pageAdapter.addFragment(new TaskResultsFragment(), "Results");
        viewPager.setAdapter(pageAdapter);
    }

    private void setupTabIcons(){
        //tabLayout.getTabAt(0).setIcon(tabIcons[0]);
    }

}
