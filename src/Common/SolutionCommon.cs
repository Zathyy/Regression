using Sharpmake; // contains the entire Sharpmake object library.
using System;

namespace Regression;

public class SolutionCommon : Solution
{
    public SolutionCommon() : base(typeof(Target))
    {
        Name = "SolutionCommon";
    }

    [Configure]
    public virtual void ConfigureAll(Solution.Configuration conf, Target target)
    {
        conf.SolutionPath = @"[solution.RootPath]" + Globals.ProjectCodeFolderPath + @"\solutions";
        conf.SolutionFileName = "[solution.Name][target.DevEnv]";
        conf.Options.Add(Options.Vc.General.CharacterSet.Unicode);
    }
}
