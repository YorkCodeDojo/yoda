namespace Davids_Solutions;

public static class Exercise03
{
    /// <summary>
    /// Writes the numbers 5 4 3 2 1 to File0
    /// </summary>
    public static async Task Do()
    {
        
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            // Commands
            23,                    // WRITE TO FILE
            0,                     // file number                                  (immediate)
            0x5,                   // write the content of this memory location.   (immediate)
            5,                     // number of bytes to write                     (immediate)
            
            0,                     // HALT
            
            // Variables
            5,                   // (our value)
            4,                   // (our value)
            3,                   // (our value)
            2,                   // (our value)
            1,                   // (our value)
        ]);
        
    }
}