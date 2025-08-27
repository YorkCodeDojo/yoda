# Guide to Programming the YODA

## Running the machine
The machine can optionally started with the following command line arguments:

* The first argument (if provided) is the folder to look for the files in.   If missing then the current folder will be used.
* The second argument is the `--debug` flag.  


## Memory

It has 256 memory locations (`0x0-0xFF`), each of which can hold a single byte.  This is laid out as below


| Address      | Use                    |
|--------------|------------------------|
| 0xFE -> 0xFF | Interrupt Vector Table |
| 0xF8 -> 0xFD | LCD Display            |
| 0x?? -> 0xF7 | Bottom of Stack        |
| 0x00 -> 0x?? | Program + Data         |

Due to the lack of memory space,  the machine comes with a couple of instructions (`WriteToFile` and `LoadFromFile`) which allows memory to be written to/from files.

## Booting

At startup,  memory is populated from a boot file.   
i.e. If the boot file contains the bytes 1 2 3,  then the first three memory locations will be populated with 1, 2 and 3.  The remaining 253 bytes will be initialised with zeros.

## Commands

Each command consists of an opcode and between 0 and 4 operands.  Opcodes and Operands are always 1 byte each.
e.g.
* The `halt` command (which causes the machine to stop) is defined by the opcode `0x00` and has no operands.

* The `add` command is defined by the opcodes and is then followed by three operands.  The two numbers to add and the location for the result.

## Internal State
 
Internally the machine tracks for three items of state.  These are not directly accessible to programmer.

| Name                | Use                                    | Default Value |
|---------------------|----------------------------------------|---------------|
| Instruction Pointer | Address of the next command to execute | 0x00          |
| Stack Pointer       | Address of the head of the stack       | 0xFC          |
| Interrupts Enabled  | Controls if the interrupts will fire   | True          |

## Instruction Pointer

The `instruction pointer (IP)` contains the index of the memory location of the next command to execute.  When the program starts this set to 0. After the command is executed,  the instruction pointer is incremented by the length of the command (e.g. +1 for `halt`,  +4 for `add`).  

The exception to this are the `JUMP` commands which allow the instruction pointer to be changed to a specified address


## Files

Input and Output to the machine can be done by read/writing the memory to a file.  16 files (0-15) are available.  
All the files are identical but files 8-15 are given a `.txt` file extension.



## Operand Types

There are two ways of providing the values for the operands.  Immediate Addressing and Memory Addressing.   

This is best explained by an example.



`0x40`-`0x47`
For the immediate version of ADD,  then `ADD 10 20 5` means take the numbers 10 and 20 add them together and place the total in memory location 5
For the memory version of ADD, then `ADD 10 20 5` means take the value in memory location 10 and add it to the value in memory location 20.  Then write the total to the memory location
specified by the value in memory location 5

Each command has several opcodes,  depending on which opcodes you wish to be IMMEDIATE or MEMORY addressing


## Interrupts

When the outside world needs to inform the machine about an event, then an interrupt is triggered.  An interrupt vector (IV) table holds the address of the code to run when the interrupt is triggered.

| Address | Name                |
|---------|---------------------|
| 0xFE    | Right Arrow pressed |
| 0xFF    | Left Arrow pressed  |

i.e. Address 0xFE needs to contain the address of the code to run when the right arrow is pressed.  This code is know as the interrupt service routine (ISR)

When your code has finished processing the interrupt it should called `RET` to return back to the instruction is was processing before the interrupt was triggered.

Other related commands are
* `WAIT` which causes the CPU to sleep for 1/10 of a second (100 ms)
* `CIF` which stands for Clear Interrupt Flag.  This switches off the processing of interrupts
* `SIF` which stands for Set Interrupt Flag.  This resumes the processing of interrupts


## Screen Memory
Bytes 0xF8 to 0xFD are mapped to the machines ASCII LCD display.


| Address | Description |
|---------|-------------|
| 0xF8    | 1st digit   |
| 0xF9    | 2nd Digit   |
| 0xFA    | 3rd Digit   |
| 0xFB    | 4th Digit   |
| 0xFC    | 5th Digit   |
| 0xFD    | Refresh     |

Setting Bit 0 of the refresh byte to 1 will cause the screen to draw.





TODO:
Document exercises
Document operand types
Document all commands
Support for stack
Support CIF SIF
Different languages