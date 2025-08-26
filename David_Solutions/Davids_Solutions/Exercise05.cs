namespace Davids_Solutions;

public static class Exercise05
{
    /// <summary>
    /// Decreases the value in File1 by 1 each time it runs
    /// </summary>
    public static async Task Do()
    {
        var fileNumber = (byte)1;
        var pValue = (byte)0x0A;
        
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            // Commands
            OpCode.LoadFromFileII,  //0x0   
            fileNumber,             //0x1 file number                                  (immediate)
            pValue,                 //0x2 write the content of this memory location.   (immediate)

            OpCode.DecI,            //0x3
            pValue,                 //0x4
            
            OpCode.SaveToFileIII,   //0x5 
            fileNumber,             //0x6 file number                                  (immediate)
            pValue,                 //0x7 write the content of this memory location.   (immediate)
            1,                      //0x8 number of bytes to write                     (immediate)
            
            OpCode.Halt,            //0x9
            
            // Variables
            0,                      //0x0A
        ]);

        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/1",
        [
            16
        ]);

    }
}