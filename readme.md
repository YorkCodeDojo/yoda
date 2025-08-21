## Introduction

The City of York has decided to enter the space program.  However, due to very tight time constraints they have decided to outsource the programming of their flight computer to your company.

The computer is a custom-built machine, which runs York’s Obscenely Dumb Architecture.  (YODA)

Your task will be to complete a set of tasks to ensure the spaceship reaches the moon and doesn't come crashing down in the Ouse.

One thing....  the fancy AI tools like cursor won't work in space,  so you will need to bring your favourite hex editor.


## The machine

For such an important task,  the machine is actually pretty basic.  

* It has 256 memory locations (0x0-0xFF), each of which can hold a single byte.

* At startup,  memory is populated from a boot file.   
i.e. If the boot file contains the bytes 1 2 3,  then the first three memory locations will be populated with 1, 2 and 3.  The remaining 253 bytes
will be initialised with zeros.

* Each command consists of an opcode and between 0 and 4 operands.  Opcodes and Operands are always 1 byte.
e.g.
The halt command (which causes the machine to stop) is defined by the opcode 0x0 and has no operands
The add command is defined by the opcode 0x???? and is then followed by three operands.  The two numbers to add and the location for the result.

* An instruction pointer (IP) contains the index of the memory location of the next command to execute.  When the program starts this set to 0
After the command is executed,  the instruction pointer is incremented by the length of the command (e.g. +1 for halt,  +4 for add).  

The exception to this is JUMP commands which allow the instruction pointer to be changed to a specified address







Example File
For example,  if the first 5 bytes of boot file contained

?? 2 3 5 0

Then the numbers 2 and 3 would be added together placed in memory location 0x5






## Exercise 1
Before the spaceship can be launched a couple of checks need to be carried out.  First we need to check that the machine is
capable of running a program.

The simplest program you can write is one which does nothing and stops straight away.  This can be done by 
having your program just contain the halt command.  

Populate the boot file with a single byte with the value 0 (0 is the op code for halt)

Run the virtual machine and check it runs without an error.


## Exercise 2
We also need to check that the machine is correctly reporting errors.  The easiest way to crash the machine is to
get it an op code it doesn't understand.   For example 255

Run the virtual machine and check it gives you an error.  It should also create a couple of crash_dump files to aid you with your troubleshooting.


## Exercise 3
We can now launch the spacecraft.  To do this,  we need to write 5 bytes to file 0 containing the values 5 4 3 2 1

Your program should consist of the 
  WriteToFile command with it's 3 operands  (ie 4 bytes)
  Halt command (1 byte)
  5 bytes of memory containing your 5 values

Run the virtual machine, and check that File0 is correctly populated with your 5 values



## Exercise 4
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




