using System;
using System.Web.UI;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web.UI.Sheer;

namespace Sitecore.ContentSearch.Spatial.DataTypes.CustomControls
{
    public class GeoLocationControl : Shell.Applications.ContentEditor.Text, IContentField
    {
        public readonly string MapUrl = "/sitecore/shell/applications/geolocation/map.aspx";


        public GeoLocationControl()
        {
            Class = "scContentControl";
            Activation = true;
        }

        public string Source
        {
            get { return GetViewStateString("Source"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                var str = MainUtil.UnmapPath(value);
                if (str.EndsWith("/", StringComparison.InvariantCulture))
                {
                    str = str.Substring(0, str.Length - 1);
                }
                SetViewStateString("Source", str);
            }
        }

        public string ItemVersion
        {
            get { return GetViewStateString("Version"); }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                SetViewStateString("Version", value);
            }
        }

        public string GetValue()
        {
            return Value;
        }

        public void SetValue(string value)
        {
            Value = value;
        }


        public override void HandleMessage(Message message)
        {
            base.HandleMessage(message);
            if (message["id"] != ID)
            {
                return;
            }
            switch (message.Name)
            {
                case "geolocation:refresh":
                    RefreshMap();
                    break;
            }
        }

        private void RefreshMap()
        {
            var src = GetMapUrl();
            SheerResponse.SetAttribute(ID + "_map", "src", src);
            SheerResponse.Eval("scContent.startValidators()");
        }

        protected string GetMapUrl()
        {
            var latlon = Value.Split(',');
            return latlon.Length == 2
                ? $"{MapUrl}?lat={latlon[0]}&lon={latlon[1]}&ctrlid={ID}"
                : $"{MapUrl}?ctrlid={ID}";
        }

        protected override void DoRender(HtmlTextWriter output)
        {
            base.DoRender(output);
            Assert.ArgumentNotNull(output, "output");
            var src = " src=\"" + GetMapUrl()
                      + "\"";
            var iframeId = " id=\"" + ID + "_map\"";
            output.Write("<div id=\"" + ID + "_pane\" class=\"scContentControlImagePane\" style=\"width:503px\">");
            var clientEvent = Sitecore.Context.ClientPage.GetClientEvent(ID + ".Browse");
            output.Write("<div class=\"scContentControlImageImage\" onclick=\"" + clientEvent + "\">");
            output.Write("<iframe" + iframeId + src +
                         " frameborder=\"0\" marginwidth=\"0\" marginheight=\"0\" width=\"100%\" height=\"380px\" allowtransparency=\"allowtransparency\"></iframe>");
            output.Write("</div>");
            output.Write("</div>");
        }
    }
}