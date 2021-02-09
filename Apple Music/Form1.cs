using System;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;

namespace Apple_Music
{
    public partial class Form1 : Form
    {
        #region Variables
        private readonly DiscordRichPresence rpc = new DiscordRichPresence();
        #endregion

        public Form1()
        {
            InitializeComponent();
            // Handle window resizing
            Resize += Form_Resize;
            HandleResize();

            // Disconnect from Discord RPC when window is closed
            FormClosing += Form_Closing;
            InitializeWebView();
        }

        // Window resizing
        private void Form_Resize(object sender, EventArgs e) => HandleResize();

        private void HandleResize() => webView.Size = ClientSize - new Size(webView.Location);

        // Do all the shit that has to be done after the webview initialization is done.
        private async void InitializeWebView()
        {
            // Initialize WebView
            await webView.EnsureCoreWebView2Async();
            // Change window title when document title changes
            webView.CoreWebView2.DocumentTitleChanged += AppleMusic_TitleChanged;
            // Remove extra shit from the website (like the Open in iTunes button) when page loads
            webView.CoreWebView2.SourceChanged += AppleMusic_RemoveExtraElements;
            webView.CoreWebView2.NavigationCompleted += AppleMusic_RemoveExtraElements;
            // Start getting data from now playing
            //webView.CoreWebView2.SourceChanged += AppleMusic_InitDiscordRPC;
            webView.CoreWebView2.NavigationCompleted += AppleMusic_InitDiscordRPC;
            // Receive messages from webapp
            webView.CoreWebView2.WebMessageReceived += AppleMusic_DiscordRPC;
        }

        private void AppleMusic_TitleChanged(object sender, object e) => Text = webView.CoreWebView2.DocumentTitle;

        // Credit for the JS code: https://github.com/iiFir3z/Apple-Music-Electron/
        private async void AppleMusic_RemoveExtraElements(object sender, object e)
        {
            await webView.CoreWebView2.ExecuteScriptAsync(
                "const elements = document.getElementsByClassName('web-navigation__native-upsell'); while (elements.length > 0) elements[0].remove();");
            await webView.CoreWebView2.ExecuteScriptAsync("while (elements.length > 0) elements[0].remove();");
        }

        // Credit for the JS code: https://github.com/iiFir3z/Apple-Music-Electron/
        private async void AppleMusic_InitDiscordRPC(object sender, object e)
        {
            // yeah...
            rpc.Initialize();
            // Get playing state
            await webView.CoreWebView2.ExecuteScriptAsync(
                "MusicKit.getInstance().addEventListener( MusicKit.Events.playbackStateDidChange, (a) => {" +
                "window.chrome.webview.postMessage({state: a.state});" +
                "});");
            // Get music data. I'm so sorry for making you see this
            await webView.CoreWebView2.ExecuteScriptAsync(
                "MusicKit.getInstance().addEventListener( MusicKit.Events.mediaItemStateDidChange, function() {" +
                "const nowPlayingItem =  MusicKit.getInstance().nowPlayingItem; let attributes  = {}; if (nowPlayingItem != null) { attributes = nowPlayingItem.attributes; }" +
                "attributes.name = attributes.name ? attributes.name : null;" +
                "attributes.durationInMillis = attributes.durationInMillis ? attributes.durationInMillis : 0;" +
                "attributes.albumName = attributes.albumName ? attributes.albumName : null;" +
                "attributes.artistName = attributes.artistName ? attributes.artistName : null;" +
                "window.chrome.webview.postMessage(attributes);" +
                "})");
        }

        private void AppleMusic_DiscordRPC(object sender, CoreWebView2WebMessageReceivedEventArgs args)
        {
            var json = args.WebMessageAsJson;
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            var data = JsonSerializer.Deserialize<MusicKitResponse>(json, options);
            rpc.UpdatePresence(data);
        }

        private void Form_Closing(object sender, EventArgs e) => rpc.EndConnection();
    }
}