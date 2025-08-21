namespace Davids_Solutions;

public static class Exercise02
{
    // This application will crash the machine, as it attempts to execute an unknown opcode
    public static async Task Do()
    {
        await File.WriteAllBytesAsync(Constants.BootFile, 
        [
            0xFF    // Unknown opcode
        ]);
        
    }
}