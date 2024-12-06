using Sharpmake; // contains the entire Sharpmake object library.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

[module: Reference("Sharpmake.CommonPlatforms.dll")]

namespace Regression
{
    [Fragment, Flags]
    public enum Optimization
    {
        Debug   = 1 << 0,
        Release = 1 << 1,
        Dist   = 1 << 2,
    }

    [Fragment, Flags]
    public enum RunTimeLibrary
    {
        Static = 1 << 0,
        Dynamic = 1 << 1,
    }

    [DebuggerDisplay("\"{Platform}_{DevEnv}\" {Name}")]
    public class Target : ITarget
    {
        public Optimization     Optimization;
        public Platform         Platform;
        public BuildSystem      BuildSystem;
        public DevEnv           DevEnv;
        public OutputType       OutputType;
        public DotNetFramework  Framework;
        public Blob             Blob;
        public RunTimeLibrary   RunTimeLibrary;

        public Target() { }

        public Target(
            Platform platform,
            DevEnv devEnv,
            Optimization optimization,
            OutputType outputType = OutputType.Lib,
            DotNetFramework framework = DotNetFramework.net8_0,
            BuildSystem buildSystem = BuildSystem.MSBuild,
            Blob blob = Blob.NoBlob,
            RunTimeLibrary runTimeLibrary = RunTimeLibrary.Static
            )
        {
            Platform = platform;
            DevEnv = devEnv;
            Optimization = optimization;
            OutputType = outputType;
            Framework = framework;
            BuildSystem = buildSystem;
            Blob = blob;
            RunTimeLibrary = runTimeLibrary;
        }

        public static DevEnv GetDefaultDevEnv()
        {
            return DevEnv.vs2022;
        }

        public static Optimization GetDefaultOptimization()
        {
            return Optimization.Debug | Optimization.Release | Optimization.Dist;
        }

        public static Blob GetDefaultBlobSetting()
        {
            return Blob.NoBlob;
        }

        public static Blob GetFastBuildBlobSetting()
        {
            return Blob.FastBuildUnitys;
        }

        public static Target[] GetDefaultTargets(bool withFastBuild = false, Optimization? optimiziation = null)
        {
            List<Target> result = new List<Target>();

            result.AddRange(GetWin64Targets(withFastBuild, optimiziation));

            return result.ToArray();
        }

        public static Target[] GetWin64Targets(bool withFastBuild = false, Optimization? optimiziation = null)
        {

            List<Target> result = new List<Target>();

            Optimization optimizationMask = optimiziation.HasValue ? optimiziation.Value : GetDefaultOptimization();

            result.Add(
                new Target(
                    Platform.win64,
                    GetDefaultDevEnv(),
                    optimizationMask,
                    OutputType.Lib,
                    DotNetFramework.net8_0,
                    BuildSystem.MSBuild,
                    GetDefaultBlobSetting()
                )
            );

            /*
            if (Globals.CustomArguments.EnableFastBuild && withFastBuild)
            {
                result.Add(
                    new Target(
                        Platform.win64,
                        GetDefaultDevEnv(),
                        optimizationMask,
                        Usage.Common,
                        OutputType.Lib,
                        DotNetFramework.net8_0,
                        BuildSystem.FastBuild,
                        GetFastBuildBlobSetting()
                    )
                );
            }
            */
    
            return result.ToArray();
        }

        public override string Name
        {
            get
            {
                string name = Optimization.ToString();
                if (BuildSystem == BuildSystem.FastBuild)
                {
                    name += "_" + BuildSystem.FastBuild;
                    if (Blob == Blob.NoBlob)
                        name += "_NoBlob";
                }

                return name;
            }
        }

        public string NameForSolution
        {
            get
            {
                string name = Optimization.ToString();
                if (Blob == Blob.Blob)
                    name += "_" + Blob;

                if (BuildSystem == BuildSystem.FastBuild)
                    name += "_" + BuildSystem.FastBuild;

                return name;
            }
        }

        public string ShortSuffix
        {
            get
            {
                string shortString;
                switch (Optimization)
                {
                    case Optimization.Debug: shortString = "Debug"; break;
                    case Optimization.Release: shortString = "Release"; break;
                    case Optimization.Dist: shortString = "Dist"; break;
                    default: throw new Error("Invalid optimization value, Danski noobski");
                }

                if (BuildSystem == BuildSystem.FastBuild)
                    shortString += "x";

                return shortString;
            }
        }

        public string TargetSuffix
        {
            get
            {
                StringBuilder result = new StringBuilder();
                result.Append("_" + Platform);

                result.Append("_" + ShortSuffix);

                return result.ToString().ToLower();
            }
        }
    }
}
