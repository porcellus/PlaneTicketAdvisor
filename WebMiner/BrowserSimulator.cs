using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebMiner
{
    public class BrowserSimulator
    {
        Awesomium.Core.WebView view_;
        public BrowserSimulator(Awesomium.Core.WebView b)
        {
            this.view_ = b;
            view_.DocumentReady += InitializeJavascript;
            view_.AddressChanged += ViewAddressChanged;
        }

        public Uri Source
        {
            get { return view_.Source; }
            set { view_.Source = value; }
        }

        public bool IsDocumentReady
        {
            get { return view_.IsDocumentReady; }
        }

        public Awesomium.Core.ISurface Surface
        {
            get { return view_.Surface; }
        }

        public event Awesomium.Core.UrlEventHandler AddressChanged;

        private void ViewAddressChanged(object sender, Awesomium.Core.UrlEventArgs ev)
        {
            if (AddressChanged != null)
                AddressChanged(this, ev);
        }

        public void MoveMouse(int x, int y)
        {
            view_.InjectMouseMove(x, y);
        }

        public void DoClick(bool isLeft=true)
        {
            view_.InjectMouseDown(isLeft ? Awesomium.Core.MouseButton.Left : Awesomium.Core.MouseButton.Right);
            view_.InjectMouseUp(isLeft ? Awesomium.Core.MouseButton.Left : Awesomium.Core.MouseButton.Right);
        }

        public void ClickElement(string id)
        {
            Awesomium.Core.JSValue[] x = view_.ExecuteJavascriptWithResult("findPosById('"+id+"');");
            this.MoveMouse((int)x[0], (int)x[1]);
            this.DoClick();
        }

        public void Type(string input)
        {
            var ev = new Awesomium.Core.WebKeyboardEvent();
            foreach (char c in input)
            {
                ev.Text = c.ToString();
                ev.KeyIdentifier = c.ToString();
                ev.Type = Awesomium.Core.WebKeyboardEventType.KeyDown;
                view_.InjectKeyboardEvent(ev);
                ev.Type = Awesomium.Core.WebKeyboardEventType.Char;
                view_.InjectKeyboardEvent(ev);
                ev.Type = Awesomium.Core.WebKeyboardEventType.KeyUp;
                view_.InjectKeyboardEvent(ev);
            }
        }

        public virtual void InitializeJavascript(object sender, Awesomium.Core.WebViewEventArgs e)
        {
            string functions = @"
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

                function getResultObject(x){
                    var obj = {
                        perprice: x.getElementsByClassName('per-price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
                        fullprice: x.getElementsByClassName('price')[0].getElementsByClassName('integer')[0].innerHTML.trim().replace(' ',''),
        
                        outCompany: x.getElementsByClassName('company')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                        outStartTime: x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                        outStartStation: x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
        
                        outArriveTime: x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                        outArriveStation: x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[0].innerHTML.trim(),
                    };
    
                    if(x.getElementsByClassName('company').length >= 2 && x.getElementsByClassName('company')[1].getElementsByClassName('content').length >= 2){
                        obj.backCompany = x.getElementsByClassName('company')[1].getElementsByClassName('content')[1].innerHTML.trim();
                        obj.backStartStation = x.getElementsByClassName('from-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                        obj.backStartTime = x.getElementsByClassName('from-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                        obj.backArriveStation = x.getElementsByClassName('to-station')[1].getElementsByClassName('content')[1].innerHTML.trim();
                        obj.backArriveTime = x.getElementsByClassName('to-time')[1].getElementsByClassName('content')[1].innerHTML.trim();
                    }
    
                    return obj;
                }

                function getResults(){
                    var retVal = [];
                    var elements=document.getElementsByClassName('resultitem');
                    for(var i=0;i<elements.length;i++){
                        retVal[i]=getResultObject(elements[i]);
                    }
                    return retVal;
                }
";
            view_.ExecuteJavascript(functions);

            if (view_.GetLastError() != Awesomium.Core.Error.None) throw new Exception(view_.GetLastError().ToString());
        }

        public List<Awesomium.Core.JSValue> stringToObjArray(string str){
            List<Awesomium.Core.JSValue> arr = new List<Awesomium.Core.JSValue>();
            return arr;
        }

        public Awesomium.Core.JSValue ExecuteJavascriptWithResult(string script)
        {
            return view_.ExecuteJavascriptWithResult(script);
        }

        public virtual List<Ticket> GetResults()
        {
            List<Ticket> retVal = new List<Ticket>();
            Awesomium.Core.JSValue[] res= view_.ExecuteJavascriptWithResult("objArrayToString(getResults());");

            foreach (string r in res)
                retVal.Add(WebMiner.TicketFromString(r));

            return retVal;
        }
    }
}
