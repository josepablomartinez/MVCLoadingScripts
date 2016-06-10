using Mapgenix.Canvas;
using Mapgenix.Layers;
using Mapgenix.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;

namespace Mapgenix.GSuite.Mvc
{
    public class Map : BaseControl
    {
        private const string SynchronizeDefaultInfoHiddenFieldName = "__RestoreStatusField";
        private const string SynchronizeVectorsHiddenFieldName = "__RestoreVectors";
        private const string BaseLayerChangedStatusField = "__BASELAYERCHANGEDSTATUSSTORAGE";
        private const string SynchronizeSimpleMarkerOverlayFieldName = "__SimpleMarkerOverlayField";

        private RectangleShape _currentExtent;
        private double _currentScale;
        private HighlightFeatureOverlay _highlightOverlay;
        private GeographyUnit _mapUnit;
        private BaseOverlay _activeBaseOverlay;
        private string _lastBaseLayerName;
        private BackgroundLayer _mapBackground;
        private BaseOverlay _backgroundOverlay;
        private EditFeatureOverlay _editOverlay;
        private LayerOverlay _staticOverlay;
        private LayerOverlay _dynamicOverlay;
        private InMemoryMarkerOverlay _markerOverlay;
        private GeoKeyedCollection<BaseOverlay> _customOverlays;
        private GeoKeyedCollection<BasePopup> _popups;
        private ContextMenu _contextMenu;
        private Collection<double> _clientZoomLevelScales;

        private double _widthInPixels;
        private double _heightInPixels;

        private MapTools _mapTools;

        public int Width
        {
            set { Attributes.Merge("width", value.ToString()); }
            internal get 
            {
                int number;
                bool result = Int32.TryParse(Attributes.GetValue("width"), out number);
                if (result)
                {
                    return number;
                }
                return 0;
            }
        }

        public int Height
        {
            set { Attributes.Merge("height", value.ToString()); }
            internal get
            {
                int number;
                bool result = Int32.TryParse(Attributes.GetValue("height"), out number);
                if (result)
                {
                    return number;
                }
                return 0;
            }
        }

        /// <summary>Initialize a new instance of the Map class.</summary>
        /// <overloads>Initialize a new instance of the Map class.</overloads>
        public Map(object htmlAttributes = null)
            : this(Guid.NewGuid().ToString(), 640, 480, htmlAttributes) { }

        /// <summary>
        /// Initialize a new instance of the Map class with identifier, map width, and map height
        /// specified.
        /// </summary>
        /// <overloads>Initialize a new instance of the Map class.</overloads>
        public Map(string id, int width = 640, int height = 480, object htmlAttributes = null)
            : base("div", TagRenderMode.SelfClosing)
        {
            //AssociatedControlID = associatedControlID;
            //HtmlAttributes = htmlAttributes;

            //ID = associatedControlID;
            //Width = width;
            //Height = height;
            //_widthInPixels = double.NaN;
            //_heightInPixels = double.NaN;
            //_currentScale = double.NaN;

            //_clientZoomLevelScales = new Collection<double>();

            ID = id;
            Width = width;
            Height = height;
            _widthInPixels = double.NaN;
            _heightInPixels = double.NaN;
            _currentScale = double.NaN;
            Zoom = 0;

            _clientZoomLevelScales = new Collection<double>();
            SyncClientZoomLevels(new ZoomLevelSet());
        }

        #region Properties

        public Collection<double> ClientZoomLevelScales
        {
            get { return _clientZoomLevelScales; }
        }

        [JsonMember(MemberName = "resolutions")]
        protected Collection<double> ClientResolutions
        {
            get
            {
                Collection<double> resolutions = new Collection<double>();
                foreach (double scale in _clientZoomLevelScales)
                {
                    resolutions.Add(MapUtilities.GetResolutionFromScale(scale, _mapUnit));
                }
                return resolutions;
            }
        }


        [Browsable(false)]
        public double WidthInPixels
        {
            get
            {
                //Validators.CheckMapActualWidthAndHeightValid(_widthInPixels);
                return _widthInPixels;
            }
        }


        [Browsable(false)]
        public double HeightInPixels
        {
            get
            {
                //Validators.CheckMapActualWidthAndHeightValid(_heightInPixels);
                return _heightInPixels;
            }
        }

        //[JsonMember(MemberName = "cid")]
        //public override string ClientID
        //{
        //    get
        //    {
        //        //return base.ClientID;
        //        return base.ID;
        //    }
        //}


        //[JsonMember(MemberName = "uid")]
        //public override string UniqueID
        //{
        //    get
        //    {
        //        //return base.UniqueID;
        //        return base.ID;
        //    }
        //}


        [Browsable(false)]
        public BackgroundLayer MapBackground
        {
            get
            {
                if (_mapBackground == null)
                {
                    _mapBackground = new BackgroundLayer();
                }
                return _mapBackground;
            }
            set
            {
                _mapBackground = value;
            }
        }


        [JsonMember(MemberName = "contextMenu")]
        [Browsable(false)]
        public ContextMenu ContextMenu
        {
            get { return _contextMenu; }
            set { _contextMenu = value; }
        }

        [JsonMember(MemberName = "currentExtent")]
        [Browsable(false)]
        public RectangleShape CurrentExtent
        {
            get
            {
                return _currentExtent;
            }
            set
            {
                PointShape center = value.GetCenterPoint();
                CenterX = center.X;
                CenterY = center.Y;
                Zoom = -1;
                _currentExtent = value;
            }
        }


        //[JsonMember(MemberName = "cursor")]
        //public CursorType Cursor
        //{
        //    get
        //    {
        //        object stateObject = ViewState["Cursor"];
        //        if (stateObject != null)
        //        {
        //            return (CursorType)stateObject;
        //        }
        //        else
        //        {
        //            ViewState["Cursor"] = CursorType.Default;
        //            return CursorType.Default;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["Cursor"] = value;
        //    }
        //}


        //public WebConfigRegistrationMode WebConfigRegisterMode
        //{
        //    get
        //    {
        //        object stateObject = ViewState["webConfigRegistable"];
        //        if (stateObject != null)
        //        {
        //            return (WebConfigRegistrationMode)stateObject;
        //        }
        //        else
        //        {
        //            ViewState["webConfigRegistable"] = WebConfigRegistrationMode.Automatic;
        //            return WebConfigRegistrationMode.Automatic;
        //        }
        //    }
        //    set { ViewState["webConfigRegistable"] = value; }
        //}


        //public ResourceDeploymentMode ResourceDeploymentMode
        //{
        //    get
        //    {
        //        object stateObject = ViewState["allowResoucesCopied"];
        //        if (stateObject != null)
        //        {
        //            return (ResourceDeploymentMode)stateObject;
        //        }
        //        else
        //        {
        //            ViewState["allowResoucesCopied"] = ResourceDeploymentMode.Automatic;
        //            return ResourceDeploymentMode;
        //        }
        //    }
        //    set { ViewState["allowResoucesCopied"] = value; }
        //}


        [JsonMember(MemberName = "baselayerid", ValuePropertyName = "Id")]
        [Browsable(false)]
        public BaseOverlay ActiveBaseOverlay
        {
            get
            {
                if (_activeBaseOverlay == null)
                {
                    if (_backgroundOverlay != null)
                    {
                        _activeBaseOverlay = _backgroundOverlay;
                    }
                    else if (_staticOverlay != null)
                    {
                        _activeBaseOverlay = _staticOverlay;
                    }
                    else if (_customOverlays != null && _customOverlays.Count > 0)
                    {
                        foreach (BaseOverlay overlay in _customOverlays)
                        {
                            if (overlay.IsBaseOverlay)
                            {
                                _activeBaseOverlay = overlay;
                                break;
                            }
                        }
                    }
                }
                return _activeBaseOverlay;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "ActiveBaseOverlay");
                BaseOverlay targetOverlay = (BaseOverlay)value;
                if (targetOverlay.IsBaseOverlay && (object.ReferenceEquals(targetOverlay, _backgroundOverlay) || object.ReferenceEquals(targetOverlay, _staticOverlay) || (_customOverlays != null && _customOverlays.Contains(targetOverlay))))
                {
                    _activeBaseOverlay = targetOverlay;
                }
                else
                {
                    throw new ArgumentException("Invalid base overlay object.Please check whether the overlay is base overlay and is added to the map.", "value");
                }
            }
        }


        [JsonMember(MemberName = "backgroundOverlay")]
        [Browsable(false)]
        public BaseOverlay BackgroundOverlay
        {
            get
            {
                return _backgroundOverlay;
            }
            set
            {
                //Validators.CheckParameterIsNotNull(value, "BackgroundOverlay");
                BaseOverlay targetOverlay = (BaseOverlay)value;
                if (targetOverlay.IsBaseOverlay)
                {
                    _backgroundOverlay = targetOverlay;
                }
                else
                {
                    throw new ArgumentException("Invalid background overlay object.Please check whether the overlay is base overlay.", "value");
                }
            }
        }


        [Browsable(false)]
        public LayerOverlay StaticOverlay
        {
            get
            {
                if (_staticOverlay == null)
                {
                    _staticOverlay = new LayerOverlay("StaticOverlay", true, TileType.MultipleTile);
                }
                return _staticOverlay;
            }
        }


        [JsonMember(MemberName = "staticOverlay")]
        protected LayerOverlay StaticOverlayForJson
        {
            get
            {
                return _staticOverlay;
            }
        }


        [Browsable(false)]
        public LayerOverlay DynamicOverlay
        {
            get
            {
                if (_dynamicOverlay == null)
                {
                    _dynamicOverlay = new LayerOverlay("DynamicOverlay", false, TileType.SingleTile);
                    _dynamicOverlay.TransitionEffect = TransitionEffect.None;
                }
                return _dynamicOverlay;
            }
        }


        [JsonMember(MemberName = "dynamicOverlay")]
        protected LayerOverlay DynamicOverlayForJson
        {
            get
            {
                return _dynamicOverlay;
            }
        }


        [Browsable(false)]
        public InMemoryMarkerOverlay MarkerOverlay
        {
            get
            {
                if (_markerOverlay == null)
                {
                    _markerOverlay = new InMemoryMarkerOverlay("MarkerOverlay");
                }
                return _markerOverlay;
            }
        }


        [JsonMember(MemberName = "markerOverlay")]
        protected InMemoryMarkerOverlay MarkerOverlayForJson
        {
            get
            {
                return _markerOverlay;
            }
        }


        [JsonMember(MemberName = "layers")]
        [Browsable(false)]
        public GeoKeyedCollection<BaseOverlay> CustomOverlays
        {
            get
            {
                if (_customOverlays == null)
                {
                    _customOverlays = new GeoKeyedCollection<BaseOverlay>();
                }
                return _customOverlays;
            }
        }


        [JsonMember(MemberName = "editOverlay")]
        [Browsable(false)]
        public EditFeatureOverlay EditOverlay
        {
            get
            {
                if (_editOverlay == null)
                {
                    _editOverlay = new EditFeatureOverlay("EditOverlay");
                }
                return _editOverlay;
            }
        }


        [JsonMember(MemberName = "highlightOverlay")]
        [Browsable(false)]
        public HighlightFeatureOverlay HighlightOverlay
        {
            get
            {
                if (_highlightOverlay == null)
                {
                    _highlightOverlay = new HighlightFeatureOverlay("HighlightOverlay");
                }
                return _highlightOverlay;
            }
        }


        [JsonMember(MemberName = "controls")]
        [Browsable(false)]
        public MapTools MapTools
        {
            get
            {
                if (_mapTools != null)
                {
                    return _mapTools;
                }
                else
                {
                    _mapTools = new MapTools();
                    return _mapTools;
                }
            }
            protected set
            {
                _mapTools = value;
            }
        }


        [JsonMember(MemberName = "units")]
        public GeographyUnit MapUnit
        {
            get
            {
                return _mapUnit;
            }
            set
            {
                //Validators.CheckMapUnitIsValid(value);
                _mapUnit = value;
            }
        }



        //[JsonMember(MemberName = "onClientDrawEnd")]
        //public string OnClientDrawEnd
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientDrawEnd"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return string.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientDrawEnd"] = value;
        //    }
        //}


        //[JsonMember(MemberName = "onClientEditEnd")]
        //public string OnClientEditEnd
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientEditEnd"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return String.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientEditEnd"] = value;
        //    }
        //}


        //[JsonMember(MemberName = "onClientClick")]
        //public string OnClientClick
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientClick"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return String.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientClick"] = value;
        //    }
        //}


        //[JsonMember(MemberName = "onClientExtentChanged")]
        //public string OnClientExtentChanged
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientExtentChanged"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return String.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientExtentChanged"] = value;
        //    }
        //}


        //[JsonMember(MemberName = "onClientBaseOverlayChanged")]
        //public string OnClientBaseOverlayChanged
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientBaseOverlayChanged"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return String.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientBaseOverlayChanged"] = value;
        //    }
        //}


        //[JsonMember(MemberName = "onClientDoubleClick")]
        //public string OnClientDoubleClick
        //{
        //    get
        //    {
        //        object stateObject = ViewState["OnClientDoubleClick"];
        //        if (stateObject != null)
        //        {
        //            return (string)stateObject;
        //        }
        //        else
        //        {
        //            return String.Empty;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["OnClientDoubleClick"] = value;
        //    }
        //}


        [JsonMember(MemberName = "popups")]
        [Browsable(false)]
        public GeoKeyedCollection<BasePopup> Popups
        {
            get
            {
                if (_popups == null)
                {
                    _popups = new GeoKeyedCollection<BasePopup>();
                }
                return _popups;
            }
        }


        //[JsonMember(MemberName = "restrictedExtent")]
        //[Browsable(false)]
        //public RectangleShape RestrictedExtent
        //{
        //    get
        //    {
        //        object stateObject = ViewState["RestrictedExtent"];
        //        if (stateObject != null)
        //        {
        //            return (RectangleShape)stateObject;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["RestrictedExtent"] = value;
        //    }
        //}

        public double CurrentScale
        {
            get
            {
                return _currentScale;
            }
            set
            {
                Zoom = MapUtilities.GetZoomFromScale(value, ClientZoomLevelScales);
                _currentScale = value;
            }
        }


        //[JsonMember(MemberName = "cursorUrl")]
        //public Uri CustomCursorUri
        //{
        //    get
        //    {
        //        Uri customCursorUri = ViewState["CustomCuror"] as Uri;
        //        if (customCursorUri != null)
        //        {
        //            return customCursorUri;
        //        }
        //        else
        //        {
        //            return null;
        //        }
        //    }
        //    set
        //    {
        //        ViewState["CustomCuror"] = value;
        //    }
        //}


        //public bool IsDefaultJavascriptLibraryDisabled
        //{
        //    get { return ViewState["IsDefaultJavascriptLibraryDisabled"] == null ? false : ((bool)ViewState["IsDefaultJavascriptLibraryDisabled"]); }
        //    set { ViewState["IsDefaultJavascriptLibraryDisabled"] = value; }
        //}


        //public bool IsDomTreeCollectedWhenPostback
        //{
        //    get
        //    {
        //        if (ViewState["IsDomTreeCollectedWhenPostback"] == null)
        //        {
        //            return false;
        //        }
        //        else
        //        {
        //            return (bool)ViewState["IsDomTreeCollectedWhenPostback"];
        //        }
        //    }
        //    set { ViewState["IsDomTreeCollectedWhenPostback"] = value; }
        //}


        //[JsonMember(MemberName = "pageName")]
        //protected string PageName
        //{
        //    get
        //    {
        //        string pageName = Page.Request.FilePath;
        //        pageName = pageName.Substring(pageName.LastIndexOf('/') + 1);

        //        if (pageName.IndexOf('.') != -1)
        //            pageName = pageName.Remove(pageName.IndexOf('.'));

        //        return pageName;
        //    }
        //}


        //[JsonMember(MemberName = "click")]
        //protected bool HasClickEvent
        //{
        //    get
        //    {
        //        EventHandler<MapClickedEventArgs> handler = Events[EventClick] as EventHandler<MapClickedEventArgs>;
        //        return (handler != null);
        //    }
        //}


        //[JsonMember(MemberName = "dblClick")]
        //protected bool HasDoubleClickEvent
        //{
        //    get
        //    {
        //        EventHandler<MapClickedEventArgs> handler = Events[EventDoubleClick] as EventHandler<MapClickedEventArgs>;
        //        return (handler != null);
        //    }
        //}


        //[JsonMember(MemberName = "baseOverlayChanged")]
        //protected bool HasBaseLayerChangedEvent
        //{
        //    get
        //    {
        //        EventHandler<BaseOverlayChangedEventArgs> handler = (EventHandler<BaseOverlayChangedEventArgs>)Events[EventBaseOverlayChanged];
        //        return (handler != null);
        //    }
        //}


        //[JsonMember(MemberName = "hasTrackShapeEnd")]
        //protected bool HasTrackShapeEndEvent
        //{
        //    get
        //    {
        //        EventHandler handler = Events[EventTrackShapeFinished] as EventHandler;
        //        return (handler != null);
        //    }
        //}


        //[JsonMember(MemberName = "extentChanged")]
        //protected bool HasExtentChangedEvent
        //{
        //    get
        //    {
        //        EventHandler<ExtentChangedEventArgs> handler = Events[EventExtentChanged] as EventHandler<ExtentChangedEventArgs>;
        //        return (handler != null);
        //    }
        //}


        [JsonMember(MemberName = "centerX")]
        protected double CenterX { get; set; }


        [JsonMember(MemberName = "centerY")]
        protected double CenterY { get; set; }


        [JsonMember(MemberName = "zoom")]
        private int Zoom { get; set; }


        //[JsonMember(MemberName = "rootPath")]
        //protected string PageRootPath
        //{
        //    get
        //    {
        //        string applicationPath = Page.Request.ApplicationPath;
        //        if (!applicationPath.EndsWith("/", StringComparison.Ordinal))
        //        {
        //            applicationPath = String.Concat(applicationPath, "/");
        //        }
        //        return applicationPath;
        //    }
        //}

        private double MapWidth
        {
            get
            {
                double mapWidth = _widthInPixels;
                if (Double.IsNaN(mapWidth))
                {
                    //mapWidth = Width.Value;
                    mapWidth = Convert.ToDouble(Width);
                }
                return mapWidth;
            }
        }

        private double MapHeight
        {
            get
            {
                double mapHeight = _heightInPixels;
                if (Double.IsNaN(mapHeight))
                {
                    //mapHeight = Height.Value;
                    mapHeight = Convert.ToDouble(Height);
                }
                return mapHeight;
            }
        }

        #endregion Properties

        public void RenderControl(HtmlTextWriter writer)
        {
            RenderHtml(writer);
        }


        protected override void RenderHtml(HtmlTextWriter writer)
        {
            //RegisterThirdPartLibrary();
            //RegisterJavascriptLibrary();

            TagBuilder wrapper = new TagBuilder("table");
            //merge attributes
            wrapper.MergeAttributes(HtmlAttributes != null ? HtmlHelper.AnonymousObjectToHtmlAttributes(HtmlAttributes) : null);

            wrapper.MergeAttribute("id", String.Format(CultureInfo.InvariantCulture, "{0}Container", ID));
            wrapper.MergeAttribute("cellpadding", "0");
            wrapper.MergeAttribute("cellspacing", "0");
            wrapper.MergeAttribute("border", "0");
            wrapper.Attributes.Add("style", string.Format("width:{0}px;height:{1}px;", Width, Height));

            TagBuilder tbody = new TagBuilder("tbody");
            TagBuilder row = new TagBuilder("tr");

            TagBuilder col = new TagBuilder("td");
            col.Attributes.Add("style",
                String.Format(CultureInfo.InvariantCulture,
                    "text-align:left;background-image:url(bg_GeoResource.axd?ClientID={0}&PageName={1}&Fresh={2}&Format=image/png&Quality=100);margin:0px;height:100%;overflow:visible;",
                    ID,
                    "Demo", //TODO: Property
                    DateTime.Now.ToString("mmssms", CultureInfo.InvariantCulture)
                    ));


            //writer.WriteBeginTag("table");
            //writer.WriteAttribute("id", String.Format(CultureInfo.InvariantCulture, "{0}Container", ClientID));
            //writer.WriteAttribute("cellpadding", "0");
            //writer.WriteAttribute("cellspacing", "0");
            //writer.WriteAttribute("border", "0");
            //writer.Write(" style=\"");
            //writer.WriteStyleAttribute("width", Width.ToString());
            //writer.WriteStyleAttribute("height", Height.ToString());
            //writer.Write("\">");

            //writer.WriteFullBeginTag("tr");
            //writer.WriteBeginTag("td");
            //writer.WriteAttribute("style",
            //    String.Format(CultureInfo.InvariantCulture,
            //        "text-align:left;background-image:url(bg_GeoResource.axd?ClientID={0}&PageName={1}&Fresh={2}&Format=image/png&Quality=100);margin:0px;height:100%;overflow:visible;",
            //        ClientID,
            //        PageName,
            //        DateTime.Now.ToString("mmssms", CultureInfo.InvariantCulture)
            //        ));

            //writer.Write(">");

            //base.Render(writer);

            //writer.WriteEndTag("td");
            //writer.WriteEndTag("tr");
            //writer.WriteEndTag("table");

            row.InnerHtml += col.ToString();
            tbody.InnerHtml += row.ToString();
            wrapper.InnerHtml += tbody.ToString();

            writer.Write(wrapper.ToString());
        }

        //private void RegisterJavascriptLibrary()
        //{
        //    //if (!IsDefaultJavascriptLibraryDisabled)
        //    //{
        //        RegisterJavascriptLibraryCore();
        //    //}
        //}


        //protected virtual void RegisterJavascriptLibraryCore()
        //{
        //    RegisterScriptReference("opl");
        //    RegisterScriptReference("extension");
        //    RegisterScriptReference("helper");
        //    RegisterScriptReference("parser");
        //    RegisterScriptReference("func");
        //    RegisterScriptReference("cm");
        //}

        //private void RegisterThirdPartLibrary()
        //{
        //    if (_backgroundOverlay != null)
        //    {
        //        _backgroundOverlay.RegisterJavaScriptLibrary(Page);
        //    }

        //    if (_customOverlays != null)
        //    {
        //        foreach (BaseOverlay overlay in _customOverlays)
        //        {
        //            overlay.RegisterJavaScriptLibrary(Page);
        //        }
        //    }
        //}

        //private void RegisterScriptReference(string referenceKey)
        //{
        //    string scriptUrl = String.Format(CultureInfo.InvariantCulture, "{0}_GeoResource.axd", referenceKey);
        //    RegisterScriptReference(referenceKey, scriptUrl);

        //    if (_viewContext == null)
        //    {
        //        throw new ArgumentNullException("viewContext");
        //    }

        //    //If script list does not already exist then initialise
        //    if (!_viewContext.HttpContext.Items.Contains("client-script-list"))
        //        _viewContext.HttpContext.Items["client-script-list"] = new Dictionary<string, KeyValuePair<string, string[]>>();

        //    var scripts = _viewContext.HttpContext.Items["client-script-list"] as Dictionary<string, KeyValuePair<string, string[]>>;

        //    //Ensure scripts are not added twice
        //    var scriptFilePath = _viewContext.HttpContext.Server.MapPath(DetermineScriptToRender(_viewContext, scriptPath));
        //    if (!scripts.ContainsKey(scriptFilePath))
        //        scripts.Add(scriptFilePath, new KeyValuePair<string, string[]>(scriptPath, dependancies));
        //}

        //private void RegisterScriptReference(string referenceKey, string scriptUrl)
        //{
        //    if (!Page.ClientScript.IsClientScriptBlockRegistered(Page.GetType(), referenceKey))
        //    {
        //        string scriptReference = String.Format(CultureInfo.InvariantCulture, "<script type='text/javascript' src='{0}'></script>", scriptUrl);
        //        Page.ClientScript.RegisterClientScriptBlock(Page.GetType(), referenceKey, scriptReference);
        //    }
        //}

        private static string DetermineScriptToRender(ViewContext viewContext, string url, string minifiedSuffix = "min.js")
        {
            //Initialise a list that will contain potential scripts to render
            var possibleScriptsToRender = new List<string>() { url };

#if DEBUG
            //Don't add minified scripts in debug mode
#else
            //Add minified path of script to list
            possibleScriptsToRender.Insert(0, Path.ChangeExtension(url, minifiedSuffix));
#endif

            var validScriptsToRender = possibleScriptsToRender.Where(s => File.Exists(viewContext.HttpContext.Server.MapPath(s)));
            if (!validScriptsToRender.Any())
                throw new FileNotFoundException("Unable to render " + url + " as none of the following scripts exist:" +
                    string.Join(Environment.NewLine, possibleScriptsToRender));
            else
                return validScriptsToRender.First(); //Return first existing file in list (minified file in release mode)
        }

        public void SyncClientZoomLevels(IEnumerable<ZoomLevel> zoomLevels)
        {
            _clientZoomLevelScales.Clear();
            foreach (ZoomLevel level in zoomLevels)
            {
                if (level.Scale != 0)
                {
                    _clientZoomLevelScales.Add(level.Scale);
                }
                else
                {
                    break;
                }
            }
        }

        public void SyncClientZoomLevels(ZoomLevelSet zoomLevelSet)
        {
            SyncClientZoomLevels(zoomLevelSet.GetZoomLevels());
        }
    }
}
