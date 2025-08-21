namespace Davids_Solutions;

/// <summary>
/// Stack based functions
/// </summary>
public class Exercise04
{
    public static async Task Do()
    {
        // File0 contains our program
        // File1 contains the Add function
        // File2 contains the numbers to be added
        // File3 will contain the result

        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/0",
        [
            OpCode.JumpIfZero,   //0x00  
            0xFF,                 //0x01 (Final byte is always zero)
            0x0B,                 //0x02 (Start of code)
            
            // Constants
            1,                  //0x03 (file containing our Add function)
            2,                  //0x04 (file containing our Input)
            3,                  //0x05 (file containing our Result)
            200,                //0x06 (start of Add function)
            100,                //0x07 First number to add
            101,                //0x08 Second number to add
            102,                //0x09 Location of the result
            0xFE,               //0x0A Top of stack (grows down)
        
            // Loads the contents of file 1 (the add function) into 200 onwards
            OpCode.LoadFromFileII, //0x0B
            0x03, //0x0C (FileNumber)
            200, //0x0D (Memory location)

            
            // Loads the contents of file 2 (our inputs) into 100 and 101
            OpCode.LoadFromFileII, //0x0E
            0x04, //0x0F (FileNumber)
            100,  //0x10 (LHS)
            
            // Calling convention is....
            //   other stack entries
            //   arg0
            //   arg1
            //   argN
            //   return Address
            //   result (added by function before return)
            
            // Push LHS to the stack
            OpCode.Copy,
            0x06,   // Address of the memory location for the address of the LHS
            0x08,    // Address of the memory location holding the top of the stack        
            OpCode.Dec, 0x08, // Move the stack from down 

            // Push RHS to the stack
            OpCode.Copy,
            0x07,                // Address of the memory location for the address of the RHS
            0x08,                // Address of the memory location holding the top of the stack        
            OpCode.Dec, 0x08,   // Move the stack from down 
            
            // Push Return Address to the stack
            OpCode.Copy,
            0x07,                // Address of the memory location for the address of the RHS
            0x08,                // Address of the memory location holding the top of the stack        
            OpCode.Dec, 0x08,   // Move the stack from down 
            
            // Run the function
            OpCode.JumpIfZero,   //0x00  
            0xFF,                 //0x01 (Final byte is always zero)
            0xE,                  //0x02 (Start of code)
            
            // Return Address
            OpCode.Nop,         //0xE
            
            // Preserve the result
            OpCode.Inc, 0x08,   // Move the stack up
            OpCode.Copy,
            0x08,                // Address of the memory location for the address of the RHS
            0x08,                // Address of the memory location holding the top of the stack  
            
            // Restore the stack pointer (+3)
            OpCode.Inc, 0x08,   // Move the stack up (return address)
            OpCode.Inc, 0x08,   // Move the stack up (arg 2)
            OpCode.Inc, 0x08,   // Move the stack up (arg 1)
            
            // Write result to a file
            OpCode.SaveToFileIII,  //0xF
            0x05,                //0x10. File2
            0x15,                //0x11. LHS
            0x18,                //0x12, 1 byte
            
            OpCode.Halt,        //0x13
     
            
        ]);
        
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/1", 
        [
            // Pop inputs from the stack
            
            // Start of loop
            OpCode.Nop,          //0x3
            
            // Check if we have run out of numbers to add
            OpCode.JumpIfZero,   //0x4  
            0x16,                 //0x5 (Location of the RHS)
            0xE,                  //0x6. (EndOfLoop)
            
            // Increase the left hand side
            OpCode.Inc,         //0x7 
            0x15,                 //0x8 
            
            // Decrease the right hand side
            OpCode.Dec,         //0x9
            0x16,                //0xA 
            
            // Continue loop (always)
            OpCode.JumpIfZero, //0xB 
            0xFF,               //0xC (Value is always zero)
            0x3,                //0xD (StartOfLoop)
            
            // End of loop
            OpCode.Nop,         //0xE
            
            // Write result to a file
            OpCode.SaveToFileIII,  //0xF
            0x17,                //0x10. File2
            0x15,                //0x11. LHS
            0x18,                //0x12, 1 byte
            
            OpCode.Halt,        //0x13
            
            // Constants
            1,                  //0x14 (source file number)
            100,                //0x15 (LHS)
            101,                //0x16 (RHS)
            2,                  //0x17 (Target file)
            1,                  //0x18 (length)
        ]);

        // Input file
        await File.WriteAllBytesAsync("/Users/davidbetteridge/SimpleInstructionMachine/Files/2", [4,5]);
    }
}