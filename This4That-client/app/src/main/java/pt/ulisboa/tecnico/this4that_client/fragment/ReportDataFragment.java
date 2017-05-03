package pt.ulisboa.tecnico.this4that_client.fragment;

import android.content.Context;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;

import java.util.ArrayList;
import java.util.List;

import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.Answer;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.CSTask;
import pt.ulisboa.tecnico.this4that_client.Domain.CSTask.InteractiveTask;
import pt.ulisboa.tecnico.this4that_client.Enums.TaskTypeEnum;
import pt.ulisboa.tecnico.this4that_client.GlobalApp;
import pt.ulisboa.tecnico.this4that_client.R;
import pt.ulisboa.tecnico.this4that_client.activity.MainActivity;


public class ReportDataFragment extends Fragment {

    private CSTask task;
    private static GlobalApp globalApp;

    //UI elements
    private TextView txtQuestion;
    private Button btnAnswer1;
    private Button btnAnswer2;
    private Button btnAnswer3;
    private Button btnAnswer4;

    public ReportDataFragment() {
        // Required empty public constructor
    }

    public MainActivity getParentActivity(){
        return (MainActivity) getActivity();
    }

    public static ReportDataFragment newInstance(String taskId) {
        ReportDataFragment fragment = new ReportDataFragment();
        Bundle args = new Bundle();
        args.putString("taskId", taskId);
        fragment.setArguments(args);
        return fragment;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        String taskID;

        super.onCreate(savedInstanceState);
        this.globalApp = (GlobalApp) getParentActivity().getApplicationContext();
        taskID = getArguments().getString("taskId");
        this.task = globalApp.getMyTasks().get(taskID);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View actualView = inflater.inflate(R.layout.fragment_report_data, container, false);

        setupUIElements(actualView);

        return actualView;
    }



    private void setupUIElements(View view) {

        InteractiveTask interactiveTask;

        interactiveTask = task.getInteractiveTask();
        if (interactiveTask != null){
            //set question ui element
            this.txtQuestion = (TextView) view.findViewById(R.id.txtQuestion);
            this.txtQuestion.setText(interactiveTask.getQuestion());

            setButtons(view, interactiveTask);

        }

    }

    private void setButtons(View view, InteractiveTask interactiveTask){


        Answer answer;

        //first Button
        btnAnswer1 = (Button) view.findViewById(R.id.btnFirstAnswer);
        if (interactiveTask.getAnswers().size() > 0)
        {
            answer = interactiveTask.getAnswers().get(0);
            //the tag will be used to save the answerId
            btnAnswer1.setTag(answer.getAnswerId());
            btnAnswer1.setText(answer.getAnswer());
            btnAnswer1.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    globalApp.getServerAPI().reportTask(ReportDataFragment.this, btnAnswer1.getTag()
                                                        , task.getTaskID(), globalApp.getUserInfo().getUserId()
                                                        , TaskTypeEnum.InteractiveTask);
                }
            });
        }
        else
            btnAnswer1.setVisibility(View.GONE);

        //secondButton
        btnAnswer2 = (Button) view.findViewById(R.id.btnSecondAnswer);
        if (interactiveTask.getAnswers().size() > 1)
        {
            answer = interactiveTask.getAnswers().get(1);
            btnAnswer2.setTag(answer.getAnswerId());
            btnAnswer2.setText(answer.getAnswer());
            btnAnswer2.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    globalApp.getServerAPI().reportTask(ReportDataFragment.this, btnAnswer2.getTag()
                            , task.getTaskID(), globalApp.getUserInfo().getUserId()
                            , TaskTypeEnum.InteractiveTask);
                }
            });
        }
        else
            btnAnswer2.setVisibility(View.GONE);

        //thirdButton
        btnAnswer3 = (Button) view.findViewById(R.id.btnThirdAnswer);
        if (interactiveTask.getAnswers().size() > 2)
        {
            answer = interactiveTask.getAnswers().get(2);
            btnAnswer3.setTag(answer.getAnswerId());
            btnAnswer3.setText(answer.getAnswer());
            btnAnswer3.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    globalApp.getServerAPI().reportTask(ReportDataFragment.this, btnAnswer3.getTag()
                            , task.getTaskID(), globalApp.getUserInfo().getUserId()
                            , TaskTypeEnum.InteractiveTask);
                }
            });
        }
        else
            btnAnswer3.setVisibility(View.GONE);

        //fourth button
        btnAnswer4 = (Button) view.findViewById(R.id.btnFourthAnswer);
        if (interactiveTask.getAnswers().size() > 3)
        {
            answer = interactiveTask.getAnswers().get(3);
            btnAnswer4.setTag(answer.getAnswerId());
            btnAnswer4.setText(answer.getAnswer());
            btnAnswer4.setOnClickListener(new View.OnClickListener() {
                @Override
                public void onClick(View view) {
                    globalApp.getServerAPI().reportTask(ReportDataFragment.this, btnAnswer4.getTag()
                            , task.getTaskID(), globalApp.getUserInfo().getUserId()
                            , TaskTypeEnum.InteractiveTask);
                }
            });
        }
        else
            btnAnswer4.setVisibility(View.GONE);
    }

}
