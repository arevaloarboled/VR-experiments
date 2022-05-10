library(psychTestR)
library(htmltools)
library(shiny)
library(plumber)
library(jsonlite)

print("Run Unity.... waiting for Unity...");
server = make.socket(host="localhost",port=6000,server=T)


questions <- join(
  
  
  one_button_page(
    div(class="text-left",
        renderText({
          invalidateLater(1000)
          paste("The current time is", Sys.time())
        }),
        h3( "Thank you for participating in the experiment."),
        p("Supervisor: Julian Villegas"),
        p("Description: As part of a research project, we are investigating how people perceive the sound presented through headphones. In this experiment, you will be asked to tell:"),
        p ("* Whether the sound can be heard outside or inside the head"),
        p ("* its apparent movement"),
        p ("* Its origin and final position"),
        p ("* If moving, its orbit"),
        p ("Please answer carefully. All data collected in this experiment will be anonymized."),
        p ("Risks and Benefits: You will hear the recorded sound. There are no known risks associated with this procedure."),
        p ("Time Engagement: Your participation takes less than 30 minutes. There are 5 parts.
"),
        p ("* Short questionnaire about yourself (about 1 minute)"),
        p ("* Practice session (about 10 minutes)"),
        p ("* Main experiment (about 15 minutes)"),
        p ("Participant's Rights: If you decide to participate in this experiment, please understand that participation is voluntary and you have the right to withdraw or withdraw your consent at any time without penalty. Personal privacy is maintained with all public and written data obtained from this study. "),
        p ("If you press the [Next] button, you agree to participate in this experiment as a judge. If you are not satisfied with this, do not press the [Next] button. If you press the] button, you can also confirm that the following conditions are met. "),
        p ("* You are over 18 years old."),
        p ("You understand and agree to this text.")
        
        
    ),
    
    
    admin_ui = NULL,
    button_text = "Next",
    on_complete = NULL
  ),
  
  
  
  
  
  text_input_page(
    label = "Name",
    prompt = "Please enter your name.",
    validate = function(answer, ...) {
      if (answer == "")
        "Please enter your name."
      else TRUE
    },
    on_complete = function(answer, state, ...) {
      set_global(key = "Name", value = answer,
                 state = state)
    }),
  
  text_input_page(
    label = "id",
    prompt = "Please enter the student id.",
    validate = function(answer, ...) {
      if (answer == "")
        "Please enter the student id."
      else TRUE
    },
    on_complete = function(answer, state, ...) {
      set_global(key = "id", value = answer,
                 state = state)
    }),
  
  
  text_input_page(
    label = "Hearing",
    prompt = "Do you have any hearing problem?",
    validate = function(answer, ...) {
      if (answer == "")
        "Please enter if you have any hearing problem. Example: yes or no."
      else TRUE
    },
    on_complete = function(answer, state, ...) {
      set_global(key = "hearing_problem", value = answer,
                 state = state)
    }),  
  # NAFC_page(
  #   label = "Hearing",
  #   prompt = "Do you have any hearing problem?",
  #   choices = c("No", "Yes"),
  #   
  #   on_complete = function(answer, state, ...) {
  #     set_global(key = "hearing_problem", value = answer,
  #                state = state)
  #   }),
  
  
  NAFC_page(
    label = "Sex",
    prompt = "What is your Sex?",
    choices = c("Male", "Female"),
    
    on_complete = function(answer, state, ...) {
      set_global(key = "sex", value = answer,
                 state = state)
    }),
  
  text_input_page(
    label = "Age",
    prompt = "How old are you?",
    validate = function(answer, ...) {
      if (answer == "")
        "Please enter your age."
      else TRUE
    },
    on_complete = function(answer, state, ...) {
      set_global(key = "age", value = answer,
                 state = state)
    })
)



practicebefore <- join(
  one_button_page(
    "Practice",
    admin_ui = NULL,
    button_text = "Next",
    on_complete = NULL
  )
)

#practice
practice_files=sort(list.files('./practice', pattern="wav$"));#,recursive=T);
practice_counter=1;
practice<-c();
for (j in seq(1,length(practice_files))) {
  practice <- c(practice,
                code_block(function(state,...) {
                  i<-practice_files[practice_counter];
                  current_practice<-unlist(strsplit(i,'.wav'));
                  practice_dir<-i;
                  shiny::showNotification(paste("Now testing",paste('practice/',current_practice,sep=''),"in Unity",toString(practice_counter),"/",toString(length(practice_files))));
                  ##Sockets
                  write.socket(server,paste('practice/',current_practice,sep=''));
                  print(practice_dir);
                  bytes_size = read.socket(server,loop=T);
                  ans<-read.socket(server,maxlen=as.integer(bytes_size),loop=T);
                  print(ans);
                  # close.socket(sender)
                  # close.socket(receiver)
                  practice_counter<<-practice_counter+1;
                  set_global(key = current_practice, value = ans, state = state)
                  results(state)<-results(state)[[current_practice]]<-ans;
                })
  );
}




experimentbefore <- join(
  one_button_page(
    "Main block",
    admin_ui = NULL,
    button_text = "Next",
    on_complete = NULL
  )
)




#experiment
#get audios
audio_files=list.files('./audio', pattern="wav$");#,recursive=T);
audio_files=sample(audio_files);
counter=1;
experiment<-c();
for (j in seq(1,length(audio_files))) {
  experiment <- c(experiment,
                  code_block(function(state,...) {
                    i<-audio_files[counter];
                    current_audio<-unlist(strsplit(i,'.wav'));
                    audio_dir<-i;
                    shiny::showNotification(paste("Now testing",paste('audio/',current_audio,sep=''),"in Unity",toString(counter),"/",toString(length(audio_files))));
                    ##Sockets
                    write.socket(server,paste('audio/',current_audio,sep=''));
                    print(audio_dir);
                    bytes_size = read.socket(server,loop=T);
                    ans<-read.socket(server,maxlen=as.integer(bytes_size),loop=T);
                    print(ans);
                    # close.socket(sender)
                    # close.socket(receiver)
                    counter<<-counter+1;
                    set_global(key = current_audio, value = ans, state = state)
                    results(state)<-results(state)[[current_audio]]<-ans;
                  })
  );
}

# write.socket(server,"{/wait/}");
last <- join(
  code_block(function(state,...) { close.socket(server);}),
  elt_save_results_to_disk(complete = TRUE),
  reactive_page(function(state, ...) {
    final_page(paste0("Thank you for your hard work. Thank you for your cooperation."))
    
  })
)

#demo <- randomise_at_run_time(label = "demo",logic = demo)
#experiment <- randomise_at_run_time(label = "experiment",logic = experiment)

timeline <- join(questions, practicebefore,practice, experimentbefore, experiment, last)


make_test(elts = timeline, 
          opt = demo_options(
            title = "Experimental form",
            admin_password = "arevalo",
            researcher_email = "jarevalo29@javerianacali.edu.co",
            demo = TRUE
          )
)

