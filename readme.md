## Introduction

The City of York has decided to enter the space program.  However, due to very tight time constraints they have decided to outsource the programming of their flight computer to your company.

The computer is a custom-built machine, which runs York’s Obscenely Dumb Architecture.  (YODA)

Your task will be to complete a set of tasks to ensure the spaceship reaches the moon and doesn't come crashing down in the Ouse.

One thing....  the fancy AI tools like cursor won't work in space,  so you will need to bring your favourite hex editor.


## The machine

For such an important task,  the machine is actually pretty basic.  




Example File
For example,  if the first 5 bytes of boot file contained

IP
?? 02 07 05 00 

Then the instruction pointers starts at 0x00 which contains the ADD command.  The numbers 2 and 7 are added together
and placed into memory location 5.  The instruction pointer then advances 4 bytes to location 0x04

            IP 
?? 02 07 05 00 09

Location 0x04 contains a 0 which is the halt command so then the program exits.




## Exercise 1
Before the spaceship can be launched a couple of checks need to be carried out.  First we need to check that the machine is
capable of running a program.

The simplest program you can write is one which does nothing and stops straight away.  This can be done by 
having your program just contain the halt command.  

Populate the boot file with a single byte with the value 0x00 (0 is the op code for halt)

Run the virtual machine and check it runs without an error.


## Exercise 2
We also need to check that the machine is correctly reporting errors.  The easiest way to crash the machine is to
get it an opcode it doesn't understand.   For example 255 (0xFF)

Run the virtual machine and check it gives you an error.  It should also create a couple of crash_dump files to aid you with your troubleshooting.


## Exercise 3
We can now launch the spacecraft.  To do this,  we need to write 5 bytes to file 0 containing the values 5 4 3 2 1

Your program should consist of the 
  WriteToFile command with it's 3 operands  (ie 4 bytes)
  Halt command (1 byte)
  5 bytes of memory containing your 5 values

Run the virtual machine, and check that File0 is correctly populated with your 5 values


## Exercise 4
Oh!! the spacecraft didn't launch.  After a long root cause analysis meeting it was determined that you created the wrong type of file
and also the message you were told wasn't correct.

Rather than creating a binary file with the countdown sequence you were meant to create a human-readable text file with the text
"5 4 3 2 1 LIFTOFF"

Use file 9 instead of file 0.  Once you have created your file open it in a text editor to check it displays corrected.

Hint...  ASCII




## Exercise 5
We need to keep track of how much fuel the craft is using.   Starting with a value of 123 in File1,  write a program that
each time it runs,  decreases the value in File1 by 1

In addition to your previous commands,  you will also need to use the `LoadFromFile` and `Dec` commands.


--- IMPORTANT MESSAGES TO ALL TEAMS FROM THE FLIGHT CENTRE ---

To all engineering teams,

We are concerned about the time it is taking to write these applications,  and the number of reported bugs.  We tasked a top-rated external
consultancy to investigate about how things could be streamlined.  They have done a great job and have produced a 2000 page report.  The summary
of this report is that maybe programming directly in bytes is the issue.

One of their main recommendations is that

Engineering teams should not manually create their boot files.  Instead, they should write applications to create these files.  These applications can then make 
use of constants and helper functions to make the programs more readable.

For example instead of writing the value 0 to a file,  the program should use the constant HALT


regards

Flight Centre,

--- END OF MESSAGE --



## Exercise 6

Add 2 numbers


## Exercise 7

Subtract 2 numbers



## Exercise 1
To start rocket running, write a program (in file 0) which writes the value 123 into file 1. 



## Exercise 2
Write a program to add 1 to a number.

Steps:
* Manually populate file 1 with the number you wish to increase.
* Write a program (in file 0) which
  * Loads in the contents of file 1
  * Increments the value
  * Writes the contents back to file 1


## Exercise 2
Write a program to add 2 (1 byte) numbers together

Steps:
* Manually populate file 1 with the two numbers you wish to add.
* Write a program (in file 0) which
    * Loads in the contents of file 1
    * Adds the values
    * Writes the contents back to file 1




Output module
setting bit 0 of byte X to a 1 will write the contents of the last 7 bytes to the screen

Input module
Interrupt can be triggered
SETIQ locationToJumpTo
RETIQ return from the interrupt



TODO:
Document exercises
Different languages