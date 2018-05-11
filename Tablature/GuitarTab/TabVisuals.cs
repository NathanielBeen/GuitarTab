﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace GuitarTab
{
    public class TabVisuals : FrameworkElement
    {
        private VisualCollection children;
        private ObservableCollection<TabDrawingVisual> collection;

        public TabVisuals()
            :base()
        {
            children = new VisualCollection(this);
        }

        public void setVisualCollection(ObservableCollection<TabDrawingVisual> new_collection)
        {
            subscribeNewCollection(collection, new_collection);
            collection = new_collection;
            addVisuals(collection);
        }

        public void subscribeNewCollection(ObservableCollection<TabDrawingVisual> old_collection, ObservableCollection<TabDrawingVisual> new_collection)
        {
            if (old_collection != null) { old_collection.CollectionChanged -= handleCollectionChanged; }
            if (new_collection != null) { new_collection.CollectionChanged += handleCollectionChanged; }
        }

        public void handleCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null) { removeVisuals(args.OldItems); }
            if (args.NewItems != null) { addVisuals(args.NewItems); }
        }

        public void removeVisuals(IList items)
        {
            foreach (DrawingVisual visual in items)
            {
                children.Remove(visual);
            }
            InvalidateVisual();
        }

        public void addVisuals(IList items)
        {
            foreach (DrawingVisual visual in items)
            {
                children.Add(visual);
            }
            InvalidateVisual();
        }

        protected override int VisualChildrenCount { get { return children.Count; } }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index > children.Count) { return null; }
            return children[index];
        }
    }

    public static class VisualDrawingExtensions
    {
        public static void DrawArc(this DrawingContext dc, Pen pen, Brush brush, Rect rect, double startDegrees, double sweepDegrees)
        {
            GeometryDrawing arc = CreateArcDrawing(rect, startDegrees, sweepDegrees);
            dc.DrawGeometry(brush, pen, arc.Geometry);
        }

        private static GeometryDrawing CreateArcDrawing(Rect rect, double startDegrees, double sweepDegrees)
        {
            // degrees to radians conversion
            double startRadians = startDegrees * Math.PI / 180.0;
            double sweepRadians = sweepDegrees * Math.PI / 180.0;

            // x and y radius
            double dx = rect.Width / 2;
            double dy = rect.Height / 2;

            // determine the start point 
            double xs = rect.X + dx + (Math.Cos(startRadians) * dx);
            double ys = rect.Y + dy + (Math.Sin(startRadians) * dy);

            // determine the end point 
            double xe = rect.X + dx + (Math.Cos(startRadians + sweepRadians) * dx);
            double ye = rect.Y + dy + (Math.Sin(startRadians + sweepRadians) * dy);

            // draw the arc into a stream geometry
            StreamGeometry streamGeom = new StreamGeometry();
            using (StreamGeometryContext ctx = streamGeom.Open())
            {
                bool isLargeArc = Math.Abs(sweepDegrees) > 180;
                SweepDirection sweepDirection = sweepDegrees < 0 ? SweepDirection.Counterclockwise : SweepDirection.Clockwise;

                ctx.BeginFigure(new Point(xs, ys), false, false);
                ctx.ArcTo(new Point(xe, ye), new Size(dx, dy), 0, isLargeArc, sweepDirection, true, false);
            }

            // create the drawing
            GeometryDrawing drawing = new GeometryDrawing();
            drawing.Geometry = streamGeom;
            return drawing;
        }
    }
}
