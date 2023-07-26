using Dalamud.Game.ClientState.Objects.Types;
using ECommons.MathHelpers;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator.Gui
{
    internal unsafe static class TabMainConfig
    {
        internal static bool OpcodeFound = false;
        internal static void Draw()
        {
            ImGuiEx.Text("0x");
            ImGui.SameLine();
            ImGui.SetNextItemWidth(50f);
            ImGuiEx.InputHex("OP码", ref P.config.MapEventOpcode);

            ImGui.Separator();

            ImGui.Checkbox("测试", ref P.config.Test); 
            ImGui.Separator();
            if (Svc.ClientState.TerritoryType == 838)
            {
                if (OpcodeFound)
                {
                    ImGuiEx.Text(Environment.TickCount % 400 > 200 ? ImGuiColors.ParsedGreen : Vector4.Zero, "找到OP码并记录d!");
                }
                else
                {
                    ImGuiEx.Text(ImGuiColors.DalamudOrange, "向前走，直到流星坠落地面");
                }
            }
            else
            {
                ImGuiEx.Text(ImGuiColors.DalamudYellow, "进入亚马乌罗提获取OP码");
            }
            ImGui.Separator();
            ImGuiEx.Text("调试:");
            ImGuiEx.Text($"二运: {SanctityStartTime}/{IsSanctity()}");
            ImGuiEx.Text($"死刻: {DeathStartTime}/{IsDeath()}");
            if (Svc.Targets.Target != null)
            {
                var angle = ConeHandler.GetAngleTo(Svc.Targets.Target.Position.ToVector2());
                ImGuiEx.Text(ConeHandler.IsInCone(Svc.Targets.Target.Position.ToVector2()) ? ImGuiColors.DalamudRed : ImGuiColors.DalamudWhite, $"{angle}");
                if (Svc.Targets.Target is Character c)
                {
                    ImGuiEx.Text($"{c.NameId}");
                }
            }
            if (DalamudReflector.TryGetDalamudStartInfo(out var info))
            {
                ImGuiEx.TextCopy($"{info.GameVersion.ToString()}");
            }
        }
    }
}
