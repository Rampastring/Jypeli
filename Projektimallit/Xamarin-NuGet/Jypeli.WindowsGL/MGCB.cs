﻿using MonoDevelop.Core;
using MonoDevelop.Ide;
using MonoDevelop.Projects;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Jypeli.Projects
{
	/// <summary>
	/// MonoGame Content Builder
	/// </summary>
	public class MGCB
	{
		private string mgcbPath;
		private FilePath contentDir;
		private FilePath outputDir;

		public MGCB(FilePath contentDir, FilePath outputDir)
		{
			this.contentDir = contentDir.CanonicalPath;
			this.outputDir = outputDir.CanonicalPath;
			this.mgcbPath = null;

			string assemblyPath = Path.GetDirectoryName(typeof(MGCB).Assembly.Location);
			if (assemblyPath.EndsWith(@"bin\Debug", StringComparison.InvariantCulture))
			{
				// We're debugging the plugin
				mgcbPath = Path.Combine(assemblyPath, @"..\..\tools\MGCB\MGCB.exe");
			}
			else
			{
				var mgcbPaths = new string[]
				{
					Path.Combine(assemblyPath, "tools", "MGCB", "MGCB.exe"),
					Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"MSBuild\MonoGame\v3.0\Tools", "MGCB.exe"),
					//Path.Combine("/Applications/Pipeline.app/Contents/MonoBundle", "MGCB.exe"),
					//Path.Combine("/bin", "mgcb")
				};

				foreach (var path in mgcbPaths)
				{
					if (!File.Exists(path))
						continue;

					this.mgcbPath = path;
					break;
				}
			}

			if (this.mgcbPath == null)
				throw new NotSupportedException("MGCB.exe not found");
		}

		public void BuildContent(ProgressMonitor monitor, FilePath contentFile, string importer, string processor)
		{
			FilePath workingDir = contentFile.CanonicalPath.ParentDirectory;
			FilePath contentSubdir = workingDir.ToRelative(this.contentDir);
			string outDir = this.outputDir.Combine(contentSubdir).ToString();
			string justFileName = contentFile.ToRelative(workingDir).FileName;

			var process = new Process();
			process.StartInfo.WorkingDirectory = workingDir.ToString();

			if (!Directory.Exists(outDir))
				Directory.CreateDirectory(outDir);

			if (Environment.OSVersion.Platform == PlatformID.Win32NT)
			{
				process.StartInfo.FileName = this.mgcbPath;
				process.StartInfo.Arguments = "/platform:WindowsGL";
			}
			else if (Directory.Exists("/Applications") && Directory.Exists("/Users"))
			{
				process.StartInfo.FileName = "mono";
				process.StartInfo.Arguments = string.Format("\"{0}\" /platform:MacOS", this.mgcbPath);
			}
			else {
				process.StartInfo.FileName = this.mgcbPath;
				process.StartInfo.Arguments = "/platform:Linux";
			}

			process.StartInfo.Arguments += String.Format(
				" /Build:\"{0}\" /Importer:{1} /Processor:{2} /outputDir:\"{3}\"",
				justFileName, importer, processor, outDir);

			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.OutputDataReceived += (sender, args) => monitor.Log.WriteLine(args.Data);

			monitor.Log.WriteLine("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments);

			// Fire off the process.
			process.Start();
			process.BeginOutputReadLine();
			process.WaitForExit();
		}
	}
}