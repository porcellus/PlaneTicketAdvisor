using System;
using System.Globalization;
using Awesomium.Core;

namespace WebMiner
{
    public class BrowserSimulator
    {
        readonly WebView _view;
        public BrowserSimulator( string url = "")
        {
            _view = WebCore.CreateWebView(1024, 768);
            _view.DocumentReady += OnDocumentReady;
            _view.DocumentReady += InitializeJavascript;
            _view.AddressChanged += ViewAddressChanged;
            _view.Source = new Uri(url);
        }

        public Uri Source
        {
            get { return _view.Source; }
            set { _view.Source = value; }
        }

        public bool IsDocumentReady
        {
            get { return _view.IsDocumentReady; }
        }

        public ISurface Surface
        {
            get { return _view.Surface; }
        }

        public event UrlEventHandler AddressChanged;
        public event WebViewEventHandler DocumentReady;

        private void ViewAddressChanged(object sender, Awesomium.Core.UrlEventArgs ev)
        {
            if (AddressChanged != null)
                AddressChanged(this, ev);
        }

        private void OnDocumentReady(object sender, UrlEventArgs urlEventArgs)
        {
            if (DocumentReady != null)
                DocumentReady(this, urlEventArgs);
        }

        public void MoveMouse(int x, int y)
        {
            _view.InjectMouseMove(x, y);
        }

        public void DoClick(bool isLeft=true)
        {
            _view.InjectMouseDown(isLeft ? MouseButton.Left : MouseButton.Right);
            _view.InjectMouseUp(isLeft ? MouseButton.Left : MouseButton.Right);
        }

        public void ClickElement(string id)
        {
            JSValue[] x = _view.ExecuteJavascriptWithResult("findPosById('"+id+"');");
            MoveMouse((int)x[0], (int)x[1]);
            DoClick();
        }

        public void Type(string input)
        {
            var ev = new WebKeyboardEvent();
            foreach (char c in input)
            {
                ev.Text = c.ToString(CultureInfo.InvariantCulture);
                ev.KeyIdentifier = c.ToString(CultureInfo.InvariantCulture);
                ev.Type = WebKeyboardEventType.KeyDown;
                _view.InjectKeyboardEvent(ev);
                ev.Type = WebKeyboardEventType.Char;
                _view.InjectKeyboardEvent(ev);
                ev.Type = WebKeyboardEventType.KeyUp;
                _view.InjectKeyboardEvent(ev);
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
            _view.ExecuteJavascript(functions);

            if (_view.GetLastError() != Error.None) throw new Exception(_view.GetLastError().ToString());
        }

        public JSValue ExecuteJavascriptWithResult(string script)
        {
            return _view.ExecuteJavascriptWithResult(script);            
        }
    }
}
