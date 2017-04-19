package pt.ulisboa.tecnico.this4that_client.fragment;

import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.support.v4.app.DialogFragment;
import android.support.v7.app.AlertDialog;
import android.util.Log;

import pt.ulisboa.tecnico.this4that_client.JSON.DTO.GetMyTasksResponseDTO;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.activity.CreateTaskActivity;


public class ConfirmTaskCreation extends DialogFragment {

    public static ConfirmTaskCreation newInstance(String message, String postBody) {
        ConfirmTaskCreation frag = new ConfirmTaskCreation();
        Bundle args = new Bundle();
        args.putString("message", message);
        args.putString("postBody", postBody);
        frag.setArguments(args);
        return frag;
    }

    @Override
    public Dialog onCreateDialog(Bundle savedInstanceState) {

        String message = getArguments().getString("message");
        final String postBody = getArguments().getString("postBody");

        // Use the Builder class for convenient dialog construction
        AlertDialog.Builder builder = new AlertDialog.Builder(getActivity());
        builder.setTitle("Confirm Task Creation");
        builder.setMessage(message)
               .setPositiveButton("Confirm", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {
                        createRemoteTask(postBody);
                    }
                })
               .setNegativeButton("Cancel", new DialogInterface.OnClickListener() {
                    public void onClick(DialogInterface dialog, int id) {

                    }
                });
        // Create the AlertDialog object and return it
        return builder.create();
    }

    public boolean createRemoteTask(String postBody){

        CreateTaskActivity createTaskActivity;

        try{
            createTaskActivity = (CreateTaskActivity) getActivity();

            if (!createTaskActivity.getGlobalApp()
                                   .getServerAPI()
                                   .createCSTask(postBody, createTaskActivity)){
                return false;
            }
            return true;

        }catch (Exception ex){
            Log.d("This4That", ex.getMessage());
            return false;
        }
    }
}
