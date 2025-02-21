using MudBlazor;

namespace SalonSync.Web
{
    public static class WebConfiguration
    {
        public const string HttpClientName = "salonsyncapi";

        public static string BackendUrl { get; set; } = string.Empty;

        public static readonly MudTheme Theme = new()
        {
            Typography = new Typography
            {
                Default = new DefaultTypography()
                {
                    FontFamily = ["Raleway", "sans-serif"]
                }
            },
            PaletteLight = new PaletteLight
            {
                Primary = "#5EAEE1",
                PrimaryContrastText = new MudBlazor.Utilities.MudColor("#000000"),
                Secondary = Colors.LightBlue.Darken3,
                Background = Colors.Gray.Lighten4,
                AppbarBackground = "#0B5A8D",
                AppbarText = Colors.Shades.White,
                TextPrimary = Colors.Shades.Black,
                DrawerText = Colors.Shades.White,
                DrawerBackground = "#0B5A8D",
            },
            PaletteDark = new PaletteDark()
            {
                Primary = Colors.LightBlue.Accent3,
                AppbarBackground = "#073b5d",
                DrawerBackground = "#073b5d",
                AppbarText = Colors.Shades.White,
                Secondary = Colors.LightBlue.Darken3,
                PrimaryContrastText = new MudBlazor.Utilities.MudColor("#000000"),
            }

        };
    }
}
