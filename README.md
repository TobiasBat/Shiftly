# Shiftly









## Microcontroller Control Program

The Programm was tested and developed for an ESP32-DevKitC V4 board using MicroPython 1.21.0. 

The program supports the following commands: 

| Command                                                      |
| ------------------------------------------------------------ |
| *Command Line:* <br />`stepper=<int> degree=<int> microsteps=<int>`<br />*REST*: <br />`PUT` ` ?stepper=<int>&degree=<int>&microsteps=<int>` <br><br>Rotates the stepper motor to the corresponding degree. After the rotation is completed,the motor is rotated to the given number of degrees. microsteps manipulate the speedof the rotation. |
| *Command Line:* <br>`set stepper=<int> degree=<int>`<br>*REST:*<br>`PUT /set?stepper=<int>\&degree=<int>`<br><br>The `set` command is used to set the internal step counter. When executed, the internal step counter is set to the provided value without rotating the motor. This command is used to adjust the counter after the system loses track of the rotation of the motor. |
| *Command Line:* <br/>`step stepper=<int> degree=<int> microsteps=<int>`<br><br>*REST:*<br>`PUT /step?stepper=<int>\&degree=<int>\&microsteps=<int>`<br><br>The `step` command rotates the motor by a certain number of degrees. In contrast to the above command, the large gear is rotated by the given degrees, but the step counter is unchanged. This command is used for the initialization of the physical device. |

