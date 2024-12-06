using Sharpmake; // contains the entire Sharpmake object library.
using System;

[module: Sharpmake.Include("../tp/slang/Slang.Sharpmake.cs")]

namespace Regression;

[Generate]
class RegressionProject : BaseProject
{
    public RegressionProject()
    {
        Name = "Regression";
        SourceRootPath = @"[project.SharpmakeCsPath]";
        AddTargets(Target.GetDefaultTargets(false));
    }

    public override void ConfigureAll(Project.Configuration conf, Target target)
    {
        // Change this
        bool useSDK = false;



        base.IncludeVulkan = useSDK;
        base.ConfigureAll(conf, target);

        // Sets the include path of the library. Those will be shared with any
        // project that adds this one as a dependency. (The executable here.)
        conf.IncludePaths.Add(@"[project.SharpmakeCsPath]");

        if (!useSDK)
        {
            conf.AddPublicDependency<SlangProject>(target);
        }

        int value = useSDK ? 1 : 0;

        conf.Defines.Add($"USE_SDK={value}");


        conf.Output = Configuration.OutputType.Exe;
    }
}