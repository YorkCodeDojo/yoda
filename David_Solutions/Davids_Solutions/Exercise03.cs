namespace Davids_Solutions;

/// <summary>
/// Add two numbers
/// </summary>
public class Exercise03
{
    public static async Task Do()
    {

        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0", 
        [
            // Loads the contents of file 1 into 100 and 101
            Command.LoadFromFile, //0x0
            0x14,                 //0x1 (FileNumber is held in 0x10)
            0x15,                 //0x2 (0x11 holds the location of the LHS)
            
            // Start of loop
            Command.Nop,          //0x3
            
            // Check if we have run out of numbers to add
            Command.JumpIfZero,   //0x4  
            0x16,                 //0x5 (Location of the RHS)
            0xE,                  //0x6. (EndOfLoop)
            
            // Increase the left hand side
            Command.Inc,         //0x7 
            0x15,                 //0x8 
            
            // Decrease the right hand side
            Command.Dec,         //0x9
            0x16,                //0xA 
            
            // Continue loop (always)
            Command.JumpIfZero, //0xB 
            0xFF,               //0xC (Value is always zero)
            0x3,                //0xD (StartOfLoop)
            
            // End of loop
            Command.Nop,         //0xE
            
            // Write result to a file
            Command.SaveToFile,  //0xF
            0x17,                //0x10. File2
            0x15,                //0x11. LHS
            0x18,                //0x12, 1 byte
            
            Command.Halt,        //0x13
            
            // Constants
            1,                  //0x14 (source file number)
            100,                //0x15 (LHS)
            101,                //0x16 (RHS)
            2,                  //0x17 (Target file)
            1,                  //0x18 (length)
        ]);

        // Input file
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/1", [4,5]);
    }
}