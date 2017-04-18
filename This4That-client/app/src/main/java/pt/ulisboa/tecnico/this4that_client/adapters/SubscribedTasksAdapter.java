package pt.ulisboa.tecnico.this4that_client.adapters;

import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.R;

/**
 * Created by Calado on 11-04-2017.
 */

public class SubscribedTasksAdapter extends RecyclerView.Adapter<SubscribedTasksAdapter.ViewHolder> {

    private List<CSTask> colTasks;

    public SubscribedTasksAdapter(List<CSTask> tasks){
        this.colTasks = tasks;
    }


    @Override
    public SubscribedTasksAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.subscribed_tasks_row, parent, false);
        ViewHolder viewHolder = new ViewHolder(view);

        return viewHolder;
    }

    @Override
    public void onBindViewHolder(SubscribedTasksAdapter.ViewHolder holder, int position) {
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
