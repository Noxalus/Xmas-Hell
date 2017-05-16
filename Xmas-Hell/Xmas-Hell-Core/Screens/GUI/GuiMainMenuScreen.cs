using MonoGame.Extended;
using MonoGame.Extended.Gui;
using MonoGame.Extended.Gui.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmasHell.Screens.GUI
{
    public class GuiMainMenuScreen : GuiScreen
    {
        public GuiMainMenuScreen(GuiSkin skin) : base(skin)
        {
        }

        public override void Initialize()
        {
            var dialog = Skin.Create<GuiDialog>("dialog");
            var grid = new GuiUniformGrid { Columns = 3 };

            var items = new List<String>()
            {
                "ITEM1",
                "ITEM2",
                "ITEM3"
            };

            foreach (var item in items)
            {
                var button = Skin.Create<GuiButton>("white-button", c =>
                {
                    c.Text = item;
                    c.Margin = new Thickness(4);
                    c.Clicked += (sender, args) => Console.WriteLine("Clicked on " + item + "!");
                });
                grid.Controls.Add(button);
            }

            dialog.Controls.Add(grid);
            Controls.Add(dialog);
        }
    }
}
