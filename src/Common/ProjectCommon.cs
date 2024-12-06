using System.IO;
using Sharpmake;
using System;

namespace Regression;

public static class Globals
{
    // branch root path relative to current Vital.sharpmake location
    public static string RelativeRootPath = string.Empty;
    public static string AbsoluteRootPath = string.Empty;

    public static string ProjectCodeFolderPath = @"/";
    public static string VitalCodeFolderPath = @"source/";

    public static string OutputFolderName = @"SM_Output/";
    public static string BuildFolderName = @"SM_Build/";
    public static string TmpFolderName = @"SM_Tmp/";

    public static string GenerationSummaryPath = string.Empty;

    // Fastbuild DB Version
    // Modify this version when you need to delete the .fdb files to be sure a configuration change is applied correctly
    // For example if a platform is starting to use subst drive... Otherwise the fdb file will still contain the unsubsted
    // filenames and this will make the unity<N>.cpp files contain the real filepath instead of substed path...
    // Ideally fastbuild should handle this transparently but it doesn't right now and this will do for now.
    public static int FastBuildDBVersion = 1;

    public static Builder Builder = null;

    public static string ProjectName = "Regression";
}

// Both the library and the executable can share these base settings, so create
// a base class for both projects.
abstract class BaseProject : Project
{
    public string CodeFolderPath = @"[project.RootPath]\" + Globals.ProjectCodeFolderPath;
    public string VitalCodeFolderPath = @"[project.RootPath]\" + Globals.VitalCodeFolderPath;
    public string DataFolderPath = @"[project.RootPath]\Data";
    public string LibsFolderPath = @"[project.CodeFolderPath]\Libs";
    public string ExternFolderPath = @"[project.CodeFolderPath]\Extern";
    public string VitalExternFolderPath = @"[project.VitalCodeFolderPath]\Extern";
    public string ProjectsFolderPath = "[project.SourceRootPath]";
    public string OutputFolderPath = @"[project.CodeFolderPath]" + Globals.OutputFolderName;
    public string IntermediateFolderPath = @"[project.CodeFolderPath]" + Globals.BuildFolderName;
    public string TmpFolderPath = @"[project.CodeFolderPath]" + Globals.TmpFolderName;
    
    public bool IncludeVulkan = false;
    public bool InlcudeSlangWithVulkanSDK = false;

    public bool SlangFromSDK = false;

    public BaseProject()
        : base(typeof(Target))
    {
        RootPath = Globals.AbsoluteRootPath;
        SourceRootPath = string.Empty;
    }

    [Configure(Optimization.Release)]
    public void ConfigureRelease(Project.Configuration conf, Target target)
    {
        conf.Options.Add(Options.Vc.Compiler.FavorSizeOrSpeed.FastCode);
        conf.Options.Add(Options.Vc.Compiler.Inline.AnySuitable);
        conf.Options.Add(Options.Vc.Compiler.Optimization.FullOptimization);
        conf.Options.Add(Options.Vc.Compiler.StringPooling.Enable);
        conf.Options.Add(Options.Vc.Compiler.Exceptions.Disable);
        conf.Options.Add(Options.Vc.Compiler.RTTI.Disable);
    }

    [Configure]
    public virtual void ConfigureAll(Project.Configuration conf, Target target)
    {
        conf.Name = "[target.Name]";

        conf.ProjectFileName = "[project.Name]";
        conf.ProjectPath = "[project.ProjectsFolderPath]";

        conf.TargetPath = @"[project.OutputFolderPath][conf.Name]_[target.Platform]_[target.DevEnv]";
        conf.IntermediatePath = @"[project.IntermediateFolderPath][conf.Name]_[target.Platform]_[target.DevEnv]_[project.Name]";
        conf.TargetLibraryPath = conf.IntermediatePath;

        //conf.CreateTargetCopyCommand = Utilities.CreateCopyCommandWithRobocopy;

        conf.BlobPath = @"[project.TmpFolderPath]\Blob[project.Name]_[target.Platform]_[target.DevEnv]";
        
        #region VULAN_SDK

        if (IncludeVulkan)
        {
            string vulkanSDK = Environment.GetEnvironmentVariable("VULKAN_SDK");

            if (vulkanSDK == null)
            {
                throw new Error("VULKAN_SDK environment variable is not defined");
            }
        
            conf.IncludePaths.Add($"{vulkanSDK}/include");
            conf.LibraryPaths.Add($"{vulkanSDK}/lib");

            conf.LibraryFiles.AddRange(new []
            {
                "vulkan-1.lib",
            });

            if (target.Optimization == Optimization.Debug)
            {
                conf.LibraryFiles.AddRange(new []
                {
                    "shaderc_sharedd.lib",
                    "spirv-cross-cored.lib",
                    "spirv-cross-glsld.lib",
                    "SPIRV-Toolsd.lib",
                    "slangd.lib",
                });
            }
            else
            {
                conf.LibraryFiles.AddRange(new []
                {
                    "shaderc_shared.lib",
                    "spirv-cross-core.lib",
                    "spirv-cross-glsl.lib",
                    "slang.lib",
                });
            }
        }

        #endregion

        if (target.OutputType == OutputType.Dll)
            conf.Output = Configuration.OutputType.Dll;
        else
            conf.Output = Configuration.OutputType.Lib;

        conf.TargetFileSuffix = "[target.TargetSuffix]";

        conf.Defines.Add("UNICODE");

        //conf.Options.Add(new Options.Vc.Librarian.DisableSpecificWarnings("26812"));

        conf.Options.Add(Options.Vc.General.WindowsTargetPlatformVersion.Latest);
        conf.Options.Add(Options.Vc.Compiler.RTTI.Enable);
        conf.Options.Add(Options.Vc.Compiler.CppLanguageStandard.CPP20);
        conf.Options.Add(Options.Vc.General.WarningLevel.Level4);
        conf.Options.Add(Options.Vc.General.TreatWarningsAsErrors.Disable);
        conf.Options.Add(Options.Vc.Linker.TreatLinkerWarningAsErrors.Enable);
        conf.Options.Add(Options.Vc.General.CharacterSet.Unicode);
        conf.AdditionalCompilerOptions.Add("/utf-8");

        conf.IsFastBuild = target.BuildSystem == BuildSystem.FastBuild;
        if (conf.IsFastBuild)
        {
            conf.ProjectName += "_FastBuild";
            conf.ProjectFileName += ".FastBuild";
        }
    }
}