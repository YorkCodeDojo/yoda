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

## Step 1.3 - Exercise 1
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
~~~~