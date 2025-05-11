using Cosmos.System.Graphics.Fonts;
using System.Drawing;

namespace HontelOS.System.Graphics
{
    public abstract class Style
    {
        public StyleType StyleType;

        #region General
        public Color PrimaryColor;

        public Font SystemFont;
        public Color DefaultTextColor;
        #endregion

        #region System
        public Color TaskBar_BackgroundColor;
        public Color TaskBar_BorderColor;
        public Color TaskBar_IconHoverColor;

        public Color TopBar_BackgroundColor;

        public Color StartMenu_BackgroundColor;
        public Color StartMenu_ContentColor;
        #endregion

        #region GUI
        public Color Window_BackgroundColor;
        public Color Window_HandleColor;
        public Color Window_HandleButtonGlowColor;
        public Color Window_HandleTextColor;

        public Color Button_NormalColor;
        public Color Button_HoverColor;
        public Color Button_PressedColor;
        public Color Button_DisabledColor;
        public Color Button_TextColor;

        public Color Label_TextColor;

        public Color TextBox_NormalColor;
        public Color TextBox_HoverColor;
        public Color TextBox_DisabledColor;
        public Color TextBox_PlaceholderTextColor;
        public Color TextBox_TextColor;

        public Color ContextMenu_BackgroundColor;
        public Color ContextMenu_HoverColor;
        public Color ContextMenu_TextColor;

        public Color ToolTip_Color;
        public Color ToolTip_TextColor;

        public Color ItemsList_BackgroundColor;
        public Color ItemsList_HoverColor;
        public Color ItemsList_SelectedColor;
        public Color ItemsList_SelectedTextColor;
        public Color ItemsList_TextColor;

        public Color WindowNavBar_BackgroundColor;
        public Color WindowNavBar_TextColor;
        public Color WindowNavBar_HoverColor;
        public Color WindowNavBar_SelectedTextColor;
        public Color WindowNavBar_SelectedColor;
        #endregion
    }

    public class LightStyle : Style
    {
        public LightStyle()
        {
            StyleType = StyleType.Light;

            PrimaryColor = Color.FromArgb(60, 60, 255);

            SystemFont = PCScreenFont.Default;
            DefaultTextColor = Color.Black;

            TaskBar_BackgroundColor = Color.White;
            TaskBar_BorderColor = Color.FromArgb(194, 194, 194);
            TaskBar_IconHoverColor = Color.FromArgb(230, 230, 230);

            TopBar_BackgroundColor = Color.FromArgb(194, 194, 194);

            StartMenu_BackgroundColor = Color.White;
            StartMenu_ContentColor = Color.FromArgb(240, 240, 240);

            Window_BackgroundColor = Color.White;
            Window_HandleColor = Color.FromArgb(243, 243, 243);
            Window_HandleButtonGlowColor = Color.LightGray;
            Window_HandleTextColor = Color.Black;

            Button_NormalColor = Color.FromArgb(0, 122, 255);
            Button_HoverColor = Color.FromArgb(0, 150, 255);
            Button_PressedColor = Color.FromArgb(0, 100, 255);
            Button_DisabledColor = Color.Gray;
            Button_TextColor = Color.White;

            Label_TextColor = Color.Black;

            TextBox_NormalColor = Color.LightGray;
            TextBox_HoverColor = Color.FromArgb(230, 230, 230);
            TextBox_DisabledColor = Color.Gray;
            TextBox_PlaceholderTextColor = Color.Gray;
            TextBox_TextColor = Color.Black;

            ContextMenu_BackgroundColor = Color.FromArgb(240, 240, 240);
            ContextMenu_HoverColor = Color.FromArgb(180, 180, 180);
            ContextMenu_TextColor = Color.Black;

            ToolTip_Color = Color.White;
            ToolTip_TextColor = Color.Black;

            ItemsList_BackgroundColor = Color.LightGray;
            ItemsList_HoverColor = Color.FromArgb(230, 230, 230);
            ItemsList_SelectedColor = Color.FromArgb(0, 122, 255);
            ItemsList_SelectedTextColor = Color.White;
            ItemsList_TextColor = Color.Black;

            WindowNavBar_BackgroundColor = Color.LightGray;
            WindowNavBar_TextColor = Color.Black;
            WindowNavBar_HoverColor = Color.FromArgb(230, 230, 230);
            WindowNavBar_SelectedColor = Color.FromArgb(0, 122, 255);
            WindowNavBar_SelectedTextColor = Color.White;
        }
    }

    public class DarkStyle : Style
    {
        public DarkStyle()
        {
            StyleType = StyleType.Dark;

            PrimaryColor = Color.FromArgb(60, 60, 255);

            SystemFont = PCScreenFont.Default;
            DefaultTextColor = Color.White;

            TaskBar_BackgroundColor = Color.FromArgb(80, 80, 80);
            TaskBar_BorderColor = Color.FromArgb(45, 45, 45);
            TaskBar_IconHoverColor = Color.FromArgb(100, 100, 100);

            TopBar_BackgroundColor = Color.FromArgb(80, 80, 80);

            StartMenu_BackgroundColor = Color.FromArgb(80, 80, 80);
            StartMenu_ContentColor = Color.FromArgb(60, 60, 60);

            Window_BackgroundColor = Color.FromArgb(20, 20, 20);
            Window_HandleColor = Color.FromArgb(45, 45, 45);
            Window_HandleButtonGlowColor = Color.FromArgb(80, 80, 80);
            Window_HandleTextColor = Color.White;

            Button_NormalColor = Color.FromArgb(60, 60, 60);
            Button_HoverColor = Color.FromArgb(80, 80, 80);
            Button_PressedColor = Color.FromArgb(40, 40, 40);
            Button_DisabledColor = Color.FromArgb(80, 80, 80);
            Button_TextColor = Color.White;

            Label_TextColor = Color.White;

            TextBox_NormalColor = Color.FromArgb(60, 60, 60);
            TextBox_HoverColor = Color.FromArgb(80, 80, 80);
            TextBox_DisabledColor = Color.FromArgb(80, 80, 80);
            TextBox_PlaceholderTextColor = Color.Gray;
            TextBox_TextColor = Color.White;

            ContextMenu_BackgroundColor = Color.FromArgb(30, 30, 30);
            ContextMenu_HoverColor = Color.FromArgb(50, 50, 50);
            ContextMenu_TextColor = Color.White;

            ToolTip_Color = Color.FromArgb(50, 50, 50);
            ToolTip_TextColor = Color.White;

            ItemsList_BackgroundColor = Color.FromArgb(60, 60, 60);
            ItemsList_HoverColor = Color.FromArgb(80, 80, 80);
            ItemsList_SelectedColor = Color.FromArgb(60, 60, 255);
            ItemsList_TextColor = Color.White;
            ItemsList_SelectedTextColor = Color.White;

            WindowNavBar_BackgroundColor = Color.FromArgb(60, 60, 60);
            WindowNavBar_TextColor = Color.White;
            WindowNavBar_HoverColor = Color.FromArgb(80, 80, 80);
            WindowNavBar_SelectedColor = Color.FromArgb(60, 60, 255);
            WindowNavBar_SelectedTextColor = Color.White;
        }
    }

    public enum StyleType
    {
        Light,
        Dark
    }
}
