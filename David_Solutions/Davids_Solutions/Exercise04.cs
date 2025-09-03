namespace Davids_Solutions;

public static class Exercise04
{
    /// <summary>
    /// Writes the letters 5 4 3 2 1 LIFT OFF to File9
    /// </summary>
    public static async Task Do()
    {
        
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            // Commands
            23,                    // WRITE TO FILE
            9,                     // file number                                  (immediate)
            0x5,                   // write the content of this memory location.   (immediate)
            17,                    // number of bytes to write                     (immediate)
            
            0,                     // HALT   
            
            // Variables
            53,                   //5
            32,                   //Space
            52,                   //4
            32,                   //Space
            51,                   //3
            32,                   //Space
            50,                   //2
            32,                   //Space
            49,                   //1
            32,                   //Space
            76,                   //L
            73,                   //I
            70,                   //F
            84,                   //T
            79,                   //O
            70,                   //F
            70,                   //F
        ]);
        
        if (Path.Exists(Path.Combine(Constants.FilesFolder, "9.txt")))
            File.Delete(Path.Combine(Constants.FilesFolder, "9.txt"));
        
    }
}