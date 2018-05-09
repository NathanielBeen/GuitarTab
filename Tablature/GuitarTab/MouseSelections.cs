using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class MouseSelections
    {
        private MouseStateConverter converter;

        public const int CLICK = 0;
        public const int DRAG_RELEASE = 1;
        public const int MULTIPLE_DRAG_RELEASE = 2;
        public const int DRAG_SELECT = 3;

        public Selection SelectionState
        {
            get { return converter.SelectionState; }
            set { converter.SelectionState = value; }
        }

        public int EventType { get; set; }
        public bool EventHandled { get; set; }
        public bool PositionCheck
        {
            get;
            set;
        }

        private List<ModelBoundsPair> selected_objects;

        public Rect SelectedRectangle { get; set; }
        public Point SelectedPoint { get; set; }

        public event EventHandler<SelectedObjectChangedArgs> SelectedChanged;

        public MouseSelections(MouseStateConverter conv)
        {
            converter = conv;
            EventType = CLICK;
            EventHandled = true;
            PositionCheck = false;

            selected_objects = new List<ModelBoundsPair>();

            SelectedRectangle = default(Rect);
            SelectedPoint = default(Point);
        }

        public void addSelectedObject(ModelBoundsPair selected)
        {
            selected_objects.Add(selected);
            SelectedChanged?.Invoke(this, new SelectedObjectChangedArgs(selected_objects));
        }

        public void setToSingleSelectedObject(ModelBoundsPair selected)
        {
            selected_objects.Clear();
            selected_objects.Add(selected);
            SelectedChanged?.Invoke(this, new SelectedObjectChangedArgs(selected_objects));
        }

        public ModelBoundsPair getFirstSelectedObject()
        {
            return selected_objects.FirstOrDefault();
        }

        public List<ModelBoundsPair> getAllSelectedObjects()
        {
            return selected_objects;
        }

        public bool checkSelectionState(Selection wanted)
        {
            return (SelectionState == wanted && !EventHandled);
        }

        public void clearSelectedObjects()
        {
            selected_objects.Clear();
            SelectedChanged?.Invoke(this, new SelectedObjectChangedArgs(selected_objects));
        }

        public void clearOtherSelections()
        {
            SelectionState = Selection.Standard;
            EventType = CLICK;
            SelectedRectangle = default(Rect);
            SelectedPoint = default(Point);
        }

        public void clearAllSelections()
        {
            clearSelectedObjects();
            clearOtherSelections();
        }
    }

    public enum Selection
    {
        Standard,
        Add_Effect,
        Add_Multi_Effect,
        Add_Note,
        Add_Rest,
        Add_Measure,
        Set_Length
    }

    public static class SelectionExtensions
    {
        public static AddItem convertToAddItem(this Selection selection)
        {
            switch (selection)
            {
                case Selection.Standard:
                    return AddItem.None;
                case Selection.Add_Effect:
                    return AddItem.PalmMute;
                case Selection.Add_Multi_Effect:
                    return AddItem.Slide;
                case Selection.Add_Note:
                    return AddItem.Note;
                case Selection.Add_Rest:
                    return AddItem.Rest;
                case Selection.Add_Measure:
                    return AddItem.Measure;
                case Selection.Set_Length:
                    return AddItem.None;
                default:
                    return AddItem.None;
            }
        }
    }
}
