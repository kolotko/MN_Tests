using System.Runtime.CompilerServices;

namespace Snapshot.Tests;

public class VerifyGlobalSettings
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifyHttp.Initialize();
        Recording.Start();
    }
}