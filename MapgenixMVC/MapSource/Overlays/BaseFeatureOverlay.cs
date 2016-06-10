using System;
using System.Collections.ObjectModel;
using Mapgenix.FeatureSource;
using Mapgenix.Shapes;
using System.Collections.Generic;
using Mapgenix.Canvas;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public abstract class BaseFeatureOverlay : BaseOverlay
    {
        private EditTools _editTools;
        private QueryTools _queryTools;
        private InMemoryFeatureSource _featureSource;
        private FeatureOverlayStyle _style;

       
        protected BaseFeatureOverlay()
            : this(Guid.NewGuid().ToString(), false)
        { }

      
        protected BaseFeatureOverlay(string id)
            : this(id, false)
        { }

        
        protected BaseFeatureOverlay(string id, bool isBaseOverlay)
            : base(id, isBaseOverlay)
        {
            _featureSource = new InMemoryFeatureSource(new FeatureSourceColumn[] { }, new Feature[] { });
            _queryTools = new QueryTools(_featureSource);
            _editTools = new EditTools(_featureSource);
            _style = new FeatureOverlayStyle(new GeoColor(80, GeoColor.FromHtml("#0033ff")), new GeoColor(160, GeoColor.FromHtml("#0033ff")), 1);
        }

      
        public ICollection<Feature> Features
        {
            get
            {
                return _featureSource.InternalFeatures.Values;
            }
        }

      
        [JsonMember(MemberName = "features")]
        internal protected Collection<Feature> JsonFeatures
        {
            get
            {
                if (_featureSource.Projection != null && !_featureSource.Projection.IsOpen)
                {
                    _featureSource.Projection.Open();
                }

                _featureSource.Open();
                Collection<Feature> features = _featureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
                _featureSource.Close();

                if(features == null)
                {
                    features = new Collection<Feature>();
                }
                return features;
            }
        }

    
        public InMemoryFeatureSource FeatureSource
        {
            get { return _featureSource; }
        }

    
        public Collection<FeatureSourceColumn> Columns
        {
            get
            {
                Collection<FeatureSourceColumn> columns = FeatureSource.GetColumns();
                return columns;
            }
        }

      
        [JsonMember(MemberName = "style")]
        public FeatureOverlayStyle Style
        {
            get
            {
                return _style;
            }
            set
            {
                _style = value;
            }
        }

        public QueryTools QueryTools
        {
            get
            {
                return _queryTools;
            }
        }

      
        public EditTools EditTools
        {
            get
            {
                return _editTools;
            }
        }

        [JsonMember(MemberName = "otype")]
        protected override string OverlayType
        {
            get
            {
                return "VECTOR";
            }
        }
    }
}
