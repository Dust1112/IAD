using IADEditor.Content;

namespace IADEditor.Editors;

public interface IAssetEditor
{
    Asset Asset { get; }

    void SetAsset(Asset asset);
}