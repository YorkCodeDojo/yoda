using System.Reflection;
using SimpleInstructionMachine;

namespace YodaExercises;

internal static class Program
{
	private static string FilePath = null!;
	
	internal static void Main(string[] args)
	{
		//	Get individual path parts to the assembly and where the assembly is in the list
		var pathParts = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
			.Split(Path.DirectorySeparatorChar)
			.ToList();
		var assemblyIndex = pathParts.IndexOf(nameof(YodaExercises));
		//	Reassemble the paths up to the point of the assembly name
		var basePath = string.Join(Path.DirectorySeparatorChar, pathParts[..assemblyIndex]);
		//	Append Files to the path - this is the location of the boot file area
		FilePath = Path.Combine(basePath, "Files");

		//	Make sure the Files folder exists
		if(!Directory.Exists(FilePath))
			Directory.CreateDirectory(FilePath);
		
		// Exercise1();
		Exercise2();
	}

	private static void WriteFile(string name, byte[]? contents)
	{
		//	programs can only occupy 256 bytes - any more will break the sim!
		var fileLength = 1 + byte.MaxValue;
		ArgumentOutOfRangeException.ThrowIfGreaterThan(contents?.Length ?? 0, fileLength, nameof(contents));

		//	Create the 256 byte buffer for the file
		var buffer = new byte[fileLength];
		//	Copy any contents we have been sent to the buffer
		if (contents?.Length > 0)
			contents.CopyTo(buffer, 0);
		//	Dump the buffer to the specified file, ensuring we only retain the name of the file and place it correctly
		File.WriteAllBytes(Path.Combine(FilePath, Path.GetFileName(name)), buffer);
	}

	private static void MakeFileBoot(string name)
	{
		var fileName = Path.Combine(FilePath, Path.GetFileName(name));
		if(!File.Exists(fileName))
			throw new FileNotFoundException($"File {fileName} not found.");
		File.Copy(fileName, Path.Combine(FilePath, "boot"), true);
	}

	private static void RunVm(string program)
	{
		MakeFileBoot(program);

		var vm = new VirtualMachine(true);
		vm.Run(FilePath).Wait();
	}
	
	private static void Exercise1()
	{
		WriteFile("ex1", null);
		RunVm("ex1");
	}

	private static void Exercise2()
	{
		WriteFile("ex2", [0xFF]);
		RunVm("ex2");
	}
}