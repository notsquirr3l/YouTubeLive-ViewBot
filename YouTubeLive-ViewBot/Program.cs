using System;
using System.IO;
using System.Threading;

using Leaf.xNet;

namespace YouTubeLive_ViewBot {
    internal class Program {
        internal const string Title = "YouTube livestream bot";

        internal static char RandomChar(Random rand) {
            return _charset[rand.Next(_charset.Length)];
        }

        internal static string RandomString(int length = 16) {
            var result = string.Empty;
            var rand = new Random(DateTime.Now.Millisecond);

            for (var i = 0; i < length; i++) {
                result += RandomChar(rand);
            }

            return result;
        }

        internal static void OnViewsChanged() {
            Console.Title = $"{Title} | Views Sent: {_views.ToString()}";
        }

        [STAThread]
        internal static void Main(string[] args) {
            Console.Title = string.Format($"{_title} | Views Sent: {0}", _views);

            Console.Write("Video-ID: ");
            _videoID = Console.ReadLine();

            Console.Write("[1] Http Proxies\n[2] Socks4 Proxies\n[3] Socks5 Proxies\nInput: ");
            _proxyType = Convert.ToInt32(Console.ReadLine());

            Console.Write("Proxies: ");
            _proxies = File.ReadAllLines(Console.ReadLine());

            Console.Write("Threads: ");

            int num = Convert.ToInt32(Console.ReadLine());

            ThreadPool.SetMinThreads(num, num);
            try {
                while (true) {
                    Thread.Sleep(200); // wait 200ms inbetween threads to avoid system deadlock!

                    new Thread(ExecuteBot).Start();
                }
            }
            catch {
                Console.WriteLine("An error has occured!");
            }
        }

        public static void ExecuteBot() {
            var random1 = new Random();
            var random2 = new Random(DateTime.Now.Millisecond);

            while (true) {
                try {
                    int random_integer1 = random2.Next(1000, 10000) + random2.Next(1, 100) / 1000;
                    int random_integer2 = random2.Next(1000, 2000) + random2.Next(1, 10) / 1000;
                    int random_integer3 = random2.Next(1000, 10000);

                    var httpRequest = new HttpRequest();

                    httpRequest.Cookies = new CookieStorage(false, null);
                    httpRequest.KeepAlive = true;
                    httpRequest.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1 Mobile/15E148 Safari/604.1";

                    if (_proxies.Length != 0) {
                        var proxystr = _proxies[random1.Next(_proxies.Length)];

                        if (_proxyType == 1)
                            httpRequest.Proxy = HttpProxyClient.Parse(_proxies[random1.Next(_proxies.Length)]);

                        if (_proxyType == 2)
                            httpRequest.Proxy = Socks4ProxyClient.Parse(_proxies[random1.Next(_proxies.Length)]);

                        if (_proxyType == 3)
                            httpRequest.Proxy = Socks5ProxyClient.Parse(_proxies[random1.Next(_proxies.Length)]);
                    }
                    else {
                        httpRequest.Proxy = null;
                    }

                    httpRequest.AddHeader(HttpHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                    httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");

                    httpRequest.ConnectTimeout = 10000;

                    Console.WriteLine("Get -> Link");

                    string data = httpRequest.Get($"https://m.youtube.com/watch?v={_videoID}", null).ToString();

                    if (!data.Contains("plid")) {
                        Console.WriteLine("Parameter Error ->");
                        return;
                    }

                    Console.WriteLine("No Error -> Continue -> Viewbot");

                    string ei = data.Substring("ei=", "\\", 0, StringComparison.Ordinal, null);
                    string cver = data.Substring("INNERTUBE_CONTEXT_CLIENT_VERSION\":\"", "\"", 0, StringComparison.Ordinal, null);
                    string cl = data.Substring("PAGE_CL\":", ",", 0, StringComparison.Ordinal, null);
                    string c = data.Substring("c\":\"", "\"", 0, StringComparison.Ordinal, null);
                    string hl = data.Substring("hl\":\"", "\"", 0, StringComparison.Ordinal, null);
                    string cr = data.Substring("cr\":\"", "\"", 0, StringComparison.Ordinal, null);
                    string of = data.Substring("of=", "\\", 0, StringComparison.Ordinal, null);
                    string vm = data.Substring("vm=", "\\", 0, StringComparison.Ordinal, null);
                    string idpj = data.Substring("idpj\":\"", "\"", 0, StringComparison.Ordinal, null);
                    string ldpj = data.Substring("ldpj\":\"", "\"", 0, StringComparison.Ordinal, null);

                    httpRequest.AddHeader(HttpHeader.Accept, "image/png,image/svg+xml,image/*;q=0.8,video/*;q=0.8,*/*;q=0.5");
                    httpRequest.AddHeader(HttpHeader.AcceptLanguage, "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                    httpRequest.AddHeader(HttpHeader.Referer, $"https://m.youtube.com/watch?v={_videoID}");

                    httpRequest.Get(string.Concat(new object[] {
                            "https://s.youtube.com/api/stats/watchtime?ns=yt&el=detailpage&cpn=",
                            RandomString(16),
                            "&docid=",
                            _videoID,
                            "&ver=2&cmt=",
                            random_integer1.ToString().Replace(",", "."),
                            "&ei=",
                            ei,
                            "&fmt=133&fs=0&rt=",
                            random_integer2.ToString().Replace(",", "."),
                            "&of=",
                            of,
                            "&euri&lact=",
                            random_integer3,
                            "&live=dvr&cl=",
                            cl,
                            "&state=playing&vm=",
                            vm,
                            "&volume=100&c=",
                            c,
                            "&cver=",
                            cver,
                            "&cplayer=UNIPLAYER&cbrand=apple&cbr=Safari%20Mobile&cbrver=12.1.15E148&cmodel=iphone&cos=iPhone&cosver=12_2&cplatform=MOBILE&delay=5&hl=",
                            hl,
                            "&cr=",
                            cr,
                            "&rtn=",
                            random_integer2 + 300,
                            "&afmt=140&lio=1556394045.182&idpj=",
                            idpj,
                            "&ldpj=",
                            ldpj,
                            "&rti=",
                            random_integer2,
                            "&muted=0&st=",
                            random_integer1.ToString().Replace(",", "."),
                            "&et=",
                            (random_integer1 + 300.0).ToString().Replace(",", ".")
                    }), null).ToString();

                    Console.WriteLine("Thread -> Action -> Sleep");
                    Console.WriteLine("View Sent");

                    Interlocked.Increment(ref _views);
                    OnViewsChanged();

                    Thread.Sleep(20000);
                }
                catch (Exception ex) {
                    Console.WriteLine("Error -> Retry");
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        public static string _videoID;
        public static int _proxyType;
        public static string[] _proxies;

        internal const string _title = "YouTube livestream bot";
        internal const string _charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_";
        private static int _views;
    }
}