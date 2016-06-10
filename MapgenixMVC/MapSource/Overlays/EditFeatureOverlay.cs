using System;

namespace Mapgenix.GSuite.Mvc
{

    [Serializable]
    public class EditFeatureOverlay : BaseFeatureOverlay
    {
        private TrackMode _trackMode;
        private EditSettings _editSettings;

        internal EditFeatureOverlay(string id)
            : base(id, false)
        {
            this.IsVisibleInOverlaySwitcher = false;
            _trackMode = TrackMode.None;
            _editSettings = new EditSettings();
            _editSettings.IsDraggable = true;
            _editSettings.IsReshapable = true;
            _editSettings.IsResizable = false;
            _editSettings.IsRotatable = true;
        }

       
        [JsonMember(MemberName = "mode")]
        public TrackMode TrackMode
        {
            get
            {
                return _trackMode;
            }
            set
            {
                _trackMode = value;
            }
        }

       
        [JsonMember(MemberName = "editSettings")]
        public EditSettings EditSettings
        {
            get { return _editSettings; }
        }
    }
}
