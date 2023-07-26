using Dalamud.Interface.Components;
using DSREyeLocator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator.Gui
{
    internal static class TabFlames
    {
        internal static void Draw()
        {
            //ImGuiEx.TextWrapped(ImGuiColors.DalamudOrange, "This module had very little testing. It may not work or have problems. ");
            ImGui.Checkbox("启用模块", ref P.config.WrothFlames);
            if (!P.config.WrothFlames) return;
            ImGui.Checkbox("运作模式", ref P.config.WrothFlamesOperational);
            ImGuiEx.Text("     如果未选中，将在聊天中打印将要执行的宏命令");
            if (P.config.WrothFlamesOperational)
            {
                ImGuiEx.Text("测试:");
                ImGui.SameLine();
                if (ImGui.Button("attack marker"))
                {
                    P.chat.SendMessage($"/marking {FlamesResolver.GetLocalizedAttack()}1 <me>");
                }
                ImGui.SameLine();
                if (ImGui.Button("bind marker"))
                {
                    P.chat.SendMessage($"/marking {FlamesResolver.GetLocalizedBind()}1 <me>");
                }
                ImGui.SameLine();
                if (ImGui.Button("ignore marker"))
                {
                    P.chat.SendMessage($"/marking {FlamesResolver.GetLocalizedIgnore()}1 <me>");
                }
            }
            ImGui.Checkbox("在标记之间添加随机延迟（推荐）", ref P.config.FlamesEmulateDelay);
            /*if (P.config.FlamesEmulateDelay)
            {
                ImGui.SameLine();
                if (ImGui.Button("Test"))
                {
                    P.config.Test = true;
                    FlamesResolver.ChatCommands.Enqueue("/marking attack1 <1>");
                    FlamesResolver.ChatCommands.Enqueue("/marking attack2 <2>");
                    FlamesResolver.ChatCommands.Enqueue("/marking attack3 <3>");
                    FlamesResolver.ChatCommands.Enqueue("/marking attack4 <4>");
                    FlamesResolver.ChatCommands.Enqueue("/marking bind1 <5>");
                    FlamesResolver.ChatCommands.Enqueue("/marking bind2 <6>");
                    FlamesResolver.ChatCommands.Enqueue("/marking ignore1 <7>");
                    FlamesResolver.ChatCommands.Enqueue("/marking ignore2 <8>");
                }
                ImGuiComponents.HelpMarker("Will try to place markers on party members with delay. You need to be in combat.");
            }*/
            ImGui.Checkbox("仅自己", ref P.config.FlamesOnlySelf);
            if (P.config.FlamesOnlySelf)
            {
                ImGuiEx.TextWrapped(ImGuiColors.DalamudRed, "标记将仅适用于您。如果您想使用插件作为整个团队的自动标记，请取消选中“仅自己”。");
                ImGui.InputText("分散命令", ref P.config.FlamesSelfSpread, 100);
                ImGui.InputText("分摊命令", ref P.config.FlamesSelfStack, 100);
                ImGui.InputText("无buff命令", ref P.config.FlamesSelfNone, 100);
            }
            else
            {
                ImGui.Checkbox("分摊标记", ref P.config.MarkStacks);
                ImGui.Checkbox("分散标记", ref P.config.MarkSpreads);
                ImGui.Checkbox("无buff标记", ref P.config.MarkNones);
                ImGui.Checkbox("使用自定义命令", ref P.config.UseCustomCommands);
                if (P.config.UseCustomCommands)
                {
                    ImGuiEx.Text("分散玩家的命令：");
                    if (P.config.CustomCommandsSpread.Count(x => x == '\n') < 3)
                    {
                        ImGui.SameLine();
                        ImGuiEx.Text(Environment.TickCount % 1000 > 500 ? ImGuiColors.DalamudRed : ImGuiColors.DalamudYellow, "Must have at least 4 commands");
                    }
                    ImGui.InputTextMultiline("##分散", ref P.config.CustomCommandsSpread, 1000, new(ImGui.GetContentRegionAvail().X, 100));

                    ImGuiEx.Text("分摊玩家的命令:");
                    if (P.config.CustomCommandsStack.Count(x => x == '\n') < 1)
                    {
                        ImGui.SameLine();
                        ImGuiEx.Text(Environment.TickCount % 1000 > 500 ? ImGuiColors.DalamudRed : ImGuiColors.DalamudYellow, "Must have at least 4 commands");
                    }
                    ImGui.InputTextMultiline("##分摊", ref P.config.CustomCommandsStack, 1000, new(ImGui.GetContentRegionAvail().X, 100));

                    ImGuiEx.Text("无点名玩家的命令：");
                    if (P.config.CustomCommandsNone.Count(x => x == '\n') < 1)
                    {
                        ImGui.SameLine();
                        ImGuiEx.Text(Environment.TickCount % 1000 > 500 ? ImGuiColors.DalamudRed : ImGuiColors.DalamudYellow, "Must have at least 4 commands");
                    }
                    ImGui.InputTextMultiline("##无点名", ref P.config.CustomCommandsNone, 1000, new(ImGui.GetContentRegionAvail().X, 100));
                }
            }
            if(ImGui.Button("清除标记"))
            {
                ClearMarkers();
            }
        }
    }
}
