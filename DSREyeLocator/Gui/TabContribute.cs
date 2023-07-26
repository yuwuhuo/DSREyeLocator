using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSREyeLocator.Gui
{
    internal static class TabContribute
    {
        internal static void Draw()
        {
            ImGuiEx.TextWrapped("如果您发现该项目有用并希望做出贡献，您可以将一些硬币发送到以下任何一个加密钱包：");
            Donation.PrintDonationInfo();
        }
    }
}
