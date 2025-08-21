namespace Davids_Solutions;

public static class Exercise01
{
    // The simplest program does nothing but halt
    public static async Task Do()
    {
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            0       // HALT
        ]);
        
    }
}