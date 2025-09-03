# Guide to Programming the YODA

## Running the machine
The machine can optionally be started with the following command line arguments:

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

* The `add` command is defined by its opcodes and is then followed by three operands.  The two numbers to add and the location for the result.

## Internal State
 
Internally the machine tracks for three items of state.  These are not directly accessible to programmer.

| Name                | Use                                    | Default Value |
|---------------------|----------------------------------------|---------------|
| Instruction Pointer | Address of the next command to execute | 0x00          |
| Stack Pointer       | Address of the head of the stack       | 0xFC          |
| Interrupts Enabled  | Controls if the interrupts will fire   | False         |

## Instruction Pointer

The `instruction pointer (IP)` contains the index of the memory location of the next command to execute.  When the program starts this set to 0x00. After the command is executed,  the instruction pointer is incremented by the length of the command (e.g. +1 for `halt`,  +4 for `add`).  

The exception to this are the `JUMP` commands which allow the instruction pointer to be changed to a specified address


## Files

Input and Output to the machine can be done by read/writing the memory to a file.  16 files (0-15) are available.  
All the files are identical but files 8-15 are given a `.txt` file extension.



## Operand Types

There are three ways of providing the values for the operands.  `Immediate` Addressing, `Direct` Memory Addressing and `Indirect` Memory Addressing

This is best explained by looking at the `ADD` command as an example.  The add command has 3 operands,  which are the two values to add and the location to store the result.

The two values to add can either be supplied as the actual values,  for example `ADD 7 10` would give 17.   This is called Immediate Addressing.

Alternately,  the memory addresses in which to find the values can be supplied.  For example ADD [7] [10] would add together the values held in memory locations 7 and 10.  This is called Direct Memory Addressing.

For the 3rd operand,  the result,  Immediate Addressing doesn't make sense.  i.e.  You can't store the result in `23` but you can store the result in memory location 23 - i.e. direct memory addressing.   The final more complicated case is Indirect Memory Addresses,  this refers to the memory location held in memory location 23.

For example

Memory-34 == 3
Memory-56 == 10

`ADD 12 [34] [[56]]`  says add 12 to the value at location 34,  which in this case is 3 giving the total of 15. This is then written to the memory location
pointed two by memory location 56,  in this case location 10.

Memory-10 == 15

By convention,  plain numbers 123 refer to the immediate values.  Numbers in square brackets [123]  are direct memory locations,  and in double brackets [[123]] are indirect memory locations.


How do we tell the machine which addressing mode we wish to use for each operand?  For the YODA machine,  each command actually has multiple opcodes.  For example the ADD command has 8 different opcodes,  for the 8 possible combinations of addressing modes.

For example 0x47 means `ADD LHS RHS [Result]` and 0x42 means `ADD [LHS] RHS [[Result]]`



## Interrupts

When the outside world needs to inform the machine about an event, then an interrupt is triggered.  An interrupt vector (IV) table holds the address of the code to run when the interrupt is triggered.

| Address | Name                |
|---------|---------------------|
| 0xFE    | Right Arrow pressed |
| 0xFF    | Left Arrow pressed  |

i.e. Address 0xFE needs to contain the address of the code to run when the right arrow is pressed.  This code is know as the interrupt service routine (ISR)

When your code has finished processing the interrupt it should call `RET` to return back to the instruction it was processing before the interrupt was triggered.

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


# Commands

## Halt - OpCode 0x00
The `halt` command causes the machine to exit.

## Wait - OpCode 0x01
The `wait` command causes the CPU to sleep for 1/10 of a second.

## Ret - OpCode 0x02
The `ret` command sets the instruction pointer to the address popped off the top of the stack.
If the stack is empty then an underflow error is reported.
This is commonly called at the end of an Interrupt Service Routine (ISR) or after previously calling `JumpWithReturn`

## Nop - OpCode 0x03
Does nothing other than increase the instruction pointer.

## Sif - OpCode 0x04
Sets the interrupt flag.  This allows interrupts to call the ISRs.

## Cif - OpCode 0x05
Clears the interrupt flag.  This prevents interrupts from calling the ISRs.

## SaveToFile - OpCodes 0x10 -> 0x17
This is a 3 operand command which copies the specified values from memory to a file.
The operands are

| Name       | Description                            | Address Modes       |
|------------|----------------------------------------|---------------------|
| FileNumber | Number of the file to write 0-15       | Immediate or Direct |
| Location   | Address of the first location to write | Direct or Indirect  |
| Length     | Number of bytes to write               | Immediate or Direct |


| OpCode | File Number | Start Location | Length    |
|--------|-------------|----------------|-----------|
| 0x10   | Direct      | InDirect       | Direct    |
| 0x11   | Direct      | InDirect       | Immediate |
| 0x12   | Direct      | Direct         | Direct    |
| 0x13   | Direct      | Direct         | Immediate |
| 0x14   | Immediate   | InDirect       | Direct    |
| 0x15   | Immediate   | InDirect       | Immediate |
| 0x16   | Immediate   | Direct         | Direct    |
| 0x17   | Immediate   | Direct         | Immediate |

All the files are identical but files 8-15 are given a `.txt` file extension.



## LoadFromFile - OpCodes 0x20 -> 0x23
This is a 2 operand command which replaces the specified memory with the contents of a file

| Name       | Description                            | Address Modes       |
|------------|----------------------------------------|---------------------|
| FileNumber | Number of the file to read 0-15        | Immediate or Direct |
| Location   | Address of the first location to write | Direct or Indirect  |


| OpCode | File Number | Start Location |
|--------|-------------|----------------|
| 0x20   | Direct      | InDirect       |
| 0x21   | Direct      | Direct.        |
| 0x22   | Immediate   | InDirect.      |
| 0x23   | Immediate   | Direct         |

All the files are identical but files 8-15 are expected to have a `.txt` file extension.




## Write - OpCodes 0x30 -> 0x33
This is a 2 operand command which a value to a single memory location

| Name     | Description                    | Address Modes       |
|----------|--------------------------------|---------------------|
| Value    | The value to write             | Immediate or Direct |
| Location | The memory address to write to | Direct or Indirect  |


| OpCode | File Number | Start Location |
|--------|-------------|----------------|
| 0x30   | Direct      | InDirect       |
| 0x31   | Direct      | Direct.        |
| 0x32   | Immediate   | InDirect.      |
| 0x33   | Immediate   | Direct         |




## Add - OpCodes 0x40 -> 0x47
This is a 3 operand command which adds 2 values together.
The operands are

| Name   | Description             | Address Modes       |
|--------|-------------------------|---------------------|
| LHS    | First number            | Immediate or Direct |
| RHS    | Second number           | Immediate or Direct |
| Result | Location for the result | Direct or Indirect  |


| OpCode | LHS       | RHS       | Result   |
|--------|-----------|-----------|----------|
| 0x40   | Direct    | Direct    | Indirect |
| 0x41   | Direct    | Direct    | Direct   |
| 0x42   | Direct    | Immediate | Indirect |
| 0x43   | Direct    | Immediate | Direct   |
| 0x44   | Immediate | Direct    | Indirect |
| 0x45   | Immediate | Direct    | Direct   |
| 0x46   | Immediate | Immediate | Indirect |
| 0x47   | Immediate | Immediate | Direct   |


## Sub - OpCodes 0x50 -> 0x57
This is a 3 operand command which subtracts the RHS from the LHS
The operands are

| Name   | Description             | Address Modes       |
|--------|-------------------------|---------------------|
| LHS    | First number            | Immediate or Direct |
| RHS    | Second number           | Immediate or Direct |
| Result | Location for the result | Direct or Indirect  |


| OpCode | LHS       | RHS       | Result   |
|--------|-----------|-----------|----------|
| 0x50   | Direct    | Direct    | Indirect |
| 0x51   | Direct    | Direct    | Direct   |
| 0x52   | Direct    | Immediate | Indirect |
| 0x53   | Direct    | Immediate | Direct   |
| 0x54   | Immediate | Direct    | Indirect |
| 0x55   | Immediate | Direct    | Direct   |
| 0x56   | Immediate | Immediate | Indirect |
| 0x57   | Immediate | Immediate | Direct   |


## Inc - OpCodes 0x60 -> 0x61
This is a single operand command which increases a value by 1

| Name     | Description                  | Address Modes      |
|----------|------------------------------|--------------------|
| Location | The memory address to update | Direct or Indirect |


| OpCode | Location |
|--------|----------|
| 0x60   | Indirect |
| 0x61   | Direct   |


## Dec - OpCodes 0x70 -> 0x71
This is a single operand command which decreases a value by 1

| Name     | Description                  | Address Modes      |
|----------|------------------------------|--------------------|
| Location | The memory address to update | Direct or Indirect |


| OpCode | Location |
|--------|----------|
| 0x70   | Indirect |
| 0x71   | Direct   |



## JumpIfZero - OpCodes 0x80 -> 0x83
This is a 2 operand command which switches the instruction pointer to a different location,  but only if the specified value is 0
If the value is not zero, then the instruction pointer moves to the next command as normal.
Note,  the return address is NOT pushed onto the stack,  so `RET` cannot be used.

| Name     | Description                   | Address Modes       |
|----------|-------------------------------|---------------------|
| Value    | The value to check            | Immediate or Direct |
| Location | The memory address to jump to | Immediate or Direct |


| OpCode | File Number | Location  |
|--------|-------------|-----------|
| 0x80   | Direct      | Direct    |
| 0x81   | Direct      | Direct    |
| 0x82   | Direct      | Immediate |
| 0x83   | Direct      | Immediate |


## JumpWithReturn - OpCodes 0x90 -> 0x91
This is a 1 operand command which always switches the instruction pointer to a different location, but first pushes the
next instruction pointer onto the stack.

| Name     | Description                    | Address Modes       |
|----------|--------------------------------|---------------------|
| Location | The memory address to write to | Immediate or Direct |


| OpCode | Location |
|--------|----------|
| 0x90   | Indirect |
| 0x91   | Direct   |

