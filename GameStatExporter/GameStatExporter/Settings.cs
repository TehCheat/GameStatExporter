using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace GameStatExporter;

public class Settings : ISettings
{
    public Settings()
    {
        Enable = new ToggleNode(false);
    }

    public ToggleNode Enable { get; set; }
    public ToggleNode AddCommentDescriptions { get; set; } = new ToggleNode(false);
    public ToggleNode AddAttributeDescriptions { get; set; } = new ToggleNode(false);
    public ToggleNode LeaveEmptyLineBetweenStats { get; set; } = new ToggleNode(false);
    public ToggleNode UseSpacesInsteadOfTabs { get; set; } = new ToggleNode(true);
    public ToggleNode ForceLFNewline { get; set; } = new ToggleNode(true);

    public TextNode SavePath { get; set; } = new TextNode("");

    [Menu("Export!")]
    public ButtonNode ExportButtonNode { get; set; } = new ButtonNode();
}