# Manual solutions by Mark and Henry

## Step 1 - prepare the ground

All projects benefit from a little preparation and good groundwork... Reading the readme and the manual (I know, shock, horror actually reading!!), it states that the boot files we need to be executed should all be 256 bytes long. Therefore, as a first step, creating a template boot file would benefit later exercises, where the template could be copied and edited _manually_, i.e. there's no actual coding per se, just old fashioned mangling of the boot files, a la flicking of switches to set the program states as happened on _really_ old systems!

### Step 1.1 - template boot file
To create the boot file, as we were using a combination of dotnet on MXLinux and node on OSX, we both had a supply of lovely unix-style commands that can be utilised to generate the file. One simple way to generate the file is to create a small file with an initial number of zero-byte entries, then concatenate these files in order to create the ultimate goal of a file (called `boot-allzero`) that contains 256 zero bytes. The command `printf` was used initially to create a small 4-byte long file, then this was used to essentially double-up the length of the file (through intermediate files) until the magic file was generated.
Starting in our "solution folder" called `Mark-Henry-Manual-Method`, a shell was opened and the following commands executed sequentially:

```shell
printf "\x00\x00\x00\x00" > boot004
cat boot004 > boot008 && cat boot004 >> boot008
cat boot008 > boot016 && cat boot008 >> boot016
cat boot016 > boot032 && cat boot016 >> boot032
cat boot032 > boot064 && cat boot032 >> boot064
cat boot064 > boot128 && cat boot064 >> boot128
cat boot128 > boot-allzero && cat boot128 >> boot-allzero
```
Checking the final file, we can use the following command to verify it is as expected:
```shell
hexdump boot-allzero 
0000000 0000 0000 0000 0000 0000 0000 0000 0000
*
0000100
```
This confirms the boot-file is indeed 256 bytes long and contains all zeros :)
### Step 1.2 - Verify YODA compiles and works as expected
Before starting the exercises proper, it would be nice to know that the machine can be run successfully. Opening a fresh terminal window, then navigating into the YODA folder, the following commands were executed to ensure the program compiles using the selected methods: dotnet and TypeScript.
#### Step 1.2.1 - dotnet checks
1. From the terminal, navigate into the YODA folder
2. At the terminal, use the command `dotnet build SimpleInstructionMachine.csproj` to build the project. If all works correctly, the  something like the following shall be output to the terminal window:
```terminaloutput
Restore complete (0.5s)
SimpleInstructionMachine succeeded (0.3s) → bin/Debug/net9.0/SimpleInstructionMachine.dll

Build succeeded in 1.2s
```
3. Next step is to get it to run, without specifying any boot file, which ought to inform us whether it is ready and if it succeeded in booting, which it shouldn't be able to as no boot fille has been supplied yet. Issuing the following command at the termninal `dotnet run` should result in something like the following output:
```terminaloutput
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: .

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

No boot file found.


Program completed successfully
```
4. Success! Well, kind of... It runs without nasty errors, but doesn't do anything

#### Step 1.2.2 - TypeScript checks
1. From the terminal, navigate into the YODA_TypeScript folder
2. First thing we need to do is make sure we have the toolchain needed to make this work. First up is node (or nvm, if you happen to operate in differing versions of node). To check if it is installed, try executing `node --version` from the terminal window. If successful, then node is installed; if not then you need to follow the required steps to install node for your own system as it's beyond the scope of what we're trying to do today!
3. Next, as we have the program in TypeScript we need to convert to JavaScript so node knows how to run it... From the terminal, try the command `tsc --version` - if a value is returned, then we're good to go. Otherwise, try using npm to install it globally using the following command: `npm install -g typescript`. If that completes successfully, then the typescript package should now be able to compile the TypeScript source to JavaScript for us.
4. From the manual, we now need to ensure that the remainder of the required files are present, so we need to run `npm install` and wait for it to fetch/install everything else we need
5. If all is well up to this point, then we should be ready to execute `npm start` to ensure that the program runs without any nasty errors. If it works as expected, then you should see something like the following output:
```terminaloutput
> simple-instruction-machine@1.0.0 start
> ts-node main.ts

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: .

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

No boot file found.


Program completed successfully
```
6. Success again (hopefully)! Again, it runs without errors, but not boot file has been supplied, so it's not really doing anything.

## Exercise 1
Now it's time to generate a dummy boot file and check it works. We do this by doing the following from the terminal in the `Mark-Henry-Manual-Method` folder:
```shell
cp boot-allzero boot-ex1
cp boot-ex1 boot
```
We've now created a boot file for use in Exercise 1 (boot-ex1) from the template and then copied that to the actual boot file the simulator will use. Time to test the simulator properly, with a blank boot file. First, from the dotnet platform: 
```terminaloutput
dotnet run ../Mark-Henry-Manual-Method/ --debug
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).


Program completed successfully
```
Then, from node:
```terminaloutput
npm start ../Mark-Henry-Manual-Method/

> simple-instruction-machine@1.0.0 start
> ts-node main.ts ../Mark-Henry-Manual-Method/

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory initialised using boot file (../Mark-Henry-Manual-Method/boot).


Program completed successfully
```
## Exercise 2
A blank boot file is now working, so time to throw a virtual spanner in the works to make sure the simulator operates as expected and generates error files when given a weird instruction. To set this up, we need to generate the boot file for example 2 from the template, then edit the file and insert the dodgy instruction as follows:
```shell
cp boot-allzero boot-ex2
hexedit boot-ex2
```
Once opened, change the first byte in the boot file to `FF`, as per the instructions, then save the file and exit hexedit. Once back in the terminal, the following command was executed to copy the modified file to become the new boot file:
```shell
cp boot-ex2 boot
```
Now the boot file is prepared, time to test using dotnet first:
```terminaloutput
dotnet run ../Mark-Henry-Manual-Method/ --debug
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).
Your program has crashed,  things aren't looking to good for the space craft.

Unknown command 255
Instruction Pointer: 0
Opcode: 255

A crash dump containing all the memory has been written to : crash_dump and crash_dump.txt
```
Then, with node:
```terminaloutput
npm start ../Mark-Henry-Manual-Method/ debug

> simple-instruction-machine@1.0.0 start
> ts-node main.ts ../Mark-Henry-Manual-Method/ debug

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory initialised using boot file (../Mark-Henry-Manual-Method/boot).
Your program has crashed!
Unknown command 255
Instruction Pointer: 0
Opcode: 255
```
In both cases, we received the correct output, i.e. opCode `0xFF` is invalid, both in dotnet and node and crash dumps were written into the appropriate locations detailing what was wrong :)

## Exercise 3
In order to complete this exercise, we debated which of the opCodes representing the SaveToFile instructions might be best to use. As the exercise hints strongly which opCode might be the one to use, we opted for the Immediate/Immediate/Immediate option, as this would allow us to specify the file, a location in memory for the data and a length all in one "easy" instruction. To accomplish this, we created the new boot file, then edited the file using hexedit again:
```shell
cp boot-allzero boot-ex3
hexedit boot-ex3
```
Once inside hexedit, starting at address 0, the following entries were input:
`17`, `00`, `20`, `05`, `01`.
Now the program is in-place, using hexedit again, this time at address 0x20, the following entries were made: `05`, `04`, `03`, `02`, `01`. Once completed, the file was saved and hexedit was exited. Now we have the file,time to do a quick check on the contents:
```terminaloutput
hexdump boot-ex3
0000000 0017 0520 0001 0000 0000 0000 0000 0000
0000010 0000 0000 0000 0000 0000 0000 0000 0000
0000020 0405 0203 0001 0000 0000 0000 0000 0000
0000030 0000 0000 0000 0000 0000 0000 0000 0000
*
0000100
```
Hmm... That doesn't look right! A quick review of the `man` page for hexdump hints why: the default output groups contents into pairs of bytes, which means the values are going to appear grouped as if they were 16-bit values. Time to try a different output option:
```terminaloutput
hexdump -C boot-ex3
00000000  17 00 20 05 01 00 00 00  00 00 00 00 00 00 00 00  |.. .............|
00000010  00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  |................|
00000020  05 04 03 02 01 00 00 00  00 00 00 00 00 00 00 00  |................|
00000030  00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  |................|
*
00000100
```
Ok - that looks much better. All bytes are being displayed sequentially and it also tries to convert the values into ASCII text as well! The prepared boot file was then copied as the new boot file using:
```shell
cp boot-ex3 boot
```
Now the boot file is ready, time to test it again, dotnet first, followed by node:
```terminaloutput
dotnet run ../Mark-Henry-Manual-Method/ --debug
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).
00000000 SaveToFile:: Writing 5 bytes starting at 00000020 to file 0.
00000004 Wait::


Program completed successfully


npm start ../Mark-Henry-Manual-Method/ debug

> simple-instruction-machine@1.0.0 start
> ts-node main.ts ../Mark-Henry-Manual-Method/ debug

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory initialised using boot file (../Mark-Henry-Manual-Method/boot).
0 SaveToFile:: Writing 5 bytes from 20.
4 Wait::


Program completed successfully
```
Looks like the program has done as requested, so time to check it:
```terminaloutput
hexdump -C ../Mark-Henry-Manual-Method/0
00000000  05 04 03 02 01                                    |.....|
00000005
```
Looks like we have the right contents in the specified file, so we'll count that as successful!

## Exercise 4
As this looks like a very similar exercise as the previous one, we re-used the previous file, copying it to provide the boot-ex4 file we needed, then edited it:
```shell
cp boot-ex3 boot-ex4
hexedit boot-ex4
```
Inside hexedit, instead of using the hex editing facility, time to use the character side - that'll make life so much easier! Once there, at address 0x20, the string `5 4 3 2 1 LIFTOFF` was entered, noting that it needed 17 bytes (0x11 hex) to store it. Now the string is entered, time alter the opCode instructions so they perform the correct operation: switching back to the hex side, address 0x01 was altered to `09` and address 0x03 was altered to `11`, ensuring the output is directed to file 9 and has a length of 17 bytes. Saving the file in hexedit and quitting, we checked the file for correctness:
```terminaloutput
hexdump -C boot-ex4
00000000  17 09 20 11 01 00 00 00  00 00 00 00 00 00 00 00  |.. .............|
00000010  00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  |................|
00000020  35 20 34 20 33 20 32 20  31 20 4c 49 46 54 4f 46  |5 4 3 2 1 LIFTOF|
00000030  46 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  |F...............|
00000040  00 00 00 00 00 00 00 00  00 00 00 00 00 00 00 00  |................|
*
00000100
```
That looks good, so time to copy it as the boot file:
```shell
cp boot-ex4 boot
```
Now, test again with dotnet and node:
```terminaloutput
dotnet run ../Mark-Henry-Manual-Method/ --debug
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).
00000000 SaveToFile:: Writing 17 bytes starting at 00000020 to file 9.
00000004 Wait::


Program completed successfully


npm start ../Mark-Henry-Manual-Method/ debug

> simple-instruction-machine@1.0.0 start
> ts-node main.ts ../Mark-Henry-Manual-Method/ debug

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory initialised using boot file (../Mark-Henry-Manual-Method/boot).
0 SaveToFile:: Writing 17 bytes from 20.
4 Wait::


Program completed successfully
```
As the output should be a text file, we checked the output using the following:
```terminaloutput
cat 9.txt 
5 4 3 2 1 LIFTOFF
```
Again, that looks good, so we'll take that as a success :)

## Exercise 5
After looking at the description, we decided to make this a "2 boot file" approach - one to actually write the fuel quantity to file 1, then another to decrement the fuel value. To start with, we created the two new boot files to be edited using the following commands:
```shell
cp boot-allzero boot-refuel123
cp boot-allzero boot-ex5
```
Looking at the instructions, the first step of writing a program to "refuel" should be very similar to writing output in earlier exercises, so we'll re-use the same methods. Starting with the command:
```shell
hexedit boot-refuel123
```
We then entered, starting from address 0x00, the following: `17`, `01`, `20`, `01`, then at address 0x20, the value `7B` (hex for 123 decimal) was entered and the file saved and hexedit quit. Checking the file works as expected, the following was executed:
```terminaloutput
cp boot-refuel123 boot

dotnet run ../Mark-Henry-Manual-Method/ --debug && hexdump -C ../Mark-Henry-Manual-Method/1
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).
00000000 SaveToFile:: Writing 1 bytes starting at 00000020 to file 1.


Program completed successfully
00000000  7b                                                |{|
00000001
```
Now we have the "refuel" program working as expected, time to move on to the program to read the fuel level, decrement it, then write out the new value. The plan is to:
1. Use one of the LoadFromFile opCodes to load the current fuel value into a "data area" in the simulator's memory
2. Decrement the value in the "data area"
3. Write out the new value to the file
4. Use a "high" memory address that sits between the end of the program and under the stack area. For no real reason, address 0xC0 was used, as it fits those requirements pretty well.

The decision was made to use the "immediate" memory access methods as these work very nicely for us. Running `hexedit boot-ex5` allows us to enter the following hex values for the program: `23`, `01`, `C0`, `71`, `C0`, `17`, `01`, `C0`, `01`. Saving the file and quitting hexedit, the following commands were executed:
```terminaloutput
cp boot-ex5 boot

dotnet run ../Mark-Henry-Manual-Method/ --debug && hexdump -C ../Mark-Henry-Manual-Method/1
Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory has been initialised using the boot file (../Mark-Henry-Manual-Method/boot).
00000000 LoadFromFile:: Reading from file 1 into 000000c0.
00000003 Dec::  Decreasing value in 000000c0 from 123 to 122
00000005 SaveToFile:: Writing 1 bytes starting at 000000c0 to file 1.


Program completed successfully
00000000  7a                                                |z|
00000001
```
All looking good so far - dotnet opens the file, decrements and writes it back. Time for node to do the same:
```terminaloutput
npm start ../Mark-Henry-Manual-Method/ debug && hexdump -C ../Mark-Henry-Manual-Method/1

> simple-instruction-machine@1.0.0 start
> ts-node main.ts ../Mark-Henry-Manual-Method/ debug

Starting landing computer running York's Obscenely Dumb Architecture (YODA) - Release Build 12x.11g-34 + Anti-gravity module
Folder Path: ../Mark-Henry-Manual-Method/

Connecting to Engine Control System.... SUCCESS!
Connecting to Landing Control System.... SUCCESS!
Connecting to Interplanetary Communication System.... SUCCESS!
All systems are GO!

Memory initialised using boot file (../Mark-Henry-Manual-Method/boot).
0 LoadFromFile:: Reading into c0.
3 Dec:: 122 -> 121
5 SaveToFile:: Writing 1 bytes from c0.


Program completed successfully
00000000  79                                                |y|
00000001
```
Success - node also does exactly as expected and we've now used 2 fuel units! Hope that won't compromise the launch too much... Although we could always refuel ;)