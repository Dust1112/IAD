using System.Diagnostics;
using IADEditor.Common;
using IADEditor.Content.Enums;

namespace IADEditor.Content;

public abstract class Asset : ViewModelBase
{
    public AssetType Type { get; private set; }

    public Asset(AssetType type)
    {
        Debug.Assert(type != AssetType.Unknown);
        Type = type;
    }
}