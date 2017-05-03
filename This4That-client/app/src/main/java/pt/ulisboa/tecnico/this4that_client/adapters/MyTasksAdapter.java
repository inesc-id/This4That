package pt.ulisboa.tecnico.this4that_client.adapters;

import android.support.v4.app.FragmentActivity;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.fragment.MyTasksFragment;
import pt.ulisboa.tecnico.this4that_client.fragment.ReportMainFragment;

/**
 * Created by Calado on 11-04-2017.
 */

public class MyTasksAdapter extends RecyclerView.Adapter<MyTasksAdapter.ViewHolder> {

    private List<CSTask> colTasks;
    private MyTasksFragment myTasksFragment;

    public MyTasksAdapter(List<CSTask> tasks, MyTasksFragment fragment){
        this.colTasks = tasks;
        this.myTasksFragment = fragment;
    }


    @Override
    public MyTasksAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.mytasks_row, parent, false);
        ViewHolder viewHolder = new ViewHolder(view);

        return viewHolder;
    }

    @Override
    public void onBindViewHolder(MyTasksAdapter.ViewHolder holder, final int position) {
        holder.itemView.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                myTasksFragment.replaceFragment(ReportMainFragment.newInstance(colTasks.get(position))
                                                , false, false);

            }
        });
        holder.txtTaskName.setText(this.colTasks.get(position).getName());
    }

    @Override
    public int getItemCount() {
        return colTasks.size();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder{

        private TextView txtTaskName;
        private Button btnSubscribe;

        public ViewHolder(View view) {
            super(view);
            txtTaskName = (TextView) view.findViewById(R.id.txtTaskName);
            btnSubscribe = (Button) view.findViewById(R.id.btnSubscribeTask);
        }
    }
}
