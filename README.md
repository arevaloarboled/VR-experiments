# VR-experiments

This system was developed to perform a subjective evaluation of audio stimulus in VR. To keep listeners focused on the audio stimulus, all visual component is removed and their head orientation is tracked to ensure their right posture of them while listening to the stimulus. After the listeners were exposed to the stimulus, the system allows them to accurately record the auditory path of audio sources perceived respective to the listener's head. 

This system is composed of 2 elements: R and Unity. While R through **psychTestR** collects experiment data, the subject interacts in Unity in a VR scene. In this way, all experiment setup is done by R, which later communicates and controls the environment in Unity as shown in the sequence diagram at the end of this readme. Subject input in the VR scene is going to be performed only by the **right** controller (please check **Input mapping** section). The left controller can be used by the experimenter to guide the subject while wearing the VR goggles.

The experiment procedure is composed of 2 blocks: Practice and main block. The practice block is going to use in alphabetic order the stimuli in the `practice` folder. The idea of this block is to allow the subjects to familiarize themselves with the system and receive feedback from the experimenter. Once this block is over, the main block begins. This block is going to use stimuli from the `audio` folder randomly sorted. Feedback from the experimenter is not expected to be provided during this block.

## Requirements
### Hardware
This system is developed for [HTC Vive Virtual Reality System](https://www.amazon.com/HTC-VIVE-Virtual-Reality-System-pc/dp/B00VF5NT4I). Headphones connected directly to the computer (no through the HTC VIVE headset) is strongly suggested.
### Software
- [ Unity 2020.1.9f1 ](https://download.unity3d.com/download_unity/145f5172610f/Windows64EditorInstaller/UnitySetup64-2020.1.9f1.exe)
- [R](https://cran.r-project.org/bin/)
### R libraries
```r
install.packages("devtools")
devtools::install_github("pmcharrison/psychTestR")
install.packages("htmltools")
install.packages("shiny")
install.packages("plumber")
install.packages("jsonlite")
```

## Getting start
1. Download the repository and import it in Unity Hub. 
2. Modify the [introduction](https://github.com/arevaloarboled/VR-experiments/blob/45199805c9180d68d291366c952c371b6b2dbd98/Assets/Resources/app.R#L20-L37) in `/Assets/Resources/app.R` to fit with your experiment. 
3. Prepare your practice and experiment stimuli in the `/Assets/Resources/practice/` and `/Assets/Resources/audio/` respectively.
## How to run the experiment
1. Run **first** the `app.R` script, (be sure you are running the script in the `/Assets/Resources` folder, otherwise experimental data cannot be loaded).
2. Run the `PerformTest` scene in Unity.
3. Launch browser to [localhost:3838](localhost:3838). 
4. Input subject information (name, age, hearing problems, etc.)

After this, the practice part begins. Ask the subject to follow the instructions on the screen. Feedback from the experimenter can be provided by using the left controller. After practice is over, proceed to the main block. **Please do not provide any feedback from here.**

When all experiments are over, run `/Assets/Resources/Collect.R` to collect all the results in one file.

## Input mapping
![Alt text](mapping.png?raw=true "Input mapping")
- **Record:** To record the path, hold the ***trigger*** button. Once it is released, a confirmation screen is going to be displayed.
- **Confirmation buttons:** To confirm the path drawn, select yes or no in the ***track pad*** of the controller by clicking the top or bottom of it. To avoid a mistake, confirmation should be provided. In other words, to submit or repeat the recording please press the selection 2 times.
- **Repeat:** Press this button to repeat a stimulus.
- **Change coordinate system:** System illustrates control position with spherical coordinates by default. To display them in the euclidean system and vice-versa, press the ***Grip*** button.

## Sequence diagram of the experiment

![Alt text](sequence.png?raw=true "Input mapping")

## Authors
- Mayu Tsukamoto
- Camilo Arevalo
- Julián Villegas
