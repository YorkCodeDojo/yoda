namespace Davids_Solutions;

public static class Exercise06
{
    /// <summary>
    /// Write 5 - 4 -3 - 2 - 1 to the display
    /// </summary>
    public static async Task Do()
    {
        var pValuesRemaining = (byte)0xD2;
        var pDigit = (byte)0xD1;
        var endAddress = (byte)0x2C;
        
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            OpCode.WriteII, pValuesRemaining, 5,   // 0x00 Initialise number of values remaining
            OpCode.WriteII, pDigit, (byte)'5',     // 0x03 Initialise ASCII value of first digit to display
            
            // ;Start of loop
            OpCode.JumpIfZeroMI,  pValuesRemaining, endAddress, // 0x06 Jump to the end if we are out of value.
            OpCode.WriteIM,  Screen.LCD_2,   pDigit,  // 0x09 Display the first digit
            OpCode.WriteII,  Screen.ControlFlags,   0,  // 0x0C Refresh the screen
            OpCode.WriteII,  Screen.ControlFlags,   1,  // 0x0FRefresh the screen
            OpCode.DecI, pDigit, // 0x013
            OpCode.DecI, pValuesRemaining, //0x015
            OpCode.Wait,  //0x017
            OpCode.JumpIfZeroMI,  0xD3, 0x06, // 0x018 Back to the top of the loop
            
            // ;End of loop
            OpCode.Halt,            //0x2C End
            
        ]);

    }
}