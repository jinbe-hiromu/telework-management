using Microsoft.Maui.Handlers;

namespace WorkScheduler.Views.Controls;

public class WebViewWithCookie : WebView
{
    static WebViewWithCookie()
    {
        WebViewHandler.Mapper.AppendToMapping(nameof(IWebView.Source),
#if WINDOWS
			async
#endif
            (handler, view) =>
        {
            if (view is WebViewWithCookie webView)
            {
                var url = webView.Url!;
#if ANDROID
                var headers = new Dictionary<string, string>
                {
                    ["Cookie"] = webView.Cookie
                };
                handler.PlatformView.LoadUrl(url, headers);
#elif iOS
                var webRequest = new NSMutableUrlRequest(new NSUrl(url));
                var headerKey = new NSString("Cookie");
                var headerValue = new NSString(webView.Cookie);
                var dictionary = new NSDictionary(headerKey, headerValue);
                webRequest.Headers = dictionary;

                handler.PlatformView.LoadRequest(webRequest);
#elif WINDOWS
		        await handler.PlatformView.EnsureCoreWebView2Async();

			    var request = handler.PlatformView.CoreWebView2.Environment.CreateWebResourceRequest(
				    uri: url,
				    Method: "GET",
				    postData: Stream.Null.AsRandomAccessStream(),
				    Headers: null);

			    request.Headers.SetHeader("Cookie", webView.Cookie);
			    //request.Headers.SetHeader("Connection", "");
			    request.Headers.SetHeader("Accept-Language", "ja,en;q=0.9,en-GB;q=0.8,en-US;q=0.7");

			    handler.PlatformView.CoreWebView2.NavigateWithWebResourceRequest(request);
#else
                throw new NotImplementedException();
#endif
            }

        });
    }

    public static readonly BindableProperty CookieProperty = BindableProperty.Create(nameof(Cookie), typeof(string), typeof(WebViewWithCookie), "");

    public string Cookie
    {
        get => GetValue(CookieProperty) as string;
        set => SetValue(CookieProperty, value);
    }

    public static readonly BindableProperty UrlProperty = BindableProperty.Create(nameof(Url), typeof(string), typeof(WebViewWithCookie), "");
    public string Url
    {
        get => GetValue(UrlProperty) as string;
        set => SetValue(UrlProperty, value);
    }
}
