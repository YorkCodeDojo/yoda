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
            OpCode.JumpWithReturnI, 0x1F,            // 0x03
            
            // Configure the IVT
            OpCode.WriteII, InterruptVectorTable.LEFT_ARROW, 0x0F,   // 0x05
            OpCode.WriteII, InterruptVectorTable.RIGHT_ARROW, 0x17,  // 0x08
            
            // Wait for interrupts
            OpCode.Wait,        //0xB
            OpCode.JumpIfZeroMI, 0xD1, 0xB, //0x0C
            
            // ISR for the Left Arrow
            OpCode.WriteMI,  pScreenAddress,  (byte)' ',  // 0x0F Clear the craft
            OpCode.DecI, pCraftLocation,  //0x12
            OpCode.JumpWithReturnI, 0x1F, //0x14
            OpCode.Ret,                   //0x16
            
            // ISR for the Right Arrow
            OpCode.WriteMI,  pScreenAddress,  (byte)' ',  // 0x17 Clear the craft
            OpCode.IncI, pCraftLocation,  //0x1A
            OpCode.JumpWithReturnI, 0x1F, //0x1C
            OpCode.Ret,         //0x1E
            
            // Function to write to the screen
            OpCode.AddMII, pCraftLocation, Screen.LCD_0, pScreenAddress,  //0x1F
            OpCode.WriteMI,  pScreenAddress,  (byte)'-',  // Display the craft digit
            OpCode.WriteII,  Screen.ControlFlags,   0,  // Refresh the screen
            OpCode.WriteII,  Screen.ControlFlags,   1,  // Refresh the screen
            OpCode.Ret,
            
        ]);

    }
}