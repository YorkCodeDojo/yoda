namespace Davids_Solutions;

public class Exercise1
{
    static class Command
    {
        public const byte Halt = 0;
        public const byte LoadFromFile = 1;
        public const byte SaveToFile = 2;
        public const byte Write = 3;
        public const byte Add = 4;
        public const byte Inc = 5;
        public const byte Dec = 6;
    }
    
    public static async Task Do()
    {
        
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", 
        [
            // Commands
            Command.SaveToFile, //0x0
            0x5,                //0x1
            0x6,                //0x2
            0x7,                //0x3,
            
            Command.Halt,       //0x4
            
            // Constants
            1,                  //0x5 (file number)
            123,                //0x6 (value to write)
            1                   //0x7 (number of bytes to write)
        ]);
        
    }
}