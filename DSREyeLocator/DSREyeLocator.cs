using Dalamud.Game;
using Dalamud.Game.ClientState.Objects.SubKinds;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.Network;
using Dalamud.Logging;
using Dalamud.Plugin;
using DSREyeLocator.Core;
using DSREyeLocator.Gui;
using ECommons.Automation;
using ECommons.GameFunctions;
using ECommons.Hooks;
using ECommons.MathHelpers;
using ECommons.Opcodes;
using ECommons.Reflection;
using Newtonsoft.Json;
using System;

namespace DSREyeLocator
{
    public unsafe class DSREyeLocator : IDalamudPlugin
    {
        public string Name => "绝龙诗工具箱";
        internal const uint DSRTerritory = 968;
        internal static DSREyeLocator P { get; private set; }
        internal WindowSystem ws;
        internal ConfigWindow configWindow;
        internal OverlayWindow overlayWindow;
        internal Config config;
        internal Chat chat;

        public DSREyeLocator(DalamudPluginInterface pi)
        {
            P = this;
            ECommonsMain.Init(pi, this, Module.ObjectFunctions, Module.DalamudReflector);
            new TickScheduler(delegate
            {
                config = Svc.PluginInterface.GetPluginConfig() as Config ?? new();
                ws = new();
                configWindow = new();
                ws.AddWindow(configWindow);
                overlayWindow = new();
                ws.AddWindow(overlayWindow);
                //Svc.GameNetwork.NetworkMessage += OnNetworkMessage;
                Svc.Framework.Update += Tick;
                Svc.PluginInterface.UiBuilder.Draw += ws.Draw;
                Svc.PluginInterface.UiBuilder.OpenConfigUi += delegate { configWindow.IsOpen = true; };
                Svc.Condition.ConditionChange += Condition_ConditionChange;

                if (DalamudReflector.TryGetDalamudStartInfo(out var info))
                {
                    OpcodeUpdater.DownloadOpcodes($"https://github.com/NightmareXIV/MyDalamudPlugins/raw/main/opcodes/{info.GameVersion}.txt",
                        (dic) =>
                        {
                            if (dic.TryGetValue("地图效果", out var code))
                            {
                                config.MapEventOpcode = code;
                                PluginLog.Information($"下载的地图效果OP码 0x{code:X}");
                            }
                        });
                }

                Headmarker.Init();
                new ChangelogWindow(config, 1, delegate
                {
                    ImGuiEx.Text("DSR Eye Locator 已重命名为 DSR Toolbox，并且包含一些 \n" +
                        "其他绝龙诗功能. " +
                        "\n\n当我在战斗中取得进展时，我可能会在未来添加其他一些内容。" +
                        "\n默认情况下，仅启用龙眼定位器，与插件之前的行为相匹配。  ");
                });
                Svc.ClientState.TerritoryChanged += TerrChanged;
                Svc.Commands.AddHandler("/eye", new(delegate { configWindow.IsOpen = true; }) { HelpMessage = "打开设置界面" });
                chat = new();
                MapEffect.Init((a1, a2, a3, a4) =>
                {
                    EyeResolver.ProcessMapEffect(a2, a4);
                });
            });
        }

        public void Dispose()
        {
            Svc.Commands.RemoveHandler("/eye");
            //Svc.GameNetwork.NetworkMessage -= OnNetworkMessage;
            Svc.Framework.Update -= Tick;
            Svc.PluginInterface.UiBuilder.Draw -= ws.Draw;
            Svc.Condition.ConditionChange -= Condition_ConditionChange;
            Safe(overlayWindow.Dispose);
            Safe(Headmarker.Dispose);
            Svc.ClientState.TerritoryChanged -= TerrChanged;
            ECommonsMain.Dispose();
        }

        private void TerrChanged(object sender, ushort e)
        {
            if (P.config.MapEffectDbg)
            {
                P.config.MapEffectLog.RemoveAll(x => x.structs.Count == 0);
                P.config.MapEffectLog.Add((e, new()));
            }
        }

        private void Tick(Framework framework)
        {
            if (Svc.ClientState.LocalPlayer == null || Svc.Condition[ConditionFlag.DutyRecorderPlayback]) return;
            if(Svc.ClientState.TerritoryType == DSRTerritory || P.config.Test)
            {
                Safe(delegate
                {
                    if(P.config.EyeEnabled) EyeTick();
                    if(P.config.ChainEnabled) ChainsResolver.ChainsTick();
                    if (P.config.WrothFlames) FlamesTick();
                });
            }
        }

        internal void Condition_ConditionChange(ConditionFlag flag, bool value)
        {
            if (flag == ConditionFlag.InCombat && Svc.ClientState.TerritoryType == DSRTerritory)
            {
                if (value)
                {
                    PluginLog.Debug("战斗开始");
                }
                else
                {
                    PluginLog.Debug("战斗结束");
                    Headmarker.HeadmarkerInfos.Clear();
                    if (P.config.WrothFlames)
                    {
                        if (FlamesResolved && ClearScheduler != null)
                        {
                            FlamesClearRequested = true;
                        }
                    }
                }
                FlamesResolved = false;
            }
        }
    }
}
