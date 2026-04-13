using Android.App;
using Android.OS;
using Android.Views;
using Android.Webkit;

namespace StalkerPDA.UI.Fragments
{
    public class MapFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var webView = new WebView(Activity);
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.Settings.SetSupportZoom(true);
            webView.Settings.BuiltInZoomControls = true;
            webView.Settings.DisplayZoomControls = false;

            webView.SetBackgroundColor(Android.Graphics.Color.ParseColor("#1A2A3A"));
            webView.SetWebViewClient(new TacticalMapClient());

            webView.LoadUrl("https://map.stalker.wiki/");

            return webView;
        }

        private class TacticalMapClient : WebViewClient
        {
            public override void OnPageFinished(WebView view, string url)
            {
                base.OnPageFinished(view, url);

                string jsInjection = @"
                    javascript:(function() {
                        var style = document.createElement('style');
                        style.innerHTML = `
                            header, footer, nav, aside, .sidebar, .menu, 
                            .leaflet-control-container, 
                            .left_aside, .status_aside, .other_status, .filters_aside,
                            .header-container, .footer-container, 
                            #top-bar, #bottom-bar { 
                                display: none !important; 
                            }
                            
                            .leaflet-pane, .leaflet-tile-pane img {
                                filter: grayscale(100%) sepia(100%) hue-rotate(5deg) saturate(300%) brightness(60%) contrast(150%) !important;
                            }
                            body, html, .leaflet-container {
                                background-color: #14120E !important;
                            }
                        `;
                        document.head.appendChild(style);
                        setInterval(function() {
                            var elements = document.querySelectorAll('div, span, p, a');
                            for(var i=0; i<elements.length; i++) {
                                var text = elements[i].innerText;
                                if(text && (text.includes('СТВОРЕНО В УКРАЇНІ') || text.includes('©') || text.includes('WWW.STALKER.WIKI'))) {
                                    elements[i].style.display = 'none';
                                }
                            }
                        }, 500);
                    })();
                ";

                view.EvaluateJavascript(jsInjection, null);
            }
        }
    }
}