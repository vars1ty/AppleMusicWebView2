namespace Apple_Music
{
    /// <summary>
    /// Commonly used strings.
    /// </summary>
    internal struct Commons
    {
        public const string main_view_darker =
                "document.getElementById(\"web-main\").style.background = 'rgb(17, 17, 17)';",
            nav_container_darker =
                "document.getElementById(\"web-navigation-container\").style.background = 'rgb(17, 17, 17)';",
            nav_top_chrome_darker =
                "document.getElementsByClassName(\"web-chrome\")[0].style.background = 'rgb(17, 17, 17)';",
            nav_playback_darker =
                "document.getElementsByClassName(\"web-chrome-playback-lcd\")[0].style.background = 'rgb(17, 17, 17)';",
            global_primary_color = "document.querySelector(\":root\").style.setProperty('--primaryColor', '#00ffad');";
    }
}