package pt.ulisboa.tecnico.this4that_client.adapters;

import android.content.Context;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.R;

/**
 * Created by Calado on 11-04-2017.
 */

public class TopicsAdapter extends RecyclerView.Adapter<TopicsAdapter.ViewHolder> {

    private List<String> topics;

    public TopicsAdapter(List<String> topics){
        this.topics = topics;
    }


    @Override
    public TopicsAdapter.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(parent.getContext()).inflate(R.layout.topics_row, parent, false);
        ViewHolder viewHolder = new ViewHolder(view, parent.getContext());

        return viewHolder;
    }

    @Override
    public void onBindViewHolder(TopicsAdapter.ViewHolder holder, int position) {
        holder.txtTaskName.setText(this.topics.get(position));
    }

    @Override
    public int getItemCount() {
        return topics.size();
    }

    public static class ViewHolder extends RecyclerView.ViewHolder{

        private TextView txtTaskName;
        private Button btnSubscribe;

        public ViewHolder(View view, final Context context) {
            super(view);
            txtTaskName = (TextView) view.findViewById(R.id.txtTopicName);
            btnSubscribe = (Button) view.findViewById(R.id.btnSubscribeTask);
            btnSubscribe.setOnClickListener(new View.OnClickListener(){

                @Override
                public void onClick(View view) {
                    Toast.makeText(context, "Subscribe", Toast.LENGTH_SHORT).show();
                }
            });
        }
    }
}

