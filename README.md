# Shiftly

Shiftly is a novel shape-shifting haptic device that renders plausible haptic feedback when touching virtual objects in VR. By changing its shape, different geometries of virtual objects can be approximated to provide haptic feedback for the user’s hand. 

This repo contains the CAD files to print the device, the microcontroller for the device, and the VR application used to evaluate Shiftly.



If you use any information or material from this repo, please cite this publication: 


*T. Batik, H. Brument, K. Vasylevska and H. Kaufmann, "Shiftly: A Novel Origami Shape-Shifting Haptic Device for Virtual Reality," in IEEE Transactions on Visualization and Computer Graphics, doi: 10.1109/TVCG.2025.3549548.*




More detailed information about the fabrication of the prototype and the integration of the different parts can be found in my thesis: 

*Batik, T. (2023). Design and evaluation of a novel shape changing haptic device for virtual reality [Diploma Thesis, Technische Universität Wien]. reposiTUm. https://doi.org/10.34726/hss.2023.111843*



## Content

The content of this repo is aimed at making the results of *Shiftly: A Novel Origami Shape-Shifting Haptic Device for Virtual Reality* reproducible. 

* `Device` contains the stl files that are used to print Shiftly and the origami pattern. 
* `Kinematic Model` contains a sample implementation of the Kinematic model outlined in [1] and the evaluation results.
* `Microcontroller-control-program` contains the microcontroller code that runs on the device.
* `VR-Apps` The Unity source code for the VR applications to evaluate and show the capabilities of Shiftly.



#### Device

CAD and 3D printed files for Shiftly. We printed the parts in standard PLA on an FDM printer—especially the mechanical parts requiring extensive post-processing. Generally, CAD files are designed with relatively small tolerances and need, in our case, the removal of a lot of material after printing. The fusin360 files are also included; feel free to adapt the files to suit your printer. Further details about assembling are outlined in [[2]](https://doi.org/10.34726/hss.2023.111843).



#### Kinematic-model

The ` Kinematic-model/evaluation ` folder contains the images of the results of the evaluation of the kinematic model in [1]. `Kinematic-model/sample-code` contains a sample Python implementation of the kinematic model outlined in [1]. 



#### Microcontroller Control Program

This program is used on Shiftly's microcontroller to handle the communication between the VR application and the microcontroller and control the motor rotation.

The Program was tested and developed for an ESP32-DevKitC V4 board using MicroPython 1.21.0. The required hardware elements, the program's functionality, and a corresponding circuit diagram can be found in [[2]](https://doi.org/10.34726/hss.2023.111843). The communication between the microcontroller and the PC VR application is implemented via USB and Wifi — booth APIs over the same functionality.



#### VR-Apps

The VR applications that are used to test and evaluate Shiftly. Implemented in Unity 2021.3. For a HTC VIVE, Leap Motion Controller, and a VIVE tracker 2.0. More details in [[2]](https://doi.org/10.34726/hss.2023.111843).



----

[1] Shiftly: A Novel Origami Shape-Shifting Haptic Device for Virtual Reality
Tobias Batik, Hugo Brument, Khrystyna Vasylevska, Hannes Kaufmann, Currently in Review, 2025

[2] Batik, T. (2023). Design and evaluation of a novel shape changing haptic device for virtual reality [Diploma Thesis, Technische Universität Wien]. reposiTUm. https://doi.org/10.34726/hss.2023.111843

