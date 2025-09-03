--- 10TH SEPT 1951 :: IMPORTANT MESSAGE TO ALL TEAMS FROM THE FLIGHT CENTRE ---

To all engineering teams,

As you may have seen from our press release,  The City of York has decided to enter the space race,  and due to time constraints they have decided to out-source the programming of their flight systems to our company.

They have developed a computer especially for this task,  named the YODA and have provided us with the attached manual.md file.

--- END OF MESSAGE --

---

## Exercise 1
Before the spaceship can be launched a couple of checks need to be carried out.  First we need to check that the machine is capable of running a program.

The simplest program you can write is one which does nothing and stops straight away.  This can be done by having your program just contain the halt command.  

Populate the boot file with a single byte with the value 0x00 (0 is the op code for halt)

Run the machine and check it completes without an error.


## Exercise 2
We also need to check that the machine is correctly reporting errors.  The easiest way to crash the machine is to give it an opcode it doesn't understand.   For example 255 (0xFF)

Run the machine and check it gives you an error.  It should also create a couple of `crash_dump` files to aid you with your troubleshooting.


## Exercise 3
We can now launch the spacecraft.  To do this,  we need to write 5 bytes to file 0 containing the values 5 4 3 2 1

Your program should consist of 
1.  `WriteToFile` command with it's 3 operands  (ie 4 bytes)
2.  `Halt command` (1 byte)
3.  5 bytes of memory containing your 5 values

Run the machine, and check that `File0` is correctly populated with your 5 values


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
consultancy to investigate about how things could be streamlined.  They have done a great job and have produced a 2000-page report.  The summary
of this report is that maybe programming directly in bytes is the issue.

One of their main recommendations is that

Engineering teams should not manually create their boot files.  Instead, they should write applications to create these files.  These applications can then make 
use of constants and helper functions to make the programs more readable.

For example instead of writing the value 0 to a file,  the program should use the constant HALT


regards

Flight Centre,

--- END OF MESSAGE --


## Exercise 6

Sorry to keep missing you around,  but we have changed the requirements again.  To launch the spacecraft,  5 4 3 2 1 should be written to the LCD output.
The digits should be displayed one at a time,  with a 100ms delay between them.



## Exercise 7

The control system can now be written.  The current position of the craft can be shown on the LCD by a single `-`

For example

```
[ ][ ][-][ ][ ]
```

The astronauts should be able to position the craft with the use of the left/right arrow keys.

For example after pressing the right arrow twice

```
[ ][ ][ ][ ][-]
```


Hint: Read section on interrupts


## Exercise 8

One of the other development teams is having trouble getting the `SUB` opcodes to work.  Can you provide them with a workaround?



