using Dalamud.Interface.Components;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator.Gui
{
    internal static class TabEyeConfig
    {
        internal static void Draw()
        {
            ImGui.Checkbox("启用模块", ref P.config.EyeEnabled);
            if (!P.config.EyeEnabled) return;
            if (ImGui.Checkbox("启用眼睛和托尔丹的连线（需要 Splatoon)", ref P.config.EnableTether))
            {
                if (P.config.EnableTether && !DalamudReflector.TryGetDalamudPlugin("Splatoon", out _))
                {
                    Notify.Error("您未安装Splatoon");
                    P.config.EnableTether = false;
                }
            }
            if (P.config.EnableTether)
            {
                ImGui.SetNextItemWidth(50f);
                ImGui.InputFloat("连线粗细", ref P.config.Thickness);
                P.config.Thickness.ValidateRange(0.1f, 50f);
                var col = P.config.Color.ToVector4();
                ImGui.ColorEdit4("连线颜色", ref col);
                P.config.Color = col.ToUint();
                Safe(delegate
                {
                    if (Svc.Targets.Target != null && P.config.Test) SplatoonManager.DrawLine(SplatoonManager.Get(), Svc.ClientState.LocalPlayer.Position,
                        Svc.Targets.Target.Position, P.config.Color, P.config.Thickness);
                });
            }
            ImGui.Checkbox("启用标题", ref P.config.EnableBanner);
            if (P.config.EnableBanner)
            {
                ImGui.SetNextItemWidth(50f);
                ImGui.DragInt("垂直偏移", ref P.config.VerticalOffset);
                ImGui.SetNextItemWidth(50f);
                ImGui.DragInt("水平偏移", ref P.config.HorizontalOffset);
                ImGui.SetNextItemWidth(50f);
                ImGui.DragFloat("范围", ref P.config.Scale, 0.002f, 0.1f, 10f);
                P.config.Scale.ValidateRange(0.1f, 10f);
            }
            ImGui.Checkbox("闪烁", ref P.config.BannerBlink);
            ImGuiEx.WithTextColor(ImGuiColors.DalamudOrange, delegate
            {
                ImGui.Checkbox("延迟显示信息（推荐）", ref P.config.Delay);
            });
            ImGuiComponents.HelpMarker("延迟显示连线和标题(二运出去时/死刻回来时)");
            if (P.config.Delay)
            {
                ImGuiEx.TextWrapped("您可以根据每个机制您需要多少时间来设定延迟:");
                if (ImGui.SmallButton("重置"))
                {
                    var c = new Config();
                    P.config.SanctityDelay = c.SanctityDelay;
                    P.config.DeathDelay = c.DeathDelay;
                }
                ImGui.SetNextItemWidth(50f);
                ImGui.DragInt("二运读条延迟, ms", ref P.config.SanctityDelay, 10, 0, 15000);
                ImGuiEx.Text("   - 二运读条时间是17731 ms");
                ImGui.SetNextItemWidth(50f);
                ImGui.DragInt("至天之阵：死刻延迟, ms", ref P.config.DeathDelay, 10, 0, 30000);
                ImGuiEx.Text("   - 至天之阵：死刻读条时间是 34255 ms");
            }
        }
    }
}
