using System;
using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.CoreModels;
using SubSonicMedia.Interfaces;

namespace StrixMusic.Cores.OpenSubsonic.Models;

public abstract class OpenSubsonicCoreModelBase : ICoreModel
{
    private readonly OpenSubsonicCore _sourceCore;
    
    protected OpenSubsonicCoreModelBase(ICore sourceCore)
    {
        Guard.IsOfType<OpenSubsonicCore>(sourceCore);
        if (sourceCore is not OpenSubsonicCore openSubsonicCore)
            throw new ArgumentException($"Source core must be an {nameof(OpenSubsonicCore)}, got {sourceCore.GetType().Name}");

        _sourceCore = openSubsonicCore;
    }

    protected ISubsonicClient Client => _sourceCore.Client;

    public ICore SourceCore => _sourceCore;
}
