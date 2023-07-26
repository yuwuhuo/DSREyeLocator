using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Interface.Components;
using ECommons.MathHelpers;
using ECommons.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator.Gui
{
    internal unsafe class ConfigWindow : Window
    {

        public ConfigWindow() : base($"{P.Name} 设置")
        {
            this.SizeConstraints = new()
            {
                MaximumSize = new(9999, 9999),
                MinimumSize = new(400, 200)
            };
        }

        public override void Draw()
        {
            KoFiButton.DrawRight();
            ImGuiEx.EzTabBar("绝龙眼主选项卡",
                ("模块", delegate
                {
                    ImGuiEx.EzTabBar("功能",
                        ("[P2/P5] 龙眼定位", TabEyeConfig.Draw, P.config.EyeEnabled ? ImGuiColors.ParsedGreen : null, true),
                        ("[P1/P5] 火链连线", TabChains.Draw, P.config.ChainEnabled ? ImGuiColors.ParsedGreen : null, true),
                        ("[P6] 火焰自动标记", TabFlames.Draw, P.config.WrothFlames ? ImGuiColors.ParsedGreen : null, true)
                        );
                }, ImGuiColors.DalamudOrange, true
                ),
                ("选项", TabMainConfig.Draw, null, true),
                ("调试", Debug.Draw, ImGuiColors.DalamudGrey3, true)
                );
        }

        public override void OnClose()
        {
            P.config.Test = false;
            Svc.PluginInterface.SavePluginConfig(P.config);
            Notify.Success("设置保存");
        }
    }
}
