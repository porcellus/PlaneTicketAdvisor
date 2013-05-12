using System;
using System.Globalization;
using System.Threading;
using Awesomium.Core;

namespace WebMiner
{
    public class BrowserSimulator: IDisposable
    {
        public static void Update()
        {
            BrowserThread.ExecuteAction(WebCore.Update);
            //WebCore.Update();
        }
        
        private WebView _view;
        public BrowserSimulator( string url = "")
        {
            BrowserThread.ExecuteAction( ()=>
                {
                    _view = WebCore.CreateWebView(1024, 768);
                    _view.DocumentReady += OnDocumentReady;
                    _view.DocumentReady += InitializeJavascript;
                    _view.AddressChanged += ViewAddressChanged;
                    _view.Source = new Uri(url);
                });
        }

        public Uri Source
        {
            get { return _view.Source; }
            set
            {
                BrowserThread.ExecuteAction(() =>
                    { _view.Source = value; });
            }
        }

        public bool IsDocumentReady
        {
            get
            {
                return (bool) BrowserThread.ExecuteFunction(() =>
                    {
                        WebCore.Update();
                        return _view.IsDocumentReady;
                    });
            }
        }

        public ISurface Surface
        {
            get { return _view.Surface; }
        }

        public event UrlEventHandler AddressChanged;
        public event WebViewEventHandler DocumentReady;

        private void ViewAddressChanged(object sender, UrlEventArgs ev)
        {
            if(AddressChanged != null)
                AddressChanged(this, ev);
        }

        private void OnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            if (DocumentReady != null)
                DocumentReady(this, urlEventArgs);
        }

        public void MoveMouse(int x, int y)
        {
            BrowserThread.ExecuteAction(() => 
                _view.InjectMouseMove(x, y)
            );
        }

        public void DoClick(bool isLeft=true)
        {
            BrowserThread.ExecuteAction(() =>
                {
                    _view.InjectMouseDown(isLeft ? MouseButton.Left : MouseButton.Right);
                    _view.InjectMouseUp(isLeft ? MouseButton.Left : MouseButton.Right);
                });
        }

        public void ClickElement(string id)
        {
            JSValue x =
                BrowserThread.ExecuteFunction(()=>
                _view.ExecuteJavascriptWithResult("findPosById('" + id + "');")
            );
            if(x.IsUndefined || !x.IsArray) throw new ArgumentException("Element not found.");
            var pos = (JSValue[]) x;
            MoveMouse((int)pos[0], (int)pos[1]);
            DoClick();
        }

        public void ClickElementPersistent(string id)
        {
            while (true)
            {
                try
                {
                    ClickElement(id);
                    return;
                }
                catch (ArgumentException)
                {
                    Thread.Sleep(100);
                }
            }
        }

        public void Type(string input)
        {
            var ev = new WebKeyboardEvent();
            foreach (char c in input)
            {
                ev.Text = c.ToString(CultureInfo.InvariantCulture);
                ev.KeyIdentifier = c.ToString(CultureInfo.InvariantCulture);
                BrowserThread.ExecuteAction(() =>
                    {

                        ev.Type = WebKeyboardEventType.KeyDown;
                        _view.InjectKeyboardEvent(ev);
                        ev.Type = WebKeyboardEventType.Char;
                        _view.InjectKeyboardEvent(ev);
                        ev.Type = WebKeyboardEventType.KeyUp;
                        _view.InjectKeyboardEvent(ev);
                    });
            }
        }

        protected virtual void InitializeJavascript(object sender, WebViewEventArgs e)
        {
            const string functions = @"
                function findPos(obj) {
                    var curleft = 0;
                    var curtop = 0;
                    if(obj.offsetLeft) curleft += parseInt(obj.offsetLeft);
                    if(obj.offsetTop) curtop += parseInt(obj.offsetTop);
                    if(obj.scrollTop && obj.scrollTop > 0) curtop -= parseInt(obj.scrollTop);
                    if(obj.offsetParent) {
                        var pos = findPos(obj.offsetParent);
                        curleft += pos[0];
                        curtop += pos[1];
                    } else if(obj.ownerDocument) {
                        var thewindow = obj.ownerDocument.defaultView;
                        if(!thewindow && obj.ownerDocument.parentWindow)
                            thewindow = obj.ownerDocument.parentWindow;
                        if(thewindow) {
                            if(thewindow.frameElement) {
                                var pos = findPos(thewindow.frameElement);
                                curleft += pos[0];
                                curtop += pos[1];
                            }
                        }
                    }

                    return [curleft,curtop];
                }

                function findPosById(id){
                    return findPos(document.getElementById(id));
                }


                function objToString(obj){
                    var ret='';
                    for(var prop in obj){
                        ret+= prop + '=\'' + obj[prop] + '\', ';
                    }
                    return ret;
                }

                function objArrayToString(arr){
                    var ret=[];
                    for(var i=0;i<arr.length;i++){
                        ret[i]=objToString(arr[i]);
                    }
                    return ret;
                }
";

            BrowserThread.ExecuteAction(() =>
            _view.ExecuteJavascript(functions)
            );

            //if (_view.GetLastError() != Error.None) throw new Exception(_view.GetLastError().ToString());
        }

        public JSValue ExecuteJavascriptWithResult(string script)
        {
            return
                BrowserThread.ExecuteFunction(() => 
                _view.ExecuteJavascriptWithResult(script)
            ); 
        }

        public void Dispose()
        {
            _view.Dispose();
        }
    }
}
