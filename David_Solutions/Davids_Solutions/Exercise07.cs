namespace Davids_Solutions;

public static class Exercise07
{
    /// <summary>
    /// Write -  to the display and move it left and right using arrow keys
    /// </summary>
    public static async Task Do()
    {
        var pCraftLocation = (byte)0xC2;
        var pScreenAddress = (byte)0xC1;
        
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            // ; Craft starts in the centre
            OpCode.WriteII, pCraftLocation, 2,       // 0x00
            OpCode.JumpWithReturnI, 0x20,            // 0x03
            
            // Configure the IVT
            OpCode.WriteII, InterruptVectorTable.LEFT_ARROW, 0x10,   // 0x05
            OpCode.WriteII, InterruptVectorTable.RIGHT_ARROW, 0x18,  // 0x08
            OpCode.Sif, // 0x0B
            
            // Wait for interrupts
            OpCode.Wait,        //0x0C
            OpCode.JumpIfZeroMI, 0xD1, 0xC, //0x0D
            
            // ISR for the Left Arrow
            OpCode.WriteMI,  pScreenAddress,  (byte)' ',  // 0x10 Clear the craft
            OpCode.DecI, pCraftLocation,  //0x13
            OpCode.JumpWithReturnI, 0x20, //0x15
            OpCode.Ret,                   //0x17
            
            // ISR for the Right Arrow
            OpCode.WriteMI,  pScreenAddress,  (byte)' ',  // 0x18 Clear the craft
            OpCode.IncI, pCraftLocation,  //0x1B
            OpCode.JumpWithReturnI, 0x20, //0x1D
            OpCode.Ret,         //0x1F
            
            // Function to write to the screen
            OpCode.AddMII, pCraftLocation, Screen.LCD_0, pScreenAddress,  //0x20
            OpCode.WriteMI,  pScreenAddress,  (byte)'-',  // Display the craft digit
            OpCode.WriteII,  Screen.ControlFlags,   0,  // Refresh the screen
            OpCode.WriteII,  Screen.ControlFlags,   1,  // Refresh the screen
            OpCode.Ret,
            
        ]);

    }
}