namespace Davids_Solutions;

public static class Exercise02b
{
    public static async Task Do()
    {
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", 
        [
            // OpCodes
            OpCode.SaveToFileIII, //0x0
            1,                     //0x1  -- file number in location 5                    (immediate)
            0x5,                   //0x2. -- write the content of this memory location.   (immediate)
            1,                     //0x3  -- number of bytes to write                     (immediate)
            
            OpCode.Halt,          //0x4
            
            // Variables
            123,                   //0x5 (our value)
        ]);
        
        
        // // Locations of variables / constants
        // const byte pFileNumber = 100;
        // const byte pLengthToWrite = 101;
        // const byte pNumberToIncrement = 102;
        //
        // var commands = new byte[]
        // {
        //     // Load file. LoadFromFile [FileNumber] TargetLocation
        //     OpCode.LoadFromFile, pFileNumber, pNumberToIncrement,
        //
        //     // Increment
        //     OpCode.Inc, pNumberToIncrement,
        //
        //     // Save. [FileNumber] SourceLocation [Length]
        //     OpCode.SaveToFile, pFileNumber, pNumberToIncrement, pLengthToWrite
        // };
        // var fixedSize = new byte[1024];
        // commands.CopyTo(fixedSize, 0);
        //
        //
        // // Initial values
        //
        // // Write the file number into memory location 100
        // fixedSize[pFileNumber] = 1;
        //
        // // Number of bytes to write
        // fixedSize[pLengthToWrite] = 1;
        //
        // await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", fixedSize);


        // Input file
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/1", [42]);
    }
}