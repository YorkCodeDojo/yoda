namespace Davids_Solutions;

public static class Exercise01
{
    public static async Task Do()
    {
        
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", 
        [
            // Commands
            OpCode.SaveToFileIII, //0x0
            1,                     //0x1  -- file number in location 5                    (immediate)
            0x5,                   //0x2. -- write the content of this memory location.   (immediate)
            1,                     //0x3  -- number of bytes to write                     (immediate)
            
            OpCode.Halt,          //0x4
            
            // Variables
            124,                   //0x5 (our value)
        ]);
        
    }
}