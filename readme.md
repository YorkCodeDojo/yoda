## Introduction

The City of York has decided to enter the space program.  However, due to very tight time constraints they have decided to outsource the programming of their flight computer to your company.

The computer is a custom-built machine, which runs York’s Obscenely Dumb Architecture.  (YODA)

Your task will be to complete a set of tasks to ensure the spaceship reaches the moon and doesn't come crashing down in the Ouse.

One thing....  the fancy AI tools like cursor won't work in space,  so you will need to bring your favourite hex editor.


## The machine

For such an important task,  the machine is actually pretty basic.  

* It has 256 memory locations (0x0-0xFF), each of which can hold a single byte.  Due to the lack of memory space,  the machine comes with a couple of
instructions (WriteToFile and LoadFromFile) which allows memory to be written to/from files.

* At startup,  memory is populated from a boot file.   
i.e. If the boot file contains the bytes 1 2 3,  then the first three memory locations will be populated with 1, 2 and 3.  The remaining 253 bytes
will be initialised with zeros.

* Each command consists of an opcode and between 0 and 4 operands.  Opcodes and Operands are always 1 byte each.
e.g.
The halt command (which causes the machine to stop) is defined by the opcode 0x0 and has no operands
The add command is defined by the opcode 0x???? and is then followed by three operands.  The two numbers to add and the location for the result.

* An instruction pointer (IP) contains the index of the memory location of the next command to execute.  When the program starts this set to 0
After the command is executed,  the instruction pointer is incremented by the length of the command (e.g. +1 for halt,  +4 for add).  

The exception to this is JUMP commands which allow the instruction pointer to be changed to a specified address

* Input and Output to the machine is done by read/writing the memory to a file.  16 files (0-15) are available.  All the files are identical
but files 8-15 are given a .txt file extension.

* There are two ways of providing the values for the operands.  Immediate Addressing and Memory Addressing.   This is best explained by an example.

For the immediate version of ADD,  then `ADD 10 20 5` means take the numbers 10 and 20 add them together and place the total in memory location 5
For the memory version of ADD, then `ADD 10 20 5` means take the value in memory location 10 and add it to the value in memory location 20.  Then write the total to the memory location
specified by the value in memory location 5

Each command has several opcodes,  depending on which opcodes you wish to be IMMEDIATE or MEMORY addressing

* When the outside world needs to inform the machine about an event, then an interrupt is triggered.  An interrupt vector (IV) table holds the
address of the code to run when the interrupt is triggered.

| Address | Name               |
|---------|--------------------|
| 0xFE    | Space bar pressed  |
| 0xFF    | Return key pressed |

i.e. Address 0xFE needs to contain the address of the code to run when the space bar is pressed.  This code is know as the interrupt service routine (ISR)

When your code has finished processing the interrupt it should called RET to return back to the instruction is was processing 
before the interrupt was triggered.

Other related commands are
* SLEEP which causes the CPU to sleep for 1/10 of a second (100 ms)
* WAIT which causes the CPU to sleep until an interrupt is triggered.
* CIF which stands for Clear Interrupt Flag.  This switches off the processing of interrupts
* SIF which stands for Set Interrupt Flag.  This resumes the processing of interrupts


* Screen Memory
Bytes 0xF8 to 0xFD are mapped to the machines ASCII LCD display.


| Address | Description |
|---------|-------------|
| 0xF8    | 1st digit   |
| 0xF9    | 2nd Digit   |
| 0xFA    | 3rd Digit   |
| 0xFB    | 4th Digit   |
| 0xFC    | 5th Digit   |
| 0xFD    | Refresh     |

Updating Bit 0 of the refresh byte will cause the screen to draw.




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
