using Sharpmake; // contains the entire Sharpmake object library.
using System;

namespace Regression;

[Generate]
class SlangProject : BaseProject
{
    public SlangProject()
    {
        Name = "Slang";
        SourceRootPath = @"[project.SharpmakeCsPath]";

        AddTargets(Target.GetDefaultTargets(false));
    }

    public override void ConfigureAll(Project.Configuration conf, Target target)
    {
        base.ConfigureAll(conf, target);
        conf.SolutionFolder = "Extern";

        conf.IncludePaths.Add(@"[project.SharpmakeCsPath]/include");

        if (target.Optimization == Optimization.Debug)
        {
            conf.LibraryPaths.Add(@"[project.SharpmakeCsPath]/Debug/lib");

            conf.LibraryFiles.AddRange(new[]
            {
                "slang.lib",
            });

            conf.TargetDependsFiles.Add(@"[project.SharpmakeCsPath]/Debug/bin/slang.dll");
        }
        else
        {
            conf.LibraryPaths.Add(@"[project.SharpmakeCsPath]/Release/lib");

            conf.LibraryFiles.AddRange(new[]
            {
                "slang.lib"
            });

            conf.TargetDependsFiles.Add(@"[project.SharpmakeCsPath]/Release/bin/slang.dll");
            conf.TargetDependsFiles.Add(@"[project.SharpmakeCsPath]/Release/bin/gfx.dll");
        }

        conf.Output = Configuration.OutputType.Lib;
    }
}
