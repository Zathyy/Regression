using Sharpmake; // contains the entire Sharpmake object library.
using System;
using System.IO;

[module: Sharpmake.Include("src/Common/CommonTargets.cs")]
[module: Sharpmake.Include("src/Common/SolutionCommon.cs")]
[module: Sharpmake.Include("src/Common/ProjectCommon.cs")]
[module: Sharpmake.Include("src/Regression.Sharpmake.cs")]

namespace Regression;

[Generate]
public class RegressionSolution : SolutionCommon
{
    public RegressionSolution()
    {
        Name = "Regression";
        AddTargets(Target.GetDefaultTargets(false));
    }

    public override void ConfigureAll(Solution.Configuration conf, Target target)
    {
        base.ConfigureAll(conf, target);
        conf.Name = @"[target.Optimization]_[target.OutputType]";
        conf.SolutionPath = @"[solution.SharpmakeCsPath]\generated";

        // Adds the projects to the solution. Note that because the executable
        // has a dependency to the library, Sharpmake can automatically figures
        // out that it should add the library to the solution too, so the
        // second line is not actually needed.
        conf.AddProject<RegressionProject>(target);
    }
}

public static class Main
{
    [Sharpmake.Main]
    public static void SharpmakeMain(Sharpmake.Arguments arguments)
    {
        FileInfo sharpmakeFileInfo = Sharpmake.Util.GetCurrentSharpmakeFileInfo();
        string sharpmakeFileDirectory = Sharpmake.Util.PathMakeStandard(sharpmakeFileInfo.DirectoryName);
        Globals.AbsoluteRootPath = Sharpmake.Util.PathGetAbsolute(sharpmakeFileDirectory, Globals.RelativeRootPath);

        // Tells Sharpmake to generate the solution described by
        // SimpleLibrarySolution.
        arguments.Generate<RegressionSolution>();
    }
}