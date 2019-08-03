using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace RectO
{
    public class LayerCanvas : Canvas
    {
        public ImageBrush image = new ImageBrush();

        private List<Visual> visuals = new List<Visual>();
        int completedVisualsCount = 0;

        public LayerCanvas()
        {
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.LowQuality);
            this.Background = image;
        }

        protected override int VisualChildrenCount
        {
            get { return visuals.Count; }
        }
        protected override Visual GetVisualChild(int index)
        {
            return visuals[index];
        }
        public int GetVisualCount()
        {
            return VisualChildrenCount;
        }
        public void SaveVisual()
        {
            completedVisualsCount++;
        }
        public void AddVisual(Visual vis)
        {
            visuals.Add(vis);
            base.AddVisualChild(vis);
            base.AddLogicalChild(vis);
        }
        public void DeleteVisual()
        {
            if (visuals.Count > completedVisualsCount)
            {
                base.RemoveVisualChild(visuals[visuals.Count - 1]);
                base.RemoveLogicalChild(visuals[visuals.Count - 1]);
                visuals.RemoveAt(visuals.Count - 1);
            }
        }
        public void ClearVisual()
        {
            completedVisualsCount = 0;
            while (visuals.Count != 0)
                DeleteVisual();
        }
    }
}
