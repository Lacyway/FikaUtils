using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace FikaUtils
{
	internal class Program
	{
		static void Main(string[] args)
		{
			string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();

#if RELEASE
			using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
			{
				WindowsPrincipal principal = new(identity);
				if (!principal.IsInRole(WindowsBuiltInRole.Administrator))
				{
					LogError("Executable does not have admin rights! Please run as admin. Quitting...");
					Console.ReadKey();
					Environment.Exit(1);
				}
			}
#endif

			Console.WriteLine($"Fika Utils loaded! Version {version}");
			Console.WriteLine();
			PrintTasks();
			while (!Console.KeyAvailable)
			{
				switch (Console.ReadKey(true).Key)
				{
					case ConsoleKey.D1:
						SetupFirewallRules();
						break;
					case ConsoleKey.D2:
						RemoveFirewallRules();
						break;
					case ConsoleKey.Escape:
						Environment.Exit(0);
						break;
					default:
						LogError("Incorrect key!");
						break;
				}
			}
		}

		private static void LogError(string message, bool restart = false)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ResetColor();
			if (restart)
			{
				Console.WriteLine("Press any key to go back...");
				Console.ReadKey(true);
				Console.Clear();
				PrintTasks();
			}
		}

		private static void LogHeader(string message)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void LogWarning(string message)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		private static void PrintTasks()
		{
			Console.WriteLine("Select a task:");
			Console.WriteLine("1 - Add Firewall Rules");
			Console.WriteLine("2 - Remove Firewall Rules");
			Console.WriteLine();
			Console.WriteLine("Click a button to continue...");
		}

		private static void SetupFirewallRules()
		{
			LogHeader("Setting up Firewall Rules");
			string runningDir = Path.GetDirectoryName(AppContext.BaseDirectory);
			if (string.IsNullOrEmpty(runningDir))
			{
				LogError("Unable to find running path! Make sure you are running the executable with admin rights!", true);
				return;
			}

#if RELEASE
			if (!Path.Exists(runningDir + @"\SPT.Server.exe"))
			{
				LogError("Unable to find 'SPT.Server.exe', make sure you extracted the executable to your SPT installation folder!", true);
				return;
			}

			if (!Path.Exists(runningDir + @"\EscapeFromTarkov.exe"))
			{
				LogError("Unable to find 'EscapeFromTarkov.exe', make sure you extracted the executable to your SPT installation folder!", true);
				return;
			}
#endif

			ProcessStartInfo info = new("netsh.exe")
			{
				Arguments = "advfirewall firewall add rule name=\"#FIKA TCP 6969 IN\" dir=in action=allow protocol=TCP localport=6969 enable=yes profile=public,private",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				RedirectStandardInput = true
			};
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = "advfirewall firewall add rule name=\"#FIKA TCP 6969 OUT\" dir=out action=allow protocol=TCP localport=6969 enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = "advfirewall firewall add rule name=\"#FIKA UDP 25565 IN\" dir=in action=allow protocol=UDP localport=25565 enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = "advfirewall firewall add rule name=\"#FIKA UDP 25565 OUT\" dir=out action=allow protocol=UDP localport=25565 enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA Tarkov IN\" dir=in action=allow program=\"{runningDir}\\EscapeFromTarkov.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA Tarkov OUT\" dir=out action=allow program=\"{runningDir}\\EscapeFromTarkov.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA SPT.SERVER IN\" dir=in action=allow program=\"{runningDir}\\SPT.Server.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA SPT.SERVER OUT\" dir=out action=allow program=\"{runningDir}\\SPT.Server.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA SPT.LAUNCHER IN\" dir=in action=allow program=\"{runningDir}\\SPT.Launcher.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			info.Arguments = $"advfirewall firewall add rule name=\"#FIKA SPT.LAUNCHER OUT\" dir=out action=allow program=\"{runningDir}\\SPT.Launcher.exe\" enable=yes profile=public,private";
			Console.WriteLine($"Running command: netsh {info.Arguments}");
			Process.Start(info).WaitForExit();
			
			ResetConsole();
			PrintTasks();
		}

		private static void ResetConsole()
		{
			Console.WriteLine();
			Console.WriteLine("Done! Press any key to go back...");
			Console.ReadKey(true);
			Console.Clear();
		}

		private static void RemoveFirewallRules()
		{
			LogWarning("WARNING: This will delete all TCP rules for 6969 and UDP rules for 25565 and all Fika specific rules.\nAre you sure? Y/N");
			bool accepted = false;
			while (!Console.KeyAvailable && !accepted)
			{
				switch (Console.ReadKey(true).Key)
				{
					case ConsoleKey.Y:
						accepted = true;
						break;
					case ConsoleKey.N:
						Console.Clear();
						PrintTasks();
						return;
					default:
						LogError("Incorrect key!");
						break;
				}
			}

			string runningDir = Path.GetDirectoryName(AppContext.BaseDirectory);
			if (string.IsNullOrEmpty(runningDir))
			{
				LogError("Unable to find running path! Make sure you are running the executable with admin rights!", true);
				return;
			}

			string firewallScript = Path.Join(runningDir, @"\RemoveFirewallRules.ps1");
			LogHeader("Removing old Firewall Rules");

			using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FikaUtils.RemoveFirewallRules.ps1");
			using StreamReader sr = new(stream);
			string content = sr.ReadToEnd();

			if (File.Exists(firewallScript))
			{
				File.Delete(firewallScript);
			}
			File.WriteAllText(firewallScript, content);
			ProcessStartInfo info = new()
			{
				UseShellExecute = false,
				Arguments = $"-ExecutionPolicy Bypass -File {firewallScript}",
				FileName = "powershell.exe",
				CreateNoWindow = true
			};
			Process.Start(info).WaitForExit();

			File.Delete(firewallScript);

			ResetConsole();
			PrintTasks();
		}
	}
}
