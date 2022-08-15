using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ExileCore;

namespace GameStatExporter;

public class Core : BaseSettingsPlugin<Settings>
{
    private static readonly char[] TrimStartChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

    public Core()
    {
        Name = "GameStatExporter";
    }

    public override bool Initialise()
    {
        base.Initialise();

        Settings.ExportButtonNode.OnPressed += Export;
        return true;
    }

    public override void OnPluginDestroyForHotReload()
    {
        Settings.ExportButtonNode.OnPressed -= Export;
    }

    private void Export()
    {
        if (string.IsNullOrEmpty(Settings.SavePath.Value))
        {
            LogError("Save path is not set", 5);

            return;
        }

        if (!Directory.Exists(Path.GetDirectoryName(Settings.SavePath.Value)))
        {
            LogError("Incorrect export path (No such directory)", 5);

            return;
        }

        var sb = new StringBuilder();

        if (Settings.AddAttributeDescriptions)
        {
            sb.AppendLine("using System.ComponentModel;");
            sb.AppendLine();
        }

        sb.AppendLine("namespace ExileCore.Shared.Enums;");
        sb.AppendLine();
        sb.AppendLine("public enum GameStat");
        sb.AppendLine("{");

        var duplicatesDict = new Dictionary<string, int>();

        foreach (var statsRecord in GameController.Files.Stats.records)
        {
            var descr = string.IsNullOrEmpty(statsRecord.Value.UserFriendlyName)
                ? statsRecord.Value.Key
                : statsRecord.Value.UserFriendlyName;

            if (Settings.AddCommentDescriptions)
            {
                sb.AppendLine("\t/// <summary>");
                sb.AppendLine($"\t/// {descr}");
                sb.AppendLine("\t/// </summary>");
            }

            if (Settings.AddAttributeDescriptions)
            {
                sb.AppendLine($"\t[Description(\"{descr}\")]");
            }

            var formattedName = FormatName(statsRecord.Key);

            if (duplicatesDict.TryGetValue(formattedName, out var duplicatesCount))
            {
                duplicatesCount++;
                formattedName += duplicatesCount;
                duplicatesDict[formattedName] = duplicatesCount;
            }
            else
            {
                duplicatesDict.Add(formattedName, 1);
            }

            sb.AppendLine($"\t{formattedName} = {statsRecord.Value.ID},");
            if (Settings.LeaveEmptyLineBetweenStats)
            {
                sb.AppendLine();
            }
        }

        sb.AppendLine("}");
        if (Settings.UseSpacesInsteadOfTabs)
        {
            sb.Replace("\t", new string(' ', 4));
        }

        if (Settings.ForceLFNewline)
        {
            sb.Replace("\r", null);
        }

        File.WriteAllText(Settings.SavePath.Value, sb.ToString());
    }

    private static string FormatName(string name)
    {
        var renamed = name.Replace("%", "pct").Replace("+", "").Replace("-", "").TrimStart(TrimStartChars);
        var info = CultureInfo.InvariantCulture.TextInfo;
        renamed = info.ToTitleCase(renamed).Replace("_", string.Empty);

        return renamed;
    }
}