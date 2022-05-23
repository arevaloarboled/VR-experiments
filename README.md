# VR-experiments

This system was developed to perform a subjective evaluation of audio stimuli in VR. All visual components were removed to keep listeners focused on the audio stimulus. The orientation of the listener’s head is tracked to ensure the correct posture while listening to the stimuli. The system allows the listeners to record the auditory path corresponding to the stimulus relative to the listener’s head after exposition.

This system is comprised of two components: `psychTestR` written in R, the other in Unity. While `psychTestR` collects experimental data, the subject interacts in Unity with a VR scene. All the experimental design is done in R which controls the environment in Unity, as shown in the sequence diagram at the bottom of this readme.

Subjective input in the VR scene is performed only by the right controller (refer to the Input mapping section). The left controller can be used by the experimenter to guide the subject while wearing the VR goggles.

The experiment procedure is comprised of two blocks. The practice block uses the stimuli in the practice folder sorred by alphabetic order. This block allows the subjects to familiarize with the apparatus and receive feedback from the experimenter. The main block follows the practice one. In this case,  the stimuli from the audio folder is presented in a random permutation fashion. Feedback from the experimenter is not expected to be provided during this block.

## Requirements
## OS
Windows
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
- **Confirmation buttons:** To confirm the path drawn, select yes or no in the ***track pad*** of the controller by clicking the top or bottom of it. To avoid a mistake, confirmation should be provided. In other words, to submit or repeat the recording please press the selection twice.
- **Repeat:** Press this button to repeat a stimulus.
- **Change coordinate system:** System illustrates control position with spherical coordinates by default. To display them in the euclidean system and vice-versa, press the ***Grip*** button.

## Sequence diagram of the experiment

![Alt text](sequence.png?raw=true "Input mapping")

## Authors
- Mayu Tsukamoto
- Camilo Arevalo
- Julián Villegas
